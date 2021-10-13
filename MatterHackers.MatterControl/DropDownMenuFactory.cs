using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl
{
	public class DropDownMenuFactory
	{
		public BorderDouble Margin = new BorderDouble(0.0, 0.0);

		public RGBA_Bytes normalFillColor = new RGBA_Bytes(0, 0, 0, 0);

		public RGBA_Bytes hoverFillColor = new RGBA_Bytes(0, 0, 0, 50);

		public RGBA_Bytes pressedFillColor = new RGBA_Bytes(0, 0, 0, 0);

		public RGBA_Bytes disabledFillColor = new RGBA_Bytes(255, 255, 255, 50);

		public RGBA_Bytes normalBorderColor = new RGBA_Bytes(255, 255, 255, 0);

		public RGBA_Bytes hoverBorderColor = new RGBA_Bytes(0, 0, 0, 0);

		public RGBA_Bytes pressedBorderColor = new RGBA_Bytes(0, 0, 0, 0);

		public RGBA_Bytes disabledBorderColor = new RGBA_Bytes(0, 0, 0, 0);

		public RGBA_Bytes checkedBorderColor = new RGBA_Bytes(255, 255, 255, 0);

		public RGBA_Bytes normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();

		public RGBA_Bytes hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();

		public RGBA_Bytes pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();

		public RGBA_Bytes disabledTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();

		public int fontSize = 12;

		public double borderWidth = 1.0;

		public double FixedWidth = 10.0;

		public double FixedHeight = 30.0;

		public DynamicDropDownMenu Generate(string label = "", TupleList<string, Func<bool>> optionList = null, Direction direction = 1)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			DynamicDropDownMenu dynamicDropDownMenu = new DynamicDropDownMenu((GuiWidget)(object)CreateButtonViewStates(label), direction);
			((GuiWidget)dynamicDropDownMenu).set_VAnchor((VAnchor)2);
			((GuiWidget)dynamicDropDownMenu).set_HAnchor((HAnchor)8);
			dynamicDropDownMenu.MenuAsWideAsItems = false;
			((Menu)dynamicDropDownMenu).set_AlignToRightEdge(true);
			dynamicDropDownMenu.NormalColor = normalFillColor;
			dynamicDropDownMenu.HoverColor = hoverFillColor;
			dynamicDropDownMenu.BorderColor = normalBorderColor;
			((GuiWidget)dynamicDropDownMenu).set_BackgroundColor(dynamicDropDownMenu.NormalColor);
			if (optionList != null)
			{
				foreach (Tuple<string, Func<bool>> option in optionList)
				{
					dynamicDropDownMenu.addItem(option.Item1, option.Item2);
				}
				return dynamicDropDownMenu;
			}
			return dynamicDropDownMenu;
		}

		private ButtonViewStates CreateButtonViewStates(string label)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Expected O, but got Unknown
			ButtonViewStates val = new ButtonViewStates((GuiWidget)(object)new DropDownButtonBase(label, normalFillColor, normalBorderColor, normalTextColor, borderWidth, Margin, fontSize, (FlowDirection)0, FixedHeight), (GuiWidget)(object)new DropDownButtonBase(label, hoverFillColor, hoverBorderColor, hoverTextColor, borderWidth, Margin, fontSize, (FlowDirection)0, FixedHeight), (GuiWidget)(object)new DropDownButtonBase(label, pressedFillColor, pressedBorderColor, pressedTextColor, borderWidth, Margin, fontSize, (FlowDirection)0, FixedHeight), (GuiWidget)(object)new DropDownButtonBase(label, disabledFillColor, disabledBorderColor, disabledTextColor, borderWidth, Margin, fontSize, (FlowDirection)0, FixedHeight));
			((GuiWidget)val).set_Padding(new BorderDouble(0.0, 0.0));
			return val;
		}
	}
}
