using System;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrinterControls
{
	public class ActionControlsWidget : FlowLayoutWidget
	{
		protected string editWindowLabel;

		protected string label;

		protected FlowLayoutWidget presetButtonsContainer;

		protected TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		public ActionControlsWidget()
			: this((FlowDirection)3)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Expected O, but got Unknown
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Expected O, but got Unknown
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			textImageButtonFactory.normalFillColor = RGBA_Bytes.White;
			textImageButtonFactory.FixedHeight = 24.0 * GuiWidget.get_DeviceScale();
			textImageButtonFactory.fontSize = 12.0;
			textImageButtonFactory.borderWidth = 1.0;
			textImageButtonFactory.normalBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);
			textImageButtonFactory.hoverBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);
			textImageButtonFactory.disabledTextColor = RGBA_Bytes.Gray;
			textImageButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.normalTextColor = RGBA_Bytes.Black;
			textImageButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_Padding(new BorderDouble(5.0));
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_BackgroundColor(ActiveTheme.get_Instance().get_TertiaryBackgroundColor());
			FlowLayoutWidget val2 = val;
			((GuiWidget)val2).set_HAnchor((HAnchor)(((GuiWidget)val2).get_HAnchor() | 5));
			((GuiWidget)val2).set_VAnchor((VAnchor)8);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 5.0));
			((GuiWidget)val3).set_HAnchor((HAnchor)(((GuiWidget)val3).get_HAnchor() | 5));
			presetButtonsContainer = GetMacroButtonContainer();
			((GuiWidget)val3).AddChild((GuiWidget)(object)presetButtonsContainer, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val2, -1);
		}

		private FlowLayoutWidget GetMacroButtonContainer()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 3.0, 0.0));
			((GuiWidget)val).set_Padding(new BorderDouble(0.0, 3.0, 3.0, 3.0));
			PrinterSettings instance = ActiveSliceSettings.Instance;
			if (instance == null || !Enumerable.Any<GCodeMacro>(instance.ActionMacros()))
			{
				return val;
			}
			foreach (GCodeMacro macro in ActiveSliceSettings.Instance.ActionMacros())
			{
				Button val2 = textImageButtonFactory.Generate(GCodeMacro.FixMacroName(macro.Name));
				((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 0.0, 5.0, 0.0));
				((GuiWidget)val2).add_Click((EventHandler<MouseEventArgs>)delegate
				{
					macro.Run();
				});
				((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			}
			return val;
		}
	}
}
