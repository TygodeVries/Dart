using OpenTK.Mathematics;
using Runtime.Component.Core;
using Runtime.Physics;
using Runtime.Calc;
using Runtime.Objects;
using Runtime.Scenes;
using Runtime.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Component.Physics
{
    public class Rigidbody : IComponent
    {
        public override void Update()
        {
            if (GetComponent<Transform>() == null)
            {
                Console.WriteLine("Rigidbody requires Transform to function!");
                return;
            }

            PhysicsSolver solver = Scene.main!.physicsSolver;
            Move(velocity * (float) Time.deltaTime);
            velocity += new Vector3(0, -7, 0) * (float) Time.deltaTime;
        }

        // The speed in units/second
        public Vector3 velocity = new Vector3(0, 0, 0);

        public void Move(Vector3 delta)
        {
         if (GetComponent<Transform>() is Transform t)
         {
            t.position += delta;
            ICollider? collider = GetComponent<ICollider>();
            if (collider == null)
            {
               Console.WriteLine("Rigidbody has no collider attached!");
               return;
            }
            bool hasAnyOverlap = Scene.main!.physicsSolver.HasAnyOverlap(collider);
            if (hasAnyOverlap)
            {
               // Undo!!
               t.position -= delta;
               velocity = Vector3.Zero;
            }
         }
         else
            Debug.Error("Rigidbody without Transform");
        }
    }
}
