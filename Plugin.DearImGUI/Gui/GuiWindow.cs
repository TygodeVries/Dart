using ImGuiNET;
using Runtime.DearImGUI.Backend;
using Runtime.Logging;

namespace Runtime.DearImGUI.Gui
{
    public abstract class GuiWindow
    {
        public bool WriteHeaderAndFooter = true;
        private int? id;
        public int GetId()
        {
            return id.Value;
        }

        internal void InitId()
        {
            id = 0;
            foreach (GuiWindow guiWindow in ImGuiRenderPass.guiWindows)
            {
                if (this.GetName() == guiWindow.GetName())
                {
                    id++;
                }
            }
        }

        internal static Queue<GuiWindow> windowsToOpen = new Queue<GuiWindow>();
        internal static Queue<GuiWindow> windowsToClose = new Queue<GuiWindow>();
        public static void Enable(GuiWindow window)
        {
            windowsToOpen.Enqueue(window);
        }

        private static List<string> restore = new List<string>();
        public static void DisableAll()
        {
            restore.Clear();
            foreach (GuiWindow window in ImGuiRenderPass.guiWindows)
            {
                restore.Add(window.GetType().AssemblyQualifiedName);
                Disable(window);
            }
        }

        public static void RestoreAll()
        {
            foreach (string win in restore)
            {
                Debug.Log($"Attempting to restore window: {win}");
                Type type = UserCode.GetTypeOf(win);
                GuiWindow? window = (GuiWindow)Activator.CreateInstance(type);
                Enable(window);
            }
        }

        public static void Disable(GuiWindow window)
        {
            windowsToClose.Enqueue(window);
        }

        public abstract void Render();
        public virtual string GetName()
        {
            return "Unnamed Window";
        }

        public bool Begin()
        {
            bool isOpen = true;
            bool visible = ImGui.Begin($"{GetName()}##{GetId()}", ref isOpen);

            if (!isOpen)
            {
                GuiWindow.Disable(this);
                ImGui.End();
            }

            return visible && isOpen;
        }

    }
}
