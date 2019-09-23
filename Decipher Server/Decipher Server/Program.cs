using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Decipher_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            int xmlCount = 0;
            int loaded = 0;
            if (!File.Exists("TextDictionary.dat"))
            {
                File.Create("TextDictionary.dat");
            }
            using (BinaryReader binaryReader = new BinaryReader(File.OpenRead("TextDictionary.dat")))
            {
                while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
                {
                    string[] array = binaryReader.ReadString().Split(new string[1]
                    {
                        " 8==) "
                    }, StringSplitOptions.None);
                    if (!Server.translationDic.ContainsKey(array[0]))
                    {
                        Server.translationDic.Add(array[0], array[1]);
                        loaded++;
                    }
                }
            }
            //Weapons
            string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\WeaponsXML\\")
                              .Where(p => p.EndsWith(".xml"))
                              .ToArray();
            foreach (var path in files)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/data/Weapons/Item");
                string korName = "", engName = "";
                foreach (XmlNode node in nodeList)
                {
                    korName = node.SelectSingleNode("Korean-Name").InnerText;
                    engName = node.SelectSingleNode("English-Name").InnerText;
                    if (!Server.translationDic.ContainsKey(korName))
                    {
                        Server.translationDic.Add(korName, engName);
                        xmlCount++;
                    }
                }
            }
            //SkillSpell
            string[] file2 = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\SKillSpellXML\\")
                  .Where(p => p.EndsWith(".xml"))
                  .ToArray();
            foreach (var path in file2)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/data/Skill_Spell");
                string korName = "", engName = "";
                foreach (XmlNode node in nodeList)
                {
                    korName = node.SelectSingleNode("Korean_Name").InnerText;
                    engName = node.SelectSingleNode("English_Name").InnerText;
                    if (!Server.translationDic.ContainsKey(korName))
                    {
                        Server.translationDic.Add(korName, engName);
                        xmlCount++;
                    }
                }
            }
            //SkillSpell
            string[] file3 = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\ItemsXML\\")
                  .Where(p => p.EndsWith(".xml"))
                  .ToArray();
            foreach (var path in file3)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/data/Item");
                string korName = "", engName = "";
                foreach (XmlNode node in nodeList)
                {
                    korName = node.SelectSingleNode("Korean_Name").InnerText;
                    engName = node.SelectSingleNode("English_Name").InnerText;
                    if (!Server.translationDic.ContainsKey(korName))
                    {
                        Server.translationDic.Add(korName, engName);
                        xmlCount++;
                    }
                }
            }
            new Server().Start();
            Console.WriteLine("Loaded " + xmlCount + " translations from XML into server memory.");
            Console.WriteLine("Loaded " + loaded + " translations from database into server memory.");
            Console.WriteLine("Server started...");
            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
