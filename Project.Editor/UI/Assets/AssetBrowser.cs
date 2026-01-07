using ImGuiNET;
using Project.Editor.UI.Scenes;
using Runtime;
using Runtime.Data;
using Runtime.DearImGUI.Gui;
using Runtime.Logging;
using Runtime.Objects;
using Runtime.Scenes;

namespace Project.Editor.UI.Assets
{
    public class AssetBrowser : GuiWindow
    {
        public override string GetName()
        {
            return "Asset Browser";
        }

        public override void Render()
        {
            if (!SceneEditor.IsEnabledInCurrentScene())
            {
                ImGui.Text("Sorry!");
                ImGui.Text("This window is only active in the scene editor.");
                return;
            }

            List<Asset> assets = Game.GetAssetDatabase().GetAllAssetsOfType(".prefab");
            foreach (Asset asset in assets)
            {
                if (ImGui.Button(asset.GetName()))
                {
                    GameObject gm = GameObject.LoadFromFile(asset);

                    if (gm == null)
                    {
                        Debug.Error("Gameobject could not be loaded!");
                        return;
                    }
                    Scene.main.Instantiate(gm);
                    Scene.main.SaveToFile(Scene.main.GetAsset());
                }
            }
        }
    }
}
