using System.Drawing;
using System.Linq;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using RCi.Tutorials.Csgo.Cheat.External.Gfx.Math;

namespace RCi.Tutorials.Csgo.Cheat.External.Gfx
{
    /// <summary>
    /// Graphics device drawing extensions.
    /// </summary>
    public static class DeviceExtensions
    {
        public static void DrawCapsuleWorld(this Graphics graphics, Color color, Vector3 start, Vector3 end, float radius, int segments, int layers)
        {
            var normal = end - start;
            normal.Normalize();

            var halfSphere0 = GfxMath.GetHalfSphere(start, -normal, radius, segments, layers);
            var halfSphere1 = GfxMath.GetHalfSphere(end, normal, radius, segments, layers);

            // world to screen + draw layered circles
            for (var i = 0; i < layers; i++)
            {
                for (var j = 0; j < segments + 1; j++)
                {
                    halfSphere0[i][j] = graphics.TransformWorldToScreen(halfSphere0[i][j]);
                    halfSphere1[i][j] = graphics.TransformWorldToScreen(halfSphere1[i][j]);
                }

                graphics.DrawPolylineScreen(color, halfSphere0[i]);
                graphics.DrawPolylineScreen(color, halfSphere1[i]);
            }

            // draw verticals of half-spheres (connect layered circles)
            var halfSphereTopScreen0 = graphics.TransformWorldToScreen(start - normal * radius);
            var halfSphereTopScreen1 = graphics.TransformWorldToScreen(end + normal * radius);
            var verticals0 = new Vector3[layers + 1];
            var verticals1 = new Vector3[layers + 1];
            for (var vertexId = 0; vertexId < segments + 1; vertexId++)
            {
                for (var layerId = 0; layerId < layers; layerId++)
                {
                    verticals0[layerId] = halfSphere0[layerId][vertexId];
                    verticals1[layerId] = halfSphere1[layerId][vertexId];
                }
                verticals0[layers] = halfSphereTopScreen0;
                verticals1[layers] = halfSphereTopScreen1;

                graphics.DrawPolylineScreen(color, verticals0);
                graphics.DrawPolylineScreen(color, verticals1);
            }

            // draw vertical cylinder edges between half-spheres
            graphics.DrawCylinderSidesWorld(color, start, end, radius, segments);
        }

        private static void DrawCylinderSidesWorld(this Graphics graphics, Color color, Vector3 start, Vector3 end, float radius, int segments)
        {
            var normal = end - start;
            normal.Normalize();

            var vertices0 = GfxMath.GetCircleVertices(start, normal, radius, segments);
            var vertices1 = GfxMath.GetCircleVertices(end, normal, radius, segments);

            for (var i = 0; i < vertices0.Length; i++)
            {
                vertices0[i] = graphics.TransformWorldToScreen(vertices0[i]);
                vertices1[i] = graphics.TransformWorldToScreen(vertices1[i]);

                graphics.DrawPolylineScreen(color, vertices0[i], vertices1[i]);
            }
        }

        /// <summary>
        /// Draw polyline in world space.
        /// </summary>
        public static void DrawPolylineWorld(this Graphics graphics, Color color, params Vector3[] verticesWorld)
        {
            var verticesScreen = verticesWorld.Select(graphics.TransformWorldToScreen).ToArray();
            graphics.DrawPolylineScreen(color, verticesScreen);
        }

        /// <summary>
        /// Draw 2D polyline in screen space.
        /// </summary>
        public static void DrawPolylineScreen(this Graphics graphics, Color color, params Vector3[] vertices)
        {
            if (vertices.Length < 2 || vertices.Any(v => !v.IsValidScreen()))
            {
                return;
            }

            var vertexStreamZeroData = vertices.Select(v => new CustomVertex.TransformedColored(v.X, v.Y, v.Z, 0, color.ToArgb())).ToArray();
            graphics.Device.VertexFormat = VertexFormats.Diffuse | VertexFormats.Transformed;
            graphics.Device.DrawUserPrimitives(PrimitiveType.LineStrip, vertexStreamZeroData.Length - 1, vertexStreamZeroData);
        }
    }
}
