using System;
using System.Text;
using Community.CsharpSqlite;

namespace SQLiteWin32
{
	public static class SQLite3
	{
		public enum Result
		{
			OK = 0,
			Error = 1,
			Internal = 2,
			Perm = 3,
			Abort = 4,
			Busy = 5,
			Locked = 6,
			NoMem = 7,
			ReadOnly = 8,
			Interrupt = 9,
			IOError = 10,
			Corrupt = 11,
			NotFound = 12,
			Full = 13,
			CannotOpen = 14,
			LockErr = 0xF,
			Empty = 0x10,
			SchemaChngd = 17,
			TooBig = 18,
			Constraint = 19,
			Mismatch = 20,
			Misuse = 21,
			NotImplementedLFS = 22,
			AccessDenied = 23,
			Format = 24,
			Range = 25,
			NonDBFile = 26,
			Row = 100,
			Done = 101
		}

		public enum ConfigOption
		{
			SingleThread = 1,
			MultiThread,
			Serialized
		}

		public enum ColType
		{
			Integer = 1,
			Float,
			Text,
			Blob,
			Null
		}

		public static Result Open(string filename, out sqlite3 db)
		{
			return (Result)Sqlite3.sqlite3_open(filename, ref db);
		}

		public static Result Open(string filename, out sqlite3 db, int flags, IntPtr zVfs)
		{
			return (Result)Sqlite3.sqlite3_open_v2(filename, ref db, flags, (string)null);
		}

		public static Result Close(sqlite3 db)
		{
			return (Result)Sqlite3.sqlite3_close(db);
		}

		public static Result BusyTimeout(sqlite3 db, int milliseconds)
		{
			return (Result)Sqlite3.sqlite3_busy_timeout(db, milliseconds);
		}

		public static int Changes(sqlite3 db)
		{
			return Sqlite3.sqlite3_changes(db);
		}

		public static Vdbe Prepare2(sqlite3 db, string query)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			Vdbe result = new Vdbe();
			int num = Sqlite3.sqlite3_prepare_v2(db, query, Encoding.UTF8.GetByteCount(query), ref result, 0);
			if (num != 0)
			{
				throw SQLiteException.New((Result)num, GetErrmsg(db));
			}
			return result;
		}

		public static Result Step(Vdbe stmt)
		{
			return (Result)Sqlite3.sqlite3_step(stmt);
		}

		public static Result Reset(Vdbe stmt)
		{
			return (Result)Sqlite3.sqlite3_reset(stmt);
		}

		public new static Result Finalize(Vdbe stmt)
		{
			return (Result)Sqlite3.sqlite3_finalize(stmt);
		}

		public static long LastInsertRowid(sqlite3 db)
		{
			return Sqlite3.sqlite3_last_insert_rowid(db);
		}

		public static string GetErrmsg(sqlite3 db)
		{
			return Sqlite3.sqlite3_errmsg(db);
		}

		public static int BindParameterIndex(Vdbe stmt, string name)
		{
			return Sqlite3.sqlite3_bind_parameter_index(stmt, name);
		}

		public static int BindNull(Vdbe stmt, int index)
		{
			return Sqlite3.sqlite3_bind_null(stmt, index);
		}

		public static int BindInt(Vdbe stmt, int index, int val)
		{
			return Sqlite3.sqlite3_bind_int(stmt, index, val);
		}

		public static int BindInt64(Vdbe stmt, int index, long val)
		{
			return Sqlite3.sqlite3_bind_int64(stmt, index, val);
		}

		public static int BindDouble(Vdbe stmt, int index, double val)
		{
			return Sqlite3.sqlite3_bind_double(stmt, index, val);
		}

		public static int BindText(Vdbe stmt, int index, string val, int n, IntPtr free)
		{
			return Sqlite3.sqlite3_bind_text(stmt, index, val, n, (dxDel)null);
		}

		public static int BindBlob(Vdbe stmt, int index, byte[] val, int n, IntPtr free)
		{
			return Sqlite3.sqlite3_bind_blob(stmt, index, val, n, (dxDel)null);
		}

		public static int ColumnCount(Vdbe stmt)
		{
			return Sqlite3.sqlite3_column_count(stmt);
		}

		public static string ColumnName(Vdbe stmt, int index)
		{
			return Sqlite3.sqlite3_column_name(stmt, index);
		}

		public static string ColumnName16(Vdbe stmt, int index)
		{
			return Sqlite3.sqlite3_column_name(stmt, index);
		}

		public static ColType ColumnType(Vdbe stmt, int index)
		{
			return (ColType)Sqlite3.sqlite3_column_type(stmt, index);
		}

		public static int ColumnInt(Vdbe stmt, int index)
		{
			return Sqlite3.sqlite3_column_int(stmt, index);
		}

		public static long ColumnInt64(Vdbe stmt, int index)
		{
			return Sqlite3.sqlite3_column_int64(stmt, index);
		}

		public static double ColumnDouble(Vdbe stmt, int index)
		{
			return Sqlite3.sqlite3_column_double(stmt, index);
		}

		public static string ColumnText(Vdbe stmt, int index)
		{
			return Sqlite3.sqlite3_column_text(stmt, index);
		}

		public static string ColumnText16(Vdbe stmt, int index)
		{
			return Sqlite3.sqlite3_column_text(stmt, index);
		}

		public static byte[] ColumnBlob(Vdbe stmt, int index)
		{
			return Sqlite3.sqlite3_column_blob(stmt, index);
		}

		public static int ColumnBytes(Vdbe stmt, int index)
		{
			return Sqlite3.sqlite3_column_bytes(stmt, index);
		}

		public static string ColumnString(Vdbe stmt, int index)
		{
			return Sqlite3.sqlite3_column_text(stmt, index);
		}

		public static byte[] ColumnByteArray(Vdbe stmt, int index)
		{
			return ColumnBlob(stmt, index);
		}
	}
}
