using Properties;
using System.IO;

namespace Doumi.Nexon.IO
{
    public class NexonMapFile : NexonFileFormat<NexonMapFile>
    {
        public class Tile
        {
            public readonly ushort Floor;

            public readonly bool Solid;

            public readonly ushort Wall0;

            public readonly ushort Wall1;

            public Tile(ushort floor, ushort wall0, ushort wall1)
            {
                Floor = floor;
                Wall0 = wall0;
                Wall1 = wall1;
                Solid = IsSolid;
            }

            public bool IsSolid => (Wall0 > 0 && (sotp[Wall0 - 1] & 15) == 15) || (Wall1 > 0 && (sotp[Wall1 - 1] & 15) == 15);

            /*private bool IsSolid(ushort wall0, ushort wall1)
			{
				if (wall0 == 0 && wall1 == 0)
				{
					return false;
				}
				if (wall0 == 0)
				{
                    try
                    {
                        return sotp[wall1 - 1] != 0;
                    }
                    catch { }
				}
				if (wall1 == 0)
				{
                    try
                    {
                        return sotp[wall0 - 1] != 0;
                    }
                    catch { }
				}
                try
                {
                    if (sotp[wall0 - 1] == 0)
                    {
                        return sotp[wall1 - 1] != 0;
                    }
                }
                catch { }
				return true;
			}
		}*/
        }

        private static byte[] sotp = Resources.sotp;

        private Tile[] tiles;

        public Tile this[int index]
        {
            get
            {
                try
                {
                    return tiles[index];
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                tiles[index] = value;
            }
        }

        public override void Serialize(BinaryReader reader)
        {
            int num = (int)(reader.BaseStream.Length / 6);
            tiles = new Tile[reader.BaseStream.Length / 6];
            for (int i = 0; i < num; i++)
            {
                tiles[i] = new Tile(reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16());
            }
        }

        public override void Serialize(BinaryWriter writer)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                writer.Write(tiles[i].Floor);
                writer.Write(tiles[i].Wall0);
                writer.Write(tiles[i].Wall1);
            }
        }
    }
}
