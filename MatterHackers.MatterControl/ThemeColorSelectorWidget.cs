using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl
{
	public class ThemeColorSelectorWidget : FlowLayoutWidget
	{
		private GuiWidget colorToChangeTo;

		private int containerHeight = (int)(34.0 * GuiWidget.get_DeviceScale() + 0.5);

		private int colorSelectSize = (int)(32.0 * GuiWidget.get_DeviceScale() + 0.5);

		public ThemeColorSelectorWidget(GuiWidget colorToChangeTo)
			: this((FlowDirection)0)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Expected O, but got Unknown
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_Padding(new BorderDouble(2.0, 0.0));
			this.colorToChangeTo = colorToChangeTo;
			int count = ActiveTheme.get_AvailableThemes().Count;
			List<IThemeColors> availableThemes = ActiveTheme.get_AvailableThemes();
			int num = 0;
			for (int i = 0; i < count / 2; i++)
			{
				FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
				((GuiWidget)val).set_Width((double)containerHeight);
				FlowLayoutWidget val2 = val;
				((GuiWidget)val2).AddChild((GuiWidget)(object)CreateThemeButton(availableThemes[num]), -1);
				int index = num + count / 2;
				((GuiWidget)val2).AddChild((GuiWidget)(object)CreateThemeButton(availableThemes[index]), -1);
				((GuiWidget)this).AddChild((GuiWidget)(object)val2, -1);
				num++;
			}
			((GuiWidget)this).set_BackgroundColor(RGBA_Bytes.White);
			((GuiWidget)this).set_Width((double)(containerHeight * (count / 2)));
		}

		public Button CreateThemeButton(IThemeColors theme)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Expected O, but got Unknown
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Expected O, but got Unknown
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Expected O, but got Unknown
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Expected O, but got Unknown
			GuiWidget val = new GuiWidget((double)colorSelectSize, (double)colorSelectSize, (SizeLimitsToSet)1);
			val.set_BackgroundColor(theme.get_PrimaryAccentColor());
			GuiWidget val2 = new GuiWidget((double)colorSelectSize, (double)colorSelectSize, (SizeLimitsToSet)1);
			val2.set_BackgroundColor(theme.get_SecondaryAccentColor());
			GuiWidget val3 = new GuiWidget((double)colorSelectSize, (double)colorSelectSize, (SizeLimitsToSet)1);
			val3.set_BackgroundColor(theme.get_SecondaryAccentColor());
			GuiWidget val4 = new GuiWidget((double)colorSelectSize, (double)colorSelectSize, (SizeLimitsToSet)1);
			Button val5 = new Button(0.0, 0.0, (GuiWidget)new ButtonViewStates(val, val2, val3, val4));
			((GuiWidget)val5).set_Name(theme.get_Name());
			((GuiWidget)val5).add_Click((EventHandler<MouseEventArgs>)delegate(object s, MouseEventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				string name = ((GuiWidget)s).get_Name();
				ActiveSliceSettings.Instance.SetValue("active_theme_name", name);
				ActiveTheme.set_Instance(ActiveTheme.GetThemeColors(name));
			});
			((GuiWidget)val5).add_MouseEnterBounds((EventHandler)delegate
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				colorToChangeTo.set_BackgroundColor(theme.get_PrimaryAccentColor());
			});
			((GuiWidget)val5).add_MouseLeaveBounds((EventHandler)delegate
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				colorToChangeTo.set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			});
			return val5;
		}
	}
}
