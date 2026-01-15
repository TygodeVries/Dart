using Runtime.Graphics.Renderers;
using Runtime.Objects;

namespace Runtime.Tests
{
    internal class TestComponent : Objects.Component
    {
        [Inspectable] bool IsAlive;
        [Inspectable] Mesh asset;
        public TestComponent()
        {

        }
    }
}
