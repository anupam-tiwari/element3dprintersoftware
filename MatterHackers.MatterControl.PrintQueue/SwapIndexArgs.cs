using System;

namespace MatterHackers.MatterControl.PrintQueue
{
	public class SwapIndexArgs : EventArgs
	{
		internal int indexA;

		internal int indexB;

		internal SwapIndexArgs(int indexA, int indexB)
		{
			this.indexA = indexA;
			this.indexB = indexB;
		}
	}
}
