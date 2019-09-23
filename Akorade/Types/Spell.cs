using Doumi.Nexon.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akorade.Types
{
    public class Spell : SlotObject
    {
        private string text;

        public Spell(NexonPacket packet)
        {
            Slot = packet.ReadU1();
        }

        public Spell(byte slot, ushort icon, byte type, string text, string note, byte span, byte currentLevel, byte maximumLevel, DateTime min, string translated)
        {
            base.Slot = slot;
            Icon = icon;
            Type = type;
            Text = text;
            Note = note;
            Span = span;
            CurrentLevel = currentLevel;
            MaximumLevel = maximumLevel;
            LastUsed = min;
            Recharge = TimeSpan.MinValue;
            EnglishName = translated;
        }

        private bool ShouldSerializeIcon() =>
            false;

        private bool ShouldSerializeName() =>
            false;

        private bool ShouldSerializeNote() =>
            false;

        private bool ShouldSerializeSlot() =>
            false;

        private bool ShouldSerializeSpan() =>
            false;

        private bool ShouldSerializeText() =>
            false;

        private bool ShouldSerializeType() =>
            false;

        public ushort Icon { get; set; }

        [Category("Timing")]
        public DateTime LastUsed { get; set; }

        [Browsable(false)]
        public bool CanUse => ((DateTime.Now - LastUsed) > Recharge);

        public string Note { get; set; }

        [Category("Timing")]
        public TimeSpan Recharge { get; set; }

        public byte Span { get; set; }

        internal byte CurrentLevel { get; set; }
        internal byte MaximumLevel { get; set; }

        public string EnglishName { get; set; }

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                int index = value.IndexOf(" (Lev:");
                if (index > 0)
                {
                    base.Name = value.Substring(0, index);
                }
                text = value;
            }
        }

        public byte Type { get; set; }
    }
}
