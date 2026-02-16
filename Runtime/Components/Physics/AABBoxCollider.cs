using Runtime.Calc;
using Runtime.Components.Core;
using Runtime.Graphics.Pipeline;
using Runtime.Objects;
using Runtime.Physics.Raycasts;

namespace Runtime.Components.Physics
{

    /// <summary>
    /// An axis allinged bounding box
    /// </summary>
    public class AABBoxCollider : ICollider
    {
        public override void DrawGizmos()
        {
            Vector3 center = GetCenter();
            Vector3 half = size * 0.5f;

            Vector3 min = center - half;
            Vector3 max = center + half;

            Vector4 c000 = new(min.x, min.y, min.z, 1);
            Vector4 c001 = new(min.x, min.y, max.z, 1);
            Vector4 c010 = new(min.x, max.y, min.z, 1);
            Vector4 c011 = new(min.x, max.y, max.z, 1);

            Vector4 c100 = new(max.x, min.y, min.z, 1);
            Vector4 c101 = new(max.x, min.y, max.z, 1);
            Vector4 c110 = new(max.x, max.y, min.z, 1);
            Vector4 c111 = new(max.x, max.y, max.z, 1);

            var gizmo = GizmoRenderPass.GetInstance();

            // Bottom face
            gizmo.AddLine(c000, c100);
            gizmo.AddLine(c100, c101);
            gizmo.AddLine(c101, c001);
            gizmo.AddLine(c001, c000);

            // Top face
            gizmo.AddLine(c010, c110);
            gizmo.AddLine(c110, c111);
            gizmo.AddLine(c111, c011);
            gizmo.AddLine(c011, c010);

            // Vertical edges
            gizmo.AddLine(c000, c010);
            gizmo.AddLine(c100, c110);
            gizmo.AddLine(c101, c111);
            gizmo.AddLine(c001, c011);
        }

        /// <summary>
        /// The center of the bounding box
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCenter()
        {
            Transform? transform = GetComponent<Transform>();
            if (transform != null)
                return transform.position + offset;
            return Vector3.Zero + offset;
        }

        [Inspectable] public Vector3 offset = new Vector3(0, 0, 0);

        /// <summary>
        /// The size of the bouding box
        /// </summary>
        [Inspectable] public Vector3 size;

        /// <summary>
        /// If a spesific point overlaps the collider
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override bool HasOverlap(Vector3 point)
        {
            Vector3 center = GetCenter();
            return point.x < center.x + (size.x * 0.5f) && point.x > center.x - (size.x * 0.5f) &&
                point.y < center.y + (size.y * 0.5f) && point.y > center.y - (size.y * 0.5f) &&
                point.z < center.z + (size.z * 0.5f) && point.z > center.z - (size.z * 0.5f);
        }

        /// <summary>
        /// Check if another collider is overlapping
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool HasOverlap(ICollider other)
        {
            Vector3 centerA = GetCenter();
            Vector3 centerB = ((AABBoxCollider)other).GetCenter();

            Vector3 sizeA = size * 0.5f;
            Vector3 sizeB = ((AABBoxCollider)other).size * 0.5f;

            bool overlapX = centerA.x - sizeA.x < centerB.x + sizeB.x &&
                            centerA.x + sizeA.x > centerB.x - sizeB.x;

            bool overlapY = centerA.y - sizeA.y < centerB.y + sizeB.y &&
                            centerA.y + sizeA.y > centerB.y - sizeB.y;

            bool overlapZ = centerA.z - sizeA.z < centerB.z + sizeB.z &&
                            centerA.z + sizeA.z > centerB.z - sizeB.z;

            return overlapX && overlapY && overlapZ;
        }

        /// <summary>
        /// Cast the ray at JUST this collider, ignoring all others in the scene.
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public override (float distance, Vector3 normal) Raycast(Raycast ray)
        {
            Vector3 center = GetCenter();
            Vector3 min = center - (size / 2);
            Vector3 max = center + (size / 2);

            float tmin = float.NegativeInfinity;
            float tmax = float.PositiveInfinity;

            Vector3 hitNormal = Vector3.Zero;

            for (int i = 0; i < 3; i++)
            {
                float origin = ray.position[i];
                float dir = ray.direction[i];
                float slabMin = min[i];
                float slabMax = max[i];

                if (MathF.Abs(dir) < 0.0001f)
                {
                    if (origin < slabMin || origin > slabMax)
                        return (-1, Vector3.Zero);
                }
                else
                {
                    float t1 = (slabMin - origin) / dir;
                    float t2 = (slabMax - origin) / dir;

                    float enterT = t1;
                    float exitT = t2;
                    int sign = -1;

                    if (t1 > t2)
                    {
                        (t1, t2) = (t2, t1);
                        sign = 1;
                    }

                    if (t1 > tmin)
                    {
                        tmin = t1;

                        hitNormal = Vector3.Zero;
                        hitNormal[i] = sign;
                    }

                    tmax = MathF.Min(tmax, t2);

                    if (tmin > tmax)
                        return (-1, Vector3.Zero);
                }
            }

            if (tmin >= 0)
                return (tmin, hitNormal);

            return (tmax, Vector3.Zero - hitNormal);
        }


    }
}
