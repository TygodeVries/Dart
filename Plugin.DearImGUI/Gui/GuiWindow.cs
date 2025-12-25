using ImGuiNET;
using Runtime.DearImGUI.Backend;

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

        public static void Disable(GuiWindow window)
        {
            windowsToClose.Enqueue(window);
        }

        public abstract void Render();
        public virtual string GetName()
        {
            return "Unnamed Window";
        }

        public void Begin()
        {
            bool isOpen = true;
            ImGui.Begin($"{GetName()}##{GetId()}", ref isOpen, ImGuiWindowFlags.None);
            if (!isOpen)
            {
                GuiWindow.Disable(this);
                return;
            }
        }
    }
}
