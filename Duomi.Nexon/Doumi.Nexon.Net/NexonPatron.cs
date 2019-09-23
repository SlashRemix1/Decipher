using System.Text;

namespace Doumi.Nexon.Net
{
	public abstract class NexonPatron
	{
		public NexonSocket Client
		{
			get;
			set;
		}

		public byte[] Hash
		{
			get;
			set;
		}

		public byte Seed
		{
			get;
			set;
		}

		public NexonSocket Server
		{
			get;
			set;
		}


		public NexonPatron(NexonSocket client, NexonSocket server)
		{
			Client = client;
			Server = server;
			Seed = 0;
			Hash = Encoding.ASCII.GetBytes("NexonInc.");
		}
	}
}
