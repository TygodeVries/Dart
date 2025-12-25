using ImGuiNET;

namespace Project.Editor.UI.Inspectors.Inspections
{
    public class PrefabAssetInspection : AssetInspection
    {
        public override void Render()
        {
            ImGui.Text("Open the prefab to edit....");
        }
    }
}
