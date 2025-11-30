using OpenTK.Mathematics;
using Runtime.Component.Core;
using Runtime.Physics.Raycasts;

namespace Runtime.Component.Physics
{

    /// <summary>
    /// An axis allinged bounding box
    /// </summary>
    public class AABBBoxCollider : ICollider
    {

        /// <summary>
        /// The center of the bounding box
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCenter()
        {
            Transform? transform = GetComponent<Transform>();
            if (transform != null)
                return transform.position;
            return Vector3.Zero;
        }

        /// <summary>
        /// The size of the bouding box
        /// </summary>
        public Vector3 size;

        /// <summary>
        /// If a spesific point overlaps the collider
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override bool HasOverlap(Vector3 point)
        {
            Vector3 center = GetCenter();
            return point.X < center.X + (size.X * 0.5f) && point.X > center.X - (size.X * 0.5f) &&
                point.Y < center.Y + (size.Y * 0.5f) && point.Y > center.Y - (size.Y * 0.5f) &&
                point.Z < center.Z + (size.Z * 0.5f) && point.Z > center.Z - (size.Z * 0.5f);
        }

        /// <summary>
        /// Check if another collider is overlapping
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool HasOverlap(ICollider other)
        {
            Vector3 centerA = GetCenter();
            Vector3 centerB = ((AABBBoxCollider)other).GetCenter();

            Vector3 sizeA = size * 0.5f;
            Vector3 sizeB = ((AABBBoxCollider)other).size * 0.5f;

            bool overlapX = centerA.X - sizeA.X < centerB.X + sizeB.X &&
                            centerA.X + sizeA.X > centerB.X - sizeB.X;

            bool overlapY = centerA.Y - sizeA.Y < centerB.Y + sizeB.Y &&
                            centerA.Y + sizeA.Y > centerB.Y - sizeB.Y;

            bool overlapZ = centerA.Z - sizeA.Z < centerB.Z + sizeB.Z &&
                            centerA.Z + sizeA.Z > centerB.Z - sizeB.Z;

            return overlapX && overlapY && overlapZ;
        }

        /// <summary>
        /// Cast the ray at JUST this collider, ignoring all others in the scene.
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public override float Raycast(Raycast ray)
        {
            Transform? transform = GetComponent<Transform>();
            Vector3 center = transform?.position ?? Vector3.Zero;
            Vector3 min = center - (size / 2);
            Vector3 max = center + (size / 2);

            float tmin = float.NegativeInfinity;
            float tmax = float.PositiveInfinity;

            for (int i = 0; i < 3; i++)
            {
                float origin = ray.position[i];
                float dir = ray.direction[i];
                float slabMin = min[i];
                float slabMax = max[i];

                if (MathF.Abs(dir) < 0.0001f)
                {
                    if (origin < slabMin || origin > slabMax)
                        return -1;
                }
                else
                {
                    float t1 = (slabMin - origin) / dir;
                    float t2 = (slabMax - origin) / dir;

                    if (t1 > t2)
                    {
                        float tmp = t1;
                        t1 = t2;
                        t2 = tmp;
                    }

                    tmin = MathF.Max(tmin, t1);
                    tmax = MathF.Min(tmax, t2);

                    if (tmin > tmax)
                        return -1;
                }
            }

            return tmin >= 0 ? tmin : tmax;
        }

    }
}
