using FeatureTestProject;
using ImGuiNET;
using Runtime.Audio;
using Runtime.DearImGUI.Gui;
using Runtime.Objects;
using Runtime.Plugin.Terrain;
using Runtime.WindowsNative;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Example.Windows
{
	internal class FireWindow: GuiWindow
	{
		Navigation terrain;
		NavigationPiece start, end;
		PathFinder pathfinder;
		Random r;
		public
		FireWindow(PathFinder finder, Navigation terrain, NavigationPiece start, NavigationPiece end)
		{
			r = new Random();
			pathfinder = finder;
			this.terrain = terrain;
			this.start = start;
			this.end = end;
		}
		double next_step = 0.500;
		int steps_taken = 0;
		bool reset = true;
		public override void Render()
		{
			Runtime.Graphics.Pipeline.GizmoRenderPass gizmo = Runtime.Graphics.Pipeline.GizmoRenderPass.GetInstance();

			gizmo.AddLine
			(
				terrain.GetVector(start),
				terrain.GetVector(end)
			);

			next_step -= Runtime.Calc.Time.deltaTime;
			bool step = next_step < 0;
			if (step)
			{
				next_step = 0.020;
			}

			ImGui.Begin("Path");

			if (ImGui.Button("Init"))
			{
				steps_taken = 0;
				pathfinder.Init(terrain, start, end);
			}
			if (ImGui.Button("Step") || step)
			{
				if (reset||steps_taken > 3000)
				{
					MeshNavigation.MeshNavigationPiece s = start as MeshNavigation.MeshNavigationPiece;
					MeshNavigation.MeshNavigationPiece e = end as MeshNavigation.MeshNavigationPiece;
					s!.vertex_index = (int)(r.NextInt64() % 504482);
					e!.vertex_index = (int)(r.NextInt64() % 504482);
					pathfinder.Init(terrain, s!, e!);
					reset = false;
					steps_taken = 0;
				}

				for (int steps = 0; steps < 10; steps++)
				{
					steps_taken++;
					if (!pathfinder.Step(terrain, end))
					{
						Runtime.Logging.Debug.Log("Arrived!");
						// 504482
						next_step = 3;
						reset = true;
						break;
					}
				}
			}
			ImGui.End();
		}

	}
}
