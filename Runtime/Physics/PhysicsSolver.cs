using Runtime.Calc;
using Runtime.Components.Physics;
using Runtime.Physics.Raycasts;

namespace Runtime.Physics
{
    public class PhysicsSolver
    {
        public List<ICollider> colliders = new List<ICollider>();

        /// <summary>
        /// If the given collider has overlap with any other colliders
        /// </summary>
        /// <param name="collider">The collider to test for</param>
        /// <returns>true if there is overlap, false if not</returns>
        public bool HasAnyOverlap(ICollider collider)
        {
            foreach (ICollider other in colliders)
            {
                if (other == null) continue;
                if (other == collider) continue;

                if (other.HasOverlap(collider))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Shoot the ray
        /// </summary>
        /// <returns>The result if something was hit, null if nothing was hit</returns>
        public RaycastResult? ShootRaycast(Raycast raycast)
        {
            float closestDistance = -1;
            ICollider? closestCollider = null;
            Vector3 surfaceNormal = new Vector3();

            foreach (ICollider collider in colliders)
            {
                if (raycast.ignore.Contains(collider))
                    continue;

                var result = collider.Raycast(raycast);

                // We missed!
                if (result.distance < 0)
                    continue;

                if (result.distance < closestDistance || closestDistance < 0)
                {
                    closestCollider = collider;
                    closestDistance = result.distance;
                    surfaceNormal = result.normal;
                }
            }

            if (closestCollider == null)
            {
                return null;
            }

            return new RaycastResult(
                distance: closestDistance!,
                collider: closestCollider!,
                hit: (raycast.position + (raycast.direction * closestDistance))!,
                surfaceNormal: surfaceNormal
            );
        }
    }
}
