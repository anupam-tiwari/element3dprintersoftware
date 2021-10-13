using System;

namespace MatterHackers.MatterControl.DataStorage
{
	public class ApplicationSession : Entity
	{
		public int PrintCount
		{
			get;
			set;
		}

		public DateTime SessionEnd
		{
			get;
			set;
		}

		public DateTime SessionStart
		{
			get;
			set;
		}

		public ApplicationSession()
		{
			SessionStart = DateTime.Now;
			PrintCount = 0;
		}
	}
}
