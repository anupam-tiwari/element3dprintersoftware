using System;
using MatterHackers.MatterControl.DataStorage;

namespace SQLiteWin32
{
	[AttributeUsage(AttributeTargets.Property)]
	public class UniqueAttribute : IndexedAttribute
	{
		public override bool Unique
		{
			get
			{
				return true;
			}
			set
			{
			}
		}
	}
}
