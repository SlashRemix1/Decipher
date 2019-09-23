using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akorade.Types
{
    [Flags]
    internal enum MythosiaClass : byte
    {
        Peasant,
        Warrior,
        Rogue,
        Wizard,
        Priest,
        Monk,
    }
}
