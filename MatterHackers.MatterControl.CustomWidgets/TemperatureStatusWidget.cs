using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.CustomWidgets
{
	public abstract class TemperatureStatusWidget : FlowLayoutWidget
	{
		protected TextWidget actualTemp;

		protected ProgressBar progressBar;

		protected TextWidget targetTemp;

		protected EventHandler unregisterEvents;

		private int fontSize = 14;

		public TemperatureStatusWidget(string dispalyName)
			: this((FlowDirection)0)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Expected O, but got Unknown
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Expected O, but got Unknown
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Expected O, but got Unknown
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Expected O, but got Unknown
			TextWidget val = new TextWidget(dispalyName, 0.0, 0.0, (double)fontSize, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			val.set_AutoExpandBoundsToText(true);
			((GuiWidget)val).set_VAnchor((VAnchor)2);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 8.0, 0.0));
			TextWidget val2 = val;
			((GuiWidget)this).AddChild((GuiWidget)(object)val2, -1);
			ProgressBar val3 = new ProgressBar(200, 6);
			val3.set_FillColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 0.0, 10.0, 0.0));
			val3.set_BorderColor(RGBA_Bytes.Transparent);
			((GuiWidget)val3).set_BackgroundColor(new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 50));
			((GuiWidget)val3).set_VAnchor((VAnchor)2);
			progressBar = val3;
			((GuiWidget)this).AddChild((GuiWidget)(object)progressBar, -1);
			TextWidget val4 = new TextWidget("", 0.0, 0.0, (double)fontSize, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			val4.set_AutoExpandBoundsToText(true);
			((GuiWidget)val4).set_VAnchor((VAnchor)2);
			((GuiWidget)val4).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 0.0));
			((GuiWidget)val4).set_Width(60.0);
			actualTemp = val4;
			((GuiWidget)this).AddChild((GuiWidget)(object)actualTemp, -1);
			VerticalLine verticalLine = new VerticalLine();
			((GuiWidget)verticalLine).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)verticalLine).set_Margin(new BorderDouble(8.0, 0.0));
			((GuiWidget)this).AddChild((GuiWidget)(object)verticalLine, -1);
			TextWidget val5 = new TextWidget("", 0.0, 0.0, (double)fontSize, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			val5.set_AutoExpandBoundsToText(true);
			((GuiWidget)val5).set_VAnchor((VAnchor)2);
			((GuiWidget)val5).set_Margin(new BorderDouble(0.0, 0.0, 8.0, 0.0));
			((GuiWidget)val5).set_Width(60.0);
			targetTemp = val5;
			((GuiWidget)this).AddChild((GuiWidget)(object)targetTemp, -1);
			UiThread.RunOnIdle((Action)UpdateTemperatures);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
		}

		public abstract void UpdateTemperatures();
	}
}
