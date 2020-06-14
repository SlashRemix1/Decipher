using Akorade;
using Doumi.Nexon.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Doumi.PuppyServer
{
    internal sealed class PuppyClient
    {
        private readonly object Sync = new object();
        internal Socket ClientSocket { get; }
        internal bool Connected { get; set; }
        private byte[] ClientSegment;
        private List<byte> ClientBuffer;
        internal List<(object identifier, TaskCompletionSource<object> notifier)> TaskCompletionSources;

        internal PuppyClient()
        {
            //create empty buffers for data to be stored in
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Connected = false;
            ClientSegment = new byte[ushort.MaxValue];
            ClientBuffer = new List<byte>(ushort.MaxValue);
            TaskCompletionSources = new List<(object identifier, TaskCompletionSource<object> notifier)>();
        }

        

        internal async Task Connect()
        {
            //await ClientSocket.ConnectAsync("puppyserver.dynu.net", 6969);
            await ClientSocket.ConnectAsync("127.0.0.1", 6969); //(Use this if running a server from your local computer.)

            if (ClientSocket.Connected)
                Connected = true;
            else
                throw new SocketException(6969);

            //create a byte segment to hold the received information
            var args = new SocketAsyncEventArgs();
            args.SetBuffer(ClientSegment, 0, ushort.MaxValue);
            //our method will be raised whenever data is received
            args.Completed += ClientReceiveEvent;

            //begin receiving data asynchronously
            ClientSocket.ReceiveAsync(args);
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
                    int packetLength = ((ClientBuffer[0] << 8) + ClientBuffer[1]);

                    //if its longer than the buffer size, we havent received the full packet yet
                    if (packetLength > ClientBuffer.Count)
                        break;

                    //otherwise, get the packet data and remove it from the buffer
                    List<byte> packetData = ClientBuffer.GetRange(0, packetLength);
                    ClientBuffer.RemoveRange(0, packetLength);

                    PuppyPacket packet = new PuppyPacket(packetData.ToArray());
                    //do shit with packet data
                    switch ((PuppyServerCodes)packet.OpCode)
                    {
                        case PuppyServerCodes.TranslationReponse:
                            string korean = packet.ReadString16();
                            string result = packet.ReadString16();
                            (object identifier, TaskCompletionSource<object> notifier) pair = TaskCompletionSources.FirstOrDefault(obj => obj.identifier.Equals(korean));
                            TaskCompletionSources.Remove(pair);
                            if (!pair.notifier.TrySetResult(result))
                                Application.Exit();
                            break;

                        case PuppyServerCodes.EditResponse:
                            string koreanToEdit = packet.ReadString16();
                            string resultToEdit = packet.ReadString16();
                            (object identifier, TaskCompletionSource<object> notifier) pair1 = TaskCompletionSources.FirstOrDefault(obj => obj.identifier.Equals(koreanToEdit));
                            TaskCompletionSources.Remove(pair1);
                            if (!pair1.notifier.TrySetResult(resultToEdit))
                                Application.Exit();
                            break;
                    }                    
                }
            }
            catch { }
            ClientSocket.ReceiveAsync(e);
        }

        internal async Task SendAsync(params PuppyPacket[] buffer)
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
