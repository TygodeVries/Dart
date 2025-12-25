using Runtime.Component.Physics;
using Runtime.Input;
using Runtime.Objects;

namespace Runtime.Component.Test
{
    /// <summary>
    /// A test script that adds an upwards force to any attached rigid bodies.
    /// </summary>
    internal class Jumper : IComponent
    {
        public override void Update()
        {
            if (Keyboard.current.IsPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Space))
            {
                GetComponent<Rigidbody>().velocity = new Calc.Vector3(0, 10, 0);
            }
        }
    }
}
