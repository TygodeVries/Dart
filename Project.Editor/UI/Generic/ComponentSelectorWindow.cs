using ImGuiNET;
using Runtime.DearImGUI.Gui;
using Runtime.Logging;
using Runtime.Objects;
using System.Reflection;

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

            int i = 0;
            foreach (Type type in GetAllComponentTypes())
            {
                i++;
                if (typeof(Component).IsAssignableFrom(type))

                    if (ImGui.Button($"{type.FullName}##{i}"))
                    {
                        OnComponentPicked.Invoke(type);
                    }
            }
        }

        private List<Type> GetAllComponentTypes()
        {

            Assembly[] assemblies = UserCode.GetAllAssemblies();
            List<Type> types = new List<Type>();

            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    types.AddRange(assembly.GetTypes());
                }
                catch (Exception e)
                {
                    Debug.Error($"Failed to get types from assembly {assembly.FullName}. because {e}");
                }
            }

            return types;
        }
    }
}
