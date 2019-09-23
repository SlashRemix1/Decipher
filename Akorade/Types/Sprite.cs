using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akorade.Types
{
    public abstract class Sprite
    {
        public uint Guid { get; set; }

        public ushort X { get; set; }

        public ushort Y { get; set; }
    }
}
