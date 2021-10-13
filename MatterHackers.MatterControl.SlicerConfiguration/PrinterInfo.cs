using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class PrinterInfo
	{
		[JsonProperty(PropertyName = "ID")]
		private string id;

		public string ComPort
		{
			get;
			set;
		}

		[JsonIgnore]
		public string ID
		{
			get
			{
				return id;
			}
			set
			{
				if (ActiveSliceSettings.Instance.ID == ID)
				{
					ActiveSliceSettings.Instance.ID = value;
				}
				string profilePath = ProfilePath;
				if (File.Exists(profilePath))
				{
					id = value;
					File.Move(profilePath, ProfilePath);
				}
				else
				{
					id = value;
				}
				if (File.Exists(ProfilePath) && ProfileManager.Instance[id] != null)
				{
					PrinterSettings printerSettings = PrinterSettings.LoadFile(ProfilePath);
					printerSettings.ID = value;
					printerSettings.Save();
				}
			}
		}

		public string Name
		{
			get;
			set;
		}

		public string Make
		{
			get;
			set;
		}

		public string Model
		{
			get;
			set;
		}

		public string DeviceToken
		{
			get;
			set;
		}

		public bool IsDirty => ServerSHA1 != ContentSHA1;

		public bool MarkedForDelete
		{
			get;
			set;
		}

		public string ContentSHA1
		{
			get;
			set;
		}

		public string ServerSHA1
		{
			get;
			set;
		}

		[JsonIgnore]
		public string ProfilePath => ProfileManager.Instance.ProfilePath(this);

		[OnDeserialized]
		public void OnDeserialized(StreamingContext context)
		{
			if (string.IsNullOrEmpty(Make))
			{
				Make = "Other";
			}
			if (string.IsNullOrEmpty(Model))
			{
				Model = "Other";
			}
		}
	}
}
