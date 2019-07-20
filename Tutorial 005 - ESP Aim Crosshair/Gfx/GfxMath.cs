using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace RCi.Tutorials.Csgo.Cheat.External.Gfx
{
    public static class GfxMath
    {
        /// <summary>
        /// Get viewport matrix.
        /// </summary>
        public static Matrix GetMatrixViewport(Size screenSize)
        {
            return GetMatrixViewport(new Viewport
            {
                X = 0,
                Y = 0,
                Width = screenSize.Width,
                Height = screenSize.Height,
                MinZ = 0,
                MaxZ = 1,
            });
        }

        /// <summary>
        /// Get viewport matrix.
        /// </summary>
        public static Matrix GetMatrixViewport(in Viewport viewport)
        {
            return new Matrix
            {
                M11 = viewport.Width * 0.5f,
                M12 = 0,
                M13 = 0,
                M14 = 0,

                M21 = 0,
                M22 = -viewport.Height * 0.5f,
                M23 = 0,
                M24 = 0,

                M31 = 0,
                M32 = 0,
                M33 = viewport.MaxZ - viewport.MinZ,
                M34 = 0,

                M41 = viewport.X + viewport.Width * 0.5f,
                M42 = viewport.Y + viewport.Height * 0.5f,
                M43 = viewport.MinZ,
                M44 = 1
            };
        }

        /// <summary>
        /// Transform value.
        /// </summary>
        public static Vector3 Transform(this in Matrix matrix, Vector3 value)
        {
            var wInv = 1.0 / ((double)matrix.M14 * (double)value.X + (double)matrix.M24 * (double)value.Y + (double)matrix.M34 * (double)value.Z + (double)matrix.M44);
            return new Vector3
            (
                (float)(((double)matrix.M11 * (double)value.X + (double)matrix.M21 * (double)value.Y + (double)matrix.M31 * (double)value.Z + (double)matrix.M41) * wInv),
                (float)(((double)matrix.M12 * (double)value.X + (double)matrix.M22 * (double)value.Y + (double)matrix.M32 * (double)value.Z + (double)matrix.M42) * wInv),
                (float)(((double)matrix.M13 * (double)value.X + (double)matrix.M23 * (double)value.Y + (double)matrix.M33 * (double)value.Z + (double)matrix.M43) * wInv)
            );
        }
    }
}
