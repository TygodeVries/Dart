using Runtime.Calc;
using Runtime.Data;
using Runtime.Graphics.Pipeline;
using Runtime.Graphics.Renderers;
using Runtime.Logging;
using Runtime.Physics.Raycasts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Plugin.Terrain
{

	[AssetReference(new string[] { ".obj" }, nameof(FromFileObj))]
	public class Graph: AssetReference
	{
		public List<Vector4> nodes = new List<Vector4>();
		public Dictionary<int, int[]> edges = new Dictionary<int, int[]>();
		public static Graph? FromFileObj(Asset asset)
		{
			string file = asset.GetSystemPath();
			List<Vector4> positions = new List<Vector4>();
			Dictionary<int, int[]> edges = new Dictionary<int, int[]>();

			try
			{
				string[] lines = File.ReadAllLines(file);
				Dictionary<int, HashSet<int>> temp_edges = new Dictionary<int, HashSet<int>>();

				foreach (string line in lines)
				{
					if (line.StartsWith("v "))
					{
						string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
						float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
						float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
						float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
						positions.Add(new Vector4(x, y, z, 1));
					}
					else if (line.StartsWith("f "))
					{
						string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
						List<uint> faceIndices = new();

						for (int i = 1; i < parts.Length; i++)
						{
							string[] tokens = parts[i].Split('/');

							uint posIndex = uint.Parse(tokens[0]) - 1;

							faceIndices.Add(posIndex);
						}

						for (int i = 0; i < faceIndices.Count; i++)
						{
							int i1 = (int)faceIndices[i];
							int i2 = (int)faceIndices[(i + 1) % faceIndices.Count];

							if (!temp_edges.ContainsKey(i1))
								temp_edges[i1] = new HashSet<int>();
							if (!temp_edges.ContainsKey(i2))
								temp_edges[i2] = new HashSet<int>();
							temp_edges[i1].Add(i2);
							temp_edges[i2].Add(i1);
						}
					}
				}
				foreach (KeyValuePair<int, HashSet<int>> q in temp_edges)
				{
					edges[q.Key] = q.Value.ToArray();
				}
				Graph graph = new Graph()
				{ nodes = positions, edges = edges};
				graph.SetAsset(asset);
				return graph;
			}
			catch (Exception e)
			{
				Debug.Error(e.Message);
				return null;
			}
		}

	}

	public class GraphNavigation : Navigation
	{
		Graph graph;
		public void Draw()
		{
			GizmoRenderPass gizmo = GizmoRenderPass.GetInstance();
//			foreach (KeyValuePair<int, int[]> kv in edges)
//			{
//				Vector4 a = nodes[kv.Key];
//				foreach (int v in kv.Value)
//				{
//					Vector4 b = nodes[v];
//	
//					gizmo.AddLine(a, b, new Vector4(1,0,0,1), new Vector4(0,1,0,1));
//				}
//			}
		}
		public static GraphNavigation? FromFile(Asset asset)
		{
			Graph? graph = Graph.FromFileObj(asset);
			if (null == graph)
				return null;
			GraphNavigation? meshTerrain = new GraphNavigation()
			{
				graph = graph
			};

			return meshTerrain;
		}

		public class GraphNavigationPiece : NavigationPiece
		{

			public int vertex_index;

			public int CompareTo(object? obj)
			{
				if (obj is not GraphNavigationPiece q)
					return -1;
				return q.vertex_index - vertex_index;
			}
		}
		public double EstimateDistance(NavigationPiece x, NavigationPiece y)
		{
			if (x is not GraphNavigationPiece xx || y is not GraphNavigationPiece yy)
				return Double.PositiveInfinity;


			Vector4 d = (graph.nodes[xx.vertex_index] - graph.nodes[yy.vertex_index]);
			return d.Magnitude();

		}

		public NavigationPiece? FromRayCast(Raycast ray)
		{
			throw new NotImplementedException();
		}

		public NavigationPiece[] GetNeighbors(NavigationPiece x)
		{
			try
			{
				if (x is not GraphNavigationPiece xx)
					return new GraphNavigationPiece[0];
				GraphNavigationPiece[] ret = new GraphNavigationPiece[graph.edges[xx.vertex_index].Length];
				for (int cx = 0; cx < graph.edges[xx.vertex_index].Length; cx++)
				{
					ret[cx] = new GraphNavigationPiece();
					ret[cx].vertex_index = graph.edges[xx.vertex_index][cx];
				}
				return ret;
			}
			catch (Exception ex)
			{
				return new NavigationPiece[0];
			}
      }

		public Vector4 GetVector(NavigationPiece x)
		{
			if (x is not GraphNavigationPiece xx)
				return new Vector4(0, 0, 0, 1);
			return graph.nodes[xx.vertex_index];
		}

		public double TransitionCost(NavigationPiece x, NavigationPiece y)
		{
			if (x is not GraphNavigationPiece xx || y is not GraphNavigationPiece yy)
				return Double.PositiveInfinity;

			Vector4 d = graph.nodes[xx.vertex_index] - graph.nodes[yy.vertex_index];
			return d.Magnitude();
		}
	}
}
