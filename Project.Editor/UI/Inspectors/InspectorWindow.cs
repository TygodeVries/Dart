using ImGuiNET;
using Runtime.DearImGUI.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor.UI.Inspectors
{
    internal class InspectorWindow : GuiWindow
    {
        public override void Render()
        {
            ImGui.Begin("Inspector");

            ImGui.End();
        }
    }
}
