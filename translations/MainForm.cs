using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace translations
{
    public partial class MainForm : Form
    {
        private const string Delimiter = " 8==) ";
        private const string TransPath = "TextDictionary.dat";
        private Dictionary<string, string> Translations = new Dictionary<string, string>();
        private string npcTransText;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!File.Exists("TextDictionary.dat"))
                File.Create("TextDictionary.dat");

            using (var reader = new BinaryReader(File.OpenRead(TransPath)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    string[] split = reader.ReadString().Split(new string[1] { Delimiter }, StringSplitOptions.None);
                    if (!Translations.ContainsKey(split[0]))
                        Translations.Add(split[0], split[1]);
                }
            }

            Korean.DataSource = new BindingSource(Translations, null);
            Korean.DisplayMember = "Key";
            Korean.ValueMember = "Value";

            //to make it show the correct line at startup
            Korean.SelectedIndex = 1;
            Korean.SelectedIndex = 0;
        }

        private void Korean_SelectedIndexChanged(object sender, EventArgs e)
        {
            string korean = ((KeyValuePair<string, string>)Korean.SelectedItem).Key;
            English.Text = $"{korean}{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}{Korean.SelectedValue}";
        }

        private void SaveLine_Click(object sender, EventArgs e)
        {
            string korean = ((KeyValuePair<string, string>)Korean.SelectedItem).Key;
            string english = English.Text.Split(new string[1] { $"{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}" }, StringSplitOptions.None)[1];
            Translations[korean] = english;

            using (var stream = File.Create(TransPath))
            using (var writer = new BinaryWriter(stream))
            {
                foreach (var kvp in Translations)
                    writer.Write($"{kvp.Key}{Delimiter}{kvp.Value}");
            }

            (Korean.DataSource as BindingSource)[Korean.SelectedIndex] = new KeyValuePair<string, string>(korean, english);
        }


        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string myString = txtSearch.Text;
            bool found = false;
            for (int i = 0; i <= Korean.Items.Count - 1; i++)
            {
                if (Korean.Items[i].ToString().Contains(myString))
                {
                    Korean.SetSelected(i, true);
                    found = true;
                    break;
                }
            }
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtreturnList.Items.Clear();
                string s = (from d in Translations where d.Value == textBox1.Text select d.Key).FirstOrDefault();
                for (int i = 0; i <= Korean.Items.Count - 1; i++)
                {
                    if (Korean.Items[i].ToString().Contains(s))
                    {
                        Korean.SetSelected(i, true);
                        txtreturnList.Items.Add(i);
                        break;
                    }
                }
            }
            catch { }
        }

        private void TxtreturnList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int s = Int32.Parse(txtreturnList.SelectedItem.ToString());
            for (int i = 0; i <= Korean.Items.Count - 1; i++)
            {
                if (i == s)
                {
                    Korean.SetSelected(i, true);
                    break;
                }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int count = 0;
            //Weapons
            string[] files = Directory.GetFiles(Application.StartupPath + "\\WeaponsXML\\")
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
                    txtreturnList.Items.Add(korName + " - " + engName);
                    if (!Translations.ContainsKey(korName))
                        Translations.Add(korName, engName);
                    count++;
                }
            }
            //SkillSpell
            string[] file2 = Directory.GetFiles(Application.StartupPath + "\\SKillSpellXML\\")
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
                    txtreturnList.Items.Add(korName + " - " + engName);
                    if (!Translations.ContainsKey(korName))
                        Translations.Add(korName, engName);
                    count++;
                }
            }
            //SkillSpell
            string[] file3 = Directory.GetFiles(Application.StartupPath + "\\ItemsXML\\")
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
                    txtreturnList.Items.Add(korName + " - " + engName);
                    if (!Translations.ContainsKey(korName))
                        Translations.Add(korName, engName);
                    count++;
                }
            }

            using (var stream = File.Create(TransPath))
            using (var writer = new BinaryWriter(stream))
            {
                foreach (var kvp in Translations)
                    writer.Write($"{kvp.Key}{Delimiter}{kvp.Value}");
            }
            label4.Text = "Loaded " + count + " entries from XML.";
        }
    }
}
