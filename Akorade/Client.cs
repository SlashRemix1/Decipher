using Akorade.Types;
using Doumi.Nexon.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Akorade
{
    public class Client : NexonPatron<Client>
    {
        internal NexonPatron<Client>.Handler[] ClientHandlers;
        internal NexonPatron<Client>.Handler[] ServerHandlers;
        public SlotObjectCollection<Client, Skill> Skillbook;
        public SlotObjectCollection<Client, Spell> Spellbook;
        public SlotObjectCollection<Client, Item> Inventory;
        public string Name;
        public uint ID;
        internal MythosiaClass classFlag;
        public ushort X;
        public ushort Y;
        public string langSelect = "Korean";
        public ClientTab Tab { get; private set; }
        public Server InnerServer { get; set; }
        public Akorade.Types.Map Map { get; set; }
        public mainForm main { get; set; }
        public string targetElement { get; set; }

        internal void AddClassFlag(MythosiaClass flag) => classFlag |= flag;


        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event Action FieldChanged;

        public Client(NexonSocket client, NexonSocket server) : base(client, server)
        {
            ClientHandlers = new NexonPatron<Client>.Handler[0x100];
            ServerHandlers = new NexonPatron<Client>.Handler[0x100];
            Client = client;
            Server = server;
            Tab = new ClientTab(this);
            Skillbook = new SlotObjectCollection<Client, Skill>(this, 90);
            Spellbook = new SlotObjectCollection<Client, Spell>(this, 90);
            Inventory = new SlotObjectCollection<Client, Item>(this, 60);
        }

        public void SendSystemMessage(byte type, string text)
        {
            NexonClientPacket packet = new NexonClientPacket(this, 10);
            packet.WriteU1(type);
            packet.WriteC2(text);
            packet.WriteU1(0);
            Client.Send(packet);
        }

        public void SpeakGoogle(string text, uint key, byte type)
        {
            NexonServerPacket packet = new NexonServerPacket(this, 13);
            packet.WriteU1(type);
            packet.WriteU4(key);
            packet.WriteC1(text);
            Client.Send(packet);
        }

        public void Speak(string text)
        {
            NexonClientPacket packet = new NexonClientPacket(this, 14);
            packet.WriteU1(0);
            packet.WriteC1(text);
            base.Server.Send(packet);
        }


        public void Commands(string command)
        {
            char[] separator = new char[] { ' ' };
            string[] strArray = command.Split(separator);
            string s = strArray[0];
            switch (s)
            {
                case "help":
                    Speak("도움");
                    return;
                case "practice":
                    Speak("연습");
                    return;
            }
        }

        public void OnChangeMap(Akorade.Types.Map map)
        {
            Map = map;
            if (FieldChanged != null)
            {
                FieldChanged();
            }
        }
    }
}
