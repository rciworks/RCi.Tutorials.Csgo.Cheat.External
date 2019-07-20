using System;
using System.Runtime.InteropServices;

namespace RCi.Tutorials.Csgo.Cheat.External.Sys
{
    public static class Kernel32
    {
        /// <summary>
        /// Reads data from an area of memory in a specified process.
        /// The entire area to be read must be accessible or the operation fails.
        /// </summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out, MarshalAs(UnmanagedType.AsAny)] object lpBuffer, int dwSize, out int lpNumberOfBytesRead);
    }
}
