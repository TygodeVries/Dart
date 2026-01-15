using ImGuiNET;
using Project.Editor.UI.Generic;
using Runtime;
using Runtime.Data;
using Runtime.DearImGUI.Gui;

namespace Project.Editor.UI.ProjectSetting
{
    public class GameSettingsWindow : GuiWindow
    {
        public override string GetName()
        {
            return "Game Settings";
        }

        public override void Render()
        {
            string? startScene = GameSettings.GetGameSettings().StartScene;
            if (startScene == null)
            {
                startScene = "None";
            }
            if (ImGui.Button(startScene))
            {
                AssetSelectorWindow window = new AssetSelectorWindow(".scene", Game.GetAssetDatabase());
                GuiWindow.Enable(window);
                window.OnSelect += (AssetSelectionResult result) =>
                {
                    GameSettings.GetGameSettings().StartScene = result.asset.GetPath();
                };
            }
        }
    }
}
