using Akorade.Types;
using Doumi.Nexon.Net;
using Doumi.PuppyServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Akorade
{
    public class Server
    {
        private IPEndPoint clientEndPoint;
        private IPEndPoint serverEndPoint;
        private Socket socket;
        private Thread thread;
        private mainForm main;

        //dialog
        private ushort clr1;
        private ushort clr2;
        private byte dialogType;
        private uint guid;
        private ushort img1;
        private ushort img2;
        private string name;
        private byte objectType;
        private string text;
        private byte unkb;
        private byte unkc;
        byte numDia;
        private byte unka;

        public ClientTab Tab { get; private set; }
        public Server server { get; private set; }
        public static Dictionary<string, string> translationDic = new Dictionary<string, string>();

        public async Task requestTranslationEdit(string trans, string edit)
        {
            PuppyPacket puppie = new PuppyPacket((byte)PuppyClientCodes.RequestEdit);
            puppie.WriteString16(trans);
            puppie.WriteString16(edit);
            _ = main.pup.SendAsync(puppie);

            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
            main.pup.TaskCompletionSources.Add((trans, taskCompletionSource));

            string result = (await taskCompletionSource.Task) as string;
            translationDic[trans] = result;
        }

        public async Task<string> checkTransExist(string trans)
        {
            if (!translationDic.ContainsKey(trans))
            {
                PuppyPacket puppie = new PuppyPacket((byte)PuppyClientCodes.RequestTranslation);
                puppie.WriteString16(trans);
                _ = main.pup.SendAsync(puppie);

                TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
                main.pup.TaskCompletionSources.Add((trans, taskCompletionSource));

                string result = (await taskCompletionSource.Task) as string;
                translationDic[trans] = result;
                return result;
            }
            return translationDic[trans];
        }

        public Server(IPAddress address, int port, mainForm form)
        {
            clientEndPoint = new IPEndPoint(IPAddress.Loopback, port);
            serverEndPoint = new IPEndPoint(address, port);
            main = form;
            server = this;
        }

        public void Start()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(clientEndPoint);
            socket.Listen(30);
            thread = new Thread(new ThreadStart(Accept));
            thread.Start();
        }

        protected void PatronConnected(Client client)
        {
            client.Initialize();
            client.HookClient(14, new NexonPatron<Client>.Handler(ClientHandler_0x0E_PublicChat));
            client.HookClient(16, new NexonPatron<Client>.Handler(ClientHandler_0x10_ClientJoin));
            client.HookClient(57, new NexonPatron<Client>.Handler(ClientHander_0x39_GossipMenu));
            client.HookClient(58, new NexonPatron<Client>.Handler(ClientHandler_0x3A_DialogOptions));


            client.HookServer(0, new NexonPatron<Client>.Handler(ServerHandler_0x00_SeedHash));
            client.HookServer(3, new NexonPatron<Client>.Handler(ServerHandler_0x03_Redirect));
            client.HookServer(5, new NexonPatron<Client>.Handler(ServerHandler_0x05_UserID));
            client.HookServer(7, new NexonPatron<Client>.Handler(ServerHandler_0x07_DisplayItemMonster));
            client.HookServer(10, new NexonPatron<Client>.Handler(ServerHandler_0x0A_SystemMessage));
            client.HookServer(13, new NexonPatron<Client>.Handler(PacketHandler_0x0D_PublicChat));
            client.HookServer(15, new NexonPatron<Client>.Handler(ServerHandler_0x0F_AddItem));
            client.HookServer(21, new NexonPatron<Client>.Handler(ServerHandler_0x15_MapInfo));
            client.HookServer(23, new NexonPatron<Client>.Handler(ServerHandler_0x17_AddSpell));
            client.HookServer(24, new NexonPatron<Client>.Handler(ServerHandler_0x18_RemoveSpell));
            client.HookServer(44, new NexonPatron<Client>.Handler(ServerHandler_0x2C_AddSkill));
            client.HookServer(45, new NexonPatron<Client>.Handler(ServerHandler_0x2D_RemoveSkill));
            client.HookServer(47, new NexonPatron<Client>.Handler(ServerHandler_0x2F_MerchantMenu));
            client.HookServer(48, new NexonPatron<Client>.Handler(ServerHandler_0x30_PursuitDialog));
            client.HookServer(51, new NexonPatron<Client>.Handler(ServerHandler_0x33_DisplayUser));
            client.HookServer(100, new NexonPatron<Client>.Handler(ServerHandler_0x64_Unknown));
        }

        private void ServerHandler_0x05_UserID(Client client, NexonPacket packet)
        {
            uint num1;
            byte num2;
            num1 = packet.ReadU4();
            packet.ReadU1();
            packet.ReadU1();
            num2 = packet.ReadU1();
            client.ID = num1;
            client.AddClassFlag((MythosiaClass)num2);
            client.Client.Send(packet);
        }

        private void ServerHandler_0x18_RemoveSpell(Client patron, NexonPacket packet)
        {
            Spell spell = new Spell(packet);
            patron.Client.Send(packet);
            patron.Spellbook.Remove(spell.Slot);
        }

        private async void ServerHandler_0x17_AddSpell(Client patron, NexonPacket packet)
        {
            byte slot = packet.ReadU1();
            ushort icon = packet.ReadU2();
            byte type = packet.ReadU1();
            string text = packet.ReadC1();
            string note = packet.ReadC1();
            byte span = packet.ReadU1();
            byte unka = packet.ReadU1();
            int recharge = packet.ReadS4();
            int castTime = packet.ReadS4();
            byte currentLvl = 0;
            byte maxLvl = 0;
            Match match = Regex.Match(text, @"^(.*?) \([a-zA-Z]+:(\d+)/(\d+)\)$");
            if (match.Success)
            {
                text = match.Groups[1].Value;
                byte.TryParse(match.Groups[2].Value, out currentLvl);
                byte.TryParse(match.Groups[3].Value, out maxLvl);
            }
            string transSkill = await checkTransExist(text);
            Spell spell = new Spell(slot, icon, type, text, note, span, currentLvl, maxLvl, DateTime.MinValue, transSkill);

            NexonClientPacket packet2 = new NexonClientPacket(patron, 23);
            packet2.WriteU1(slot);
            packet2.WriteU2(icon);
            packet2.WriteU1(type);
            packet2.WriteC1(spell.EnglishName + " (Lev:" + currentLvl + "/" + maxLvl + ")");
            packet2.WriteC1(note);
            packet2.WriteU1(span);
            packet2.WriteU1(unka);
            packet2.WriteS4(recharge);
            packet2.WriteS4(castTime);
            patron.Client.Send(packet2);

            patron.Spellbook.Add(spell);

        }

        private async void PacketHandler_0x0D_PublicChat(Client patron, NexonPacket packet)
        {
            packet.Decrypt();
            byte num = packet.ReadU1();
            uint key = packet.ReadU4();
            string str = packet.ReadC1();
            switch (num)
            {
                //Speak
                case 0:
                    string name = str.Substring(0, str.IndexOf(": "));
                    string chat = str.Substring(str.LastIndexOf(": "));
                    string transChat = await checkTransExist(chat);
                    if (patron.langSelect == "Korean")
                        patron.Client.Send(packet);
                    if (patron.langSelect == "English" && patron.ID != key)
                        patron.SpeakGoogle(name + transChat, key, 0);
                    if (patron.langSelect == "English" && patron.ID == key)
                        patron.Client.Send(packet);
                    break;
                //Shout
                case 1:
                    string name1 = str.Substring(0, str.IndexOf("! "));
                    string chat1 = str.Substring(str.LastIndexOf("! "));
                    string transChat1 = await checkTransExist(chat1);
                    if (patron.langSelect == "Korean")
                        patron.Client.Send(packet);
                    if (patron.langSelect == "English" && patron.ID != key)
                        patron.SpeakGoogle(name1 + transChat1, key, 0);
                    if (patron.langSelect == "English" && patron.ID == key)
                        patron.Client.Send(packet);
                    break;
                //Chant
                case 2:
                    patron.Client.Send(packet);
                    break;
            }
        }

        private void ClientHandler_0x0E_PublicChat(Client patron, NexonPacket packet)
        {
            packet.Decrypt();
            byte s = packet.ReadU1();
            string message = packet.ReadC1();

            if (!message.StartsWith("/"))
                patron.Server.Send(packet);
            else
                patron.Commands(message.Substring(1));
        }

        private void ClientHander_0x39_GossipMenu(Client patron, NexonPacket packet)
        {
            byte gameObjectType = packet.ReadU1();
            int ObjectId = packet.ReadS4();
            ushort pursuitId = packet.ReadU2();
            string arguments = packet.ReadC1();

            var kvp = translationDic.FirstOrDefault(entry => entry.Value == arguments);
            if (kvp.Key != null)
            {
                NexonClientPacket packet2 = new NexonClientPacket(patron, 57);
                packet2.WriteU1(gameObjectType);
                packet2.WriteS4(ObjectId);
                packet2.WriteU2(pursuitId);
                packet2.WriteC1(kvp.Key);
                packet2.WriteU1(0);
                patron.Server.Send(packet2);
            }
            else
                patron.Server.Send(packet);
        }

        private void ServerHandler_0x0F_AddItem(Client patron, NexonPacket packet)
        {
            Item item = new Item(packet);
            patron.Inventory.Add(item);
            patron.Client.Send(packet);
        }

        private async void ServerHandler_0x30_PursuitDialog(Client patron, NexonPacket packet)
        {
            var options = new List<string>();
            dialogType = packet.ReadU1();
            if (dialogType == 0)
            {
                objectType = packet.ReadU1();
                guid = packet.ReadU4();
                img1 = packet.ReadU2();
                clr1 = packet.ReadU2();
                unkb = packet.ReadU1();
                img2 = packet.ReadU2();
                clr2 = packet.ReadU2();
                unkc = packet.ReadU1();
                name = packet.ReadC1();
                text = packet.ReadC2();

                NexonServerPacket packet2 = new NexonServerPacket(patron, 48);
                packet2.WriteU1(dialogType);
                packet2.WriteU1(objectType);
                packet2.WriteU4(guid);
                packet2.WriteU2(img1);
                packet2.WriteU2(clr1);
                packet2.WriteU1(unkb);
                packet2.WriteU2(img2);
                packet2.WriteU2(clr2);
                packet2.WriteU1(unkc);
                string transname = await checkTransExist(name);
                packet2.WriteC1(transname);
                string transtext = await checkTransExist(text);
                string fixColor = Regex.Replace(transtext, @"{= ", "{=");
                packet2.WriteC2(fixColor);
                patron.Client.Send(packet2);

                Program.Form.ThreadSafeInvoke(() => patron.Tab.lblnpcName.Text = name);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.txtnpcName.Text = transname);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.lblkoreanText.Text = text);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.txtenglishNPCText.Text = fixColor);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.lstnpcOptions.Items.Clear());
                Program.Form.ThreadSafeInvoke(() => patron.Tab.lstKoreanNPCOptions.Items.Clear());
            }
            if (dialogType == 15)
            {
                patron.Client.Send(packet);
            }
            if (dialogType == 2)
            {
                objectType = packet.ReadU1();
                guid = packet.ReadU4(); ;
                img1 = packet.ReadU2();
                clr1 = packet.ReadU2();
                unkb = packet.ReadU1();
                img2 = packet.ReadU2();
                clr2 = packet.ReadU2();
                unkc = packet.ReadU1();
                name = packet.ReadC1();
                text = packet.ReadC2();
                byte num = packet.ReadU1();
                for (int index = 0; index < num; index++)
                    options.Add(packet.ReadC1());

                NexonServerPacket packet2 = new NexonServerPacket(patron, 48);
                packet2.WriteU1(dialogType);
                packet2.WriteU1(objectType);
                packet2.WriteU4(guid);
                packet2.WriteU2(img1);
                packet2.WriteU2(clr1);
                packet2.WriteU1(unkb);
                packet2.WriteU2(img2);
                packet2.WriteU2(clr2);
                packet2.WriteU1(unkc);

                //string to translate
                List<string> toTranslate = new List<string>();
                toTranslate.Add(name);
                toTranslate.Add(text);
                toTranslate.AddRange(options);

                //results of translation
                List<string> translated = new List<string>();
                //list of translation tasks
                List<Task<string>> tasks = new List<Task<string>>();
                //generate the translation tasks based on the toTranslate list
                toTranslate.ForEach(str => tasks.Add(checkTransExist(str)));
                //await completion of all translations
                translated = (await Task.WhenAll(tasks)).ToList();
                string fixColor = Regex.Replace(translated[1], @"{= ", "{=");

                //text is first in the list
                packet2.WriteC1(translated[0]);
                packet2.WriteC2(fixColor);
                packet2.WriteU1(num);


                //skip the first index, the rest are options
                for (int i = 0; i < options.Count; i++)
                {
                    string fixColors = Regex.Replace(translated[i + 2], @"{= ", "{=");
                    packet2.WriteC1(fixColors);
                }

                patron.Client.Send(packet2);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.lblnpcName.Text = name);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.txtnpcName.Text = translated[0]);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.lblkoreanText.Text = text);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.txtenglishNPCText.Text = translated[1]);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.lstnpcOptions.Items.Clear());
                Program.Form.ThreadSafeInvoke(() => patron.Tab.lstKoreanNPCOptions.Items.Clear());
                for (int i = 0; i < options.Count; i++)
                {
                    Program.Form.ThreadSafeInvoke(() => patron.Tab.lstKoreanNPCOptions.Items.Add(options[i]));
                    Program.Form.ThreadSafeInvoke(() => patron.Tab.lstnpcOptions.Items.Add(translated[i + 2]));
                }
            }
            if (dialogType != 0 && dialogType != 2)
            {
                patron.Client.Send(packet);
            }
        }

        private async void ServerHandler_0x2F_MerchantMenu(Client patron, NexonPacket packet)
        {
            var options = new List<string>();
            dialogType = packet.ReadU1();
            List<(string text, ushort pursuitId)> itemC = new List<(string text, ushort pursuitId)>();
            List<(ushort sprite, ushort color, uint itemCount, string itemName, string unk, string meta, uint? time)> itembuy = new List<(ushort sprite, ushort color, uint itemCount, string itemName, string unk, string meta, uint? time)>();
            List<(byte unka, ushort sprite, byte unkb, string name)> skillsSpells = new List<(byte unka, ushort sprite, byte unkb, string name)>();

            if (dialogType == 0)
            {
                objectType = packet.ReadU1();
                guid = packet.ReadU4();
                unka = packet.ReadU1();
                img1 = packet.ReadU2();
                clr1 = packet.ReadU2();
                unkb = packet.ReadU1();
                img2 = packet.ReadU2();
                clr2 = packet.ReadU2();
                unkc = packet.ReadU1();
                name = packet.ReadC1();
                text = packet.ReadC2();
                numDia = packet.ReadU1();

                for (int index = 0; index < numDia; ++index)
                    itemC.Add((packet.ReadC1(), packet.ReadU2()));

                NexonServerPacket packet2 = new NexonServerPacket(patron, 47);
                packet2.WriteU1(dialogType);
                packet2.WriteU1(objectType);
                packet2.WriteU4(guid);
                packet2.WriteU1(unka);
                packet2.WriteU2(img1);
                packet2.WriteU2(clr1);
                packet2.WriteU1(unkb);
                packet2.WriteU2(img2);
                packet2.WriteU2(clr2);
                packet2.WriteU1(unkc);

                //string to translate
                List<string> toTranslate = new List<string>();
                toTranslate.Add(name);
                toTranslate.Add(text);
                //loop through the tuple pairs and add the text to be translated
                for (int i = 0; i < itemC.Count; i++)
                    toTranslate.Add(itemC[i].text);

                //results of translation
                List<string> translated = new List<string>();
                //list of translation tasks
                List<Task<string>> tasks = new List<Task<string>>();
                //generate the translation tasks based on the toTranslate list
                toTranslate.ForEach(str => tasks.Add(checkTransExist(str)));
                //await completion of all translations
                translated = (await Task.WhenAll(tasks)).ToList();

                //name is first in the list
                packet2.WriteC1(translated[0]);
                packet2.WriteC2(translated[1]);
                packet2.WriteU1(numDia);

                //skip the first index, the rest are options
                for (int i = 0; i < itemC.Count; i++)
                {
                    packet2.WriteC1(translated[i + 2]);
                    //itemC starts at index 0, so subtract 1
                    packet2.WriteU2(itemC[i].pursuitId);
                }
                patron.Client.Send(packet2);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.lblnpcName.Text = name);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.txtnpcName.Text = translated[0]);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.lblkoreanText.Text = text);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.txtenglishNPCText.Text = translated[1]);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.lstnpcOptions.Items.Clear());
                Program.Form.ThreadSafeInvoke(() => patron.Tab.lstKoreanNPCOptions.Items.Clear());
                for (int i = 0; i < itemC.Count; i++)
                {
                    Program.Form.ThreadSafeInvoke(() => patron.Tab.lstKoreanNPCOptions.Items.Add(itemC[i].text));
                    Program.Form.ThreadSafeInvoke(() => patron.Tab.lstnpcOptions.Items.Add(translated[i + 2]));
                }
            }

            if (dialogType == 2)
            {
                objectType = packet.ReadU1();
                guid = packet.ReadU4();
                unka = packet.ReadU1();
                img1 = packet.ReadU2();
                clr1 = packet.ReadU2();
                unkb = packet.ReadU1();
                img2 = packet.ReadU2();
                clr2 = packet.ReadU2();
                unkc = packet.ReadU1();
                name = packet.ReadC1();
                text = packet.ReadC2();
                numDia = packet.ReadU1();
                ushort thisPursuitId = packet.ReadU2();


                NexonServerPacket packet2 = new NexonServerPacket(patron, 47);
                packet2.WriteU1(dialogType);
                packet2.WriteU1(objectType);
                packet2.WriteU4(guid);
                packet2.WriteU1(unka);
                packet2.WriteU2(img1);
                packet2.WriteU2(clr1);
                packet2.WriteU1(unkb);
                packet2.WriteU2(img2);
                packet2.WriteU2(clr2);
                packet2.WriteU1(unkc);
                string transname = await checkTransExist(name);
                packet2.WriteC1(transname);
                string transtext = await checkTransExist(text);
                string fixColor = Regex.Replace(transtext, @"{= ", "{=");
                packet2.WriteC2(fixColor);
                packet2.WriteU2(thisPursuitId);
                patron.Client.Send(packet2);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.lblnpcName.Text = name);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.txtnpcName.Text = transname);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.lblkoreanText.Text = text);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.txtenglishNPCText.Text = fixColor);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.lstnpcOptions.Items.Clear());
                Program.Form.ThreadSafeInvoke(() => patron.Tab.lstKoreanNPCOptions.Items.Clear());
            }
            if (dialogType == 7 || dialogType == 6)
            {
                objectType = packet.ReadU1();
                guid = packet.ReadU4();
                unka = packet.ReadU1();
                img1 = packet.ReadU2();
                clr1 = packet.ReadU2();
                unkb = packet.ReadU1();
                img2 = packet.ReadU2();
                clr2 = packet.ReadU2();
                unkc = packet.ReadU1();
                name = packet.ReadC1();
                text = packet.ReadC2();
                ushort thisPursuitId = packet.ReadU2();
                ushort count = packet.ReadU2();

                for (int i = 0; i < count; i++)
                {
                    byte unka = packet.ReadU1();
                    ushort sprite = packet.ReadU2();
                    byte unkb = packet.ReadU1();
                    string name = packet.ReadC1();
                    skillsSpells.Add((unka, sprite, unkb, name));
                }

                NexonServerPacket packet2 = new NexonServerPacket(patron, 47);
                packet2.WriteU1(dialogType);
                packet2.WriteU1(objectType);
                packet2.WriteU4(guid);
                packet2.WriteU1(unka);
                packet2.WriteU2(img1);
                packet2.WriteU2(clr1);
                packet2.WriteU1(unkb);
                packet2.WriteU2(img2);
                packet2.WriteU2(clr2);
                packet2.WriteU1(unkc);
                packet2.WriteC1(name);
                //string to translate
                List<string> toTranslate = new List<string>();
                toTranslate.Add(text);
                toTranslate.AddRange(skillsSpells.Select(obj => obj.name));

                //results of translation
                List<string> translated = new List<string>();
                //list of translation tasks
                List<Task<string>> tasks = new List<Task<string>>();
                //generate the translation tasks based on the toTranslate list
                toTranslate.ForEach(str => tasks.Add(checkTransExist(str)));
                //await completion of all translations
                translated = (await Task.WhenAll(tasks)).ToList();

                //text is first in the list
                packet2.WriteC2(translated[0]);
                packet2.WriteU2(thisPursuitId);
                packet2.WriteU2(count);

                for (int i = 0; i < count; i++)
                {
                    packet2.WriteU1(skillsSpells[i].unka);
                    packet2.WriteU2(skillsSpells[i].sprite);
                    packet2.WriteU1(skillsSpells[i].unkb);
                    packet2.WriteC1(translated[i+1]);
                }
                patron.Client.Send(packet2);
                //Program.Form.ThreadSafeInvoke(() => patron.Form.txtnpcDialog.Text = text + Environment.NewLine + Environment.NewLine + translated[0]);
            }
            if (dialogType == 4)
            {
                objectType = packet.ReadU1();
                guid = packet.ReadU4();
                unka = packet.ReadU1();
                img1 = packet.ReadU2();
                clr1 = packet.ReadU2();
                unkb = packet.ReadU1();
                img2 = packet.ReadU2();
                clr2 = packet.ReadU2();
                unkc = packet.ReadU1();
                name = packet.ReadC1();
                text = packet.ReadC2();

                ushort thisPursuitId = packet.ReadU2();
                ushort count = packet.ReadU2();

                for (int i = 0; i < count; i++)
                {
                    ushort sprite = packet.ReadU2();
                    ushort color = packet.ReadU2();
                    uint itemCount = packet.ReadU4();
                    string itemName = packet.ReadC1();
                    string unk = packet.ReadC1();
                    string meta = packet.ReadC1();
                    uint? time = (packet.ReadU1() == 0) ? null : new uint?(packet.ReadU4());
                    itembuy.Add((sprite, color, itemCount, itemName, unk, meta, time));
                }

                NexonServerPacket packet2 = new NexonServerPacket(patron, 47);
                packet2.WriteU1(dialogType);
                packet2.WriteU1(objectType);
                packet2.WriteU4(guid);
                packet2.WriteU1(unka);
                packet2.WriteU2(img1);
                packet2.WriteU2(clr1);
                packet2.WriteU1(unkb);
                packet2.WriteU2(img2);
                packet2.WriteU2(clr2);
                packet2.WriteU1(unkc);

                //string to translate
                List<string> toTranslate = new List<string>();
                toTranslate.Add(name);
                toTranslate.Add(text);
                toTranslate.AddRange(itembuy.Select(obj => obj.itemName));

                //results of translation
                List<string> translated = new List<string>();
                //list of translation tasks
                List<Task<string>> tasks = new List<Task<string>>();
                //generate the translation tasks based on the toTranslate list
                toTranslate.ForEach(str => tasks.Add(checkTransExist(str)));
                //await completion of all translations
                translated = (await Task.WhenAll(tasks)).ToList();

                //text is second in the list
                packet2.WriteC1(translated[0]);
                packet2.WriteC2(translated[1]);
                packet2.WriteU2(thisPursuitId);
                packet2.WriteU2(count);


                for (int i = 0; i < count; i++)
                {
                    packet2.WriteU2(itembuy[i].sprite);
                    packet2.WriteU2(itembuy[i].color);
                    packet2.WriteU4(itembuy[i].itemCount);
                    packet2.WriteC1(translated[i + 2]);
                    packet2.WriteC1(itembuy[i].unk);
                    packet2.WriteC1(itembuy[i].meta);

                    if (itembuy[i].time == null)
                        packet2.WriteU1(0);
                    else
                        packet2.WriteU4(itembuy[i].time.Value);
                }
                patron.Client.Send(packet2);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.lblnpcName.Text = name);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.txtnpcName.Text = translated[0]);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.lblkoreanText.Text = text);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.txtenglishNPCText.Text = translated[1]);
                Program.Form.ThreadSafeInvoke(() => patron.Tab.lstnpcOptions.Items.Clear());
                Program.Form.ThreadSafeInvoke(() => patron.Tab.lstKoreanNPCOptions.Items.Clear());
                for (int i = 0; i < count; i++)
                {
                    Program.Form.ThreadSafeInvoke(() => patron.Tab.lstKoreanNPCOptions.Items.Add(itembuy[i].itemName));
                    Program.Form.ThreadSafeInvoke(() => patron.Tab.lstnpcOptions.Items.Add(translated[i + 2]));
                }

            }
            if (dialogType == 9)
            {
                objectType = packet.ReadU1();
                guid = packet.ReadU4();
                unka = packet.ReadU1();
                img1 = packet.ReadU2();
                clr1 = packet.ReadU2();
                unkb = packet.ReadU1();
                img2 = packet.ReadU2();
                clr2 = packet.ReadU2();
                unkc = packet.ReadU1();
                name = packet.ReadC1();
                text = packet.ReadC2();
                ushort thisPursuitId = packet.ReadU2();

                NexonServerPacket packet2 = new NexonServerPacket(patron, 47);
                packet2.WriteU1(dialogType);
                packet2.WriteU1(objectType);
                packet2.WriteU4(guid);
                packet2.WriteU1(unka);
                packet2.WriteU2(img1);
                packet2.WriteU2(clr1);
                packet2.WriteU1(unkb);
                packet2.WriteU2(img2);
                packet2.WriteU2(clr2);
                packet2.WriteU1(unkc);
                string transName = await checkTransExist(name);
                packet2.WriteC1(transName);
                string transtext = await checkTransExist(text);
                packet2.WriteC2(transtext);
                packet2.WriteU2(thisPursuitId);
                IEnumerable<byte> source = from e in patron.Skillbook
                                           where e != null
                                           select e.Slot;
                packet2.WriteU1((byte)source.Count<byte>());
                foreach (byte num in source)
                {
                    packet2.WriteU1(num);
                }
                packet2.WriteU1(0);
                packet2.Length = packet2.Offset;
                patron.Client.Send(packet2);
            }
            /*if (dialogType == 5)
            {
                objectType = packet.ReadU1();
                guid = packet.ReadU4();
                unka = packet.ReadU1();
                img1 = packet.ReadU2();
                clr1 = packet.ReadU2();
                unkb = packet.ReadU1();
                img2 = packet.ReadU2();
                clr2 = packet.ReadU2();
                unkc = packet.ReadU1();
                name = packet.ReadC1();
                text = packet.ReadC2();
                ushort thisPursuitId = packet.ReadU2();

                NexonServerPacket packet2 = new NexonServerPacket(patron, 47);
                packet2.WriteU1(dialogType);
                packet2.WriteU1(objectType);
                packet2.WriteU4(guid);
                packet2.WriteU1(unka);
                packet2.WriteU2(img1);
                packet2.WriteU2(clr1);
                packet2.WriteU1(unkb);
                packet2.WriteU2(img2);
                packet2.WriteU2(clr2);
                packet2.WriteU1(unkc);
                packet2.WriteC1(name);
                string transtext = await checkTransExist(text);
                packet2.WriteC2(transtext);
                packet2.WriteU2(thisPursuitId);
                IEnumerable<byte> source = from e in patron.Inventory
                                           where e != null
                                           select e.Slot;
                packet2.WriteU1((byte)source.Count<byte>());
                foreach (byte num in source)
                {
                    packet2.WriteU1(num);
                }
                packet2.WriteU1(0);
                packet2.Length = packet2.Offset;
                patron.Client.Send(packet2);
                //Program.Form.ThreadSafeInvoke(() => patron.Form.txtnpcDialog.Text = text + Environment.NewLine + Environment.NewLine + transtext);
            }*/
            //If you make a packet handler, be sure to add it here so it doesnt overlap
            if (dialogType != 0 && dialogType != 4 && dialogType != 6 && dialogType != 7 && dialogType != 9 && dialogType != 2)
            {
                patron.Client.Send(packet);
            }
        }

        private void ServerHandler_0x64_Unknown(Client patron, NexonPacket packet)
        {
            packet.Decrypt();
            byte num = packet.ReadU1();
            if (packet.ReadU1() != 1)
            {
                patron.Client.Send(packet);
            }
            else
            {
                NexonClientPacket packet2 = new NexonClientPacket(patron, 0x6a);
                packet2.WriteU1(num);
                packet2.WriteU1(2);
                packet2.WriteU1(1);
                patron.Server.Send(packet2);
            }
        }

        private void ServerHandler_0x00_SeedHash(Client patron, NexonPacket packet)
        {
            if (packet.ReadU1() == 0)
            {
                packet.ReadU4();
                patron.Seed = packet.ReadU1();
                patron.Hash = packet.ReadB1();
            }
            patron.Client.Send(packet);
        }

        private void ClientHandler_0x3A_DialogOptions(Client patron, NexonPacket packet)
        {
            packet.ReadU1();
            uint num = packet.ReadU4();
            uint num2 = packet.ReadU4();
            packet.ReadU1();
            uint num3 = packet.ReadU1();
            if (((num == 0x1389) && (num2 == 2)) && ((num3 > 0) && (num3 <= 5)))
            {
                patron.Server.Send(packet);
            }
            else
            {
                patron.Server.Send(packet);
            }
        }

        private void ServerHandler_0x33_DisplayUser(Client patron, NexonPacket packet)
        {
            Aisling aisling = new Aisling(packet);
            patron.Map.Aislings.AddOrUpdate(aisling.Guid, aisling, (key, value) => value.CopyFrom(aisling));
            packet.Code = patron.Server33;
            patron.Client.Send(packet);
        }

        private void ServerHandler_0x2D_RemoveSkill(Client client, NexonPacket packet)
        {
            Skill skill = new Skill(packet);
            client.Client.Send(packet);
            client.Skillbook.Remove(skill.Slot);
        }

        private async void ServerHandler_0x2C_AddSkill(Client client, NexonPacket packet)
        {
            byte slot = packet.ReadU1();
            ushort icon = packet.ReadU2();
            string text = packet.ReadC1();
            int recharge = packet.ReadS4();
            int castTime = packet.ReadS4();
            byte currentLvl = 0;
            byte maxLvl = 0;
            Match match = Regex.Match(text, @"^(.*?) \([a-zA-Z]+:(\d+)/(\d+)\)$");
            if (match.Success)
            {
                text = match.Groups[1].Value;
                byte.TryParse(match.Groups[2].Value, out currentLvl);
                byte.TryParse(match.Groups[3].Value, out maxLvl);
            }

            string transSkill = await checkTransExist(text);
            Skill skill = new Skill(slot, text, icon, currentLvl, maxLvl, DateTime.MinValue, transSkill);

            NexonClientPacket packet2 = new NexonClientPacket(client, 44);
            packet2.WriteU1(slot);
            packet2.WriteU2(icon);
            packet2.WriteC1(transSkill + " (Lev:" + currentLvl + "/" + maxLvl + ")");
            packet2.WriteS4(recharge);
            packet2.WriteS4(castTime);
            client.Client.Send(packet2);
            client.Skillbook.Add(skill);
        }

        public bool loadedSkills = false;

        private void ServerHandler_0x15_MapInfo(Client patron, NexonPacket packet)
        {
            loadedSkills = false;
            Program.Form.ThreadSafeInvoke(() => patron.Tab.lstEnglishSkill.Items.Clear());
            Program.Form.ThreadSafeInvoke(() => patron.Tab.lstKoreanSkill.Items.Clear());
            Program.Form.ThreadSafeInvoke(() => patron.Tab.lstEngSpell.Items.Clear());
            Program.Form.ThreadSafeInvoke(() => patron.Tab.lstKoreanSpell.Items.Clear());
            ushort file = packet.ReadU2();
            byte cols = packet.ReadU1();
            byte rows = packet.ReadU1();
            byte flag = packet.ReadU1();
            cols = (byte)(cols | (packet.ReadU1() << 8));
            rows = (byte)(rows | (packet.ReadU1() << 8));
            ushort hash = packet.ReadU2();
            string name = packet.ReadC1();
            Task<string> task = checkTransExist(name);
            task.Wait();
            string engName = task.Result;

            Map field = new Map(file, cols, rows, flag, hash, name, engName);

            NexonServerPacket packet2 = new NexonServerPacket(patron, 21);
            packet2.WriteU2(file);
            packet2.WriteU1((byte)cols);
            packet2.WriteU1((byte)rows);
            packet2.WriteU1(flag);
            packet2.WriteU1(0);
            packet2.WriteU1(0);
            packet2.WriteU1((byte)(hash / 256));
            packet2.WriteU1((byte)(hash % 256));
            packet2.WriteC1(engName);
            patron.Client.Send(packet2);

            patron.OnChangeMap(field);
            Program.Form.ThreadSafeInvoke(() => patron.Tab.lblMapName.Text = name);
            Program.Form.ThreadSafeInvoke(() => patron.Tab.txtMapName.Text = engName);
            if (!loadedSkills)
            {
                Program.Form.ThreadSafeInvoke(() => patron.Tab.LoadSkills());
                loadedSkills = true;
            }
        }

        private async void ServerHandler_0x07_DisplayItemMonster(Client patron, NexonPacket packet)
        {
            packet.Decrypt();
            ushort num3;
            ushort xpos;
            ushort ypos;
            uint guid;
            if (patron.Map != null)
            {
                num3 = packet.ReadU2();
                for (int i = 0; i < num3; i++)
                {
                    xpos = packet.ReadU2();
                    ypos = packet.ReadU2();
                    guid = packet.ReadU4();
                    ushort icon = packet.ReadU2();
                    if (icon >= 0x8000)
                    {
                        ushort tint = packet.ReadU2();
                        byte unka = packet.ReadU1();
                        string name = packet.ReadC1();
                        GroundItem mapItem = new GroundItem(xpos, ypos, guid, icon, tint, unka, name);
                        patron.Map.MapItems.AddOrUpdate(mapItem.Guid, mapItem, (key, value) => value.CopyFrom(mapItem));
                        DisplayItem(patron, mapItem);
                    }
                    else
                    {
                        uint unka = packet.ReadU4();
                        byte face = packet.ReadU1();
                        byte unkb = packet.ReadU1();
                        byte tint = packet.ReadU1();
                        byte unkc = packet.ReadU1();
                        byte type = packet.ReadU1();
                        if (type != 2)
                        {
                            Monster monster = new Monster(xpos, ypos, guid, icon, unka, face, unkb, tint, unkc, type);
                            patron.Map.Monsters.AddOrUpdate(monster.Guid, monster, (key, value) => value.CopyFrom(monster));
                            DisplayMonster(patron, monster);
                        }
                        else
                        {
                            string name = packet.ReadC1();
                            string engName = await checkTransExist(name);
                            Mundane mundane = new Mundane(xpos, ypos, guid, icon, unka, face, unkb, tint, unkc, type, name, engName);
                            patron.Map.Mundanes.AddOrUpdate(mundane.Guid, mundane, (key, value) => value.CopyFrom(mundane));
                            DisplayMundane(patron, mundane);
                        }
                    }
                }
            }
        }

        private async void ServerHandler_0x0A_SystemMessage(Client client, NexonPacket packet)
        {
            byte num = packet.ReadU1();
            string text = packet.ReadC2();
            switch (num)
            {
                //Private
                case 0:
                    string s = await checkTransExist(text);
                    string fixColor = Regex.Replace(s, @"{= ", "{=");
                    client.SendSystemMessage(0, fixColor);
                    break;
                //Orange Bar Variables
                case 3:
                    if (text.EndsWith("를 외워주셨습니다."))
                    {
                        string user = text.Substring(0, text.IndexOf("님이 "));
                        int x = text.IndexOf("님이 ") + "님이 ".Length;
                        int pX = text.LastIndexOf("를 외워주셨습니다.");
                        string spell = text.Substring(x, pX - x);
                        string transSpell = await checkTransExist(spell);
                        client.SendSystemMessage(3, user + " cast " + transSpell + " on you.");
                        return;
                    }
                    if (text.EndsWith("를 가합니다."))
                    {
                        string user = text.Substring(0, text.IndexOf("가 "));
                        int x = text.IndexOf("가 ") + "가 ".Length;
                        int pX = text.LastIndexOf("를 가합니다.");
                        string spell = text.Substring(x, pX - x);
                        string transSpell = await checkTransExist(spell);
                        client.SendSystemMessage(3, user + " cast " + transSpell + " on you.");
                        return;
                    }
                    if (text.EndsWith("님 그룹에 참여"))
                    {
                        string member = text.Substring(0, text.LastIndexOf("님 그룹에 참여"));
                        client.SendSystemMessage(3, member + " has joined the group.");
                        return;
                    }
                    if (text.EndsWith("능력이 향상되었습니다."))
                    {
                        string spell = text.Substring(0, text.LastIndexOf("능력이 향상되었습니다."));
                        string transSpell = await checkTransExist(spell);
                        client.SendSystemMessage(3, transSpell + " has improved.");
                        return;
                    }
                    if (text.EndsWith("를 외웠습니다."))
                    {
                        string spell = text.Substring(0, text.LastIndexOf("를 외웠습니다."));
                        string transSpell = await checkTransExist(spell);
                        client.SendSystemMessage(3, "You cast " + transSpell + ".");
                        return;
                    }
                    if (text.StartsWith("경험치가"))
                    {
                        if (int.TryParse(Regex.Match(text, @"(\d+) 올랐습니다.").Groups[1].Value, out int exp))
                        {
                            client.SendSystemMessage(3, "Experience increased by " + exp + ".");
                            return;
                        }
                    }
                    if (text.StartsWith("{=wAP가 "))
                    {
                        if (int.TryParse(Regex.Match(text, @"(\d+) 올랐습니다.").Groups[1].Value, out int exp))
                        {
                            client.SendSystemMessage(3, "Ability increased by " + exp + ".");
                            return;
                        }
                    }
                    if (text.EndsWith("(를) 받았습니다."))
                    {
                        string name = text.Substring(0, text.IndexOf("(를) 받았습니다."));
                        client.SendSystemMessage(3, "You have received " + name);
                        return;
                    }
                    if (text.EndsWith("님 그룹 탈퇴"))
                    {
                        string name = text.Substring(0, text.IndexOf("님 그룹 탈퇴"));
                        client.SendSystemMessage(3, name + " has left the group.");
                        return;
                    }
                    //Let's block worldshouts.
                    Match worldshout;
                    if ((worldshout = Regex.Match(text, "!{=[A-Za-z]", RegexOptions.Singleline)).Success)
                    {
                        return;
                    }
                    if ((worldshout = Regex.Match(text, "{=[A-Za-z]###", RegexOptions.Singleline)).Success)
                    {
                        return;
                    }
                    string s1 = await checkTransExist(text);
                    string fixColor1 = Regex.Replace(s1, @"{= ", "{=");
                    client.SendSystemMessage(3, fixColor1);
                    Program.Form.ThreadSafeInvoke(() => client.Tab.lstKoreanStatus.Items.Add(text));
                    Program.Form.ThreadSafeInvoke(() => client.Tab.lstEnglishStatus.Items.Add(s1));
                    return;
                //Nexon
                case 5:
                    return;
                //Server
                case 8:
                    Match senseMatch;
                    if (text.StartsWith("       *****  Sense"))
                    {
                        if ((senseMatch = Regex.Match(text, @"DEFENSE NATURE: *([\w ]+)", RegexOptions.Singleline)).Success)
                        {
                            if (senseMatch.Groups[1].Value == "무속성")
                                client.targetElement = "No attributes";
                            if (senseMatch.Groups[1].Value == "물")
                                client.targetElement = "Earth";
                            if (senseMatch.Groups[1].Value == "암흑")
                                client.targetElement = "Light";
                            if (senseMatch.Groups[1].Value == "번개")
                                client.targetElement = "Lightning";
                            if (senseMatch.Groups[1].Value == "금")
                                client.targetElement = "Dark";
                            if (senseMatch.Groups[1].Value == "신성")
                                client.targetElement = "Water";
                            if (senseMatch.Groups[1].Value == "불")
                                client.targetElement = "Water";
                            if (senseMatch.Groups[1].Value == "땅")
                                client.targetElement = "Wind";
                        }
                        if (client.targetElement == "No attributes")
                            client.SendSystemMessage(0, "No defensive attributes.");
                        else
                            client.SendSystemMessage(0, "Use " + client.targetElement + " Necklace.");
                    }
                    client.SendSystemMessage(8, text);
                    return;
                //Party
                case 11:
                    break;
                //Guild
                case 12:
                    Match match;
                    if ((match = Regex.Match(text, " ^<!([a-zA-Z]+)> (.*)$")).Success)
                    {
                        string guildMemberName = match.Groups[1].Value;
                        string guildMemberText = match.Groups[2].Value;
                        string guildTextEnglish = await checkTransExist(guildMemberText);
                        string format = string.Format("<!{0}> {1}", guildMemberName, guildTextEnglish);

                        client.SendSystemMessage(12, format);
                        return;
                    }
                    break;
            }
            client.Client.Send(packet);
        }

        private void ServerHandler_0x03_Redirect(Client client, NexonPacket packet)
        {
            packet.WriteU4(0x100007f);
            client.Client.Send(packet);
        }

        private void ClientHandler_0x10_ClientJoin(Client client, NexonPacket packet)
        {
            client.Seed = packet.ReadU1();
            client.Hash = packet.ReadB1();
            client.Name = packet.ReadC1();
            client.Tab.Text = client.Name;
            if (!client.Name.StartsWith("socket["))
            {
                Program.Form.AddTab(client.Tab);
            }


            client.Server.Send(packet);
        }

        private void Accept()
        {
            try
            {
                NexonSocket socket;
                bool flag3;
                goto Label_0097;
            Label_0007:
                socket = new NexonSocket(this.socket.Accept());
                Client client = new Client(socket, new NexonSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
                if (client.Client.Connected)
                {
                    client.Server.Connect(serverEndPoint);
                    if (client.Server.Connected)
                    {
                        PatronConnected(client);
                        client.Client.BeginReceiveHeader(new AsyncCallback(ClientHeaderResult), client);
                        client.Server.BeginReceiveHeader(new AsyncCallback(ServerHeaderResult), client);
                    }
                }
            Label_0097:
                flag3 = true;
                goto Label_0007;
            }
            catch { }
        }

        private void ClientHeaderResult(IAsyncResult result)
        {
            Client asyncState = result.AsyncState as Client;
            NexonSocket client = asyncState.Client;
            if (client.EndReceiveHeader(result) == 0)
            {
                PatronDisconnected(asyncState);
            }
            else if (client.HeaderComplete)
            {
                client.BeginReceivePacket(new AsyncCallback(ClientPacketResult), asyncState);
            }
            else
            {
                client.BeginReceiveHeader(new AsyncCallback(ClientHeaderResult), asyncState);
            }
        }

        private void ClientPacketResult(IAsyncResult result)
        {
            Client asyncState = result.AsyncState as Client;
            NexonSocket client = asyncState.Client;
            if (client.EndReceivePacket(result) == 0)
            {
                PatronDisconnected(asyncState);
            }
            else if (!client.PacketComplete)
            {
                client.BeginReceivePacket(new AsyncCallback(ClientPacketResult), asyncState);
            }
            else
            {
                asyncState.ClientPacketReceived(new NexonClientPacket(asyncState, client));
                client.BeginReceiveHeader(new AsyncCallback(ClientHeaderResult), asyncState);
            }
        }

        private void ServerHeaderResult(IAsyncResult result)
        {
            Client asyncState = result.AsyncState as Client;
            NexonSocket server = asyncState.Server;
            if (server.EndReceiveHeader(result) == 0)
            {
                PatronDisconnected(asyncState);
            }
            else if (server.HeaderComplete)
            {
                server.BeginReceivePacket(new AsyncCallback(ServerPacketResult), asyncState);
            }
            else
            {
                server.BeginReceiveHeader(new AsyncCallback(ServerHeaderResult), asyncState);
            }
        }

        private void ServerPacketResult(IAsyncResult result)
        {
            Client asyncState = result.AsyncState as Client;
            NexonSocket server = asyncState.Server;
            if (server.EndReceivePacket(result) == 0)
            {
                PatronDisconnected(asyncState);
            }
            else if (!server.PacketComplete)
            {
                server.BeginReceivePacket(new AsyncCallback(ServerPacketResult), asyncState);
            }
            else
            {
                asyncState.ServerPacketReceived(new NexonServerPacket(asyncState, server));
                server.BeginReceiveHeader(new AsyncCallback(ServerHeaderResult), asyncState);
            }
        }

        protected void PatronDisconnected(Client client)
        {
            Program.Form.RemoveTab(client.Tab);
            if (client.Client.Connected)
            {
                client.Client.Disconnect(false);
            }
            if (client.Server.Connected)
            {
                client.Server.Disconnect(false);
            }
        }

        internal void DisplayItem(Client client, GroundItem item)
        {
            NexonServerPacket packet2 = new NexonServerPacket(client, 7);
            packet2.WriteU2(1);
            packet2.WriteU2(item.X);
            packet2.WriteU2(item.Y);
            packet2.WriteU4(item.Guid);
            packet2.WriteU2(item.Icon);
            client.Client.Send(packet2);
        }

        internal void DisplayMonster(Client client, Monster monster)
        {
            NexonServerPacket packet2 = new NexonServerPacket(client, 7);
            packet2.WriteU2(1);
            packet2.WriteU2(monster.X);
            packet2.WriteU2(monster.Y);
            packet2.WriteU4(monster.Guid);
            packet2.WriteU2(monster.Icon);
            packet2.WriteU4(monster.UnkA);
            packet2.WriteU1(monster.Face);
            packet2.WriteU1(monster.UnkB);
            packet2.WriteU1(monster.Tint);
            packet2.WriteU1(monster.UnkC);
            packet2.WriteU1(monster.Type);
            client.Client.Send(packet2);
        }

        internal void DisplayMundane(Client client, Mundane mundane)
        {
            NexonServerPacket packet2 = new NexonServerPacket(client, 7);
            packet2.WriteU2(1);
            packet2.WriteU2(mundane.X);
            packet2.WriteU2(mundane.Y);
            packet2.WriteU4(mundane.Guid);
            packet2.WriteU2(mundane.Icon);
            packet2.WriteU4(mundane.UnkA);
            packet2.WriteU1(mundane.Face);
            packet2.WriteU1(mundane.UnkB);
            packet2.WriteU1(mundane.Tint);
            packet2.WriteU1(mundane.UnkC);
            packet2.WriteU1(mundane.Type);
            packet2.WriteC1(mundane.EngName);
            client.Client.Send(packet2);
        }
    }
}
