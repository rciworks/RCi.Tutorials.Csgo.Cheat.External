using Microsoft.DirectX;
using RCi.Tutorials.Csgo.Cheat.External.Gfx;
using RCi.Tutorials.Csgo.Cheat.External.Gfx.Math;
using RCi.Tutorials.Csgo.Cheat.External.Utils;

namespace RCi.Tutorials.Csgo.Cheat.External.Features
{
    /// <summary>
    /// ESP Aim Crosshair.
    /// </summary>
    public static class EspAimCrosshair
    {
        /// <summary>
        /// Get aim crosshair in screen space.
        /// </summary>
        public static Vector3 GetPositionScreen(Graphics graphics)
        {
            var screenSize = graphics.GameProcess.WindowRectangleClient.Size;
            var aspectRatio = (double)screenSize.Width / screenSize.Height;
            var player = graphics.GameData.Player;
            var fovY = ((double)player.Fov).DegreeToRadian();
            var fovX = fovY * aspectRatio;
            var punchX = ((double)player.AimPunchAngle.X * Offsets.weapon_recoil_scale).DegreeToRadian();
            var punchY = ((double)player.AimPunchAngle.Y * Offsets.weapon_recoil_scale).DegreeToRadian();
            var pointClip = new Vector3
            (
                (float)(-punchY / fovX),
                (float)(-punchX / fovY),
                0
            );
            return player.MatrixViewport.Transform(pointClip);
        }

        /// <summary>
        /// Draw aim crosshair.
        /// </summary>
        public static void Draw(Graphics graphics)
        {
            Draw(graphics, GetPositionScreen(graphics));
        }

        /// <summary>
        /// Draw aim crosshair in screen space.
        /// </summary>
        private static void Draw(Graphics graphics, Vector3 pointScreen)
        {
            const int radius = 3;
            var color = System.Drawing.Color.Red;
            graphics.DrawPolylineScreen(color, pointScreen - new Vector3(radius, 0, 0), pointScreen + new Vector3(radius + 1, 0, 0));
            graphics.DrawPolylineScreen(color, pointScreen - new Vector3(0, radius, 0), pointScreen + new Vector3(0, radius + 1, 0));
        }
    }
}
