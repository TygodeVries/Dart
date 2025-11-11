using Runtime.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor.UI.FileSystem.FileInspectors
{
    internal class DefaultFileInspector : FileInspector
    {
        
        public override Texture GetIcon(string path)
        {
            return Defaults.GetFallbackTexture();
        }
    }
}
