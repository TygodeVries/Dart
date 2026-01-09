using ImGuiNET;
using Project.Editor.UI.Assets;
using Project.Editor.UI.FileSystem;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.ProjectSetting;
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
            ImGui.EndMainMenuBar();
        }

        private void Window()
        {
            if (ImGui.BeginMenu("Window"))
            {
                if (ImGui.MenuItem("Object Inspector"))
                {
                    GuiWindow.Enable(new InspectorWindow());
                }

                if (ImGui.MenuItem("Project Explorer"))
                {
                    GuiWindow.Enable(new ProjectWindow());
                }

                if (ImGui.MenuItem("Asset Browser"))
                {
                    GuiWindow.Enable(new AssetBrowser());
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Edit"))
            {
                if (ImGui.MenuItem("Project Settings"))
                {
                    GuiWindow.Enable(new ProjectSettingsWindow());
                }

                ImGui.EndMenu();
            }
        }

        /// <summary>
        /// Draw the correct button based on the current game state.
        /// </summary>
        private void DrawPlayButton()
        {
            if (EditorUtils.IsGameRunning())
            {
                if (ImGui.Button("Play Game"))
                {
                    EditorUtils.StartGame();
                }
            }
            else
            {
                if (ImGui.Button("Stop Game"))
                {
                    EditorUtils.StopGame();
                }
            }
        }
    }
}
