using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.PrinterControls;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class JogControls : GuiWidget
	{
		public class MoveButton : Button
		{
			private PrinterConnectionAndCommunication.Axis moveAxis;

			public double MoveAmount = 10.0;

			private double movementFeedRate;

			public MoveButton(double x, double y, GuiWidget buttonView, PrinterConnectionAndCommunication.Axis axis, double movementFeedRate)
				: this(x, y, buttonView)
			{
				MoveButton moveButton = this;
				moveAxis = axis;
				this.movementFeedRate = movementFeedRate;
				((GuiWidget)this).add_Click((EventHandler<MouseEventArgs>)delegate(object s, MouseEventArgs e)
				{
					_ = (MoveButton)s;
					if (PrinterConnectionAndCommunication.Instance.CommunicationState == PrinterConnectionAndCommunication.CommunicationStates.Printing)
					{
						if (moveButton.moveAxis == PrinterConnectionAndCommunication.Axis.Z)
						{
							double value = ActiveSliceSettings.Instance.GetValue<double>("baby_step_z_offset");
							value += moveButton.MoveAmount;
							ActiveSliceSettings.Instance.SetValue("baby_step_z_offset", value.ToString("0.##"));
						}
					}
					else
					{
						PrinterConnectionAndCommunication.Instance.MoveRelative(moveButton.moveAxis, moveButton.MoveAmount, movementFeedRate);
					}
				});
			}
		}

		public class ExtrudeButton : Button
		{
			public double MoveAmount = 10.0;

			private double movementFeedRate;

			public int ExtruderNumber;

			public ExtrudeButton(double x, double y, GuiWidget buttonView, double movementFeedRate, int extruderNumber)
				: this(x, y, buttonView)
			{
				ExtruderNumber = extruderNumber;
				this.movementFeedRate = movementFeedRate;
			}

			public override void OnClick(MouseEventArgs mouseEvent)
			{
				((GuiWidget)this).OnClick(mouseEvent);
				PrinterConnectionAndCommunication.Instance.MoveExtruderRelative(MoveAmount, movementFeedRate, ExtruderNumber);
			}
		}

		public class MoveButtonWidget : GuiWidget
		{
			private RGBA_Bytes borderColor;

			private Stroke borderStroke;

			public double BorderWidth
			{
				get;
				set;
			} = 1.0;


			public MoveButtonWidget(string label, RGBA_Bytes textColor, double fontSize = 12.0)
				: this()
			{
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Expected O, but got Unknown
				((GuiWidget)this).set_Margin(BorderDouble.op_Implicit(0));
				((GuiWidget)this).set_Padding(BorderDouble.op_Implicit(0));
				borderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);
				((GuiWidget)this).AnchorAll();
				if (label != "")
				{
					TextWidget val = new TextWidget(label, 0.0, 0.0, fontSize, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
					((GuiWidget)val).set_VAnchor((VAnchor)2);
					((GuiWidget)val).set_HAnchor((HAnchor)2);
					val.set_TextColor(textColor);
					((GuiWidget)val).set_Padding(new BorderDouble(3.0, 0.0));
					TextWidget val2 = val;
					((GuiWidget)this).AddChild((GuiWidget)(object)val2, -1);
				}
			}

			public override void OnBoundsChanged(EventArgs e)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Expected O, but got Unknown
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Expected O, but got Unknown
				borderStroke = new Stroke((IVertexSource)new RoundedRect(((GuiWidget)this).get_LocalBounds(), 0.0), BorderWidth);
				((GuiWidget)this).OnBoundsChanged(e);
			}

			public override void OnDraw(Graphics2D graphics2D)
			{
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				((GuiWidget)this).OnDraw(graphics2D);
				if (BorderWidth > 0.0 && borderStroke != null)
				{
					graphics2D.Render((IVertexSource)(object)borderStroke, (IColorType)(object)borderColor);
				}
			}
		}

		public class WidgetStateColors
		{
			public RGBA_Bytes Normal
			{
				get;
				set;
			}

			public RGBA_Bytes Hover
			{
				get;
				set;
			}

			public RGBA_Bytes Pressed
			{
				get;
				set;
			}

			public RGBA_Bytes Disabled
			{
				get;
				set;
			}
		}

		public class WidgetColors
		{
			public WidgetStateColors Fill
			{
				get;
				set;
			}

			public WidgetStateColors Text
			{
				get;
				set;
			}
		}

		public class MoveButtonFactory
		{
			public BorderDouble Padding;

			public BorderDouble Margin;

			public WidgetColors Colors
			{
				get;
				set;
			} = new WidgetColors
			{
				Text = new WidgetStateColors
				{
					Normal = RGBA_Bytes.Black,
					Hover = RGBA_Bytes.White,
					Pressed = RGBA_Bytes.White,
					Disabled = RGBA_Bytes.White
				},
				Fill = new WidgetStateColors
				{
					Normal = RGBA_Bytes.White,
					Hover = new RGBA_Bytes(0, 0, 0, 50),
					Pressed = RGBA_Bytes.Transparent,
					Disabled = new RGBA_Bytes(255, 255, 255, 50)
				}
			};


			public double FontSize
			{
				get;
				set;
			} = 12.0;


			public double BorderWidth
			{
				get;
				set;
			} = 1.0;


			public MoveButton GenerateMoveButton(string label, PrinterConnectionAndCommunication.Axis axis, double movementFeedRate)
			{
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				MoveButton moveButton = new MoveButton(0.0, 0.0, (GuiWidget)(object)GetButtonView(label), axis, movementFeedRate);
				((GuiWidget)moveButton).set_Margin(BorderDouble.op_Implicit(0));
				((GuiWidget)moveButton).set_Padding(BorderDouble.op_Implicit(0));
				return moveButton;
			}

			public ExtrudeButton GenerateExtrudeButton(string label, double movementFeedRate, int extruderNumber)
			{
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				ExtrudeButton extrudeButton = new ExtrudeButton(0.0, 0.0, (GuiWidget)(object)GetButtonView(label), movementFeedRate, extruderNumber);
				((GuiWidget)extrudeButton).set_Margin(BorderDouble.op_Implicit(0));
				((GuiWidget)extrudeButton).set_Padding(BorderDouble.op_Implicit(0));
				return extrudeButton;
			}

			private ButtonViewStates GetButtonView(string label)
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_0104: Unknown result type (might be due to invalid IL or missing references)
				//IL_0109: Unknown result type (might be due to invalid IL or missing references)
				//IL_0110: Unknown result type (might be due to invalid IL or missing references)
				//IL_0118: Expected O, but got Unknown
				MoveButtonWidget moveButtonWidget = new MoveButtonWidget(label, Colors.Text.Normal);
				((GuiWidget)moveButtonWidget).set_BackgroundColor(Colors.Fill.Normal);
				moveButtonWidget.BorderWidth = BorderWidth;
				MoveButtonWidget moveButtonWidget2 = new MoveButtonWidget(label, Colors.Text.Hover);
				((GuiWidget)moveButtonWidget2).set_BackgroundColor(Colors.Fill.Hover);
				moveButtonWidget2.BorderWidth = BorderWidth;
				MoveButtonWidget moveButtonWidget3 = new MoveButtonWidget(label, Colors.Text.Pressed);
				((GuiWidget)moveButtonWidget3).set_BackgroundColor(Colors.Fill.Pressed);
				moveButtonWidget3.BorderWidth = BorderWidth;
				MoveButtonWidget moveButtonWidget4 = new MoveButtonWidget(label, Colors.Text.Disabled);
				((GuiWidget)moveButtonWidget4).set_BackgroundColor(Colors.Fill.Disabled);
				moveButtonWidget4.BorderWidth = BorderWidth;
				ButtonViewStates val = new ButtonViewStates((GuiWidget)(object)moveButtonWidget, (GuiWidget)(object)moveButtonWidget2, (GuiWidget)(object)moveButtonWidget3, (GuiWidget)(object)moveButtonWidget4);
				((GuiWidget)val).set_HAnchor((HAnchor)5);
				((GuiWidget)val).set_VAnchor((VAnchor)5);
				return val;
			}
		}

		public static double AxisMoveAmount;

		public static int EAxisMoveAmount;

		private MoveButton xPlusControl;

		private MoveButton xMinusControl;

		private MoveButton yPlusControl;

		private MoveButton yMinusControl;

		private MoveButton zPlusControl;

		private MoveButton zMinusControl;

		private MoveButtonFactory moveButtonFactory = new MoveButtonFactory();

		private List<ExtrudeButton> eMinusButtons = new List<ExtrudeButton>();

		private List<ExtrudeButton> ePlusButtons = new List<ExtrudeButton>();

		private RadioButton oneHundredButton;

		private RadioButton tenButton;

		private DisableableWidget disableableEButtons;

		private DisableableWidget tooBigForBabyStepping;

		private RadioButton movePointZeroTwoMmButton;

		private RadioButton moveOneMmButton;

		private GuiWidget keyboardFocusBorder;

		private ImageWidget keyboardImage;

		public JogControls(XYZColors colors)
			: this()
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected O, but got Unknown
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Expected O, but got Unknown
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Expected O, but got Unknown
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Expected O, but got Unknown
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Expected O, but got Unknown
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Expected O, but got Unknown
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Expected O, but got Unknown
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Expected O, but got Unknown
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Expected O, but got Unknown
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
			moveButtonFactory.Colors.Text.Normal = RGBA_Bytes.Black;
			double num = 12.0;
			double buttonSeparationDistance = 10.0;
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)(((GuiWidget)val).get_HAnchor() | 5));
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)3);
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)0);
			GuiWidget val5 = CreateXYGridControl(colors, num, buttonSeparationDistance);
			((GuiWidget)val4).AddChild(val5, -1);
			FlowLayoutWidget val6 = CreateZButtons(XYZColors.zColor, buttonSeparationDistance, out zPlusControl, out zMinusControl);
			((GuiWidget)val6).set_VAnchor((VAnchor)1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)val6, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val4, -1);
			FlowLayoutWidget val7 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)new TextWidget("Distance:", 0.0, 0.0, 12.0, (Justification)0, RGBA_Bytes.White, true, false, default(RGBA_Bytes), (TypeFace)null)).set_VAnchor((VAnchor)2);
			TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory
			{
				FixedHeight = 20.0 * GuiWidget.get_DeviceScale(),
				FixedWidth = 30.0 * GuiWidget.get_DeviceScale(),
				fontSize = 8.0,
				Margin = new BorderDouble(0.0),
				checkedBorderColor = ActiveTheme.get_Instance().get_PrimaryTextColor()
			};
			FlowLayoutWidget val8 = new FlowLayoutWidget((FlowDirection)0);
			ObservableCollection<GuiWidget> siblingRadioButtonList = new ObservableCollection<GuiWidget>();
			movePointZeroTwoMmButton = textImageButtonFactory.GenerateRadioButton("0.02");
			((GuiWidget)movePointZeroTwoMmButton).set_VAnchor((VAnchor)2);
			movePointZeroTwoMmButton.add_CheckedStateChanged((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				if (((RadioButton)sender).get_Checked())
				{
					SetXYZMoveAmount(0.02);
				}
			});
			movePointZeroTwoMmButton.set_SiblingRadioButtonList(siblingRadioButtonList);
			((GuiWidget)val8).AddChild((GuiWidget)(object)movePointZeroTwoMmButton, -1);
			RadioButton val9 = textImageButtonFactory.GenerateRadioButton("0.1");
			((GuiWidget)val9).set_VAnchor((VAnchor)2);
			val9.add_CheckedStateChanged((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				if (((RadioButton)sender).get_Checked())
				{
					SetXYZMoveAmount(0.1);
				}
			});
			val9.set_SiblingRadioButtonList(siblingRadioButtonList);
			((GuiWidget)val8).AddChild((GuiWidget)(object)val9, -1);
			moveOneMmButton = textImageButtonFactory.GenerateRadioButton("1");
			((GuiWidget)moveOneMmButton).set_VAnchor((VAnchor)2);
			moveOneMmButton.add_CheckedStateChanged((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				if (((RadioButton)sender).get_Checked())
				{
					SetXYZMoveAmount(1.0);
				}
			});
			moveOneMmButton.set_SiblingRadioButtonList(siblingRadioButtonList);
			((GuiWidget)val8).AddChild((GuiWidget)(object)moveOneMmButton, -1);
			DisableableWidget disableableWidget = new DisableableWidget();
			((GuiWidget)disableableWidget).set_VAnchor((VAnchor)8);
			((GuiWidget)disableableWidget).set_HAnchor((HAnchor)8);
			tooBigForBabyStepping = disableableWidget;
			FlowLayoutWidget val10 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)tooBigForBabyStepping).AddChild((GuiWidget)(object)val10, -1);
			tenButton = textImageButtonFactory.GenerateRadioButton("10");
			((GuiWidget)tenButton).set_VAnchor((VAnchor)2);
			tenButton.add_CheckedStateChanged((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				if (((RadioButton)sender).get_Checked())
				{
					SetXYZMoveAmount(10.0);
				}
			});
			tenButton.set_SiblingRadioButtonList(siblingRadioButtonList);
			((GuiWidget)val10).AddChild((GuiWidget)(object)tenButton, -1);
			oneHundredButton = textImageButtonFactory.GenerateRadioButton("100");
			((GuiWidget)oneHundredButton).set_VAnchor((VAnchor)2);
			oneHundredButton.add_CheckedStateChanged((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				if (((RadioButton)sender).get_Checked())
				{
					SetXYZMoveAmount(100.0);
				}
			});
			oneHundredButton.set_SiblingRadioButtonList(siblingRadioButtonList);
			((GuiWidget)val10).AddChild((GuiWidget)(object)oneHundredButton, -1);
			((GuiWidget)val8).AddChild((GuiWidget)(object)tooBigForBabyStepping, -1);
			tenButton.set_Checked(true);
			((GuiWidget)val8).set_Margin(new BorderDouble(0.0, 3.0));
			((GuiWidget)val7).AddChild((GuiWidget)(object)val8, -1);
			TextWidget val11 = new TextWidget("mm", 0.0, 0.0, 8.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val11).set_Margin(new BorderDouble(10.0, 0.0, 0.0, 0.0));
			((GuiWidget)val11).set_VAnchor((VAnchor)2);
			((GuiWidget)val10).AddChild((GuiWidget)(object)val11, -1);
			((GuiWidget)val7).set_HAnchor((HAnchor)1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val7, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)GetHotkeyControlContainer(), -1);
			GuiWidget val12 = new GuiWidget(2.0, 2.0, (SizeLimitsToSet)1);
			val12.set_VAnchor((VAnchor)5);
			val12.set_BackgroundColor(RGBA_Bytes.White);
			val12.set_Margin(new BorderDouble(num, 5.0));
			((GuiWidget)val2).AddChild(val12, -1);
			FlowLayoutWidget val13 = CreateEButtons(buttonSeparationDistance);
			DisableableWidget disableableWidget2 = new DisableableWidget();
			((GuiWidget)disableableWidget2).set_Name("disableableEButtons");
			((GuiWidget)disableableWidget2).set_HAnchor((HAnchor)8);
			((GuiWidget)disableableWidget2).set_VAnchor((VAnchor)12);
			disableableEButtons = disableableWidget2;
			((GuiWidget)disableableEButtons).AddChild((GuiWidget)(object)val13, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)disableableEButtons, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)this).set_HAnchor((HAnchor)8);
			((GuiWidget)this).set_VAnchor((VAnchor)8);
			((GuiWidget)this).set_Margin(new BorderDouble(3.0));
		}

		internal void SetEnabledLevels(bool enableBabysteppingMode, bool enableEControls)
		{
			if (enableBabysteppingMode)
			{
				if (zPlusControl.MoveAmount >= 1.0)
				{
					movePointZeroTwoMmButton.set_Checked(true);
				}
			}
			else if (zPlusControl.MoveAmount < 1.0)
			{
				moveOneMmButton.set_Checked(true);
			}
			((GuiWidget)tenButton).set_Enabled(!enableBabysteppingMode);
			((GuiWidget)oneHundredButton).set_Enabled(!enableBabysteppingMode);
			disableableEButtons.SetEnableLevel(enableEControls ? DisableableWidget.EnableLevel.Enabled : DisableableWidget.EnableLevel.Disabled);
			tooBigForBabyStepping.SetEnableLevel((!enableBabysteppingMode) ? DisableableWidget.EnableLevel.Enabled : DisableableWidget.EnableLevel.Disabled);
		}

		private void SetEMoveAmount(int moveAmount)
		{
			foreach (ExtrudeButton eMinusButton in eMinusButtons)
			{
				eMinusButton.MoveAmount = -moveAmount;
				EAxisMoveAmount = moveAmount;
			}
			foreach (ExtrudeButton ePlusButton in ePlusButtons)
			{
				ePlusButton.MoveAmount = moveAmount;
				EAxisMoveAmount = moveAmount;
			}
		}

		private void SetXYZMoveAmount(double moveAmount)
		{
			xPlusControl.MoveAmount = moveAmount;
			xMinusControl.MoveAmount = 0.0 - moveAmount;
			yPlusControl.MoveAmount = moveAmount;
			yMinusControl.MoveAmount = 0.0 - moveAmount;
			zPlusControl.MoveAmount = moveAmount;
			zMinusControl.MoveAmount = 0.0 - moveAmount;
			AxisMoveAmount = moveAmount;
		}

		private FlowLayoutWidget GetHotkeyControlContainer()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Expected O, but got Unknown
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Expected O, but got Unknown
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)8);
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			((GuiWidget)val).set_ToolTipText("Enable cursor keys for movement".Localize());
			((GuiWidget)val).set_Margin(new BorderDouble(10.0, 0.0, 0.0, 0.0));
			ImageBuffer val2 = StaticData.get_Instance().LoadIcon("hot_key_small_white.png", 19, 12);
			if (ActiveTheme.get_Instance().get_IsDarkTheme())
			{
				val2 = ExtensionMethods.InvertLightness(val2);
			}
			ImageWidget val3 = new ImageWidget(val2);
			((GuiWidget)val3).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((GuiWidget)val3).set_VAnchor((VAnchor)2);
			((GuiWidget)val3).set_HAnchor((HAnchor)2);
			((GuiWidget)val3).set_Margin(new BorderDouble(5.0));
			((GuiWidget)val3).set_Visible(false);
			keyboardImage = val3;
			GuiWidget val4 = new GuiWidget(1.0, 1.0, (SizeLimitsToSet)1);
			val4.set_MinimumSize(new Vector2(((GuiWidget)keyboardImage).get_Width() + 5.0, ((GuiWidget)keyboardImage).get_Height() + 5.0));
			keyboardFocusBorder = val4;
			keyboardFocusBorder.AddChild((GuiWidget)(object)keyboardImage, -1);
			((GuiWidget)val).AddChild(keyboardFocusBorder, -1);
			return val;
		}

		public override void OnLoad(EventArgs args)
		{
			IEnumerable<AltGroupBox> enumerable = ExtensionMethods.Parents<AltGroupBox>(keyboardFocusBorder);
			((GuiWidget)Enumerable.First<AltGroupBox>(enumerable)).add_KeyDown((EventHandler<KeyEventArgs>)JogControls_KeyDown);
			((GuiWidget)Enumerable.First<AltGroupBox>(enumerable)).add_ContainsFocusChanged((EventHandler)delegate(object sender, EventArgs e)
			{
				if ((sender as GuiWidget).get_ContainsFocus() && !UserSettings.Instance.IsTouchScreen)
				{
					((GuiWidget)keyboardImage).set_Visible(true);
				}
				else
				{
					((GuiWidget)keyboardImage).set_Visible(false);
				}
			});
			((GuiWidget)this).OnLoad(args);
		}

		private void JogControls_KeyDown(object sender, KeyEventArgs e)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Invalid comparison between Unknown and I4
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Invalid comparison between Unknown and I4
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Invalid comparison between Unknown and I4
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Invalid comparison between Unknown and I4
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Invalid comparison between Unknown and I4
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Invalid comparison between Unknown and I4
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Invalid comparison between Unknown and I4
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Invalid comparison between Unknown and I4
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Invalid comparison between Unknown and I4
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Invalid comparison between Unknown and I4
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Invalid comparison between Unknown and I4
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Invalid comparison between Unknown and I4
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Invalid comparison between Unknown and I4
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Invalid comparison between Unknown and I4
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Invalid comparison between Unknown and I4
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Invalid comparison between Unknown and I4
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Invalid comparison between Unknown and I4
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Invalid comparison between Unknown and I4
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Invalid comparison between Unknown and I4
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Invalid comparison between Unknown and I4
			double axisMoveAmount = AxisMoveAmount;
			double num = 0.0 - AxisMoveAmount;
			int eAxisMoveAmount = EAxisMoveAmount;
			int num2 = -EAxisMoveAmount;
			if (PrinterConnectionAndCommunication.Instance.CommunicationState != PrinterConnectionAndCommunication.CommunicationStates.Printing && ((int)OsInformation.get_OperatingSystem() == 1 || (int)OsInformation.get_OperatingSystem() == 2))
			{
				if ((int)e.get_KeyCode() == 90)
				{
					if (PrinterConnectionAndCommunication.Instance.CommunicationState != PrinterConnectionAndCommunication.CommunicationStates.Printing)
					{
						PrinterConnectionAndCommunication.Instance.HomeAxis(PrinterConnectionAndCommunication.Axis.Z);
					}
				}
				else if ((int)e.get_KeyCode() == 89)
				{
					PrinterConnectionAndCommunication.Instance.HomeAxis(PrinterConnectionAndCommunication.Axis.Y);
				}
				else if ((int)e.get_KeyCode() == 88)
				{
					PrinterConnectionAndCommunication.Instance.HomeAxis(PrinterConnectionAndCommunication.Axis.X);
				}
				if ((int)e.get_KeyCode() == 36)
				{
					PrinterConnectionAndCommunication.Instance.HomeAxis(PrinterConnectionAndCommunication.Axis.XYZ);
				}
				else if ((int)e.get_KeyCode() == 37)
				{
					PrinterConnectionAndCommunication.Instance.MoveRelative(PrinterConnectionAndCommunication.Axis.X, num, MovementControls.XSpeed);
				}
				else if ((int)e.get_KeyCode() == 39)
				{
					PrinterConnectionAndCommunication.Instance.MoveRelative(PrinterConnectionAndCommunication.Axis.X, axisMoveAmount, MovementControls.XSpeed);
				}
				else if ((int)e.get_KeyCode() == 38)
				{
					PrinterConnectionAndCommunication.Instance.MoveRelative(PrinterConnectionAndCommunication.Axis.Y, axisMoveAmount, MovementControls.YSpeed);
				}
				else if ((int)e.get_KeyCode() == 40)
				{
					PrinterConnectionAndCommunication.Instance.MoveRelative(PrinterConnectionAndCommunication.Axis.Y, num, MovementControls.YSpeed);
				}
				else if ((int)e.get_KeyCode() == 69)
				{
					PrinterConnectionAndCommunication.Instance.MoveRelative(PrinterConnectionAndCommunication.Axis.E, eAxisMoveAmount, MovementControls.EFeedRate(0));
				}
				else if ((int)e.get_KeyCode() == 82)
				{
					PrinterConnectionAndCommunication.Instance.MoveRelative(PrinterConnectionAndCommunication.Axis.E, num2, MovementControls.EFeedRate(0));
				}
			}
			if (((int)OsInformation.get_OperatingSystem() == 1 && (int)e.get_KeyCode() == 33) || ((int)OsInformation.get_OperatingSystem() == 2 && (int)e.get_KeyCode() == 11))
			{
				if (PrinterConnectionAndCommunication.Instance.CommunicationState == PrinterConnectionAndCommunication.CommunicationStates.Printing)
				{
					double value = ActiveSliceSettings.Instance.GetValue<double>("baby_step_z_offset");
					ActiveSliceSettings.Instance.SetValue("baby_step_z_offset", (value + axisMoveAmount).ToString("0.##"));
				}
				else
				{
					PrinterConnectionAndCommunication.Instance.MoveRelative(PrinterConnectionAndCommunication.Axis.Z, axisMoveAmount, MovementControls.ZSpeed);
				}
			}
			else if (((int)OsInformation.get_OperatingSystem() == 1 && (int)e.get_KeyCode() == 34) || ((int)OsInformation.get_OperatingSystem() == 2 && (int)e.get_KeyCode() == 12))
			{
				if (PrinterConnectionAndCommunication.Instance.CommunicationState == PrinterConnectionAndCommunication.CommunicationStates.Printing)
				{
					double value2 = ActiveSliceSettings.Instance.GetValue<double>("baby_step_z_offset");
					ActiveSliceSettings.Instance.SetValue("baby_step_z_offset", (value2 + num).ToString("0.##"));
				}
				else
				{
					PrinterConnectionAndCommunication.Instance.MoveRelative(PrinterConnectionAndCommunication.Axis.Z, num, MovementControls.ZSpeed);
				}
			}
		}

		private FlowLayoutWidget CreateEButtons(double buttonSeparationDistance)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Expected O, but got Unknown
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Expected O, but got Unknown
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Expected O, but got Unknown
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Expected O, but got Unknown
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Expected O, but got Unknown
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Expected O, but got Unknown
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Expected O, but got Unknown
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Expected O, but got Unknown
			//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0533: Unknown result type (might be due to invalid IL or missing references)
			//IL_0539: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Expected O, but got Unknown
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			int value = ActiveSliceSettings.Instance.GetValue<int>("extruder_count");
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			BorderDouble margin = default(BorderDouble);
			((BorderDouble)(ref margin))._002Ector(4.0, 0.0, 4.0, 0.0);
			if (value == 1)
			{
				ExtrudeButton extrudeButton = CreateExtrudeButton("E-", MovementControls.EFeedRate(0), 0, moveButtonFactory);
				((GuiWidget)extrudeButton).set_Margin(margin);
				((GuiWidget)extrudeButton).set_ToolTipText("Retract filament".Localize());
				((GuiWidget)val2).AddChild((GuiWidget)(object)extrudeButton, -1);
				eMinusButtons.Add(extrudeButton);
			}
			else
			{
				for (int i = 0; i < value; i++)
				{
					ExtrudeButton extrudeButton2 = CreateExtrudeButton($"E{i + 1}-", MovementControls.EFeedRate(0), i, moveButtonFactory);
					((GuiWidget)extrudeButton2).set_ToolTipText("Retract filament".Localize());
					((GuiWidget)extrudeButton2).set_Margin(margin);
					((GuiWidget)val2).AddChild((GuiWidget)(object)extrudeButton2, -1);
					eMinusButtons.Add(extrudeButton2);
				}
			}
			TextWidget val3 = new TextWidget("Retract".Localize(), 0.0, 0.0, 11.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val3.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val3).set_VAnchor((VAnchor)2);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val2).set_HAnchor((HAnchor)8);
			((GuiWidget)val2).set_VAnchor((VAnchor)8);
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)0);
			for (int j = 0; j < value; j++)
			{
				GuiWidget val5 = new GuiWidget(2.0, buttonSeparationDistance, (SizeLimitsToSet)1);
				double num = ((GuiWidget)eMinusButtons[j]).get_Width() + 6.0;
				val5.set_Margin(new BorderDouble(num / 2.0, 0.0, num / 2.0, 0.0));
				val5.set_BackgroundColor(XYZColors.eColor);
				((GuiWidget)val4).AddChild(val5, -1);
			}
			((GuiWidget)val).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)val4).set_HAnchor((HAnchor)8);
			((GuiWidget)val4).set_VAnchor((VAnchor)8);
			FlowLayoutWidget val6 = new FlowLayoutWidget((FlowDirection)0);
			if (value == 1)
			{
				ExtrudeButton extrudeButton3 = CreateExtrudeButton("E+", MovementControls.EFeedRate(0), 0, moveButtonFactory);
				((GuiWidget)extrudeButton3).set_Margin(margin);
				((GuiWidget)extrudeButton3).set_ToolTipText("Extrude filament".Localize());
				((GuiWidget)val6).AddChild((GuiWidget)(object)extrudeButton3, -1);
				ePlusButtons.Add(extrudeButton3);
			}
			else
			{
				for (int k = 0; k < value; k++)
				{
					ExtrudeButton extrudeButton4 = CreateExtrudeButton($"E{k + 1}+", MovementControls.EFeedRate(0), k, moveButtonFactory);
					((GuiWidget)extrudeButton4).set_Margin(margin);
					((GuiWidget)extrudeButton4).set_ToolTipText("Extrude filament".Localize());
					((GuiWidget)val6).AddChild((GuiWidget)(object)extrudeButton4, -1);
					ePlusButtons.Add(extrudeButton4);
				}
			}
			TextWidget val7 = new TextWidget("Extrude".Localize(), 0.0, 0.0, 11.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val7.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val7).set_VAnchor((VAnchor)2);
			((GuiWidget)val6).AddChild((GuiWidget)(object)val7, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val6, -1);
			((GuiWidget)val6).set_HAnchor((HAnchor)8);
			((GuiWidget)val6).set_VAnchor((VAnchor)8);
			((GuiWidget)val).AddChild(new GuiWidget(10.0, 6.0, (SizeLimitsToSet)1), -1);
			FlowLayoutWidget val8 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)new TextWidget("Distance:", 0.0, 0.0, 12.0, (Justification)0, RGBA_Bytes.White, true, false, default(RGBA_Bytes), (TypeFace)null)).set_VAnchor((VAnchor)2);
			TextImageButtonFactory obj = new TextImageButtonFactory
			{
				FixedHeight = 20.0 * GuiWidget.get_DeviceScale(),
				FixedWidth = 30.0 * GuiWidget.get_DeviceScale(),
				fontSize = 8.0,
				Margin = BorderDouble.op_Implicit(0),
				checkedBorderColor = ActiveTheme.get_Instance().get_PrimaryTextColor()
			};
			FlowLayoutWidget val9 = new FlowLayoutWidget((FlowDirection)0);
			RadioButton val10 = obj.GenerateRadioButton("1");
			((GuiWidget)val10).set_VAnchor((VAnchor)2);
			val10.add_CheckedStateChanged((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				if (((RadioButton)sender).get_Checked())
				{
					SetEMoveAmount(1);
				}
			});
			((GuiWidget)val9).AddChild((GuiWidget)(object)val10, -1);
			RadioButton val11 = obj.GenerateRadioButton("10");
			((GuiWidget)val11).set_VAnchor((VAnchor)2);
			val11.add_CheckedStateChanged((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				if (((RadioButton)sender).get_Checked())
				{
					SetEMoveAmount(10);
				}
			});
			((GuiWidget)val9).AddChild((GuiWidget)(object)val11, -1);
			RadioButton val12 = obj.GenerateRadioButton("100");
			((GuiWidget)val12).set_VAnchor((VAnchor)2);
			val12.add_CheckedStateChanged((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				if (((RadioButton)sender).get_Checked())
				{
					SetEMoveAmount(100);
				}
			});
			((GuiWidget)val9).AddChild((GuiWidget)(object)val12, -1);
			val11.set_Checked(true);
			((GuiWidget)val9).set_Margin(new BorderDouble(0.0, 3.0));
			((GuiWidget)val8).AddChild((GuiWidget)(object)val9, -1);
			TextWidget val13 = new TextWidget("mm", 0.0, 0.0, 8.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val13).set_VAnchor((VAnchor)2);
			((GuiWidget)val13).set_Margin(new BorderDouble(10.0, 0.0, 0.0, 0.0));
			((GuiWidget)val8).AddChild((GuiWidget)(object)val13, -1);
			((GuiWidget)val8).set_HAnchor((HAnchor)1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val8, -1);
			((GuiWidget)val).set_HAnchor((HAnchor)8);
			((GuiWidget)val).set_VAnchor((VAnchor)9);
			return val;
		}

		private static MoveButton CreateMoveButton(string label, PrinterConnectionAndCommunication.Axis axis, double moveSpeed, bool levelingButtons, MoveButtonFactory buttonFactory)
		{
			MoveButton moveButton = buttonFactory.GenerateMoveButton(label, axis, moveSpeed);
			((GuiWidget)moveButton).set_VAnchor((VAnchor)0);
			((GuiWidget)moveButton).set_HAnchor((HAnchor)0);
			((GuiWidget)moveButton).set_Height((double)(levelingButtons ? 45 : 40) * GuiWidget.get_DeviceScale());
			((GuiWidget)moveButton).set_Width((double)(levelingButtons ? 90 : 40) * GuiWidget.get_DeviceScale());
			return moveButton;
		}

		private static ExtrudeButton CreateExtrudeButton(string label, double moveSpeed, int extruderNumber, MoveButtonFactory buttonFactory = null)
		{
			ExtrudeButton extrudeButton = buttonFactory.GenerateExtrudeButton(label, moveSpeed, extruderNumber);
			((GuiWidget)extrudeButton).set_Height(40.0 * GuiWidget.get_DeviceScale());
			((GuiWidget)extrudeButton).set_Width(40.0 * GuiWidget.get_DeviceScale());
			return extrudeButton;
		}

		public static FlowLayoutWidget CreateZButtons(RGBA_Bytes color, double buttonSeparationDistance, out MoveButton zPlusControl, out MoveButton zMinusControl, bool levelingButtons = false)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Expected O, but got Unknown
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			MoveButtonFactory buttonFactory = new MoveButtonFactory
			{
				Colors = 
				{
					Fill = 
					{
						Normal = color
					}
				}
			};
			zPlusControl = CreateMoveButton("Z+", PrinterConnectionAndCommunication.Axis.Z, MovementControls.ZSpeed, levelingButtons, buttonFactory);
			((GuiWidget)zPlusControl).set_Name("Move Z positive".Localize());
			((GuiWidget)zPlusControl).set_ToolTipText("Move Z positive".Localize());
			((GuiWidget)val).AddChild((GuiWidget)(object)zPlusControl, -1);
			GuiWidget val2 = new GuiWidget(2.0, buttonSeparationDistance, (SizeLimitsToSet)1);
			val2.set_HAnchor((HAnchor)2);
			val2.set_BackgroundColor(XYZColors.zColor);
			((GuiWidget)val).AddChild(val2, -1);
			zMinusControl = CreateMoveButton("Z-", PrinterConnectionAndCommunication.Axis.Z, MovementControls.ZSpeed, levelingButtons, buttonFactory);
			((GuiWidget)zMinusControl).set_ToolTipText("Move Z negative".Localize());
			((GuiWidget)val).AddChild((GuiWidget)(object)zMinusControl, -1);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 5.0));
			return val;
		}

		private GuiWidget CreateXYGridControl(XYZColors colors, double distanceBetweenControls, double buttonSeparationDistance)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Expected O, but got Unknown
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Expected O, but got Unknown
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Expected O, but got Unknown
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Expected O, but got Unknown
			GuiWidget val = new GuiWidget();
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			moveButtonFactory.Colors.Fill.Normal = XYZColors.xColor;
			((GuiWidget)val2).set_HAnchor((HAnchor)(((GuiWidget)val2).get_HAnchor() | 2));
			((GuiWidget)val2).set_VAnchor((VAnchor)(((GuiWidget)val2).get_VAnchor() | 2));
			xMinusControl = CreateMoveButton("X-", PrinterConnectionAndCommunication.Axis.X, MovementControls.XSpeed, levelingButtons: false, moveButtonFactory);
			((GuiWidget)xMinusControl).set_ToolTipText("Move X negative".Localize());
			((GuiWidget)val2).AddChild((GuiWidget)(object)xMinusControl, -1);
			GuiWidget val3 = new GuiWidget(((GuiWidget)xMinusControl).get_Width() + buttonSeparationDistance * 2.0, 2.0, (SizeLimitsToSet)1);
			val3.set_VAnchor((VAnchor)2);
			val3.set_BackgroundColor(XYZColors.xColor);
			((GuiWidget)val2).AddChild(val3, -1);
			xPlusControl = CreateMoveButton("X+", PrinterConnectionAndCommunication.Axis.X, MovementControls.XSpeed, levelingButtons: false, moveButtonFactory);
			((GuiWidget)xPlusControl).set_ToolTipText("Move X positive".Localize());
			((GuiWidget)val2).AddChild((GuiWidget)(object)xPlusControl, -1);
			val.AddChild((GuiWidget)(object)val2, -1);
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)3);
			moveButtonFactory.Colors.Fill.Normal = XYZColors.yColor;
			((GuiWidget)val4).set_HAnchor((HAnchor)(((GuiWidget)val4).get_HAnchor() | 2));
			((GuiWidget)val4).set_VAnchor((VAnchor)(((GuiWidget)val4).get_VAnchor() | 2));
			yPlusControl = CreateMoveButton("Y+", PrinterConnectionAndCommunication.Axis.Y, MovementControls.YSpeed, levelingButtons: false, moveButtonFactory);
			((GuiWidget)yPlusControl).set_ToolTipText("Move Y positive".Localize());
			((GuiWidget)val4).AddChild((GuiWidget)(object)yPlusControl, -1);
			GuiWidget val5 = new GuiWidget(2.0, buttonSeparationDistance, (SizeLimitsToSet)1);
			val5.set_HAnchor((HAnchor)2);
			val5.set_BackgroundColor(XYZColors.yColor);
			((GuiWidget)val4).AddChild(val5, -1);
			yMinusControl = CreateMoveButton("Y-", PrinterConnectionAndCommunication.Axis.Y, MovementControls.YSpeed, levelingButtons: false, moveButtonFactory);
			((GuiWidget)yMinusControl).set_ToolTipText("Move Y negative".Localize());
			((GuiWidget)val4).AddChild((GuiWidget)(object)yMinusControl, -1);
			val.AddChild((GuiWidget)(object)val4, -1);
			val.set_HAnchor((HAnchor)8);
			val.set_VAnchor((VAnchor)8);
			val.set_VAnchor((VAnchor)1);
			val.set_Margin(new BorderDouble(0.0, 5.0, distanceBetweenControls, 5.0));
			return val;
		}
	}
}
