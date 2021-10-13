using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrinterControls.PrinterConnections
{
	public class SetupStepInstallDriver : ConnectionWizardPage
	{
		private static List<string> printerDrivers;

		private FlowLayoutWidget printerDriverContainer;

		private TextWidget printerDriverMessage;

		private Button installButton;

		private Button skipButton;

		public SetupStepInstallDriver()
		{
			((GuiWidget)headerLabel).set_Text(string.Format("Install Communication Driver".Localize()));
			printerDriverContainer = createPrinterDriverContainer();
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)printerDriverContainer, -1);
			installButton = textImageButtonFactory.Generate("Install Driver".Localize());
			((GuiWidget)installButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					if (InstallDriver())
					{
						WizardWindow.ChangeToSetupBaudOrComPortOne();
					}
				});
			});
			skipButton = textImageButtonFactory.Generate("Skip".Localize());
			((GuiWidget)skipButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				WizardWindow.ChangeToSetupBaudOrComPortOne();
			});
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)installButton, -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)skipButton, -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)cancelButton, -1);
		}

		private FlowLayoutWidget createPrinterDriverContainer()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Expected O, but got Unknown
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Expected O, but got Unknown
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 5.0));
			BorderDouble margin = default(BorderDouble);
			((BorderDouble)(ref margin))._002Ector(0.0, 0.0, 0.0, 3.0);
			printerDriverMessage = new TextWidget("This printer requires a driver for communication.".Localize(), 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			printerDriverMessage.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)printerDriverMessage).set_HAnchor((HAnchor)5);
			((GuiWidget)printerDriverMessage).set_Margin(margin);
			TextWidget val2 = new TextWidget("Driver located. Would you like to install?".Localize(), 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Margin(margin);
			((GuiWidget)val).AddChild((GuiWidget)(object)printerDriverMessage, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			return val;
		}

		private void InstallDriver(string fileName)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected I4, but got Unknown
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			OSType operatingSystem = OsInformation.get_OperatingSystem();
			switch (operatingSystem - 1)
			{
			case 0:
				if (File.Exists(fileName))
				{
					if (Path.GetExtension(fileName)!.ToUpper() == ".INF")
					{
						Process val2 = new Process();
						val2.get_StartInfo().set_Arguments(Path.GetFullPath(fileName));
						string fullPath = Path.GetFullPath(Path.Combine(".", "InfInstaller.exe"));
						val2.get_StartInfo().set_CreateNoWindow(true);
						val2.get_StartInfo().set_WindowStyle((ProcessWindowStyle)1);
						val2.get_StartInfo().set_FileName(Path.GetFullPath(fullPath));
						val2.get_StartInfo().set_Verb("runas");
						val2.get_StartInfo().set_UseShellExecute(true);
						val2.Start();
						val2.WaitForExit();
					}
					else
					{
						Process.Start(fileName);
					}
					break;
				}
				throw new Exception($"Can't find driver {fileName}.");
			case 2:
				if (File.Exists(fileName))
				{
					if (Path.GetExtension(fileName)!.ToUpper() == ".INF")
					{
						Process val = new Process();
						val.get_StartInfo().set_Arguments(Path.GetFullPath(fileName));
						string path = Path.Combine(".", "InfInstaller.exe");
						val.get_StartInfo().set_CreateNoWindow(true);
						val.get_StartInfo().set_WindowStyle((ProcessWindowStyle)1);
						val.get_StartInfo().set_FileName(Path.GetFullPath(path));
						val.get_StartInfo().set_Verb("runas");
						val.get_StartInfo().set_UseShellExecute(true);
						val.Start();
						val.WaitForExit();
						val.get_ExitCode();
					}
					else
					{
						Process.Start(fileName);
					}
					break;
				}
				throw new Exception("Can't find driver: " + fileName);
			case 1:
				break;
			}
		}

		public static List<string> PrinterDrivers()
		{
			if (printerDrivers == null)
			{
				printerDrivers = GetPrintDrivers();
			}
			return printerDrivers;
		}

		private static List<string> GetPrintDrivers()
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Invalid comparison between Unknown and I4
			List<string> list = new List<string>();
			string value = ActiveSliceSettings.Instance.GetValue("windows_driver");
			if (!string.IsNullOrEmpty(value))
			{
				string[] array = value.Split(new char[1]
				{
					','
				});
				foreach (string text in array)
				{
					OSType operatingSystem = OsInformation.get_OperatingSystem();
					if ((int)operatingSystem != 1)
					{
						continue;
					}
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
					string text2 = Path.Combine("Drivers", fileNameWithoutExtension);
					string text3 = Path.Combine(text2, text);
					if (!StaticData.get_Instance().FileExists(text3))
					{
						continue;
					}
					string fullPath = Path.GetFullPath(Path.Combine(ApplicationDataStorage.ApplicationUserDataPath, "data", "temp", "inf", fileNameWithoutExtension));
					if (!Directory.Exists(fullPath))
					{
						Directory.CreateDirectory(fullPath);
					}
					string fullPath2 = Path.GetFullPath(Path.Combine(fullPath, text));
					foreach (string file in StaticData.get_Instance().GetFiles(text2))
					{
						using Stream destination = File.OpenWrite(Path.Combine(fullPath, Path.GetFileName(file)));
						using Stream stream = StaticData.get_Instance().OpenSteam(file);
						stream.CopyTo(destination);
					}
					list.Add(fullPath2);
				}
			}
			return list;
		}

		private bool InstallDriver()
		{
			try
			{
				((GuiWidget)printerDriverMessage).set_Text("Installing".Localize() + "...");
				foreach (string item in PrinterDrivers())
				{
					InstallDriver(item);
				}
				return true;
			}
			catch (Exception)
			{
				((GuiWidget)printerDriverMessage).set_Text("Sorry, we were unable to install the driver.".Localize());
				return false;
			}
		}
	}
}
