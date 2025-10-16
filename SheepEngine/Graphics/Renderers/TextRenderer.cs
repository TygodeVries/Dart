using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Shaders;
using Runtime.Logging;
namespace Runtime.Graphics.Renderers
{
    public class TextRenderer : MeshRenderer
    {
        public static Material? textMaterial;
        public static void LoadTextMaterial()
        {
            ShaderProgram textShader = ShaderProgram.FromFile("assets/shaders/text.vert", "assets/shaders/text.frag");
            textShader.Compile();

            textMaterial = new Material(textShader);
            textMaterial.SetTexture("u_Texture", new Texture("assets/fonts/download.png"), 0);
        }

        CharacterMap characterMap = new CharacterMap();

        public void SetText(string text)
        {
            List<Vector3> verts = new List<Vector3>();
            List<uint> ind = new List<uint>();
            List<Vector2> uvs = new List<Vector2>();


            int character = 0;
            int line = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    character = 0;
                    line++;
                }
                else
                {
                    AddCharacter(verts, ind, uvs, character / 2f, -line, text[i]);
                    character++;
                }
            }

            Debug.Log($"Added {text.Length * 2} triangles for text");

            this.SetMesh(new Mesh(verts.ToArray(), ind.ToArray(), uvs.ToArray()));
        }

        private void AddCharacter(List<Vector3> verts, List<uint> ind, List<Vector2> uvs, float x, float y, char c)
        {
            uint startIndex = (uint)verts.Count;

            verts.Add(new Vector3(x, y, 0));
            verts.Add(new Vector3(x + 1, y, 0));
            verts.Add(new Vector3(x + 1, y + 1, 0));
            verts.Add(new Vector3(x, y + 1, 0));

            // First triangle
            ind.Add(startIndex);
            ind.Add(startIndex + 1);
            ind.Add(startIndex + 2);

            // Second triangle
            ind.Add(startIndex + 2);
            ind.Add(startIndex + 3);
            ind.Add(startIndex);

            uvs.AddRange(characterMap.GetCharacterUv(c));
        }

        public TextRenderer()
        {
            if (textMaterial == null)
                LoadTextMaterial();

            if(textMaterial == null)
            {
                Debug.Error("Could not access text material.");
                return;
            }

            this.material = textMaterial;
        }

        public TextRenderer(string text) : this()
        {
            SetText(text);
        }
    }

    public class CharacterMap
    {
        string fontText = " 1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,.<>/?!@#$%^&*()";
        int row = 16;
        public Vector2[] GetCharacterUv(char c)
        {
            int charIndex = fontText.IndexOf(c);
            float x = charIndex % 16;
            float y = MathF.Floor(charIndex / 16f);

            return new Vector2[]
            {
                new Vector2((float) (x + 0) / 16f, (float) 1f - (y + 1) / 16f),
                new Vector2((float) (x + 1) / 16f, (float) 1f - (y + 1) / 16f),
                new Vector2((float) (x + 1) / 16f, (float) 1f - (y + 0) / 16f),
                new Vector2((float) (x + 0) / 16f, (float) 1f - (y + 0) / 16f)
            };
        }
    }
}
