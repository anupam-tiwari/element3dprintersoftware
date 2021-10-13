using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MatterHackers.MatterControl.DataStorage;

namespace SQLiteAndroid
{
	public static class Orm
	{
		public const int DefaultMaxStringLength = 140;

		public static string SqlDecl(TableMapping.Column p, bool storeDateTimeAsTicks)
		{
			string text = "\"" + p.Name + "\" " + SqlType(p, storeDateTimeAsTicks) + " ";
			if (p.IsPK)
			{
				text += "primary key ";
			}
			if (p.IsAutoInc)
			{
				text += "autoincrement ";
			}
			if (!p.IsNullable)
			{
				text += "not null ";
			}
			if (!string.IsNullOrEmpty(p.Collation))
			{
				text = text + "collate " + p.Collation + " ";
			}
			return text;
		}

		public static string SqlType(TableMapping.Column p, bool storeDateTimeAsTicks)
		{
			Type columnType = p.ColumnType;
			if (columnType == typeof(bool) || columnType == typeof(byte) || columnType == typeof(ushort) || columnType == typeof(sbyte) || columnType == typeof(short) || columnType == typeof(int))
			{
				return "integer";
			}
			if (columnType == typeof(uint) || columnType == typeof(long))
			{
				return "bigint";
			}
			if (columnType == typeof(float) || columnType == typeof(double) || columnType == typeof(decimal))
			{
				return "float";
			}
			if (columnType == typeof(string))
			{
				int maxStringLength = p.MaxStringLength;
				return "varchar(" + maxStringLength + ")";
			}
			if (columnType == typeof(DateTime))
			{
				if (!storeDateTimeAsTicks)
				{
					return "datetime";
				}
				return "bigint";
			}
			if (columnType.IsEnum)
			{
				return "integer";
			}
			if (columnType == typeof(byte[]))
			{
				return "blob";
			}
			throw new NotSupportedException("Don't know about " + columnType);
		}

		public static bool IsPK(MemberInfo p)
		{
			return p.GetCustomAttributes(typeof(PrimaryKeyAttribute), inherit: true).Length != 0;
		}

		public static string Collation(MemberInfo p)
		{
			object[] customAttributes = p.GetCustomAttributes(typeof(CollationAttribute), inherit: true);
			if (customAttributes.Length != 0)
			{
				return ((CollationAttribute)customAttributes[0]).Value;
			}
			return string.Empty;
		}

		public static bool IsAutoInc(MemberInfo p)
		{
			return p.GetCustomAttributes(typeof(AutoIncrementAttribute), inherit: true).Length != 0;
		}

		public static IEnumerable<IndexedAttribute> GetIndices(MemberInfo p)
		{
			return Enumerable.Cast<IndexedAttribute>((IEnumerable)p.GetCustomAttributes(typeof(IndexedAttribute), inherit: true));
		}

		public static int MaxStringLength(PropertyInfo p)
		{
			object[] customAttributes = p.GetCustomAttributes(typeof(MaxLengthAttribute), inherit: true);
			if (customAttributes.Length != 0)
			{
				return ((MaxLengthAttribute)customAttributes[0]).Value;
			}
			return 140;
		}
	}
}
