using System;
using System.Runtime.InteropServices;
using RCi.Tutorials.Csgo.Cheat.External.Sys.Data;

namespace RCi.Tutorials.Csgo.Cheat.External.Sys
{
    public static class Dwmapi
    {
        /// <summary>
        /// Extends the window frame into the client area.
        /// </summary>
        [DllImport("dwmapi.dll", SetLastError = true)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);
    }
}
