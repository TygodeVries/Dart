using ImGuiNET;
using System.Numerics;

namespace Project.Editor.UI.Inspectors.Inspections
{
    internal class FolderAssetInspection : AssetInspection
    {
        public override void Open()
        {
            color = GetActiveMetaData().GetVector4("color", new Vector4(1, 1, 1, 1));
        }

        Vector4 color;
        public override void Render()
        {
            ImGui.ColorPicker4("Folder Color", ref color);
            GetActiveMetaData().SetVector4("color", color);
            GetActiveMetaData().Save();
        }
    }
}
