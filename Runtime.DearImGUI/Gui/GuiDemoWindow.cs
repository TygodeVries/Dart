using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.DearImGUI.Gui
{
    internal class GuiDemoWindow : GuiWindow
    {
        public override void Render()
        {
            ImGui.Begin("Demo Window");
            
            if(ImGui.Button("Click Me!"))
            {
                ImGui.Text("Hello!");
            }

            ImGui.End();
        }
    }
}
