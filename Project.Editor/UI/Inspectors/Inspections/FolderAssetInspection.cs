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
    internal class FolderAssetInspection : Inspection
    {
        MetaData metaData;
        public FolderAssetInspection(MetaData metaData)
        {
            this.metaData = metaData;
            color = (Vector4) metaData.GetVector4("color", new Vector4(1, 1, 1, 1));
        }

        Vector4 color;
        public override void Render()
        {
            ImGui.ColorPicker4("Folder Color", ref color);
            Debug.Log($"{color.X}, {color.Y}, {color.Z}");
            metaData.SetVector4("color", color);
            metaData.Save();
            
        }
    }
}
