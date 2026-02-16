
using Runtime.Calc;
using Runtime.Physics.Raycasts;
using Runtime.Scenes;

namespace Runtime.Components.Physics
{

    /// <summary>
    /// A collider, come on, you got this!
    /// </summary>
    public abstract class ICollider : Objects.Component
    {
        public override void Load()
        {
            AlwaysUpdate = true;
            Scene.main?.physicsSolver.colliders.Add(this);
            base.Load();
        }
        public override void Unload()
        {
            Scene.main?.physicsSolver.colliders.Remove(this);
        }

        public abstract bool HasOverlap(Vector3 point);
        public abstract bool HasOverlap(ICollider collider);
        public abstract (float distance, Vector3 normal) Raycast(Raycast raycast);
    }
}
