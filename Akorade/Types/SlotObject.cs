using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akorade.Types
{
    public class SlotObject
    {
        public SlotObject()
        {
            Name = "Empty";
        }

        public byte Slot { get; set; }
        public string Name { get; set; }
    }
}
