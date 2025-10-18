using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Plugins
{
    public class PluginData
    {
        public string MainClass { get; set; } = "none";
        public string CoreDll { get; set; } = "none";
        public string[] Dependencies { get; set; } = new string[0];
    }
}
