using Google.Apis.Auth.OAuth2;
using Google.Cloud.Translation.V2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Decipher_Server
{
    internal sealed class Client
    {
        private readonly object FileLock = new object();

        private readonly object Sync = new object();

        private byte[] ClientSegment;

        private List<byte> ClientBuffer;

        private const string Delimiter = " 8==) ";

        private static GoogleCredential cred = GoogleCredential.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\WhitePuppyTrans.json");

        private static TranslationClient client = TranslationClient.Create(cred);

        internal Socket ClientSocket
        {
            get;
        }

        internal bool Connected
        {
            get;
            set;
        }

        private Server Server
        {
            get;
        }

        internal Client(Server server, Socket socket)
        {
            Server = server;
            ClientSocket = socket;
            Connected = true;
            ClientSegment = new byte[65535];
            ClientBuffer = new List<byte>(65535);
            SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
            socketAsyncEventArgs.SetBuffer(ClientSegment, 0, 65535);
            socketAsyncEventArgs.Completed += ClientReceiveEvent;
            ClientSocket.ReceiveAsync(socketAsyncEventArgs);
        }

        public async Task<string> checkTransExist(string trans)
        {
            if (!Server.translationDic.ContainsKey(trans))
            {
                TranslationResult translationResult = await client.TranslateTextAsync(trans, "en");
                Server.translationDic.Add(trans, translationResult.TranslatedText);
                await Task.Run(delegate
                {
                    lock (FileLock)
                    {
                        using (BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite("TextDictionary.dat")))
                        {
                            binaryWriter.Seek(0, SeekOrigin.End);
                            binaryWriter.Write(trans + " 8==) " + Server.translationDic[trans]);
                        }
                    }
                });
            }
            return Server.translationDic[trans];
        }

        public async Task<string> editExistingTranslation(string korean, string englishToReplace)
        {
            if (Server.translationDic.ContainsKey(korean))
            {
                Server.translationDic[korean] = englishToReplace;
                await Task.Run(delegate
                {
                    lock (FileLock)
                    {
                        using (var stream = File.Create("TextDictionary.dat"))
                        using (var writer = new BinaryWriter(stream))
                        {
                            foreach (var kvp in Server.translationDic)
                                writer.Write($"{kvp.Key}{Delimiter}{kvp.Value}");
                        }
                    }
                });
            }
            return Server.translationDic[korean];
        }

        private void ClientReceiveEvent(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                //check for client closure
                int length = e.BytesTransferred;

                if (e.BytesTransferred == 0)
                {
                    Connected = false;
                    ClientSocket.Disconnect(false);
                }

                //add the received data to our buffer, in case we receive incomplete segments
                ClientBuffer.AddRange(e.Buffer.Take(length));

                //while theres more than 2 bytes in the buffer
                while (ClientBuffer.Count > 2)
                {
                    //get the packet length indicated by those 2 bytes
                    ushort packetLength = (ushort)((ClientBuffer[0] << 8) + ClientBuffer[1]);

                    //if its longer than the buffer size, we havent received the full packet yet
                    if (packetLength > ClientBuffer.Count)
                        break;

                    //otherwise, get the packet data and remove it from the buffer
                    List<byte> packetData = ClientBuffer.GetRange(0, packetLength);
                    ClientBuffer.RemoveRange(0, packetLength);

                    Packet packet = new Packet(packetData.ToArray());
                    //do shit with packet data
                    switch ((PuppyClientCodes)packet.OpCode)
                    {
                        case PuppyClientCodes.RequestTranslation:
                            Task.Run(async delegate
                            {
                                string korean = packet.ReadString16();
                                string text = await checkTransExist(korean);
                                Packet packet2 = new Packet((byte)PuppyServerCodes.TranslationResponse);
                                packet2.WriteString16(korean);
                                packet2.WriteString16(text);
                                await SendAsync(packet2);
                                Console.WriteLine("Translated: " + korean + " ***** " + text);
                            });
                            break;
                        case PuppyClientCodes.RequestEdit:
                            Task.Run(async delegate
                            {
                                string korean = packet.ReadString16();
                                string englishToReplace = packet.ReadString16();
                                Server.translationDic.TryGetValue(korean, out string value);
                                string fixedEnglish = await editExistingTranslation(korean, englishToReplace);

                                Packet packet2 = new Packet((byte)PuppyServerCodes.EditResponse);
                                packet2.WriteString16(korean);
                                packet2.WriteString16(fixedEnglish);
                                await SendAsync(packet2);
                                Console.WriteLine("Edit Old: " + value + ".");
                                Console.WriteLine("Edit Replaced:" + fixedEnglish);
                            });
                            break;
                    }
                }
            }
            catch { }
            ClientSocket.ReceiveAsync(e);
        }

        internal async Task SendAsync(params Packet[] buffer)
        {
            await Task.Yield();

            for (int i = 0; i < buffer.Length; i++)
            {
                if (!Connected)
                    break;

                var args = new SocketAsyncEventArgs();
                byte[] data = buffer[i].ToArray();
                args.SetBuffer(data, 0, data.Length);
                _ = ClientSocket.SendAsync(args);
            }
        }
    }
}
