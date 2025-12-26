using ImGuiNET;

namespace Project.Editor.UI.Inspectors.Inspections
{
    public class WavAssetInspection : AssetInspection
    {
        public override void Render()
        {
            ImGui.Text("Wav File!");
            if (ImGui.Button("Play Clip"))
            {

            }
        }
    }
}
