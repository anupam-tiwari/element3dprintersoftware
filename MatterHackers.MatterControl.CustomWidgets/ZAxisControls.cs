using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.PrinterControls;

namespace MatterHackers.MatterControl.CustomWidgets
{
	public class ZAxisControls : FlowLayoutWidget
	{
		private JogControls.MoveButtonFactory buttonFactory = new JogControls.MoveButtonFactory
		{
			FontSize = 13.0
		};

		public ZAxisControls(bool smallScreen)
			: this((FlowDirection)3)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Expected O, but got Unknown
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Expected O, but got Unknown
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			buttonFactory.Colors.Fill.Normal = ActiveTheme.get_Instance().get_PrimaryAccentColor();
			buttonFactory.Colors.Fill.Hover = ActiveTheme.get_Instance().get_PrimaryAccentColor();
			buttonFactory.BorderWidth = 0.0;
			buttonFactory.Colors.Text.Normal = RGBA_Bytes.White;
			TextWidget val = new TextWidget("Z+", 0.0, 0.0, (double)(smallScreen ? 12 : 15), (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			val.set_AutoExpandBoundsToText(true);
			((GuiWidget)val).set_HAnchor((HAnchor)2);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 8.0, 0.0, 0.0));
			((GuiWidget)this).AddChild((GuiWidget)val, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)CreateZMoveButton(0.1, smallScreen), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)CreateZMoveButton(0.02, smallScreen), -1);
			ZTuningWidget zTuningWidget = new ZTuningWidget(allowRemoveButton: false);
			((GuiWidget)zTuningWidget).set_HAnchor((HAnchor)10);
			((GuiWidget)zTuningWidget).set_Margin(BorderDouble.op_Implicit(10));
			((GuiWidget)this).AddChild((GuiWidget)(object)zTuningWidget, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)CreateZMoveButton(-0.02, smallScreen), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)CreateZMoveButton(-0.1, smallScreen), -1);
			TextWidget val2 = new TextWidget("Z-", 0.0, 0.0, (double)(smallScreen ? 12 : 15), (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_AutoExpandBoundsToText(true);
			((GuiWidget)val2).set_HAnchor((HAnchor)2);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 9.0));
			((GuiWidget)this).AddChild((GuiWidget)val2, -1);
			((GuiWidget)this).set_Margin(new BorderDouble(0.0));
			((GuiWidget)this).set_Margin(BorderDouble.op_Implicit(0));
			((GuiWidget)this).set_Padding(BorderDouble.op_Implicit(3));
			((GuiWidget)this).set_VAnchor((VAnchor)12);
		}

		private Button CreateZMoveButton(double moveAmount, bool smallScreen)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			JogControls.MoveButton moveButton = buttonFactory.GenerateMoveButton($"{Math.Abs(moveAmount):0.00} mm", PrinterConnectionAndCommunication.Axis.Z, MovementControls.ZSpeed);
			moveButton.MoveAmount = moveAmount;
			((GuiWidget)moveButton).set_HAnchor((HAnchor)13);
			((GuiWidget)moveButton).set_VAnchor((VAnchor)8);
			((GuiWidget)moveButton).set_Margin(new BorderDouble(0.0, 1.0));
			((GuiWidget)moveButton).set_Padding(new BorderDouble(15.0, 7.0));
			if (smallScreen)
			{
				((GuiWidget)moveButton).set_Height(45.0);
			}
			else
			{
				((GuiWidget)moveButton).set_Height(55.0);
			}
			((GuiWidget)moveButton).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			return (Button)(object)moveButton;
		}
	}
}
