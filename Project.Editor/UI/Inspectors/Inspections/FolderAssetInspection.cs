using ImGuiNET;
using Runtime.Calc;

namespace Project.Editor.UI.Inspectors.Inspections
{
    internal class FolderAssetInspection : AssetInspection
    {
        public override void Open()
        {
            color = GetActiveMetaData().GetVector4("color", new Vector4(1, 1, 1, 1)).ToNumerics();
        }

        System.Numerics.Vector4 color;
        public override void Render()
        {
            if (ImGui.ColorPicker4("Folder Color", ref color))
            {
                GetActiveMetaData().SetVector4("color", new Vector4(color));
                GetActiveMetaData().Save();
            }
        }
    }
}
