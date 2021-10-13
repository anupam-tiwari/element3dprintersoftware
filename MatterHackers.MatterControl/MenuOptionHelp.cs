using System.Collections.Generic;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;

namespace MatterHackers.MatterControl
{
	public class MenuOptionHelp : MenuBase
	{
		public MenuOptionHelp()
			: base("Help".Localize())
		{
			((GuiWidget)this).set_Name("Help Menu");
		}

		protected override IEnumerable<MenuItemAction> GetMenuActions()
		{
			return new List<MenuItemAction>
			{
				new MenuItemAction("MatterControl Wiki".Localize(), delegate
				{
					MatterControlApplication.Instance.LaunchBrowser("http://wiki.mattercontrol.com");
				}),
				new MenuItemAction("------------------------", null),
				new MenuItemAction("Contact Aether".Localize(), delegate
				{
					MatterControlApplication.Instance.LaunchBrowser("http://www.aether1.com/contact/");
				}),
				new MenuItemAction("------------------------", null),
				new MenuItemAction("About Element".Localize(), delegate
				{
					AboutWindow.Show();
				})
			};
		}
	}
}
