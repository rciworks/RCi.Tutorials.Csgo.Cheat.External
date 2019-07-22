using System.Threading;
using RCi.Tutorials.Csgo.Cheat.External.Data;
using RCi.Tutorials.Csgo.Cheat.External.Data.Internal;
using RCi.Tutorials.Csgo.Cheat.External.Gfx.Math;
using RCi.Tutorials.Csgo.Cheat.External.Sys.Data;
using RCi.Tutorials.Csgo.Cheat.External.Utils;

namespace RCi.Tutorials.Csgo.Cheat.External.Features
{
    /// <summary>
    /// Trigger bot. Shoots when hovering over an enemy.
    /// </summary>
    public class TriggerBot :
        ThreadedComponent
    {
        #region // storage

        /// <inheritdoc />
        protected override string ThreadName => nameof(TriggerBot);

        /// <inheritdoc cref="GameProcess"/>
        private GameProcess GameProcess { get; set; }

        /// <inheritdoc cref="GameData"/>
        private GameData GameData { get; set; }

        #endregion

        #region // ctor

        /// <summary />
        public TriggerBot(GameProcess gameProcess, GameData gameData)
        {
            GameProcess = gameProcess;
            GameData = gameData;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();

            GameData = default;
            GameProcess = default;
        }

        #endregion

        #region // routines

        /// <summary>
        /// Is trigger bot hot key down?
        /// </summary>
        private bool IsHotKeyDown()
        {
            return WindowsVirtualKey.VK_MBUTTON.IsKeyDown();
        }

        /// <inheritdoc />
        protected override void FrameAction()
        {
            if (!GameProcess.IsValid || !IsHotKeyDown())
            {
                return;
            }

            // get aim ray in world
            var player = GameData.Player;
            if (player.AimDirection.Length() < 0.001)
            {
                return;
            }
            var aimRayWorld = new Line3D(player.EyePosition, player.EyePosition + player.AimDirection * 8192);

            // go through entities
            foreach (var entity in GameData.Entities)
            {
                if (!entity.IsAlive() || entity.AddressBase == player.AddressBase)
                {
                    continue;
                }

                // check if aim ray intersects any hitboxes of entity
                var hitBoxId = IntersectsHitBox(aimRayWorld, entity);
                if (hitBoxId >= 0)
                {
                    // shoot
                    U.MouseLeftDown();
                    U.MouseLeftUp();
                    Thread.Sleep(5);
                }
            }
        }

        /// <summary>
        /// Check if aim ray intersects any hitbox of entity.
        /// </summary>
        /// <returns>
        /// Returns id of intersected hitbox, otherwise -1.
        /// </returns>
        public static int IntersectsHitBox(Line3D aimRayWorld, Entity entity)
        {
            for (var hitBoxId = 0; hitBoxId < entity.StudioHitBoxSet.numhitboxes; hitBoxId++)
            {
                var hitBox = entity.StudioHitBoxes[hitBoxId];
                var boneId = hitBox.bone;
                if (boneId < 0 || boneId > Offsets.MAXSTUDIOBONES || hitBox.radius <= 0)
                {
                    continue;
                }

                // intersect capsule
                var matrixBoneModelToWorld = entity.BonesMatrices[boneId];
                var boneStartWorld = matrixBoneModelToWorld.Transform(hitBox.bbmin);
                var boneEndWorld = matrixBoneModelToWorld.Transform(hitBox.bbmax);
                var boneWorld = new Line3D(boneStartWorld, boneEndWorld);
                var (p0, p1) = aimRayWorld.ClosestPointsBetween(boneWorld, true);
                var distance = (p1 - p0).Length();
                if (distance < hitBox.radius * 0.9f /* trigger a little bit inside */)
                {
                    // intersects
                    return hitBoxId;
                }
            }

            return -1;
        }

        #endregion
    }
}