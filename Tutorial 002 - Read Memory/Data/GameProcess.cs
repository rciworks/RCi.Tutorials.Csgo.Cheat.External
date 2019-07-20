using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using RCi.Tutorials.Csgo.Cheat.External.Sys;
using RCi.Tutorials.Csgo.Cheat.External.Utils;

namespace RCi.Tutorials.Csgo.Cheat.External.Data
{
    /// <summary>
    /// Game process component.
    /// </summary>
    public class GameProcess :
        ThreadedComponent
    {
        #region // static

        private const string NAME_PROCESS = "csgo";

        private const string NAME_MODULE_CLIENT = "client_panorama.dll";

        private const string NAME_MODULE_ENGINE = "engine.dll";

        private const string NAME_WINDOW = "Counter-Strike: Global Offensive";

        #endregion

        #region // storage

        /// <inheritdoc />
        protected override string ThreadName => nameof(GameProcess);

        /// <inheritdoc />
        protected override TimeSpan ThreadFrameSleep { get; set; } = new TimeSpan(0, 0, 0, 0, 500);

        /// <summary>
        /// Game process.
        /// </summary>
        public Process Process { get; private set; }

        /// <summary>
        /// Client module.
        /// </summary>
        public Module ModuleClient { get; private set; }

        /// <summary>
        /// Engine module.
        /// </summary>
        public Module ModuleEngine { get; private set; }

        /// <summary>
        /// Game window handle.
        /// </summary>
        private IntPtr WindowHwnd { get; set; }

        /// <summary>
        /// Game window client rectangle.
        /// </summary>
        public Rectangle WindowRectangleClient { get; private set; }

        /// <summary>
        /// Whether game window is active.
        /// </summary>
        private bool WindowActive { get; set; }

        /// <summary>
        /// Is game process valid?
        /// </summary>
        public bool IsValid => WindowActive && !(Process is null) && !(ModuleClient is null) && !(ModuleEngine is null);

        #endregion

        #region // ctor

        /// <inheritdoc />
        public override void Dispose()
        {
            InvalidateWindow();
            InvalidateModules();

            base.Dispose();
        }

        #endregion

        #region // routines

        /// <inheritdoc />
        protected override void FrameAction()
        {
            if (!EnsureProcessAndModules())
            {
                InvalidateModules();
            }

            if (!EnsureWindow())
            {
                InvalidateWindow();
            }

            Console.WriteLine(IsValid
                ? $"0x{(int)Process.Handle:X8} {WindowRectangleClient.X} {WindowRectangleClient.Y} {WindowRectangleClient.Width} {WindowRectangleClient.Height}"
                : "Game process invalid");
        }

        /// <summary>
        /// Invalidate all game modules.
        /// </summary>
        private void InvalidateModules()
        {
            ModuleEngine?.Dispose();
            ModuleEngine = default;

            ModuleClient?.Dispose();
            ModuleClient = default;

            Process?.Dispose();
            Process = default;
        }

        /// <summary>
        /// Invalidate game window.
        /// </summary>
        private void InvalidateWindow()
        {
            WindowHwnd = IntPtr.Zero;
            WindowRectangleClient = Rectangle.Empty;
            WindowActive = false;
        }

        /// <summary>
        /// Ensure game process and modules.
        /// </summary>
        private bool EnsureProcessAndModules()
        {
            if (Process is null)
            {
                Process = Process.GetProcessesByName(NAME_PROCESS).FirstOrDefault();
            }
            if (Process is null || !Process.IsRunning())
            {
                return false;
            }

            if (ModuleClient is null)
            {
                ModuleClient = Process.GetModule(NAME_MODULE_CLIENT);
            }
            if (ModuleClient is null)
            {
                return false;
            }

            if (ModuleEngine is null)
            {
                ModuleEngine = Process.GetModule(NAME_MODULE_ENGINE);
            }
            if (ModuleEngine is null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Ensure game window.
        /// </summary>
        private bool EnsureWindow()
        {
            WindowHwnd = User32.FindWindow(null, NAME_WINDOW);
            if (WindowHwnd == IntPtr.Zero)
            {
                return false;
            }

            WindowRectangleClient = U.GetClientRectangle(WindowHwnd);
            if (WindowRectangleClient.Width <= 0 || WindowRectangleClient.Height <= 0)
            {
                return false;
            }

            WindowActive = WindowHwnd == User32.GetForegroundWindow();

            return WindowActive;
        }

        #endregion
    }
}
