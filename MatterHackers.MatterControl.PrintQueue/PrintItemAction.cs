using System;
using System.Collections.Generic;

namespace MatterHackers.MatterControl.PrintQueue
{
	public class PrintItemAction
	{
		public string Title
		{
			get;
			set;
		}

		public Action<IEnumerable<QueueRowItem>, QueueDataWidget> Action
		{
			get;
			set;
		}

		public bool SingleItemOnly
		{
			get;
			set;
		}
	}
}
