using System;

namespace MatterHackers.MatterControl.DataStorage
{
	public class PrintItem : Entity
	{
		public DateTime DateAdded
		{
			get;
			set;
		}

		public string FileLocation
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public int PrintCount
		{
			get;
			set;
		}

		[Indexed]
		public int PrintItemCollectionID
		{
			get;
			set;
		}

		public bool ReadOnly
		{
			get;
			set;
		}

		public bool Protected
		{
			get;
			set;
		}

		public bool WellPlatePrint
		{
			get;
			set;
		}

		public PrintItem()
			: this("", "")
		{
		}

		public PrintItem(string name, string fileLocation)
		{
			Name = name;
			FileLocation = fileLocation;
			DateAdded = DateTime.Now;
			PrintCount = 0;
		}
	}
}
