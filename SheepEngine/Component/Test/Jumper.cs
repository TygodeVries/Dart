using Runtime.Component.Physics;
using Runtime.Input;
using Runtime.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Component.Test
{
    internal class Jumper : IComponent
    {
        public override void Update()
        {
            if(Keyboard.current.IsPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Space))
            {
                GetComponent<Rigidbody>().velocity = new OpenTK.Mathematics.Vector3(0, 10, 0);
            }
        }
    }
}
