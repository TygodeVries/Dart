using Runtime.Calc;
using Runtime.Components.Core;
using Runtime.Input;
using Runtime.Objects;

namespace Project.Editor.Preview
{
    internal class RotationPreview : Component
    {
        bool isKeyboard;
        public RotationPreview(bool isKeyboard)
        {
            this.isKeyboard = isKeyboard;
        }

        public override void Update()
        {
            if (!isKeyboard)
            {
                GetComponent<Transform>()?.Rotate(0, (float)Time.deltaTime * 1f, 0);

                if (Mouse.current.leftPressed)
                    GetComponent<Transform>()?.Rotate(-Mouse.current.mouseDelta.y / 2, Mouse.current.mouseDelta.x / 2, 0);
            }
            else
            {
                float xRot = 0;
                if (Keyboard.current.IsPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Left))
                {
                    xRot = -1;
                }

                if (Keyboard.current.IsPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Right))
                {
                    xRot = 1;
                }

                float yRot = 0;
                if (Keyboard.current.IsPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Up))
                {
                    yRot = -1;
                }

                if (Keyboard.current.IsPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Down))
                {
                    yRot = 1;
                }

                GetComponent<Transform>()?.Rotate(yRot * (float)Time.deltaTime * 100, xRot * (float)Time.deltaTime * 100, 0);
            }
        }
    }
}
