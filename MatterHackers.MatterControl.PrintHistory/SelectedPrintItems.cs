using System;
using System.Collections.Generic;

namespace MatterHackers.MatterControl.PrintHistory
{
	public class SelectedPrintItems<T> : List<T>
	{
		public event EventHandler OnAdd;

		public event EventHandler OnRemove;

		public new void Add(T item)
		{
			base.Add(item);
			if (this.OnAdd != null)
			{
				this.OnAdd(this, null);
			}
		}

		public new void Remove(T item)
		{
			base.Remove(item);
			if (this.OnRemove != null)
			{
				this.OnRemove(this, null);
			}
		}
	}
}
