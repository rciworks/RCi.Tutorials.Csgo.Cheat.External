using System.Runtime.InteropServices;
using Microsoft.DirectX;

namespace RCi.Tutorials.Csgo.Cheat.External.Data.Raw
{
    /// <summary>
    /// https://github.com/ValveSoftware/source-sdk-2013/blob/0d8dceea4310fde5706b3ce1c70609d72a38efdf/sp/src/public/studio.h#L2050
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct studiohdr_t
    {
        public int id;
        public int version;
        public int checksum;
        public fixed byte name[64];
        public int length;
        public Vector3 eyeposition;
        public Vector3 illumposition;
        public Vector3 hull_min;
        public Vector3 hull_max;
        public Vector3 view_bbmin;
        public Vector3 view_bbmax;
        public int flags;
        public int numbones;                // bones
        public int boneindex;
        public int numbonecontrollers;      // bone controllers
        public int bonecontrollerindex;
        public int numhitboxsets;
        public int hitboxsetindex;
    }
}
