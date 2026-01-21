using Runtime.Graphics.Renderers;

namespace Runtime.Data
{
    public class PrimativeMesh
    {
        public static Mesh CreateCubeMesh()
        {
            float[] vertices = {
        // Front (+Z)
        -1f, -1f,  1f,
         1f, -1f,  1f,
         1f,  1f,  1f,
        -1f,  1f,  1f,
        // Back (-Z)
        -1f, -1f, -1f,
        -1f,  1f, -1f,
         1f,  1f, -1f,
         1f, -1f, -1f,
        // Left (-X)
        -1f, -1f, -1f,
        -1f, -1f,  1f,
        -1f,  1f,  1f,
        -1f,  1f, -1f,
        // Right (+X)
         1f, -1f, -1f,
         1f,  1f, -1f,
         1f,  1f,  1f,
         1f, -1f,  1f,
        // Top (+Y)
        -1f,  1f, -1f,
        -1f,  1f,  1f,
         1f,  1f,  1f,
         1f,  1f, -1f,
        // Bottom (-Y)
        -1f, -1f, -1f,
         1f, -1f, -1f,
         1f, -1f,  1f,
        -1f, -1f,  1f,
    };

            uint[] indices = {
        0, 1, 2, 0, 2, 3,       // front
        4, 5, 6, 4, 6, 7,       // back
        8, 9,10, 8,10,11,       // left
       12,13,14,12,14,15,       // right
       16,17,18,16,18,19,       // top
       20,21,22,20,22,23        // bottom
    };

            Mesh cube = new Mesh(vertices, indices);
            return cube;
        }

    }
}
