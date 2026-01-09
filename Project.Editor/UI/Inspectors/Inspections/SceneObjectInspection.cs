using ImGuiNET;
using OpenTK.Windowing.GraphicsLibraryFramework;
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
            if (ImGui.Button("Delete") || Keyboard.current.IsPressedThisFrame(Keys.Delete) || Keyboard.current.IsPressedThisFrame(Keys.Backspace))
            {
                Scene.main.DestroyObject(target);
                InspectorWindow.GetActive().SetInspection(null);
            }

            if (ImGui.Button("Move") || Keyboard.current.IsPressedThisFrame(Keys.M))
            {
                target.AddComponent(new CasualPlace()
                {
                    startPos = target.GetComponent<Transform>().position
                });
            }
        }
    }
}
