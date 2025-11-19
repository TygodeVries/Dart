using Runtime.Component.Physics;
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
            float distance = -1;
            ICollider? closestCollider = null;

            foreach (ICollider collider in colliders)
            {
                float newDistance = collider.Raycast(raycast);

                // We missed!
                if (distance < 0)
                    continue;

                if (newDistance < distance)
                {
                    closestCollider = collider;
                    distance = newDistance;
                }
            }

            if (closestCollider == null)
            {
                return null;
            }

            return new RaycastResult(
                distance: distance!,
                collider: closestCollider!,
                hit: (raycast.position + (raycast.direction * distance))!
            );
        }
    }
}
