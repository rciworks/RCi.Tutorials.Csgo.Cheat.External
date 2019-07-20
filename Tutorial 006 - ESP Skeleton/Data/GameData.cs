using System.Linq;
using RCi.Tutorials.Csgo.Cheat.External.Data.Internal;
using RCi.Tutorials.Csgo.Cheat.External.Utils;

namespace RCi.Tutorials.Csgo.Cheat.External.Data
{
    /// <summary>
    /// Game data retrieved from process.
    /// </summary>
    public class GameData :
        ThreadedComponent
    {
        #region // storage

        /// <inheritdoc />
        protected override string ThreadName => nameof(GameData);

        /// <inheritdoc cref="GameProcess"/>
        private GameProcess GameProcess { get; set; }

        /// <inheritdoc cref="Player"/>
        public Player Player { get; set; }

        /// <inheritdoc cref="Entity"/>
        public Entity[] Entities { get; private set; }

        #endregion

        #region // ctor

        /// <summary />
        public GameData(GameProcess gameProcess)
        {
            GameProcess = gameProcess;
            Player = new Player();
            Entities = Enumerable.Range(0, 64).Select(index => new Entity(index)).ToArray();
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();

            Entities = default;
            Player = default;
            GameProcess = default;
        }

        #endregion

        /// <inheritdoc />
        protected override void FrameAction()
        {
            if (!GameProcess.IsValid)
            {
                return;
            }

            Player.Update(GameProcess);
            foreach (var entity in Entities)
            {
                entity.Update(GameProcess);
            }
        }
    }
}
