using System;
using Community.CsharpSqlite;

namespace SQLiteWin32
{
	public class PreparedSqlLiteInsertCommand : IDisposable
	{
		internal static readonly Vdbe NullStatement;

		public bool Initialized
		{
			get;
			set;
		}

		protected SQLiteConnection Connection
		{
			get;
			set;
		}

		public string CommandText
		{
			get;
			set;
		}

		protected Vdbe Statement
		{
			get;
			set;
		}

		internal PreparedSqlLiteInsertCommand(SQLiteConnection conn)
		{
			Connection = conn;
		}

		public int ExecuteNonQuery(object[] source)
		{
			_ = Connection.Trace;
			SQLite3.Result result = SQLite3.Result.OK;
			if (!Initialized)
			{
				Statement = Prepare();
				Initialized = true;
			}
			if (source != null)
			{
				for (int i = 0; i < source.Length; i++)
				{
					SQLiteCommand.BindParameter(Statement, i + 1, source[i], Connection.StoreDateTimeAsTicks);
				}
			}
			result = SQLite3.Step(Statement);
			switch (result)
			{
			case SQLite3.Result.Done:
			{
				int result2 = SQLite3.Changes(Connection.Handle);
				SQLite3.Reset(Statement);
				return result2;
			}
			case SQLite3.Result.Error:
			{
				string errmsg = SQLite3.GetErrmsg(Connection.Handle);
				SQLite3.Reset(Statement);
				throw SQLiteException.New(result, errmsg);
			}
			default:
				SQLite3.Reset(Statement);
				throw SQLiteException.New(result, result.ToString());
			}
		}

		protected virtual Vdbe Prepare()
		{
			return SQLite3.Prepare2(Connection.Handle, CommandText);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (Statement != NullStatement)
			{
				try
				{
					SQLite3.Finalize(Statement);
				}
				finally
				{
					Statement = NullStatement;
					Connection = null;
				}
			}
		}

		~PreparedSqlLiteInsertCommand()
		{
			Dispose(disposing: false);
		}
	}
}
