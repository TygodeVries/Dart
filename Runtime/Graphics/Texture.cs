using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Runtime.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Graphics
{
    public class Texture : IDisposable
    {
        byte[] pixels;
        public int width;
        public int height;
        public int Handle;
        public bool isUploaded = false;
        public Texture(int width, int height, byte[] pixels)
        {
            this.width = width;
            this.height = height;
            this.pixels = pixels;
        }

        public static Texture LoadFromPng(string path, int maxWidth = 8192, int maxHeight = 8192, bool upload = true)
        {
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

            Texture texture = new Texture(image.Width, image.Height, pixels);
            if(upload) texture.Upload();

            image.Dispose();
            return texture;
        }

        public void Upload()
        {
            isUploaded = true;
            Handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2d, Handle);
            GL.TexParameterf(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameterf(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameterf(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
            Debug.Log($"Uploading texture of {width}x{height}");

            if (GL.GetError() != ErrorCode.NoError)
                Debug.Log($"OpenGL has an error: {GL.GetError()}");
        }

        public void Use(TextureUnit textureUnit)
        {
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2d, Handle);
        }

        public void Dispose()
        {
            if (Handle != 0)
            {
                GL.DeleteTexture(Handle);
                Handle = 0;
            }
        }
    }
}
