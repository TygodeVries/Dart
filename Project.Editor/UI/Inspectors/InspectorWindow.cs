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
        private Inspection inspection;
        public void SetInspection(Inspection inspection)
        {
            this.inspection = inspection;
        }

        static InspectorWindow activeWindow;
        public static InspectorWindow GetActive()
        {
            return activeWindow;
        }

        public InspectorWindow()
        {
            activeWindow = this;
        }

        public override void Render()
        {
            ImGui.Begin("Inspector");

            if (inspection != null)
            {
                inspection.Render();
            }else
            {
                ImGui.Text("Nothing here!");
            }
            ImGui.End();
        }
    }
}
