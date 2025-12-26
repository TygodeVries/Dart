using ImGuiNET;

namespace Runtime.DearImGUI.Gui
{
    internal class GuiDemoWindow : GuiWindow
    {

        string val = "Test";
        public override void Render()
        {
            if (ImGui.Button("Click Me!"))
            {
                ImGui.Text("Hello!");
            }

            ImGui.InputText("Lable", ref val, 100);
        }
    }
}
