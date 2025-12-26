using FeatureTestProject;
using ImGuiNET;
using Runtime.Audio;
using Runtime.DearImGUI.Gui;
using Runtime.Objects;
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
		GameObject? emitter;
		public
		FireWindow(GameObject emitter)
		{
			this.emitter = emitter;
		}
		public override void Render()
		{
			ImGui.Begin("Fire!");

			if (ImGui.Button("Fire!"))
			{
				emitter?.GetComponent<ParticleTest>()?.Fire();
			}

			ImGui.End();
		}

	}
}
