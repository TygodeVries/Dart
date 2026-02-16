using Runtime.Calc;
using Runtime.Components.Physics;

namespace Runtime.Physics.Raycasts
{
    public class RaycastResult
    {
        public float distance;
        public Vector3 hit;
        public ICollider collider;
        public Vector3 surfaceNormal;
        public RaycastResult(float distance, ICollider collider, Vector3 hit, Vector3 surfaceNormal)
        {
            this.distance = distance;
            this.collider = collider;
            this.hit = hit;
            this.surfaceNormal = surfaceNormal;
        }
    }
}
