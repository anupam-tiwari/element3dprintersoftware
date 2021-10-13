using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.PrintHistory;
using MatterHackers.MatterControl.PrintLibrary;
using MatterHackers.MatterControl.PrintLibrary.Provider;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.MatterControl.SettingsManagement;
using MatterHackers.MatterControl.SlicerConfiguration;
using Newtonsoft.Json;

namespace MatterHackers.MatterControl
{
	public class ApplicationController
	{
		public class CloudSyncEventArgs : EventArgs
		{
			public bool IsAuthenticated
			{
				get;
				set;
			}
		}

		public Action RedeemDesignCode;

		public Action EnterShareCode;

		private static ApplicationController globalInstance;

		public RootedObjectEventHandler AdvancedControlsPanelReloading = new RootedObjectEventHandler();

		public RootedObjectEventHandler CloudSyncStatusChanged = new RootedObjectEventHandler();

		public RootedObjectEventHandler DoneReloadingAll = new RootedObjectEventHandler();

		public RootedObjectEventHandler PluginsLoaded = new RootedObjectEventHandler();

		public static Action SignInAction;

		public static Action SignOutAction;

		public static Action WebRequestFailed;

		public static Action WebRequestSucceeded;

		public const string EnvironmentName = "";

		public static Func<string, Task<Dictionary<string, string>>> GetProfileHistory;

		public static Func<PrinterInfo, string, Task<PrinterSettings>> GetPrinterProfileAsync;

		public static Func<string, IProgress<SyncReportType>, Task> SyncPrinterProfiles;

		public static Func<Task<OemProfileDictionary>> GetPublicProfileList;

		public static Func<string, Task<PrinterSettings>> DownloadPublicProfileAsync;

		public ApplicationView MainView;

		private EventHandler unregisterEvents;

		private static int applicationInstanceCount = 0;

		private static TypeFace monoSpacedTypeFace = null;

		private static string cacheDirectory = Path.Combine(ApplicationDataStorage.ApplicationUserDataPath, "data", "temp", "cache");

		private bool pendingReloadRequest;

		private static int reloadCount = 0;

		public LibraryDataView CurrentLibraryDataView;

		private EventHandler unregisterEvent;

		public SlicePresetsWindow EditMaterialPresetsWindow
		{
			get;
			set;
		}

		public SlicePresetsWindow EditQualityPresetsWindow
		{
			get;
			set;
		}

		public static int ApplicationInstanceCount
		{
			get
			{
				if (applicationInstanceCount == 0)
				{
					Assembly entryAssembly = Assembly.GetEntryAssembly();
					if (entryAssembly != null)
					{
						string value = Path.GetFileNameWithoutExtension(entryAssembly.Location)!.ToUpper();
						Process[] processes = Process.GetProcesses();
						foreach (Process val in processes)
						{
							try
							{
								if (val != null && val.get_ProcessName() != null && val.get_ProcessName().ToUpper().Contains(value))
								{
									applicationInstanceCount++;
								}
							}
							catch
							{
							}
						}
					}
				}
				return applicationInstanceCount;
			}
		}

		public static TypeFace MonoSpacedTypeFace
		{
			get
			{
				if (monoSpacedTypeFace == null)
				{
					monoSpacedTypeFace = TypeFace.LoadFrom(StaticData.get_Instance().ReadAllText(Path.Combine("Fonts", "LiberationMono.svg")));
				}
				return monoSpacedTypeFace;
			}
		}

		public static ApplicationController Instance
		{
			get
			{
				if (globalInstance == null)
				{
					globalInstance = new ApplicationController();
					LoadOemOrDefaultTheme();
					_ = ProfileManager.Instance.IsGuestProfile;
					if (UserSettings.Instance.IsTouchScreen)
					{
						new LibraryProviderSQLite(null, null, null, null);
						_ = PrintHistoryData.Instance;
						globalInstance.MainView = new TouchscreenView();
					}
					else
					{
						globalInstance.MainView = new DesktopView();
					}
					ActiveSliceSettings.ActivePrinterChanged.RegisterEvent((EventHandler)delegate
					{
						Instance.ReloadAll();
					}, ref globalInstance.unregisterEvents);
				}
				return globalInstance;
			}
		}

		public static event EventHandler Load;

		public event EventHandler ApplicationClosed;

		public ApplicationController()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			ActiveTheme.ThemeChanged.RegisterEvent((EventHandler)delegate
			{
				ReloadAll();
			}, ref unregisterEvents);
			ApplicationClosed += delegate
			{
				ApplicationSettings.Instance.ReleaseClientToken();
			};
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)delegate
			{
				PrinterConnectionAndCommunication.CommunicationStates communicationState = PrinterConnectionAndCommunication.Instance.CommunicationState;
				if (communicationState == PrinterConnectionAndCommunication.CommunicationStates.Printing && UserSettings.Instance.IsTouchScreen)
				{
					UiThread.RunOnIdle((Action)PrintingWindow.Show);
				}
			}, ref unregisterEvents);
		}

		public void StartSignIn()
		{
			if (PrinterConnectionAndCommunication.Instance.PrinterIsPrinting || PrinterConnectionAndCommunication.Instance.PrinterIsPaused)
			{
				UiThread.RunOnIdle((Action)delegate
				{
					StyledMessageBox.ShowMessageBox(null, "Please wait until the print has finished and try again.".Localize(), "Can't sign in while printing".Localize());
				});
			}
			else
			{
				SignInAction?.Invoke();
			}
		}

		public static async Task<T> LoadCacheableAsync<T>(string cacheKey, string cacheScope, Func<Task<T>> collector, string staticDataFallbackPath = null) where T : class
		{
			string cachePath = CacheablePath(cacheScope, cacheKey);
			try
			{
				T val = await collector();
				if (val != null)
				{
					File.WriteAllText(cachePath, JsonConvert.SerializeObject((object)val, (Formatting)1));
					return val;
				}
			}
			catch
			{
			}
			try
			{
				if (File.Exists(cachePath))
				{
					return JsonConvert.DeserializeObject<T>(File.ReadAllText(cachePath));
				}
			}
			catch
			{
			}
			try
			{
				if (staticDataFallbackPath != null && StaticData.get_Instance().FileExists(staticDataFallbackPath))
				{
					return JsonConvert.DeserializeObject<T>(StaticData.get_Instance().ReadAllText(staticDataFallbackPath));
				}
			}
			catch
			{
				return null;
			}
			return null;
		}

		public static string CacheablePath(string cacheScope, string cacheKey)
		{
			string text = Path.Combine(cacheDirectory, cacheScope);
			Directory.CreateDirectory(text);
			return Path.Combine(text, cacheKey);
		}

		public void StartSignOut()
		{
			if (PrinterConnectionAndCommunication.Instance.PrinterIsPrinting || PrinterConnectionAndCommunication.Instance.PrinterIsPaused)
			{
				UiThread.RunOnIdle((Action)delegate
				{
					StyledMessageBox.ShowMessageBox(null, "Please wait until the print has finished and try again.".Localize(), "Can't log out while printing".Localize());
				});
			}
			else if (true)
			{
				StyledMessageBox.ShowMessageBox(delegate(bool clickedSignOut)
				{
					if (clickedSignOut)
					{
						SignOutAction?.Invoke();
					}
				}, "Are you sure you want to sign out? You will not have access to your printer profiles or cloud library.".Localize(), "Sign Out?".Localize(), StyledMessageBox.MessageType.YES_NO, "Sign Out".Localize(), "Cancel".Localize());
			}
			else
			{
				SignOutAction?.Invoke();
			}
		}

		public void ReloadAll()
		{
			if (pendingReloadRequest || MainView == null)
			{
				return;
			}
			pendingReloadRequest = true;
			UiThread.RunOnIdle((Action)delegate
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Expected O, but got Unknown
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Expected O, but got Unknown
				QuickTimer val = new QuickTimer($"ReloadAll_{reloadCount++}:");
				try
				{
					PopOutManager.SaveIfClosed = false;
					WidescreenPanel.PreChangePanels.CallEvents((object)this, (EventArgs)null);
					ApplicationView mainView = MainView;
					if (mainView != null)
					{
						((GuiWidget)mainView).CloseAllChildren();
					}
					QuickTimer val2 = new QuickTimer("ReloadAll_AddElements");
					try
					{
						MainView?.CreateAndAddChildren();
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
					PopOutManager.SaveIfClosed = true;
					RootedObjectEventHandler doneReloadingAll = DoneReloadingAll;
					if (doneReloadingAll != null)
					{
						doneReloadingAll.CallEvents((object)null, (EventArgs)null);
					}
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
				pendingReloadRequest = false;
			});
		}

		public void OnApplicationClosed()
		{
			this.ApplicationClosed?.Invoke(null, null);
		}

		private static void LoadOemOrDefaultTheme()
		{
			ActiveTheme.SuspendEvents();
			string themeColor = OemSettings.Instance.ThemeColor;
			if (string.IsNullOrEmpty(themeColor))
			{
				ActiveTheme.set_Instance(ActiveTheme.GetThemeColors("Blue - Light"));
			}
			else
			{
				ActiveTheme.set_Instance(ActiveTheme.GetThemeColors(themeColor));
			}
			ActiveTheme.ResumeEvents();
		}

		public void ReloadAdvancedControlsPanel()
		{
			AdvancedControlsPanelReloading.CallEvents((object)this, (EventArgs)null);
		}

		public void SwitchToPurchasedLibrary()
		{
			if (CurrentLibraryDataView?.CurrentLibraryProvider?.GetRootProvider() == null)
			{
				return;
			}
			LibraryProviderSelector libraryProviderSelector = CurrentLibraryDataView.CurrentLibraryProvider.GetRootProvider() as LibraryProviderSelector;
			if (libraryProviderSelector != null)
			{
				LibraryProvider purchaseProvider = libraryProviderSelector.GetPurchasedLibrary();
				UiThread.RunOnIdle((Action)delegate
				{
					CurrentLibraryDataView.CurrentLibraryProvider = purchaseProvider;
				});
			}
		}

		public void SwitchToSharedLibrary()
		{
			if (CurrentLibraryDataView?.CurrentLibraryProvider?.GetRootProvider() == null)
			{
				return;
			}
			LibraryProviderSelector libraryProviderSelector = CurrentLibraryDataView.CurrentLibraryProvider.GetRootProvider() as LibraryProviderSelector;
			if (libraryProviderSelector != null)
			{
				LibraryProvider sharedProvider = libraryProviderSelector.GetSharedLibrary();
				UiThread.RunOnIdle((Action)delegate
				{
					CurrentLibraryDataView.CurrentLibraryProvider = sharedProvider;
				});
			}
		}

		public void ChangeCloudSyncStatus(bool userAuthenticated, string reason = "")
		{
			UserSettings.Instance.set("CredentialsInvalid", userAuthenticated ? "false" : "true");
			UserSettings.Instance.set("CredentialsInvalidReason", userAuthenticated ? "" : reason);
			CloudSyncStatusChanged.CallEvents((object)this, (EventArgs)new CloudSyncEventArgs
			{
				IsAuthenticated = userAuthenticated
			});
			if (!string.IsNullOrEmpty(AuthenticationData.Instance.ActiveSessionUsername) && AuthenticationData.Instance.ActiveSessionUsername != AuthenticationData.Instance.LastSessionUsername)
			{
				AuthenticationData.Instance.LastSessionUsername = AuthenticationData.Instance.ActiveSessionUsername;
			}
			UserChanged();
		}

		public void UserChanged()
		{
			ProfileManager.ReloadActiveUser();
			ProfileManager.Instance.EnsurePrintersImported();
			ProfileManager profileManager = ProfileManager.Load("guest");
			int num;
			if (profileManager == null)
			{
				num = 0;
			}
			else
			{
				ObservableCollection<PrinterInfo> profiles = profileManager.Profiles;
				num = ((((profiles != null) ? new bool?(Enumerable.Any<PrinterInfo>((IEnumerable<PrinterInfo>)profiles)) : null) == true) ? 1 : 0);
			}
			if (num != 0 && !ProfileManager.Instance.IsGuestProfile && !ProfileManager.Instance.PrintersImported)
			{
				WizardWindow.Show<CopyGuestProfilesToUser>("/CopyGuestProfiles", "Copy Printers");
			}
		}

		public void OnLoadActions()
		{
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Invalid comparison between Unknown and I4
			ApplicationController.Load?.Invoke(this, null);
			Instance.UserChanged();
			if (!File.Exists("/storage/sdcard0/Download/LaunchTestPrint.stl"))
			{
				if (WizardWindow.ShouldShowAuthPanel?.Invoke() ?? false)
				{
					if (ApplicationSettings.Instance.get("SuppressAuthPanel") != "True")
					{
						UiThread.RunOnIdle((Action)delegate
						{
							WizardWindow.ShowPrinterSetup();
						});
					}
				}
				else if (SyncPrinterProfiles == null)
				{
					RunSetupIfRequired();
				}
				else
				{
					SyncPrinterProfiles("ApplicationController.OnLoadActions()", null).ContinueWith(delegate
					{
						RunSetupIfRequired();
					});
				}
				if ((int)OsInformation.get_OperatingSystem() == 5 && UserSettings.Instance.get("SoftwareLicenseAccepted") != "true")
				{
					UiThread.RunOnIdle((Action)delegate
					{
						WizardWindow.Show<LicenseAgreementPage>("SoftwareLicense", "Software License Agreement");
					});
				}
			}
			else
			{
				StartPrintingTest();
			}
			if (ActiveSliceSettings.Instance.PrinterSelected && ActiveSliceSettings.Instance.GetValue<bool>("auto_connect"))
			{
				UiThread.RunOnIdle((Action)delegate
				{
					PrinterConnectionAndCommunication.Instance.ConnectToActivePrinter();
				}, 2.0);
			}
		}

		private static void RunSetupIfRequired()
		{
			Instance.ReloadAdvancedControlsPanel();
			if (!Enumerable.Any<PrinterInfo>(ProfileManager.Instance.ActiveProfiles))
			{
				UiThread.RunOnIdle((Action)delegate
				{
					WizardWindow.ShowPrinterSetup();
				});
			}
		}

		public void StartPrintingTest()
		{
			QueueData.Instance.RemoveAll();
			QueueData.Instance.AddItem(new PrintItemWrapper(new PrintItem("LaunchTestPrint", "/storage/sdcard0/Download/LaunchTestPrint.stl")));
			PrinterConnectionAndCommunication.Instance.ConnectToActivePrinter();
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)delegate
			{
				if (PrinterConnectionAndCommunication.Instance.CommunicationState == PrinterConnectionAndCommunication.CommunicationStates.Connected)
				{
					PrinterConnectionAndCommunication.Instance.PrintActivePartIfPossible();
				}
			}, ref unregisterEvent);
		}

		public void ReloadLibrarySelectorUI()
		{
			LibraryProviderSelector.Reload();
		}

		public void ReloadLibraryUI()
		{
			PrintLibraryWidget.Reload();
		}

		public void DownloadToImageAsync(ImageBuffer imageToLoadInto, string uriToLoad, bool scaleToImageX, IRecieveBlenderByte scalingBlender = null)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Expected O, but got Unknown
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Expected O, but got Unknown
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			if (scalingBlender == null)
			{
				scalingBlender = (IRecieveBlenderByte)new BlenderBGRA();
			}
			WebClient val = new WebClient();
			val.add_DownloadDataCompleted((DownloadDataCompletedEventHandler)delegate(object sender, DownloadDataCompletedEventArgs e)
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Expected O, but got Unknown
				try
				{
					Stream stream = new MemoryStream(e.get_Result());
					ImageBuffer val2 = new ImageBuffer(10, 10);
					if (scaleToImageX)
					{
						StaticData.get_Instance().LoadImageData(stream, val2);
						while (val2.get_Width() > imageToLoadInto.get_Width() * 2)
						{
							ImageBuffer val3 = new ImageBuffer(val2.get_Width() / 2, val2.get_Height() / 2, 32, scalingBlender);
							val3.NewGraphics2D().Render((IImageByte)(object)val2, 0.0, 0.0, 0.0, (double)val3.get_Width() / (double)val2.get_Width(), (double)val3.get_Height() / (double)val2.get_Height());
							val2 = val3;
						}
						double num = (double)imageToLoadInto.get_Width() / (double)val2.get_Width();
						imageToLoadInto.Allocate(imageToLoadInto.get_Width(), (int)((double)val2.get_Height() * num), imageToLoadInto.get_Width() * (imageToLoadInto.get_BitDepth() / 8), imageToLoadInto.get_BitDepth());
						imageToLoadInto.NewGraphics2D().Render((IImageByte)(object)val2, 0.0, 0.0, 0.0, num, num);
					}
					else
					{
						StaticData.get_Instance().LoadImageData(stream, imageToLoadInto);
					}
					imageToLoadInto.MarkImageChanged();
				}
				catch
				{
				}
			});
			try
			{
				val.DownloadDataAsync(new Uri(uriToLoad));
			}
			catch
			{
			}
		}

		public bool ConditionalCancelPrint()
		{
			bool canceled = false;
			if (PrinterConnectionAndCommunication.Instance.SecondsPrinted > 120)
			{
				StyledMessageBox.ShowMessageBox(delegate(bool response)
				{
					if (response)
					{
						UiThread.RunOnIdle((Action)delegate
						{
							PrinterConnectionAndCommunication.Instance.Stop();
						});
						canceled = true;
					}
					canceled = false;
				}, "Cancel the current print?".Localize(), "Cancel Print?".Localize(), StyledMessageBox.MessageType.YES_NO, "Cancel Print".Localize(), "Continue Printing".Localize());
			}
			else
			{
				PrinterConnectionAndCommunication.Instance.Stop();
				canceled = false;
			}
			return canceled;
		}
	}
}
