using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl
{
	public class SplitButtonFactory
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

		public bool invertImageLocation;

		public bool AllowThemeToAdjustImage = true;

		private string imageName;

		public double FixedHeight = 30.0 * GuiWidget.get_DeviceScale();

		public SplitButton Generate(TupleList<string, Func<bool>> buttonList, Direction direction = 1, string imageName = null)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			this.imageName = imageName;
			DynamicDropDownMenu dynamicDropDownMenu = CreateMenu(direction);
			if (buttonList.Count > 1)
			{
				((GuiWidget)dynamicDropDownMenu).set_Name(buttonList[1].Item1 + " Menu");
			}
			((GuiWidget)dynamicDropDownMenu).set_Margin(default(BorderDouble));
			Button button = CreateButton(buttonList[0]);
			for (int i = 1; i < buttonList.Count; i++)
			{
				dynamicDropDownMenu.addItem(buttonList[i].Item1, buttonList[i].Item2);
			}
			return new SplitButton(button, dynamicDropDownMenu);
		}

		private Button CreateButton(Tuple<string, Func<bool>> buttonInfo)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			Button obj = new TextImageButtonFactory
			{
				FixedHeight = FixedHeight,
				normalFillColor = normalFillColor,
				normalTextColor = normalTextColor,
				hoverTextColor = hoverTextColor,
				hoverFillColor = hoverFillColor,
				borderWidth = 1.0,
				normalBorderColor = normalBorderColor,
				hoverBorderColor = hoverBorderColor
			}.Generate(buttonInfo.Item1, imageName, null, null, null, centerText: true);
			((GuiWidget)obj).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				buttonInfo.Item2();
			});
			return obj;
		}

		private DynamicDropDownMenu CreateMenu(Direction direction = 1)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			DropDownMenuFactory dropDownMenuFactory = new DropDownMenuFactory();
			dropDownMenuFactory.normalFillColor = normalFillColor;
			dropDownMenuFactory.hoverFillColor = hoverFillColor;
			dropDownMenuFactory.pressedFillColor = pressedFillColor;
			dropDownMenuFactory.pressedFillColor = pressedFillColor;
			dropDownMenuFactory.normalBorderColor = normalBorderColor;
			dropDownMenuFactory.hoverBorderColor = hoverBorderColor;
			dropDownMenuFactory.pressedBorderColor = pressedBorderColor;
			dropDownMenuFactory.disabledBorderColor = disabledBorderColor;
			dropDownMenuFactory.normalTextColor = normalTextColor;
			dropDownMenuFactory.hoverTextColor = hoverTextColor;
			dropDownMenuFactory.pressedTextColor = pressedTextColor;
			dropDownMenuFactory.disabledTextColor = disabledTextColor;
			DynamicDropDownMenu dynamicDropDownMenu = dropDownMenuFactory.Generate("", null, direction);
			((GuiWidget)dynamicDropDownMenu).set_Height(FixedHeight);
			dynamicDropDownMenu.BorderColor = normalBorderColor;
			dynamicDropDownMenu.HoverArrowColor = hoverTextColor;
			dynamicDropDownMenu.NormalArrowColor = normalTextColor;
			((GuiWidget)dynamicDropDownMenu).set_BackgroundColor(normalFillColor);
			return dynamicDropDownMenu;
		}
	}
}
