using System;
using RCi.Tutorials.Csgo.Cheat.External.Data;

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
        public static void Main() => new Program().Run();

        #endregion

        #region // storage

        /// <inheritdoc cref="GameProcess"/>
        private GameProcess GameProcess { get; set; }
        private GameData GameData { get; set; }

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

            GameProcess.Start();
            GameData.Start();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            GameData.Dispose();
            GameData = default;

            GameProcess.Dispose();
            GameProcess = default;
        }

        #endregion
    }
}
