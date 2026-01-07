using ImGuiNET;
using Runtime.Data;
using Runtime.DearImGUI.Gui;

namespace Project.Editor.UI.Generic
{
    public class RenameAssetWindow : GuiWindow
    {
        Asset asset;
        private byte[] name = new byte[128];

        public RenameAssetWindow(Asset asset)
        {
            this.asset = asset;
            name = System.Text.Encoding.UTF8.GetBytes(asset.GetName());
        }

        public override string GetName()
        {
            return "Rename Asset";
        }

        public override void Render()
        {
            ImGui.Text("Enter a new name:");
            ImGui.InputText("Name", name, (uint)name.Length);

            if (ImGui.Button("Rename"))
            {
                Console.WriteLine(GetString(name));
                File.Move(asset.GetSystemPath(), asset.GetFolder().GetSystemPath() + $"/{GetString(name)}");
                // Move the meta
                if (File.Exists(asset.GetSystemPath() + ".meta")) // Check if there is a meta file
                    File.Move(asset.GetSystemPath() + ".meta", asset.GetFolder().GetSystemPath() + $"/{GetString(name)}.meta");
            }
        }

        private static string GetString(byte[] buffer)
        {
            int length = Array.IndexOf(buffer, (byte)0);
            if (length < 0) length = buffer.Length;
            return System.Text.Encoding.UTF8.GetString(buffer, 0, length);
        }
    }
}
