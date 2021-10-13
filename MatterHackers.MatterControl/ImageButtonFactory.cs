using MatterHackers.Agg;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;

namespace MatterHackers.MatterControl
{
	public class ImageButtonFactory
	{
		public bool InvertImageColor
		{
			get;
			set;
		} = true;


		public static CheckBox CreateToggleSwitch(bool initialState)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Expected O, but got Unknown
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Expected O, but got Unknown
			string text = "On";
			string text2 = "Off";
			if (StaticData.get_Instance() != null)
			{
				text = text.Localize();
				text2 = text2.Localize();
			}
			CheckBox val = new CheckBox((GuiWidget)new ToggleSwitchView(text, text2, 60.0 * GuiWidget.get_DeviceScale(), 24.0 * GuiWidget.get_DeviceScale(), ActiveTheme.get_Instance().get_PrimaryBackgroundColor(), new RGBA_Bytes(220, 220, 220), ActiveTheme.get_Instance().get_PrimaryAccentColor(), ActiveTheme.get_Instance().get_PrimaryTextColor()));
			val.set_Checked(initialState);
			return val;
		}

		public Button Generate(string normalImageName, string hoverImageName, string pressedImageName = null, string disabledImageName = null)
		{
			if (hoverImageName == null)
			{
				hoverImageName = normalImageName;
			}
			if (pressedImageName == null)
			{
				pressedImageName = hoverImageName;
			}
			if (disabledImageName == null)
			{
				disabledImageName = normalImageName;
			}
			ImageBuffer normalImage = StaticData.get_Instance().LoadIcon(normalImageName);
			ImageBuffer hoverImage = StaticData.get_Instance().LoadIcon(pressedImageName);
			ImageBuffer pressedImage = StaticData.get_Instance().LoadIcon(hoverImageName);
			ImageBuffer disabledImage = StaticData.get_Instance().LoadIcon(disabledImageName);
			return Generate(normalImage, hoverImage, pressedImage, disabledImage);
		}

		public Button Generate(ImageBuffer normalImage, ImageBuffer hoverImage, ImageBuffer pressedImage = null, ImageBuffer disabledImage = null)
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Expected O, but got Unknown
			//IL_0074: Expected O, but got Unknown
			//IL_0074: Expected O, but got Unknown
			//IL_0074: Expected O, but got Unknown
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Expected O, but got Unknown
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Expected O, but got Unknown
			if (hoverImage == (ImageBuffer)null)
			{
				hoverImage = normalImage;
			}
			if (pressedImage == (ImageBuffer)null)
			{
				pressedImage = hoverImage;
			}
			if (disabledImage == (ImageBuffer)null)
			{
				disabledImage = normalImage;
			}
			if (!ActiveTheme.get_Instance().get_IsDarkTheme() && InvertImageColor)
			{
				ExtensionMethods.InvertLightness(normalImage);
				ExtensionMethods.InvertLightness(pressedImage);
				ExtensionMethods.InvertLightness(hoverImage);
				ExtensionMethods.InvertLightness(disabledImage);
			}
			ButtonViewStates val = new ButtonViewStates((GuiWidget)new ImageWidget(normalImage), (GuiWidget)new ImageWidget(hoverImage), (GuiWidget)new ImageWidget(pressedImage), (GuiWidget)new ImageWidget(disabledImage));
			Button val2 = new Button(0.0, 0.0, (GuiWidget)(object)val);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0));
			((GuiWidget)val2).set_Padding(new BorderDouble(0.0));
			return val2;
		}
	}
}
