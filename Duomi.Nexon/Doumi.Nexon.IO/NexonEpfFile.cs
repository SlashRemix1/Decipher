using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Doumi.Nexon.IO
{
	public class NexonEpfFile : NexonFileFormat<NexonEpfFile>
	{
		public class Frame
		{
			public Rectangle Bounds;

			public byte[] Data;

			public int Address1;

			public int Address2;

			public Frame(BinaryReader reader)
			{
				int top = reader.ReadUInt16();
				int left = reader.ReadUInt16();
				int bottom = reader.ReadUInt16();
				int right = reader.ReadUInt16();
				Bounds = Rectangle.FromLTRB(left, top, right, bottom);
				Address1 = reader.ReadInt32();
				Address2 = reader.ReadInt32();
			}

			public unsafe Bitmap Render(NexonPalFile palette)
			{
				Bitmap bitmap = new Bitmap(Bounds.Width, Bounds.Height, PixelFormat.Format32bppArgb);
				BitmapData bitmapData = bitmap.LockBits(Bounds, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
				int* ptr = (int*)(void*)bitmapData.Scan0;
				for (int i = 0; i < Data.Length; i++)
				{
					byte b = Data[i];
					if (b != 0)
					{
						ptr[i] = palette[b];
					}
				}
				bitmap.UnlockBits(bitmapData);
				return bitmap;
			}
		}

		private Rectangle bounds;

		private Frame[] frames;

		private int address;

		private short unknown;

		public int Count => frames.Length;

		public Frame this[int index] => frames[index];

		public void RenderTo(ref Image[] images, NexonPalFile palette)
		{
			images = new Image[Count];
			for (int i = 0; i < Count; i++)
			{
				images[i] = frames[i].Render(palette);
			}
		}

		public override void Serialize(BinaryReader reader)
		{
			frames = new Frame[reader.ReadInt16()];
			bounds = new Rectangle(0, 0, reader.ReadInt16(), reader.ReadInt16());
			unknown = reader.ReadInt16();
			address = reader.ReadInt32();
			reader.BaseStream.Seek(address, SeekOrigin.Current);
			for (int i = 0; i < frames.Length; i++)
			{
				frames[i] = new Frame(reader);
			}
			Frame[] array = frames;
			foreach (Frame frame in array)
			{
				reader.BaseStream.Seek(frame.Address1 + 12, SeekOrigin.Begin);
				frame.Data = reader.ReadBytes(frame.Bounds.Width * frame.Bounds.Height);
			}
		}

		public override void Serialize(BinaryWriter writer)
		{
		}
	}
}
