using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akorade.Types
{
    public class GroundItem : Sprite<GroundItem>
    {
        public GroundItem(ushort xpos, ushort ypos, uint guid, ushort icon, ushort tint, byte unka, string name)
        {
            X = xpos;
            Y = ypos;
            Guid = guid;
            Icon = icon;
            Tint = tint;
            UnkA = unka;
            Name = name;
        }

        public override GroundItem CopyFrom(GroundItem value)
        {
            X = value.X;
            Y = value.Y;
            Guid = value.Guid;
            Icon = value.Icon;
            UnkA = value.UnkA;
            Tint = value.Tint;
            Name = value.Name;
            return this;
        }

        public ushort Icon { get; private set; }

        public string Name { get; private set; }

        public ushort Tint { get; private set; }

        public byte UnkA { get; private set; }
    }
}
