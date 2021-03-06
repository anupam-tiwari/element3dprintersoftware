using System;

namespace SQLiteAndroid
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
