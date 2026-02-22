using OpenTK.Graphics.OpenGL;
using Runtime.Calc;
using Runtime.Data;
using Runtime.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Runtime.Graphics
{
    [AssetReference(new string[] { ".png" }, nameof(LoadFromPngSimple))]
    public class ImageTexture : Texture
    {
        private byte[] pixels;
        public int width;
        public int height;
        public int Handle = 0;
        public bool isUploaded = false;
        public byte[] GetPixels()
        {
            return pixels;
        }
        public ImageTexture(int width, int height, byte[] pixels)
        {
            this.width = width;
            this.height = height;
            this.pixels = pixels;
        }


        private static Dictionary<string, ImageTexture> cache = new Dictionary<string, ImageTexture>();
        public static void RemoveFromCache(Asset asset)
        {
            cache.Remove(asset.GetSystemPath());
        }
        public static ImageTexture LoadFromPngSimple(Asset asset)
        {
            return LoadFromPng(asset);
        }

        public static ImageTexture LoadFromPng(Asset asset, int maxWidth = 8192, int maxHeight = 8192, bool upload = true, bool useCache = true)
        {
            if (cache.ContainsKey(asset.GetSystemPath()) && useCache)
            {
                return cache[asset.GetSystemPath()];
            }

            Debug.Log($"Creating texture of {maxWidth}, {maxHeight}, {upload}");
            string path = asset.GetSystemPath();
            if (!File.Exists(path))
            {
                Debug.Error($"Failed to load image from path {path}. File does not exist!");
                return null;
            }

            Image<Rgba32> image = Image.Load<Rgba32>(path);

            int newWidth = image.Width;
            int newHeight = image.Height;

            if (image.Width > maxWidth || image.Height > maxHeight)
            {
                float ratioX = (float)maxWidth / image.Width;
                float ratioY = (float)maxHeight / image.Height;
                float ratio = Math.Min(ratioX, ratioY);

                newWidth = (int)(image.Width * ratio);
                newHeight = (int)(image.Height * ratio);

                image.Mutate(x => x.Resize(newWidth, newHeight));
                Debug.Log($"Resized image from {image.Width}x{image.Height} to {newWidth}x{newHeight}");
            }

            byte[] pixels = new byte[4 * image.Width * image.Height];
            image.CopyPixelDataTo(pixels);
            image.Dispose();

            ImageTexture texture = new ImageTexture(image.Width, image.Height, pixels);
            if (upload) texture.Upload();
            texture.SetAsset(asset);

            if (useCache)
                cache.Add(asset.GetSystemPath(), texture);
            return texture;
        }

        public void Upload()
        {
            if (isUploaded)
                return;
            isUploaded = true;
            MainThread.Run(() =>
            {
                if (Handle == 0)
                {
                    Handle = GL.GenTexture();
                    Debug.Log("New handle created: " + Handle);
                }
                if (GL.GetError() != ErrorCode.NoError)
                    Debug.Log($"AAAH OpenGL has an error: {GL.GetError()}");

                GL.BindBuffer(BufferTarget.PixelUnpackBuffer, 0);
                if (GL.GetError() != ErrorCode.NoError)
                    Debug.Log($"OpenGL has an error: {GL.GetError()}");
                GL.BindTexture(TextureTarget.Texture2d, Handle);
                if (GL.GetError() != ErrorCode.NoError)
                    Debug.Log($"1OpenGL has an error: {GL.GetError()}");
                GL.TexParameterf(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                if (GL.GetError() != ErrorCode.NoError)
                    Debug.Log($"2OpenGL has an error: {GL.GetError()}");
                GL.TexParameterf(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                if (GL.GetError() != ErrorCode.NoError)
                    Debug.Log($"3OpenGL has an error: {GL.GetError()}");
                GL.TexParameterf(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                if (GL.GetError() != ErrorCode.NoError)
                    Debug.Log($"4OpenGL has an error: {GL.GetError()}");
                GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
                Debug.Log($"Uploading texture of {width}x{height}");

                if (GL.GetError() != ErrorCode.NoError)
                    Debug.Log($"5OpenGL has an error: {GL.GetError()}");
            });
        }

        public override void Use(TextureUnit textureUnit)
        {
            if (!isUploaded)
                Upload();
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2d, Handle);
        }

        ~ImageTexture()
        {
            if (Handle != 0)
            {
                MainThread.Run(() =>
                {
                    GL.DeleteTexture(Handle);
                    Handle = 0;
                });
            }
        }
    }
}
