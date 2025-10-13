using OpenTK.Mathematics;
using Runtime.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Component.Physics
{
    public abstract class ICollider : IComponent
    {
        public abstract bool HasOverlap(Vector3 point);
        public abstract bool HasOverlap(ICollider collider);
    }
}
