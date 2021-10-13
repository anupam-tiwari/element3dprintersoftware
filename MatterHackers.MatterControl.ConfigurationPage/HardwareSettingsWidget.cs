using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.EeProm;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.ConfigurationPage
{
	public class HardwareSettingsWidget : SettingsViewBase
	{
		private Button openGcodeTerminalButton;

		private Button openCameraButton;

		private DisableableWidget eePromControlsContainer;

		private DisableableWidget terminalCommunicationsContainer;

		private EventHandler unregisterEvents;

		private static EePromMarlinWindow openEePromMarlinWidget;

		private static EePromRepetierWindow openEePromRepetierWidget;

		private string noEepromMappingMessage = "Oops! There is no eeprom mapping for your printer's firmware.".Localize() + "\n\n" + "You may need to wait a minute for your printer to finish initializing.".Localize();

		private string noEepromMappingTitle = "Warning - No EEProm Mapping".Localize();

		public HardwareSettingsWidget()
			: base("Hardware".Localize())
		{
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			DisableableWidget disableableWidget = new DisableableWidget();
			((GuiWidget)disableableWidget).set_Visible(false);
			eePromControlsContainer = disableableWidget;
			((GuiWidget)eePromControlsContainer).AddChild((GuiWidget)(object)GetEEPromControl(), -1);
			terminalCommunicationsContainer = new DisableableWidget();
			((GuiWidget)terminalCommunicationsContainer).AddChild((GuiWidget)(object)GetGcodeTerminalControl(), -1);
			((GuiWidget)mainContainer).AddChild((GuiWidget)(object)new HorizontalLine(separatorLineColor), -1);
			((GuiWidget)mainContainer).AddChild((GuiWidget)(object)eePromControlsContainer, -1);
			((GuiWidget)mainContainer).AddChild((GuiWidget)(object)new HorizontalLine(separatorLineColor), -1);
			((GuiWidget)mainContainer).AddChild((GuiWidget)(object)terminalCommunicationsContainer, -1);
			DisableableWidget disableableWidget2 = new DisableableWidget();
			((GuiWidget)disableableWidget2).AddChild((GuiWidget)(object)GetCameraControl(), -1);
			if (ApplicationSettings.Instance.get("HardwareHasCamera") == "true")
			{
				((GuiWidget)mainContainer).AddChild((GuiWidget)(object)new HorizontalLine(separatorLineColor), -1);
				((GuiWidget)mainContainer).AddChild((GuiWidget)(object)disableableWidget2, -1);
			}
			((GuiWidget)this).AddChild((GuiWidget)(object)mainContainer, -1);
			AddHandlers();
			SetEnabledStates();
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		private FlowLayoutWidget GetCameraControl()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Expected O, but got Unknown
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Expected O, but got Unknown
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Expected O, but got Unknown
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 4.0));
			ImageBuffer val2 = ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon("camera-24x24.png", 24, 24));
			val2.SetRecieveBlender((IRecieveBlenderByte)new BlenderPreMultBGRA());
			GuiWidget.get_DeviceScale();
			if (!ActiveTheme.get_Instance().get_IsDarkTheme())
			{
				ExtensionMethods.InvertLightness(val2);
			}
			ImageWidget val3 = new ImageWidget(val2);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 0.0, 6.0, 0.0));
			TextWidget val4 = new TextWidget("Camera Monitoring".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val4.set_AutoExpandBoundsToText(true);
			val4.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val4).set_VAnchor((VAnchor)2);
			openCameraButton = textImageButtonFactory.Generate("Preview".Localize().ToUpper());
			((GuiWidget)openCameraButton).add_Click((EventHandler<MouseEventArgs>)openCameraPreview_Click);
			((GuiWidget)openCameraButton).set_Margin(new BorderDouble(6.0, 0.0, 0.0, 0.0));
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)openCameraButton, -1);
			if (ApplicationSettings.Instance.get("HardwareHasCamera") == "true")
			{
				GuiWidget val5 = (GuiWidget)new FlowLayoutWidget((FlowDirection)0);
				val5.set_VAnchor((VAnchor)2);
				val5.set_Margin(new BorderDouble(16.0, 0.0, 0.0, 0.0));
				CheckBox val6 = ImageButtonFactory.CreateToggleSwitch(ActiveSliceSettings.Instance.GetValue<bool>("publish_bed_image"));
				val6.add_CheckedStateChanged((EventHandler)delegate(object sender, EventArgs e)
				{
					CheckBox val7 = sender as CheckBox;
					ActiveSliceSettings.Instance.SetValue("publish_bed_image", val7.get_Checked() ? "1" : "0");
				});
				val5.AddChild((GuiWidget)(object)val6, -1);
				val5.SetBoundsToEncloseChildren();
				((GuiWidget)val).AddChild(val5, -1);
			}
			return val;
		}

		private FlowLayoutWidget GetGcodeTerminalControl()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Expected O, but got Unknown
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Expected O, but got Unknown
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Expected O, but got Unknown
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 4.0));
			ImageBuffer val2 = ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon("terminal-24x24.png", 24, 24));
			val2.SetRecieveBlender((IRecieveBlenderByte)new BlenderPreMultBGRA());
			GuiWidget.get_DeviceScale();
			if (!ActiveTheme.get_Instance().get_IsDarkTheme())
			{
				ExtensionMethods.InvertLightness(val2);
			}
			ImageWidget val3 = new ImageWidget(val2);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 6.0, 6.0, 0.0));
			TextWidget val4 = new TextWidget("G-Code Terminal".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val4.set_AutoExpandBoundsToText(true);
			val4.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val4).set_VAnchor((VAnchor)2);
			openGcodeTerminalButton = textImageButtonFactory.Generate("Show Terminal".Localize().ToUpper());
			((GuiWidget)openGcodeTerminalButton).set_Name("Show Terminal Button");
			((GuiWidget)openGcodeTerminalButton).add_Click((EventHandler<MouseEventArgs>)openGcodeTerminalButton_Click);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)openGcodeTerminalButton, -1);
			return val;
		}

		private FlowLayoutWidget GetEEPromControl()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Expected O, but got Unknown
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 4.0));
			TextWidget val2 = new TextWidget("EEProm".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_AutoExpandBoundsToText(true);
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_VAnchor((VAnchor)2);
			ImageBuffer val3 = StaticData.get_Instance().LoadIcon("leveling_32x32.png", 24, 24);
			if (!ActiveTheme.get_Instance().get_IsDarkTheme())
			{
				ExtensionMethods.InvertLightness(val3);
			}
			((GuiWidget)new ImageWidget(val3)).set_Margin(new BorderDouble(0.0, 0.0, 6.0, 0.0));
			Button val4 = textImageButtonFactory.Generate("Configure".Localize().ToUpper());
			((GuiWidget)val4).add_Click((EventHandler<MouseEventArgs>)configureEePromButton_Click);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val4, -1);
			return val;
		}

		private void AddHandlers()
		{
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)delegate
			{
				SetEnabledStates();
			}, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.EnableChanged.RegisterEvent((EventHandler)delegate
			{
				SetEnabledStates();
			}, ref unregisterEvents);
		}

		private void openCameraPreview_Click(object sender, EventArgs e)
		{
			MatterControlApplication.Instance.OpenCameraPreview();
		}

		private void configureEePromButton_Click(object sender, EventArgs mouseEvent)
		{
			UiThread.RunOnIdle((Action)delegate
			{
				switch (PrinterConnectionAndCommunication.Instance.FirmwareType)
				{
				case PrinterConnectionAndCommunication.FirmwareTypes.Repetier:
					if (openEePromRepetierWidget != null)
					{
						((GuiWidget)openEePromRepetierWidget).BringToFront();
					}
					else
					{
						openEePromRepetierWidget = new EePromRepetierWindow();
						((GuiWidget)openEePromRepetierWidget).add_Closed((EventHandler<ClosedEventArgs>)delegate
						{
							openEePromRepetierWidget = null;
						});
					}
					break;
				case PrinterConnectionAndCommunication.FirmwareTypes.Marlin:
					if (openEePromMarlinWidget != null)
					{
						((GuiWidget)openEePromMarlinWidget).BringToFront();
					}
					else
					{
						openEePromMarlinWidget = new EePromMarlinWindow();
						((GuiWidget)openEePromMarlinWidget).add_Closed((EventHandler<ClosedEventArgs>)delegate
						{
							openEePromMarlinWidget = null;
						});
					}
					break;
				default:
					PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow("M115");
					StyledMessageBox.ShowMessageBox(null, noEepromMappingMessage, noEepromMappingTitle);
					break;
				}
			});
		}

		private void openGcodeTerminalButton_Click(object sender, EventArgs mouseEvent)
		{
			UiThread.RunOnIdle((Action)TerminalWindow.Show);
		}

		private void SetEnabledStates()
		{
			if (!ActiveSliceSettings.Instance.PrinterSelected)
			{
				eePromControlsContainer.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				terminalCommunicationsContainer.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
			}
			else
			{
				switch (PrinterConnectionAndCommunication.Instance.CommunicationState)
				{
				case PrinterConnectionAndCommunication.CommunicationStates.Disconnected:
				case PrinterConnectionAndCommunication.CommunicationStates.AttemptingToConnect:
				case PrinterConnectionAndCommunication.CommunicationStates.FailedToConnect:
				case PrinterConnectionAndCommunication.CommunicationStates.Disconnecting:
				case PrinterConnectionAndCommunication.CommunicationStates.ConnectionLost:
					eePromControlsContainer.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
					terminalCommunicationsContainer.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
					break;
				case PrinterConnectionAndCommunication.CommunicationStates.Connected:
				case PrinterConnectionAndCommunication.CommunicationStates.FinishedPrint:
					eePromControlsContainer.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
					terminalCommunicationsContainer.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
					break;
				case PrinterConnectionAndCommunication.CommunicationStates.PrintingFromSd:
					eePromControlsContainer.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
					terminalCommunicationsContainer.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
					break;
				case PrinterConnectionAndCommunication.CommunicationStates.PreparingToPrint:
				case PrinterConnectionAndCommunication.CommunicationStates.Printing:
				{
					PrinterConnectionAndCommunication.DetailedPrintingState printingState = PrinterConnectionAndCommunication.Instance.PrintingState;
					if ((uint)printingState <= 3u)
					{
						eePromControlsContainer.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
						terminalCommunicationsContainer.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
						break;
					}
					throw new NotImplementedException();
				}
				case PrinterConnectionAndCommunication.CommunicationStates.Paused:
					eePromControlsContainer.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
					terminalCommunicationsContainer.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
					break;
				default:
					throw new NotImplementedException();
				}
			}
			((GuiWidget)this).Invalidate();
		}
	}
}
