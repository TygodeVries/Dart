using ImGuiNET;
using Runtime.DearImGUI.Gui;

namespace Project.Editor.UI
{
    internal class DockerWindow : GuiWindow
    {
        public DockerWindow()
        {
            WriteHeaderAndFooter = false;
        }

        public override void Render()
        {
            ImGui.DockSpaceOverViewport();
        }
    }
}
