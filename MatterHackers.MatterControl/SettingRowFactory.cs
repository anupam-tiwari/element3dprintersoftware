using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;

namespace MatterHackers.MatterControl
{
	public class SettingRowFactory
	{
		public readonly int intEditWidth = (int)(60.0 * GuiWidget.get_DeviceScale() + 0.5);

		public readonly int doubleEditWidth = (int)(60.0 * GuiWidget.get_DeviceScale() + 0.5);

		public readonly int multiLineEditHeight = (int)(120.0 * GuiWidget.get_DeviceScale() + 0.5);

		private TabIndexKeeper tabIndexKeeper;

		private int tabIndex;

		public int TabIndex
		{
			get
			{
				if (tabIndexKeeper != null)
				{
					return tabIndexKeeper.TabIndex;
				}
				return tabIndex;
			}
			set
			{
				if (tabIndexKeeper != null)
				{
					tabIndexKeeper.TabIndex = value;
				}
				else
				{
					tabIndex = value;
				}
			}
		}

		public SettingRowFactory()
		{
		}

		public SettingRowFactory(TabIndexKeeper indexKeeper)
		{
			tabIndexKeeper = indexKeeper;
		}

		public FlowLayoutWidget NewRowWithLablesAndEdit(string settingName, GuiWidget settingEdit, string settingUnit = null)
		{
			FlowLayoutWidget val = NewRow();
			((GuiWidget)val).AddChild(MakeSettingName(settingName), -1);
			((GuiWidget)val).AddChild(settingEdit, -1);
			if (settingUnit != null)
			{
				((GuiWidget)val).AddChild(MakeUnits(settingUnit), -1);
			}
			return val;
		}

		public FlowLayoutWidget NewRow()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 2.0));
			((GuiWidget)val).set_Padding(new BorderDouble(3.0));
			((GuiWidget)val).set_HAnchor((HAnchor)13);
			return val;
		}

		public GuiWidget MakeSettingName(string name)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Expected O, but got Unknown
			//IL_0077: Expected O, but got Unknown
			GuiWidget val = new GuiWidget();
			val.set_Padding(new BorderDouble(0.0, 0.0, 5.0, 0.0));
			val.set_HAnchor((HAnchor)13);
			val.set_VAnchor((VAnchor)10);
			val.AddChild((GuiWidget)new WrappedTextWidget(name.Localize(), 10.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), true), -1);
			return val;
		}

		public MHNumberEdit MakeEditWidget(double initialValue, int width, bool isFloat, string toolTip = "")
		{
			bool allowDecimals = isFloat;
			MHNumberEdit mHNumberEdit = new MHNumberEdit(0.0, 0.0, 0.0, 12.0, width, 0.0, allowNegatives: false, allowDecimals, -2147483648.0, 2147483647.0, 1.0, TabIndex++)
			{
				SelectAllOnFocus = true
			};
			if (!string.IsNullOrEmpty(toolTip))
			{
				((GuiWidget)mHNumberEdit).set_ToolTipText(toolTip);
			}
			mHNumberEdit.ActuallNumberEdit.set_Value(initialValue);
			((TextEditWidget)mHNumberEdit.ActuallNumberEdit).get_InternalTextEditWidget().MarkAsStartingState();
			return mHNumberEdit;
		}

		public MHNumberEdit MakeDoubleEdit(double value, string toolTip = "")
		{
			return MakeEditWidget(value, doubleEditWidth, isFloat: true, toolTip);
		}

		public MHNumberEdit MakeIntEdit(int value, string toolTip = "")
		{
			return MakeEditWidget(value, intEditWidth, isFloat: false, toolTip);
		}

		public MHTextEditWidget MakePercentEdit(double initialValue, string toolTip = "")
		{
			MHTextEditWidget mHTextEditWidget = new MHTextEditWidget(initialValue + "%", 0.0, 0.0, 12.0, doubleEditWidth - 2, 0.0, multiLine: false, TabIndex++);
			mHTextEditWidget.SelectAllOnFocus = true;
			if (!string.IsNullOrEmpty(toolTip))
			{
				((GuiWidget)mHTextEditWidget).set_ToolTipText(toolTip);
			}
			((GuiWidget)mHTextEditWidget.ActualTextEditWidget).add_KeyUp((EventHandler<KeyEventArgs>)delegate(object sender, KeyEventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				TextEditWidget val2 = (TextEditWidget)sender;
				string text = ((GuiWidget)val2).get_Text().Trim();
				if (text.Contains("%"))
				{
					text = text.Substring(0, text.IndexOf("%"));
				}
				double.TryParse(text, out var result);
				text = result.ToString();
				text += "%";
				((GuiWidget)val2).set_Text(text);
			});
			mHTextEditWidget.ActualTextEditWidget.get_InternalTextEditWidget().add_AllSelected((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				InternalTextEditWidget val = (InternalTextEditWidget)sender;
				int num = ((GuiWidget)val).get_Text().IndexOf("%");
				if (num != -1)
				{
					val.SetSelection(0, num - 1);
				}
			});
			return mHTextEditWidget;
		}

		public GuiWidget MakeUnits(string units)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected O, but got Unknown
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Expected O, but got Unknown
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Expected O, but got Unknown
			GuiWidget val = new GuiWidget();
			val.set_HAnchor((HAnchor)0);
			val.set_VAnchor((VAnchor)10);
			val.set_Width(50.0 * GuiWidget.get_DeviceScale());
			GuiWidget val2 = new GuiWidget();
			val2.set_HAnchor((HAnchor)13);
			val2.set_VAnchor((VAnchor)10);
			val2.set_Padding(new BorderDouble(5.0, 0.0));
			GuiWidget val3 = val2;
			val3.AddChild((GuiWidget)new WrappedTextWidget(units.Localize(), 8.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), true), -1);
			val.AddChild(val3, -1);
			return val;
		}
	}
}
