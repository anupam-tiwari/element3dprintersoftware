using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.DataStorage.ClassicDB;
using MatterHackers.MatterControl.SettingsManagement;
using Newtonsoft.Json;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class ProfileManager
	{
		public static RootedObjectEventHandler ProfilesListChanged;

		private static ProfileManager activeInstance;

		private static EventHandler unregisterEvents;

		public const string ProfileExtension = ".printer";

		public const string ConfigFileExtension = ".slice";

		public const string ProfileDocExtension = ".profiles";

		private object writeLock = new object();

		public static List<string> ThemeIndexNameMapping;

		public static ProfileManager Instance
		{
			get
			{
				return activeInstance;
			}
			private set
			{
				activeInstance = value;
				if (!(ActiveSliceSettings.Instance?.ID != activeInstance.LastProfileID))
				{
					return;
				}
				PrinterSettings lastProfile = LoadProfileAsync(activeInstance.LastProfileID).Result;
				if (MatterControlApplication.IsLoading)
				{
					ActiveSliceSettings.Instance = lastProfile ?? PrinterSettings.Empty;
					return;
				}
				UiThread.RunOnIdle((Action)delegate
				{
					ActiveSliceSettings.Instance = lastProfile ?? PrinterSettings.Empty;
				});
			}
		}

		public string UserName
		{
			get;
			set;
		}

		[JsonIgnore]
		private string UserProfilesDirectory => GetProfilesDirectoryForUser(UserName);

		[JsonIgnore]
		public string ProfilesDocPath => GetProfilesDocPathForUser(UserName);

		[JsonIgnore]
		public bool IsGuestProfile => UserName == "guest";

		public ObservableCollection<PrinterInfo> Profiles
		{
			get;
		} = new ObservableCollection<PrinterInfo>();


		[JsonIgnore]
		public IEnumerable<PrinterInfo> ActiveProfiles => Enumerable.ToList<PrinterInfo>(Enumerable.Where<PrinterInfo>((IEnumerable<PrinterInfo>)Profiles, (Func<PrinterInfo, bool>)((PrinterInfo profile) => !profile.MarkedForDelete)));

		[JsonIgnore]
		public PrinterInfo ActiveProfile
		{
			get
			{
				string text = ActiveSliceSettings.Instance?.ID;
				if (text == null)
				{
					return null;
				}
				return this[text];
			}
		}

		public PrinterInfo this[string profileID] => Enumerable.FirstOrDefault<PrinterInfo>(Enumerable.Where<PrinterInfo>((IEnumerable<PrinterInfo>)Profiles, (Func<PrinterInfo, bool>)((PrinterInfo p) => p.ID == profileID)));

		[JsonIgnore]
		public string LastProfileID
		{
			get
			{
				return UserSettings.Instance.get($"ActiveProfileID-{UserName}");
			}
			set
			{
				UserSettings.Instance.set($"ActiveProfileID-{UserName}", value);
			}
		}

		public bool PrintersImported
		{
			get;
			set;
		}

		static ProfileManager()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Expected O, but got Unknown
			ProfilesListChanged = new RootedObjectEventHandler();
			activeInstance = null;
			ThemeIndexNameMapping = new List<string>
			{
				"Blue - Dark",
				"Teal - Dark",
				"Green - Dark",
				"Light Blue - Dark",
				"Orange - Dark",
				"Purple - Dark",
				"Red - Dark",
				"Pink - Dark",
				"Grey - Dark",
				"Pink - Dark",
				"Blue - Light",
				"Teal - Light",
				"Green - Light",
				"Light Blue - Light",
				"Orange - Light",
				"Purple - Light",
				"Red - Light",
				"Pink - Light",
				"Grey - Light",
				"Pink - Light"
			};
			ActiveSliceSettings.SettingChanged.RegisterEvent((EventHandler)SettingsChanged, ref unregisterEvents);
			ReloadActiveUser();
		}

		private static string GetProfilesDocPathForUser(string userName)
		{
			return Path.Combine(GetProfilesDirectoryForUser(userName), string.Format("{0}{1}", userName, ".profiles"));
		}

		private static string GetProfilesDirectoryForUser(string userName)
		{
			string path = ((userName == "guest") ? userName : (userName ?? ""));
			string text = Path.Combine(ApplicationDataStorage.ApplicationUserDataPath, "Profiles", path);
			Directory.CreateDirectory(text);
			return text;
		}

		public static void ReloadActiveUser()
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Expected O, but got Unknown
			string fileSystemSafeUserName = AuthenticationData.Instance.FileSystemSafeUserName;
			if (string.IsNullOrEmpty(fileSystemSafeUserName) || !(Instance?.UserName == fileSystemSafeUserName))
			{
				if (Instance?.Profiles != null)
				{
					Instance.Profiles.remove_CollectionChanged(new NotifyCollectionChangedEventHandler(Profiles_CollectionChanged));
				}
				Instance = Load(fileSystemSafeUserName);
				Instance.Profiles.add_CollectionChanged(new NotifyCollectionChangedEventHandler(Profiles_CollectionChanged));
			}
		}

		public static ProfileManager Load(string userName)
		{
			if (string.IsNullOrEmpty(userName))
			{
				userName = "guest";
			}
			string profilesDocPathForUser = GetProfilesDocPathForUser(userName);
			ProfileManager profileManager;
			if (File.Exists(profilesDocPathForUser))
			{
				profileManager = JsonConvert.DeserializeObject<ProfileManager>(File.ReadAllText(profilesDocPathForUser));
				profileManager.UserName = userName;
			}
			else
			{
				profileManager = new ProfileManager
				{
					UserName = userName
				};
			}
			return profileManager;
		}

		internal static void SettingsChanged(object sender, EventArgs e)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			if (Instance?.ActiveProfile == null)
			{
				return;
			}
			string data = ((StringEventArgs)e).get_Data();
			if (!(data == "printer_name"))
			{
				if (data == "com_port")
				{
					Instance.ActiveProfile.ComPort = ActiveSliceSettings.Instance.Helpers.ComPort();
					Instance.Save();
				}
			}
			else
			{
				Instance.ActiveProfile.Name = ActiveSliceSettings.Instance.GetValue("printer_name");
				Instance.Save();
			}
		}

		public string ProfilePath(string printerID)
		{
			return ProfilePath(this[printerID]);
		}

		public string ProfilePath(PrinterInfo printer)
		{
			return Path.Combine(UserProfilesDirectory, printer.ID + ".printer");
		}

		public PrinterSettings LoadWithoutRecovery(string profileID)
		{
			PrinterInfo printerInfo = Instance[profileID];
			string text = printerInfo?.ProfilePath;
			if (text != null && File.Exists(text) && !printerInfo.MarkedForDelete)
			{
				try
				{
					return PrinterSettings.LoadFile(text);
				}
				catch
				{
					return null;
				}
			}
			return null;
		}

		public static async Task<PrinterSettings> LoadProfileAsync(string profileID, bool useActiveInstance = true)
		{
			if (useActiveInstance && ActiveSliceSettings.Instance?.ID == profileID)
			{
				return ActiveSliceSettings.Instance;
			}
			PrinterInfo printerInfo = Instance[profileID];
			if (printerInfo == null)
			{
				return null;
			}
			PrinterSettings printerSettings = Instance.LoadWithoutRecovery(profileID);
			if (printerSettings != null)
			{
				if (printerSettings.GetValue("printer_name") == "")
				{
					printerSettings.SetValue("printer_name", printerInfo.Name);
				}
				return printerSettings;
			}
			if (ApplicationController.GetPrinterProfileAsync != null)
			{
				printerSettings = await ApplicationController.GetPrinterProfileAsync(printerInfo, null);
				if (printerSettings != null)
				{
					printerSettings.Save();
					return printerSettings;
				}
			}
			return await PrinterSettings.RecoverProfile(printerInfo);
		}

		public static bool DuplicateFromExisting()
		{
			if (ActiveSliceSettings.Instance != null)
			{
				IEnumerable<string> enumerable = Enumerable.Select<PrinterInfo, string>(Instance.ActiveProfiles, (Func<PrinterInfo, string>)((PrinterInfo p) => p.Name));
				string value = ActiveSliceSettings.Instance.GetValue("printer_name");
				string make = ActiveSliceSettings.Instance.OemLayer["make"];
				string model = ActiveSliceSettings.Instance.OemLayer["model"];
				PrinterInfo printerInfo = new PrinterInfo
				{
					Name = agg_basics.GetNonCollidingName(enumerable, value),
					ID = Guid.NewGuid().ToString(),
					Make = make,
					Model = model
				};
				((Collection<PrinterInfo>)(object)Instance.Profiles).Add(printerInfo);
				PrinterSettings instance = ActiveSliceSettings.Instance;
				instance.ID = printerInfo.ID;
				instance.ClearValue("device_token");
				printerInfo.DeviceToken = "";
				instance.Helpers.SetName(printerInfo.Name);
				instance.Save();
			}
			return false;
		}

		internal static bool ImportFromExisting(string settingsFilePath)
		{
			if (string.IsNullOrEmpty(settingsFilePath) || !File.Exists(settingsFilePath))
			{
				return false;
			}
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(settingsFilePath);
			IEnumerable<string> enumerable = Enumerable.Select<PrinterInfo, string>(Instance.ActiveProfiles, (Func<PrinterInfo, string>)((PrinterInfo p) => p.Name));
			PrinterInfo printerInfo = new PrinterInfo
			{
				Name = agg_basics.GetNonCollidingName(enumerable, fileNameWithoutExtension),
				ID = Guid.NewGuid().ToString(),
				Make = "Other",
				Model = "Other"
			};
			bool result = false;
			string a = Path.GetExtension(settingsFilePath)!.ToLower();
			if (!(a == ".printer"))
			{
				if (a == ".ini")
				{
					PrinterSettingsLayer printerSettingsLayer = PrinterSettingsLayer.LoadFromIni(settingsFilePath);
					PrinterSettings printerSettings = new PrinterSettings
					{
						ID = printerInfo.ID
					};
					bool flag = false;
					printerSettings.OemLayer = new PrinterSettingsLayer();
					printerSettings.OemLayer["make"] = "Other";
					printerSettings.OemLayer["model"] = "Other";
					foreach (KeyValuePair<string, string> item in printerSettingsLayer)
					{
						if (printerSettings.Contains(item.Key))
						{
							flag = true;
							if (printerSettings.GetValue(item.Key).Trim() != item.Value)
							{
								printerSettings.OemLayer[item.Key] = item.Value;
							}
						}
					}
					if (flag)
					{
						printerSettings.UserLayer["printer_name"] = printerInfo.Name;
						printerSettings.ClearValue("device_token");
						printerInfo.DeviceToken = "";
						printerInfo.Make = printerSettings.OemLayer["make"] ?? "Other";
						printerInfo.Model = printerSettings.OemLayer["model"] ?? "Other";
						((Collection<PrinterInfo>)(object)Instance.Profiles).Add(printerInfo);
						printerSettings.Helpers.SetName(printerInfo.Name);
						printerSettings.Save();
						result = true;
					}
				}
			}
			else
			{
				((Collection<PrinterInfo>)(object)Instance.Profiles).Add(printerInfo);
				PrinterSettings printerSettings2 = PrinterSettings.LoadFile(settingsFilePath);
				printerSettings2.ID = printerInfo.ID;
				printerSettings2.ClearValue("device_token");
				printerInfo.DeviceToken = "";
				printerSettings2.Helpers.SetName(printerInfo.Name);
				if (printerSettings2.OemLayer.ContainsKey("make"))
				{
					printerInfo.Make = printerSettings2.OemLayer["make"];
				}
				if (printerSettings2.OemLayer.ContainsKey("model"))
				{
					printerInfo.Model = printerSettings2.OemLayer["model"] ?? "Other";
				}
				printerSettings2.Save();
				result = true;
			}
			return result;
		}

		internal static async Task<bool> CreateProfileAsync(string make, string model, string printerName)
		{
			string guid = Guid.NewGuid().ToString();
			PublicDevice publicDevice = OemSettings.Instance.OemProfiles[make][model];
			if (publicDevice == null)
			{
				return false;
			}
			PrinterSettings printerSettings = await LoadOemProfileAsync(publicDevice, make, model);
			if (printerSettings == null)
			{
				return false;
			}
			printerSettings.ID = guid;
			printerSettings.DocumentVersion = PrinterSettings.LatestVersion;
			printerSettings.UserLayer["printer_name".ToString()] = printerName;
			printerSettings.UserLayer["active_theme_name"] = ActiveTheme.get_Instance().get_Name();
			((Collection<PrinterInfo>)(object)Instance.Profiles).Add(new PrinterInfo
			{
				Name = printerName,
				ID = guid,
				Make = make,
				Model = model
			});
			printerSettings.Save();
			Instance.LastProfileID = guid;
			ActiveSliceSettings.Instance = printerSettings;
			return true;
		}

		public static async Task<PrinterSettings> LoadOemProfileAsync(PublicDevice publicDevice, string make, string model)
		{
			string cacheScope = Path.Combine("public-profiles", make);
			string cachePath = ApplicationController.CacheablePath(cacheScope, publicDevice.CacheKey);
			return await ApplicationController.LoadCacheableAsync(publicDevice.CacheKey, cacheScope, async () => (File.Exists(cachePath) || ApplicationController.DownloadPublicProfileAsync == null) ? null : (await ApplicationController.DownloadPublicProfileAsync(publicDevice.ProfileToken)), Path.Combine("Profiles", make, model + ".printer"));
		}

		public void EnsurePrintersImported()
		{
			if (IsGuestProfile && !PrintersImported)
			{
				ClassicSqlitePrinterProfiles.ImportPrinters(Instance, UserProfilesDirectory);
				PrintersImported = true;
				Save();
			}
		}

		private static void Profiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Instance.Save();
			ProfilesListChanged.CallEvents((object)null, (EventArgs)null);
			ApplicationController.SyncPrinterProfiles?.Invoke("ProfileManager.Profiles_CollectionChanged()", null);
		}

		public void Save()
		{
			lock (writeLock)
			{
				File.WriteAllText(ProfilesDocPath, JsonConvert.SerializeObject((object)this, (Formatting)1));
			}
		}
	}
}
