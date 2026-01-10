using Runtime.Calc;
using Runtime.Data;
using Runtime.Physics.Raycasts;
using Runtime.Graphics.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runtime.Graphics.Pipeline;

namespace Runtime.Plugin.Terrain
{
	public class MeshNavigation : Navigation
	{
		List<Vector4> nodes = new List<Vector4>();
		Dictionary<int, int[]> edges = new Dictionary<int, int[]>();
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
		public static MeshNavigation? FromFile(Asset asset)
		{
			Mesh? mesh = Mesh.FromFileObjTriangles(asset);
			if (null == mesh)
				return null;
			MeshNavigation? meshTerrain = new MeshNavigation();
			for (int cx = 0; cx < mesh.vertices.Length / 3; cx++)
			{
				meshTerrain.nodes.Add(new Vector4(
					mesh.vertices[3 * cx],
					mesh.vertices[3 * cx+1],
					mesh.vertices[3 * cx+2], 1));
			}
			Dictionary<int, HashSet<int>> temp_edges = new Dictionary<int, HashSet<int>>();

			for (int cx = 0; cx < mesh.indices.Length / 3; cx++)
			{
				int a = (int)mesh.indices[3 * cx];
				int b = (int)mesh.indices[3 * cx + 1];
				int c = (int)mesh.indices[3 * cx + 2];
				if (!temp_edges.ContainsKey(a))
					temp_edges[a] = new HashSet<int>();
				if (!temp_edges.ContainsKey(b))
					temp_edges[b] = new HashSet<int>();
				if (!temp_edges.ContainsKey(c))
					temp_edges[c] = new HashSet<int>();
				temp_edges[a].Add(b);
				temp_edges[a].Add(c);
				temp_edges[b].Add(a);
				temp_edges[b].Add(c);
				temp_edges[c].Add(b);
				temp_edges[c].Add(a);
			}
			foreach (KeyValuePair<int, HashSet<int> > q in temp_edges)
			{
				meshTerrain.edges[q.Key] = q.Value.ToArray();
			}
			return meshTerrain;
		}

		public class MeshNavigationPiece : NavigationPiece
		{

			public int vertex_index;

			public int CompareTo(object? obj)
			{
				if (obj is not MeshNavigationPiece q)
					return -1;
				return q.vertex_index - vertex_index;
			}
		}
		public double EstimateDistance(NavigationPiece x, NavigationPiece y)
		{
			if (x is not MeshNavigationPiece xx || y is not MeshNavigationPiece yy)
				return Double.PositiveInfinity;


			Vector4 d = (nodes[xx.vertex_index] - nodes[yy.vertex_index]);
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
				if (x is not MeshNavigationPiece xx)
					return new MeshNavigationPiece[0];
				MeshNavigationPiece[] ret = new MeshNavigationPiece[edges[xx.vertex_index].Length];
				for (int cx = 0; cx < edges[xx.vertex_index].Length; cx++)
				{
					ret[cx] = new MeshNavigationPiece();
					ret[cx].vertex_index = edges[xx.vertex_index][cx];
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
			if (x is not MeshNavigationPiece xx)
				return new Vector4(0, 0, 0, 1);
			return nodes[xx.vertex_index];
		}

		public double TransitionCost(NavigationPiece x, NavigationPiece y)
		{
			if (x is not MeshNavigationPiece xx || y is not MeshNavigationPiece yy)
				return Double.PositiveInfinity;

			Vector4 d = nodes[xx.vertex_index] - nodes[yy.vertex_index];
			return d.Magnitude();
		}
	}
}
