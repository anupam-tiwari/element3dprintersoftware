using System.Runtime.InteropServices;

namespace MatterHackers.MatterControl.FrostedSerial
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct tDeviceInfo
	{
		public uint c_iflag;

		public uint c_oflag;

		public uint c_cflag;

		public uint c_lflag;

		public unsafe fixed byte c_cc[20];

		public uint c_ispeed;

		public uint c_ospeed;
	}
}
