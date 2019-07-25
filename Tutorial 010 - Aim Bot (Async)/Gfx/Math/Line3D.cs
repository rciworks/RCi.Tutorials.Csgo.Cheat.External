using Microsoft.DirectX;

namespace RCi.Tutorials.Csgo.Cheat.External.Gfx.Math
{
    /// <summary>
    /// Line in 3d world.
    /// http://geomalgorithms.com/a07-_distance.html
    /// http://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html
    /// </summary>
    public readonly struct Line3D
    {
        #region // storage

        /// <summary>
        /// Start and end points.
        /// </summary>
        public readonly Vector3 StartPoint, EndPoint;

        #endregion

        #region // ctor

        /// <summary />
        public Line3D(Vector3 startPoint, Vector3 endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        #endregion

        #region // routines

        /// <summary>
        /// Get closest points between lines.
        /// </summary>
        public (Vector3, Vector3) ClosestPointsBetween(Line3D other)
        {
            if (IsParallelTo(other))
            {
                return (StartPoint, other.ClosestPointTo(StartPoint, false));
            }
            var u = GetDirection();
            var v = other.GetDirection();
            var w = StartPoint - other.StartPoint;
            var uu = u.Dot(u);
            var uv = u.Dot(v);
            var vv = v.Dot(v);
            var uw = u.Dot(w);
            var vw = v.Dot(w);
            var sc = (uv * vw - vv * uw) / (uu * vv - uv * uv);
            var tc = (uu * vw - uv * uw) / (uu * vv - uv * uv);
            return (StartPoint + sc * u, other.StartPoint + tc * v);
        }

        /// <summary>
        /// Get closes points between lines.
        /// </summary>
        public (Vector3, Vector3) ClosestPointsBetween(Line3D other, bool mustBeOnSegments)
        {
            if (!IsParallelTo(other) || !mustBeOnSegments)
            {
                var pair = ClosestPointsBetween(other);
                if (!mustBeOnSegments)
                {
                    return pair;
                }

                if ((pair.Item1 - StartPoint).Length() <= GetLength() &&
                    (pair.Item1 - EndPoint).Length() <= GetLength() &&
                    (pair.Item2 - other.StartPoint).Length() <= other.GetLength() &&
                    (pair.Item2 - other.EndPoint).Length() <= other.GetLength())
                {
                    return pair;
                }
            }

            var checkPoint = other.ClosestPointTo(StartPoint, true);
            var distance = (checkPoint - StartPoint).Length();
            var closestPair = (StartPoint, checkPoint);
            var minDistance = distance;

            checkPoint = other.ClosestPointTo(EndPoint, true);
            distance = (checkPoint - EndPoint).Length();
            if (distance < minDistance)
            {
                closestPair = (EndPoint, checkPoint);
                minDistance = distance;
            }

            checkPoint = ClosestPointTo(other.StartPoint, true);
            distance = (checkPoint - other.StartPoint).Length();
            if (distance < minDistance)
            {
                closestPair = (checkPoint, other.StartPoint);
                minDistance = distance;
            }

            checkPoint = ClosestPointTo(other.EndPoint, true);
            distance = (checkPoint - other.EndPoint).Length();
            if (distance < minDistance)
            {
                closestPair = (checkPoint, other.EndPoint);
            }

            return closestPair;
        }

        /// <summary>
        /// Get closest point to other point.
        /// </summary>
        public Vector3 ClosestPointTo(Vector3 value, bool mustBeOnSegment)
        {
            var direction = GetDirection();
            var dotProduct = (value - StartPoint).Dot(direction);

            if (mustBeOnSegment)
            {
                if (dotProduct < 0)
                {
                    dotProduct = 0;
                }

                var length = GetLength();
                if (dotProduct > length)
                {
                    dotProduct = length;
                }
            }

            return StartPoint + dotProduct * direction;
        }

        /// <summary>
        /// Get direction of the line.
        /// </summary>
        public Vector3 GetDirection()
        {
            return (EndPoint - StartPoint).Normalized();
        }

        /// <summary>
        /// Get length of a line.
        /// </summary>
        public float GetLength()
        {
            return (EndPoint - StartPoint).Length();
        }

        /// <summary>
        /// Is line parallel to other line?
        /// </summary>
        public bool IsParallelTo(Line3D other)
        {
            return GetDirection().IsParallelTo(other.GetDirection());
        }

        #endregion
    }
}
