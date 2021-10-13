using System;
using System.Collections.Generic;
using System.Linq;
using Community.CsharpSqlite;

namespace SQLiteWin32
{
	public class SQLiteCommand
	{
		private class Binding
		{
			public string Name
			{
				get;
				set;
			}

			public object Value
			{
				get;
				set;
			}

			public int Index
			{
				get;
				set;
			}
		}

		private SQLiteConnection _conn;

		private List<Binding> _bindings;

		internal static IntPtr NegativePointer = new IntPtr(-1);

		public string CommandText
		{
			get;
			set;
		}

		internal SQLiteCommand(SQLiteConnection conn)
		{
			_conn = conn;
			_bindings = new List<Binding>();
			CommandText = "";
		}

		public int ExecuteNonQuery()
		{
			_ = _conn.Trace;
			SQLite3.Result result = SQLite3.Result.OK;
			Vdbe stmt = Prepare();
			result = SQLite3.Step(stmt);
			Finalize(stmt);
			switch (result)
			{
			case SQLite3.Result.Done:
				return SQLite3.Changes(_conn.Handle);
			case SQLite3.Result.Error:
			{
				string errmsg = SQLite3.GetErrmsg(_conn.Handle);
				throw SQLiteException.New(result, errmsg);
			}
			default:
				throw SQLiteException.New(result, result.ToString());
			}
		}

		public IEnumerable<T> ExecuteDeferredQuery<T>()
		{
			return ExecuteDeferredQuery<T>(_conn.GetMapping(typeof(T)));
		}

		public List<T> ExecuteQuery<T>()
		{
			return Enumerable.ToList<T>(ExecuteDeferredQuery<T>(_conn.GetMapping(typeof(T))));
		}

		public List<T> ExecuteQuery<T>(TableMapping map)
		{
			return Enumerable.ToList<T>(ExecuteDeferredQuery<T>(map));
		}

		protected virtual void OnInstanceCreated(object obj)
		{
		}

		public IEnumerable<T> ExecuteDeferredQuery<T>(TableMapping map)
		{
			_ = _conn.Trace;
			Vdbe stmt = Prepare();
			try
			{
				TableMapping.Column[] cols = new TableMapping.Column[SQLite3.ColumnCount(stmt)];
				for (int i = 0; i < cols.Length; i++)
				{
					string columnName = SQLite3.ColumnName16(stmt, i);
					cols[i] = map.FindColumn(columnName);
				}
				while (SQLite3.Step(stmt) == SQLite3.Result.Row)
				{
					object obj = Activator.CreateInstance(map.MappedType);
					for (int j = 0; j < cols.Length; j++)
					{
						if (cols[j] != null)
						{
							SQLite3.ColType type = SQLite3.ColumnType(stmt, j);
							object val = ReadCol(stmt, j, type, cols[j].ColumnType);
							cols[j].SetValue(obj, val);
						}
					}
					OnInstanceCreated(obj);
					yield return (T)obj;
				}
			}
			finally
			{
				SQLite3.Finalize(stmt);
			}
		}

		public T ExecuteScalar<T>()
		{
			_ = _conn.Trace;
			T result = default(T);
			Vdbe stmt = Prepare();
			if (SQLite3.Step(stmt) == SQLite3.Result.Row)
			{
				SQLite3.ColType colType = SQLite3.ColumnType(stmt, 0);
				if (colType != SQLite3.ColType.Null)
				{
					result = (T)ReadCol(stmt, 0, colType, typeof(T));
				}
			}
			Finalize(stmt);
			return result;
		}

		public void Bind(string name, object val)
		{
			_bindings.Add(new Binding
			{
				Name = name,
				Value = val
			});
		}

		public void Bind(object val)
		{
			Bind(null, val);
		}

		public override string ToString()
		{
			string[] array = new string[1 + _bindings.Count];
			array[0] = CommandText;
			int num = 1;
			foreach (Binding binding in _bindings)
			{
				array[num] = $"  {num - 1}: {binding.Value}";
				num++;
			}
			return string.Join(Environment.NewLine, array);
		}

		private Vdbe Prepare()
		{
			Vdbe val = SQLite3.Prepare2(_conn.Handle, CommandText);
			BindAll(val);
			return val;
		}

		private new void Finalize(Vdbe stmt)
		{
			SQLite3.Finalize(stmt);
		}

		private void BindAll(Vdbe stmt)
		{
			int num = 1;
			foreach (Binding binding in _bindings)
			{
				if (binding.Name != null)
				{
					binding.Index = SQLite3.BindParameterIndex(stmt, binding.Name);
				}
				else
				{
					binding.Index = num++;
				}
				BindParameter(stmt, binding.Index, binding.Value, _conn.StoreDateTimeAsTicks);
			}
		}

		internal static void BindParameter(Vdbe stmt, int index, object value, bool storeDateTimeAsTicks)
		{
			if (value == null)
			{
				SQLite3.BindNull(stmt, index);
				return;
			}
			if (value is int)
			{
				SQLite3.BindInt(stmt, index, (int)value);
				return;
			}
			if (value is string)
			{
				SQLite3.BindText(stmt, index, (string)value, -1, NegativePointer);
				return;
			}
			if (value is byte || value is ushort || value is sbyte || value is short)
			{
				SQLite3.BindInt(stmt, index, Convert.ToInt32(value));
				return;
			}
			if (value is bool)
			{
				SQLite3.BindInt(stmt, index, ((bool)value) ? 1 : 0);
				return;
			}
			if (value is uint || value is long)
			{
				SQLite3.BindInt64(stmt, index, Convert.ToInt64(value));
				return;
			}
			if (value is float || value is double || value is decimal)
			{
				SQLite3.BindDouble(stmt, index, Convert.ToDouble(value));
				return;
			}
			if (value is DateTime)
			{
				if (storeDateTimeAsTicks)
				{
					SQLite3.BindInt64(stmt, index, ((DateTime)value).Ticks);
				}
				else
				{
					SQLite3.BindText(stmt, index, ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"), -1, NegativePointer);
				}
				return;
			}
			if (value.GetType().IsEnum)
			{
				SQLite3.BindInt(stmt, index, Convert.ToInt32(value));
				return;
			}
			if (value is byte[])
			{
				SQLite3.BindBlob(stmt, index, (byte[])value, ((byte[])value).Length, NegativePointer);
				return;
			}
			throw new NotSupportedException("Cannot store type: " + value.GetType());
		}

		private object ReadCol(Vdbe stmt, int index, SQLite3.ColType type, Type clrType)
		{
			if (type == SQLite3.ColType.Null)
			{
				return null;
			}
			if (clrType == typeof(string))
			{
				return SQLite3.ColumnString(stmt, index);
			}
			if (clrType == typeof(int))
			{
				return SQLite3.ColumnInt(stmt, index);
			}
			if (clrType == typeof(bool))
			{
				return SQLite3.ColumnInt(stmt, index) == 1;
			}
			if (clrType == typeof(double))
			{
				return SQLite3.ColumnDouble(stmt, index);
			}
			if (clrType == typeof(float))
			{
				return (float)SQLite3.ColumnDouble(stmt, index);
			}
			if (clrType == typeof(DateTime))
			{
				if (_conn.StoreDateTimeAsTicks)
				{
					return new DateTime(SQLite3.ColumnInt64(stmt, index));
				}
				return DateTime.Parse(SQLite3.ColumnString(stmt, index));
			}
			if (clrType.IsEnum)
			{
				return SQLite3.ColumnInt(stmt, index);
			}
			if (clrType == typeof(long))
			{
				return SQLite3.ColumnInt64(stmt, index);
			}
			if (clrType == typeof(uint))
			{
				return (uint)SQLite3.ColumnInt64(stmt, index);
			}
			if (clrType == typeof(decimal))
			{
				return (decimal)SQLite3.ColumnDouble(stmt, index);
			}
			if (clrType == typeof(byte))
			{
				return (byte)SQLite3.ColumnInt(stmt, index);
			}
			if (clrType == typeof(ushort))
			{
				return (ushort)SQLite3.ColumnInt(stmt, index);
			}
			if (clrType == typeof(short))
			{
				return (short)SQLite3.ColumnInt(stmt, index);
			}
			if (clrType == typeof(sbyte))
			{
				return (sbyte)SQLite3.ColumnInt(stmt, index);
			}
			if (clrType == typeof(byte[]))
			{
				return SQLite3.ColumnByteArray(stmt, index);
			}
			throw new NotSupportedException("Don't know how to read " + clrType);
		}
	}
}
