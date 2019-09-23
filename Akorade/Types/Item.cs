using Doumi.Nexon.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akorade.Types
{
    public class Item : SlotObject
    {
        public Item(NexonPacket packet)
        {
            Slot = packet.ReadU1();
            Icon = packet.ReadU2();
            Tint = packet.ReadU2();
            Name = packet.ReadC1();
            Meta = packet.ReadC1();
            Unk = packet.ReadU2();
            Mass = packet.ReadU2();
            Flag = packet.ReadU1();
            MaximumDurability = packet.ReadU4();
            CurrentDurability = packet.ReadU4();
            UnkA = packet.ReadU1();
            UnkB = packet.ReadU2();
            EnglishName = Name;
        }

        public Item(byte slot, ushort icon, ushort tint, string name, string meta, ushort unk, ushort mass, byte flag, uint maximumDurability, uint currentDurability, byte num3, ushort num4, string translation)
        {
            base.Slot = slot;
            Icon = icon;
            Tint = tint;
            base.Name = name;
            Meta = meta;
            Unk = unk;
            Mass = mass;
            Flag = flag;
            MaximumDurability = maximumDurability;
            CurrentDurability = currentDurability;
            UnkA = num3;
            UnkB = num4;
            EnglishName = translation;
        }

        public Item CopyFrom(Item value)
        {
            base.Slot = value.Slot;
            Icon = value.Icon;
            Tint = value.Tint;
            base.Name = value.Name;
            Meta = value.Meta;
            Unk = value.Unk;
            Mass = value.Mass;
            Flag = value.Flag;
            MaximumDurability = value.MaximumDurability;
            CurrentDurability = value.CurrentDurability;
            UnkA = value.UnkA;
            UnkB = value.UnkB;
            EnglishName = value.EnglishName;
            return this;
        }

        public uint CurrentDurability { get; set; }

        public byte Flag { get; set; }

        public ushort Icon { get; set; }

        public ushort Mass { get; set; }

        public ushort Unk { get; set; }

        public uint MaximumDurability { get; set; }

        public byte UnkA { get; set; }
        public ushort UnkB { get; set; }

        public string Meta { get; set; }

        public ushort Tint { get; set; }
        public string EnglishName { get; set; }
    }
}
