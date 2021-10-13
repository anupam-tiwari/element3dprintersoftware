using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl.ActionBar
{
	internal class TemperatureWidgetBase : GuiWidget
	{
		private TextWidget currentTempIndicator;

		protected TextWidget temperatureTypeName;

		protected Button preheatButton;

		protected TextImageButtonFactory whiteButtonFactory = new TextImageButtonFactory
		{
			FixedHeight = 18.0 * GuiWidget.get_DeviceScale(),
			fontSize = 7.0,
			normalFillColor = RGBA_Bytes.White,
			normalTextColor = RGBA_Bytes.DarkGray
		};

		private static RGBA_Bytes borderColor = new RGBA_Bytes(255, 255, 255);

		private int borderWidth = 2;

		public string IndicatorValue
		{
			get
			{
				return ((GuiWidget)currentTempIndicator).get_Text();
			}
			set
			{
				if (((GuiWidget)currentTempIndicator).get_Text() != value)
				{
					((GuiWidget)currentTempIndicator).set_Text(value);
				}
			}
		}

		public TemperatureWidgetBase(string textValue)
			: this(52.0 * GuiWidget.get_DeviceScale(), 52.0 * GuiWidget.get_DeviceScale(), (SizeLimitsToSet)1)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Expected O, but got Unknown
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Expected O, but got Unknown
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Expected O, but got Unknown
			((GuiWidget)this).set_BackgroundColor(new RGBA_Bytes(255, 255, 255, 200));
			((GuiWidget)this).set_Margin(new BorderDouble(0.0, 2.0) * GuiWidget.get_DeviceScale());
			TextWidget val = new TextWidget("", 0.0, 0.0, 8.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val.set_AutoExpandBoundsToText(true);
			((GuiWidget)val).set_HAnchor((HAnchor)2);
			((GuiWidget)val).set_VAnchor((VAnchor)4);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 3.0));
			val.set_TextColor(ActiveTheme.get_Instance().get_SecondaryAccentColor());
			((GuiWidget)val).set_Visible(false);
			temperatureTypeName = val;
			((GuiWidget)this).AddChild((GuiWidget)(object)temperatureTypeName, -1);
			TextWidget val2 = new TextWidget(textValue, 0.0, 0.0, 11.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			((GuiWidget)val2).set_HAnchor((HAnchor)2);
			((GuiWidget)val2).set_VAnchor((VAnchor)2);
			val2.set_AutoExpandBoundsToText(true);
			currentTempIndicator = val2;
			((GuiWidget)this).AddChild((GuiWidget)(object)currentTempIndicator, -1);
			GuiWidget val3 = new GuiWidget();
			val3.set_HAnchor((HAnchor)5);
			val3.set_Height(18.0 * GuiWidget.get_DeviceScale());
			GuiWidget val4 = val3;
			((GuiWidget)this).AddChild(val4, -1);
			preheatButton = whiteButtonFactory.Generate("Preheat".Localize().ToUpper());
			((GuiWidget)preheatButton).set_Cursor((Cursors)3);
			((GuiWidget)preheatButton).set_Visible(false);
			((GuiWidget)preheatButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				SetTargetTemperature();
			});
			val4.AddChild((GuiWidget)(object)preheatButton, -1);
		}

		public override void OnMouseEnterBounds(MouseEventArgs mouseEvent)
		{
			((GuiWidget)temperatureTypeName).set_Visible(true);
			if (PrinterConnectionAndCommunication.Instance.PrinterIsConnected && !PrinterConnectionAndCommunication.Instance.PrinterIsPrinting)
			{
				((GuiWidget)preheatButton).set_Visible(true);
			}
			((GuiWidget)this).OnMouseEnterBounds(mouseEvent);
		}

		public override void OnMouseLeaveBounds(MouseEventArgs mouseEvent)
		{
			((GuiWidget)temperatureTypeName).set_Visible(false);
			((GuiWidget)preheatButton).set_Visible(false);
			((GuiWidget)this).OnMouseLeaveBounds(mouseEvent);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			((GuiWidget)this).OnDraw(graphics2D);
			RoundedRect val = new RoundedRect(((GuiWidget)this).get_LocalBounds(), 0.0);
			graphics2D.Render((IVertexSource)new Stroke((IVertexSource)(object)val, (double)borderWidth), (IColorType)(object)borderColor);
		}

		protected virtual void SetTargetTemperature()
		{
		}
	}
}
