namespace MatterHackers.MatterControl.DataStorage
{
	public class Printer : Entity
	{
		public string _features
		{
			get;
			set;
		}

		public bool AutoConnect
		{
			get;
			set;
		}

		public string BaudRate
		{
			get;
			set;
		}

		public string ComPort
		{
			get;
			set;
		}

		public string CurrentSlicingEngine
		{
			get;
			set;
		}

		public int DefaultSettingsCollectionId
		{
			get;
			set;
		}

		public string DeviceToken
		{
			get;
			set;
		}

		public string DeviceType
		{
			get;
			set;
		}

		public bool DoPrintLeveling
		{
			get;
			set;
		}

		public string DriverType
		{
			get;
			set;
		}

		public string Make
		{
			get;
			set;
		}

		public string ManualMovementSpeeds
		{
			get;
			set;
		}

		public string MaterialCollectionIds
		{
			get;
			set;
		}

		public string Model
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string PrintLevelingJsonData
		{
			get;
			set;
		}

		public string PrintLevelingProbePositions
		{
			get;
			set;
		}

		public int QualityCollectionId
		{
			get;
			set;
		}

		public Printer()
		{
			Make = "Unknown";
			Model = "Unknown";
		}
	}
}
