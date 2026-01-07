using Project.Editor.UI.Scenes;
using Runtime.Calc;
using Runtime.Component.Core;
using Runtime.Component.Physics;
using Runtime.Input;
using Runtime.Objects;
using Runtime.Physics.Raycasts;
using Runtime.Scenes;

namespace Project.Editor.Components
{
    public class CasualPlace : IComponent
    {
        public CasualPlace()
        {
            this.alwaysUpdate = true;
        }
        public override void Update()
        {
            if (Keyboard.current.IsPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Z))
            {
                Rotate();
            }
            else
            {
                Default();
            }

            if (Mouse.current.LeftPressedThisFrame())
            {
                gameObject.RemoveComponent(this.GetType());
                Scene.main.SaveToFile(Scene.main.GetAsset());
                SceneEditor.FinishedPlace();
            }
        }

        private void Rotate()
        {
            GetComponent<Transform>().Rotate(new Vector3(0, Mouse.current.mouseDelta.x, 0));
        }

        private void Default()
        {
            Raycast raycast = Camera.main.GetRaycastFromMouse();
            raycast.ignore.Add(GetComponent<ICollider>());

            RaycastResult? result = raycast.CastInMainScene();
            if (result != null)
            {
                GetComponent<Transform>().position = result.hit;
            }
        }
    }
}
