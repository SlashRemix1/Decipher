using System.IO;

namespace Doumi.Nexon.IO
{
	public class NexonPalFile : NexonFileFormat<NexonPalFile>
	{
		private int[] colors = new int[256];

		public int this[int index] => colors[index];

		public override void Serialize(BinaryReader reader)
		{
			for (int i = 0; i < colors.Length; i++)
			{
				colors[i] = (-16777216 | (reader.ReadByte() << 16) | (reader.ReadByte() << 8) | reader.ReadByte());
			}
		}

		public override void Serialize(BinaryWriter writer)
		{
			for (int i = 0; i < colors.Length; i++)
			{
				writer.Write((byte)(colors[i] >> 16));
				writer.Write((byte)(colors[i] >> 8));
				writer.Write((byte)colors[i]);
			}
		}
	}
}
