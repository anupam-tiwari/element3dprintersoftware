using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.MatterControl.SlicerConfiguration;
using Newtonsoft.Json;

namespace MatterHackers.MatterControl.SettingsManagement
{
	public class OemSettings
	{
		private static OemSettings instance;

		public bool UseSimpleModeByDefault;

		public string ThemeColor = "";

		public string AffiliateCode = "";

		public string WindowTitleExtra = "";

		public bool ShowShopButton = true;

		public bool CheckForUpdatesOnFirstRun;

		public static OemSettings Instance
		{
			get
			{
				if (instance == null)
				{
					instance = JsonConvert.DeserializeObject<OemSettings>(StaticData.get_Instance().ReadAllText(Path.Combine("OEMSettings", "Settings.json")));
				}
				return instance;
			}
		}

		public List<string> PrinterWhiteList
		{
			get;
			private set;
		} = new List<string>();


		public List<ManufacturerNameMapping> ManufacturerNameMappings
		{
			get;
			set;
		}

		public List<string> PreloadedLibraryFiles
		{
			get;
		} = new List<string>();


		public List<KeyValuePair<string, string>> AllOems
		{
			get;
			private set;
		}

		public OemProfileDictionary OemProfiles
		{
			get;
			set;
		}

		internal void SetManufacturers(IEnumerable<KeyValuePair<string, string>> unorderedManufacturers, List<string> whitelist = null)
		{
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			KeyValuePair<string, string> item = new KeyValuePair<string, string>(null, null);
			foreach (KeyValuePair<string, string> item2 in (IEnumerable<KeyValuePair<string, string>>)Enumerable.OrderBy<KeyValuePair<string, string>, string>(unorderedManufacturers, (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> k) => k.Value)))
			{
				if (item2.Value == "Other")
				{
					item = item2;
				}
				else
				{
					list.Add(item2);
				}
			}
			if (item.Key != null)
			{
				list.Add(item);
			}
			if (whitelist != null)
			{
				PrinterWhiteList = whitelist;
			}
			IEnumerable<KeyValuePair<string, string>> enumerable = ((list != null) ? Enumerable.Where<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>)list, (Func<KeyValuePair<string, string>, bool>)((KeyValuePair<string, string> keyValue) => PrinterWhiteList.Contains(keyValue.Key))) : null);
			if (enumerable == null || Enumerable.Count<KeyValuePair<string, string>>(enumerable) == 0)
			{
				enumerable = list;
			}
			List<KeyValuePair<string, string>> list2 = new List<KeyValuePair<string, string>>();
			foreach (KeyValuePair<string, string> keyValue2 in enumerable)
			{
				string value = keyValue2.Value;
				string text = Enumerable.FirstOrDefault<ManufacturerNameMapping>(Enumerable.Where<ManufacturerNameMapping>((IEnumerable<ManufacturerNameMapping>)ManufacturerNameMappings, (Func<ManufacturerNameMapping, bool>)((ManufacturerNameMapping m) => m.NameOnDisk == keyValue2.Key)))?.NameOnDisk;
				if (!string.IsNullOrEmpty(text))
				{
					value = text;
				}
				list2.Add(new KeyValuePair<string, string>(keyValue2.Key, value));
			}
			AllOems = list2;
		}

		[OnDeserialized]
		private void Deserialized(StreamingContext context)
		{
			OemProfiles = LoadOemProfiles();
			Dictionary<string, string> unorderedManufacturers = Enumerable.ToDictionary<string, string>((IEnumerable<string>)OemProfiles.Keys, (Func<string, string>)((string oem) => oem));
			SetManufacturers(unorderedManufacturers);
		}

		private OemProfileDictionary LoadOemProfiles()
		{
			string text = ApplicationController.CacheablePath("public-profiles", "oemprofiles.json");
			return JsonConvert.DeserializeObject<OemProfileDictionary>(File.Exists(text) ? File.ReadAllText(text) : StaticData.get_Instance().ReadAllText(Path.Combine("Profiles", "oemprofiles.json")));
		}

		public async Task ReloadOemProfiles(IProgress<SyncReportType> syncReport = null)
		{
			if (ApplicationController.GetPublicProfileList == null)
			{
				return;
			}
			await ApplicationController.LoadCacheableAsync("oemprofiles.json", "public-profiles", async delegate
			{
				OemProfileDictionary oemProfileDictionary = await ApplicationController.GetPublicProfileList();
				if (oemProfileDictionary != null)
				{
					OemProfiles = oemProfileDictionary;
					SetManufacturers(Enumerable.ToDictionary<string, string>((IEnumerable<string>)oemProfileDictionary.Keys, (Func<string, string>)((string oem) => oem)));
				}
				return oemProfileDictionary;
			});
			await DownloadMissingProfiles(syncReport);
		}

		private async Task DownloadMissingProfiles(IProgress<SyncReportType> syncReport)
		{
			SyncReportType reportValue = new SyncReportType();
			int index = 0;
			foreach (string oem in OemProfiles.Keys)
			{
				string cacheScope = Path.Combine("public-profiles", oem);
				index++;
				foreach (string model in OemProfiles[oem].Keys)
				{
					PublicDevice publicDevice = OemProfiles[oem][model];
					if (!File.Exists(ApplicationController.CacheablePath(cacheScope, publicDevice.CacheKey)))
					{
						await Task.Delay(20000);
						await ProfileManager.LoadOemProfileAsync(publicDevice, oem, model);
						if (syncReport != null)
						{
							reportValue.actionLabel = $"Downloading public profiles for {oem}...";
							reportValue.percComplete = (double)index / (double)OemProfiles.Count;
							syncReport.Report(reportValue);
						}
					}
				}
			}
		}

		private OemSettings()
		{
			ManufacturerNameMappings = new List<ManufacturerNameMapping>();
		}
	}
}
