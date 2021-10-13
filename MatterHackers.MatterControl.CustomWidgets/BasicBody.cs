using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.ImageProcessing;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.CustomWidgets
{
	public class BasicBody : GuiWidget
	{
		private TextWidget partName;

		private TextWidget printerName;

		private ProgressDial progressDial;

		private TextWidget timeWidget;

		private List<ExtruderStatusWidget> extruderStatusWidgets;

		private void CheckOnPrinter()
		{
			if (!((GuiWidget)this).get_HasBeenClosed())
			{
				GetProgressInfo();
				PrinterConnectionAndCommunication.CommunicationStates communicationState = PrinterConnectionAndCommunication.Instance.CommunicationState;
				if ((uint)(communicationState - 4) > 1u && communicationState != PrinterConnectionAndCommunication.CommunicationStates.Paused)
				{
					((GuiWidget)this).CloseOnIdle();
				}
				UiThread.RunOnIdle((Action)CheckOnPrinter, 1.0);
			}
		}

		private void GetProgressInfo()
		{
			int secondsPrinted = PrinterConnectionAndCommunication.Instance.SecondsPrinted;
			int num = secondsPrinted / 3600;
			int num2 = secondsPrinted / 60 - num * 60;
			secondsPrinted %= 60;
			((GuiWidget)timeWidget).set_Text((num <= 0) ? $"{num2}:{secondsPrinted:00}" : $"{num}:{num2:00}:{secondsPrinted:00}");
			progressDial.LayerCount = PrinterConnectionAndCommunication.Instance.CurrentlyPrintingLayer;
			progressDial.LayerCompletedRatio = PrinterConnectionAndCommunication.Instance.RatioIntoCurrentLayer;
			progressDial.CompletedRatio = PrinterConnectionAndCommunication.Instance.PercentComplete / 100.0;
		}

		public override void OnLoad(EventArgs args)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Expected O, but got Unknown
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Expected O, but got Unknown
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Expected O, but got Unknown
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Expected O, but got Unknown
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Expected O, but got Unknown
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Expected O, but got Unknown
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Expected O, but got Unknown
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Expected O, but got Unknown
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_047d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Expected O, but got Unknown
			//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0553: Unknown result type (might be due to invalid IL or missing references)
			//IL_055e: Expected O, but got Unknown
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ca: Expected O, but got Unknown
			//IL_060d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_066b: Expected O, but got Unknown
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_067c: Expected O, but got Unknown
			//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).OnLoad(args);
			bool flag = ((GuiWidget)this).get_Parent().get_Width() <= 1180.0;
			((GuiWidget)this).set_Padding(flag ? new BorderDouble(20.0, 5.0) : new BorderDouble(50.0, 30.0));
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			FlowLayoutWidget val2 = val;
			((GuiWidget)this).AddChild((GuiWidget)(object)val2, -1);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_VAnchor((VAnchor)5);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_Margin(flag ? new BorderDouble(30.0, 5.0, 30.0, 0.0) : new BorderDouble(30.0, 20.0, 30.0, 0.0));
			FlowLayoutWidget val4 = val3;
			((GuiWidget)val2).AddChild((GuiWidget)(object)val4, -1);
			int num = (flag ? 300 : 500);
			ImageBuffer val5 = PartThumbnailWidget.GetImageForItem(PrinterConnectionAndCommunication.Instance.ActivePrintItem, num, num);
			if (val5 == (ImageBuffer)null)
			{
				val5 = StaticData.get_Instance().LoadImage(Path.Combine("Images", "Screensaver", "part_thumbnail.png"));
			}
			WhiteToColor.DoWhiteToColor(val5, ActiveTheme.get_Instance().get_PrimaryAccentColor());
			ImageWidget val6 = new ImageWidget(val5);
			((GuiWidget)val6).set_VAnchor((VAnchor)2);
			((GuiWidget)val6).set_Margin(flag ? new BorderDouble(0.0, 0.0, 20.0, 0.0) : new BorderDouble(0.0, 0.0, 50.0, 0.0));
			ImageWidget val7 = val6;
			((GuiWidget)val4).AddChild((GuiWidget)(object)val7, -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)PrintingWindow.CreateVerticalLine(), -1);
			HorizontalSpacer horizontalSpacer = new HorizontalSpacer();
			((GuiWidget)horizontalSpacer).set_VAnchor((VAnchor)10);
			HorizontalSpacer horizontalSpacer2 = horizontalSpacer;
			((GuiWidget)val4).AddChild((GuiWidget)(object)horizontalSpacer2, -1);
			FlowLayoutWidget val8 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val8).set_Margin(new BorderDouble(50.0, 0.0));
			((GuiWidget)val8).set_VAnchor((VAnchor)10);
			((GuiWidget)val8).set_HAnchor((HAnchor)10);
			FlowLayoutWidget val9 = val8;
			((GuiWidget)horizontalSpacer2).AddChild((GuiWidget)(object)val9, -1);
			ProgressDial obj = new ProgressDial();
			((GuiWidget)obj).set_HAnchor((HAnchor)2);
			((GuiWidget)obj).set_Height(200.0 * GuiWidget.get_DeviceScale());
			((GuiWidget)obj).set_Width(200.0 * GuiWidget.get_DeviceScale());
			progressDial = obj;
			((GuiWidget)val9).AddChild((GuiWidget)(object)progressDial, -1);
			FlowLayoutWidget val10 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val10).set_HAnchor((HAnchor)10);
			((GuiWidget)val10).set_Margin(BorderDouble.op_Implicit(3));
			FlowLayoutWidget val11 = val10;
			((GuiWidget)val9).AddChild((GuiWidget)(object)val11, -1);
			ImageBuffer val12 = StaticData.get_Instance().LoadImage(Path.Combine("Images", "Screensaver", "time.png"));
			if (!ActiveTheme.get_Instance().get_IsDarkTheme())
			{
				ExtensionMethods.InvertLightness(val12);
			}
			((GuiWidget)val11).AddChild((GuiWidget)new ImageWidget(val12), -1);
			TextWidget val13 = new TextWidget("", 0.0, 0.0, 22.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			val13.set_AutoExpandBoundsToText(true);
			((GuiWidget)val13).set_Margin(new BorderDouble(10.0, 0.0, 0.0, 0.0));
			((GuiWidget)val13).set_VAnchor((VAnchor)2);
			timeWidget = val13;
			((GuiWidget)val11).AddChild((GuiWidget)(object)timeWidget, -1);
			int num2 = 350;
			TextWidget val14 = new TextWidget(ActiveSliceSettings.Instance.GetValue("printer_name"), 0.0, 0.0, 16.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val14).set_HAnchor((HAnchor)2);
			((GuiWidget)val14).set_MinimumSize(new Vector2((double)num2, ((GuiWidget)this).get_MinimumSize().y));
			((GuiWidget)val14).set_Width((double)num2);
			((GuiWidget)val14).set_Margin(new BorderDouble(0.0, 3.0));
			printerName = val14;
			((GuiWidget)val9).AddChild((GuiWidget)(object)printerName, -1);
			TextWidget val15 = new TextWidget(PrinterConnectionAndCommunication.Instance.ActivePrintItem.GetFriendlyName(), 0.0, 0.0, 16.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val15).set_HAnchor((HAnchor)2);
			((GuiWidget)val15).set_MinimumSize(new Vector2((double)num2, ((GuiWidget)this).get_MinimumSize().y));
			((GuiWidget)val15).set_Width((double)num2);
			((GuiWidget)val15).set_Margin(new BorderDouble(0.0, 3.0));
			partName = val15;
			((GuiWidget)val9).AddChild((GuiWidget)(object)partName, -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)PrintingWindow.CreateVerticalLine(), -1);
			ZAxisControls zAxisControls = new ZAxisControls(flag);
			((GuiWidget)zAxisControls).set_Margin(new BorderDouble(50.0, 0.0, 0.0, 0.0));
			((GuiWidget)zAxisControls).set_VAnchor((VAnchor)2);
			((GuiWidget)zAxisControls).set_Width(135.0);
			ZAxisControls zAxisControls2 = zAxisControls;
			((GuiWidget)val4).AddChild((GuiWidget)(object)zAxisControls2, -1);
			FlowLayoutWidget val16 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val16).set_VAnchor((VAnchor)9);
			((GuiWidget)val16).set_HAnchor((HAnchor)10);
			((GuiWidget)val16).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 0.0));
			FlowLayoutWidget val17 = val16;
			((GuiWidget)val2).AddChild((GuiWidget)(object)val17, -1);
			int value = ActiveSliceSettings.Instance.GetValue<int>("extruder_count");
			extruderStatusWidgets = Enumerable.ToList<ExtruderStatusWidget>(Enumerable.Select<int, ExtruderStatusWidget>(Enumerable.Range(0, value), (Func<int, ExtruderStatusWidget>)((int i) => new ExtruderStatusWidget(i))));
			if (ActiveSliceSettings.Instance.GetValue<bool>("has_heated_bed"))
			{
				FlowLayoutWidget val18 = new FlowLayoutWidget((FlowDirection)3);
				((GuiWidget)val17).AddChild((GuiWidget)(object)val18, -1);
				for (int j = 0; j < value; j++)
				{
					ExtruderStatusWidget extruderStatusWidget = extruderStatusWidgets[j];
					((GuiWidget)extruderStatusWidget).set_Margin(new BorderDouble(0.0, 0.0, 20.0, 0.0));
					((GuiWidget)val18).AddChild((GuiWidget)(object)extruderStatusWidget, -1);
				}
				BedStatusWidget bedStatusWidget = new BedStatusWidget(flag);
				((GuiWidget)bedStatusWidget).set_VAnchor((VAnchor)2);
				((GuiWidget)val17).AddChild((GuiWidget)(object)bedStatusWidget, -1);
			}
			else if (value == 1)
			{
				((GuiWidget)val17).AddChild((GuiWidget)(object)extruderStatusWidgets[0], -1);
			}
			else
			{
				FlowLayoutWidget val19 = new FlowLayoutWidget((FlowDirection)3);
				((GuiWidget)val17).AddChild((GuiWidget)(object)val19, -1);
				FlowLayoutWidget val20 = new FlowLayoutWidget((FlowDirection)3);
				((GuiWidget)val17).AddChild((GuiWidget)(object)val20, -1);
				for (int k = 0; k < value; k++)
				{
					ExtruderStatusWidget extruderStatusWidget2 = extruderStatusWidgets[k];
					if (k % 2 == 0)
					{
						((GuiWidget)extruderStatusWidget2).set_Margin(new BorderDouble(0.0, 0.0, 20.0, 0.0));
						((GuiWidget)val19).AddChild((GuiWidget)(object)extruderStatusWidget2, -1);
					}
					else
					{
						((GuiWidget)val20).AddChild((GuiWidget)(object)extruderStatusWidget2, -1);
					}
				}
			}
			UiThread.RunOnIdle((Action)delegate
			{
				CheckOnPrinter();
			});
		}

		public BasicBody()
			: this()
		{
			((GuiWidget)this).set_VAnchor((VAnchor)5);
			((GuiWidget)this).set_HAnchor((HAnchor)5);
		}
	}
}
