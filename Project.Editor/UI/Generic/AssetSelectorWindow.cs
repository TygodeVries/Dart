using ImGuiNET;
using Runtime.Data;
using Runtime.DearImGUI.Gui;
using Runtime.Logging;
using System.Reflection;

namespace Project.Editor.UI.Generic
{
    public class AssetSelectorWindow : GuiWindow
    {

        List<Asset> assets;
        public AssetSelectorWindow(string filetype, AssetDatabase assetDatabase)
        {
            Debug.Log("Creating selection window for type " + filetype);
            assets = assetDatabase.GetAllAssetsOfType(filetype);
        }

        public override string GetName()
        {
            return "Select an Asset";
        }

        private byte[] search = new byte[128];

        public override void Render()
        {
            ImGui.InputText("Search", search, (uint)search.Length);
            ImGui.Text($"Active: {ImGui.IsItemActive()}");

            string searchText = GetString(search);

            foreach (Asset asset in assets)
            {
                if (asset.GetPath().ToLower().Contains(searchText.ToLower()))
                {
                    if (ImGui.Button(asset.GetPath()))
                    {
                        OnSelect?.Invoke(new AssetSelectionResult(asset));

                        GuiWindow.Disable(this);
                    }
                }
            }
        }

        private static string GetString(byte[] buffer)
        {
            int length = Array.IndexOf(buffer, (byte)0);
            if (length < 0) length = buffer.Length;
            return System.Text.Encoding.UTF8.GetString(buffer, 0, length);
        }


        public Action<AssetSelectionResult> OnSelect;
    }

    public class AssetSelectionResult
    {
        public Asset asset;

        public AssetSelectionResult(Asset asset)
        {
            this.asset = asset;
        }

        public object CreateInstance(Type type)
        {
            AssetReferenceAttribute att = type.GetCustomAttribute<AssetReferenceAttribute>();
            return att.CreateInstance(type, asset);
        }
    }
}
