using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Media;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Gaming.Game;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MarchingSquares;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.MatterControl.PluginSystem;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.MatterControl.SettingsManagement;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.MatterSlice;
using MatterHackers.PolygonMesh.Processors;
using MatterHackers.RenderOpenGl;
using MatterHackers.RenderOpenGl.OpenGl;
using MatterHackers.VectorMath;
using Mindscape.Raygun4Net;
using Newtonsoft.Json;

namespace MatterHackers.MatterControl
{
	public class MatterControlApplication : SystemWindow
	{
		public enum ReportSeverity2
		{
			Warning,
			Error
		}

		public bool RestartOnClose;

		private static MatterControlApplication instance;

		private string[] commandLineArgs;

		private string confirmExit = "Confirm Exit".Localize();

		private bool DoCGCollectEveryDraw;

		private int drawCount;

		private AverageMillisecondTimer millisecondTimer = new AverageMillisecondTimer();

		private DataViewGraph msGraph;

		private string savePartsSheetExitAnywayMessage = "You are currently saving a parts sheet, are you sure you want to exit?".Localize();

		private bool ShowMemoryUsed;

		private Stopwatch totalDrawTime = new Stopwatch();

		private const int RaygunMaxNotifications = 15;

		private static int raygunNotificationCount;

		private static RaygunClient _raygunClient;

		private bool dropWasOnChild = true;

		private EventHandler unregisterEvent;

		private bool closeHasBeenConfirmed;

		private bool closeMessageBoxIsOpen;

		private bool showNamesUnderMouse;

		public static string MCWSBaseUri
		{
			get;
		}

		public static bool CameraInUseByExternalProcess
		{
			get;
			set;
		}

		public static bool IsLoading
		{
			get;
			private set;
		}

		public static MatterControlApplication Instance
		{
			get
			{
				if (instance == null)
				{
					instance = CreateInstance();
					((SystemWindow)instance).ShowAsSystemWindow();
				}
				return instance;
			}
		}

		private static Vector2 minSize
		{
			get;
			set;
		}

		public event EventHandler PictureTaken;

		public void ConfigureWifi()
		{
		}

		private static RaygunClient GetCorrectClient()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			if ((int)OsInformation.get_OperatingSystem() != 2)
			{
				return new RaygunClient("hQIlyUUZRGPyXVXbI6l1dA==");
			}
			return new RaygunClient("qmMBpKy3OSTJj83+tkO7BQ==");
		}

		public static void RequestPowerShutDown()
		{
		}

		private static void ChangeToCurrentDirectory()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Invalid comparison between Unknown and I4
			string directoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);
			if (string.IsNullOrEmpty(directoryName) && (int)OsInformation.get_OperatingSystem() == 2)
			{
				Directory.SetCurrentDirectory(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]));
			}
			else
			{
				Directory.SetCurrentDirectory(directoryName);
			}
		}

		static MatterControlApplication()
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Invalid comparison between Unknown and I4
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			MCWSBaseUri = "https://mattercontrol.appspot.com";
			CameraInUseByExternalProcess = false;
			raygunNotificationCount = 0;
			_raygunClient = GetCorrectClient();
			IsLoading = true;
			minSize = new Vector2(600.0, 600.0);
			if ((int)OsInformation.get_OperatingSystem() == 2 && StaticData.get_Instance() == null)
			{
				ChangeToCurrentDirectory();
			}
			if (StaticData.get_Instance() == null)
			{
				StaticData.set_Instance((IStaticData)new FileSystemStaticData());
			}
		}

		private MatterControlApplication(double width, double height)
			: this(width, height)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Expected O, but got Unknown
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Expected O, but got Unknown
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			ApplicationSettings.Instance.set("HardwareHasCamera", "false");
			((GuiWidget)this).set_Name("Element");
			UserSettings.Instance.Fields.StartCount = UserSettings.Instance.Fields.StartCount + 1;
			commandLineArgs = Environment.GetCommandLineArgs();
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			bool hasBeenRun;
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				string text = commandLineArgs[i];
				switch (text.ToUpper())
				{
				case "FORCE_SOFTWARE_RENDERING":
					GL.set_HardwareAvailable(false);
					break;
				case "CLEAR_CACHE":
					AboutWidget.DeleteCacheData(0);
					break;
				case "SHOW_MEMORY":
					ShowMemoryUsed = true;
					break;
				case "DO_GC_COLLECT_EVERY_DRAW":
					ShowMemoryUsed = true;
					DoCGCollectEveryDraw = true;
					break;
				case "CONNECT_TO_PRINTER":
					if (i + 1 <= commandLineArgs.Length)
					{
						PrinterConnectionAndCommunication.Instance.ConnectToActivePrinter();
					}
					break;
				case "START_PRINT":
				{
					if (i + 1 > commandLineArgs.Length)
					{
						break;
					}
					hasBeenRun = false;
					i++;
					string text3 = commandLineArgs[i];
					QueueData.Instance.RemoveAll();
					if (string.IsNullOrEmpty(text3))
					{
						break;
					}
					string fileNameWithoutExtension2 = Path.GetFileNameWithoutExtension(text3);
					QueueData.Instance.AddItem(new PrintItemWrapper(new PrintItem(fileNameWithoutExtension2, text3)));
					PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)delegate
					{
						if (!hasBeenRun && PrinterConnectionAndCommunication.Instance.CommunicationState == PrinterConnectionAndCommunication.CommunicationStates.Connected)
						{
							hasBeenRun = true;
							PrinterConnectionAndCommunication.Instance.PrintActivePartIfPossible();
						}
					}, ref unregisterEvent);
					break;
				}
				case "SLICE_AND_EXPORT_GCODE":
					if (i + 1 <= commandLineArgs.Length)
					{
						i++;
						string text2 = commandLineArgs[i];
						QueueData.Instance.RemoveAll();
						if (!string.IsNullOrEmpty(text2))
						{
							string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text2);
							PrintItemWrapper printItemWrapper = new PrintItemWrapper(new PrintItem(fileNameWithoutExtension, text2));
							QueueData.Instance.AddItem(printItemWrapper);
							SlicingQueue.Instance.QueuePartForSlicing(printItemWrapper);
							new ExportPrintItemWindow(printItemWrapper).ExportGcodeCommandLineUtility(fileNameWithoutExtension);
						}
					}
					break;
				}
				MeshFileIo.ValidFileExtensions().Contains(Path.GetExtension(text)!.ToUpper());
			}
			if (File.Exists("RunUnitTests.txt") && !Clipboard.get_IsInitialized())
			{
				Clipboard.SetSystemClipboard((ISystemClipboard)new WindowsFormsClipboard());
			}
			GuiWidget.set_DefaultEnforceIntegerBounds(true);
			if (UserSettings.Instance.IsTouchScreen)
			{
				GuiWidget.set_DeviceScale(1.3);
				SystemWindow.set_ShareSingleOsWindow(true);
			}
			PerformanceTimer val = new PerformanceTimer("Startup", "MainView");
			try
			{
				((GuiWidget)this).AddChild((GuiWidget)(object)ApplicationController.Instance.MainView, -1);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			((GuiWidget)this).set_MinimumSize(minSize);
			((GuiWidget)this).set_Padding(new BorderDouble(0.0));
			((GuiWidget)this).AnchorAll();
			if (GL.get_HardwareAvailable())
			{
				((SystemWindow)this).set_UseOpenGL(true);
			}
			string releaseVersion = VersionInfo.Instance.ReleaseVersion;
			((SystemWindow)this).set_Title(StringHelper.FormatWith("Element {0}", new object[1]
			{
				releaseVersion
			}));
			if (OemSettings.Instance.WindowTitleExtra != null && OemSettings.Instance.WindowTitleExtra.Trim().Length > 0)
			{
				((SystemWindow)this).set_Title(((SystemWindow)this).get_Title() + StringHelper.FormatWith(" - {1}", new object[2]
				{
					releaseVersion,
					OemSettings.Instance.WindowTitleExtra
				}));
			}
			UiThread.RunOnIdle((Action)CheckOnPrinter);
			string text4 = ApplicationSettings.Instance.get("DesktopPosition");
			if (!string.IsNullOrEmpty(text4))
			{
				string[] array = text4.Split(new char[1]
				{
					','
				});
				int num = Math.Max(int.Parse(array[0]), -10);
				int num2 = Math.Max(int.Parse(array[1]), -10);
				((SystemWindow)this).set_DesktopPosition(new Point2D(num, num2));
			}
			else
			{
				((SystemWindow)this).set_DesktopPosition(new Point2D(-1, -1));
			}
			((SystemWindow)this).set_Maximized(ApplicationSettings.Instance.get("MainWindowMaximized") == "true");
		}

		public void TakePhoto(string imageFileName)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Expected O, but got Unknown
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			ImageBuffer val = new ImageBuffer(640, 480);
			Graphics2D obj = val.NewGraphics2D();
			obj.Clear((IColorType)(object)RGBA_Bytes.White);
			obj.DrawString("No Camera Detected", 320.0, 240.0, 24.0, (Justification)1, (Baseline)3, default(RGBA_Bytes), false, default(RGBA_Bytes));
			obj.DrawString(DateTime.Now.ToString(), 320.0, 200.0, 12.0, (Justification)1, (Baseline)3, default(RGBA_Bytes), false, default(RGBA_Bytes));
			ImageIO.SaveImageData(imageFileName, (IImageByte)(object)val);
			this.PictureTaken?.Invoke(null, null);
		}

		public override void OnDragEnter(FileDropEventArgs fileDropEventArgs)
		{
			((GuiWidget)this).OnDragEnter(fileDropEventArgs);
			if (!fileDropEventArgs.get_AcceptDrop())
			{
				foreach (string droppedFile in fileDropEventArgs.DroppedFiles)
				{
					string text = Path.GetExtension(droppedFile)!.ToUpper();
					if ((text != "" && MeshFileIo.ValidFileExtensions().Contains(text)) || text == ".GCODE" || text == ".ZIP")
					{
						fileDropEventArgs.set_AcceptDrop(true);
					}
				}
				dropWasOnChild = false;
			}
			else
			{
				dropWasOnChild = true;
			}
		}

		public override void OnDragOver(FileDropEventArgs fileDropEventArgs)
		{
			((GuiWidget)this).OnDragOver(fileDropEventArgs);
			if (!fileDropEventArgs.get_AcceptDrop())
			{
				foreach (string droppedFile in fileDropEventArgs.DroppedFiles)
				{
					string text = Path.GetExtension(droppedFile)!.ToUpper();
					if ((text != "" && MeshFileIo.ValidFileExtensions().Contains(text)) || text == ".GCODE" || text == ".ZIP")
					{
						fileDropEventArgs.set_AcceptDrop(true);
					}
				}
				dropWasOnChild = false;
			}
			else
			{
				dropWasOnChild = true;
			}
		}

		public override void OnDragDrop(FileDropEventArgs fileDropEventArgs)
		{
			((GuiWidget)this).OnDragDrop(fileDropEventArgs);
			if (!dropWasOnChild)
			{
				QueueDataWidget.DoAddFiles(fileDropEventArgs.DroppedFiles);
			}
		}

		public void ReportException(Exception e, string key = "", string value = "", ReportSeverity2 warningLevel = ReportSeverity2.Warning)
		{
			string text = UserSettings.Instance.get("UpdateFeedType");
			if ((string.IsNullOrEmpty(text) || text != "release" || OemSettings.Instance.WindowTitleExtra == "Experimental") && raygunNotificationCount++ < 15)
			{
				((RaygunClientBase)_raygunClient).Send(e);
			}
		}

		public static MatterControlApplication CreateInstance(int overrideWidth = -1, int overrideHeight = -1)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Expected O, but got Unknown
			int num = 0;
			int num2 = 0;
			if (UserSettings.Instance.IsTouchScreen)
			{
				minSize = new Vector2(800.0, 480.0);
			}
			string text = ApplicationSettings.Instance.get("WindowSize");
			if (text != null && text != "")
			{
				string[] array = text.Split(new char[1]
				{
					','
				});
				num = Math.Max(int.Parse(array[0]), (int)minSize.x + 1);
				num2 = Math.Max(int.Parse(array[1]), (int)minSize.y + 1);
			}
			else
			{
				Point2D desktopSize = OsInformation.get_DesktopSize();
				if (overrideWidth != -1)
				{
					num = overrideWidth;
				}
				else if (num < desktopSize.x)
				{
					num = 1280;
				}
				if (overrideHeight != -1)
				{
					num2 = overrideHeight;
				}
				else if (num2 < desktopSize.y)
				{
					num2 = 720;
				}
			}
			PerformanceTimer val = new PerformanceTimer("Startup", "Total");
			try
			{
				instance = new MatterControlApplication(num, num2);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			return instance;
		}

		[STAThread]
		public static void Main()
		{
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Expected O, but got Unknown
			PerformanceTimer.GetParentWindowFunction = () => (GuiWidget)(object)instance;
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			ChangeToCurrentDirectory();
			Datastore.Instance.Initialize();
			string text = UserSettings.Instance.get("UpdateFeedType");
			if (string.IsNullOrEmpty(text) || text != "release" || OemSettings.Instance.WindowTitleExtra == "Experimental")
			{
				Application.add_ThreadException(new ThreadExceptionEventHandler(Application_ThreadException));
				AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			}
			_ = Instance;
		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			if (raygunNotificationCount++ < 15)
			{
				((RaygunClientBase)_raygunClient).Send(e.get_Exception());
			}
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (raygunNotificationCount++ < 15)
			{
				((RaygunClientBase)_raygunClient).Send(e.ExceptionObject as Exception);
			}
		}

		public static void WriteTestGCodeFile()
		{
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			using StreamWriter streamWriter = new StreamWriter("PerformanceTest.gcode");
			int num = 150;
			int num2 = 200;
			double num3 = 50.0;
			Vector2 val = default(Vector2);
			((Vector2)(ref val))._002Ector(150.0, 100.0);
			streamWriter.WriteLine("G28 ; home all axes");
			streamWriter.WriteLine("G90 ; use absolute coordinates");
			streamWriter.WriteLine("G21 ; set units to millimeters");
			streamWriter.WriteLine("G92 E0");
			streamWriter.WriteLine("G1 F7800");
			streamWriter.WriteLine("G1 Z" + 5);
			WriteMove(streamWriter, val);
			Vector2 val2 = default(Vector2);
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					((Vector2)(ref val2))._002Ector(num3, 0.0);
					((Vector2)(ref val2)).Rotate(6.2831854820251465 / (double)num2 * (double)j);
					WriteMove(streamWriter, val + val2);
				}
			}
			streamWriter.WriteLine("M84     ; disable motors");
		}

		public void LaunchBrowser(string targetUri)
		{
			UiThread.RunOnIdle((Action)delegate
			{
				Process.Start(targetUri);
			});
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Expected O, but got Unknown
			UserSettings.Instance.Fields.StartCountDurringExit = UserSettings.Instance.Fields.StartCount;
			TerminalWindow.CloseIfOpen();
			if (PrinterConnectionAndCommunication.Instance.CommunicationState != PrinterConnectionAndCommunication.CommunicationStates.PrintingFromSd)
			{
				PrinterConnectionAndCommunication.Instance.Disable();
			}
			PrinterConnectionAndCommunication.Instance.HaltConnectionThread();
			SlicingQueue.Instance.ShutDownSlicingThread();
			ApplicationController.Instance.OnApplicationClosed();
			Datastore.Instance.Exit();
			if (RestartOnClose)
			{
				string location = Assembly.GetExecutingAssembly().Location;
				string directoryName = Path.GetDirectoryName(location);
				ProcessStartInfo val = new ProcessStartInfo();
				val.set_Arguments(StringHelper.FormatWith("\"{0}\" \"{1}\"", new object[2]
				{
					location,
					1000
				}));
				val.set_FileName(Path.Combine(directoryName, "Launcher.exe"));
				val.set_WindowStyle((ProcessWindowStyle)1);
				val.set_CreateNoWindow(true);
				Process.Start(val);
			}
			((SystemWindow)this).OnClosed(e);
		}

		public override void OnClosing(out bool cancelClose)
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			cancelClose = false;
			ApplicationSettings.Instance.set("MainWindowMaximized", ((SystemWindow)this).get_Maximized().ToString().ToLower());
			if (!((SystemWindow)this).get_Maximized())
			{
				ApplicationSettings.Instance.set("WindowSize", $"{((GuiWidget)this).get_Width()},{((GuiWidget)this).get_Height()}");
				ApplicationSettings.Instance.set("DesktopPosition", $"{((SystemWindow)this).get_DesktopPosition().x},{((SystemWindow)this).get_DesktopPosition().y}");
			}
			QueueData.Instance.SaveDefaultQueue();
			if (closeMessageBoxIsOpen)
			{
				cancelClose = true;
			}
			if (!closeHasBeenConfirmed && !closeMessageBoxIsOpen && PrinterConnectionAndCommunication.Instance.PrinterIsPrinting)
			{
				cancelClose = true;
				closeMessageBoxIsOpen = true;
				if (PrinterConnectionAndCommunication.Instance.CommunicationState != PrinterConnectionAndCommunication.CommunicationStates.PrintingFromSd)
				{
					StyledMessageBox.ShowMessageBox(ConditionalyCloseNow, "Are you sure you want to abort the current print and close Element?".Localize(), "Abort Print".Localize(), StyledMessageBox.MessageType.YES_NO);
				}
				else
				{
					StyledMessageBox.ShowMessageBox(ConditionalyCloseNow, "Are you sure you want exit while a print is running from SD Card?\n\nNote: If you exit, it is recommended you wait until the print is completed before running Element again.".Localize(), "Exit while printing".Localize(), StyledMessageBox.MessageType.YES_NO);
				}
			}
			else if (PartsSheet.IsSaving())
			{
				StyledMessageBox.ShowMessageBox(onConfirmExit, savePartsSheetExitAnywayMessage, confirmExit, StyledMessageBox.MessageType.YES_NO);
				cancelClose = true;
			}
			else if (!cancelClose)
			{
				((GuiWidget)this).OnClosing(ref cancelClose);
			}
		}

		private void ConditionalyCloseNow(bool continueWithShutdown)
		{
			closeMessageBoxIsOpen = false;
			if (continueWithShutdown)
			{
				closeHasBeenConfirmed = true;
				if (PrinterConnectionAndCommunication.Instance.CommunicationState != PrinterConnectionAndCommunication.CommunicationStates.PrintingFromSd && (PrinterConnectionAndCommunication.Instance.CommunicationState != PrinterConnectionAndCommunication.CommunicationStates.Paused || PrinterConnectionAndCommunication.Instance.PrePauseCommunicationState != PrinterConnectionAndCommunication.CommunicationStates.PrintingFromSd))
				{
					PrinterConnectionAndCommunication.Instance.Disable();
				}
				MatterControlApplication matterControlApplication = Instance;
				matterControlApplication.RestartOnClose = false;
				((GuiWidget)matterControlApplication).Close();
			}
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			totalDrawTime.Restart();
			GuiWidget.DrawCount = 0;
			PerformanceTimer val = new PerformanceTimer("Draw Timer", "MC Draw");
			try
			{
				((GuiWidget)this).OnDraw(graphics2D);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			totalDrawTime.Stop();
			millisecondTimer.Update((int)totalDrawTime.get_ElapsedMilliseconds());
			if (ShowMemoryUsed)
			{
				long totalMemory = GC.GetTotalMemory(forceFullCollection: false);
				((SystemWindow)this).set_Title(StringHelper.FormatWith("Allocated = {0:n0} : {1:000}ms, d{2} Size = {3}x{4}, onIdle = {5:00}:{6:00}, widgetsDrawn = {7}", new object[8]
				{
					totalMemory,
					millisecondTimer.GetAverage(),
					drawCount++,
					((GuiWidget)this).get_Width(),
					((GuiWidget)this).get_Height(),
					UiThread.get_CountExpired(),
					UiThread.get_Count(),
					GuiWidget.DrawCount
				}));
				if (DoCGCollectEveryDraw)
				{
					GC.Collect();
				}
			}
		}

		public override void OnLoad(EventArgs args)
		{
			string[] array = commandLineArgs;
			foreach (string path in array)
			{
				string text = Path.GetExtension(path)!.ToUpper();
				if (text.Length > 1 && MeshFileIo.ValidFileExtensions().Contains(text))
				{
					QueueData.Instance.AddItem(new PrintItemWrapper(new PrintItem(Path.GetFileName(path), Path.GetFullPath(path))));
				}
			}
			TerminalWindow.ShowIfLeftOpen();
			ApplicationController.Instance.OnLoadActions();
			IsLoading = false;
		}

		private static void HtmlWindowTest()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Expected O, but got Unknown
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				SystemWindow htmlTestWindow = new SystemWindow(640.0, 480.0);
				string text = "";
				text = File.ReadAllText(Path.Combine("C:\\Users\\lbrubaker\\Downloads", "test1.html"));
				HtmlWidget htmlWidget = new HtmlWidget(text, RGBA_Bytes.Black);
				GuiWidget val = new GuiWidget();
				val.set_HAnchor((HAnchor)0);
				val.set_VAnchor((VAnchor)5);
				((GuiWidget)htmlWidget).AddChild(val, -1);
				((GuiWidget)htmlWidget).set_VAnchor((VAnchor)(((GuiWidget)htmlWidget).get_VAnchor() | 4));
				((GuiWidget)htmlWidget).set_BackgroundColor(RGBA_Bytes.White);
				((GuiWidget)htmlTestWindow).AddChild((GuiWidget)(object)htmlWidget, -1);
				((GuiWidget)htmlTestWindow).set_BackgroundColor(RGBA_Bytes.Cyan);
				UiThread.RunOnIdle((Action<object>)delegate
				{
					htmlTestWindow.ShowAsSystemWindow();
				}, (object)1, 0.0);
			}
			catch
			{
			}
		}

		public override void OnMouseMove(MouseEventArgs mouseEvent)
		{
			if (GuiWidget.DebugBoundsUnderMouse)
			{
				((GuiWidget)this).Invalidate();
			}
			((SystemWindow)this).OnMouseMove(mouseEvent);
		}

		public override void OnParentChanged(EventArgs e)
		{
			File.Exists("RunUnitTests.txt");
			((GuiWidget)this).OnParentChanged(e);
			FindAndInstantiatePlugins();
			if (ApplicationController.Instance.PluginsLoaded != null)
			{
				ApplicationController.Instance.PluginsLoaded.CallEvents((object)null, (EventArgs)null);
			}
		}

		public void OpenCameraPreview()
		{
			_ = ApplicationSettings.Instance.get("HardwareHasCamera") == "true";
		}

		public void PlaySound(string fileName)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			if ((int)OsInformation.get_OperatingSystem() == 1)
			{
				using Stream stream = StaticData.get_Instance().OpenSteam(Path.Combine("Sounds", fileName));
				new SoundPlayer(stream).Play();
			}
		}

		private static void WriteMove(StreamWriter file, Vector2 center)
		{
			file.WriteLine("G1 X" + center.x + " Y" + center.y);
		}

		private void CheckOnPrinter()
		{
			try
			{
				PrinterConnectionAndCommunication.Instance.OnIdle();
			}
			catch (Exception)
			{
			}
			UiThread.RunOnIdle((Action)CheckOnPrinter);
		}

		private void FindAndInstantiatePlugins()
		{
			PluginFinder<MatterControlPlugin> obj = new PluginFinder<MatterControlPlugin>((string)null, (IComparer<MatterControlPlugin>)null);
			string oEMName = ApplicationSettings.Instance.GetOEMName();
			foreach (MatterControlPlugin plugin in obj.Plugins)
			{
				Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(plugin.GetPluginInfoJSon());
				if (dictionary != null && dictionary.ContainsKey("OEM"))
				{
					if (dictionary["OEM"] == oEMName)
					{
						plugin.Initialize((GuiWidget)(object)this);
					}
				}
				else
				{
					plugin.Initialize((GuiWidget)(object)this);
				}
			}
		}

		private void onConfirmExit(bool messageBoxResponse)
		{
			if (messageBoxResponse)
			{
				bool flag = default(bool);
				((GuiWidget)this).OnClosing(ref flag);
			}
		}

		private static void AssertDebugNotDefined()
		{
		}

		public override void OnKeyDown(KeyEventArgs keyEvent)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Invalid comparison between Unknown and I4
			if ((int)keyEvent.get_KeyCode() == 112)
			{
				showNamesUnderMouse = !showNamesUnderMouse;
			}
			((GuiWidget)this).OnKeyDown(keyEvent);
		}

		public static void CheckKnownAssemblyConditionalCompSymbols()
		{
			AssertDebugNotDefined();
			GCodeFile.AssertDebugNotDefined();
			Graphics2D.AssertDebugNotDefined();
			SystemWindow.AssertDebugNotDefined();
			InvertLightness.AssertDebugNotDefined();
			TranslationMap.AssertDebugNotDefined();
			MarchingSquaresByte.AssertDebugNotDefined();
			MatterControlPlugin.AssertDebugNotDefined();
			MatterSlice.AssertDebugNotDefined();
			MeshViewerWidget.AssertDebugNotDefined();
			GLMeshTrianglePlugin.AssertDebugNotDefined();
		}

		public bool IsNetworkConnected()
		{
			return true;
		}
	}
}
