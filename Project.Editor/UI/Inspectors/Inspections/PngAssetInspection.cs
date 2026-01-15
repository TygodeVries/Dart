using ImGuiNET;
using Runtime.Graphics;

namespace Project.Editor.UI.Inspectors.Inspections
{
    internal class PngAssetInspection : AssetInspection
    {
        Texture? texture;
        public override void Loaded()
        {
            texture = Texture.LoadFromPng(GetAsset());
        }

        public override void Render()
        {
            ImGui.Text($"Resolution: {texture!.width}x{texture!.height}");

        }
    }
}
