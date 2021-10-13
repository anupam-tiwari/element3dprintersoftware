namespace MatterHackers.MatterControl.DataStorage
{
	public class PrintItemCollection : Entity
	{
		public string Key
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		[Indexed]
		public int ParentCollectionID
		{
			get;
			set;
		}

		public PrintItemCollection()
		{
		}

		public PrintItemCollection(string name, string collectionKey)
		{
			Name = name;
			Key = collectionKey;
		}
	}
}
