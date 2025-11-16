using ImGuiNET;
using Runtime.DearImGUI.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor.UI.Inspectors
{
    /// <summary>
    /// Render the inspector.
    /// </summary>
    internal class InspectorWindow : GuiWindow
    {
        private Inspection? inspection;

        /// <summary>
        /// Set what the inspector is inspecting.
        /// </summary>
        /// <param name="inspection"></param>
        public void SetInspection(Inspection inspection)
        {
            if(this.inspection != null)
            {
                this.inspection.Close();
            }

            this.inspection = inspection;
            if(this.inspection == null)
            {
                return;
            }

            this.inspection.Open();
        }

        static InspectorWindow? activeWindow = null;
        
        /// <summary>
        /// Get the inspector the user is currently targeting.
        /// </summary>
        /// <returns></returns>
        public static InspectorWindow GetActive()
        {
            if (activeWindow == null)
            {
                activeWindow = new InspectorWindow();
                GuiWindow.Enable(activeWindow);
            }

            return activeWindow;
        }

        public InspectorWindow()
        {
            activeWindow = this;
        }

        /// <summary>
        /// Render the Inspector based on the current Inspection
        /// </summary>
        public override void Render()
        {
            ImGui.Begin("Inspector");

            if (inspection != null)
            {
                ImGui.Text($"Handled by: {inspection.GetType().Name}");
                ImGui.NewLine();
                inspection.Render();
            }else
            {
                ImGui.Text("Hmm, the should be more here...");
            }
            ImGui.End();
        }
    }
}
