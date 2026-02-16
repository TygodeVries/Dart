using Project.Editor.UI.Scenes;
using Runtime.Calc;
using Runtime.Components.Core;
using Runtime.Components.Physics;
using Runtime.Graphics.Pipeline;
using Runtime.Input;
using Runtime.Objects;
using Runtime.Physics.Raycasts;
using Runtime.Scenes;

namespace Project.Editor.Components
{
    public class CasualPlace : Component
    {
        internal Vector3? startPos;
        public CasualPlace()
        {
            this.AlwaysUpdate = true;
        }

        public override void Load()
        {
            transform = GetComponent<Transform>()!;
        }

        public override void Update()
        {
            if (Keyboard.current.IsPressed(Key.Z))
            {
                Rotate();
            }
            else if (Keyboard.current.IsPressed(Key.LeftShift))
            {
                Vertical();
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

            if (Keyboard.current.IsPressedThisFrame(Key.Escape))
            {
                if (startPos != null)
                {
                    transform.position = startPos.Value;

                    gameObject.RemoveComponent(this.GetType());
                    Scene.main.SaveToFile(Scene.main.GetAsset());
                    SceneEditor.FinishedPlace();
                }
                else
                {
                    SceneEditor.CancelPlace();
                }
            }
        }

        Transform transform;
        private void Vertical()
        {
            GizmoRenderPass.GetInstance().AddLine(new Vector4(lastDefaultPosisiton, 1), new Vector4(transform.position, 1));
            transform.position.y += -Mouse.current.mouseDelta.y * 0.01f;
        }

        private void Rotate()
        {
            transform.Rotate(new Vector3(0, Mouse.current.mouseDelta.x, 0));
        }


        Vector3 lastDefaultPosisiton;
        private void Default()
        {
            Raycast? raycast = Camera.main?.GetRaycastFromMouse();
            raycast?.ignore.Add(GetComponent<ICollider>());

            RaycastResult? result = raycast?.CastInMainScene();
            if (result != null)
            {
                lastDefaultPosisiton = result.hit;
                transform.position = result.hit;
            }
        }
    }
}
