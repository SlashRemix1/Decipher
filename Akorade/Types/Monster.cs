using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akorade.Types
{
    public class Monster : Sprite<Monster>
    {

        public Monster(ushort x, ushort y, uint guid, ushort icon, uint unka, byte face, byte unkb, byte tint, byte unkc, byte type)
        {
            X = x;
            Y = y;
            Guid = guid;
            Icon = icon;
            UnkA = unka;
            Face = face;
            UnkB = unkb;
            Tint = tint;
            UnkC = unkc;
            Type = type;
        }

        public override Monster CopyFrom(Monster value)
        {
            base.X = value.X;
            base.Y = value.Y;
            base.Guid = value.Guid;
            Icon = value.Icon;
            UnkA = value.UnkA;
            Face = value.Face;
            UnkB = value.UnkB;
            Tint = value.Tint;
            UnkC = value.UnkC;
            Type = value.Type;
            return this;
        }

        public byte Face { get; set; }

        public ushort Icon { get; set; }

        public byte Tint { get; set; }

        public byte Type { get; set; }

        public uint UnkA { get; set; }

        public byte UnkB { get; set; }

        public byte UnkC { get; set; }
    }
}
