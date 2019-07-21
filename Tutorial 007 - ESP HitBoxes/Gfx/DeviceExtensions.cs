using System.Drawing;
using System.Linq;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using RCi.Tutorials.Csgo.Cheat.External.Utils;

namespace RCi.Tutorials.Csgo.Cheat.External.Gfx
{
    /// <summary>
    /// Graphics device drawing extensions.
    /// </summary>
    public static class DeviceExtensions
    {
        /// <summary>
        /// Draw polyline in world space.
        /// </summary>
        public static void DrawPolylineWorld(this Graphics graphics, Color color, params Vector3[] verticesWorld)
        {
            var verticesScreen = verticesWorld.Select(v => graphics.GameData.Player.MatrixViewProjectionViewport.Transform(v)).ToArray();
            graphics.DrawPolylineScreen(verticesScreen, color);
        }

        /// <summary>
        /// Draw 2D polyline in screen space.
        /// </summary>
        public static void DrawPolylineScreen(this Graphics graphics, Vector3[] vertices, Color color)
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
