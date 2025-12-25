using ImGuiNET;
using Runtime;
using Runtime.DearImGUI.Gui;

namespace Project.Editor.UI.Generic
{
    public class AssetSelectorWindow : GuiWindow
    {

        List<string> files;
        public AssetSelectorWindow(string filetype)
        {
            files = Game.GetAssetDatabase().GetAllAssetsOfType(filetype);
        }

        private byte[] search = new byte[128];

        public override void Render()
        {
            ImGui.InputText("Search", search, (uint)search.Length);
            ImGui.Text($"Active: {ImGui.IsItemActive()}");

            string searchText = GetString(search);

            foreach (string file in files)
            {
                if (file.ToLower().Contains(searchText.ToLower()))
                {
                    if (ImGui.Button(file))
                    {
                        OnSelect?.Invoke(new AssetSelectionResult(file));
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
        public string FilePath;

        public AssetSelectionResult(string filePath)
        {
            FilePath = filePath;
        }
    }
}
