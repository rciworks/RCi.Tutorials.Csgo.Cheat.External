using System.Runtime.InteropServices;

namespace RCi.Tutorials.Csgo.Cheat.External.Data.Raw
{
    /// <summary>
    /// https://github.com/ValveSoftware/source-sdk-2013/blob/0d8dceea4310fde5706b3ce1c70609d72a38efdf/sp/src/public/mathlib/mathlib.h#L237
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct matrix3x4_t
    {
        public float m00; // xAxis.x
        public float m10; // yAxis.x
        public float m20; // zAxis.x
        public float m30; // vecOrigin.x

        public float m01; // xAxis.y
        public float m11; // yAxis.y
        public float m21; // zAxis.y
        public float m31; // vecOrigin.y

        public float m02; // xAxis.z
        public float m12; // yAxis.z
        public float m22; // zAxis.z
        public float m32; // vecOrigin.z
    }
}
