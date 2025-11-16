using ImGuiNET;

namespace Project.Editor.UI.Inspectors.Inspections
{
    internal class DefaultAssetInspection : AssetInspection
    {
        public override void Render()
        {
            ImGui.Text("Asset Type: Unknown");
        }
    }
}
