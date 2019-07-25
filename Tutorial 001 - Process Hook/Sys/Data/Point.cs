using System.Runtime.InteropServices;

namespace RCi.Tutorials.Csgo.Cheat.External.Sys.Data
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/windef/ns-windef-point
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int X, Y;
    }
}
