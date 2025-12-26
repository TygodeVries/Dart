
using Runtime.Calc;
using Runtime.Component.Core;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Shaders;
using Runtime.Logging;
namespace Runtime.Graphics.Renderers
{
    public class TextRenderer : MeshRenderer
    {
        private static Material? worldTextMaterial;
        private static Material? uiTextMaterial;
        public static void LoadTextMaterials()
        {
            ShaderProgram worldTextShader = ShaderProgram.FromFile("assets/shaders/worldText.vert", "assets/shaders/worldText.frag");
            worldTextShader.Compile();

            worldTextMaterial = new Material(worldTextShader);
            worldTextMaterial.SetTexture("u_Texture", Texture.LoadFromPng("assets/fonts/download.png"), 0);

            ShaderProgram uiTextShader = ShaderProgram.FromFile("assets/shaders/uiText.vert", "assets/shaders/uiText.frag");
            uiTextShader.Compile();

            uiTextMaterial = new Material(uiTextShader)
            {
                matrixEnabled = false
            };
            uiTextMaterial.SetTexture("u_Texture", Texture.LoadFromPng("assets/fonts/download.png"), 0);
        }


        public float fontSize = 0.1f;
        public float characterDistance = 0.5f;

        CharacterMap characterMap = new CharacterMap();

        public void Apply()
        {
            SetText(text);
        }

        public override void OnLoad()
        {
            Apply();
            base.OnLoad();
        }

        string text;
        public void SetText(string text)
        {
            this.text = text;
            List<Vector3> verts = new List<Vector3>();
            List<uint> ind = new List<uint>();
            List<Vector2> uvs = new List<Vector2>();


            Vector2 offset = new Vector2();
            Transform? transform = GetComponent<Transform>();
            if (transform != null)
            {
                offset.x = transform.position.x;
                offset.y = transform.position.y;
            }

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
                    AddCharacter(verts, ind, uvs, (character * characterDistance * fontSize) + offset.x, (-line * fontSize) + offset.y, text[i]);
                    character++;
                }
            }

            this.SetMesh(new Mesh(verts.ToArray(), ind.ToArray(), uvs.ToArray()));
        }

        private void AddCharacter(List<Vector3> verts, List<uint> ind, List<Vector2> uvs, float x, float y, char c)
        {
            uint startIndex = (uint)verts.Count;


            verts.Add(new Vector3(x, y, 0));
            verts.Add(new Vector3(x + fontSize, y, 0));
            verts.Add(new Vector3(x + fontSize, y + fontSize, 0));
            verts.Add(new Vector3(x, y + fontSize, 0));

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

        public TextRenderer(TextSpace textSpace)
        {
            text = "";
            if (textSpace == TextSpace.World)
            {
                if (worldTextMaterial == null)
                    LoadTextMaterials();

                if (worldTextMaterial == null)
                {
                    Debug.Error("Could not access text material.");
                    return;
                }

                this.material = worldTextMaterial;
            }
            else
            {
                if (uiTextMaterial == null)
                    LoadTextMaterials();

                if (uiTextMaterial == null)
                {
                    Debug.Error("Could not access text material.");
                    return;
                }

                this.material = uiTextMaterial;
            }
        }

        public TextRenderer(string text, TextSpace textSpace) : this(textSpace)
        {
            SetText(text);
        }
    }

    public enum TextSpace
    {
        /// <summary>
        /// The text is a 3d object in the world
        /// </summary>
        World,

        /// <summary>
        /// The text is attached to the camera
        /// </summary>
        Camera
    }

    public class CharacterMap
    {
        string fontText = " 1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,.<>/?!@#$%^&*()";
        public Vector2[] GetCharacterUv(char c)
        {
            int charIndex = fontText.IndexOf(c);
            float x = charIndex % 16;
            float y = MathF.Floor(charIndex / 16f);

            return new Vector2[]
            {
                new Vector2((float) (x + 0) / 16f, (float) 1f - ((y + 1) / 16f)),
                new Vector2((float) (x + 1) / 16f, (float) 1f - ((y + 1) / 16f)),
                new Vector2((float) (x + 1) / 16f, (float) 1f - ((y + 0) / 16f)),
                new Vector2((float) (x + 0) / 16f, (float) 1f - ((y + 0) / 16f))
            };
        }
    }
}
