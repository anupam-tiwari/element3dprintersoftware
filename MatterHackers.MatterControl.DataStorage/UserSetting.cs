using System;

namespace MatterHackers.MatterControl.DataStorage
{
	public class UserSetting : Entity
	{
		public DateTime DateLastModified
		{
			get;
			set;
		}

		[Indexed]
		public string Name
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}

		public override void Commit()
		{
			DateLastModified = DateTime.Now;
			base.Commit();
		}
	}
}
