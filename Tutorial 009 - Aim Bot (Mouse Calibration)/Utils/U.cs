using System;
using System.Linq;
using System.Runtime.InteropServices;
using RCi.Tutorials.Csgo.Cheat.External.Sys;
using RCi.Tutorials.Csgo.Cheat.External.Sys.Data;

namespace RCi.Tutorials.Csgo.Cheat.External.Utils
{
    /// <summary>
    /// Helper class.
    /// </summary>
    public static class U
    {
        #region // degrees <-> radians

        private const double _PI_Over_180 = Math.PI / 180.0;

        private const double _180_Over_PI = 180.0 / Math.PI;

        public static double DegreeToRadian(this double degree)
        {
            return degree * _PI_Over_180;
        }

        public static double RadianToDegree(this double radian)
        {
            return radian * _180_Over_PI;
        }

        public static float DegreeToRadian(this float degree)
        {
            return (float)(degree * _PI_Over_180);
        }

        public static float RadianToDegree(this float radian)
        {
            return (float)(radian * _180_Over_PI);
        }

        #endregion

        /// <summary>
        /// Get window client rectangle.
        /// </summary>
        public static System.Drawing.Rectangle GetClientRectangle(IntPtr handle)
        {
            return User32.ClientToScreen(handle, out var point) && User32.GetClientRect(handle, out var rect)
                ? new System.Drawing.Rectangle(point.X, point.Y, rect.Right - rect.Left, rect.Bottom - rect.Top)
                : default;
        }

        /// <summary>
        /// Get module by name.
        /// </summary>
        public static Module GetModule(this System.Diagnostics.Process process, string moduleName)
        {
            var processModule = process.GetProcessModule(moduleName);
            return processModule is null || processModule.BaseAddress == IntPtr.Zero
                ? default
                : new Module(process, processModule);
        }

        /// <summary>
        /// Get process module by name.
        /// </summary>
        public static System.Diagnostics.ProcessModule GetProcessModule(this System.Diagnostics.Process process, string moduleName)
        {
            return process?.Modules.OfType<System.Diagnostics.ProcessModule>()
                .FirstOrDefault(a => string.Equals(a.ModuleName.ToLower(), moduleName.ToLower()));
        }

        /// <summary>
        /// Check if value is infinity or NaN.
        /// </summary>
        public static bool IsInfinityOrNaN(this float value)
        {
            return float.IsNaN(value) || float.IsInfinity(value);
        }

        /// <summary>
        /// Is key Down?
        /// </summary>
        public static bool IsKeyDown(this WindowsVirtualKey key)
        {
            return (User32.GetAsyncKeyState((int)key) & 0x8000) != 0;
        }

        /// <summary>
        /// Get if process is running.
        /// </summary>
        public static bool IsRunning(this System.Diagnostics.Process process)
        {
            try
            {
                System.Diagnostics.Process.GetProcessById(process.Id);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Send mouse left down.
        /// </summary>
        public static void MouseLeftDown()
        {
            var mouseMoveInput = new Input
            {
                type = SendInputEventType.InputMouse,
                mi =
                {
                    dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTDOWN
                },
            };
            User32.SendInput(1, ref mouseMoveInput, Marshal.SizeOf<Input>());
        }

        /// <summary>
        /// Send mouse left up.
        /// </summary>
        public static void MouseLeftUp()
        {
            var mouseMoveInput = new Input
            {
                type = SendInputEventType.InputMouse,
                mi =
                {
                    dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTUP
                },
            };
            User32.SendInput(1, ref mouseMoveInput, Marshal.SizeOf<Input>());
        }

        /// <summary>
        /// Read process memory.
        /// </summary>
        public static T Read<T>(this System.Diagnostics.Process process, IntPtr lpBaseAddress)
            where T : unmanaged
        {
            return Read<T>(process.Handle, lpBaseAddress);
        }

        /// <summary>
        /// Read process memory from module.
        /// </summary>
        public static T Read<T>(this Module module, int offset)
            where T : unmanaged
        {
            return Read<T>(module.Process.Handle, module.ProcessModule.BaseAddress + offset);
        }

        /// <summary>
        /// Read process memory.
        /// </summary>
        public static T Read<T>(IntPtr hProcess, IntPtr lpBaseAddress)
            where T : unmanaged
        {
            var size = Marshal.SizeOf<T>();
            var buffer = (object)default(T);
            Kernel32.ReadProcessMemory(hProcess, lpBaseAddress, buffer, size, out var lpNumberOfBytesRead);
            return lpNumberOfBytesRead == size ? (T)buffer : default;
        }

        /// <summary>
        /// Convert value to team.
        /// </summary>
        public static Data.Raw.Team ToTeam(this int teamNum)
        {
            switch (teamNum)
            {
                case 1:
                    return Data.Raw.Team.Spectator;
                case 2:
                    return Data.Raw.Team.Terrorists;
                case 3:
                    return Data.Raw.Team.CounterTerrorists;
                default:
                    return Data.Raw.Team.Unknown;
            }
        }
    }
}
