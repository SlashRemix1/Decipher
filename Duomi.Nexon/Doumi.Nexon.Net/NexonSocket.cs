using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace Doumi.Nexon.Net
{
	public class NexonSocket : Socket
	{
		internal SocketError error;

		internal byte[] header;

		internal int headerLength;

		internal int headerOffset;

		private Mutex mutexPacketSend;

		internal byte[] packet;

		internal int packetLength;

		internal int packetOffset;

		private static readonly int processId = Process.GetCurrentProcess().Id;

		public bool HeaderComplete => headerOffset == headerLength;

		public bool PacketComplete => packetOffset == packetLength;

		public byte Tick
		{
			get;
			set;
		}

		public event Action<NexonPacket> OnDataSending;

		public NexonSocket(Socket socket)
			: base(socket.DuplicateAndClose(processId))
		{
			header = new byte[3];
			packet = new byte[32768];
			headerLength = 3;
			mutexPacketSend = new Mutex();
		}

		public NexonSocket(SocketInformation socketInformation)
			: base(socketInformation)
		{
			header = new byte[3];
			packet = new byte[32768];
			headerLength = 3;
			mutexPacketSend = new Mutex();
		}

		public NexonSocket(SocketType socketType, ProtocolType protocolType)
			: base(socketType, protocolType)
		{
			header = new byte[3];
			packet = new byte[32768];
			headerLength = 3;
			mutexPacketSend = new Mutex();
		}

		public NexonSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
			: base(addressFamily, socketType, protocolType)
		{
			header = new byte[3];
			packet = new byte[32768];
			headerLength = 3;
			mutexPacketSend = new Mutex();
		}

		public IAsyncResult BeginReceiveHeader(AsyncCallback callback, object state)
		{
			return BeginReceive(header, headerOffset, headerLength - headerOffset, SocketFlags.None, out error, callback, state);
		}

		public IAsyncResult BeginReceivePacket(AsyncCallback callback, object state)
		{
			return BeginReceive(packet, packetOffset, packetLength - packetOffset, SocketFlags.None, out error, callback, state);
		}

		public int EndReceiveHeader(IAsyncResult result)
		{
			int num = EndReceive(result, out error);
			headerOffset += num;
			if (HeaderComplete)
			{
				packetOffset = 0;
				packetLength = ((header[1] << 8) | header[2]);
			}
			return num;
		}

		public int EndReceivePacket(IAsyncResult result)
		{
			int num = EndReceive(result, out error);
			packetOffset += num;
			if (PacketComplete)
			{
				headerOffset = 0;
				headerLength = 3;
			}
			return num;
		}

		public void Send(NexonPacket packet)
		{
			mutexPacketSend.WaitOne();
			if (packet.Secured)
			{
				packet.Decrypt();
				packet.Tick = Tick++;
			}
			if (this.OnDataSending != null)
			{
				this.OnDataSending(packet);
			}
			if (packet.Secured)
			{
				packet.Encrypt();
			}
			byte[] array = packet.ToArray();
			Send(array, 0, array.Length, SocketFlags.None, out error);
			mutexPacketSend.ReleaseMutex();
		}
	}
}
