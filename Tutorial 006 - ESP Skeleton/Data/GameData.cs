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

        #endregion

        #region // ctor

        /// <summary />
        public GameData(GameProcess gameProcess)
        {
            GameProcess = gameProcess;
            Player = new Player();
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();

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
        }
    }
}
