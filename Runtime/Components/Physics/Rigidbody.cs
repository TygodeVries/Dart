using Runtime.Calc;
using Runtime.Components.Core;
using Runtime.Logging;
using Runtime.Scenes;

namespace Runtime.Components.Physics
{
    public class Rigidbody : Objects.Component
    {
        private Transform? transform;
        private ICollider? collider;

        public Vector3 velocity = Vector3.Zero;
        public float gravity = -7f;

        public override void Load()
        {
            transform = GetComponent<Transform>();
            collider = GetComponent<ICollider>();

            if (transform == null)
                Debug.Error("Rigidbody requires Transform!");

            if (collider == null)
                Debug.Warning("Rigidbody has no collider attached.");
        }

        public override void Update()
        {
            if (transform == null)
                return;

            float dt = (float)Time.deltaTime;

            // Apply gravity
            velocity.y += gravity * dt;

            MoveAxisSeparated(velocity * dt);
        }

        private void MoveAxisSeparated(Vector3 delta)
        {
            if (transform == null || collider == null)
            {
                transform!.position += delta;
                return;
            }

            MoveAxis(new Vector3(delta.x, 0, 0), ref velocity.x);

            MoveAxis(new Vector3(0, delta.y, 0), ref velocity.y);

            MoveAxis(new Vector3(0, 0, delta.z), ref velocity.z);
        }

        private void MoveAxis(Vector3 delta, ref float velocityAxis)
        {
            if (transform == null)
                return;

            transform.position += delta;

            if (Scene.main!.physicsSolver.HasAnyOverlap(collider!))
            {
                transform.position -= delta;

                velocityAxis = 0f;
            }
        }
    }
}
