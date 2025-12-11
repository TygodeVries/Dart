using Runtime.Calc;
using Runtime.Component.Physics;

namespace Runtime.Physics.Raycasts
{
    public class RaycastResult
    {
        public float distance;
        public Vector3 hit;
        public ICollider collider;

        public RaycastResult(float distance, ICollider collider, Vector3 hit)
        {
            this.distance = distance;
            this.collider = collider;
            this.hit = hit;
        }
    }
}
