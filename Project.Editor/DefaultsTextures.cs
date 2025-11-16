using Runtime.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor
{
    internal class DefaultsTextures
    {
        private static Texture? fallbackTexture;
        /// <summary>
        /// A fallback texture that will be used in case an error has occured.
        /// </summary>
        /// <returns></returns>
        public static Texture GetFallbackTexture()
        {
            if(fallbackTexture == null)
            {
                fallbackTexture = Texture.LoadFromPng("assets/textures/icons/Unknown.png");
            }
            
            return fallbackTexture;
        }

        private static Texture? loadingTexture;

        /// <summary>
        /// A generic texture used for loading
        /// </summary>
        /// <returns></returns>
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
