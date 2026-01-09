using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Inspectors.Inspections;
using Project.Editor.UI.Scenes;
using Runtime.Components.Core;
using Runtime.Input;
using Runtime.Objects;
using Runtime.Physics.Raycasts;

namespace Project.Editor.Components
{
    internal class SceneObjectInspectable : Component
    {
        public SceneObjectInspectable()
        {
            this.AlwaysUpdate = true;
        }

        public override void Update()
        {
            if (Mouse.current.LeftPressedThisFrame() && !SceneEditor.IsPlacing())
            {
                Raycast raycast = Camera.main.GetRaycastFromMouse();
                if (raycast == null)
                    return;

                RaycastResult result = raycast.CastInMainScene();
                if (result == null)
                    return;

                if (result.collider.gameObject == gameObject)
                {
                    InspectorWindow.GetActive().SetInspection(new SceneObjectInspection(gameObject));
                }

            }
        }
    }
}
