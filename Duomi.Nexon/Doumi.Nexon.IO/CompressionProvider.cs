using Ionic.Zlib;

namespace Doumi.Nexon.IO
{
	public static class CompressionProvider
	{
		public static byte[] Deflate(byte[] buffer)
		{
			return ZlibStream.CompressBuffer(buffer);
		}

		public static byte[] Inflate(byte[] buffer)
		{
			return ZlibStream.UncompressBuffer(buffer);
		}
	}
}
