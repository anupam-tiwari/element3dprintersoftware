using System;

namespace MatterHackers.MatterControl
{
	public class MenuItemAction
	{
		public string Title
		{
			get;
			set;
		}

		public Action Action
		{
			get;
			set;
		}

		public MenuItemAction(string title, Action action)
		{
			Title = title;
			Action = action;
		}
	}
}
