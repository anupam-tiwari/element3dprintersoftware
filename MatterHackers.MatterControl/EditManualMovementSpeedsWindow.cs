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
	public class EditManualMovementSpeedsWindow : SystemWindow
	{
		protected TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private Action<string> functionToCallOnSave;

		private List<string> axisLabels = new List<string>();

		private List<GuiWidget> valueEditors = new List<GuiWidget>();

		public EditManualMovementSpeedsWindow(string windowTitle, string movementSpeedsString, Action<string> functionToCallOnSave)
			: this(260.0, 300.0)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected O, but got Unknown
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Expected O, but got Unknown
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Expected O, but got Unknown
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Expected O, but got Unknown
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Expected O, but got Unknown
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Expected O, but got Unknown
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Expected O, but got Unknown
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Expected O, but got Unknown
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Expected O, but got Unknown
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Expected O, but got Unknown
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_045c: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Expected O, but got Unknown
			//IL_0522: Unknown result type (might be due to invalid IL or missing references)
			//IL_058d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Expected O, but got Unknown
			//IL_0619: Unknown result type (might be due to invalid IL or missing references)
			//IL_0623: Unknown result type (might be due to invalid IL or missing references)
			//IL_062a: Expected O, but got Unknown
			((SystemWindow)this).set_AlwaysOnTopOfMain(true);
			((SystemWindow)this).set_Title(LocalizedString.Get(windowTitle));
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).AnchorAll();
			((GuiWidget)val).set_Padding(new BorderDouble(3.0, 0.0, 3.0, 5.0));
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 3.0, 0.0, 0.0));
			((GuiWidget)val2).set_Padding(new BorderDouble(0.0, 3.0, 0.0, 3.0));
			string arg = LocalizedString.Get("Movement Speeds Presets".Localize());
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
			val8.set_Width(76.0);
			val8.set_Height(16.0);
			val8.set_Margin(new BorderDouble(3.0, 0.0));
			TextWidget val9 = new TextWidget("mm/s".Localize(), 0.0, 0.0, 10.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val9).set_HAnchor((HAnchor)1);
			((GuiWidget)val9).set_VAnchor((VAnchor)2);
			val8.AddChild((GuiWidget)(object)val9, -1);
			((GuiWidget)val6).AddChild(val7, -1);
			((GuiWidget)val6).AddChild(val8, -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)val6, -1);
			string[] array = movementSpeedsString.Split(new char[1]
			{
				','
			});
			int num = 1;
			int num2 = 0;
			for (int i = 0; i < Enumerable.Count<string>((IEnumerable<string>)array) - 1; i += 2)
			{
				FlowLayoutWidget val10 = new FlowLayoutWidget((FlowDirection)0);
				((GuiWidget)val10).set_Padding(new BorderDouble(3.0));
				((GuiWidget)val10).set_HAnchor((HAnchor)(((GuiWidget)val10).get_HAnchor() | 5));
				TextWidget val11 = ((!array[i].StartsWith("e")) ? new TextWidget(string.Format("{0} {1}", "Axis".Localize(), array[i].ToUpper()), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null) : new TextWidget(string.Format("{0}(s)", "Extruder".Localize()), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null));
				((GuiWidget)val11).set_VAnchor((VAnchor)2);
				((GuiWidget)val10).AddChild((GuiWidget)(object)val11, -1);
				((GuiWidget)val10).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
				axisLabels.Add(array[i]);
				double result = 0.0;
				double.TryParse(array[i + 1], out result);
				result /= 60.0;
				MHNumberEdit mHNumberEdit = new MHNumberEdit(result, 0.0, 0.0, 12.0, 60.0, 0.0, allowNegatives: false, allowDecimals: true, 0.0, 2147483647.0, 1.0, num2++);
				((GuiWidget)mHNumberEdit).set_Margin(new BorderDouble(3.0));
				((GuiWidget)val10).AddChild((GuiWidget)(object)mHNumberEdit, -1);
				valueEditors.Add((GuiWidget)(object)mHNumberEdit);
				((GuiWidget)val4).AddChild((GuiWidget)(object)val10, -1);
				num++;
			}
			textImageButtonFactory.FixedHeight = fixedHeight;
			((SystemWindow)this).ShowAsSystemWindow();
			((GuiWidget)this).set_MinimumSize(new Vector2(260.0, 300.0));
			Button val12 = textImageButtonFactory.Generate("Save".Localize());
			((GuiWidget)val12).add_Click((EventHandler<MouseEventArgs>)save_Click);
			Button val13 = textImageButtonFactory.Generate("Cancel".Localize());
			((GuiWidget)val13).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)base.Close);
			});
			FlowLayoutWidget val14 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val14).set_HAnchor((HAnchor)5);
			((GuiWidget)val14).set_Padding(new BorderDouble(0.0, 3.0));
			GuiWidget val15 = new GuiWidget();
			val15.set_HAnchor((HAnchor)5);
			((GuiWidget)val14).AddChild(val15, -1);
			((GuiWidget)val14).AddChild((GuiWidget)(object)val13, -1);
			((GuiWidget)val14).AddChild((GuiWidget)(object)val12, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val14, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private void save_Click(object sender, EventArgs mouseEvent)
		{
			UiThread.RunOnIdle((Action)DoSave_Click);
		}

		private void DoSave_Click()
		{
			bool flag = true;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < Enumerable.Count<GuiWidget>((IEnumerable<GuiWidget>)valueEditors); i++)
			{
				if (!flag)
				{
					stringBuilder.Append(",");
				}
				flag = false;
				stringBuilder.Append(axisLabels[i]);
				stringBuilder.Append(",");
				double result = 0.0;
				double.TryParse(valueEditors[i].get_Text(), out result);
				stringBuilder.Append((result * 60.0).ToString());
			}
			functionToCallOnSave(stringBuilder.ToString());
			((GuiWidget)this).Close();
		}
	}
}
