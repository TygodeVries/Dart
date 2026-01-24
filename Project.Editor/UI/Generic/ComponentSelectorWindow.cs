using ImGuiNET;
using Runtime.DearImGUI.Gui;
using Runtime.Logging;
using Runtime.Objects;

namespace Project.Editor.UI.Generic
{
    internal class ComponentSelectorWindow : GuiWindow
    {
        public Action<Type> OnComponentPicked;
        public override void Render()
        {
            if (OnComponentPicked == null)
            {
                Debug.Warning("Closed Component Selector Window, no action was provided");
                GuiWindow.Disable(this);
            }

            foreach (Type type in GetAllComponentTypes())
            {
                if (ImGui.Button(type.Name))
                {
                    OnComponentPicked.Invoke(type);
                }
            }
        }

        private List<Type> GetAllComponentTypes()
        {
            // Maybe cache this?
            return AppDomain.CurrentDomain.GetAssemblies()
               .SelectMany(s => s.GetTypes())
               .Where(p => typeof(Component).IsAssignableFrom(p))
               .ToList();
        }
    }
}
