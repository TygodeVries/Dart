using Runtime.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor
{
    internal class Defaults
    {
        private static Texture? fallbackTexture;
        public static Texture GetFallbackTexture()
        {
            if(fallbackTexture == null)
            {
                fallbackTexture = Texture.LoadFromPng("assets/textures/icons/Unknown.png");
            }
            
            return fallbackTexture;
        }

        private static Texture? loadingTexture;
        public static Texture GetLoadingTexture()
        {
            if (loadingTexture == null)
            {
                loadingTexture = Texture.LoadFromPng("assets/textures/icons/loading.png");
            }

            return loadingTexture;
        }
    }
}
