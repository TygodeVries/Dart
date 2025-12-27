using Runtime.Physics.Raycasts;
using System.Diagnostics.CodeAnalysis;
using Runtime.Calc;
using Runtime.Graphics.Renderers;
using OpenTK.Graphics.OpenGL;
using Runtime.Graphics.Shaders;
namespace Runtime.Plugin.Terrain
{
	public interface TerrainPiece:IComparable
	{
		
	}
	public interface Terrain
	{
		TerrainPiece? FromRayCast(Raycast ray);
		double EstimateDistance(TerrainPiece x, TerrainPiece y);
		double TransitionCost(TerrainPiece x, TerrainPiece y);
		TerrainPiece[] GetNeighbors(TerrainPiece x);
		Vector4 GetVector(TerrainPiece x);
	}
	public class PathFinder: Renderer
	{
		PriorityQueue<TerrainPiece, double> boundary = new PriorityQueue<TerrainPiece, double>();
		SortedDictionary<TerrainPiece, TerrainPiece> ss = new SortedDictionary<TerrainPiece, TerrainPiece>();
		Terrain terrain;
		public PathFinder()
		{

		}
		public TerrainPiece? Backtrace(TerrainPiece x)
		{
			return ss[x];
		}
		public void Init(Terrain terrain, TerrainPiece start, TerrainPiece end)
		{
			ss.Clear();
			boundary.Clear();
			TerrainPiece[] nn = terrain.GetNeighbors(start);
			foreach (TerrainPiece item in nn)
			{
				ss.Add(item, start);
				boundary.Enqueue(item, terrain.EstimateDistance(item, end) + terrain.TransitionCost(start, item));
			}
		}
		ShaderProgram program;
		public override void OnLoad()
		{
			program = ShaderProgram.FromFile("assets\\shaders\\lit.vert", "assets\\shaders\\unlit.frag");
			program.Compile();
			base.OnLoad();
		}
		public override void Render()
		{
			program.Use();
			TerrainPiece[] keys = ss.Keys.ToArray();
			TerrainPiece[] values = ss.Values.ToArray();
			Vector4[] a = new Vector4[ss.Count * 2];
			for (int cx = 0; cx < ss.Count; cx++)
			{
				a[2 * cx] = terrain.GetVector(keys[cx]);
				a[2 * cx + 1] = terrain.GetVector(values[cx]);
			}
		//	GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Double, true, 0, a);
			GL.DrawArrays(PrimitiveType.Lines, 0, a.Length);

		}

		public bool Step(Terrain terrain, TerrainPiece end)
		{
			TerrainPiece current = boundary.Dequeue();
			if (0 == current.CompareTo(end))
				return false;
			TerrainPiece[] nn = terrain.GetNeighbors(current);
			foreach (TerrainPiece item in nn)
			{
				if (!ss.ContainsKey(item))
				{
					ss.Add(item, current);
					boundary.Enqueue(item, terrain.EstimateDistance(item, end) + terrain.TransitionCost(current, item));
				}
			}
			return true;
		}
		TerrainPiece[]? FindPath(Terrain terrain, TerrainPiece start, TerrainPiece end)
		{
			Init(terrain, start, end);
			while (boundary.Count>0)
			{
				TerrainPiece current = boundary.Dequeue();
				if (current == end)
					break;
				TerrainPiece[] nn = terrain.GetNeighbors(current);
				foreach (TerrainPiece item in nn)
				{
					if (!ss.ContainsKey(item))
					{
						ss.Add(item, current);

						boundary.Enqueue(item, terrain.EstimateDistance(item, end));
					}
				}
			}
			return null;
		}
	}
}
