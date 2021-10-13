using System;

namespace SQLiteWin32
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ColumnAttribute : Attribute
	{
		public string Name
		{
			get;
			set;
		}

		public ColumnAttribute(string name)
		{
			Name = name;
		}
	}
}
