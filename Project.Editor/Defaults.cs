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
                fallbackTexture = new Texture("assets/textures/icons/Unknown.png");
            }
            
            return fallbackTexture;
        }
    }
}
