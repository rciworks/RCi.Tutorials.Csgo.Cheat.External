using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using RCi.Tutorials.Csgo.Cheat.External.Data;
using RCi.Tutorials.Csgo.Cheat.External.Sys;
using RCi.Tutorials.Csgo.Cheat.External.Sys.Data;
using RCi.Tutorials.Csgo.Cheat.External.Utils;
using Point = System.Drawing.Point;

namespace RCi.Tutorials.Csgo.Cheat.External.Gfx
{
    /// <summary>
    /// Overlay window for graphics.
    /// </summary>
    public class WindowOverlay :
        ThreadedComponent
    {
        #region // storage

        /// <inheritdoc />
        protected override TimeSpan ThreadFrameSleep { get; set; } = new TimeSpan(0, 0, 0, 0, 500);

        /// <inheritdoc cref="GameProcess"/>
        private GameProcess GameProcess { get; set; }

        /// <summary>
        /// Physical overlay window.
        /// </summary>
        public Form Window { get; private set; }

        #endregion

        #region // ctor

        /// <summary />
        public WindowOverlay(GameProcess gameProcess)
        {
            GameProcess = gameProcess;

            // create window
            Window = new Form
            {
                Name = "Overlay Window",
                Text = "Overlay Window",
                MinimizeBox = false,
                MaximizeBox = false,
                FormBorderStyle = FormBorderStyle.None,
                TopMost = true,
                Width = 16,
                Height = 16,
                Left = -32000,
                Top = -32000,
                StartPosition = FormStartPosition.Manual,
            };

            Window.Load += (sender, args) =>
            {
                var exStyle = User32.GetWindowLong(Window.Handle, User32.GWL_EXSTYLE);
                exStyle |= User32.WS_EX_LAYERED;
                exStyle |= User32.WS_EX_TRANSPARENT;

                // make the window's border completely transparent
                User32.SetWindowLong(Window.Handle, User32.GWL_EXSTYLE, (IntPtr)exStyle);

                // set the alpha on the whole window to 255 (solid)
                User32.SetLayeredWindowAttributes(Window.Handle, 0, 255, User32.LWA_ALPHA);
            };
            Window.SizeChanged += (sender, args) => ExtendFrameIntoClientArea();
            Window.LocationChanged += (sender, args) => ExtendFrameIntoClientArea();
            Window.Closed += (sender, args) => System.Windows.Application.Current.Shutdown();

            // show window
            Window.Show();
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();

            Window.Close();
            Window.Dispose();
            Window = default;

            GameProcess = default;
        }

        #endregion

        #region // routines

        /// <summary>
        /// Extend the window frame into the client area.
        /// </summary>
        private void ExtendFrameIntoClientArea()
        {
            var margins = new Margins
            {
                Left = -1,
                Right = -1,
                Top = -1,
                Bottom = -1,
            };
            Dwmapi.DwmExtendFrameIntoClientArea(Window.Handle, ref margins);
        }

        /// <inheritdoc />
        protected override void FrameAction()
        {
            Update(GameProcess.WindowRectangleClient);
        }

        /// <summary>
        /// Update position and size.
        /// </summary>
        private void Update(Rectangle windowRectangleClient)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Window.BackColor = Color.Blue; // TODO: temporary

                if (Window.Location != windowRectangleClient.Location || Window.Size != windowRectangleClient.Size)
                {
                    if (windowRectangleClient.Width > 0 && windowRectangleClient.Height > 0)
                    {
                        // valid
                        Window.Location = windowRectangleClient.Location;
                        Window.Size = windowRectangleClient.Size;
                    }
                    else
                    {
                        // invalid
                        Window.Location = new Point(-32000, -32000);
                        Window.Size = new Size(16, 16);
                    }
                }
            }, DispatcherPriority.Normal);
        }

        #endregion
    }
}
