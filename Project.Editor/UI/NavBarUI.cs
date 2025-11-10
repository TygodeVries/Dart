using ImGuiNET;
using Runtime.DearImGUI.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor.UI
{
    internal class NavBarUI : GuiWindow
    {
        public override void Render()
        {
            ImGui.SetWindowFontScale(1.1f);
            ImGui.BeginMainMenuBar();

            DrawPlayButton();

            ImGui.EndMainMenuBar();
        }

        public void DrawPlayButton()
        {
            if (Editor.IsGameRunning())
            {
                if (ImGui.Button("Play"))
                {
                    Editor.StartGame();
                }
            }
            else
            {
                if (ImGui.Button("Stop"))
                {
                    Editor.StopGame();
                }
            }
        }
    }
}
