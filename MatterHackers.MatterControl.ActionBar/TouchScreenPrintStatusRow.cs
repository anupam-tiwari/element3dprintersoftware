using System;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
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
	public class TouchScreenPrintStatusRow : FlowLayoutWidget
	{
		private TextWidget activePrintLabel;

		private TextWidget activePrintName;

		private PartThumbnailWidget activePrintPreviewImage;

		private TextWidget activePrintStatus;

		private TemperatureWidgetBase bedTemperatureWidget;

		private TemperatureWidgetBase extruderTemperatureWidget;

		private QueueDataView queueDataView;

		private EventHandler unregisterEvents;

		private Button setupButton;

		public TouchScreenPrintStatusRow(QueueDataView queueDataView)
			: this((FlowDirection)0)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			UiThread.RunOnIdle((Action)OnIdle);
			((GuiWidget)this).set_Padding(new BorderDouble(0.0, 0.0, 6.0, 6.0));
			((GuiWidget)this).set_Margin(new BorderDouble(6.0, 3.0, 0.0, 0.0));
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
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		public override void OnMouseUp(MouseEventArgs mouseEvent)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			int num = 20;
			RectangleDouble val = default(RectangleDouble);
			((RectangleDouble)(ref val))._002Ector(((GuiWidget)this).get_Width() - (double)num, ((GuiWidget)this).get_Height() - (double)num, ((GuiWidget)this).get_Width(), ((GuiWidget)this).get_Height());
			if (((RectangleDouble)(ref val)).Contains(mouseEvent.get_Position()) && ((GuiWidget)this).get_MouseCaptured())
			{
				((ButtonBase)setupButton).ClickButton((MouseEventArgs)null);
			}
			else
			{
				((GuiWidget)this).OnMouseUp(mouseEvent);
			}
		}

		private void AddChildElements()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Expected O, but got Unknown
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Expected O, but got Unknown
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			((GuiWidget)val).set_Width(120.0);
			extruderTemperatureWidget = new TemperatureWidgetExtruder();
			((GuiWidget)extruderTemperatureWidget).set_VAnchor((VAnchor)4);
			bedTemperatureWidget = new TemperatureWidgetBed();
			((GuiWidget)bedTemperatureWidget).set_VAnchor((VAnchor)4);
			((GuiWidget)val).AddChild((GuiWidget)(object)extruderTemperatureWidget, -1);
			((GuiWidget)val).AddChild(new GuiWidget(6.0, 6.0, (SizeLimitsToSet)1), -1);
			if (ActiveSliceSettings.Instance.GetValue<bool>("has_heated_bed"))
			{
				((GuiWidget)val).AddChild((GuiWidget)(object)bedTemperatureWidget, -1);
			}
			((GuiWidget)val).AddChild(new GuiWidget(6.0, 6.0, (SizeLimitsToSet)1), -1);
			FlowLayoutWidget val2 = CreateActivePrinterInfoWidget();
			PrintActionRow printActionRow = new PrintActionRow(queueDataView);
			((GuiWidget)printActionRow).set_VAnchor((VAnchor)4);
			ImageButtonFactory imageButtonFactory = new ImageButtonFactory();
			imageButtonFactory.InvertImageColor = false;
			setupButton = imageButtonFactory.Generate(ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon("icon_gear_dot.png")), null);
			((GuiWidget)setupButton).set_Margin(new BorderDouble(6.0, 0.0, 0.0, 0.0));
			((GuiWidget)setupButton).set_VAnchor((VAnchor)2);
			((GuiWidget)setupButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				WizardWindow.Show<SetupOptionsPage>("/SetupOptions", "Setup Wizard");
			});
			((GuiWidget)this).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)printActionRow, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)setupButton, -1);
			((GuiWidget)this).set_Height(80.0);
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
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected O, but got Unknown
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Expected O, but got Unknown
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Expected O, but got Unknown
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_Margin(new BorderDouble(6.0, 0.0, 6.0, 0.0));
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_VAnchor((VAnchor)2);
			((GuiWidget)val).set_Height(80.0);
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_Name("PrintStatusRow.ActivePrinterInfo.TopRow");
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			activePrintLabel = getPrintStatusLabel("Next Print".Localize() + ":", 11);
			((GuiWidget)activePrintLabel).set_VAnchor((VAnchor)4);
			((GuiWidget)val2).AddChild((GuiWidget)(object)activePrintLabel, -1);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			activePrintPreviewImage = new PartThumbnailWidget(null, "part_icon_transparent_100x100.png", "building_thumbnail_100x100.png", PartThumbnailWidget.ImageSizes.Size50x50);
			((GuiWidget)activePrintPreviewImage).set_VAnchor((VAnchor)4);
			((GuiWidget)activePrintPreviewImage).set_Padding(new BorderDouble(0.0));
			activePrintPreviewImage.HoverBackgroundColor = default(RGBA_Bytes);
			activePrintPreviewImage.BorderWidth = 3.0;
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val4).set_VAnchor((VAnchor)(((GuiWidget)val4).get_VAnchor() | 4));
			((GuiWidget)val4).set_Margin(new BorderDouble(8.0, 0.0, 0.0, 4.0));
			activePrintName = getPrintStatusLabel("this is the biggest name we will allow", 14);
			activePrintName.set_AutoExpandBoundsToText(false);
			activePrintStatus = getPrintStatusLabel("this is the biggest label we will allow - bigger", 11);
			activePrintStatus.set_AutoExpandBoundsToText(false);
			((GuiWidget)activePrintStatus).set_Text("");
			((GuiWidget)activePrintStatus).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 3.0));
			((GuiWidget)val4).AddChild((GuiWidget)(object)activePrintName, -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)activePrintStatus, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)activePrintPreviewImage, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			return val;
		}

		private Button GetAutoLevelIndicator()
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			ImageButtonFactory imageButtonFactory = new ImageButtonFactory();
			imageButtonFactory.InvertImageColor = false;
			string normalImageName = Path.Combine("PrintStatusControls", "leveling-16x16.png");
			string hoverImageName = Path.Combine("PrintStatusControls", "leveling-16x16.png");
			Button autoLevelButton = imageButtonFactory.Generate(normalImageName, hoverImageName);
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
			if (ActiveSliceSettings.Instance != null)
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
			if (PrinterConnectionAndCommunication.Instance.ActivePrintItem != null)
			{
				int totalSecondsInPrint = PrinterConnectionAndCommunication.Instance.TotalSecondsInPrint;
				int num = totalSecondsInPrint / 3600;
				int num2 = totalSecondsInPrint / 60 - num * 60;
				totalSecondsInPrint %= 60;
				string text = "Est. Print Time".Localize();
				string arg = "Calculating...".Localize();
				string text2 = ((totalSecondsInPrint > 0) ? ((num <= 0) ? $"{text}: {num2}m {totalSecondsInPrint:00}s" : $"{text}: {num}h {num2:00}m {totalSecondsInPrint:00}s") : ((totalSecondsInPrint >= 0) ? $"{text}: {arg}" : "Streaming GCode...".Localize()));
				((GuiWidget)activePrintLabel).set_Text("Next Print".Localize() + ":");
				switch (PrinterConnectionAndCommunication.Instance.CommunicationState)
				{
				case PrinterConnectionAndCommunication.CommunicationStates.PreparingToPrint:
					((GuiWidget)activePrintLabel).set_Text("Preparing To Print".Localize() + ":");
					break;
				case PrinterConnectionAndCommunication.CommunicationStates.Printing:
					((GuiWidget)activePrintLabel).set_Text(PrinterConnectionAndCommunication.Instance.PrintingStateString);
					((GuiWidget)activePrintStatus).set_Text(text2);
					break;
				case PrinterConnectionAndCommunication.CommunicationStates.Paused:
					((GuiWidget)activePrintLabel).set_Text("Printing Paused".Localize() + ":");
					((GuiWidget)activePrintStatus).set_Text(text2);
					break;
				case PrinterConnectionAndCommunication.CommunicationStates.FinishedPrint:
					((GuiWidget)activePrintLabel).set_Text("Done Printing".Localize() + ":");
					((GuiWidget)activePrintStatus).set_Text(text2);
					break;
				case PrinterConnectionAndCommunication.CommunicationStates.Disconnected:
					((GuiWidget)activePrintStatus).set_Text("Not connected. Press 'Connect' to enable printing.".Localize());
					break;
				case PrinterConnectionAndCommunication.CommunicationStates.AttemptingToConnect:
					((GuiWidget)activePrintStatus).set_Text("Attempting to Connect".Localize() + "...");
					break;
				case PrinterConnectionAndCommunication.CommunicationStates.FailedToConnect:
				case PrinterConnectionAndCommunication.CommunicationStates.ConnectionLost:
					((GuiWidget)activePrintStatus).set_Text("Connection Failed".Localize() + ": " + PrinterConnectionAndCommunication.Instance.ConnectionFailureMessage);
					break;
				default:
					((GuiWidget)activePrintStatus).set_Text(ActiveSliceSettings.Instance.PrinterSelected ? "" : "Select a Printer.".Localize());
					break;
				}
			}
			else
			{
				((GuiWidget)activePrintLabel).set_Text("Next Print".Localize() + ":");
				((GuiWidget)activePrintStatus).set_Text("Press 'Add' to choose an item to print".Localize());
			}
		}
	}
}
