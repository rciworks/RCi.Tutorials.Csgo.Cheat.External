using System.Runtime.InteropServices;
using Microsoft.DirectX;

namespace RCi.Tutorials.Csgo.Cheat.External.Data.Raw
{
    /// <summary>
    /// https://github.com/ValveSoftware/source-sdk-2013/blob/0d8dceea4310fde5706b3ce1c70609d72a38efdf/sp/src/public/studio.h#L420
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct mstudiobbox_t
    {
        public int bone;
        public int group;                   // intersection group
        public Vector3 bbmin;               // bounding box
        public Vector3 bbmax;
        public int szhitboxnameindex;       // offset to the name of the hitbox.
        public fixed int unused[3];
        public float radius;                // when radius is -1 it's box, otherwise it's capsule (cylinder with spheres on the end)
        public fixed int pad[4];
    }
}
