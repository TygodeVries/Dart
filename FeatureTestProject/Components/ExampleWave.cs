using Runtime.Calc;
using Runtime.Component.Core;
using Runtime.Component.Physics;
using Runtime.Input;
using Runtime.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureTestProject.Components
{
    internal class ExampleWave : IComponent
    {

        double timer = 0;
        public override void Update()
        {
         
            if(Keyboard.current.IsPressedThisFrame(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Up))
            {
                GetComponent<Rigidbody>().velocity = new OpenTK.Mathematics.Vector3(0, 4, 0);
            }
        }
    }
}
