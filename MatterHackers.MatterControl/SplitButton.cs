using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl
{
	public class SplitButton : FlowLayoutWidget
	{
		private Button defaultButton;

		private DynamicDropDownMenu altChoices;

		private Button DefaultButton => defaultButton;

		public SplitButton(string buttonText, Direction direction = 1)
			: this((FlowDirection)0)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_HAnchor((HAnchor)8);
			((GuiWidget)this).set_VAnchor((VAnchor)8);
			defaultButton = CreateDefaultButton(buttonText);
			altChoices = CreateDropDown(direction);
			((GuiWidget)defaultButton).set_VAnchor((VAnchor)2);
			((GuiWidget)this).AddChild((GuiWidget)(object)defaultButton, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)altChoices, -1);
		}

		public SplitButton(Button button, DynamicDropDownMenu menu)
			: this((FlowDirection)0)
		{
			((GuiWidget)this).set_HAnchor((HAnchor)8);
			((GuiWidget)this).set_VAnchor((VAnchor)8);
			defaultButton = button;
			altChoices = menu;
			((GuiWidget)defaultButton).set_VAnchor((VAnchor)2);
			((GuiWidget)this).AddChild((GuiWidget)(object)defaultButton, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)altChoices, -1);
		}

		public void AddItem(string name, Func<bool> clickFunction)
		{
			altChoices.addItem(name, clickFunction);
		}

		private DynamicDropDownMenu CreateDropDown(Direction direction)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			DynamicDropDownMenu dynamicDropDownMenu = new DynamicDropDownMenu("", direction);
			((GuiWidget)dynamicDropDownMenu).set_VAnchor((VAnchor)2);
			dynamicDropDownMenu.MenuAsWideAsItems = false;
			((Menu)dynamicDropDownMenu).set_AlignToRightEdge(true);
			((GuiWidget)dynamicDropDownMenu).set_Height(((GuiWidget)defaultButton).get_Height());
			return dynamicDropDownMenu;
		}

		private Button CreateDefaultButton(string buttonText)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			return new TextImageButtonFactory
			{
				FixedHeight = 30.0 * GuiWidget.get_DeviceScale(),
				normalFillColor = RGBA_Bytes.White,
				normalTextColor = RGBA_Bytes.Black,
				hoverTextColor = RGBA_Bytes.Black,
				hoverFillColor = new RGBA_Bytes(255, 255, 255, 200),
				borderWidth = 1.0,
				normalBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200),
				hoverBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200)
			}.Generate(buttonText, (string)null, (string)null, (string)null, (string)null, centerText: true);
		}
	}
}
