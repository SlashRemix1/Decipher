namespace Doumi.Nexon.Net
{
	public class NexonClientPacket : NexonPacket
	{
		public override bool Secured
		{
			get
			{
				switch (base.Code)
				{
				case 0:
					return false;
				case 16:
					return false;
				default:
					return true;
				}
			}
		}

		public NexonClientPacket(NexonPatron patron, NexonSocket socket)
			: base(patron, socket)
		{
		}

		public NexonClientPacket(NexonPatron patron, byte command)
			: base(patron, command)
		{
		}

		public NexonClientPacket(NexonPatron patron, byte[] packet, int length)
			: base(patron, packet, length)
		{
		}
	}
}
