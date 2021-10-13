using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.CustomWidgets
{
	public class MeshViewerColorPicker : SystemWindow
	{
		private static readonly int paddingWidth = 2;

		private static readonly BorderDouble sharedMargin = new BorderDouble((double)(2 * paddingWidth), (double)paddingWidth);

		private int materialIndex;

		private readonly RGBA_Bytes originalMaterialColor;

		private Func<RGBA_Bytes> getNewColor;

		private Action setNewMeshColor;

		private Action setCurrentColorValues;

		private Button resetColorButton;

		private SolidSlider hueEdit;

		private SolidSlider saturationEdit;

		private SolidSlider lightnessEdit;

		private SolidSlider hslAlphaEdit;

		private MHNumberEdit redEdit;

		private MHNumberEdit greenEdit;

		private MHNumberEdit blueEdit;

		private MHNumberEdit rgbAlphaEdit;

		private int tabIndex;

		public MeshViewerColorPicker(int materialIndexToChange)
			: this(200.0, 200.0)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Invalid comparison between Unknown and I4
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Expected O, but got Unknown
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Expected O, but got Unknown
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Expected O, but got Unknown
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Expected O, but got Unknown
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Expected O, but got Unknown
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Expected O, but got Unknown
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Expected O, but got Unknown
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Expected O, but got Unknown
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Expected O, but got Unknown
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			materialIndex = materialIndexToChange;
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)this).set_MinimumSize(new Vector2(((GuiWidget)this).get_Width(), ((GuiWidget)this).get_Height()));
			if ((int)OsInformation.get_OperatingSystem() == 1)
			{
				((GuiWidget)this).set_MaximumSize(((GuiWidget)this).get_MinimumSize());
			}
			((SystemWindow)this).set_Title(StringHelper.FormatWith("Element: Tool {0} Color", new object[1]
			{
				materialIndex
			}));
			originalMaterialColor = MeshViewerWidget.GetMaterialColor(materialIndex);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).AnchorAll();
			((GuiWidget)val).set_Padding(new BorderDouble(0.0, (double)paddingWidth));
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			TabControl val2 = new TabControl((Orientation)0);
			((GuiWidget)val2).AnchorAll();
			val2.get_TabBar().set_BorderColor(new RGBA_Bytes(0, 0, 0, 0));
			((GuiWidget)val2.get_TabBar()).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val3).AnchorAll();
			((GuiWidget)val3).set_Padding(new BorderDouble(0.0, (double)paddingWidth));
			AddSeparator(val3);
			MakeHSLSliders((GuiWidget)(object)val3);
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val4).AnchorAll();
			((GuiWidget)val4).set_Padding(new BorderDouble(0.0, (double)paddingWidth));
			AddSeparator(val4);
			MakeRGBABoxes((GuiWidget)(object)val4);
			Tab val5 = (Tab)new SimpleTextTabWidget(new TabPage((GuiWidget)(object)val3, "HSL"), "HSL View Tab", 16.0, ActiveTheme.get_Instance().get_TabLabelSelected(), default(RGBA_Bytes), ActiveTheme.get_Instance().get_TabLabelUnselected(), default(RGBA_Bytes));
			val2.AddTab(val5);
			Tab val6 = (Tab)new SimpleTextTabWidget(new TabPage((GuiWidget)(object)val4, "RGB"), "WellPlate View Tab", 16.0, ActiveTheme.get_Instance().get_TabLabelSelected(), default(RGBA_Bytes), ActiveTheme.get_Instance().get_TabLabelUnselected(), default(RGBA_Bytes));
			val2.AddTab(val6);
			TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();
			FlowLayoutWidget val7 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val7).set_HAnchor((HAnchor)5);
			((GuiWidget)val).AddChild((GuiWidget)(object)val7, -1);
			setNewMeshColor = (Action)Delegate.Combine(setNewMeshColor, (Action)delegate
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				if (getNewColor != null)
				{
					RGBA_Bytes val10 = getNewColor();
					MeshViewerWidget.SetMaterialColor(materialIndex, val10);
					((GuiWidget)resetColorButton).set_Visible(val10 != MeshViewerWidget.GetOGMaterialColor(materialIndex));
					((GuiWidget)MatterControlApplication.Instance).Invalidate();
				}
			});
			resetColorButton = textImageButtonFactory.Generate("Reset".Localize());
			((GuiWidget)resetColorButton).set_Visible(originalMaterialColor != MeshViewerWidget.GetOGMaterialColor(materialIndex));
			((GuiWidget)resetColorButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				MeshViewerWidget.SetMaterialColor(materialIndex, MeshViewerWidget.GetOGMaterialColor(materialIndex));
				((GuiWidget)resetColorButton).set_Visible(false);
				setCurrentColorValues?.Invoke();
				((GuiWidget)MatterControlApplication.Instance).Invalidate();
			});
			((GuiWidget)val7).AddChild((GuiWidget)(object)resetColorButton, -1);
			((GuiWidget)val7).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			Button val8 = textImageButtonFactory.Generate("Cancel".Localize());
			((GuiWidget)val8).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				MeshViewerWidget.SetMaterialColor(materialIndex, originalMaterialColor);
				((GuiWidget)MatterControlApplication.Instance).Invalidate();
				((GuiWidget)this).Close();
			});
			((GuiWidget)val7).AddChild((GuiWidget)(object)val8, -1);
			Button val9 = textImageButtonFactory.Generate("OK".Localize());
			((GuiWidget)val9).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				setNewMeshColor?.Invoke();
				((GuiWidget)this).Close();
			});
			((GuiWidget)val7).AddChild((GuiWidget)(object)val9, -1);
		}

		private void AddSeparator(FlowLayoutWidget flowToAddTo)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Expected O, but got Unknown
			GuiWidget val = new GuiWidget();
			val.set_BackgroundColor(new RGBA_Bytes(200, 200, 200));
			val.set_Height(2.0);
			val.set_HAnchor((HAnchor)5);
			val.set_Margin(new BorderDouble(3.0));
			GuiWidget val2 = val;
			((GuiWidget)flowToAddTo).AddChild(val2, -1);
		}

		private void MakeHSLSliders(GuiWidget widgetToAddTo)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			RGBA_Bytes val = originalMaterialColor;
			RGBA_Floats colorF = ((RGBA_Bytes)(ref val)).GetAsRGBA_Floats();
			double hue0To1 = default(double);
			double saturation0To1 = default(double);
			double lightness0To1 = default(double);
			((RGBA_Floats)(ref colorF)).GetHSL(ref hue0To1, ref saturation0To1, ref lightness0To1);
			hueEdit = MakeSliderRowAndAdd(widgetToAddTo, "Hue", hue0To1);
			saturationEdit = MakeSliderRowAndAdd(widgetToAddTo, "Saturation", saturation0To1);
			lightnessEdit = MakeSliderRowAndAdd(widgetToAddTo, "Lightness", lightness0To1);
			val = originalMaterialColor;
			hslAlphaEdit = MakeSliderRowAndAdd(widgetToAddTo, "Opacity", ((RGBA_Bytes)(ref val)).get_Alpha0To1());
			getNewColor = () => new RGBA_Bytes(RGBA_Floats.FromHSL(hueEdit.Value, saturationEdit.Value, lightnessEdit.Value, hslAlphaEdit.Value));
			setCurrentColorValues = (Action)Delegate.Combine(setCurrentColorValues, (Action)delegate
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				RGBA_Bytes materialColor2 = MeshViewerWidget.GetMaterialColor(materialIndex);
				colorF = ((RGBA_Bytes)(ref materialColor2)).GetAsRGBA_Floats();
				((RGBA_Floats)(ref colorF)).GetHSL(ref hue0To1, ref saturation0To1, ref lightness0To1);
				hueEdit.Value = hue0To1;
				saturationEdit.Value = saturation0To1;
				lightnessEdit.Value = lightness0To1;
				hslAlphaEdit.Value = ((RGBA_Floats)(ref colorF)).get_Alpha0To1();
			});
			EventHandler value = delegate
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				RGBA_Bytes materialColor = MeshViewerWidget.GetMaterialColor(materialIndex);
				redEdit.ActuallNumberEdit.set_Value((double)((RGBA_Bytes)(ref materialColor)).get_Red0To255());
				greenEdit.ActuallNumberEdit.set_Value((double)((RGBA_Bytes)(ref materialColor)).get_Green0To255());
				blueEdit.ActuallNumberEdit.set_Value((double)((RGBA_Bytes)(ref materialColor)).get_Blue0To255());
				rgbAlphaEdit.ActuallNumberEdit.set_Value((double)((RGBA_Bytes)(ref materialColor)).get_Alpha0To255());
			};
			hueEdit.ValueChanged += delegate
			{
				setNewMeshColor?.Invoke();
			};
			saturationEdit.ValueChanged += delegate
			{
				setNewMeshColor?.Invoke();
			};
			lightnessEdit.ValueChanged += delegate
			{
				setNewMeshColor?.Invoke();
			};
			hslAlphaEdit.ValueChanged += delegate
			{
				setNewMeshColor?.Invoke();
			};
			hueEdit.SliderReleased += value;
			saturationEdit.SliderReleased += value;
			lightnessEdit.SliderReleased += value;
			hslAlphaEdit.SliderReleased += value;
		}

		private SolidSlider MakeSliderRowAndAdd(GuiWidget widgetToAddTo, string label, double defaultValue)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Expected O, but got Unknown
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(sharedMargin);
			TextWidget val2 = new TextWidget(label.Localize(), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			SolidSlider solidSlider = new SolidSlider(default(Vector2), 10.0, 0.0, 1.0, (Orientation)0);
			solidSlider.Value = defaultValue;
			solidSlider.TotalWidthInPixels = 100.0;
			((GuiWidget)val).AddChild((GuiWidget)(object)solidSlider, -1);
			widgetToAddTo.AddChild((GuiWidget)(object)val, -1);
			return solidSlider;
		}

		private void MakeRGBABoxes(GuiWidget widgetToAddTo)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			RGBA_Bytes val = originalMaterialColor;
			redEdit = Make0To255BoxRowAndAdd(widgetToAddTo, "Red", ((RGBA_Bytes)(ref val)).get_Red0To255());
			val = originalMaterialColor;
			greenEdit = Make0To255BoxRowAndAdd(widgetToAddTo, "Green", ((RGBA_Bytes)(ref val)).get_Green0To255());
			val = originalMaterialColor;
			blueEdit = Make0To255BoxRowAndAdd(widgetToAddTo, "Blue", ((RGBA_Bytes)(ref val)).get_Blue0To255());
			val = originalMaterialColor;
			rgbAlphaEdit = Make0To255BoxRowAndAdd(widgetToAddTo, "Opacity", ((RGBA_Bytes)(ref val)).get_Alpha0To255());
			setCurrentColorValues = (Action)Delegate.Combine(setCurrentColorValues, (Action)delegate
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				RGBA_Bytes materialColor = MeshViewerWidget.GetMaterialColor(materialIndex);
				redEdit.ActuallNumberEdit.set_Value((double)((RGBA_Bytes)(ref materialColor)).get_Red0To255());
				greenEdit.ActuallNumberEdit.set_Value((double)((RGBA_Bytes)(ref materialColor)).get_Green0To255());
				blueEdit.ActuallNumberEdit.set_Value((double)((RGBA_Bytes)(ref materialColor)).get_Blue0To255());
				rgbAlphaEdit.ActuallNumberEdit.set_Value((double)((RGBA_Bytes)(ref materialColor)).get_Alpha0To255());
			});
			EventHandler<KeyEventArgs> eventHandler = delegate
			{
				RGBA_Floats val2 = default(RGBA_Floats);
				((RGBA_Floats)(ref val2))._002Ector(redEdit.ActuallNumberEdit.get_Value() / 255.0, greenEdit.ActuallNumberEdit.get_Value() / 255.0, blueEdit.ActuallNumberEdit.get_Value() / 255.0, rgbAlphaEdit.ActuallNumberEdit.get_Value() / 255.0);
				double value = default(double);
				double value2 = default(double);
				double value3 = default(double);
				((RGBA_Floats)(ref val2)).GetHSL(ref value, ref value2, ref value3);
				hueEdit.Value = value;
				saturationEdit.Value = value2;
				lightnessEdit.Value = value3;
				hslAlphaEdit.Value = ((RGBA_Floats)(ref val2)).get_Alpha0To1();
			};
			((GuiWidget)redEdit.ActuallNumberEdit).add_KeyUp(eventHandler);
			((GuiWidget)greenEdit.ActuallNumberEdit).add_KeyUp(eventHandler);
			((GuiWidget)blueEdit.ActuallNumberEdit).add_KeyUp(eventHandler);
			((GuiWidget)rgbAlphaEdit.ActuallNumberEdit).add_KeyUp(eventHandler);
		}

		private MHNumberEdit Make0To255BoxRowAndAdd(GuiWidget widgetToAddTo, string label, double defaultValue)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(sharedMargin);
			TextWidget val2 = new TextWidget(label.Localize() + " (0-255)", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			MHNumberEdit mHNumberEdit = new MHNumberEdit(defaultValue, 0.0, 0.0, 12.0, 34.0, 0.0, allowNegatives: false, allowDecimals: false, 0.0, 255.0, 1.0, tabIndex++);
			((GuiWidget)val).AddChild((GuiWidget)(object)mHNumberEdit, -1);
			widgetToAddTo.AddChild((GuiWidget)(object)val, -1);
			return mHNumberEdit;
		}
	}
}
