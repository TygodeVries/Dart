using OpenTK.Graphics.OpenGL;
using Runtime.Data;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Runtime.Graphics
{
    public class RenderTexture : Texture
    {
        public int Framebuffer;

        public int ColorTexture;
        public int Depthbuffer;

        public int Width;
        public int Height;

        public RenderTexture(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            // Crea the buffer
            Framebuffer = GL.GenFramebuffer();

            // Start editing the buffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);

            // Bind color texture
            ColorTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2d, ColorTexture);
            GL.TexImage2D(
                TextureTarget.Texture2d,
                0,
                InternalFormat.Rgba,
                width,
                height,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                IntPtr.Zero
                );

            // Set params for texture
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureBaseLevel, 0);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMaxLevel, 0);

            GL.FramebufferTexture2D(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2d,
                ColorTexture,
                0);

            // Depth

            Depthbuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, Depthbuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent24, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, Depthbuffer);

            // Reset
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void SaveToAsset(Asset asset)
        {
            byte[] pixels = new byte[Width * Height * 4];
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);
            GL.ReadPixels(0, 0, Width, Height, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            byte[] data = (byte[])pixels.Clone();

            using Image<Rgba32> image = new Image<Rgba32>(Width, Height);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int idx = ((y * Width) + x) * 4;
                    byte r = data[idx + 0];
                    byte g = data[idx + 1];
                    byte b = data[idx + 2];
                    byte a = data[idx + 3];

                    // Flip vertically for ImageSharp
                    image[x, Height - y - 1] = new Rgba32(r, g, b, a);
                }
            }

            image.Save(asset.GetSystemPath());
        }


        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);
            GL.Viewport(0, 0, Width, Height);
        }

        public void Unbind(int screenX, int screenY)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, screenX, screenY);
        }

        public void Dispose()
        {
            GL.DeleteFramebuffer(Framebuffer);
            GL.DeleteTexture(ColorTexture);
            GL.DeleteRenderbuffer(Depthbuffer);
        }

        public override void Use(TextureUnit textureUnit)
        {
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2d, ColorTexture);
        }
    }
}
