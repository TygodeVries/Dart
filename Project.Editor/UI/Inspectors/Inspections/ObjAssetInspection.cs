using ImGuiNET;
using Runtime.Calc;
using Runtime.Graphics.Renderers;

namespace Project.Editor.UI.Inspectors.Inspections
{
    public class ObjAssetInspection : AssetInspection
    {
        Mesh? mesh;
        public override void Loaded()
        {
            mesh = Mesh.FromFileObj(GetActiveFilePath())!;
        }

        public override void Render()
        {
            if (mesh == null)
                return;


            ImGui.Text($"Vertex Count: {Format.Number(mesh.vertices.Length / 3)}");
            ImGui.Text($"Index Count: {Format.Number(mesh.indices.Length)} ({Format.Number(mesh.indices.Length / 3)} triangles)");
        }

        public override void Close()
        {
            // Dispose??
        }
    }
}
