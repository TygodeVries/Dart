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

        public Texture(string path)
        {
            Image<Rgba32> image = Image.Load<Rgba32>(path);
           
            pixels = new byte[4 * image.Width * image.Height];
            image.CopyPixelDataTo(pixels);

            width = image.Width;
            height = image.Height;
            Upload();
        }

        public void Upload()
        {
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
