using System.Collections.Generic;

namespace MatterHackers.MatterControl.PrintQueue
{
	public abstract class PrintItemMenuExtension
	{
		public abstract IEnumerable<PrintItemAction> GetMenuItems();
	}
}
