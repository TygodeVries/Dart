using Runtime.Calc;
using Runtime.Components.Core;
using Runtime.Physics.Raycasts;

namespace Runtime.Components.Physics
{
    public class SphereCollider : ICollider
    {
        public float radius = 1;
        public Vector3 GetCenter()
        {
            Vector3 center = new Vector3(0, 0, 0);
            if (GetComponent<Transform>() is Transform t)
            {
                center = t.position;
            }
            return center;
        }
        public override bool HasOverlap(Vector3 point)
        {
            return (GetCenter() - point).Magnitude() <= radius;
        }

        public override bool HasOverlap(ICollider collider)
        {
            throw new NotImplementedException();
        }
        float Inner(Vector3 a, Vector3 b)
        {
            return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
        }
        public override (float, Vector3) Raycast(Raycast raycast)
        {
            // #TODO add normal
            Vector3 mu = raycast.position - GetCenter();
            Vector3 d = raycast.direction;

            float ud = Inner(mu, d);
            float dd = Inner(d, d);
            float uu = Inner(mu, mu);
            float det = (ud * ud) - (dd * (uu - (radius * radius)));
            if (det < 0)
                return (-1, Vector3.Zero);
            det = MathF.Sqrt(det) / dd;
            float e = -ud;
            float i = e - det;
            if (i >= 0)
                return (i, Vector3.Zero);
            i = e + det;
            return (i, Vector3.Zero);
        }
    }
}
