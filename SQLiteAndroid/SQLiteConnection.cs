using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using MatterHackers.MatterControl.DataStorage;

namespace SQLiteAndroid
{
	public class SQLiteConnection : IDisposable, ISQLite
	{
		private struct IndexedColumn
		{
			public int Order;

			public string ColumnName;
		}

		private struct IndexInfo
		{
			public string IndexName;

			public string TableName;

			public bool Unique;

			public List<IndexedColumn> Columns;
		}

		public class ColumnInfo
		{
			[Column("name")]
			public string Name
			{
				get;
				set;
			}

			public override string ToString()
			{
				return Name;
			}
		}

		private bool _open;

		private TimeSpan _busyTimeout;

		private Dictionary<string, TableMapping> _mappings;

		private Dictionary<string, TableMapping> _tables;

		private Stopwatch _sw;

		private long _elapsedMilliseconds;

		private int _trasactionDepth;

		private Random _rand = new Random();

		internal static readonly IntPtr NullHandle;

		private static bool _preserveDuringLinkMagic;

		private object locker = new object();

		public IntPtr Handle
		{
			get;
			private set;
		}

		public string DatabasePath
		{
			get;
			private set;
		}

		public bool TimeExecution
		{
			get;
			set;
		}

		public bool Trace
		{
			get;
			set;
		}

		public bool StoreDateTimeAsTicks
		{
			get;
			private set;
		}

		public TimeSpan BusyTimeout
		{
			get
			{
				return _busyTimeout;
			}
			set
			{
				_busyTimeout = value;
				if (Handle != NullHandle)
				{
					SQLite3.BusyTimeout(Handle, (int)_busyTimeout.TotalMilliseconds);
				}
			}
		}

		public IEnumerable<TableMapping> TableMappings
		{
			get
			{
				if (_tables == null)
				{
					return Enumerable.Empty<TableMapping>();
				}
				return _tables.Values;
			}
		}

		public bool IsInTransaction => _trasactionDepth > 0;

		public SQLiteConnection(string databasePath, bool storeDateTimeAsTicks = false)
			: this(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, storeDateTimeAsTicks)
		{
		}

		public SQLiteConnection(string databasePath, SQLiteOpenFlags openFlags, bool storeDateTimeAsTicks = false)
		{
			DatabasePath = databasePath;
			IntPtr db;
			SQLite3.Result result = SQLite3.Open(GetNullTerminatedUtf8(DatabasePath), out db, (int)openFlags, IntPtr.Zero);
			Handle = db;
			if (result != 0)
			{
				throw SQLiteException.New(result, $"Could not open database file: {DatabasePath} ({result})");
			}
			_open = true;
			StoreDateTimeAsTicks = storeDateTimeAsTicks;
			BusyTimeout = TimeSpan.FromSeconds(0.1);
		}

		static SQLiteConnection()
		{
			NullHandle = IntPtr.Zero;
			_preserveDuringLinkMagic = false;
			if (_preserveDuringLinkMagic)
			{
				new ColumnInfo().Name = "magic";
			}
		}

		private static byte[] GetNullTerminatedUtf8(string s)
		{
			byte[] array = new byte[Encoding.UTF8.GetByteCount(s) + 1];
			Encoding.UTF8.GetBytes(s, 0, s.Length, array, 0);
			return array;
		}

		public TableMapping GetMapping(Type type)
		{
			if (_mappings == null)
			{
				_mappings = new Dictionary<string, TableMapping>();
			}
			if (!_mappings.TryGetValue(type.FullName, out var value))
			{
				value = new TableMapping(type);
				_mappings[type.FullName] = value;
			}
			return value;
		}

		public TableMapping GetMapping<T>()
		{
			return GetMapping(typeof(T));
		}

		public int DropTable<T>()
		{
			return DropTable(typeof(T));
		}

		public int DropTable(Type ty)
		{
			lock (locker)
			{
				TableMapping mapping = GetMapping(ty);
				string query = $"drop table if exists \"{mapping.TableName}\"";
				return Execute(query);
			}
		}

		public int CreateTable<T>()
		{
			return CreateTable(typeof(T));
		}

		public int CreateTable(Type ty)
		{
			lock (locker)
			{
				if (_tables == null)
				{
					_tables = new Dictionary<string, TableMapping>();
				}
				if (!_tables.TryGetValue(ty.FullName, out var value))
				{
					value = GetMapping(ty);
					_tables.Add(ty.FullName, value);
				}
				string str = "create table if not exists \"" + value.TableName + "\"(\n";
				IEnumerable<string> enumerable = Enumerable.Select<TableMapping.Column, string>((IEnumerable<TableMapping.Column>)value.Columns, (Func<TableMapping.Column, string>)((TableMapping.Column p) => Orm.SqlDecl(p, StoreDateTimeAsTicks)));
				string str2 = string.Join(",\n", Enumerable.ToArray<string>(enumerable));
				str += str2;
				str += ")";
				int num = Execute(str);
				if (num == 0)
				{
					MigrateTable(value);
				}
				Dictionary<string, IndexInfo> dictionary = new Dictionary<string, IndexInfo>();
				TableMapping.Column[] columns = value.Columns;
				foreach (TableMapping.Column column in columns)
				{
					foreach (IndexedAttribute index in column.Indices)
					{
						string text = index.Name ?? (value.TableName + "_" + column.Name);
						if (!dictionary.TryGetValue(text, out var value2))
						{
							IndexInfo indexInfo = default(IndexInfo);
							indexInfo.IndexName = text;
							indexInfo.TableName = value.TableName;
							indexInfo.Unique = index.Unique;
							indexInfo.Columns = new List<IndexedColumn>();
							value2 = indexInfo;
							dictionary.Add(text, value2);
						}
						if (index.Unique != value2.Unique)
						{
							throw new Exception("All the columns in an index must have the same value for their Unique property");
						}
						value2.Columns.Add(new IndexedColumn
						{
							Order = index.Order,
							ColumnName = column.Name
						});
					}
				}
				foreach (string key in dictionary.Keys)
				{
					IndexInfo indexInfo2 = dictionary[key];
					string text2 = string.Join("\",\"", Enumerable.ToArray<string>(Enumerable.Select<IndexedColumn, string>((IEnumerable<IndexedColumn>)Enumerable.OrderBy<IndexedColumn, int>((IEnumerable<IndexedColumn>)indexInfo2.Columns, (Func<IndexedColumn, int>)((IndexedColumn i) => i.Order)), (Func<IndexedColumn, string>)((IndexedColumn i) => i.ColumnName))));
					string query = string.Format("create {3} index if not exists \"{0}\" on \"{1}\"(\"{2}\")", key, indexInfo2.TableName, text2, indexInfo2.Unique ? "unique" : "");
					num += Execute(query);
				}
				return num;
			}
		}

		public IEnumerable<ColumnInfo> GetTableInfo(string tableName)
		{
			string query = "pragma table_info(\"" + tableName + "\")";
			return Query<ColumnInfo>(query, Array.Empty<object>());
		}

		private void MigrateTable(TableMapping map)
		{
			IEnumerable<ColumnInfo> tableInfo = GetTableInfo(map.TableName);
			List<TableMapping.Column> list = new List<TableMapping.Column>();
			TableMapping.Column[] columns = map.Columns;
			foreach (TableMapping.Column column in columns)
			{
				bool flag = false;
				foreach (ColumnInfo item in tableInfo)
				{
					flag = string.Compare(column.Name, item.Name, StringComparison.InvariantCultureIgnoreCase) == 0;
					if (flag)
					{
						break;
					}
				}
				if (!flag)
				{
					list.Add(column);
				}
			}
			foreach (TableMapping.Column item2 in list)
			{
				string query = "alter table \"" + map.TableName + "\" add column " + Orm.SqlDecl(item2, StoreDateTimeAsTicks);
				Execute(query);
			}
		}

		protected virtual SQLiteCommand NewCommand()
		{
			return new SQLiteCommand(this);
		}

		public SQLiteCommand CreateCommand(string cmdText, params object[] ps)
		{
			if (!_open)
			{
				throw SQLiteException.New(SQLite3.Result.Error, "Cannot create commands from unopened database");
			}
			SQLiteCommand sQLiteCommand = NewCommand();
			sQLiteCommand.CommandText = cmdText;
			foreach (object val in ps)
			{
				sQLiteCommand.Bind(val);
			}
			return sQLiteCommand;
		}

		public int Execute(string query, params object[] args)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			SQLiteCommand sQLiteCommand = CreateCommand(query, args);
			if (TimeExecution)
			{
				if (_sw == null)
				{
					_sw = new Stopwatch();
				}
				_sw.Reset();
				_sw.Start();
			}
			int result = sQLiteCommand.ExecuteNonQuery();
			if (TimeExecution)
			{
				_sw.Stop();
				_elapsedMilliseconds += _sw.get_ElapsedMilliseconds();
			}
			return result;
		}

		public T ExecuteScalar<T>(string query, params object[] args)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			lock (locker)
			{
				SQLiteCommand sQLiteCommand = CreateCommand(query, args);
				if (TimeExecution)
				{
					if (_sw == null)
					{
						_sw = new Stopwatch();
					}
					_sw.Reset();
					_sw.Start();
				}
				T result = sQLiteCommand.ExecuteScalar<T>();
				if (TimeExecution)
				{
					_sw.Stop();
					_elapsedMilliseconds += _sw.get_ElapsedMilliseconds();
				}
				return result;
			}
		}

		public List<T> Query<T>(string query, params object[] args) where T : new()
		{
			SQLiteCommand sQLiteCommand = CreateCommand(query, args);
			lock (locker)
			{
				return sQLiteCommand.ExecuteQuery<T>();
			}
		}

		public IEnumerable<T> DeferredQuery<T>(string query, params object[] args) where T : new()
		{
			return CreateCommand(query, args).ExecuteDeferredQuery<T>();
		}

		public List<object> Query(TableMapping map, string query, params object[] args)
		{
			return CreateCommand(query, args).ExecuteQuery<object>(map);
		}

		public IEnumerable<object> DeferredQuery(TableMapping map, string query, params object[] args)
		{
			return CreateCommand(query, args).ExecuteDeferredQuery<object>(map);
		}

		public ITableQuery<T> Table<T>() where T : new()
		{
			lock (locker)
			{
				return new TableQuery<T>(this);
			}
		}

		public T Get<T>(object pk) where T : new()
		{
			TableMapping mapping = GetMapping(typeof(T));
			return Enumerable.First<T>((IEnumerable<T>)Query<T>(mapping.GetByPrimaryKeySql, new object[1]
			{
				pk
			}));
		}

		public T Get<T>(Expression<Func<T, bool>> predicate) where T : new()
		{
			return Table<T>().Where(predicate).First();
		}

		public T Find<T>(object pk) where T : new()
		{
			TableMapping mapping = GetMapping(typeof(T));
			return Enumerable.FirstOrDefault<T>((IEnumerable<T>)Query<T>(mapping.GetByPrimaryKeySql, new object[1]
			{
				pk
			}));
		}

		public object Find(object pk, TableMapping map)
		{
			return Enumerable.FirstOrDefault<object>((IEnumerable<object>)Query(map, map.GetByPrimaryKeySql, pk));
		}

		public T Find<T>(Expression<Func<T, bool>> predicate) where T : new()
		{
			return Table<T>().Where(predicate).FirstOrDefault();
		}

		public void BeginTransaction()
		{
			if (Interlocked.CompareExchange(ref _trasactionDepth, 1, 0) == 0)
			{
				try
				{
					Execute("begin transaction");
				}
				catch (Exception ex)
				{
					SQLiteException ex2 = ex as SQLiteException;
					if (ex2 != null)
					{
						switch (ex2.Result)
						{
						case SQLite3.Result.Busy:
						case SQLite3.Result.NoMem:
						case SQLite3.Result.Interrupt:
						case SQLite3.Result.IOError:
						case SQLite3.Result.Full:
							RollbackTo(null, noThrow: true);
							break;
						}
					}
					else
					{
						Interlocked.Decrement(ref _trasactionDepth);
					}
					throw;
				}
				return;
			}
			throw new InvalidOperationException("Cannot begin a transaction while already in a transaction.");
		}

		public string SaveTransactionPoint()
		{
			int num = Interlocked.Increment(ref _trasactionDepth) - 1;
			string text = "S" + (short)_rand.Next(32767) + "D" + num;
			try
			{
				Execute("savepoint " + text);
				return text;
			}
			catch (Exception ex)
			{
				SQLiteException ex2 = ex as SQLiteException;
				if (ex2 != null)
				{
					switch (ex2.Result)
					{
					case SQLite3.Result.Busy:
					case SQLite3.Result.NoMem:
					case SQLite3.Result.Interrupt:
					case SQLite3.Result.IOError:
					case SQLite3.Result.Full:
						RollbackTo(null, noThrow: true);
						break;
					}
				}
				else
				{
					Interlocked.Decrement(ref _trasactionDepth);
				}
				throw;
			}
		}

		public void Rollback()
		{
			RollbackTo(null, noThrow: false);
		}

		public void RollbackTo(string savepoint)
		{
			RollbackTo(savepoint, noThrow: false);
		}

		private void RollbackTo(string savepoint, bool noThrow)
		{
			try
			{
				if (string.IsNullOrEmpty(savepoint))
				{
					if (Interlocked.Exchange(ref _trasactionDepth, 0) > 0)
					{
						Execute("rollback");
					}
				}
				else
				{
					DoSavePointExecute(savepoint, "rollback to ");
				}
			}
			catch (SQLiteException)
			{
				if (!noThrow)
				{
					throw;
				}
			}
		}

		public void Release(string savepoint)
		{
			DoSavePointExecute(savepoint, "release ");
		}

		private void DoSavePointExecute(string savepoint, string cmd)
		{
			int num = savepoint.IndexOf('D');
			if (num >= 2 && savepoint.Length > num + 1 && int.TryParse(savepoint.Substring(num + 1), out var result) && 0 <= result && result < _trasactionDepth)
			{
				Thread.VolatileWrite(ref _trasactionDepth, result);
				Execute(cmd + savepoint);
				return;
			}
			throw new ArgumentException("savePoint", "savePoint is not valid, and should be the result of a call to SaveTransactionPoint.");
		}

		public void Commit()
		{
			if (Interlocked.Exchange(ref _trasactionDepth, 0) != 0)
			{
				Execute("commit");
			}
		}

		public void RunInTransaction(Action action)
		{
			lock (locker)
			{
				try
				{
					string savepoint = SaveTransactionPoint();
					action();
					Release(savepoint);
				}
				catch (Exception)
				{
					Rollback();
					throw;
				}
			}
		}

		public int InsertAll(IEnumerable objects)
		{
			lock (locker)
			{
				int c = 0;
				RunInTransaction(delegate
				{
					foreach (object? @object in objects)
					{
						c += Insert(@object);
					}
				});
				return c;
			}
		}

		public int InsertAll(IEnumerable objects, string extra)
		{
			int c = 0;
			RunInTransaction(delegate
			{
				foreach (object? @object in objects)
				{
					c += Insert(@object, extra);
				}
			});
			return c;
		}

		public int InsertAll(IEnumerable objects, Type objType)
		{
			int c = 0;
			RunInTransaction(delegate
			{
				foreach (object? @object in objects)
				{
					c += Insert(@object, objType);
				}
			});
			return c;
		}

		public int Insert(object obj)
		{
			if (obj == null)
			{
				return 0;
			}
			lock (locker)
			{
				return Insert(obj, "", obj.GetType());
			}
		}

		public int InsertOrReplace(object obj)
		{
			if (obj == null)
			{
				return 0;
			}
			return Insert(obj, "OR REPLACE", obj.GetType());
		}

		public int Insert(object obj, Type objType)
		{
			return Insert(obj, "", objType);
		}

		public int InsertOrReplace(object obj, Type objType)
		{
			return Insert(obj, "OR REPLACE", objType);
		}

		public int Insert(object obj, string extra)
		{
			if (obj == null)
			{
				return 0;
			}
			return Insert(obj, extra, obj.GetType());
		}

		public int Insert(object obj, string extra, Type objType)
		{
			if (obj == null || objType == null)
			{
				return 0;
			}
			TableMapping mapping = GetMapping(objType);
			TableMapping.Column[] array = ((string.Compare(extra, "OR REPLACE", StringComparison.InvariantCultureIgnoreCase) == 0) ? mapping.InsertOrReplaceColumns : mapping.InsertColumns);
			object[] array2 = new object[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = array[i].GetValue(obj);
			}
			int result = mapping.GetInsertCommand(this, extra).ExecuteNonQuery(array2);
			if (mapping.HasAutoIncPK)
			{
				long id = SQLite3.LastInsertRowid(Handle);
				mapping.SetAutoIncPK(obj, id);
			}
			return result;
		}

		public int Update(object obj)
		{
			if (obj == null)
			{
				return 0;
			}
			lock (locker)
			{
				return Update(obj, obj.GetType());
			}
		}

		public int Update(object obj, Type objType)
		{
			if (obj == null || objType == null)
			{
				return 0;
			}
			TableMapping mapping = GetMapping(objType);
			TableMapping.Column pk = mapping.PK;
			if (pk == null)
			{
				throw new NotSupportedException("Cannot update " + mapping.TableName + ": it has no PK");
			}
			IEnumerable<TableMapping.Column> enumerable = Enumerable.Where<TableMapping.Column>((IEnumerable<TableMapping.Column>)mapping.Columns, (Func<TableMapping.Column, bool>)((TableMapping.Column p) => p != pk));
			List<object> list = new List<object>(Enumerable.Select<TableMapping.Column, object>(enumerable, (Func<TableMapping.Column, object>)((TableMapping.Column c) => c.GetValue(obj))));
			list.Add(pk.GetValue(obj));
			string query = string.Format("update \"{0}\" set {1} where {2} = ? ", mapping.TableName, string.Join(",", Enumerable.ToArray<string>(Enumerable.Select<TableMapping.Column, string>(enumerable, (Func<TableMapping.Column, string>)((TableMapping.Column c) => "\"" + c.Name + "\" = ? ")))), pk.Name);
			return Execute(query, list.ToArray());
		}

		public int UpdateAll(IEnumerable objects)
		{
			int c = 0;
			RunInTransaction(delegate
			{
				foreach (object? @object in objects)
				{
					c += Update(@object);
				}
			});
			return c;
		}

		public int Delete(object objectToDelete)
		{
			lock (locker)
			{
				TableMapping mapping = GetMapping(objectToDelete.GetType());
				TableMapping.Column pK = mapping.PK;
				if (pK == null)
				{
					throw new NotSupportedException("Cannot delete " + mapping.TableName + ": it has no PK");
				}
				string query = $"delete from \"{mapping.TableName}\" where \"{pK.Name}\" = ?";
				return Execute(query, pK.GetValue(objectToDelete));
			}
		}

		public int Delete<T>(object primaryKey)
		{
			TableMapping mapping = GetMapping(typeof(T));
			TableMapping.Column pK = mapping.PK;
			if (pK == null)
			{
				throw new NotSupportedException("Cannot delete " + mapping.TableName + ": it has no PK");
			}
			string query = $"delete from \"{mapping.TableName}\" where \"{pK.Name}\" = ?";
			return Execute(query, primaryKey);
		}

		public int DeleteAll<T>()
		{
			TableMapping mapping = GetMapping(typeof(T));
			string query = $"delete from \"{mapping.TableName}\"";
			return Execute(query);
		}

		~SQLiteConnection()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			Close();
		}

		public void Close()
		{
			if (!_open || !(Handle != NullHandle))
			{
				return;
			}
			try
			{
				if (_mappings != null)
				{
					foreach (TableMapping value in _mappings.Values)
					{
						value.Dispose();
					}
				}
				SQLite3.Result result = SQLite3.Close(Handle);
				if (result != 0)
				{
					string errmsg = SQLite3.GetErrmsg(Handle);
					throw SQLiteException.New(result, errmsg);
				}
			}
			finally
			{
				Handle = NullHandle;
				_open = false;
			}
		}
	}
}
