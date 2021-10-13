using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.PrintLibrary
{
	internal class MenuEnableData
	{
		internal bool multipleItems;

		internal bool protectedItems;

		internal bool collectionItems;

		internal bool shareItems;

		internal MenuItem menuItemToChange;

		internal MenuEnableData(MenuItem menuItemToChange, bool multipleItems, bool protectedItems, bool collectionItems, bool shareItems = false)
		{
			this.menuItemToChange = menuItemToChange;
			this.multipleItems = multipleItems;
			this.protectedItems = protectedItems;
			this.collectionItems = collectionItems;
			this.shareItems = shareItems;
		}
	}
}
