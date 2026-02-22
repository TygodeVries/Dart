
using Runtime.Calc;
using Runtime.Components.Core;
using Runtime.Data;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Shaders;
using Runtime.Logging;
namespace Runtime.Graphics.Renderers
{
    public class TextRenderer : MeshRenderer
    {
        private static Material? worldTextMaterial;
        private static Material? uiTextMaterial;
        public void LoadTextMaterials()
        {
            ShaderProgram worldTextShader = ShaderProgram.FromFile(assetDatabase.GetAsset("assets/shaders/worldText.vert"), assetDatabase.GetAsset("assets/shaders/worldText.frag"));
            worldTextShader.Compile();

            worldTextMaterial = new Material(worldTextShader);
            worldTextMaterial.SetTexture("u_Texture", ImageTexture.LoadFromPng(font.texture));

            ShaderProgram uiTextShader = ShaderProgram.FromFile(assetDatabase.GetAsset("assets/shaders/uiText.vert"), assetDatabase.GetAsset("assets/shaders/uiText.frag"));
            uiTextShader.Compile();

            uiTextMaterial = new Material(uiTextShader)
            {
                matrixEnabled = false
            };
            uiTextMaterial.SetTexture("u_Texture", ImageTexture.LoadFromPng(font.texture));
        }


        public float fontSize = 0.1f;
        public float characterDistance = 0.5f;

        private Font? font;

        public void Apply()
        {
            SetText(text);
        }

        private AssetDatabase assetDatabase;

        public override void Load()
        {
            if (font == null)
            {
                font = new Font(assetDatabase.GetAsset("assets/fonts/download.png"));
            }

            LoadTextMaterials();
            Apply();
            base.Load();
        }

        private string text;
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

            uvs.AddRange(font.GetCharacterUv(c));
        }

        public TextRenderer(TextSpace textSpace, AssetDatabase assetDatabase)
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

            this.assetDatabase = assetDatabase;
        }

        public TextRenderer(string text, TextSpace textSpace, AssetDatabase database) : this(textSpace, database)
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

    public class Font
    {
        public Font(Asset texture)
        {
            this.texture = texture;
        }

        public Asset texture;
        private string fontText = " 1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,.<>/?!@#$%^&*()";
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
