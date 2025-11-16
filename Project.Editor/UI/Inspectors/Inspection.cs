using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor.UI.Inspectors
{
    /// <summary>
    /// Something that the inspector is inspecting, like objects, files, etc.
    /// </summary>
    public abstract class Inspection
    {
        public abstract void Render();

        /// <summary>
        /// Called when the inspection is first opened
        /// </summary>
        public virtual void Open()
        {

        }


        /// <summary>
        /// Called when unloading the inspection.
        /// </summary>
        public virtual void Close()
        {

        }
    }
}
