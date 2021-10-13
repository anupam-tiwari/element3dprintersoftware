using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class EditTemperaturePresetsWindow : SystemWindow
	{
		protected TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private EventHandler functionToCallOnSave;

		private List<GuiWidget> listWithValues = new List<GuiWidget>();

		public EditTemperaturePresetsWindow(string windowTitle, string temperatureSettings, EventHandler functionToCallOnSave)
			: this(360.0, 300.0)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Expected O, but got Unknown
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Expected O, but got Unknown
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Expected O, but got Unknown
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Expected O, but got Unknown
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Expected O, but got Unknown
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Expected O, but got Unknown
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Expected O, but got Unknown
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Expected O, but got Unknown
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Expected O, but got Unknown
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Expected O, but got Unknown
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Expected O, but got Unknown
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Expected O, but got Unknown
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Expected O, but got Unknown
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_060a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0611: Expected O, but got Unknown
			//IL_061c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0629: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0660: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_066f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0672: Unknown result type (might be due to invalid IL or missing references)
			//IL_0679: Expected O, but got Unknown
			//IL_0717: Unknown result type (might be due to invalid IL or missing references)
			//IL_0766: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d6: Expected O, but got Unknown
			//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0803: Expected O, but got Unknown
			((SystemWindow)this).set_AlwaysOnTopOfMain(true);
			((SystemWindow)this).set_Title(LocalizedString.Get(windowTitle));
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).AnchorAll();
			((GuiWidget)val).set_Padding(new BorderDouble(3.0, 0.0, 3.0, 5.0));
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 3.0, 0.0, 0.0));
			((GuiWidget)val2).set_Padding(new BorderDouble(0.0, 3.0, 0.0, 3.0));
			string arg = "Temperature Shortcut Presets".Localize();
			TextWidget val3 = new TextWidget($"{arg}:", 0.0, 0.0, 14.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val3.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_VAnchor((VAnchor)1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val4).set_HAnchor((HAnchor)5);
			((GuiWidget)val4).set_VAnchor((VAnchor)5);
			((GuiWidget)val4).set_Padding(new BorderDouble(3.0));
			((GuiWidget)val4).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((GuiWidget)val).AddChild((GuiWidget)(object)val4, -1);
			this.functionToCallOnSave = functionToCallOnSave;
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			double fixedHeight = textImageButtonFactory.FixedHeight;
			textImageButtonFactory.FixedHeight = 30.0 * GuiWidget.get_DeviceScale();
			TextWidget val5 = new TextWidget(windowTitle, 0.0, 0.0, 10.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val5).set_Margin(new BorderDouble(3.0));
			((GuiWidget)val5).set_HAnchor((HAnchor)1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)val5, -1);
			FlowLayoutWidget val6 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val6).set_Padding(new BorderDouble(3.0, 6.0));
			((GuiWidget)val6).set_HAnchor((HAnchor)(((GuiWidget)val6).get_HAnchor() | 5));
			GuiWidget val7 = new GuiWidget();
			val7.set_HAnchor((HAnchor)5);
			GuiWidget val8 = new GuiWidget();
			val8.set_Width(66.0);
			val8.set_Height(16.0);
			val8.set_Margin(new BorderDouble(3.0, 0.0));
			TextWidget val9 = new TextWidget(string.Format("Label".Localize()), 0.0, 0.0, 10.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val9).set_HAnchor((HAnchor)1);
			((GuiWidget)val9).set_VAnchor((VAnchor)2);
			val8.AddChild((GuiWidget)(object)val9, -1);
			GuiWidget val10 = new GuiWidget();
			val10.set_Width(66.0);
			val10.set_Height(16.0);
			val10.set_Margin(new BorderDouble(3.0, 0.0));
			TextWidget val11 = new TextWidget($"Temp (C)", 0.0, 0.0, 10.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val11).set_HAnchor((HAnchor)1);
			((GuiWidget)val11).set_VAnchor((VAnchor)2);
			val10.AddChild((GuiWidget)(object)val11, -1);
			((GuiWidget)val6).AddChild(val7, -1);
			((GuiWidget)val6).AddChild(val8, -1);
			((GuiWidget)val6).AddChild(val10, -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)val6, -1);
			string[] array = temperatureSettings.Split(new char[1]
			{
				','
			});
			int num = 1;
			int tabIndex = 0;
			for (int i = 0; i < Enumerable.Count<string>((IEnumerable<string>)array) - 1; i += 2)
			{
				FlowLayoutWidget val12 = new FlowLayoutWidget((FlowDirection)0);
				((GuiWidget)val12).set_Padding(new BorderDouble(3.0));
				((GuiWidget)val12).set_HAnchor((HAnchor)(((GuiWidget)val12).get_HAnchor() | 5));
				string arg2 = "Preset".Localize();
				TextWidget val13 = new TextWidget(string.Format("{1} {0}", num, arg2), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
				((GuiWidget)val13).set_VAnchor((VAnchor)2);
				((GuiWidget)val12).AddChild((GuiWidget)(object)val13, -1);
				((GuiWidget)val12).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
				MHTextEditWidget mHTextEditWidget = new MHTextEditWidget(array[i], 0.0, 0.0, 12.0, 60.0, 0.0, multiLine: false, tabIndex++);
				((GuiWidget)mHTextEditWidget).set_Margin(new BorderDouble(3.0));
				((GuiWidget)val12).AddChild((GuiWidget)(object)mHTextEditWidget, -1);
				listWithValues.Add((GuiWidget)(object)mHTextEditWidget);
				double result = 0.0;
				double.TryParse(array[i + 1], out result);
				MHNumberEdit mHNumberEdit = new MHNumberEdit(result, 0.0, 0.0, 12.0, 60.0, 0.0, allowNegatives: false, allowDecimals: false, 0.0, 2147483647.0, 1.0, tabIndex++);
				((GuiWidget)mHNumberEdit).set_Margin(new BorderDouble(3.0));
				((GuiWidget)val12).AddChild((GuiWidget)(object)mHNumberEdit, -1);
				listWithValues.Add((GuiWidget)(object)mHNumberEdit);
				((GuiWidget)val4).AddChild((GuiWidget)(object)val12, -1);
				num++;
			}
			FlowLayoutWidget val14 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val14).set_Padding(new BorderDouble(3.0));
			((GuiWidget)val14).set_HAnchor((HAnchor)(((GuiWidget)val14).get_HAnchor() | 5));
			TextWidget val15 = new TextWidget("Max Temp".Localize(), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val15).set_VAnchor((VAnchor)2);
			((GuiWidget)val14).AddChild((GuiWidget)(object)val15, -1);
			((GuiWidget)val14).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			double result2 = 0.0;
			double.TryParse(array[Enumerable.Count<string>((IEnumerable<string>)array) - 1], out result2);
			MHNumberEdit mHNumberEdit2 = new MHNumberEdit(result2, 0.0, 0.0, 12.0, 60.0, 0.0, allowNegatives: false, allowDecimals: false, 0.0, 2147483647.0, 1.0, tabIndex);
			((GuiWidget)mHNumberEdit2).set_Margin(new BorderDouble(3.0));
			((GuiWidget)val14).AddChild((GuiWidget)(object)mHNumberEdit2, -1);
			listWithValues.Add((GuiWidget)(object)mHNumberEdit2);
			((GuiWidget)val4).AddChild((GuiWidget)(object)val14, -1);
			textImageButtonFactory.FixedHeight = fixedHeight;
			((SystemWindow)this).ShowAsSystemWindow();
			((GuiWidget)this).set_MinimumSize(new Vector2(360.0, 300.0));
			Button val16 = textImageButtonFactory.Generate("Save".Localize());
			((GuiWidget)val16).add_Click((EventHandler<MouseEventArgs>)save_Click);
			Button val17 = textImageButtonFactory.Generate("Cancel".Localize());
			((GuiWidget)val17).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				((GuiWidget)this).CloseOnIdle();
			});
			FlowLayoutWidget val18 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val18).set_HAnchor((HAnchor)5);
			((GuiWidget)val18).set_Padding(new BorderDouble(0.0, 3.0));
			GuiWidget val19 = new GuiWidget();
			val19.set_HAnchor((HAnchor)5);
			((GuiWidget)val18).AddChild(val19, -1);
			((GuiWidget)val18).AddChild((GuiWidget)(object)val17, -1);
			((GuiWidget)val18).AddChild((GuiWidget)(object)val16, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val18, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private void save_Click(object sender, EventArgs mouseEvent)
		{
			UiThread.RunOnIdle((Action)save_OnIdle);
		}

		private void save_OnIdle()
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Expected O, but got Unknown
			bool flag = true;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (GuiWidget listWithValue in listWithValues)
			{
				if (!flag)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(listWithValue.get_Text());
				flag = false;
			}
			functionToCallOnSave(this, (EventArgs)new StringEventArgs(stringBuilder.ToString()));
			((GuiWidget)this).CloseOnIdle();
		}
	}
}
