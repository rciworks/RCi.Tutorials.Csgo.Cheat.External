using System;

namespace RCi.Tutorials.Csgo.Cheat.External.Sys.Data
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nc-winuser-hookproc
    /// </summary>
    public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
}
