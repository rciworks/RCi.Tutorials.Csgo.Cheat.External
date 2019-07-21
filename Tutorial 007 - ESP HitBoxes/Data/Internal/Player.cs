using System;
using Microsoft.DirectX;
using RCi.Tutorials.Csgo.Cheat.External.Gfx;
using RCi.Tutorials.Csgo.Cheat.External.Utils;

namespace RCi.Tutorials.Csgo.Cheat.External.Data.Internal
{
    /// <summary>
    /// Player data.
    /// </summary>
    public class Player :
        EntityBase
    {
        #region // storage

        /// <summary>
        /// Matrix from world space to clipping space.
        /// </summary>
        public Matrix MatrixViewProjection { get; private set; }

        /// <summary>
        /// Matrix from clipping space to screen space.
        /// </summary>
        public Matrix MatrixViewport { get; private set; }

        /// <summary>
        /// Matrix from world space to screen space.
        /// </summary>
        public Matrix MatrixViewProjectionViewport { get; private set; }

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

        /// <inheritdoc />
        protected override IntPtr ReadAddressBase(GameProcess gameProcess)
        {
            return gameProcess.ModuleClient.Read<IntPtr>(Offsets.dwLocalPlayer);
        }

        /// <inheritdoc />
        public override bool Update(GameProcess gameProcess)
        {
            if (!base.Update(gameProcess))
            {
                return false;
            }

            // get matrices
            MatrixViewProjection = Matrix.TransposeMatrix(gameProcess.ModuleClient.Read<Matrix>(Offsets.dwViewMatrix));
            MatrixViewport = GfxMath.GetMatrixViewport(gameProcess.WindowRectangleClient.Size);
            MatrixViewProjectionViewport = MatrixViewProjection * MatrixViewport;

            // read data
            ViewOffset = gameProcess.Process.Read<Vector3>(AddressBase + Offsets.m_vecViewOffset);
            EyePosition = Origin + ViewOffset;
            ViewAngles = gameProcess.Process.Read<Vector3>(gameProcess.ModuleEngine.Read<IntPtr>(Offsets.dwClientState) + Offsets.dwClientState_ViewAngles);
            AimPunchAngle = gameProcess.Process.Read<Vector3>(AddressBase + Offsets.m_aimPunchAngle);
            Fov = gameProcess.Process.Read<int>(AddressBase + Offsets.m_iFOV);
            if (Fov == 0) Fov = 90; // correct for default

            // calc data
            AimDirection = GetAimDirection(ViewAngles, AimPunchAngle);

            return true;
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
