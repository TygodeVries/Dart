using ImGuiNET;
using Runtime.DearImGUI.Gui;

namespace Project.Editor.UI
{
    /// <summary>
    /// The nav bar is the ui at the top of the screen.
    /// </summary>
    internal class NavBarUI : GuiWindow
    {

        /// <summary>
        /// Render the NavBarUI with ImGui.
        /// </summary>
        public override void Render()
        {
            ImGui.SetWindowFontScale(1.1f);
            ImGui.BeginMainMenuBar();

            DrawPlayButton();

            ImGui.EndMainMenuBar();
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
