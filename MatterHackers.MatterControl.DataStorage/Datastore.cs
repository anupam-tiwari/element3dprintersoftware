using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MatterHackers.Agg.PlatformAbstract;
using SQLiteAndroid;
using SQLiteUnix;
using SQLiteWin32;

namespace MatterHackers.MatterControl.DataStorage
{
	public class Datastore
	{
		private bool wasExited;

		public bool ConnectionError;

		public ISQLite dbSQLite;

		private string datastoreLocation = ApplicationDataStorage.Instance.DatastorePath;

		private static Datastore globalInstance;

		private ApplicationSession activeSession;

		private bool TEST_FLAG;

		private List<Type> dataStoreTables = new List<Type>
		{
			typeof(PrintItemCollection),
			typeof(PrinterSetting),
			typeof(CustomCommands),
			typeof(SystemSetting),
			typeof(UserSetting),
			typeof(ApplicationSession),
			typeof(PrintItem),
			typeof(PrintTask),
			typeof(Printer),
			typeof(SliceSetting),
			typeof(SliceSettingsCollection)
		};

		public static Datastore Instance
		{
			get
			{
				if (globalInstance == null)
				{
					globalInstance = new Datastore();
				}
				return globalInstance;
			}
			internal set
			{
				globalInstance = value;
			}
		}

		public Datastore()
		{
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Expected I4, but got Unknown
			if (!File.Exists(datastoreLocation))
			{
				ApplicationDataStorage.Instance.FirstRun = true;
			}
			OSType operatingSystem = OsInformation.get_OperatingSystem();
			switch (operatingSystem - 1)
			{
			case 0:
				dbSQLite = new SQLiteWin32.SQLiteConnection(datastoreLocation);
				break;
			case 1:
				dbSQLite = new SQLiteUnix.SQLiteConnection(datastoreLocation);
				break;
			case 2:
				dbSQLite = new SQLiteUnix.SQLiteConnection(datastoreLocation);
				break;
			case 4:
				dbSQLite = new SQLiteAndroid.SQLiteConnection(datastoreLocation);
				break;
			default:
				throw new NotImplementedException();
			}
			if (!TEST_FLAG)
			{
				return;
			}
			foreach (Type dataStoreTable in dataStoreTables)
			{
				try
				{
					dbSQLite.DropTable(dataStoreTable);
				}
				catch
				{
				}
			}
		}

		public void Exit()
		{
			if (wasExited)
			{
				return;
			}
			wasExited = true;
			if (activeSession != null)
			{
				activeSession.SessionEnd = DateTime.Now;
				activeSession.Commit();
			}
			Thread.Sleep(100);
			try
			{
				dbSQLite.Close();
			}
			catch (Exception)
			{
				Thread.Sleep(1000);
				try
				{
					dbSQLite.Close();
				}
				catch (Exception)
				{
				}
			}
		}

		public void Initialize()
		{
			if (TEST_FLAG)
			{
				ValidateSchema();
				GenerateSampleData();
			}
			else
			{
				ValidateSchema();
			}
			StartSession();
		}

		public int RecordCount(string tableName)
		{
			string query = $"SELECT COUNT(*) FROM {tableName};";
			return Convert.ToInt32(Instance.dbSQLite.ExecuteScalar<string>(query, Array.Empty<object>()));
		}

		private void StartSession()
		{
			activeSession = new ApplicationSession();
			dbSQLite.Insert(activeSession);
		}

		private void GenerateSampleData()
		{
			for (int i = 1; i <= 5; i++)
			{
				Printer printer = new Printer();
				printer.ComPort = $"COM{i}";
				printer.BaudRate = "250000";
				printer.Name = $"Printer {i}";
				Instance.dbSQLite.Insert(printer);
			}
		}

		private void ValidateSchema()
		{
			foreach (Type dataStoreTable in dataStoreTables)
			{
				dbSQLite.CreateTable(dataStoreTable);
			}
		}

		public bool WasExited()
		{
			return Instance.wasExited;
		}
	}
}
