using Microsoft.DirectX;

namespace RCi.Tutorials.Csgo.Cheat.External.Gfx.Math
{
    public readonly struct Plane3D
    {
        #region // storage

        /// <summary>
        /// Plane's normal.
        /// </summary>
        public readonly Vector3 Normal;

        /// <summary>
        /// Distance from origin (0,0,0) to plane along its normal.
        /// </summary>
        public readonly float Distance;

        #endregion

        #region // ctor

        /// <summary/>
        public Plane3D(Vector3 normal, float distance)
        {
            Normal = normal.Normalized();
            Distance = distance;
        }

        /// <summary/>
        public Plane3D(Vector3 normal, Vector3 point) :
            this(normal, -normal.Dot(point))
        {
        }

        #endregion

        #region // routines

        /// <summary>
        /// Project point on a plane. 
        /// </summary>
        /// <returns>
        /// planeOrigin - origin of a plane
        /// vector - Vector from plane origin to projected point
        /// </returns>
        public (Vector3 planeOrigin, Vector3 vector) ProjectVector(Vector3 vector)
        {
            var planeOrigin = ProjectPoint(new Vector3());
            return (planeOrigin, ProjectPoint(vector) - planeOrigin);
        }

        /// <summary>
        /// Project point on a plane.
        /// </summary>
        /// <remarks>
        /// "https://en.wikipedia.org/wiki/3D_projection"
        /// "https://en.wikipedia.org/wiki/Projection_(linear_algebra)"
        /// </remarks>
        public Vector3 ProjectPoint(Vector3 point)
        {
            return point - (Normal.Dot(point) + Distance) * Normal;
        }

        #endregion
    }
}
