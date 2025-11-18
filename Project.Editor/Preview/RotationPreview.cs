using Runtime.Calc;
using Runtime.Component.Core;
using Runtime.Input;
using Runtime.Objects;

namespace Project.Editor.Preview
{
    internal class RotationPreview : IComponent
    {
        public override void Update()
        {
            GetComponent<Transform>()?.Rotate(0, (float)Time.deltaTime * 10f, 0);

            if (Mouse.current.leftPressed)
                GetComponent<Transform>()?.Rotate(-Mouse.current.mouseDelta.Y / 2, Mouse.current.mouseDelta.X / 2, 0);
        }
    }
}
