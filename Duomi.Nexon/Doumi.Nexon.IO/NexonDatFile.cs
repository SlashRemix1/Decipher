using System.IO;
using System.Text;

namespace Doumi.Nexon.IO
{
	public class NexonDatFile : NexonFileFormat<NexonDatFile>
	{
		public class Entry
		{
			private string name;

			public uint Addr
			{
				get;
				set;
			}

			public byte[] Data
			{
				get;
				set;
			}

			public string Name
			{
				get
				{
					return name;
				}
				set
				{
					int num = value.IndexOf("\0");
					name = ((num != -1) ? value.Substring(0, num) : value);
				}
			}
		}

		private Entry[] entries;

		public int Count => entries.Length - 1;

		public Entry this[int index] => entries[index];

		public override void Serialize(BinaryReader reader)
		{
			entries = new Entry[reader.ReadUInt32()];
			for (int i = 0; i < entries.Length; i++)
			{
				entries[i] = new Entry();
				entries[i].Addr = reader.ReadUInt32();
				entries[i].Name = Encoding.ASCII.GetString(reader.ReadBytes(13));
			}
			for (int j = 0; j < Count; j++)
			{
				uint addr = entries[j].Addr;
				uint addr2 = entries[j + 1].Addr;
				entries[j].Data = reader.ReadBytes((int)(addr2 - addr));
			}
		}

		public override void Serialize(BinaryWriter writer)
		{
		}
	}
}
