using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl
{
	public class LinkButtonFactory
	{
		public double fontSize = 14.0;

		public double padding = 3.0;

		public RGBA_Bytes fillColor = new RGBA_Bytes(63, 63, 70, 0);

		public RGBA_Bytes borderColor = new RGBA_Bytes(37, 37, 38, 0);

		public RGBA_Bytes textColor = ActiveTheme.get_Instance().get_PrimaryAccentColor();

		public BorderDouble margin = new BorderDouble(0.0, 3.0);

		public Button Generate(string buttonText)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected O, but got Unknown
			LinkButtonViewBase buttonWidgetPressed = getButtonWidgetPressed(buttonText);
			LinkButtonViewBase buttonWidgetHover = getButtonWidgetHover(buttonText);
			LinkButtonViewBase buttonWidgetNormal = getButtonWidgetNormal(buttonText);
			LinkButtonViewBase buttonWidgetDisabled = getButtonWidgetDisabled(buttonText);
			ButtonViewStates val = new ButtonViewStates((GuiWidget)(object)buttonWidgetNormal, (GuiWidget)(object)buttonWidgetHover, (GuiWidget)(object)buttonWidgetPressed, (GuiWidget)(object)buttonWidgetDisabled);
			Button val2 = new Button(0.0, 0.0, (GuiWidget)(object)val);
			((GuiWidget)val2).set_Margin(margin);
			((GuiWidget)val2).set_Cursor((Cursors)3);
			return val2;
		}

		private LinkButtonViewBase getButtonWidgetPressed(string buttonText)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			return new LinkButtonViewBase(buttonText, fontSize, padding, textColor);
		}

		private LinkButtonViewBase getButtonWidgetHover(string buttonText)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			return new LinkButtonViewBase(buttonText, fontSize, padding, textColor);
		}

		public LinkButtonViewBase getButtonWidgetNormal(string buttonText)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			return new LinkButtonViewBase(buttonText, fontSize, padding, textColor, isUnderlined: true);
		}

		private LinkButtonViewBase getButtonWidgetDisabled(string buttonText)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			return new LinkButtonViewBase(buttonText, fontSize, padding, textColor);
		}
	}
}
