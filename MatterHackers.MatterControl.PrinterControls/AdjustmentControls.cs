using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication.Io;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PrinterControls
{
	public class AdjustmentControls : ControlWidgetBase
	{
		private MHNumberEdit feedRateValue;

		private MHNumberEdit extrusionValue;

		private SolidSlider feedRateRatioSlider;

		private SolidSlider extrusionRatioSlider;

		private readonly double minExtrutionRatio = 0.5;

		private readonly double maxExtrusionRatio = 3.0;

		private readonly double minFeedRateRatio = 0.25;

		private readonly double maxFeedRateRatio = 3.0;

		private EventHandler unregisterEvents;

		public AdjustmentControls()
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Expected O, but got Unknown
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Expected O, but got Unknown
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Expected O, but got Unknown
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Expected O, but got Unknown
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Expected O, but got Unknown
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0505: Expected O, but got Unknown
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0533: Unknown result type (might be due to invalid IL or missing references)
			//IL_0539: Unknown result type (might be due to invalid IL or missing references)
			//IL_053f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0574: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Expected O, but got Unknown
			//IL_0594: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_0608: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
			AltGroupBox altGroupBox = new AltGroupBox((GuiWidget)new TextWidget("Tuning Adjustment".Localize(), 0.0, 0.0, 18.0, (Justification)0, ActiveTheme.get_Instance().get_SecondaryAccentColor(), true, false, default(RGBA_Bytes), (TypeFace)null));
			((GuiWidget)altGroupBox).set_Margin(BorderDouble.op_Implicit(0));
			altGroupBox.BorderColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			((GuiWidget)altGroupBox).set_HAnchor((HAnchor)5);
			AltGroupBox altGroupBox2 = altGroupBox;
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 0.0));
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Padding(new BorderDouble(3.0, 0.0, 3.0, 0.0));
			FlowLayoutWidget val2 = val;
			double totalWidthInPixels = 300.0 * GuiWidget.get_DeviceScale();
			double thumbWidth = 10.0 * GuiWidget.get_DeviceScale();
			if (UserSettings.Instance.IsTouchScreen)
			{
				thumbWidth = 15.0 * GuiWidget.get_DeviceScale();
			}
			TextWidget val3 = new TextWidget("", 0.0, 0.0, 4.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 6.0, 0.0, 0.0));
			((GuiWidget)val2).AddChild((GuiWidget)(object)val3, -1);
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val4).set_HAnchor((HAnchor)5);
			((GuiWidget)val4).set_Margin(BorderDouble.op_Implicit(0));
			((GuiWidget)val4).set_VAnchor((VAnchor)8);
			FlowLayoutWidget val5 = val4;
			TextWidget val6 = new TextWidget("Speed Multiplier".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val6).set_MinimumSize(new Vector2(140.0, 0.0) * GuiWidget.get_DeviceScale());
			val6.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val6).set_VAnchor((VAnchor)2);
			TextWidget val7 = val6;
			((GuiWidget)val5).AddChild((GuiWidget)(object)val7, -1);
			SolidSlider solidSlider = new SolidSlider(default(Vector2), thumbWidth, minFeedRateRatio, maxFeedRateRatio, (Orientation)0);
			((GuiWidget)solidSlider).set_Name("Feed Rate Slider");
			((GuiWidget)solidSlider).set_Margin(new BorderDouble(5.0, 0.0));
			solidSlider.Value = FeedRateMultiplyerStream.FeedRateRatio;
			((GuiWidget)solidSlider).set_HAnchor((HAnchor)5);
			solidSlider.TotalWidthInPixels = totalWidthInPixels;
			feedRateRatioSlider = solidSlider;
			feedRateRatioSlider.View.BackgroundColor = default(RGBA_Bytes);
			feedRateRatioSlider.ValueChanged += delegate
			{
				feedRateValue.ActuallNumberEdit.set_Value(Math.Round(feedRateRatioSlider.Value, 2));
			};
			feedRateRatioSlider.SliderReleased += delegate
			{
				FeedRateMultiplyerStream.FeedRateRatio = Math.Round(feedRateRatioSlider.Value, 2);
				ActiveSliceSettings.Instance.SetValue("feedrate_ratio", FeedRateMultiplyerStream.FeedRateRatio.ToString());
			};
			((GuiWidget)val5).AddChild((GuiWidget)(object)feedRateRatioSlider, -1);
			MHNumberEdit obj = new MHNumberEdit(Math.Round(FeedRateMultiplyerStream.FeedRateRatio, 2), 0.0, 0.0, 12.0, minValue: minFeedRateRatio, maxValue: maxFeedRateRatio, pixelWidth: 40.0 * GuiWidget.get_DeviceScale(), pixelHeight: 0.0, allowNegatives: false, allowDecimals: true);
			((GuiWidget)obj).set_Name("Feed Rate NumberEdit");
			obj.SelectAllOnFocus = true;
			((GuiWidget)obj).set_Margin(new BorderDouble(0.0, 0.0, 5.0, 0.0));
			((GuiWidget)obj).set_VAnchor((VAnchor)10);
			((GuiWidget)obj).set_Padding(BorderDouble.op_Implicit(0));
			feedRateValue = obj;
			((TextEditWidget)feedRateValue.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				feedRateRatioSlider.Value = feedRateValue.ActuallNumberEdit.get_Value();
				FeedRateMultiplyerStream.FeedRateRatio = Math.Round(feedRateRatioSlider.Value, 2);
				ActiveSliceSettings.Instance.SetValue("feedrate_ratio", FeedRateMultiplyerStream.FeedRateRatio.ToString());
			});
			((GuiWidget)val5).AddChild((GuiWidget)(object)feedRateValue, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val5, -1);
			textImageButtonFactory.FixedHeight = (int)((GuiWidget)feedRateValue).get_Height() + 1;
			textImageButtonFactory.borderWidth = 1.0;
			textImageButtonFactory.normalBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);
			textImageButtonFactory.hoverBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);
			Button val8 = textImageButtonFactory.Generate("Set".Localize());
			((GuiWidget)val8).set_VAnchor((VAnchor)2);
			((GuiWidget)val5).AddChild((GuiWidget)(object)val8, -1);
			FlowLayoutWidget val9 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val9).set_HAnchor((HAnchor)5);
			((GuiWidget)val9).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 10.0));
			((GuiWidget)val9).set_VAnchor((VAnchor)8);
			FlowLayoutWidget val10 = val9;
			TextWidget val11 = new TextWidget("Extrusion Multiplier".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val11).set_MinimumSize(new Vector2(140.0, 0.0) * GuiWidget.get_DeviceScale());
			val11.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val11).set_VAnchor((VAnchor)2);
			TextWidget val12 = val11;
			((GuiWidget)val10).AddChild((GuiWidget)(object)val12, -1);
			SolidSlider solidSlider2 = new SolidSlider(default(Vector2), thumbWidth, minExtrutionRatio, maxExtrusionRatio, (Orientation)0);
			((GuiWidget)solidSlider2).set_Name("Extrusion Multiplier Slider");
			solidSlider2.TotalWidthInPixels = totalWidthInPixels;
			((GuiWidget)solidSlider2).set_HAnchor((HAnchor)5);
			((GuiWidget)solidSlider2).set_Margin(new BorderDouble(5.0, 0.0));
			solidSlider2.Value = ExtrusionMultiplyerStream.ExtrusionRatio;
			extrusionRatioSlider = solidSlider2;
			extrusionRatioSlider.View.BackgroundColor = default(RGBA_Bytes);
			extrusionRatioSlider.ValueChanged += delegate
			{
				extrusionValue.ActuallNumberEdit.set_Value(Math.Round(extrusionRatioSlider.Value, 2));
			};
			extrusionRatioSlider.SliderReleased += delegate
			{
				ExtrusionMultiplyerStream.ExtrusionRatio = Math.Round(extrusionRatioSlider.Value, 2);
				ActiveSliceSettings.Instance.SetValue("extrusion_ratio", ExtrusionMultiplyerStream.ExtrusionRatio.ToString());
			};
			MHNumberEdit obj2 = new MHNumberEdit(Math.Round(ExtrusionMultiplyerStream.ExtrusionRatio, 2), 0.0, 0.0, 12.0, minValue: minExtrutionRatio, maxValue: maxExtrusionRatio, pixelWidth: 40.0 * GuiWidget.get_DeviceScale(), pixelHeight: 0.0, allowNegatives: false, allowDecimals: true);
			((GuiWidget)obj2).set_Name("Extrusion Multiplier NumberEdit");
			obj2.SelectAllOnFocus = true;
			((GuiWidget)obj2).set_Margin(new BorderDouble(0.0, 0.0, 5.0, 0.0));
			((GuiWidget)obj2).set_VAnchor((VAnchor)10);
			((GuiWidget)obj2).set_Padding(BorderDouble.op_Implicit(0));
			extrusionValue = obj2;
			((TextEditWidget)extrusionValue.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				extrusionRatioSlider.Value = extrusionValue.ActuallNumberEdit.get_Value();
				ExtrusionMultiplyerStream.ExtrusionRatio = Math.Round(extrusionRatioSlider.Value, 2);
				ActiveSliceSettings.Instance.SetValue("extrusion_ratio", ExtrusionMultiplyerStream.ExtrusionRatio.ToString());
			});
			((GuiWidget)val10).AddChild((GuiWidget)(object)extrusionRatioSlider, -1);
			((GuiWidget)val10).AddChild((GuiWidget)(object)extrusionValue, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val10, -1);
			textImageButtonFactory.FixedHeight = (int)((GuiWidget)extrusionValue).get_Height() + 1;
			Button val13 = textImageButtonFactory.Generate("Set".Localize());
			((GuiWidget)val13).set_VAnchor((VAnchor)2);
			((GuiWidget)val10).AddChild((GuiWidget)(object)val13, -1);
			((GuiWidget)altGroupBox2).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)altGroupBox2, -1);
			ActiveSliceSettings.SettingChanged.RegisterEvent((EventHandler)delegate(object s, EventArgs e)
			{
				StringEventArgs val14 = e as StringEventArgs;
				if (((val14 != null) ? val14.get_Data() : null) == "extrusion_ratio")
				{
					double value = ActiveSliceSettings.Instance.GetValue<double>("extrusion_ratio");
					extrusionRatioSlider.Value = value;
					extrusionValue.ActuallNumberEdit.set_Value(Math.Round(value, 2));
				}
				else if (((val14 != null) ? val14.get_Data() : null) == "feedrate_ratio")
				{
					double value2 = ActiveSliceSettings.Instance.GetValue<double>("feedrate_ratio");
					feedRateRatioSlider.Value = value2;
					feedRateValue.ActuallNumberEdit.set_Value(Math.Round(value2, 2));
				}
			}, ref unregisterEvents);
		}

		public override void OnLoad(EventArgs args)
		{
			((GuiWidget)this).set_Width(((GuiWidget)this).get_Width() + 1.0);
			((GuiWidget)this).OnLoad(args);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}
	}
}
