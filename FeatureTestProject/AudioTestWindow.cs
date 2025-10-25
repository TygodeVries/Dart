using ImGuiNET;
using Runtime.DearImGUI.Gui;
using Runtime.WindowsNative;
using Runtime.WindowsNative.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureTestProject
{
    internal class AudioTestWindow : GuiWindow
    {
        Sample sample = Sample.ReadSample("assets\\sounds\\gurgle.wav");

        public override void Render()
        {
            ImGui.Begin("Hello");
            
            if(ImGui.Button("Press me!"))
            {
                WindowsNative.GetAudio().Play(sample);
            }

            ImGui.End();
        }
    }
}
