using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor.UI.FileSystem.FileInspectors
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class FileInspectorAttribute : Attribute
    {
        public string FileExtension { get; }

        public FileInspectorAttribute(string fileExtension)
        {
            FileExtension = fileExtension;
        }
    }
}
