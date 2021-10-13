using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.FieldValidation;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl
{
	public class MacroDetailWidget : GuiWidget
	{
		private LinkButtonFactory linkButtonFactory = new LinkButtonFactory();

		private TextWidget macroCommandError;

		private MHTextEditWidget macroCommandInput;

		private TextWidget macroNameError;

		private MHTextEditWidget macroNameInput;

		private CheckBox showInActionMenu;

		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private EditMacrosWindow windowController;

		public MacroDetailWidget(EditMacrosWindow windowController)
			: this()
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Expected O, but got Unknown
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Expected O, but got Unknown
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Expected O, but got Unknown
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Expected O, but got Unknown
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Expected O, but got Unknown
			this.windowController = windowController;
			linkButtonFactory.fontSize = 10.0;
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).AnchorAll();
			((GuiWidget)val).set_Padding(new BorderDouble(3.0, 0.0, 3.0, 5.0));
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 3.0, 0.0, 0.0));
			((GuiWidget)val2).set_Padding(new BorderDouble(0.0, 3.0, 0.0, 3.0));
			string arg = "Edit Macro".Localize();
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
			((GuiWidget)val4).AddChild((GuiWidget)(object)CreateMacroNameContainer(), -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)CreateMacroCommandContainer(), -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)CreateMacroActionEdit(), -1);
			Button val5 = textImageButtonFactory.Generate("Save".Localize());
			((GuiWidget)val5).add_Click((EventHandler<MouseEventArgs>)SaveMacro_Click);
			Button val6 = textImageButtonFactory.Generate("Cancel".Localize());
			((GuiWidget)val6).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					windowController.ChangeToMacroList();
				});
			});
			FlowLayoutWidget val7 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val7).set_HAnchor((HAnchor)5);
			((GuiWidget)val7).set_Padding(new BorderDouble(0.0, 3.0));
			GuiWidget val8 = new GuiWidget();
			val8.set_HAnchor((HAnchor)5);
			((GuiWidget)val7).AddChild(val8, -1);
			((GuiWidget)val7).AddChild((GuiWidget)(object)val6, -1);
			((GuiWidget)val7).AddChild((GuiWidget)(object)val5, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val7, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)this).AnchorAll();
		}

		private FlowLayoutWidget CreateMacroCommandContainer()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Expected O, but got Unknown
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Expected O, but got Unknown
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 5.0));
			BorderDouble margin = default(BorderDouble);
			((BorderDouble)(ref margin))._002Ector(0.0, 0.0, 0.0, 3.0);
			string arg = "Macro Commands".Localize();
			TextWidget val2 = new TextWidget($"{arg}:", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 1.0));
			macroCommandInput = new MHTextEditWidget(windowController.ActiveMacro.GCode, 0.0, 0.0, 12.0, 0.0, 120.0, multiLine: true, 0, "", ApplicationController.MonoSpacedTypeFace);
			macroCommandInput.DrawFromHintedCache();
			((GuiWidget)macroCommandInput).set_HAnchor((HAnchor)5);
			((GuiWidget)macroCommandInput).set_VAnchor((VAnchor)5);
			((GuiWidget)macroCommandInput.ActualTextEditWidget).set_VAnchor((VAnchor)5);
			string arg2 = "This should be in 'G-Code'".Localize();
			string text = $"{arg2}.";
			macroCommandError = new TextWidget(text, 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			macroCommandError.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)macroCommandError).set_HAnchor((HAnchor)5);
			((GuiWidget)macroCommandError).set_Margin(margin);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)macroCommandInput, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)macroCommandError, -1);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			return val;
		}

		private FlowLayoutWidget CreateMacroNameContainer()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Expected O, but got Unknown
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Expected O, but got Unknown
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 5.0));
			BorderDouble margin = default(BorderDouble);
			((BorderDouble)(ref margin))._002Ector(0.0, 0.0, 0.0, 3.0);
			TextWidget val2 = new TextWidget(string.Format("{0}:", "Macro Name".Localize()), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 1.0));
			macroNameInput = new MHTextEditWidget(GCodeMacro.FixMacroName(windowController.ActiveMacro.Name));
			((GuiWidget)macroNameInput).set_HAnchor((HAnchor)5);
			string arg = "Give the macro a name".Localize();
			string text = $"{arg}.";
			macroNameError = new TextWidget(text, 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			macroNameError.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)macroNameError).set_HAnchor((HAnchor)5);
			((GuiWidget)macroNameError).set_Margin(margin);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)macroNameInput, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)macroNameError, -1);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			return val;
		}

		private FlowLayoutWidget CreateMacroActionEdit()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Expected O, but got Unknown
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 5.0));
			new BorderDouble(0.0, 0.0, 0.0, 3.0);
			CheckBox val2 = new CheckBox("Show In Action Menu".Localize());
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 1.0));
			val2.set_Checked(windowController.ActiveMacro.ActionGroup);
			showInActionMenu = val2;
			((GuiWidget)val).AddChild((GuiWidget)(object)showInActionMenu, -1);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			return val;
		}

		private void SaveActiveMacro()
		{
			windowController.ActiveMacro.Name = ((GuiWidget)macroNameInput).get_Text();
			windowController.ActiveMacro.GCode = ((GuiWidget)macroCommandInput).get_Text();
			windowController.ActiveMacro.ActionGroup = showInActionMenu.get_Checked();
			if (!ActiveSliceSettings.Instance.Macros.Contains(windowController.ActiveMacro))
			{
				ActiveSliceSettings.Instance.Macros.Add(windowController.ActiveMacro);
			}
		}

		private void SaveMacro_Click(object sender, EventArgs mouseEvent)
		{
			UiThread.RunOnIdle((Action)delegate
			{
				if (ValidateMacroForm())
				{
					SaveActiveMacro();
					windowController.RefreshMacros();
					windowController.ChangeToMacroList();
				}
			});
		}

		private bool ValidateMacroForm()
		{
			ValidationMethods @object = new ValidationMethods();
			List<FormField> list = new List<FormField>();
			FormField.ValidationHandler[] validationHandlers = new FormField.ValidationHandler[1]
			{
				@object.StringIsNotEmpty
			};
			_ = new FormField.ValidationHandler[2]
			{
				@object.StringIsNotEmpty,
				@object.StringHasNoSpecialChars
			};
			list.Add(new FormField(macroNameInput, macroNameError, validationHandlers));
			list.Add(new FormField(macroCommandInput, macroCommandError, validationHandlers));
			bool result = true;
			foreach (FormField item in list)
			{
				((GuiWidget)item.FieldErrorMessageWidget).set_Visible(false);
				if (!item.Validate())
				{
					result = false;
				}
			}
			return result;
		}
	}
}
