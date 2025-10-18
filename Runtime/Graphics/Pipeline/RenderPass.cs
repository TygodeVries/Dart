using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Graphics.Pipeline
{
    public abstract class RenderPass
    {
        public abstract void Pass();
        public virtual void Start() { }
    }
}
