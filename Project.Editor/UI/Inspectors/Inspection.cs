using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor.UI.Inspectors
{
    public abstract class Inspection
    {
        public abstract void Render();
        public virtual void Open()
        {

        }
        public virtual void Close()
        {

        }
    }
}
