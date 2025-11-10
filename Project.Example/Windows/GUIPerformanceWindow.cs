using ImGuiNET;
using Runtime.Calc;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics;
using Runtime.Graphics.Pipeline;
using Runtime.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureTestProject
{
    public class GUIPerformanceWindow : GuiWindow
    {
        static List<float> fpsHistory = new(100);
        static double sampleTimer;
        public override void Render()
        {
            ImGui.Begin("Performance");

            if (ImGui.CollapsingHeader("General"))
            {
                ImGui.Text($"FPS:");

                if (sampleTimer < 0 || fpsHistory.Count == 0)
                {
                    fpsHistory.Add((float)Time.frameRate);
                    if (fpsHistory.Count > 100)
                        fpsHistory.RemoveAt(0);
                    sampleTimer = 0.1f;
                }

                sampleTimer -= Time.deltaTime;

                float avg = fpsHistory.Average();
                float min = fpsHistory.Min();
                float max = fpsHistory.Max();

                ImGui.PlotLines(
                    "(Over time)",
                    ref fpsHistory.ToArray()[0],
                    fpsHistory.Count,
                    0,
                    $"Cur: {Time.frameRate:F1} Avg: {avg:F1} | Min: {min:F1} | Max: {max:F1}",
                    min - 5,
                    max + 5,
                    new System.Numerics.Vector2(0, 80)
                );
            }

            if (RenderCanvas.main!.GetGraphicsPipeline() is DefaultGraphicsPipeline graphicsPipeline)
            {
                if (ImGui.CollapsingHeader("Rendering"))
                {
                    ImGui.Text($"Renderers: {graphicsPipeline.GetRendererCount()}");
                }
            }

            if(Scene.main != null)
            {
                if (ImGui.CollapsingHeader("Lighting"))
                {
                    LightManager lightManager = Scene.main.GetLightManager();
                    int count = lightManager.GetPointLights().Count;
                    ImGui.Text($"Point lights: {count}");
                }
            }

            ImGui.End();
        }
    }
}
