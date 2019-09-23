using Doumi.Nexon.Net;
using System;
using System.ComponentModel;

namespace Akorade.Types
{
    public class Skill : SlotObject
    {
        private string text;

        public Skill(NexonPacket packet)
        {
            base.Slot = packet.ReadU1();
            Icon = packet.ReadU2();
            Text = packet.ReadC1();
        }

        internal Skill(byte slot, string name, ushort sprite, byte currentLevel, byte maximumLevel, DateTime min, string translated)
        {
            Slot = slot;
            Name = name;
            Icon = sprite;
            CurrentLevel = currentLevel;
            MaximumLevel = maximumLevel;
            LastUsed = min;
            Recharge = TimeSpan.MinValue;
            EnglishName = translated;
            text = name;
        }

        private bool ShouldSerializeIcon() =>
            false;

        private bool ShouldSerializeName() =>
            false;

        private bool ShouldSerializeSlot() =>
            false;

        private bool ShouldSerializeText() =>
            false;

        [Browsable(false)]
        public bool CanUse => ((DateTime.Now - LastUsed) > Recharge);

        public ushort Icon { get; set; }

        [Category("Timing")]
        public DateTime LastUsed { get; set; }

        [Category("Timing")]
        public TimeSpan Recharge { get; set; }

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
    }
}
