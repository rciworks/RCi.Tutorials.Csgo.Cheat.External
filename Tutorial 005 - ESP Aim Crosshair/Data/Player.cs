using System;
using Microsoft.DirectX;
using RCi.Tutorials.Csgo.Cheat.External.Gfx;
using RCi.Tutorials.Csgo.Cheat.External.Utils;

namespace RCi.Tutorials.Csgo.Cheat.External.Data
{
    /// <summary>
    /// Player data.
    /// </summary>
    public class Player
    {
        #region // storage

        /// <summary>
        /// Matrix from clipping space to screen space.
        /// </summary>
        public Matrix MatrixViewport { get; private set; }

        /// <summary>
        /// Model origin (in world).
        /// </summary>
        public Vector3 Origin { get; private set; }

        /// <summary>
        /// Local offset from model origin to player eyes.
        /// </summary>
        public Vector3 ViewOffset { get; private set; }

        /// <summary>
        /// Eye position (in world).
        /// </summary>
        public Vector3 EyePosition { get; private set; }

        /// <summary>
        /// View angles (in degrees).
        /// </summary>
        public Vector3 ViewAngles { get; private set; }

        /// <summary>
        /// Aim punch angles (in degrees).
        /// </summary>
        public Vector3 AimPunchAngle { get; private set; }

        /// <summary>
        /// Aim direction (in world).
        /// </summary>
        public Vector3 AimDirection { get; private set; }

        /// <summary>
        /// Player vertical field of view (in degrees).
        /// </summary>
        public int Fov { get; private set; }

        #endregion

        #region // routines

        /// <summary>
        /// Update data from process.
        /// </summary>
        public void Update(GameProcess gameProcess)
        {
            var addressBase = gameProcess.ModuleClient.Read<IntPtr>(Offsets.dwLocalPlayer);
            if (addressBase == IntPtr.Zero)
            {
                return;
            }

            // get matrices
            MatrixViewport = GfxMath.GetMatrixViewport(gameProcess.WindowRectangleClient.Size);

            // read data
            Origin = gameProcess.Process.Read<Vector3>(addressBase + Offsets.m_vecOrigin);
            ViewOffset = gameProcess.Process.Read<Vector3>(addressBase + Offsets.m_vecViewOffset);
            EyePosition = Origin + ViewOffset;
            ViewAngles = gameProcess.Process.Read<Vector3>(gameProcess.ModuleEngine.Read<IntPtr>(Offsets.dwClientState) + Offsets.dwClientState_ViewAngles);
            AimPunchAngle = gameProcess.Process.Read<Vector3>(addressBase + Offsets.m_aimPunchAngle);
            Fov = gameProcess.Process.Read<int>(addressBase + Offsets.m_iFOV);
            if (Fov == 0) Fov = 90; // correct for default

            // calc data
            AimDirection = GetAimDirection(ViewAngles, AimPunchAngle);
        }

        /// <summary>
        /// Get aim direction.
        /// </summary>
        private static Vector3 GetAimDirection(Vector3 viewAngles, Vector3 aimPunchAngle)
        {
            var phi = (viewAngles.X + aimPunchAngle.X * Offsets.weapon_recoil_scale).DegreeToRadian();
            var theta = (viewAngles.Y + aimPunchAngle.Y * Offsets.weapon_recoil_scale).DegreeToRadian();

            // https://en.wikipedia.org/wiki/Spherical_coordinate_system
            return Vector3.Normalize(new Vector3
            (
                (float)(Math.Cos(phi) * Math.Cos(theta)),
                (float)(Math.Cos(phi) * Math.Sin(theta)),
                (float)-Math.Sin(phi)
            ));
        }

        #endregion
    }
}
