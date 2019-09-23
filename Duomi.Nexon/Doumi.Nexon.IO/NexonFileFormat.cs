using System.IO;

namespace Doumi.Nexon.IO
{
	public abstract class NexonFileFormat<T> where T : NexonFileFormat<T>, new()
	{
		public static T FromBuffer(byte[] buffer)
		{
			using (MemoryStream stream = new MemoryStream(buffer))
			{
				return FromStream(stream);
			}
		}

		public static T FromStream(Stream stream)
		{
			using (BinaryReader reader = new BinaryReader(stream))
			{
				T val = new T();
				val.Serialize(reader);
				return val;
			}
		}

		public abstract void Serialize(BinaryReader reader);

		public abstract void Serialize(BinaryWriter writer);
	}
}
