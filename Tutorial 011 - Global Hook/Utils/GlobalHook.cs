using System;
using System.Diagnostics;
using RCi.Tutorials.Csgo.Cheat.External.Sys;
using RCi.Tutorials.Csgo.Cheat.External.Sys.Data;

namespace RCi.Tutorials.Csgo.Cheat.External.Utils
{
    public class GlobalHook :
        IDisposable
    {
        #region // storage

        /// <inheritdoc cref="HookType"/>
        public HookType HookType { get; }

        /// <summary>
        /// Hook callback delegate.
        /// </summary>
        private HookProc HookProc { get; set; }

        /// <summary>
        /// Hook handle.
        /// </summary>
        public IntPtr HookHandle { get; private set; }

        #endregion

        #region // ctor

        /// <summary />
        public GlobalHook(HookType hookType, HookProc hookProc)
        {
            HookType = hookType;
            HookProc = hookProc;
            HookHandle = Hook(HookType, HookProc);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ReleaseUnmanagedResources();

            // prevent destructor (since we already release unmanaged resources)
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Destructor. This should be called by finalized if Dispose is not called.
        /// </summary>
        ~GlobalHook()
        {
            ReleaseUnmanagedResources();
        }

        /// <summary>
        /// Unhook. Should be called once.
        /// </summary>
        private void ReleaseUnmanagedResources()
        {
            // unhook and reset handle
            UnHook(HookHandle);
            HookHandle = default;

            // release callback reference, will let GC to collect it
            HookProc = default;
        }

        #endregion

        #region // routines

        /// <summary>
        /// Install an application-defined hook procedure into a hook chain.
        /// </summary>
        private static IntPtr Hook(HookType hookType, HookProc hookProc)
        {
            using (var currentProcess = Process.GetCurrentProcess())
            {
                using (var curModule = currentProcess.MainModule)
                {
                    if (curModule is null)
                    {
                        throw new ArgumentNullException(nameof(curModule));
                    }

                    var hHook = User32.SetWindowsHookEx((int)hookType, hookProc, Kernel32.GetModuleHandle(curModule.ModuleName), 0);
                    if (hHook == IntPtr.Zero)
                    {
                        throw new ArgumentException("Hook failed.");
                    }

                    return hHook;
                }
            }
        }

        /// <summary>
        /// Remove a hook procedure installed in a hook chain.
        /// </summary>
        private static void UnHook(IntPtr hHook)
        {
            if (!User32.UnhookWindowsHookEx(hHook))
            {
                throw new ArgumentException("UnHook failed.");
            }
        }

        #endregion
    }
}
