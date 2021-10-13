namespace MatterHackers.MatterControl.DataStorage
{
	public class SliceSetting : Entity
	{
		public string Name
		{
			get;
			set;
		}

		[Indexed]
		public int SettingsCollectionId
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}
	}
}
