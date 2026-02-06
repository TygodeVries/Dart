using OpenTK.Graphics.OpenGL;
using Runtime.Data;

namespace Runtime.Graphics
{
    public abstract class Texture : AssetReference
    {
        public abstract void Use(TextureUnit textureUnit);
    }
}
