using System.Runtime.InteropServices;

namespace RCi.Tutorials.Csgo.Cheat.External.Sys.Data
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-taginput
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct Input
    {
        [FieldOffset(0)] public SendInputEventType type;
        [FieldOffset(4)] public MouseInput mi;
        [FieldOffset(4)] public KeybdInput ki;
        [FieldOffset(4)] public HardwareInput hi;
    }
}
