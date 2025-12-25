using Runtime.Calc;
using Runtime.Component.Core;
using Runtime.Input;
using Runtime.Objects;

namespace Project.Editor.Preview
{
    internal class CameraPreview : IComponent
    {
        float scroll = -3;
        public override void Update()
        {
            GetComponent<Transform>()!.position = new Vector3(0, 0, scroll);
            scroll += Mouse.current.scroll.y;
        }
    }
}
