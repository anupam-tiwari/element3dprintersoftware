using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrinterControls
{
	public class RunningMacroPage : WizardPage
	{
		public class MacroCommandData
		{
			public bool waitOk;

			public string title = "";

			public bool showMaterialSelector;

			public double countDown;

			public double expireTime;

			public double expectedTemperature;

			public ImageBuffer image;
		}

		private long startTimeMs;

		private ProgressBar progressBar;

		private TextWidget progressBarText;

		private long timeToWaitMs;

		private EventHandler unregisterEvents;

		private double startingTemp;

		public RunningMacroPage(MacroCommandData macroData)
			: base("Cancel", macroData.title)
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Expected O, but got Unknown
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Expected O, but got Unknown
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Expected O, but got Unknown
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Expected O, but got Unknown
			((GuiWidget)cancelButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				PrinterConnectionAndCommunication.Instance.MacroCancel();
			});
			if (macroData.showMaterialSelector)
			{
				int num = 0;
				PresetSelectorWidget presetSelectorWidget = new PresetSelectorWidget(string.Format(string.Format("{0} {1}", "Material".Localize(), num + 1)), RGBA_Bytes.Transparent, NamedSettingsLayers.Material, num);
				((GuiWidget)presetSelectorWidget).set_BackgroundColor(RGBA_Bytes.Transparent);
				((GuiWidget)presetSelectorWidget).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 15.0));
				((GuiWidget)contentRow).AddChild((GuiWidget)(object)presetSelectorWidget, -1);
			}
			PrinterConnectionAndCommunication.Instance.WroteLine.RegisterEvent((EventHandler)LookForTempRequest, ref unregisterEvents);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)cancelButton, -1);
			if (macroData.waitOk | (macroData.expireTime > 0.0))
			{
				Button val = textImageButtonFactory.Generate("Continue".Localize());
				((GuiWidget)val).add_Click((EventHandler<MouseEventArgs>)delegate
				{
					PrinterConnectionAndCommunication.Instance.MacroContinue();
					UiThread.RunOnIdle((Action)delegate
					{
						WizardWindow wizardWindow = WizardWindow;
						if (wizardWindow != null)
						{
							((GuiWidget)wizardWindow).Close();
						}
					});
				});
				((GuiWidget)footerRow).AddChild((GuiWidget)(object)val, -1);
			}
			if (macroData.image != (ImageBuffer)null)
			{
				ImageWidget val2 = new ImageWidget(macroData.image);
				((GuiWidget)val2).set_HAnchor((HAnchor)2);
				((GuiWidget)val2).set_Margin(new BorderDouble(5.0, 15.0));
				ImageWidget val3 = val2;
				((GuiWidget)contentRow).AddChild((GuiWidget)(object)val3, -1);
			}
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)0);
			ProgressBar val5 = new ProgressBar((int)(150.0 * GuiWidget.get_DeviceScale()), (int)(15.0 * GuiWidget.get_DeviceScale()));
			val5.set_FillColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			val5.set_BorderColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val5).set_BackgroundColor(RGBA_Bytes.White);
			((GuiWidget)val5).set_Margin(new BorderDouble(3.0, 0.0, 0.0, 10.0));
			progressBar = val5;
			TextWidget val6 = new TextWidget("", 0.0, 0.0, 10.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			val6.set_AutoExpandBoundsToText(true);
			((GuiWidget)val6).set_Margin(new BorderDouble(5.0, 0.0, 0.0, 0.0));
			progressBarText = val6;
			((GuiWidget)val4).AddChild((GuiWidget)(object)progressBar, -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)progressBarText, -1);
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)progressBar).set_Visible(false);
			if (macroData.countDown > 0.0)
			{
				timeToWaitMs = (long)(macroData.countDown * 1000.0);
				startTimeMs = UiThread.get_CurrentTimerMs();
				UiThread.RunOnIdle((Action)CountDownTime);
			}
		}

		public static void Show(MacroCommandData macroData)
		{
			WizardWindow.Show("Macro", "Running Macro", new RunningMacroPage(macroData));
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (e.get_OsEvent())
			{
				PrinterConnectionAndCommunication.Instance.MacroCancel();
			}
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		private void CountDownTime()
		{
			((GuiWidget)progressBar).set_Visible(true);
			long num = UiThread.get_CurrentTimerMs() - startTimeMs;
			progressBar.set_RatioComplete((timeToWaitMs == 0L) ? 1.0 : Math.Max(0.0, Math.Min(1.0, (double)num / (double)timeToWaitMs)));
			int num2 = (int)(((double)timeToWaitMs - (double)timeToWaitMs * progressBar.get_RatioComplete()) / 1000.0);
			((GuiWidget)progressBarText).set_Text($"Time Remaining: {num2 / 60:#0}:{num2 % 60:00}");
			if (!((GuiWidget)this).get_HasBeenClosed() && progressBar.get_RatioComplete() < 1.0)
			{
				UiThread.RunOnIdle((Action)CountDownTime, 0.2);
			}
		}

		private void LookForTempRequest(object sender, EventArgs e)
		{
			StringEventArgs val = e as StringEventArgs;
			if (val != null && val.get_Data().Contains("M104"))
			{
				startingTemp = PrinterConnectionAndCommunication.Instance.GetActualExtruderTemperature(0);
				UiThread.RunOnIdle((Action)ShowTempChangeProgress);
			}
		}

		private void ShowTempChangeProgress()
		{
			((GuiWidget)progressBar).set_Visible(true);
			double targetExtruderTemperature = PrinterConnectionAndCommunication.Instance.GetTargetExtruderTemperature(0);
			double actualExtruderTemperature = PrinterConnectionAndCommunication.Instance.GetActualExtruderTemperature(0);
			double num = targetExtruderTemperature - startingTemp;
			double num2 = actualExtruderTemperature - startingTemp;
			double num3 = ((num != 0.0) ? (num2 / num) : 1.0);
			progressBar.set_RatioComplete(Math.Min(Math.Max(0.0, num3), 1.0));
			((GuiWidget)progressBarText).set_Text($"Temperature: {actualExtruderTemperature:0} / {targetExtruderTemperature:0}");
			if (!((GuiWidget)this).get_HasBeenClosed() && num3 < 1.0)
			{
				UiThread.RunOnIdle((Action)ShowTempChangeProgress, 1.0);
			}
		}
	}
}
