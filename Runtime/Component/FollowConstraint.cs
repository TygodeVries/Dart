using Runtime.Calc;
using Runtime.Component.Core;
using Runtime.Logging;
using Runtime.Objects;

namespace Runtime.Component
{
    public class FollowConstraint : IComponent
    {
        public Transform target;
        Transform? me;
        public Vector3 offset;

        /// <summary>
        /// Create FollowConstraint with another transform
        /// </summary>
        /// <param name="transform">The transforms to follow</param>
        public FollowConstraint(Transform transform)
        {
            target = transform;
        }

        public override void OnLoad()
        {
            me = GetComponent<Transform>();
        }

        public override void Update()
        {
            if (me == null)
            {
                Debug.Error("Missing a transform on the FollowConstraint, the object attached did not have a transform");
                return;
            }

            me.position = target.position + offset;
        }
    }
}
