namespace MatterHackers.MatterControl.PrintQueue
{
	internal class ButtonEnableData
	{
		internal bool multipleItems;

		internal bool collectionItems;

		internal ButtonEnableData(bool multipleItems, bool protectedItems, bool collectionItems)
		{
			this.multipleItems = multipleItems;
			this.collectionItems = collectionItems;
		}
	}
}
