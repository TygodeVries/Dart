using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor.UI.FileSystem.FileInspectors
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class AssetManagerAttribute : Attribute
    {
        public string FileExtension { get; }

        public AssetManagerAttribute(string fileExtension)
        {
            FileExtension = fileExtension;
        }
    }
}
