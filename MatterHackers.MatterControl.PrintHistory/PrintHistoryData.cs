using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl.PrintHistory
{
	public class PrintHistoryData
	{
		public static readonly int RecordLimit = 20;

		public RootedObjectEventHandler HistoryCleared = new RootedObjectEventHandler();

		public bool ShowTimestamp;

		private static PrintHistoryData instance;

		public static PrintHistoryData Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new PrintHistoryData();
					PrinterConnectionAndCommunication.Instance.ConnectionSucceeded.RegisterEvent((EventHandler)PrintRecovery.CheckIfNeedToRecoverPrint, ref PrintHistoryData.unregisterEvents);
				}
				return instance;
			}
		}

		private static event EventHandler unregisterEvents;

		public IEnumerable<PrintTask> GetHistoryItems(int recordCount)
		{
			string query = ((!(UserSettings.Instance.get("PrintHistoryFilterShowCompleted") == "true")) ? $"SELECT * FROM PrintTask ORDER BY PrintStart DESC LIMIT {recordCount};" : $"SELECT * FROM PrintTask WHERE PrintComplete = 1 ORDER BY PrintStart DESC LIMIT {recordCount};");
			return Datastore.Instance.dbSQLite.Query<PrintTask>(query, Array.Empty<object>());
		}

		internal void ClearHistory()
		{
			Datastore.Instance.dbSQLite.ExecuteScalar<PrintTask>("DELETE FROM PrintTask;", Array.Empty<object>());
			HistoryCleared.CallEvents((object)this, (EventArgs)null);
		}
	}
}
