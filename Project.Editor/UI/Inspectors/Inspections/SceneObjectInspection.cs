using ImGuiNET;
using Project.Editor.Components;
using Runtime.Components.Core;
using Runtime.Input;
using Runtime.Objects;
using Runtime.Scenes;

namespace Project.Editor.UI.Inspectors.Inspections
{
    internal class SceneObjectInspection : Inspection
    {
        GameObject target;
        public SceneObjectInspection(GameObject gameObject)
        {
            target = gameObject;
        }

        public override void Render()
        {
            ImGui.Text(target.GetAsset()?.GetName());
            if (ImGui.Button("Delete") || Keyboard.current.IsPressedThisFrame(Key.Delete) || Keyboard.current.IsPressedThisFrame(Key.Backspace))
            {
                Scene.main.DestroyObject(target);
                InspectorWindow.GetActive().SetInspection(null);
            }

            if (ImGui.Button("Move") || Keyboard.current.IsPressedThisFrame(Key.M))
            {
                target.AddComponent(new CasualPlace()
                {
                    startPos = target.GetComponent<Transform>().position
                });
            }

            foreach (Component component in target.GetComponents())
            {
                component.DrawGizmos();
                ImGui.Text(component.GetType().Name);
            }
        }
    }
}
