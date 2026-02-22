using ImGuiNET;
using Runtime.DearImGUI.Gui;

namespace Project.Editor.UI.Build_Mode
{
    public class BuildBar : GuiWindow
    {
        public BuildBar()
        {
            WriteHeaderAndFooter = false;
        }
        public override void Render()
        {
            var viewport = ImGui.GetMainViewport();

            float height = 25f;

            ImGui.SetNextWindowPos(new System.Numerics.Vector2(
                viewport.WorkPos.X,
                viewport.WorkPos.Y + viewport.WorkSize.Y - height));

            ImGui.SetNextWindowSize(new System.Numerics.Vector2(
                viewport.WorkSize.X,
                height));

            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);

            ImGui.Begin("BottomBar",
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoSavedSettings |
                ImGuiWindowFlags.MenuBar);

            if (ImGui.BeginMenuBar())
            {
                ImGui.Button("Technical");
                ImGui.Button("Scenery");

                ImGui.EndMenuBar();
            }

            ImGui.End();

            ImGui.PopStyleVar(2);
        }
    }
}
