using System;

namespace MatterHackers.MatterControl.PrintQueue
{
	public class IndexArgs : EventArgs
	{
		internal int index;

		public int Index => index;

		internal IndexArgs(int index)
		{
			this.index = index;
		}
	}
}
