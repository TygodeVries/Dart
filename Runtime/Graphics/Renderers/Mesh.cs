using Runtime.Calc;
using Runtime.Data;
using Runtime.Logging;
using System.Globalization;
namespace Runtime.Graphics.Renderers
{
    [AssetReference(new string[] { ".obj" }, nameof(FromFileObj))]
    public class Mesh : AssetReference
    {
        public static Mesh? FromFileObj(Asset asset)
        {
            string file = asset.GetSystemPath();
            List<Vector3> positions = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> texcoords = new List<Vector2>();

            List<float> finalVertices = new List<float>();
            List<float> finalNormals = new List<float>();
            List<float> finalUVs = new List<float>();
            List<uint> finalIndices = new List<uint>();

            Dictionary<string, uint> vertexMap = new Dictionary<string, uint>();
            try
            {
                string[] lines = File.ReadAllLines(file);

                foreach (string line in lines)
                {
                    if (line.StartsWith("v "))
                    {
                        string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                        float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                        positions.Add(new Vector3(x, y, z));
                    }
                    else if (line.StartsWith("vt "))
                    {
                        string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        float u = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        float v = float.Parse(parts[2], CultureInfo.InvariantCulture);
                        texcoords.Add(new Vector2(u, v));
                    }
                    else if (line.StartsWith("vn "))
                    {
                        string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                        float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                        normals.Add(new Vector3(x, y, z));
                    }
                    else if (line.StartsWith("f "))
                    {
                        string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        List<uint> faceIndices = new();

                        for (int i = 1; i < parts.Length; i++)
                        {
                            string[] tokens = parts[i].Split('/');

                            int posIndex = int.Parse(tokens[0]) - 1;
                            int uvIndex = (tokens.Length > 1 && !string.IsNullOrEmpty(tokens[1])) ? int.Parse(tokens[1]) - 1 : -1;
                            int normIndex = (tokens.Length > 2 && !string.IsNullOrEmpty(tokens[2])) ? int.Parse(tokens[2]) - 1 : -1;

                            string key = $"{posIndex}/{uvIndex}/{normIndex}";
                            if (!vertexMap.TryGetValue(key, out uint index))
                            {
                                Vector3 pos = positions[posIndex];
                                Vector2 uv = (uvIndex >= 0 && uvIndex < texcoords.Count) ? texcoords[uvIndex] : Vector2.Zero;
                                Vector3 norm = (normIndex >= 0 && normIndex < normals.Count) ? normals[normIndex] : Vector3.Zero;

                                finalVertices.Add(pos.x);
                                finalVertices.Add(pos.y);
                                finalVertices.Add(pos.z);

                                finalUVs.Add(uv.y);
                                finalUVs.Add(uv.x);

                                finalNormals.Add(norm.x);
                                finalNormals.Add(norm.y);
                                finalNormals.Add(norm.z);

                                index = (uint)((finalVertices.Count / 3) - 1);
                                vertexMap[key] = index;
                            }

                            faceIndices.Add(index);
                        }

                        for (int i = 1; i < faceIndices.Count - 1; i++)
                        {
                            finalIndices.Add(faceIndices[0]);
                            finalIndices.Add(faceIndices[i]);
                            finalIndices.Add(faceIndices[i + 1]);
                        }
                    }
                }

                Mesh mesh = new Mesh(finalVertices.ToArray(), finalIndices.ToArray())
                {
                    normals = finalNormals.ToArray(),
                    uvs = finalUVs.ToArray()
                };

                mesh.RecalculateTangents();

                mesh.SetAsset(asset);
                return mesh;
            }
            catch (Exception e)
            {
                Debug.Error(e.Message);
                return null;
            }
        }

        public float[] vertices;
        public uint[] indices;
        public float[]? normals;
        public float[]? uvs;
        public float[]? tangents;

        public Vector3 CalculateBounds()
        {
            if (vertices.Length <= 6)
                return new Vector3(1, 1, 1);

            Vector3 firstVert = GetVertex(0);

            Vector3 min = firstVert;
            Vector3 max = firstVert;

            for (int i = 0; i < indices.Length / 3; i++)
            {
                Vector3 vert = GetVertex(i);

                if (vert.x < min.x) min.x = vert.x;
                if (vert.y < min.y) min.y = vert.y;
                if (vert.z < min.z) min.z = vert.z;

                if (vert.x > max.x) max.x = vert.x;
                if (vert.y > max.y) max.y = vert.y;
                if (vert.z > max.z) max.z = vert.z;
            }

            return max - min;
        }

        public Vector3 GetVertex(int ind)
        {
            float x = vertices[0 + (ind * 3)];
            float y = vertices[1 + (ind * 3)];
            float z = vertices[2 + (ind * 3)];

            return new Vector3(x, y, z);
        }

        public Mesh(float[] vertices, uint[] indices)
        {
            this.vertices = vertices;
            this.indices = indices;
        }

        public Mesh(Vector3[] vertices, uint[] indices, Vector2[] uvs) : this(vertices, indices)
        {
            this.uvs = new float[uvs.Length * 2];
            for (int i = 0; i < uvs.Length; i++)
            {
                this.uvs[(i * 2) + 0] = uvs[i].x;
                this.uvs[(i * 2) + 1] = uvs[i].y;
            }
        }
        public Mesh(Vector3[] vertices, uint[] indices, Vector2[] uvs, Vector3[] normals) : this(vertices, indices, uvs)
        {
            this.normals = new float[uvs.Length * 3];
            for (int i = 0; i < normals.Length; i++)
            {
                this.normals[(3 * i) + 0] = normals[i].x;
                this.normals[(3 * i) + 1] = normals[i].y;
                this.normals[(3 * i) + 2] = normals[i].z;
            }
        }

        public Mesh(Vector3[] vertices, uint[] indices)
        {
            this.vertices = new float[vertices.Length * 3];
            for (int i = 0; i < vertices.Length; i++)
            {
                this.vertices[(i * 3) + 0] = vertices[i].x;
                this.vertices[(i * 3) + 1] = vertices[i].y;
                this.vertices[(i * 3) + 2] = vertices[i].z;
            }

            this.indices = indices;
        }

        /// <summary>
        /// https://discussions.unity.com/t/calculating-tangents-vector4/179/4
        /// </summary>
        public void RecalculateTangents()
        {
            if (null == vertices || null == indices || null == uvs || null == normals)
            {
                Debug.Error("Calculate tangents on incomplete mesh");
                return;
            }
            int vertexCount = vertices.Length / 3;
            int triangleCount = indices.Length / 3;

            Vector3[] tan1 = new Vector3[vertexCount];
            Vector3[] tan2 = new Vector3[vertexCount];
            Vector4[] tangentVecs = new Vector4[vertexCount];

            for (int a = 0; a < triangleCount; a++)
            {
                uint i1 = indices[(a * 3) + 0];
                uint i2 = indices[(a * 3) + 1];
                uint i3 = indices[(a * 3) + 2];

                Vector3 v1 = new Vector3(vertices[(i1 * 3) + 0], vertices[(i1 * 3) + 1], vertices[(i1 * 3) + 2]);
                Vector3 v2 = new Vector3(vertices[(i2 * 3) + 0], vertices[(i2 * 3) + 1], vertices[(i2 * 3) + 2]);
                Vector3 v3 = new Vector3(vertices[(i3 * 3) + 0], vertices[(i3 * 3) + 1], vertices[(i3 * 3) + 2]);

                Vector2 w1 = new Vector2(uvs[(i1 * 2) + 0], uvs[(i1 * 2) + 1]);
                Vector2 w2 = new Vector2(uvs[(i2 * 2) + 0], uvs[(i2 * 2) + 1]);
                Vector2 w3 = new Vector2(uvs[(i3 * 2) + 0], uvs[(i3 * 2) + 1]);

                float x1 = v2.x - v1.x;
                float x2 = v3.x - v1.x;
                float y1 = v2.y - v1.y;
                float y2 = v3.y - v1.y;
                float z1 = v2.z - v1.z;
                float z2 = v3.z - v1.z;

                float s1 = w2.x - w1.x;
                float s2 = w3.x - w1.x;
                float t1 = w2.y - w1.y;
                float t2 = w3.y - w1.y;

                float r = (s1 * t2) - (s2 * t1);
                r = r == 0.0f ? 1.0f : 1.0f / r;

                Vector3 sdir = new Vector3(((t2 * x1) - (t1 * x2)) * r,
                                                    ((t2 * y1) - (t1 * y2)) * r,
                                                    ((t2 * z1) - (t1 * z2)) * r);

                Vector3 tdir = new Vector3(((s1 * x2) - (s2 * x1)) * r,
                                                    ((s1 * y2) - (s2 * y1)) * r,
                                                    ((s1 * z2) - (s2 * z1)) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }

            tangents = new float[vertexCount * 4];

            for (int i = 0; i < vertexCount; ++i)
            {
                Vector3 n = new Vector3(normals[(i * 3) + 0], normals[(i * 3) + 1], normals[(i * 3) + 2]);
                Vector3 t = tan1[i];

                Vector3 tangent = t - (n * Vector3.Dot(n, t));
                tangent = Vector3.Normalize(tangent);

                float w = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0f) ? -1.0f : 1.0f;

                tangents[(i * 4) + 0] = tangent.x;
                tangents[(i * 4) + 1] = tangent.y;
                tangents[(i * 4) + 2] = tangent.z;
                tangents[(i * 4) + 3] = w;
            }
        }


    }
}
