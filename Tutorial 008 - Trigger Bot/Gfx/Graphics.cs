using System.Drawing;
using System.Windows.Threading;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using RCi.Tutorials.Csgo.Cheat.External.Data;
using RCi.Tutorials.Csgo.Cheat.External.Data.Internal;
using RCi.Tutorials.Csgo.Cheat.External.Features;
using RCi.Tutorials.Csgo.Cheat.External.Utils;

namespace RCi.Tutorials.Csgo.Cheat.External.Gfx
{
    /// <summary>
    /// Graphics for drawing onto overlay window.
    /// </summary>
    public class Graphics :
        ThreadedComponent
    {
        #region // storage

        /// <inheritdoc />
        protected override string ThreadName => nameof(Graphics);

        /// <inheritdoc cref="WindowOverlay" />
        private WindowOverlay WindowOverlay { get; set; }

        /// <inheritdoc cref="GameProcess" />
        public GameProcess GameProcess { get; private set; }

        /// <inheritdoc cref="GameData" />
        public GameData GameData { get; private set; }

        /// <inheritdoc cref="FpsCounter" />
        private FpsCounter FpsCounter { get; set; }

        /// <inheritdoc cref="Device" />
        public Device Device { get; private set; }

        /// <inheritdoc cref="Microsoft.DirectX.Direct3D.Font" />
        public Microsoft.DirectX.Direct3D.Font FontVerdana8 { get; private set; }

        #endregion

        #region // ctor

        /// <summary />
        public Graphics(WindowOverlay windowOverlay, GameProcess gameProcess, GameData gameData)
        {
            WindowOverlay = windowOverlay;
            GameProcess = gameProcess;
            GameData = gameData;
            FpsCounter = new FpsCounter();

            InitDevice();
            FontVerdana8 = new Microsoft.DirectX.Direct3D.Font(Device, new System.Drawing.Font("Verdana", 8.0f, FontStyle.Regular));
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();

            FontVerdana8.Dispose();
            FontVerdana8 = default;
            Device.Dispose();
            Device = default;

            FpsCounter = default;
            GameData = default;
            GameProcess = default;
            WindowOverlay = default;
        }

        #endregion

        #region // routines

        /// <summary>
        /// Initialize graphics device.
        /// </summary>
        private void InitDevice()
        {
            var parameters = new PresentParameters
            {
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                DeviceWindow = WindowOverlay.Window,
                MultiSampleQuality = 0,
                BackBufferFormat = Format.A8R8G8B8,
                BackBufferWidth = WindowOverlay.Window.Width,
                BackBufferHeight = WindowOverlay.Window.Height,
                EnableAutoDepthStencil = true,
                AutoDepthStencilFormat = DepthFormat.D16,
                PresentationInterval = PresentInterval.Immediate, // turn off v-sync
            };

            Device.IsUsingEventHandlers = true;
            Device = new Device(0, DeviceType.Hardware, WindowOverlay.Window, CreateFlags.HardwareVertexProcessing, parameters);
        }

        /// <inheritdoc />
        protected override void FrameAction()
        {
            if (!GameProcess.IsValid)
            {
                return;
            }

            FpsCounter.Update();

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                // set render state
                Device.RenderState.AlphaBlendEnable = true;
                Device.RenderState.AlphaTestEnable = false;
                Device.RenderState.SourceBlend = Blend.SourceAlpha;
                Device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
                Device.RenderState.Lighting = false;
                Device.RenderState.CullMode = Cull.None;
                Device.RenderState.ZBufferEnable = true;
                Device.RenderState.ZBufferFunction = Compare.Always;

                // clear scene
                Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.FromArgb(0, 0, 0, 0), 1, 0);

                // render scene
                Device.BeginScene();
                Render();
                Device.EndScene();

                // flush to screen
                Device.Present();
            }, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Render graphics.
        /// </summary>
        private void Render()
        {
            DrawWindowBorder();
            DrawFps();
            EspAimCrosshair.Draw(this);
            EspSkeleton.Draw(this);
            EspHitBoxes.Draw(this);
        }

        /// <summary>
        /// Draw fps.
        /// </summary>
        private void DrawFps()
        {
            FontVerdana8.DrawText(default, $"{FpsCounter.Fps:0} FPS", 5, 5, Color.Red);
        }

        /// <summary>
        ///  Draw window border.
        /// </summary>
        private void DrawWindowBorder()
        {
            this.DrawPolylineScreen
            (
                Color.LawnGreen,
                new Vector3(0, 0, 0),
                new Vector3(GameProcess.WindowRectangleClient.Width - 1, 0, 0),
                new Vector3(GameProcess.WindowRectangleClient.Width - 1, GameProcess.WindowRectangleClient.Height - 1, 0),
                new Vector3(0, GameProcess.WindowRectangleClient.Height - 1, 0),
                new Vector3(0, 0, 0)
            );
        }

        /// <inheritdoc cref="Player.MatrixViewProjectionViewport"/>
        public Vector3 TransformWorldToScreen(Vector3 value)
        {
            return GameData.Player.MatrixViewProjectionViewport.Transform(value);
        }

        #endregion
    }
}
