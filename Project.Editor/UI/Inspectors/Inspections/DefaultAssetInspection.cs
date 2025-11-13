using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor.UI.Inspectors.Inspections
{
    internal class DefaultAssetInspection : Inspection
    {
        public override void Render()
        {
            ImGui.Text("Asset Type: Unknown");
        }
    }
}
