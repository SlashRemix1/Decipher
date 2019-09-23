using Doumi.Nexon.Net;
using System;


namespace Akorade.Types
{
    public class Aisling : Sprite<Aisling>
    {
        public Aisling(NexonPacket packet)
        {
            X = packet.ReadU2();
            Y = packet.ReadU2();
            Face = packet.ReadU1();
            Guid = packet.ReadU4();
            Tint = packet.ReadU1();
            packet.ReadU1();
            packet.ReadU1();
            packet.ReadU1();
            packet.Offset += (packet.ReadU2() == 0xffff) ? 11 : 0x35;
            Type = packet.ReadU1();
            NameTint = packet.ReadU1();
            Name = packet.ReadC1();
        }

        public override Aisling CopyFrom(Aisling value)
        {
            X = value.X;
            Y = value.Y;
            Guid = value.Guid;
            Tint = value.Tint;
            NameTint = value.NameTint;
            return this;
        }


        public byte Type { get; set; }

        public byte NameTint { get; set; }

        public byte Face { get; set; }

        public string Name { get; set; }

        public byte Tint { get; set; }
    }
}
