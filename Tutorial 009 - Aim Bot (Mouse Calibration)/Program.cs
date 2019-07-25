using System;
using RCi.Tutorials.Csgo.Cheat.External.Data;
using RCi.Tutorials.Csgo.Cheat.External.Features;
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

        /// <inheritdoc cref="TriggerBot"/>
        private TriggerBot TriggerBot { get; set; }

        /// <inheritdoc cref="AimBot"/>
        private AimBot AimBot { get; set; }

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
            TriggerBot = new TriggerBot(GameProcess, GameData);
            AimBot = new AimBot(GameProcess, GameData);

            GameProcess.Start();
            GameData.Start();
            WindowOverlay.Start();
            Graphics.Start();
            TriggerBot.Start();
            AimBot.Start();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            AimBot.Dispose();
            AimBot = default;

            TriggerBot.Dispose();
            TriggerBot = default;

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
