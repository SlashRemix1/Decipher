using Doumi.Nexon.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Akorade.Types
{
    public class SlotObjectCollection
            <TPatron, TSlot> : IEnumerable<TSlot>, IEnumerable where TPatron : NexonPatron<TPatron> where TSlot : SlotObject
    {
        private TPatron patron;
        private TSlot[] items;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event Action<TPatron, TSlot> ItemAdded;

        [field: DebuggerBrowsable(0), CompilerGenerated]
        public event Action<TPatron, byte> ItemRemoved;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event Action<TPatron, TSlot> ItemUsed;

        public SlotObjectCollection(TPatron patron, int capacity)
        {
            this.patron = patron;
            items = new TSlot[capacity + 1];
        }

        public void Add(TSlot item)
        {
            if (item != null)
            {
                this[item.Slot] = item;
                if (ItemAdded != null)
                {
                    ItemAdded(patron, item);
                }
            }
        }

        public IEnumerator<TSlot> GetEnumerator()
        {
            foreach (TSlot slot5__3 in items)
            {
                yield return slot5__3;
            }
        }

        public void Remove(byte slot)
        {
            this[slot] = default(TSlot);
            if (ItemRemoved != null)
            {
                ItemRemoved(patron, slot);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Use(byte slot)
        {
            TSlot local = this[slot];
            if ((local != null) && (ItemUsed != null))
            {
                ItemUsed(patron, local);
            }
        }

        public int Length =>
            items.Length;

        public TSlot this[int slot]
        {
            get
            {
                return items[slot];
            }
            set
            {
                items[slot] = value;
            }
        }
    }
}
