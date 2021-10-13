using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.ConfigurationPage.PrintLeveling;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.ConfigurationPage
{
	public class CalibrationSettingsWidget : SettingsViewBase
	{
		private DisableableWidget printLevelingContainer;

		private EventHandler unregisterEvents;

		private EditLevelingSettingsWindow editLevelingSettingsWindow;

		private TextWidget printLevelingStatusLabel;

		private Button runPrintLevelingButton;

		public CalibrationSettingsWidget()
			: base("Calibration".Localize())
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			printLevelingContainer = new DisableableWidget();
			if (!ActiveSliceSettings.Instance.GetValue<bool>("has_hardware_leveling"))
			{
				((GuiWidget)printLevelingContainer).AddChild((GuiWidget)(object)GetAutoLevelControl(), -1);
				((GuiWidget)mainContainer).AddChild((GuiWidget)(object)printLevelingContainer, -1);
			}
			((GuiWidget)mainContainer).AddChild((GuiWidget)(object)new HorizontalLine(separatorLineColor), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)mainContainer, -1);
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)PrinterStatusChanged, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.EnableChanged.RegisterEvent((EventHandler)PrinterStatusChanged, ref unregisterEvents);
			SetVisibleControls();
		}

		private FlowLayoutWidget GetAutoLevelControl()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Expected O, but got Unknown
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Expected O, but got Unknown
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Expected O, but got Unknown
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_Name("AutoLevelRowItem");
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 4.0));
			ImageBuffer val2 = ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon("leveling_32x32.png", 24, 24));
			if (!ActiveTheme.get_Instance().get_IsDarkTheme())
			{
				ExtensionMethods.InvertLightness(val2);
			}
			ImageWidget val3 = new ImageWidget(val2);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 0.0, 6.0, 0.0));
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			TextWidget val4 = new TextWidget("", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val4.set_AutoExpandBoundsToText(true);
			val4.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val4).set_VAnchor((VAnchor)2);
			((GuiWidget)val4).set_Text("Software Print Leveling".Localize());
			printLevelingStatusLabel = val4;
			((GuiWidget)val).AddChild((GuiWidget)(object)printLevelingStatusLabel, -1);
			Button themedEditButton = TextImageButtonFactory.GetThemedEditButton();
			((GuiWidget)themedEditButton).set_Margin(new BorderDouble(2.0, 2.0, 2.0, 0.0));
			((GuiWidget)themedEditButton).set_VAnchor((VAnchor)4);
			((GuiWidget)themedEditButton).set_VAnchor((VAnchor)2);
			((GuiWidget)themedEditButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					if (editLevelingSettingsWindow == null)
					{
						editLevelingSettingsWindow = new EditLevelingSettingsWindow();
						((GuiWidget)editLevelingSettingsWindow).add_Closed((EventHandler<ClosedEventArgs>)delegate
						{
							editLevelingSettingsWindow = null;
						});
					}
					else
					{
						((GuiWidget)editLevelingSettingsWindow).BringToFront();
					}
				});
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)themedEditButton, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			runPrintLevelingButton = textImageButtonFactory.Generate("Configure".Localize().ToUpper());
			((GuiWidget)runPrintLevelingButton).set_Margin(new BorderDouble(6.0, 0.0, 0.0, 0.0));
			((GuiWidget)runPrintLevelingButton).set_VAnchor((VAnchor)2);
			((GuiWidget)runPrintLevelingButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					LevelWizardBase.ShowPrintLevelWizard(LevelWizardBase.RuningState.UserRequestedCalibration);
				});
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)runPrintLevelingButton, -1);
			CheckBox printLevelingSwitch = ImageButtonFactory.CreateToggleSwitch(ActiveSliceSettings.Instance.GetValue<bool>("print_leveling_enabled"));
			((GuiWidget)printLevelingSwitch).set_VAnchor((VAnchor)2);
			((GuiWidget)printLevelingSwitch).set_Margin(new BorderDouble(16.0, 0.0, 0.0, 0.0));
			printLevelingSwitch.add_CheckedStateChanged((EventHandler)delegate
			{
				ActiveSliceSettings.Instance.Helpers.DoPrintLeveling(printLevelingSwitch.get_Checked());
			});
			PrinterSettings.PrintLevelingEnabledChanged.RegisterEvent((EventHandler)delegate
			{
				printLevelingSwitch.set_Checked(ActiveSliceSettings.Instance.GetValue<bool>("print_leveling_enabled"));
			}, ref unregisterEvents);
			if (!ActiveSliceSettings.Instance.GetValue<bool>("print_leveling_required_to_print"))
			{
				((GuiWidget)val).AddChild((GuiWidget)(object)printLevelingSwitch, -1);
			}
			return val;
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		private void PrinterStatusChanged(object sender, EventArgs e)
		{
			SetVisibleControls();
			((GuiWidget)this).Invalidate();
		}

		private void SetVisibleControls()
		{
			if (!ActiveSliceSettings.Instance.PrinterSelected || PrinterConnectionAndCommunication.Instance.CommunicationState == PrinterConnectionAndCommunication.CommunicationStates.Printing || PrinterConnectionAndCommunication.Instance.PrinterIsPaused)
			{
				printLevelingContainer.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				((GuiWidget)runPrintLevelingButton).set_Enabled(true);
			}
			else
			{
				printLevelingContainer.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				((GuiWidget)runPrintLevelingButton).set_Enabled(PrinterConnectionAndCommunication.Instance.PrinterIsConnected);
			}
		}
	}
}
