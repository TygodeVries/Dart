using ImGuiNET;
using Runtime.DearImGUI.Gui;
using Runtime.Objects;

namespace Project.Editor.UI.Inspectors.Inspections
{
    public class GameObjectInspection : Inspection
    {
        GameObject gameObject;
        public GameObjectInspection(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public override void Render()
        {
            foreach (IComponent component in gameObject.GetComponents())
            {
                if (ImGui.CollapsingHeader(component.GetType().Name))
                {
                    if (ImGui.Button("Remove"))
                    {
                        Type componentType = component.GetType();
                        gameObject.RemoveComponent(componentType);
                    }
                }
            }

            if (ImGui.Button("Add Component"))
            {
                ComponentSelectorWindow guiWindow = new ComponentSelectorWindow();
                GuiWindow.Enable(guiWindow);

                guiWindow.OnComponentPicked += (Type type) =>
                {
                    gameObject.AddComponent((IComponent)Activator.CreateInstance(type));
                    GuiWindow.Disable(guiWindow);
                };
            }
        }
    }
}
