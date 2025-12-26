using ImGuiNET;
using Runtime.DearImGUI.Gui;

namespace Project.Editor.UI
{
    internal class ExampleWindow : GuiWindow
    {

        public override void Render()
        {
            if (ImGui.Button("Send the Message!"))
            {
                Environment.Exit(-1);
            }

        }
    }
}
