using System;
using System.Collections;
using System.Collections.Generic;

namespace MatterHackers.MatterControl.DataStorage
{
	public interface ISQLite
	{
		int Insert(object obj);

		int CreateTable(Type ty);

		int DropTable(Type ty);

		int Update(object obj);

		int Delete(object obj);

		ITableQuery<T> Table<T>() where T : new();

		List<T> Query<T>(string query, params object[] args) where T : new();

		T ExecuteScalar<T>(string query, params object[] args);

		int InsertAll(IEnumerable objects);

		void RunInTransaction(Action action);

		void Close();
	}
}
