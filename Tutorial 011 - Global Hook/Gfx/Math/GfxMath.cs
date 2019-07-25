using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using RCi.Tutorials.Csgo.Cheat.External.Utils;

namespace RCi.Tutorials.Csgo.Cheat.External.Gfx.Math
{
    public static class GfxMath
    {
        /// <summary>
        /// Angle between 3d vectors.
        /// </summary>
        public static float AngleTo(this Vector3 vector, Vector3 other)
        {
            return AngleBetweenUnitVectors(vector.Normalized(), other.Normalized());
        }

        /// <summary>
        /// Angle between 3d unit vectors (normalized vectors).
        /// </summary>
        public static float AngleBetweenUnitVectors(Vector3 leftNormalized, Vector3 rightNormalized)
        {
            return AcosClamped(leftNormalized.Dot(rightNormalized));
        }

        /// <summary>
        /// Signed angle between 3d vectors (about given vector).
        /// </summary>
        /// <remarks>
        /// "https://en.wikipedia.org/wiki/3D_projection"
        /// "https://en.wikipedia.org/wiki/Projection_(linear_algebra)"
        /// </remarks>
        public static float AngleToSigned(this Vector3 vector, Vector3 other, Vector3 about)
        {
            // validate
            if (vector.IsParallelTo(about, 1E-9f))
            {
                throw new ArgumentException($"'{nameof(vector)}' is parallel to '{nameof(about)}'.");
            }
            if (other.IsParallelTo(about, 1E-9f))
            {
                throw new ArgumentException($"'{nameof(other)}' is parallel to '{nameof(about)}'.");
            }

            // project vectors on a plane
            var plane = new Plane3D(about, new Vector3());
            var vectorOnPlane = plane.ProjectVector(vector).vector.Normalized();
            var otherOnPlane = plane.ProjectVector(other).vector.Normalized();

            // get angle
            var sign = vectorOnPlane.Cross(otherOnPlane).Normalized().Dot(plane.Normal);
            return AngleBetweenUnitVectors(vectorOnPlane, otherOnPlane) * sign;
        }

        /// <summary>
        /// <see cref="Math.Acos"/> with clamped [-1..1] value.
        /// </summary>
        public static float AcosClamped(float value, float tolerance = 1E-6f)
        {
            if (value > 1 - tolerance)
            {
                return 0;
            }
            if (value < tolerance - 1)
            {
                return (float)System.Math.PI;
            }
            return (float)System.Math.Acos(value);
        }

        /// <inheritdoc cref="Vector3.Cross"/>
        public static Vector3 Cross(this Vector3 left, Vector3 right)
        {
            return Vector3.Cross(left, right);
        }

        /// <inheritdoc cref="Vector3.Dot"/>
        public static float Dot(this Vector3 left, Vector3 right)
        {
            return Vector3.Dot(left, right);
        }

        /// <summary>
        /// Get 3d circle vertices.
        /// </summary>
        public static Vector3[] GetCircleVertices(Vector3 origin, Vector3 normal, double radius, int segments)
        {
            var matrixLocalToWorld = GetOrthogonalMatrix(normal, origin);
            var vertices = GetCircleVertices2D(new Vector3(), radius, segments);
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = matrixLocalToWorld.Transform(vertices[i]);
            }
            return vertices;
        }

        /// <summary>
        /// Get 2d (flat) circle vertices (x,y,0).
        /// </summary>
        public static Vector3[] GetCircleVertices2D(Vector3 origin, double radius, int segments)
        {
            var vertices = new Vector3[segments + 1];
            var step = System.Math.PI * 2 / segments;
            for (var i = 0; i < segments; i++)
            {
                var theta = step * i;
                vertices[i] = new Vector3
                (
                    (float)(origin.X + radius * System.Math.Cos(theta)),
                    (float)(origin.Y + radius * System.Math.Sin(theta)),
                    0
                );
            }
            vertices[segments] = vertices[0];
            return vertices;
        }

        /// <summary>
        /// Get half-sphere vertices.
        /// </summary>
        /// <returns>
        /// Returns array of 3d circles. Each circle is array of vertices.
        /// </returns>
        public static Vector3[][] GetHalfSphere(Vector3 origin, Vector3 normal, float radius, int segments, int layers)
        {
            normal.Normalize();
            var verticesByLayer = new Vector3[layers][];
            for (var layerId = 0; layerId < layers; layerId++)
            {
                var radiusLayer = radius - layerId * (radius / layers);
                var originLayer = origin + normal * ((float)System.Math.Cos(System.Math.Asin(radiusLayer / radius)) * radius);
                verticesByLayer[layerId] = GetCircleVertices(originLayer, normal, radiusLayer, segments);
            }
            return verticesByLayer;
        }

        /// <summary>
        /// Get matrix from given axis and origin.
        /// </summary>
        public static Matrix GetMatrix(Vector3 xAxis, Vector3 yAxis, Vector3 zAxis, Vector3 origin)
        {
            return new Matrix
            {
                M11 = xAxis.X,
                M12 = xAxis.Y,
                M13 = xAxis.Z,

                M21 = yAxis.X,
                M22 = yAxis.Y,
                M23 = yAxis.Z,

                M31 = zAxis.X,
                M32 = zAxis.Y,
                M33 = zAxis.Z,

                M41 = origin.X,
                M42 = origin.Y,
                M43 = origin.Z,
                M44 = 1,
            };
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
        /// Get orthogonal axis from given normal.
        /// </summary>
        public static void GetOrthogonalAxis(Vector3 normal, out Vector3 xAxis, out Vector3 yAxis, out Vector3 zAxis)
        {
            zAxis = normal.Normalized();
            var zAxisWorld = new Vector3(0, 0, 1);
            var angleToAxisZ = zAxis.AngleTo(zAxisWorld);
            if (angleToAxisZ < System.Math.PI * 0.25 || angleToAxisZ > System.Math.PI * 0.75)
            {
                // too close to z-axis, use y-axis
                xAxis = new Vector3(0, 1, 0).Cross(zAxis).Normalized();
            }
            else
            {
                // use z-axis
                xAxis = zAxis.Cross(zAxisWorld).Normalized();
            }
            yAxis = zAxis.Cross(xAxis).Normalized();
        }

        /// <summary>
        /// Get orthogonal matrix from given normal and origin.
        /// </summary>
        public static Matrix GetOrthogonalMatrix(Vector3 normal, Vector3 origin)
        {
            GetOrthogonalAxis(normal, out var xAxis, out var yAxis, out var zAxis);
            return GetMatrix(xAxis, yAxis, zAxis, origin);
        }

        /// <summary>
        /// Get unit vector on a sphere from euler angles.
        /// https://en.wikipedia.org/wiki/Spherical_coordinate_system
        /// </summary>
        public static Vector3 GetVectorFromEulerAngles(double phi, double theta)
        {
            return new Vector3
            (
                (float)(System.Math.Cos(phi) * System.Math.Cos(theta)),
                (float)(System.Math.Cos(phi) * System.Math.Sin(theta)),
                (float)-System.Math.Sin(phi)
            ).Normalized();
        }

        /// <summary>
        /// Is vector parallel to other vector?
        /// </summary>
        public static bool IsParallelTo(this Vector3 vector, Vector3 other, float tolerance = 1E-6f)
        {
            return System.Math.Abs(1.0 - System.Math.Abs(vector.Normalized().Dot(other.Normalized()))) <= tolerance;
        }

        /// <summary>
        /// Check if vector is valid to draw in screen space.
        /// </summary>
        public static bool IsValidScreen(this Vector3 value)
        {
            return !value.X.IsInfinityOrNaN() && !value.Y.IsInfinityOrNaN() && value.Z >= 0 && value.Z < 1;
        }

        /// <inheritdoc cref="Vector3.Normalize(Vector3)"/>
        public static Vector3 Normalized(this Vector3 value)
        {
            return Vector3.Normalize(value);
        }

        /// <summary>
        /// Convert to matrix 4x4.
        /// </summary>
        public static Matrix ToMatrix(this in Data.Raw.matrix3x4_t matrix)
        {
            return new Matrix
            {
                M11 = matrix.m00,
                M12 = matrix.m01,
                M13 = matrix.m02,

                M21 = matrix.m10,
                M22 = matrix.m11,
                M23 = matrix.m12,

                M31 = matrix.m20,
                M32 = matrix.m21,
                M33 = matrix.m22,

                M41 = matrix.m30,
                M42 = matrix.m31,
                M43 = matrix.m32,
                M44 = 1,
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
