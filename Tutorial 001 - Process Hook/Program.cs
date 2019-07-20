using System;

namespace RCi.Tutorials.Csgo.Cheat.External
{
    public class Program :
        System.Windows.Application,
        IDisposable
    {
        #region // entry point

        public static void Main() => new Program().Run();

        #endregion

        #region // ctor

        public Program()
        {
            Startup += (sender, args) => Ctor();
            Exit += (sender, args) => Dispose();
        }

        public void Ctor()
        {
        }

        public void Dispose()
        {
        }

        #endregion
    }
}
