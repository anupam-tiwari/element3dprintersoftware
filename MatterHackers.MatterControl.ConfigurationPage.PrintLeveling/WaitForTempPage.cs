using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class WaitForTempPage : InstructionsPage
	{
		private ProgressBar progressBar;

		private TextWidget progressBarText;

		private double startingTemp;

		public WaitForTempPage(string pageDescription, string instructionsText)
			: base(pageDescription, instructionsText)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Expected O, but got Unknown
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			ProgressBar val2 = new ProgressBar((int)(150.0 * GuiWidget.get_DeviceScale()), (int)(15.0 * GuiWidget.get_DeviceScale()));
			val2.set_FillColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			val2.set_BorderColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_BackgroundColor(RGBA_Bytes.White);
			((GuiWidget)val2).set_Margin(new BorderDouble(3.0, 0.0, 0.0, 10.0));
			progressBar = val2;
			TextWidget val3 = new TextWidget("", 0.0, 0.0, 10.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			val3.set_AutoExpandBoundsToText(true);
			((GuiWidget)val3).set_Margin(new BorderDouble(5.0, 0.0, 0.0, 0.0));
			progressBarText = val3;
			((GuiWidget)val).AddChild((GuiWidget)(object)progressBar, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)progressBarText, -1);
			((GuiWidget)topToBottomControls).AddChild((GuiWidget)(object)val, -1);
		}

		public override void PageIsBecomingActive()
		{
			startingTemp = PrinterConnectionAndCommunication.Instance.GetActualExtruderTemperature(0);
			UiThread.RunOnIdle((Action)ShowTempChangeProgress);
			PrinterConnectionAndCommunication.Instance.TargetBedTemperature = ActiveSliceSettings.Instance.GetValue<double>("bed_temperature");
			((GuiWidget)this).get_Parent().add_Closed((EventHandler<ClosedEventArgs>)delegate
			{
				PrinterConnectionAndCommunication.Instance.TargetBedTemperature = 0.0;
			});
			base.PageIsBecomingActive();
		}

		private void ShowTempChangeProgress()
		{
			((GuiWidget)progressBar).set_Visible(true);
			double targetBedTemperature = PrinterConnectionAndCommunication.Instance.TargetBedTemperature;
			double actualBedTemperature = PrinterConnectionAndCommunication.Instance.ActualBedTemperature;
			double num = targetBedTemperature - startingTemp;
			double num2 = actualBedTemperature - startingTemp;
			double val = ((num != 0.0) ? (num2 / num) : 1.0);
			progressBar.set_RatioComplete(Math.Min(Math.Max(0.0, val), 1.0));
			((GuiWidget)progressBarText).set_Text($"Temperature: {actualBedTemperature:0} / {targetBedTemperature:0}");
			if (!((GuiWidget)this).get_HasBeenClosed())
			{
				UiThread.RunOnIdle((Action)ShowTempChangeProgress, 1.0);
			}
		}
	}
}
