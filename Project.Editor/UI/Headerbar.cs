using ImGuiNET;
using Project.Editor.UI.FileSystem;
using Project.Editor.UI.Generic;
using Project.Editor.UI.Inspectors;
using Runtime.DearImGUI.Gui;

namespace Project.Editor.UI
{
    /// <summary>
    /// The nav bar is the ui at the top of the screen.
    /// </summary>
    internal class Headerbar : GuiWindow
    {

        public Headerbar()
        {
            WriteHeaderAndFooter = false;
        }

        /// <summary>
        /// Render the NavBarUI with ImGui.
        /// </summary>
        public override void Render()
        {
            ImGui.BeginMainMenuBar();

            DrawPlayButton();
            Window();
            Test();
            ImGui.EndMainMenuBar();
        }

        private void Test()
        {
            if (ImGui.BeginMenu("Test"))
            {
                if (ImGui.MenuItem("Asset Selector"))
                {
                    GuiWindow.Enable(new AssetSelectorWindow(".png"));
                }

                ImGui.EndMenu();
            }
        }

        private void Window()
        {
            if (ImGui.BeginMenu("Window"))
            {
                if (ImGui.MenuItem("Inspector"))
                {
                    GuiWindow.Enable(new InspectorWindow());
                }

                if (ImGui.MenuItem("Project"))
                {
                    GuiWindow.Enable(new ProjectWindow());
                }

                ImGui.EndMenu();
            }
        }

        /// <summary>
        /// Draw the correct button based on the current game state.
        /// </summary>
        private void DrawPlayButton()
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
