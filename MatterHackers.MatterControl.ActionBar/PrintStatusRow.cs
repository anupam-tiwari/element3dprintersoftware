using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.ActionBar
{
	public class PrintStatusRow : FlowLayoutWidget
	{
		private TextWidget activePrintLabel;

		private TextWidget activePrintName;

		private PartThumbnailWidget activePrintPreviewImage;

		private TextWidget activePrintStatus;

		private TemperatureWidgetBase bedTemperatureWidget;

		private QueueDataView queueDataView;

		private EventHandler unregisterEvents;

		public PrintStatusRow(QueueDataView queueDataView)
			: this((FlowDirection)0)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			UiThread.RunOnIdle((Action)OnIdle);
			((GuiWidget)this).set_Margin(new BorderDouble(6.0, 3.0, 6.0, 6.0));
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			this.queueDataView = queueDataView;
			AddChildElements();
			PrinterConnectionAndCommunication.Instance.ActivePrintItemChanged.RegisterEvent((EventHandler)delegate
			{
				UpdatePrintItemName();
				UpdatePrintStatus();
			}, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)delegate
			{
				UpdatePrintStatus();
			}, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.WroteLine.RegisterEvent((EventHandler)delegate
			{
				UpdatePrintStatus();
			}, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.ActivePrintItemChanged.RegisterEvent((EventHandler)onActivePrintItemChanged, ref unregisterEvents);
			onActivePrintItemChanged(null, null);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (activePrintPreviewImage.ItemWrapper != null)
			{
				activePrintPreviewImage.ItemWrapper.SlicingOutputMessage -= PrintItem_SlicingOutputMessage;
			}
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		private void AddChildElements()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Expected O, but got Unknown
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Expected O, but got Unknown
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			activePrintPreviewImage = new PartThumbnailWidget(null, "part_icon_transparent_100x100.png", "building_thumbnail_100x100.png", PartThumbnailWidget.ImageSizes.Size115x115);
			((GuiWidget)activePrintPreviewImage).set_VAnchor((VAnchor)4);
			((GuiWidget)activePrintPreviewImage).set_Padding(new BorderDouble(0.0));
			activePrintPreviewImage.HoverBackgroundColor = default(RGBA_Bytes);
			activePrintPreviewImage.BorderWidth = 3.0;
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			bedTemperatureWidget = new TemperatureWidgetBed();
			if (ActiveSliceSettings.Instance.GetValue<bool>("has_heated_bed"))
			{
				((GuiWidget)val).AddChild((GuiWidget)(object)bedTemperatureWidget, -1);
			}
			((GuiWidget)val).set_VAnchor((VAnchor)(((GuiWidget)val).get_VAnchor() | 4));
			((GuiWidget)val).set_Margin(new BorderDouble(6.0, 0.0, 0.0, 0.0));
			FlowLayoutWidget val2 = CreateActivePrinterInfoWidget();
			((GuiWidget)val2).set_VAnchor((VAnchor)(((GuiWidget)val2).get_VAnchor() | 4));
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val3).set_Name("PrintStatusRow.IconContainer");
			((GuiWidget)val3).set_VAnchor((VAnchor)(((GuiWidget)val3).get_VAnchor() | 4));
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 3.0));
			((GuiWidget)val3).AddChild((GuiWidget)(object)GetAutoLevelIndicator(), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)activePrintPreviewImage, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			UpdatePrintStatus();
			UpdatePrintItemName();
		}

		private FlowLayoutWidget CreateActivePrinterInfoWidget()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Expected O, but got Unknown
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_Margin(new BorderDouble(6.0, 0.0, 6.0, 0.0));
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_VAnchor((VAnchor)(((GuiWidget)val).get_VAnchor() | 4));
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_Name("PrintStatusRow.ActivePrinterInfo.TopRow");
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			activePrintLabel = getPrintStatusLabel("Next Print".Localize() + ":", 11);
			((GuiWidget)activePrintLabel).set_VAnchor((VAnchor)4);
			((GuiWidget)val2).AddChild((GuiWidget)(object)activePrintLabel, -1);
			activePrintName = getPrintStatusLabel("this is the biggest name we will allow", 14);
			activePrintName.set_AutoExpandBoundsToText(false);
			activePrintStatus = getPrintStatusLabel("this is the biggest label we will allow - bigger", 11);
			activePrintStatus.set_AutoExpandBoundsToText(false);
			((GuiWidget)activePrintStatus).set_Text("");
			((GuiWidget)activePrintStatus).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 3.0));
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)activePrintName, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)activePrintStatus, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new PrintActionRow(queueDataView), -1);
			return val;
		}

		private Button GetAutoLevelIndicator()
		{
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			ImageButtonFactory imageButtonFactory = new ImageButtonFactory();
			imageButtonFactory.InvertImageColor = false;
			ImageBuffer val = ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon("leveling_32x32.png", 16, 16));
			Button autoLevelButton = imageButtonFactory.Generate(val, val);
			((GuiWidget)autoLevelButton).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 3.0));
			((GuiWidget)autoLevelButton).set_ToolTipText("Print leveling is enabled.".Localize());
			((GuiWidget)autoLevelButton).set_Cursor((Cursors)3);
			((GuiWidget)autoLevelButton).set_Visible(ActiveSliceSettings.Instance.GetValue<bool>("print_leveling_enabled"));
			PrinterSettings.PrintLevelingEnabledChanged.RegisterEvent((EventHandler)delegate
			{
				((GuiWidget)autoLevelButton).set_Visible(ActiveSliceSettings.Instance.GetValue<bool>("print_leveling_enabled"));
			}, ref unregisterEvents);
			return autoLevelButton;
		}

		private TextWidget getPrintStatusLabel(string text, int pointSize)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			TextWidget val = new TextWidget(text, 0.0, 0.0, (double)pointSize, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val.set_TextColor(RGBA_Bytes.White);
			val.set_AutoExpandBoundsToText(true);
			((GuiWidget)val).set_MinimumSize(new Vector2(((GuiWidget)val).get_Width(), ((GuiWidget)val).get_Height()));
			return val;
		}

		private void onActivePrintItemChanged(object sender, EventArgs e)
		{
			if (activePrintPreviewImage.ItemWrapper != null)
			{
				activePrintPreviewImage.ItemWrapper.SlicingOutputMessage -= PrintItem_SlicingOutputMessage;
			}
			activePrintPreviewImage.ItemWrapper = PrinterConnectionAndCommunication.Instance.ActivePrintItem;
			if (activePrintPreviewImage.ItemWrapper != null)
			{
				activePrintPreviewImage.ItemWrapper.SlicingOutputMessage += PrintItem_SlicingOutputMessage;
			}
			((GuiWidget)activePrintPreviewImage).Invalidate();
		}

		private void OnIdle()
		{
			if (PrinterConnectionAndCommunication.Instance.PrinterIsPrinting)
			{
				UpdatePrintStatus();
			}
			if (!((GuiWidget)this).get_HasBeenClosed())
			{
				UiThread.RunOnIdle((Action)OnIdle, 1.0);
			}
		}

		private void PrintItem_SlicingOutputMessage(object sender, StringEventArgs message)
		{
			((GuiWidget)activePrintStatus).set_Text(message.get_Data());
		}

		private void SetVisibleStatus()
		{
			if (ActiveSliceSettings.Instance.PrinterSelected)
			{
				if (ActiveSliceSettings.Instance.GetValue<bool>("has_heated_bed"))
				{
					((GuiWidget)bedTemperatureWidget).set_Visible(true);
				}
				else
				{
					((GuiWidget)bedTemperatureWidget).set_Visible(false);
				}
			}
		}

		private void UpdatePrintItemName()
		{
			if (PrinterConnectionAndCommunication.Instance.ActivePrintItem != null)
			{
				((GuiWidget)activePrintName).set_Text(PrinterConnectionAndCommunication.Instance.ActivePrintItem.GetFriendlyName());
			}
			else
			{
				((GuiWidget)activePrintName).set_Text("No items in the print queue".Localize());
			}
		}

		private void UpdatePrintStatus()
		{
			string text = "Next Print".Localize() + ":";
			string text2 = ((GuiWidget)activePrintStatus).get_Text();
			if (PrinterConnectionAndCommunication.Instance.ActivePrintItem != null)
			{
				int totalSecondsInPrint = PrinterConnectionAndCommunication.Instance.TotalSecondsInPrint;
				int num = totalSecondsInPrint / 3600;
				int num2 = totalSecondsInPrint / 60 - num * 60;
				totalSecondsInPrint %= 60;
				string text3 = "Estimated Print Time".Localize();
				string arg = "Calculating...".Localize();
				string text4 = ((totalSecondsInPrint > 0) ? ((num <= 0) ? $"{text3}: {num2}m {totalSecondsInPrint:00}s" : $"{text3}: {num}h {num2:00}m {totalSecondsInPrint:00}s") : ((totalSecondsInPrint >= 0) ? $"{text3}: {arg}" : "Streaming GCode...".Localize()));
				switch (PrinterConnectionAndCommunication.Instance.CommunicationState)
				{
				case PrinterConnectionAndCommunication.CommunicationStates.PreparingToPrint:
					text = "Preparing To Print".Localize() + ":";
					break;
				case PrinterConnectionAndCommunication.CommunicationStates.Printing:
					text = PrinterConnectionAndCommunication.Instance.PrintingStateString;
					text2 = text4;
					break;
				case PrinterConnectionAndCommunication.CommunicationStates.Paused:
					text = "Printing Paused".Localize() + ":";
					text2 = text4;
					break;
				case PrinterConnectionAndCommunication.CommunicationStates.FinishedPrint:
					text = "Done Printing".Localize() + ":";
					text2 = text4;
					break;
				case PrinterConnectionAndCommunication.CommunicationStates.Disconnected:
					text2 = "Not connected. Press 'Connect' to enable printing.".Localize();
					break;
				case PrinterConnectionAndCommunication.CommunicationStates.AttemptingToConnect:
					text2 = "Attempting to Connect".Localize() + "...";
					break;
				case PrinterConnectionAndCommunication.CommunicationStates.FailedToConnect:
				case PrinterConnectionAndCommunication.CommunicationStates.ConnectionLost:
					text2 = "Connection Failed".Localize() + ": " + PrinterConnectionAndCommunication.Instance.ConnectionFailureMessage;
					break;
				default:
					text2 = (ActiveSliceSettings.Instance.PrinterSelected ? "" : "Select a Printer.".Localize());
					break;
				}
			}
			else
			{
				text = "Next Print".Localize() + ":";
				text2 = "Press 'Add' to choose an item to print".Localize();
			}
			((GuiWidget)activePrintLabel).set_Text(text);
			((GuiWidget)activePrintStatus).set_Text(text2);
		}
	}
}
