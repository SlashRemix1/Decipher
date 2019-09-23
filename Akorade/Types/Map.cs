using Doumi.Nexon.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akorade.Types
{
    public class Map
    {
        public ConcurrentDictionary<uint, Aisling> Aislings = new ConcurrentDictionary<uint, Aisling>();
        public NexonMapFile Area { get; set; }
        public ushort Cols { get; set; }
        public ushort File { get; set; }
        public byte Flag { get; set; }
        public ushort Hash { get; set; }
        public ConcurrentDictionary<uint, GroundItem> MapItems = new ConcurrentDictionary<uint, GroundItem>();
        public ConcurrentDictionary<uint, Monster> Monsters = new ConcurrentDictionary<uint, Monster>();
        public ConcurrentDictionary<uint, Mundane> Mundanes = new ConcurrentDictionary<uint, Mundane>();
        public string Name { get; set; }
        public ushort Rows { get; set; }
        public string englishName { get; set; }

        public Map(ushort file, byte cols, byte rows, byte flag, ushort hash, string name, string engName)
        {
            File = file;
            Cols = cols;
            Rows = rows;
            Flag = flag;
            Hash = hash;
            Name = name;
            englishName = engName;
            FileInfo info = new FileInfo("C:/Nexon/Legend of Darkness/maps/lod" + File + ".map");
            if (info.Exists)
            {
                Area = NexonFileFormat<NexonMapFile>.FromStream(info.OpenRead());
            }
        }
    }
}
