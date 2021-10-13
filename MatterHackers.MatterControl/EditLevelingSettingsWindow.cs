using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.ConfigurationPage.PrintLeveling;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class EditLevelingSettingsWindow : SystemWindow
	{
		protected TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private List<Vector3> positions = new List<Vector3>();

		public EditLevelingSettingsWindow()
			: this(400.0, 370.0)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Expected O, but got Unknown
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Expected O, but got Unknown
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Expected O, but got Unknown
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Expected O, but got Unknown
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Expected O, but got Unknown
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Expected O, but got Unknown
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Expected O, but got Unknown
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Expected O, but got Unknown
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Expected O, but got Unknown
			((SystemWindow)this).set_AlwaysOnTopOfMain(true);
			((SystemWindow)this).set_Title(LocalizedString.Get("Leveling Settings".Localize()));
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).AnchorAll();
			((GuiWidget)val).set_Padding(new BorderDouble(3.0, 0.0, 3.0, 5.0));
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 3.0, 0.0, 0.0));
			((GuiWidget)val2).set_Padding(new BorderDouble(0.0, 3.0, 0.0, 3.0));
			string arg = LocalizedString.Get("Sampled Positions".Localize());
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
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			double fixedHeight = textImageButtonFactory.FixedHeight;
			textImageButtonFactory.FixedHeight = 30.0 * GuiWidget.get_DeviceScale();
			PrintLevelingData printLevelingData = ActiveSliceSettings.Instance.Helpers.GetPrintLevelingData();
			for (int i = 0; i < printLevelingData.SampledPositions.Count; i++)
			{
				positions.Add(printLevelingData.SampledPositions[i]);
			}
			int num = 0;
			for (int j = 0; j < positions.Count; j++)
			{
				FlowLayoutWidget val5 = new FlowLayoutWidget((FlowDirection)0);
				((GuiWidget)val5).set_Padding(new BorderDouble(3.0));
				((GuiWidget)val5).set_HAnchor((HAnchor)(((GuiWidget)val5).get_HAnchor() | 5));
				string text = "Position".Localize();
				TextWidget val6 = new TextWidget(StringHelper.FormatWith("{0} {1,-5}", new object[2]
				{
					text,
					j + 1
				}), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
				((GuiWidget)val6).set_VAnchor((VAnchor)2);
				((GuiWidget)val5).AddChild((GuiWidget)(object)val6, -1);
				for (int k = 0; k < 3; k++)
				{
					((GuiWidget)val5).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
					string text2 = "x";
					switch (k)
					{
					case 1:
						text2 = "y";
						break;
					case 2:
						text2 = "z";
						break;
					}
					TextWidget val7 = new TextWidget(StringHelper.FormatWith("  {0}: ", new object[1]
					{
						text2
					}), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
					((GuiWidget)val7).set_VAnchor((VAnchor)2);
					((GuiWidget)val5).AddChild((GuiWidget)(object)val7, -1);
					int linkCompatibleRow = j;
					int linkCompatibleAxis = k;
					Vector3 val8 = positions[linkCompatibleRow];
					MHNumberEdit valueEdit = new MHNumberEdit(((Vector3)(ref val8)).get_Item(linkCompatibleAxis), 0.0, 0.0, 12.0, 60.0, 0.0, allowNegatives: true, allowDecimals: true, -2147483648.0, 2147483647.0, 1.0, num++);
					((TextEditWidget)valueEdit.ActuallNumberEdit).get_InternalTextEditWidget().add_EditComplete((EventHandler)delegate
					{
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						//IL_0016: Unknown result type (might be due to invalid IL or missing references)
						//IL_0045: Unknown result type (might be due to invalid IL or missing references)
						Vector3 value = positions[linkCompatibleRow];
						((Vector3)(ref value)).set_Item(linkCompatibleAxis, valueEdit.ActuallNumberEdit.get_Value());
						positions[linkCompatibleRow] = value;
					});
					((GuiWidget)valueEdit).set_Margin(new BorderDouble(3.0));
					((GuiWidget)val5).AddChild((GuiWidget)(object)valueEdit, -1);
				}
				((GuiWidget)val4).AddChild((GuiWidget)(object)val5, -1);
				((GuiWidget)val4).AddChild((GuiWidget)(object)new HorizontalLine(), -1);
			}
			textImageButtonFactory.FixedHeight = fixedHeight;
			((SystemWindow)this).ShowAsSystemWindow();
			((GuiWidget)this).set_MinimumSize(new Vector2(((GuiWidget)this).get_Width(), ((GuiWidget)this).get_Height()));
			Button val9 = textImageButtonFactory.Generate("Save".Localize());
			((GuiWidget)val9).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					PrintLevelingData printLevelingData2 = ActiveSliceSettings.Instance.Helpers.GetPrintLevelingData();
					for (int l = 0; l < printLevelingData2.SampledPositions.Count; l++)
					{
						printLevelingData2.SampledPositions[l] = positions[l];
					}
					ActiveSliceSettings.Instance.Helpers.SetPrintLevelingData(printLevelingData2, clearUserZOffset: false);
					ActiveSliceSettings.Instance.Helpers.UpdateLevelSettings();
					((GuiWidget)this).Close();
				});
			});
			Button val10 = textImageButtonFactory.Generate("Cancel".Localize());
			((GuiWidget)val10).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)base.Close);
			});
			FlowLayoutWidget val11 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val11).set_HAnchor((HAnchor)5);
			((GuiWidget)val11).set_Padding(new BorderDouble(0.0, 3.0));
			GuiWidget val12 = new GuiWidget();
			val12.set_HAnchor((HAnchor)5);
			((GuiWidget)val11).AddChild(val12, -1);
			((GuiWidget)val11).AddChild((GuiWidget)(object)val10, -1);
			((GuiWidget)val11).AddChild((GuiWidget)(object)val9, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val11, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}
	}
}
