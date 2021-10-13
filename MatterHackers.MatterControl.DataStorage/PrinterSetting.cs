using System;

namespace MatterHackers.MatterControl.DataStorage
{
	public class PrinterSetting : Entity
	{
		public DateTime DateLastModified
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
		public int PrinterId
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
