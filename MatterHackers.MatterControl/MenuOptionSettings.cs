using System;
using System.Collections.Generic;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;

namespace MatterHackers.MatterControl
{
	public class MenuOptionSettings : MenuBase
	{
		public static PopOutTextTabWidget sliceSettingsPopOut;

		public static PopOutTextTabWidget controlsPopOut;

		public MenuOptionSettings()
			: base("View".Localize())
		{
		}

		protected override IEnumerable<MenuItemAction> GetMenuActions()
		{
			return new List<MenuItemAction>
			{
				new MenuItemAction("Settings".Localize(), delegate
				{
					sliceSettingsPopOut?.ShowInWindow();
				}),
				new MenuItemAction("Controls".Localize(), delegate
				{
					controlsPopOut?.ShowInWindow();
				}),
				new MenuItemAction("Terminal".Localize(), delegate
				{
					UiThread.RunOnIdle((Action)TerminalWindow.Show);
				})
			};
		}
	}
}
