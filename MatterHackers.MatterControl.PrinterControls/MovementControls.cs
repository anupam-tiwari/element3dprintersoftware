using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.MatterControl.Utilities;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PrinterControls
{
	public class MovementControls : ControlWidgetBase
	{
		public FlowLayoutWidget manualControlsLayout;

		private Button disableMotors;

		private EditManualMovementSpeedsWindow editManualMovementSettingsWindow;

		private Button homeAllButton;

		private Button homeXYButton;

		private Button homeXButton;

		private Button homeYButton;

		internal JogControls jogControls;

		private AltGroupBox movementControlsGroupBox;

		internal List<DisableableWidget> DisableableWidgets = new List<DisableableWidget>();

		private TextWidget offsetStreamLabel;

		private LimitCallingFrequency reportDestinationChanged;

		private EventHandler unregisterEvents;

		public static double XSpeed => ActiveSliceSettings.Instance.Helpers.GetMovementSpeeds()["x"];

		public static double YSpeed => ActiveSliceSettings.Instance.Helpers.GetMovementSpeeds()["y"];

		public static double ZSpeed => ActiveSliceSettings.Instance.Helpers.GetMovementSpeeds()["z"];

		public static double EFeedRate(int extruderIndex)
		{
			Dictionary<string, double> movementSpeeds = ActiveSliceSettings.Instance.Helpers.GetMovementSpeeds();
			string key = "e" + extruderIndex;
			if (movementSpeeds.ContainsKey(key))
			{
				return movementSpeeds[key];
			}
			return movementSpeeds["e0"];
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		private DisableableWidget CreateDisableableContainer(GuiWidget widget)
		{
			DisableableWidget disableableWidget = new DisableableWidget();
			((GuiWidget)disableableWidget).AddChild(widget, -1);
			DisableableWidgets.Add(disableableWidget);
			return disableableWidget;
		}

		public MovementControls()
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Expected O, but got Unknown
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			movementControlsGroupBox = new AltGroupBox(textImageButtonFactory.GenerateGroupBoxLabelWithEdit(new TextWidget("Movement".Localize(), 0.0, 0.0, 18.0, (Justification)0, ActiveTheme.get_Instance().get_SecondaryAccentColor(), true, false, default(RGBA_Bytes), (TypeFace)null), out var editButton));
			((GuiWidget)editButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				if (editManualMovementSettingsWindow == null)
				{
					editManualMovementSettingsWindow = new EditManualMovementSpeedsWindow("Movement Speeds".Localize(), ActiveSliceSettings.Instance.Helpers.GetMovementSpeedsString(), SetMovementSpeeds);
					((GuiWidget)editManualMovementSettingsWindow).add_Closed((EventHandler<ClosedEventArgs>)delegate
					{
						editManualMovementSettingsWindow = null;
					});
				}
				else
				{
					((GuiWidget)editManualMovementSettingsWindow).BringToFront();
				}
			});
			((GuiWidget)movementControlsGroupBox).set_Margin(new BorderDouble(0.0));
			movementControlsGroupBox.TextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			AltGroupBox altGroupBox = movementControlsGroupBox;
			((GuiWidget)altGroupBox).set_HAnchor((HAnchor)(((GuiWidget)altGroupBox).get_HAnchor() | 5));
			((GuiWidget)movementControlsGroupBox).set_VAnchor((VAnchor)8);
			jogControls = new JogControls(new XYZColors());
			((GuiWidget)jogControls).set_Margin(new BorderDouble(0.0));
			manualControlsLayout = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)manualControlsLayout).set_HAnchor((HAnchor)5);
			((GuiWidget)manualControlsLayout).set_VAnchor((VAnchor)8);
			((GuiWidget)manualControlsLayout).set_Padding(new BorderDouble(3.0, 5.0, 3.0, 0.0));
			new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)manualControlsLayout).AddChild((GuiWidget)(object)CreateDisableableContainer((GuiWidget)(object)GetHomeButtonBar()), -1);
			((GuiWidget)manualControlsLayout).AddChild((GuiWidget)(object)CreateDisableableContainer(ControlWidgetBase.CreateSeparatorLine()), -1);
			((GuiWidget)manualControlsLayout).AddChild((GuiWidget)(object)jogControls, -1);
			((GuiWidget)manualControlsLayout).AddChild((GuiWidget)(object)CreateDisableableContainer(ControlWidgetBase.CreateSeparatorLine()), -1);
			((GuiWidget)manualControlsLayout).AddChild((GuiWidget)(object)CreateDisableableContainer((GuiWidget)(object)GetHWDestinationBar()), -1);
			((GuiWidget)manualControlsLayout).AddChild((GuiWidget)(object)CreateDisableableContainer(ControlWidgetBase.CreateSeparatorLine()), -1);
			((GuiWidget)movementControlsGroupBox).AddChild((GuiWidget)(object)manualControlsLayout, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)movementControlsGroupBox, -1);
		}

		private static void SetMovementSpeeds(string speedString)
		{
			if (!string.IsNullOrEmpty(speedString))
			{
				ActiveSliceSettings.Instance.SetValue("manual_movement_speeds", speedString);
				ApplicationController.Instance.ReloadAdvancedControlsPanel();
			}
		}

		private FlowLayoutWidget GetHomeButtonBar()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Expected O, but got Unknown
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Expected O, but got Unknown
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Expected O, but got Unknown
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0522: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0537: Unknown result type (might be due to invalid IL or missing references)
			//IL_0544: Unknown result type (might be due to invalid IL or missing references)
			//IL_054d: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(3.0, 0.0, 3.0, 6.0));
			((GuiWidget)val).set_Padding(new BorderDouble(0.0));
			textImageButtonFactory.borderWidth = 1.0;
			textImageButtonFactory.normalBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);
			textImageButtonFactory.hoverBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);
			ImageBuffer val2 = StaticData.get_Instance().LoadIcon("icon_home_white_24x24.png", 24, 24);
			if (ActiveTheme.get_Instance().get_IsDarkTheme())
			{
				ExtensionMethods.InvertLightness(val2);
			}
			ImageWidget val3 = new ImageWidget(val2);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 0.0, 6.0, 0.0));
			((GuiWidget)val3).set_OriginRelativeParent(((GuiWidget)val3).get_OriginRelativeParent() + new Vector2(0.0, 2.0) * GuiWidget.get_DeviceScale());
			RGBA_Bytes normalFillColor = textImageButtonFactory.normalFillColor;
			textImageButtonFactory.normalFillColor = new RGBA_Bytes(180, 180, 180);
			homeAllButton = textImageButtonFactory.Generate("ALL".Localize());
			textImageButtonFactory.normalFillColor = normalFillColor;
			((GuiWidget)homeAllButton).set_ToolTipText("Home X, Y and Z".Localize());
			((GuiWidget)homeAllButton).set_Margin(new BorderDouble(0.0, 0.0, 6.0, 0.0));
			((GuiWidget)homeAllButton).add_Click((EventHandler<MouseEventArgs>)homeAll_Click);
			textImageButtonFactory.FixedWidth = (double)(int)((GuiWidget)homeAllButton).get_Width() * GuiWidget.get_DeviceScale();
			homeXYButton = textImageButtonFactory.Generate("XY", (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)homeXYButton).set_ToolTipText("Home XY".Localize());
			((GuiWidget)homeXYButton).set_Margin(new BorderDouble(0.0, 0.0, 6.0, 0.0));
			((GuiWidget)homeXYButton).add_Click((EventHandler<MouseEventArgs>)homeXYButton_Click);
			homeXButton = textImageButtonFactory.Generate("X", (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)homeXButton).set_ToolTipText("Home X".Localize());
			((GuiWidget)homeXButton).set_Margin(new BorderDouble(0.0, 0.0, 6.0, 0.0));
			((GuiWidget)homeXButton).add_Click((EventHandler<MouseEventArgs>)homeXButton_Click);
			homeYButton = textImageButtonFactory.Generate("Y", (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)homeYButton).set_ToolTipText("Home Y".Localize());
			((GuiWidget)homeYButton).set_Margin(new BorderDouble(0.0, 0.0, 6.0, 0.0));
			((GuiWidget)homeYButton).add_Click((EventHandler<MouseEventArgs>)homeYButton_Click);
			textImageButtonFactory.normalFillColor = RGBA_Bytes.White;
			textImageButtonFactory.FixedWidth = 0.0;
			disableMotors = textImageButtonFactory.Generate("Release".Localize().ToUpper());
			((GuiWidget)disableMotors).set_Margin(new BorderDouble(0.0));
			((GuiWidget)disableMotors).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				PrinterConnectionAndCommunication.Instance.ReleaseMotors();
			});
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			GuiWidget val4 = new GuiWidget(10.0 * GuiWidget.get_DeviceScale(), 0.0, (SizeLimitsToSet)1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)homeAllButton, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)homeXYButton, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)homeXButton, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)homeYButton, -1);
			TextWidget val5 = new TextWidget("Z Offset".Localize() + ":", 0.0, 0.0, 8.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val5.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val5).set_Margin(new BorderDouble(10.0, 0.0, 0.0, 0.0));
			val5.set_AutoExpandBoundsToText(true);
			((GuiWidget)val5).set_VAnchor((VAnchor)2);
			offsetStreamLabel = val5;
			((GuiWidget)val).AddChild((GuiWidget)(object)offsetStreamLabel, -1);
			ZTuningWidget zTuningWidget = new ZTuningWidget();
			((GuiWidget)val).AddChild((GuiWidget)(object)zTuningWidget, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)disableMotors, -1);
			((GuiWidget)val).AddChild(val4, -1);
			return val;
		}

		private FlowLayoutWidget GetHWDestinationBar()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Expected O, but got Unknown
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Expected O, but got Unknown
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Expected O, but got Unknown
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(3.0, 0.0, 3.0, 6.0));
			((GuiWidget)val).set_Padding(new BorderDouble(0.0));
			TextWidget xPosition = new TextWidget("X: 0.0           ", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			TextWidget yPosition = new TextWidget("Y: 0.0           ", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			TextWidget zPosition = new TextWidget("Z: 0.0           ", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val).AddChild((GuiWidget)(object)xPosition, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)yPosition, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)zPosition, -1);
			SetDestinationPositionText(xPosition, yPosition, zPosition);
			reportDestinationChanged = new LimitCallingFrequency(1.0, delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					SetDestinationPositionText(xPosition, yPosition, zPosition);
				});
			});
			PrinterConnectionAndCommunication.Instance.DestinationChanged.RegisterEvent((EventHandler)delegate
			{
				reportDestinationChanged.CallEvent();
			}, ref unregisterEvents);
			return val;
		}

		private static void SetDestinationPositionText(TextWidget xPosition, TextWidget yPosition, TextWidget zPosition)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			Vector3 currentDestination = PrinterConnectionAndCommunication.Instance.CurrentDestination;
			((GuiWidget)xPosition).set_Text(StringHelper.FormatWith("X: {0:0.00}", new object[1]
			{
				currentDestination.x
			}));
			((GuiWidget)yPosition).set_Text(StringHelper.FormatWith("Y: {0:0.00}", new object[1]
			{
				currentDestination.y
			}));
			((GuiWidget)zPosition).set_Text(StringHelper.FormatWith("Z: {0:0.00}", new object[1]
			{
				currentDestination.z
			}));
		}

		private void homeAll_Click(object sender, EventArgs mouseEvent)
		{
			PrinterConnectionAndCommunication.Instance.HomeAxis(PrinterConnectionAndCommunication.Axis.XYZ);
		}

		private void homeXYButton_Click(object sender, EventArgs mouseEvent)
		{
			PrinterConnectionAndCommunication.Instance.HomeAxis(PrinterConnectionAndCommunication.Axis.XY);
		}

		private void homeXButton_Click(object sender, EventArgs mouseEvent)
		{
			PrinterConnectionAndCommunication.Instance.HomeAxis(PrinterConnectionAndCommunication.Axis.X);
		}

		private void homeYButton_Click(object sender, EventArgs mouseEvent)
		{
			PrinterConnectionAndCommunication.Instance.HomeAxis(PrinterConnectionAndCommunication.Axis.Y);
		}
	}
}
