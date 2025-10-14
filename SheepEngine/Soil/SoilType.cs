using Runtime.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Soil
{
    public abstract class SoilType
    {
        public abstract GameObject Build(SoilData data);
    }
}
