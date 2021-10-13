using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MatterHackers.MatterControl.DataStorage;

namespace SQLiteWin32
{
	public class TableMapping
	{
		public class Column
		{
			private PropertyInfo _prop;

			public string Name
			{
				get;
				private set;
			}

			public string PropertyName => _prop.Name;

			public Type ColumnType
			{
				get;
				private set;
			}

			public string Collation
			{
				get;
				private set;
			}

			public bool IsAutoInc
			{
				get;
				private set;
			}

			public bool IsPK
			{
				get;
				private set;
			}

			public IEnumerable<IndexedAttribute> Indices
			{
				get;
				set;
			}

			public bool IsNullable
			{
				get;
				private set;
			}

			public int MaxStringLength
			{
				get;
				private set;
			}

			public Column(PropertyInfo prop)
			{
				ColumnAttribute columnAttribute = (ColumnAttribute)Enumerable.FirstOrDefault<object>((IEnumerable<object>)prop.GetCustomAttributes(typeof(ColumnAttribute), inherit: true));
				_prop = prop;
				Name = ((columnAttribute == null) ? prop.Name : columnAttribute.Name);
				ColumnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
				Collation = Orm.Collation(prop);
				IsAutoInc = Orm.IsAutoInc(prop);
				IsPK = Orm.IsPK(prop);
				Indices = Orm.GetIndices(prop);
				IsNullable = !IsPK;
				MaxStringLength = Orm.MaxStringLength(prop);
			}

			public void SetValue(object obj, object val)
			{
				_prop.SetValue(obj, val, null);
			}

			public object GetValue(object obj)
			{
				return _prop.GetValue(obj, null);
			}
		}

		private Column _autoPk;

		private Column[] _insertColumns;

		private Column[] _insertOrReplaceColumns;

		private PreparedSqlLiteInsertCommand _insertCommand;

		private string _insertCommandExtra;

		public Type MappedType
		{
			get;
			private set;
		}

		public string TableName
		{
			get;
			private set;
		}

		public Column[] Columns
		{
			get;
			private set;
		}

		public Column PK
		{
			get;
			private set;
		}

		public string GetByPrimaryKeySql
		{
			get;
			private set;
		}

		public bool HasAutoIncPK
		{
			get;
			private set;
		}

		public Column[] InsertColumns
		{
			get
			{
				if (_insertColumns == null)
				{
					_insertColumns = Enumerable.ToArray<Column>(Enumerable.Where<Column>((IEnumerable<Column>)Columns, (Func<Column, bool>)((Column c) => !c.IsAutoInc)));
				}
				return _insertColumns;
			}
		}

		public Column[] InsertOrReplaceColumns
		{
			get
			{
				if (_insertOrReplaceColumns == null)
				{
					_insertOrReplaceColumns = Enumerable.ToArray<Column>((IEnumerable<Column>)Columns);
				}
				return _insertOrReplaceColumns;
			}
		}

		public TableMapping(Type type)
		{
			MappedType = type;
			TableAttribute tableAttribute = (TableAttribute)Enumerable.FirstOrDefault<object>((IEnumerable<object>)type.GetCustomAttributes(typeof(TableAttribute), inherit: true));
			TableName = ((tableAttribute != null) ? tableAttribute.Name : MappedType.Name);
			PropertyInfo[] properties = MappedType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
			List<Column> list = new List<Column>();
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				bool flag = propertyInfo.GetCustomAttributes(typeof(IgnoreAttribute), inherit: true).Length != 0;
				if (propertyInfo.CanWrite && !flag)
				{
					list.Add(new Column(propertyInfo));
				}
			}
			Columns = list.ToArray();
			Column[] columns = Columns;
			foreach (Column column in columns)
			{
				if (column.IsAutoInc && column.IsPK)
				{
					_autoPk = column;
				}
				if (column.IsPK)
				{
					PK = column;
				}
			}
			HasAutoIncPK = _autoPk != null;
			if (PK != null)
			{
				GetByPrimaryKeySql = $"select * from \"{TableName}\" where \"{PK.Name}\" = ?";
			}
			else
			{
				GetByPrimaryKeySql = $"select * from \"{TableName}\" limit 1";
			}
		}

		public void SetAutoIncPK(object obj, long id)
		{
			if (_autoPk != null)
			{
				_autoPk.SetValue(obj, Convert.ChangeType(id, _autoPk.ColumnType, null));
			}
		}

		public Column FindColumnWithPropertyName(string propertyName)
		{
			return Enumerable.FirstOrDefault<Column>(Enumerable.Where<Column>((IEnumerable<Column>)Columns, (Func<Column, bool>)((Column c) => c.PropertyName == propertyName)));
		}

		public Column FindColumn(string columnName)
		{
			return Enumerable.FirstOrDefault<Column>(Enumerable.Where<Column>((IEnumerable<Column>)Columns, (Func<Column, bool>)((Column c) => c.Name == columnName)));
		}

		public PreparedSqlLiteInsertCommand GetInsertCommand(SQLiteConnection conn, string extra)
		{
			if (_insertCommand == null)
			{
				_insertCommand = CreateInsertCommand(conn, extra);
				_insertCommandExtra = extra;
			}
			else if (_insertCommandExtra != extra)
			{
				_insertCommand.Dispose();
				_insertCommand = CreateInsertCommand(conn, extra);
				_insertCommandExtra = extra;
			}
			return _insertCommand;
		}

		private PreparedSqlLiteInsertCommand CreateInsertCommand(SQLiteConnection conn, string extra)
		{
			Column[] array = InsertColumns;
			string commandText;
			if (!Enumerable.Any<Column>((IEnumerable<Column>)array) && Enumerable.Count<Column>((IEnumerable<Column>)Columns) == 1 && Columns[0].IsAutoInc)
			{
				commandText = string.Format("insert {1} into \"{0}\" default values", TableName, extra);
			}
			else
			{
				if (string.Compare(extra, "OR REPLACE", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					array = InsertOrReplaceColumns;
				}
				commandText = string.Format("insert {3} into \"{0}\"({1}) values ({2})", TableName, string.Join(",", Enumerable.ToArray<string>(Enumerable.Select<Column, string>((IEnumerable<Column>)array, (Func<Column, string>)((Column c) => "\"" + c.Name + "\"")))), string.Join(",", Enumerable.ToArray<string>(Enumerable.Select<Column, string>((IEnumerable<Column>)array, (Func<Column, string>)((Column c) => "?")))), extra);
			}
			return new PreparedSqlLiteInsertCommand(conn)
			{
				CommandText = commandText
			};
		}

		protected internal void Dispose()
		{
			if (_insertCommand != null)
			{
				_insertCommand.Dispose();
				_insertCommand = null;
			}
		}
	}
}
