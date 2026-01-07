
using Runtime.Calc;
using Runtime.Objects;
using Runtime.Physics.Raycasts;
using Runtime.Scenes;

namespace Runtime.Component.Physics
{

    /// <summary>
    /// A collider, come on, you got this!
    /// </summary>
    public abstract class ICollider : IComponent
    {
        public override void OnLoad()
        {
            alwaysUpdate = true;
            Scene.main?.physicsSolver.colliders.Add(this);
            base.OnLoad();
        }
        public abstract bool HasOverlap(Vector3 point);
        public abstract bool HasOverlap(ICollider collider);
        public abstract float Raycast(Raycast raycast);
    }
}
