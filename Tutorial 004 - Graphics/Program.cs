using System;
using RCi.Tutorials.Csgo.Cheat.External.Data;
using RCi.Tutorials.Csgo.Cheat.External.Gfx;

namespace RCi.Tutorials.Csgo.Cheat.External
{
    /// <summary>
    /// Main program.
    /// </summary>
    public class Program :
        System.Windows.Application,
        IDisposable
    {
        #region // entry point

        /// <summary />
        [STAThread]
        public static void Main() => new Program().Run();

        #endregion

        #region // storage

        /// <inheritdoc cref="GameProcess"/>
        private GameProcess GameProcess { get; set; }

        /// <inheritdoc cref="GameData"/>
        private GameData GameData { get; set; }

        /// <inheritdoc cref="WindowOverlay"/>
        private WindowOverlay WindowOverlay { get; set; }

        /// <inheritdoc cref="Graphics"/>
        private Graphics Graphics { get; set; }

        #endregion

        #region // ctor

        /// <summary />
        public Program()
        {
            Startup += (sender, args) => Ctor();
            Exit += (sender, args) => Dispose();
        }

        /// <summary />
        public void Ctor()
        {
            GameProcess = new GameProcess();
            GameData = new GameData(GameProcess);
            WindowOverlay = new WindowOverlay(GameProcess);
            Graphics = new Graphics(WindowOverlay, GameProcess, GameData);

            GameProcess.Start();
            GameData.Start();
            WindowOverlay.Start();
            Graphics.Start();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Graphics.Dispose();
            Graphics = default;

            WindowOverlay.Dispose();
            WindowOverlay = default;

            GameData.Dispose();
            GameData = default;

            GameProcess.Dispose();
            GameProcess = default;
        }

        #endregion
    }
}
