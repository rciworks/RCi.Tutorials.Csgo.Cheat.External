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
            GameProcess.Start();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            GameProcess.Dispose();
            GameProcess = default;
        }

        #endregion
    }
}
