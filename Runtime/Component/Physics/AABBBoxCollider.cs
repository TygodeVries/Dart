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
        public Vector3 Size;

        /// <summary>
        /// If a spesific point overlaps the collider
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override bool HasOverlap(Vector3 point)
        {
            Vector3 center = GetCenter();
            return point.X < center.X + (Size.X * 0.5f) && point.X > center.X - (Size.X * 0.5f) &&
                point.Y < center.Y + (Size.Y * 0.5f) && point.Y > center.Y - (Size.Y * 0.5f) &&
                point.Z < center.Z + (Size.Z * 0.5f) && point.Z > center.Z - (Size.Z * 0.5f);
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

            Vector3 sizeA = Size * 0.5f;
            Vector3 sizeB = ((AABBBoxCollider)other).Size * 0.5f;

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
            Vector3 boxMin = -(Size / 2);
            Vector3 boxMax = Size / 2;

            Transform? transform = GetComponent<Transform>();

            if (transform != null)
            {
                boxMax += transform.position;
                boxMax += transform.position;
            }

            float t1 = (boxMin.X - ray.position.X) / ray.direction.X;
            float t2 = (boxMax.X - ray.position.X) / ray.direction.X;
            float t3 = (boxMin.Y - ray.position.Y) / ray.direction.Y;
            float t4 = (boxMax.Y - ray.position.Y) / ray.direction.Y;
            float t5 = (boxMin.Z - ray.position.Z) / ray.direction.Z;
            float t6 = (boxMax.Z - ray.position.Z) / ray.direction.Z;

            float tmin = MathF.Max(MathF.Max(MathF.Min(t1, t2), MathF.Min(t3, t4)), MathF.Min(t5, t6));
            float tmax = MathF.Min(MathF.Min(MathF.Max(t1, t2), MathF.Max(t3, t4)), MathF.Max(t5, t6));

            if (tmax < 0)
            {
                return -1;
            }

            if (tmin > tmax)
            {
                return -1;
            }

            if (tmin < 0f)
            {
                return tmax;
            }
            return tmin;
        }
    }
}
