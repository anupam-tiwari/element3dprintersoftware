using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.PlatformAbstract;
using Newtonsoft.Json;

namespace MatterHackers.MatterControl.SettingsManagement
{
	public class BedSettings
	{
		private static BedSettings instance;

		public RectangleInt ActualBedInImage;

		public static BedSettings Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new BedSettings();
				}
				return instance;
			}
		}

		public static void SetMakeAndModel(string make, string model)
		{
			string text = Path.Combine("PrinterSettings", make, model, "BedSettings.json");
			if (StaticData.get_Instance().FileExists(text))
			{
				instance = JsonConvert.DeserializeObject<BedSettings>(StaticData.get_Instance().ReadAllText(text));
			}
			else
			{
				instance = new BedSettings();
			}
		}

		private BedSettings()
		{
		}
	}
}
