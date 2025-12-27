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
	public class PathFinder
	{
		PriorityQueue<TerrainPiece, double> boundary = new PriorityQueue<TerrainPiece, double>();
		SortedDictionary<TerrainPiece, TerrainPiece> ss = new SortedDictionary<TerrainPiece, TerrainPiece>();
		Terrain? terrain;
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
