using OpenTK.Mathematics;
using Runtime.Objects;
using Runtime.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Component.Physics
{

    /// <summary>
    /// A collider, come on, you got this!
    /// </summary>
    public abstract class ICollider : IComponent
    {
        public override void OnLoad()
        {
            Scene.main?.physicsSolver.colliders.Add(this);
            base.OnLoad();
        }
        public abstract bool HasOverlap(Vector3 point);
        public abstract bool HasOverlap(ICollider collider);
    }
}
