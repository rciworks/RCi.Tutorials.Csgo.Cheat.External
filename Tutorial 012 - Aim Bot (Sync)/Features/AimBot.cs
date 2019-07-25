using System;
using System.Linq;
using System.Threading;
using Microsoft.DirectX;
using RCi.Tutorials.Csgo.Cheat.External.Data;
using RCi.Tutorials.Csgo.Cheat.External.Gfx.Math;
using RCi.Tutorials.Csgo.Cheat.External.Sys;
using RCi.Tutorials.Csgo.Cheat.External.Sys.Data;
using RCi.Tutorials.Csgo.Cheat.External.Utils;
using Point = System.Drawing.Point;

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

        /// <summary>
        /// Bone id to aim for, 8 = head.
        /// </summary>
        private const int AimBoneId = 8;

        /// <summary>
        /// Aim bot field of view (fov) to find targets (in radians).
        /// </summary>
        private static float AimBotFov { get; set; } = 10f.DegreeToRadian();

        /// <inheritdoc />
        protected override string ThreadName => nameof(AimBot);

        /// <inheritdoc cref="GameProcess"/>
        private GameProcess GameProcess { get; set; }

        /// <inheritdoc cref="GameData"/>
        private GameData GameData { get; set; }

        /// <summary>
        /// Global mouse hook.
        /// </summary>
        private GlobalHook MouseHook { get; set; }

        /// <summary>
        /// Synchronization object for <see cref="State"/>.
        /// </summary>
        private readonly object StateLock = new object();

        /// <inheritdoc cref="AimBotState"/>
        private AimBotState State { get; set; }

        #endregion

        #region // ctor

        /// <summary />
        public AimBot(GameProcess gameProcess, GameData gameData)
        {
            GameProcess = gameProcess;
            GameData = gameData;
            MouseHook = new GlobalHook(HookType.WH_MOUSE_LL, MouseHookCallback);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();

            MouseHook.Dispose();
            MouseHook = default;

            GameData = default;

            GameProcess = default;
        }

        #endregion

        #region // routines

        /// <inheritdoc cref="HookProc"/>
        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            return nCode < 0 || ProcessMouseMessage((MouseMessage)wParam)
                ? User32.CallNextHookEx(MouseHook.HookHandle, nCode, wParam, lParam)    // continue
                : new IntPtr(1);                                                        // suppress
        }

        /// <summary>
        /// Control <see cref="State"/> based on mouse input.
        /// </summary>
        private bool ProcessMouseMessage(MouseMessage mouseMessage)
        {
            if (mouseMessage == MouseMessage.WM_LBUTTONUP)
            {
                // left mouse up
                lock (StateLock)
                {
                    // no matter what state it was before, we just released left mouse, therefore get to idle state
                    State = AimBotState.Up;
                }
                return true;
            }

            if (mouseMessage == MouseMessage.WM_LBUTTONDOWN)
            {
                if (!GameProcess.IsValid || !GameData.Player.IsAlive() || TriggerBot.IsHotKeyDown())
                {
                    return true;
                }

                // left mouse down
                lock (StateLock)
                {
                    if (State == AimBotState.Up)
                    {
                        // change state and signal to not call CallNextHookEx (stop this left mouse down)
                        State = AimBotState.DownSuppressed;
                        return false;
                    }
                }
            }

            return true;
        }

        /// <inheritdoc />
        protected override void FrameAction()
        {
            if (!GameProcess.IsValid || !GameData.Player.IsAlive())
            {
                return;
            }

            if (IsCalibrateHotKeyDown())
            {
                Calibrate();
                return;
            }

            lock (StateLock)
            {
                if (State == AimBotState.Up)
                {
                    // idle state, still waiting for suppressed state
                    return;
                }
            }

            // get and validate aim target
            var aimPixels = Point.Empty;
            if (GetAimTarget(out var aimAngles))
            {
                // get pixels to move
                GetAimPixels(aimAngles, out aimPixels);
            }

            // try mouse down to get into normal state
            var wait = TryMouseDown();

            // try to move mouse on a target
            wait |= TryMouseMove(aimPixels);

            // give time for csgo to process simulated input
            if (wait)
            {
                // arbitrary amount of wait
                Thread.Sleep(20);
            }
        }

        /// <summary>
        /// Get aim target.
        /// </summary>
        /// <param name="aimAngles">Euler angles to aim target (in radians).</param>
        /// <returns>
        /// <see langword="true"/> if aim target was found.
        /// </returns>
        private bool GetAimTarget(out Vector2 aimAngles)
        {
            var minAngleSize = float.MaxValue;
            aimAngles = new Vector2((float)Math.PI, (float)Math.PI);
            var targetFound = false;

            foreach (var entity in GameData.Entities)
            {
                // validate
                if (!entity.IsAlive() || entity.AddressBase == GameData.Player.AddressBase)
                {
                    continue;
                }

                // get angle to bone
                GetAimAngles(entity.BonesPos[AimBoneId], out var angleToBoneSize, out var anglesToBone);

                // let it only be inside fov search space
                if (angleToBoneSize > AimBotFov)
                {
                    continue;
                }

                // check if it's closer
                if (angleToBoneSize < minAngleSize)
                {
                    minAngleSize = angleToBoneSize;
                    aimAngles = anglesToBone;
                    targetFound = true;
                }
            }

            return targetFound;
        }

        /// <summary>
        /// Get aim angle to a point.
        /// </summary>
        /// <param name="pointWorld">A point (in world) to which aim angles are calculated.</param>
        /// <param name="angleSize">Angle size (in radians) between aim direction and desired aim direction (direction to <see cref="pointWorld"/>).</param>
        /// <param name="aimAngles">Euler angles to aim target (in radians).</param>
        private void GetAimAngles(Vector3 pointWorld, out float angleSize, out Vector2 aimAngles)
        {
            var aimDirection = GameData.Player.AimDirection;
            var aimDirectionDesired = (pointWorld - GameData.Player.EyePosition).Normalized();
            angleSize = aimDirection.AngleTo(aimDirectionDesired);
            aimAngles = new Vector2
            (
                aimDirectionDesired.AngleToSigned(aimDirection, new Vector3(0, 0, 1)),
                aimDirectionDesired.AngleToSigned(aimDirection, aimDirectionDesired.Cross(new Vector3(0, 0, 1)).Normalized())
            );
        }

        /// <summary>
        /// Get pixels to move in a screen (from aim angles).
        /// </summary>
        private void GetAimPixels(Vector2 aimAngles, out Point aimPixels)
        {
            var fovRatio = 90.0 / GameData.Player.Fov;
            aimPixels = new Point
            (
                (int)Math.Round(aimAngles.X / AnglePerPixel * fovRatio),
                (int)Math.Round(aimAngles.Y / AnglePerPixel * fovRatio)
            );
        }

        /// <summary>
        /// Try to simulate mouse move.
        /// </summary>
        private bool TryMouseMove(Point aimPixels)
        {
            if (aimPixels.X == 0 && aimPixels.Y == 0)
            {
                return false;
            }

            U.MouseMove(aimPixels.X, aimPixels.Y);
            return true;
        }

        /// <summary>
        /// Try release suppressed mouse down.
        /// </summary>
        private bool TryMouseDown()
        {
            var mouseDown = false;

            lock (StateLock)
            {
                if (State == AimBotState.DownSuppressed)
                {
                    mouseDown = true;
                    State = AimBotState.Down;
                }
            }

            if (mouseDown)
            {
                U.MouseLeftDown();
            }

            return mouseDown;
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

    #region // AimBotState

    /// <summary>
    /// Aim bot state.
    /// </summary>
    public enum AimBotState
    {
        /// <summary>
        /// Hot key is up, waiting for it to be pressed.
        /// </summary>
        Up,

        /// <summary>
        /// Hot key was pressed and suppressed.
        /// </summary>
        DownSuppressed,

        /// <summary>
        /// Hot key down simulated (suppress released).
        /// </summary>
        Down,
    }

    #endregion
}
