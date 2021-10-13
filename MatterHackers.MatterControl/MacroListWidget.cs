using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl
{
	public class MacroListWidget : GuiWidget
	{
		private LinkButtonFactory linkButtonFactory = new LinkButtonFactory();

		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private EditMacrosWindow windowController;

		public MacroListWidget(EditMacrosWindow windowController)
			: this()
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Expected O, but got Unknown
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Expected O, but got Unknown
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Expected O, but got Unknown
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Expected O, but got Unknown
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Expected O, but got Unknown
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Expected O, but got Unknown
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Expected O, but got Unknown
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Expected O, but got Unknown
			MacroListWidget macroListWidget = this;
			this.windowController = windowController;
			linkButtonFactory.fontSize = 10.0;
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).AnchorAll();
			((GuiWidget)val).set_Padding(new BorderDouble(3.0, 0.0, 3.0, 5.0));
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 3.0, 0.0, 0.0));
			((GuiWidget)val2).set_Padding(new BorderDouble(0.0, 3.0, 0.0, 3.0));
			string arg = "Macro Presets".Localize();
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
			if (ActiveSliceSettings.Instance?.Macros != null)
			{
				foreach (GCodeMacro macro in ActiveSliceSettings.Instance.Macros)
				{
					FlowLayoutWidget val5 = new FlowLayoutWidget((FlowDirection)0);
					((GuiWidget)val5).set_Margin(new BorderDouble(3.0, 0.0, 3.0, 3.0));
					((GuiWidget)val5).set_HAnchor((HAnchor)5);
					((GuiWidget)val5).set_Padding(new BorderDouble(3.0));
					((GuiWidget)val5).set_BackgroundColor(RGBA_Bytes.White);
					TextWidget val6 = new TextWidget(GCodeMacro.FixMacroName(macro.Name), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
					((GuiWidget)val5).AddChild((GuiWidget)(object)val6, -1);
					((GuiWidget)val5).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
					GCodeMacro localMacroReference = macro;
					Button val7 = linkButtonFactory.Generate("edit".Localize());
					((GuiWidget)val7).set_Margin(new BorderDouble(0.0, 0.0, 5.0, 0.0));
					((GuiWidget)val7).add_Click((EventHandler<MouseEventArgs>)delegate
					{
						windowController.ChangeToMacroDetail(localMacroReference);
					});
					((GuiWidget)val5).AddChild((GuiWidget)(object)val7, -1);
					Button val8 = linkButtonFactory.Generate("remove".Localize());
					((GuiWidget)val8).add_Click((EventHandler<MouseEventArgs>)delegate
					{
						ActiveSliceSettings.Instance.Macros.Remove(localMacroReference);
						windowController.RefreshMacros();
						windowController.ChangeToMacroList();
					});
					((GuiWidget)val5).AddChild((GuiWidget)(object)val8, -1);
					((GuiWidget)val4).AddChild((GuiWidget)(object)val5, -1);
				}
			}
			Button val9 = textImageButtonFactory.Generate("Add".Localize(), "icon_circle_plus.png");
			((GuiWidget)val9).set_ToolTipText("Add a new Macro".Localize());
			((GuiWidget)val9).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				windowController.ChangeToMacroDetail(new GCodeMacro
				{
					Name = "Home All",
					GCode = "G28 ; Home All Axes"
				});
			});
			Button val10 = textImageButtonFactory.Generate("Close".Localize());
			((GuiWidget)val10).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					((GuiWidget)macroListWidget.windowController).Close();
				});
			});
			FlowLayoutWidget val11 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val11).set_HAnchor((HAnchor)5);
			((GuiWidget)val11).set_Padding(new BorderDouble(0.0, 3.0));
			GuiWidget val12 = new GuiWidget();
			val12.set_HAnchor((HAnchor)5);
			((GuiWidget)val11).AddChild((GuiWidget)(object)val9, -1);
			((GuiWidget)val11).AddChild(val12, -1);
			((GuiWidget)val11).AddChild((GuiWidget)(object)val10, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val11, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)this).AnchorAll();
		}
	}
}
