using ImGuiNET;
using Project.Editor.Data;
using Runtime.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor.UI.Inspectors.Inspections
{
    internal class FolderAssetInspection : AssetInspection
    {
        public override void Open()
        {
            color = (Vector4)GetActiveMetaData().GetVector4("color", new Vector4(1, 1, 1, 1));
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
