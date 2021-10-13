using System;
using System.Runtime.InteropServices;

namespace SQLiteUnix
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

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_open")]
		public static extern Result Open([MarshalAs(UnmanagedType.LPStr)] string filename, out IntPtr db);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_open_v2")]
		public static extern Result Open([MarshalAs(UnmanagedType.LPStr)] string filename, out IntPtr db, int flags, IntPtr zvfs);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_open_v2")]
		public static extern Result Open(byte[] filename, out IntPtr db, int flags, IntPtr zvfs);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_open16")]
		public static extern Result Open16([MarshalAs(UnmanagedType.LPWStr)] string filename, out IntPtr db);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_close")]
		public static extern Result Close(IntPtr db);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_config")]
		public static extern Result Config(ConfigOption option);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "sqlite3_win32_set_directory")]
		public static extern int SetDirectory(uint directoryType, string directoryPath);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_busy_timeout")]
		public static extern Result BusyTimeout(IntPtr db, int milliseconds);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_changes")]
		public static extern int Changes(IntPtr db);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_prepare_v2")]
		public static extern Result Prepare2(IntPtr db, [MarshalAs(UnmanagedType.LPStr)] string sql, int numBytes, out IntPtr stmt, IntPtr pzTail);

		public static IntPtr Prepare2(IntPtr db, string query)
		{
			IntPtr stmt;
			Result result = Prepare2(db, query, query.Length, out stmt, IntPtr.Zero);
			if (result != 0)
			{
				throw SQLiteException.New(result, GetErrmsg(db));
			}
			return stmt;
		}

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_step")]
		public static extern Result Step(IntPtr stmt);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_reset")]
		public static extern Result Reset(IntPtr stmt);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_finalize")]
		public new static extern Result Finalize(IntPtr stmt);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_last_insert_rowid")]
		public static extern long LastInsertRowid(IntPtr db);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_errmsg16")]
		public static extern IntPtr Errmsg(IntPtr db);

		public static string GetErrmsg(IntPtr db)
		{
			return Marshal.PtrToStringUni(Errmsg(db));
		}

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_bind_parameter_index")]
		public static extern int BindParameterIndex(IntPtr stmt, [MarshalAs(UnmanagedType.LPStr)] string name);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_bind_null")]
		public static extern int BindNull(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_bind_int")]
		public static extern int BindInt(IntPtr stmt, int index, int val);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_bind_int64")]
		public static extern int BindInt64(IntPtr stmt, int index, long val);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_bind_double")]
		public static extern int BindDouble(IntPtr stmt, int index, double val);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "sqlite3_bind_text16")]
		public static extern int BindText(IntPtr stmt, int index, [MarshalAs(UnmanagedType.LPWStr)] string val, int n, IntPtr free);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_bind_blob")]
		public static extern int BindBlob(IntPtr stmt, int index, byte[] val, int n, IntPtr free);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_count")]
		public static extern int ColumnCount(IntPtr stmt);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_name")]
		public static extern IntPtr ColumnName(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_name16")]
		private static extern IntPtr ColumnName16Internal(IntPtr stmt, int index);

		public static string ColumnName16(IntPtr stmt, int index)
		{
			return Marshal.PtrToStringUni(ColumnName16Internal(stmt, index));
		}

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_type")]
		public static extern ColType ColumnType(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_int")]
		public static extern int ColumnInt(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_int64")]
		public static extern long ColumnInt64(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_double")]
		public static extern double ColumnDouble(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_text")]
		public static extern IntPtr ColumnText(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_text16")]
		public static extern IntPtr ColumnText16(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_blob")]
		public static extern IntPtr ColumnBlob(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_bytes")]
		public static extern int ColumnBytes(IntPtr stmt, int index);

		public static string ColumnString(IntPtr stmt, int index)
		{
			return Marshal.PtrToStringUni(ColumnText16(stmt, index));
		}

		public static byte[] ColumnByteArray(IntPtr stmt, int index)
		{
			int num = ColumnBytes(stmt, index);
			byte[] array = new byte[num];
			if (num > 0)
			{
				Marshal.Copy(ColumnBlob(stmt, index), array, 0, num);
			}
			return array;
		}
	}
}
