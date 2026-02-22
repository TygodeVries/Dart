using ImGuiNET;
using Project.Editor.UI.Scenes;
using Runtime;
using Runtime.Data;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics;
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
            int count = 0;
            int itemsPerRow = (int)Math.Floor(ImGui.GetWindowWidth() / 100f) - 2;
            List<Asset> assets = Game.GetAssetDatabase().GetAllAssetsOfType(".prefab");
            foreach (Asset asset in assets)
            {
                Asset icon = IconCache.GetIconForAsset(asset);
                bool useIcon = true;

                if (!File.Exists(icon.GetSystemPath()))
                    useIcon = false;

                ImageTexture imageTexture = DefaultsTextures.GetFallbackTexture();

                if (useIcon)
                    imageTexture = ImageTexture.LoadFromPng(icon);


                if (ImGui.ImageButton($"{asset.GetSystemPath()}", imageTexture.Handle, new System.Numerics.Vector2(100, 100)))
                {

                    GameObject gm = GameObject.LoadFromFile(asset);

                    if (gm == null)
                    {
                        Debug.Error("Gameobject could not be loaded!");
                        return;
                    }

                    SceneEditor.PlaceObject(gm);

                    Scene.main.SaveToFile(Scene.main.GetAsset());
                }
                count++;

                if (count % itemsPerRow != 0)
                    ImGui.SameLine();
            }
        }
    }
}
