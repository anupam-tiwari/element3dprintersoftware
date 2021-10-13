using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.ImageProcessing;

namespace MatterHackers.MatterControl
{
	public class TextImageButtonFactory
	{
		public BorderDouble Margin = new BorderDouble(6.0, 0.0);

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

		public double fontSize = 12.0;

		public double borderWidth = 1.0;

		public bool invertImageLocation;

		public bool AllowThemeToAdjustImage = true;

		private FlowDirection flowDirection;

		public double FixedWidth;

		public double FixedHeight = 40.0;

		public double ImageSpacing;

		public Button GenerateTooltipButton(string label, string normalImageName = null, string hoverImageName = null, string pressedImageName = null, string disabledImageName = null)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			ButtonViewStates buttonView = getButtonView(label, normalImageName, hoverImageName, pressedImageName, disabledImageName);
			Button val = new Button(0.0, 0.0, (GuiWidget)(object)buttonView);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0));
			((GuiWidget)val).set_Padding(new BorderDouble(0.0));
			if (FixedWidth != 0.0)
			{
				((GuiWidget)buttonView).set_Width(FixedWidth);
				((GuiWidget)val).set_Width(FixedWidth);
			}
			((GuiWidget)buttonView).set_Height(FixedHeight);
			((GuiWidget)val).set_Height(FixedHeight);
			return val;
		}

		public Button GenerateTooltipButton(string label, ImageBuffer normalImageName, ImageBuffer hoverImageName = null, ImageBuffer pressedImageName = null, ImageBuffer disabledImageName = null)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			ButtonViewStates buttonView = getButtonView(label, normalImageName, hoverImageName, pressedImageName, disabledImageName);
			Button val = new Button(0.0, 0.0, (GuiWidget)(object)buttonView);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0));
			((GuiWidget)val).set_Padding(new BorderDouble(0.0));
			if (FixedWidth != 0.0)
			{
				((GuiWidget)buttonView).set_Width(FixedWidth);
				((GuiWidget)val).set_Width(FixedWidth);
			}
			((GuiWidget)buttonView).set_Height(FixedHeight);
			((GuiWidget)val).set_Height(FixedHeight);
			return val;
		}

		public GuiWidget GenerateGroupBoxLabelWithEdit(TextWidget textWidget, out Button editButton)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			editButton = GetThemedEditButton();
			((GuiWidget)editButton).set_Margin(new BorderDouble(2.0, 2.0, 2.0, 0.0));
			((GuiWidget)editButton).set_VAnchor((VAnchor)1);
			((GuiWidget)textWidget).set_VAnchor((VAnchor)1);
			((GuiWidget)val).AddChild((GuiWidget)(object)textWidget, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)editButton, -1);
			return (GuiWidget)val;
		}

		public static Button GetThemedEditButton()
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Expected O, but got Unknown
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Expected O, but got Unknown
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Expected O, but got Unknown
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Expected O, but got Unknown
			ImageBuffer val = StaticData.get_Instance().LoadIcon("icon_edit.png", 16, 16);
			if (ActiveTheme.get_Instance().get_IsDarkTheme())
			{
				return new Button(0.0, 0.0, (GuiWidget)new ButtonViewThreeImage((IImageByte)(object)SetToColor.CreateSetToColor(val, ColorExtensionMethods.AdjustLightness((IColorType)(object)RGBA_Bytes.White, 0.8).GetAsRGBA_Bytes()), (IImageByte)(object)SetToColor.CreateSetToColor(val, ColorExtensionMethods.AdjustLightness((IColorType)(object)RGBA_Bytes.White, 0.9).GetAsRGBA_Bytes()), (IImageByte)(object)SetToColor.CreateSetToColor(val, ColorExtensionMethods.AdjustLightness((IColorType)(object)RGBA_Bytes.White, 1.0).GetAsRGBA_Bytes())));
			}
			return new Button(0.0, 0.0, (GuiWidget)new ButtonViewThreeImage((IImageByte)(object)SetToColor.CreateSetToColor(val, ColorExtensionMethods.AdjustLightness((IColorType)(object)RGBA_Bytes.White, 0.4).GetAsRGBA_Bytes()), (IImageByte)(object)SetToColor.CreateSetToColor(val, ColorExtensionMethods.AdjustLightness((IColorType)(object)RGBA_Bytes.White, 0.2).GetAsRGBA_Bytes()), (IImageByte)(object)SetToColor.CreateSetToColor(val, ColorExtensionMethods.AdjustLightness((IColorType)(object)RGBA_Bytes.White, 0.0).GetAsRGBA_Bytes())));
		}

		public GuiWidget GenerateGroupBoxLabelWithEdit(string label, out Button editButton)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Expected O, but got Unknown
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			editButton = GetThemedEditButton();
			((GuiWidget)editButton).set_Margin(new BorderDouble(2.0, 2.0, 2.0, 0.0));
			((GuiWidget)editButton).set_VAnchor((VAnchor)1);
			TextWidget val2 = new TextWidget(label, 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val2).set_VAnchor((VAnchor)1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)editButton, -1);
			return (GuiWidget)val;
		}

		public CheckBox GenerateCheckBoxButton(string label, ImageBuffer normalImage, ImageBuffer normalToPressedImage = null, ImageBuffer pressedImage = null, ImageBuffer pressedToNormalImage = null, string pressedLabel = null)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Expected O, but got Unknown
			if (pressedImage == (ImageBuffer)null)
			{
				pressedImage = normalImage;
			}
			if (pressedToNormalImage == (ImageBuffer)null)
			{
				pressedToNormalImage = normalToPressedImage;
			}
			CheckBoxViewStates checkBoxButtonView = getCheckBoxButtonView(label, normalImage, normalToPressedImage, pressedImage, pressedToNormalImage, pressedLabel);
			if (FixedWidth != 0.0)
			{
				((GuiWidget)checkBoxButtonView).set_Width(FixedWidth);
			}
			CheckBox val = new CheckBox(0.0, 0.0, (GuiWidget)(object)checkBoxButtonView);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0));
			((GuiWidget)val).set_Padding(new BorderDouble(0.0));
			return val;
		}

		public CheckBox GenerateCheckBoxButton(string label, string normalImageName = null, string normalToPressedImageName = null, string pressedImageName = null, string pressedToNormalImageName = null, string pressedLabel = null)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Expected O, but got Unknown
			CheckBoxViewStates checkBoxButtonView = getCheckBoxButtonView(label, normalImageName, normalToPressedImageName, pressedImageName, pressedToNormalImageName, pressedLabel);
			if (FixedWidth != 0.0)
			{
				((GuiWidget)checkBoxButtonView).set_Width(FixedWidth);
			}
			CheckBox val = new CheckBox(0.0, 0.0, (GuiWidget)(object)checkBoxButtonView);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0));
			((GuiWidget)val).set_Padding(new BorderDouble(0.0));
			return val;
		}

		public Button Generate(string label, ImageBuffer normalImage, ImageBuffer hoverImage = null, ImageBuffer pressedImage = null, ImageBuffer disabledImage = null, bool centerText = false)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected O, but got Unknown
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			ButtonViewStates buttonView = getButtonView(label, normalImage, hoverImage, pressedImage, disabledImage, centerText);
			Button val = new Button(0.0, 0.0, (GuiWidget)(object)buttonView);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0));
			((GuiWidget)val).set_Padding(new BorderDouble(0.0));
			if (FixedWidth != 0.0)
			{
				((GuiWidget)buttonView).set_Width(FixedWidth);
				((GuiWidget)val).set_Width(FixedWidth);
			}
			return val;
		}

		public Button Generate(string label, string normalImageName = null, string hoverImageName = null, string pressedImageName = null, string disabledImageName = null, bool centerText = false)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected O, but got Unknown
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			ButtonViewStates buttonView = getButtonView(label, normalImageName, hoverImageName, pressedImageName, disabledImageName, centerText);
			Button val = new Button(0.0, 0.0, (GuiWidget)(object)buttonView);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0));
			((GuiWidget)val).set_Padding(new BorderDouble(0.0));
			if (FixedWidth != 0.0)
			{
				((GuiWidget)buttonView).set_Width(FixedWidth);
				((GuiWidget)val).set_Width(FixedWidth);
			}
			return val;
		}

		private ButtonViewStates getButtonView(string label, string normalImageName = null, string hoverImageName = null, string pressedImageName = null, string disabledImageName = null, bool centerText = false)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Expected O, but got Unknown
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			ImageBuffer val = null;
			ImageBuffer val2 = null;
			ImageBuffer val3 = null;
			ImageBuffer val4 = null;
			if (normalImageName != null)
			{
				val = new ImageBuffer();
				StaticData.get_Instance().LoadIcon(normalImageName, val);
			}
			if (hoverImageName != null)
			{
				val3 = new ImageBuffer();
				StaticData.get_Instance().LoadIcon(hoverImageName, val3);
			}
			if (pressedImageName != null)
			{
				val2 = new ImageBuffer();
				StaticData.get_Instance().LoadIcon(pressedImageName, val2);
			}
			if (disabledImageName != null)
			{
				val4 = new ImageBuffer();
				StaticData.get_Instance().LoadIcon(disabledImageName, val4);
			}
			return getButtonView(label, val, val3, val2, val4, centerText);
		}

		private ButtonViewStates getButtonView(string label, ImageBuffer normalImage = null, ImageBuffer hoverImage = null, ImageBuffer pressedImage = null, ImageBuffer disabledImage = null, bool centerText = false)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Expected O, but got Unknown
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Expected O, but got Unknown
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Expected O, but got Unknown
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Expected O, but got Unknown
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Expected O, but got Unknown
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Expected O, but got Unknown
			if (hoverImage == (ImageBuffer)null && normalImage != (ImageBuffer)null)
			{
				hoverImage = new ImageBuffer(normalImage);
			}
			if (pressedImage == (ImageBuffer)null && hoverImage != (ImageBuffer)null)
			{
				pressedImage = new ImageBuffer(hoverImage);
			}
			if (disabledImage == (ImageBuffer)null && normalImage != (ImageBuffer)null)
			{
				disabledImage = ExtensionMethods.Multiply(normalImage, new RGBA_Bytes(255, 255, 255, 150));
			}
			if (ActiveTheme.get_Instance().get_IsDarkTheme() && AllowThemeToAdjustImage)
			{
				if (normalImage != (ImageBuffer)null)
				{
					normalImage = new ImageBuffer(normalImage);
					ExtensionMethods.InvertLightness(normalImage);
				}
				if (pressedImage != (ImageBuffer)null)
				{
					ExtensionMethods.InvertLightness(pressedImage);
					pressedImage = new ImageBuffer(pressedImage);
				}
				if (hoverImage != (ImageBuffer)null)
				{
					hoverImage = new ImageBuffer(hoverImage);
					ExtensionMethods.InvertLightness(hoverImage);
				}
				if (disabledImage != (ImageBuffer)null)
				{
					disabledImage = new ImageBuffer(disabledImage);
					ExtensionMethods.InvertLightness(disabledImage);
				}
			}
			if (invertImageLocation)
			{
				flowDirection = (FlowDirection)2;
			}
			else
			{
				flowDirection = (FlowDirection)0;
			}
			return new ButtonViewStates((GuiWidget)(object)new TextImageWidget(label, normalFillColor, normalBorderColor, normalTextColor, borderWidth, Margin, normalImage, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight, width: 0.0, centerText: centerText, imageSpacing: ImageSpacing), (GuiWidget)(object)new TextImageWidget(label, hoverFillColor, hoverBorderColor, hoverTextColor, borderWidth, Margin, hoverImage, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight, width: 0.0, centerText: centerText, imageSpacing: ImageSpacing), (GuiWidget)(object)new TextImageWidget(label, pressedFillColor, pressedBorderColor, pressedTextColor, borderWidth, Margin, pressedImage, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight, width: 0.0, centerText: centerText, imageSpacing: ImageSpacing), (GuiWidget)(object)new TextImageWidget(label, disabledFillColor, disabledBorderColor, disabledTextColor, borderWidth, Margin, disabledImage, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight, width: 0.0, centerText: centerText, imageSpacing: ImageSpacing));
		}

		private CheckBoxViewStates getCheckBoxButtonView(string label, string normalImageName = null, string normalToPressedImageName = null, string pressedImageName = null, string pressedToNormalImageName = null, string pressedLabel = null)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected O, but got Unknown
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Expected O, but got Unknown
			ImageBuffer val = new ImageBuffer();
			ImageBuffer val2 = new ImageBuffer();
			ImageBuffer val3 = new ImageBuffer();
			ImageBuffer val4 = new ImageBuffer();
			string text = pressedLabel;
			if (pressedLabel == null)
			{
				text = label;
			}
			if (normalToPressedImageName == null)
			{
				normalToPressedImageName = pressedImageName;
			}
			if (pressedImageName == null)
			{
				pressedImageName = normalToPressedImageName;
			}
			if (pressedToNormalImageName == null)
			{
				pressedToNormalImageName = normalImageName;
			}
			if (normalImageName != null)
			{
				StaticData.get_Instance().LoadIcon(normalImageName, val);
				if (!ActiveTheme.get_Instance().get_IsDarkTheme() && AllowThemeToAdjustImage)
				{
					ExtensionMethods.InvertLightness(val);
				}
			}
			if (pressedImageName != null)
			{
				StaticData.get_Instance().LoadIcon(pressedImageName, val2);
				if (!ActiveTheme.get_Instance().get_IsDarkTheme() && AllowThemeToAdjustImage)
				{
					ExtensionMethods.InvertLightness(val2);
				}
			}
			if (normalToPressedImageName != null)
			{
				StaticData.get_Instance().LoadIcon(normalToPressedImageName, val3);
				if (!ActiveTheme.get_Instance().get_IsDarkTheme() && AllowThemeToAdjustImage)
				{
					ExtensionMethods.InvertLightness(val3);
				}
			}
			if (pressedToNormalImageName != null)
			{
				StaticData.get_Instance().LoadIcon(pressedToNormalImageName, val4);
				if (!ActiveTheme.get_Instance().get_IsDarkTheme() && AllowThemeToAdjustImage)
				{
					ExtensionMethods.InvertLightness(val4);
				}
			}
			if (invertImageLocation)
			{
				flowDirection = (FlowDirection)2;
			}
			else
			{
				flowDirection = (FlowDirection)0;
			}
			TextImageWidget obj = new TextImageWidget(label, normalFillColor, normalBorderColor, normalTextColor, borderWidth, Margin, val, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight);
			GuiWidget val7 = (GuiWidget)(object)new TextImageWidget(label, hoverFillColor, normalBorderColor, hoverTextColor, borderWidth, Margin, val, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight);
			GuiWidget val9 = (GuiWidget)(object)new TextImageWidget(label, pressedFillColor, normalBorderColor, pressedTextColor, borderWidth, Margin, val3, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight);
			GuiWidget val11 = (GuiWidget)(object)new TextImageWidget(text, pressedFillColor, pressedBorderColor, pressedTextColor, borderWidth, Margin, val2, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight);
			GuiWidget val13 = (GuiWidget)(object)new TextImageWidget(label, hoverFillColor, pressedBorderColor, hoverTextColor, borderWidth, Margin, val2, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight);
			GuiWidget val15 = (GuiWidget)(object)new TextImageWidget(label, normalFillColor, pressedBorderColor, normalTextColor, borderWidth, Margin, val4, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight);
			GuiWidget val17 = (GuiWidget)(object)new TextImageWidget(label, disabledFillColor, disabledBorderColor, disabledTextColor, borderWidth, Margin, val, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight);
			return new CheckBoxViewStates((GuiWidget)(object)obj, val7, val9, val11, val13, val15, val17);
		}

		private CheckBoxViewStates getCheckBoxButtonView(string label, ImageBuffer normalImage = null, ImageBuffer pressedImage = null, ImageBuffer normalToPressedImage = null, ImageBuffer pressedToNormalImage = null, string pressedLabel = null)
		{
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Expected O, but got Unknown
			string text = pressedLabel;
			if (pressedLabel == null)
			{
				text = label;
			}
			if (normalImage != (ImageBuffer)null && !ActiveTheme.get_Instance().get_IsDarkTheme() && AllowThemeToAdjustImage)
			{
				ExtensionMethods.InvertLightness(normalImage);
			}
			if (pressedImage != (ImageBuffer)null && !ActiveTheme.get_Instance().get_IsDarkTheme() && AllowThemeToAdjustImage)
			{
				ExtensionMethods.InvertLightness(pressedImage);
			}
			if (normalToPressedImage != (ImageBuffer)null && !ActiveTheme.get_Instance().get_IsDarkTheme() && AllowThemeToAdjustImage)
			{
				ExtensionMethods.InvertLightness(normalToPressedImage);
			}
			if (pressedToNormalImage != (ImageBuffer)null && !ActiveTheme.get_Instance().get_IsDarkTheme() && AllowThemeToAdjustImage)
			{
				ExtensionMethods.InvertLightness(pressedToNormalImage);
			}
			if (invertImageLocation)
			{
				flowDirection = (FlowDirection)2;
			}
			else
			{
				flowDirection = (FlowDirection)0;
			}
			TextImageWidget obj = new TextImageWidget(label, normalFillColor, normalBorderColor, normalTextColor, borderWidth, Margin, normalImage, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight);
			GuiWidget val3 = (GuiWidget)(object)new TextImageWidget(label, hoverFillColor, normalBorderColor, hoverTextColor, borderWidth, Margin, normalImage, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight);
			GuiWidget val5 = (GuiWidget)(object)new TextImageWidget(label, pressedFillColor, normalBorderColor, pressedTextColor, borderWidth, Margin, normalToPressedImage, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight);
			GuiWidget val7 = (GuiWidget)(object)new TextImageWidget(text, pressedFillColor, pressedBorderColor, pressedTextColor, borderWidth, Margin, pressedImage, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight);
			GuiWidget val9 = (GuiWidget)(object)new TextImageWidget(label, hoverFillColor, pressedBorderColor, hoverTextColor, borderWidth, Margin, pressedImage, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight);
			GuiWidget val11 = (GuiWidget)(object)new TextImageWidget(label, normalFillColor, pressedBorderColor, normalTextColor, borderWidth, Margin, pressedToNormalImage, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight);
			GuiWidget val13 = (GuiWidget)(object)new TextImageWidget(label, disabledFillColor, disabledBorderColor, disabledTextColor, borderWidth, Margin, normalImage, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight);
			return new CheckBoxViewStates((GuiWidget)(object)obj, val3, val5, val7, val9, val11, val13);
		}

		public RadioButton GenerateRadioButton(string label, ImageBuffer iconImage)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Expected O, but got Unknown
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Expected O, but got Unknown
			if (iconImage != (ImageBuffer)null)
			{
				ExtensionMethods.InvertLightness(iconImage);
				if (ActiveTheme.get_Instance().get_IsDarkTheme() && AllowThemeToAdjustImage)
				{
					ExtensionMethods.InvertLightness(iconImage);
				}
			}
			BorderDouble val = default(BorderDouble);
			((BorderDouble)(ref val))._002Ector(0.0);
			TextImageWidget obj = new TextImageWidget(label, normalFillColor, normalBorderColor, normalTextColor, borderWidth, val, iconImage, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight, width: FixedWidth, centerText: true);
			TextImageWidget textImageWidget = new TextImageWidget(label, hoverFillColor, hoverBorderColor, hoverTextColor, borderWidth, val, iconImage, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight, width: FixedWidth, centerText: true);
			TextImageWidget textImageWidget2 = new TextImageWidget(label, hoverFillColor, checkedBorderColor, hoverTextColor, borderWidth, val, iconImage, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight, width: FixedWidth, centerText: true);
			TextImageWidget textImageWidget3 = new TextImageWidget(label, pressedFillColor, checkedBorderColor, pressedTextColor, borderWidth, val, iconImage, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight, width: FixedWidth, centerText: true);
			TextImageWidget textImageWidget4 = new TextImageWidget(label, disabledFillColor, disabledBorderColor, disabledTextColor, borderWidth, val, iconImage, flowDirection: flowDirection, fontSize: fontSize, height: FixedHeight, width: FixedWidth, centerText: true);
			RadioButton val7 = new RadioButton((GuiWidget)new RadioButtonViewStates((GuiWidget)(object)obj, (GuiWidget)(object)textImageWidget, (GuiWidget)(object)textImageWidget2, (GuiWidget)(object)textImageWidget3, (GuiWidget)(object)textImageWidget4));
			((GuiWidget)val7).set_Margin(Margin);
			return val7;
		}

		public RadioButton GenerateRadioButton(string label, string iconImageName = null)
		{
			if (iconImageName != null)
			{
				return GenerateRadioButton(label, StaticData.get_Instance().LoadIcon(iconImageName));
			}
			return GenerateRadioButton(label, (ImageBuffer)null);
		}
	}
}
