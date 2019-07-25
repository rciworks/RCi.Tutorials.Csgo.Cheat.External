using System;
using System.Linq;
using System.Threading;
using RCi.Tutorials.Csgo.Cheat.External.Data;
using RCi.Tutorials.Csgo.Cheat.External.Gfx.Math;
using RCi.Tutorials.Csgo.Cheat.External.Sys.Data;
using RCi.Tutorials.Csgo.Cheat.External.Utils;

namespace RCi.Tutorials.Csgo.Cheat.External.Features
{
    /// <summary>
    /// Trigger bot. Shoots when hovering over an enemy.
    /// </summary>
    public class AimBot :
        ThreadedComponent
    {
        #region // storage

        /// <summary>
        /// Moving mouse one pixel will give this much of view angle change (in radians).
        /// </summary>
        private double AnglePerPixel { get; set; } = 0.00057596609244744;

        /// <inheritdoc />
        protected override string ThreadName => nameof(AimBot);

        /// <inheritdoc cref="GameProcess"/>
        private GameProcess GameProcess { get; set; }

        /// <inheritdoc cref="GameData"/>
        private GameData GameData { get; set; }

        #endregion

        #region // ctor

        /// <summary />
        public AimBot(GameProcess gameProcess, GameData gameData)
        {
            GameProcess = gameProcess;
            GameData = gameData;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();

            GameData = default;
            GameProcess = default;
        }

        #endregion

        #region // routines

        /// <inheritdoc />
        protected override void FrameAction()
        {
            if (!GameProcess.IsValid)
            {
                return;
            }

            if (IsCalibrateHotKeyDown())
            {
                Calibrate();
            }
        }

        #endregion

        #region // calibration

        /// <summary>
        /// Is calibration hot key down?
        /// </summary>
        private static bool IsCalibrateHotKeyDown()
        {
            return WindowsVirtualKey.VK_F11.IsKeyDown() && WindowsVirtualKey.VK_F12.IsKeyDown();
        }

        /// <summary>
        /// Calibrate <see cref="AnglePerPixel"/>.
        /// </summary>
        private void Calibrate()
        {
            AnglePerPixel = new[]
            {
                CalibrationMeasureAnglePerPixel(100),
                CalibrationMeasureAnglePerPixel(-200),
                CalibrationMeasureAnglePerPixel(300),
                CalibrationMeasureAnglePerPixel(-400),
                CalibrationMeasureAnglePerPixel(200),
            }.Average();
            Console.WriteLine($"{nameof(AnglePerPixel)} = {AnglePerPixel}");
        }

        /// <summary>
        /// Simulate horizontal mouse move, measure angle difference and get angle per pixel ratio (in radians).
        /// </summary>
        private double CalibrationMeasureAnglePerPixel(int deltaPixels)
        {
            // measure starting angle
            Thread.Sleep(100);
            var eyeDirectionStart = GameData.Player.EyeDirection;
            eyeDirectionStart.Z = 0;

            // rotate
            U.MouseMove(deltaPixels, 0);

            // measure end angle
            Thread.Sleep(100);
            var eyeDirectionEnd = GameData.Player.EyeDirection;
            eyeDirectionEnd.Z = 0;

            // get angle and divide by number of pixels
            return eyeDirectionEnd.AngleTo(eyeDirectionStart) / Math.Abs(deltaPixels);
        }

        #endregion
    }
}
