using OpenTK.Graphics.OpenGL;
using Runtime.Calc;
using Runtime.Data;
using Runtime.Logging;

namespace Runtime.Graphics
{
    [AssetReference(new string[] { ".sky" }, nameof(CreateFromFile))]
    public class CubemapTexture : Texture
    {
        public CubemapTexture(ImageTexture[] faces)
        {
            if (faces.Length != 6)
            {
                Debug.Error("Cubemap needs exectly 6 faces.");
                return;
            }

            int w = faces[0].width;
            int h = faces[0].height;

            if (w != h)
            {
                Debug.Error($"Faces (textures) for a cube map need to have the same width as height {w}x{h}");
                return;
            }

            for (int i = 1; i < 6; i++)
            {
                if (faces[i].width != w || faces[i].height != h)
                    Debug.Error("All cubemap faces must be the same size");
            }


            this.faces = faces;
            this.size = w;
        }

        public int Handle;
        public bool isUploaded;
        public int size;

        ImageTexture[] faces;
        // Order:
        // +X (right)
        // -X (left)
        // +Y (up)
        // -Y (down)
        // +Z (forward)
        // -Z (backward)

        public static CubemapTexture CreateFromFile(Asset asset)
        {
            string[] files = { "right.png", "left.png", "up.png", "down.png", "forward.png", "backward.png" };
            ImageTexture[] faces = new ImageTexture[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                string file = Path.Join(asset.GetFolder().GetSystemPath(), files[i]);
                if (!File.Exists(file))
                {
                    Debug.Error($"Could not find file for cubemap at {file}!");
                    continue;
                }

                ImageTexture texture = ImageTexture.LoadFromPng(Asset.FromSystemPath(asset.GetDatabase(), file), upload: false, useCache: false);
                faces[i] = texture;
            }

            CubemapTexture cmt = new CubemapTexture(faces);
            cmt.SetAsset(asset);
            return cmt;
        }

        public override void Use(TextureUnit textureUnit)
        {
            if (!isUploaded)
                Upload();
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.TextureCubeMap, Handle);
        }


        public void Upload()
        {
            if (isUploaded)
                return;
            isUploaded = true;

            MainThread.Run(() =>
            {
                Handle = GL.GenTexture();
                GL.BindTexture(TextureTarget.TextureCubeMap, Handle);

                TextureTarget[] targets = new TextureTarget[] {
                        TextureTarget.TextureCubeMapPositiveX,
                        TextureTarget.TextureCubeMapNegativeX,
                        TextureTarget.TextureCubeMapPositiveY,
                        TextureTarget.TextureCubeMapNegativeY,
                        TextureTarget.TextureCubeMapPositiveZ,
                        TextureTarget.TextureCubeMapNegativeZ,

                    };

                for (int i = 0; i < 6; i++)
                {
                    GL.TexImage2D(targets[i], 0, InternalFormat.Rgba, size, size, 0, PixelFormat.Rgba, PixelType.UnsignedByte, faces[i].GetPixels());
                }

                GL.TexParameteri(TextureTarget.TextureCubeMap,
                TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameteri(TextureTarget.TextureCubeMap,
                    TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameteri(TextureTarget.TextureCubeMap,
                    TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameteri(TextureTarget.TextureCubeMap,
                    TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameteri(TextureTarget.TextureCubeMap,
                    TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
                GL.GenerateMipmap(TextureTarget.TextureCubeMap);
                GL.TexParameteri(TextureTarget.TextureCubeMap,
                    TextureParameterName.TextureMinFilter,
                    (int)TextureMinFilter.LinearMipmapLinear);

            });
        }
    }
}
