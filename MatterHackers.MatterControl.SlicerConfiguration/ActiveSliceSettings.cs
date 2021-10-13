using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MatterHackers.Agg;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SettingsManagement;
using Newtonsoft.Json;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class ActiveSliceSettings
	{
		public static RootedObjectEventHandler ActivePrinterChanged;

		public static RootedObjectEventHandler ActiveProfileModified;

		public static RootedObjectEventHandler SettingChanged;

		private static PrinterSettings activeInstance;

		private static bool showConnectionHelp;

		private static Dictionary<string, SliceSettingData> settingsByName;

		public static PrinterSettings Instance
		{
			get
			{
				return activeInstance;
			}
			set
			{
				if (activeInstance == value || value == null)
				{
					return;
				}
				if (activeInstance != PrinterSettings.Empty)
				{
					PrinterConnectionAndCommunication.Instance.Disable();
				}
				activeInstance = value;
				BedSettings.SetMakeAndModel(activeInstance.GetValue("make"), activeInstance.GetValue("model"));
				SwitchToPrinterTheme(!MatterControlApplication.IsLoading);
				if (MatterControlApplication.IsLoading)
				{
					return;
				}
				OnActivePrinterChanged(null);
				if (Instance.PrinterSelected && Instance.GetValue<bool>("auto_connect"))
				{
					UiThread.RunOnIdle((Action)delegate
					{
						PrinterConnectionAndCommunication.Instance.ConnectToActivePrinter(showConnectionHelp);
						showConnectionHelp = false;
					}, 2.0);
				}
			}
		}

		public static List<SliceSettingData> SettingsData
		{
			get;
			private set;
		}

		public static void ShowComPortConnectionHelp()
		{
			showConnectionHelp = true;
		}

		static ActiveSliceSettings()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Expected O, but got Unknown
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Expected O, but got Unknown
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			ActivePrinterChanged = new RootedObjectEventHandler();
			ActiveProfileModified = new RootedObjectEventHandler();
			SettingChanged = new RootedObjectEventHandler();
			showConnectionHelp = false;
			SettingsData = new List<SliceSettingData>();
			SettingsData = JsonConvert.DeserializeObject<List<SliceSettingData>>(StaticData.get_Instance().ReadAllText(Path.Combine("SliceSettings", "Properties.json")));
			activeInstance = PrinterSettings.Empty;
			settingsByName = new Dictionary<string, SliceSettingData>();
			foreach (SliceSettingData settingsDatum in SettingsData)
			{
				settingsByName.Add(settingsDatum.SlicerConfigName, settingsDatum);
			}
			activeInstance = PrinterSettings.Empty;
		}

		public static void OnSettingChanged(string slicerConfigName)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			SettingChanged.CallEvents((object)null, (EventArgs)new StringEventArgs(slicerConfigName));
			if (settingsByName.TryGetValue(slicerConfigName, out var value) && value.ReloadUiWhenChanged)
			{
				UiThread.RunOnIdle((Action)ApplicationController.Instance.ReloadAll);
			}
		}

		public static void RefreshActiveInstance(PrinterSettings updatedProfile)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			bool num = activeInstance.GetValue("active_theme_name") != updatedProfile.GetValue("active_theme_name");
			activeInstance = updatedProfile;
			SettingChanged.CallEvents((object)null, (EventArgs)new StringEventArgs("printer_name"));
			if (num)
			{
				UiThread.RunOnIdle((Action)delegate
				{
					SwitchToPrinterTheme(doReloadEvent: true);
				});
			}
			else
			{
				UiThread.RunOnIdle((Action)ApplicationController.Instance.ReloadAdvancedControlsPanel);
			}
		}

		public static void SwitchToPrinterTheme(bool doReloadEvent)
		{
			if (!Instance.PrinterSelected)
			{
				return;
			}
			string value = Instance.GetValue("active_theme_name");
			if (!string.IsNullOrEmpty(value))
			{
				if (!doReloadEvent)
				{
					ActiveTheme.SuspendEvents();
				}
				ActiveTheme.set_Instance(ActiveTheme.GetThemeColors(value));
				ActiveTheme.ResumeEvents();
			}
		}

		internal static async Task SwitchToProfile(string printerID)
		{
			ProfileManager.Instance.LastProfileID = printerID;
			Instance = (await ProfileManager.LoadProfileAsync(printerID)) ?? PrinterSettings.Empty;
		}

		private static void OnActivePrinterChanged(EventArgs e)
		{
			ActivePrinterChanged.CallEvents((object)null, e);
		}
	}
}
