using ImGuiNET;
using Runtime.Graphics;

namespace Project.Editor.UI.Inspectors.Inspections
{
    internal class PngAssetInspection : AssetInspection
    {
        ImageTexture? texture;
        public override void Loaded()
        {
            texture = ImageTexture.LoadFromPng(GetAsset());
        }

        public override void Render()
        {
            ImGui.Text($"Resolution: {texture!.width}x{texture!.height}");

        }
    }
}
