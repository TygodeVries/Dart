using Runtime.Component.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Physics
{
    public class PhysicsSolver
    {
        public List<ICollider> colliders = new List<ICollider>();

        public bool HasAnyOverlap(ICollider collider)
        {
            foreach(ICollider other in colliders)
            {
                if (other == null) continue;
                if(other == collider) continue;

                if (other.HasOverlap(collider))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
