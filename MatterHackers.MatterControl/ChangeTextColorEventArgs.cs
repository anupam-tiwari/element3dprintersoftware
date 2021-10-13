using System;
using MatterHackers.Agg;

namespace MatterHackers.MatterControl
{
	public class ChangeTextColorEventArgs : EventArgs
	{
		public RGBA_Bytes color;

		public ChangeTextColorEventArgs(RGBA_Bytes color)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			this.color = color;
		}
	}
}
