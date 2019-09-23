namespace Doumi.Nexon.Net
{
	public class NexonServerPacket : NexonPacket
	{
		public override bool Secured
		{
			get
			{
				switch (base.Code)
				{
				case 0:
					return false;
				case 3:
					return false;
				case 126:
					return false;
				default:
					return true;
				}
			}
		}

		public NexonServerPacket(NexonPatron patron, NexonSocket socket)
			: base(patron, socket)
		{
		}

		public NexonServerPacket(NexonPatron patron, byte command)
			: base(patron, command)
		{
		}

		public NexonServerPacket(NexonPatron patron, byte[] packet, int length)
			: base(patron, packet, length)
		{
		}
	}
}
