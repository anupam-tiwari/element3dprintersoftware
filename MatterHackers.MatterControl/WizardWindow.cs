using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.PrinterControls.PrinterConnections;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class WizardWindow : SystemWindow
	{
		private EventHandler unregisterEvents;

		public static Action ShowAuthDialog;

		public static Action ChangeToAccountCreate;

		private static Dictionary<string, WizardWindow> allWindows = new Dictionary<string, WizardWindow>();

		public static Func<bool> ShouldShowAuthPanel
		{
			get;
			set;
		}

		private WizardWindow()
			: this(500.0 * GuiWidget.get_DeviceScale(), 500.0 * GuiWidget.get_DeviceScale())
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			((SystemWindow)this).set_AlwaysOnTopOfMain(true);
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)this).set_Padding(new BorderDouble(8.0));
			((GuiWidget)this).set_MinimumSize(new Vector2(350.0 * GuiWidget.get_DeviceScale(), 400.0 * GuiWidget.get_DeviceScale()));
			((SystemWindow)this).ShowAsSystemWindow();
		}

		public static void Close(string uri)
		{
			if (allWindows.TryGetValue(uri, out var value))
			{
				((GuiWidget)value).Close();
			}
		}

		public static void Show<PanelType>(string uri, string title) where PanelType : WizardPage, new()
		{
			WizardWindow window = GetWindow(uri);
			((SystemWindow)window).set_Title(title);
			window.ChangeToPage<PanelType>();
		}

		public static void Show(string uri, string title, WizardPage wizardPage)
		{
			WizardWindow window = GetWindow(uri);
			((SystemWindow)window).set_Title(title);
			window.ChangeToPage(wizardPage);
		}

		public static void ShowPrinterSetup(bool userRequestedNewPrinter = false)
		{
			WizardWindow window = GetWindow("PrinterSetup");
			((SystemWindow)window).set_Title("Setup Wizard".Localize());
			if (!MatterControlApplication.Instance.IsNetworkConnected())
			{
				window.ChangeToPage<SetupWizardWifi>();
			}
			else
			{
				window.ChangeToSetupPrinterForm(userRequestedNewPrinter);
			}
		}

		public static void ShowComPortSetup()
		{
			WizardWindow window = GetWindow("PrinterSetup");
			((SystemWindow)window).set_Title("Setup Wizard".Localize());
			window.ChangeToPage<SetupStepComPortOne>();
		}

		public static bool IsOpen(string uri)
		{
			if (allWindows.TryGetValue(uri, out var _))
			{
				return true;
			}
			return false;
		}

		private static WizardWindow GetWindow(string uri)
		{
			if (allWindows.TryGetValue(uri, out var value))
			{
				((GuiWidget)value).BringToFront();
			}
			else
			{
				value = new WizardWindow();
				((GuiWidget)value).add_Closed((EventHandler<ClosedEventArgs>)delegate
				{
					allWindows.Remove(uri);
				});
				allWindows[uri] = value;
			}
			return value;
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((SystemWindow)this).OnClosed(e);
		}

		public void ChangeToSetupPrinterForm(bool userRequestedNewPrinter = false)
		{
			if ((ShouldShowAuthPanel?.Invoke() ?? false) && !userRequestedNewPrinter)
			{
				ChangeToPage<ShowAuthPanel>();
			}
			else
			{
				ChangeToPage<SetupStepMakeModelName>();
			}
		}

		internal void ChangeToInstallDriverOrComPortOne()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			if (SetupStepInstallDriver.PrinterDrivers().Count > 0 && (int)OsInformation.get_OperatingSystem() == 1)
			{
				ChangeToPage<SetupStepInstallDriver>();
			}
			else
			{
				ChangeToPage<SetupStepComPortOne>();
			}
		}

		internal void ChangeToSetupBaudOrComPortOne()
		{
			if (string.IsNullOrEmpty(PrinterConnectionAndCommunication.Instance?.ActivePrinter?.GetValue("baud_rate")))
			{
				ChangeToPage<SetupStepBaudRate>();
			}
			else
			{
				ChangeToPage<SetupStepComPortOne>();
			}
		}

		internal void ChangeToPage(WizardPage pageToChangeTo)
		{
			pageToChangeTo.WizardWindow = this;
			((GuiWidget)this).CloseAllChildren();
			((GuiWidget)this).AddChild((GuiWidget)(object)pageToChangeTo, -1);
			((GuiWidget)this).Invalidate();
		}

		internal void ChangeToPage<PanelType>() where PanelType : WizardPage, new()
		{
			PanelType panel = new PanelType();
			ChangeToPage(panel);
			ApplicationController.Instance.DoneReloadingAll.RegisterEvent((EventHandler)delegate
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
				int childIndex = ((GuiWidget)this).GetChildIndex((GuiWidget)(object)panel);
				((GuiWidget)this).RemoveAllChildren();
				PanelType val = new PanelType();
				val.WizardWindow = this;
				((GuiWidget)this).AddChild((GuiWidget)(object)val, childIndex);
				((GuiWidget)panel).CloseOnIdle();
				panel = val;
			}, ref unregisterEvents);
		}
	}
}
