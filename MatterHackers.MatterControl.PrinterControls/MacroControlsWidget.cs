using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrinterControls
{
	public class MacroControlsWidget : FlowLayoutWidget
	{
		protected string editWindowLabel;

		protected string label;

		protected FlowLayoutWidget presetButtonsContainer;

		protected TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		public MacroControlsWidget()
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
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Expected O, but got Unknown
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Expected O, but got Unknown
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
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
			Button editButton;
			AltGroupBox altGroupBox = new AltGroupBox(textImageButtonFactory.GenerateGroupBoxLabelWithEdit(new TextWidget("Macros".Localize(), 0.0, 0.0, 18.0, (Justification)0, ActiveTheme.get_Instance().get_SecondaryAccentColor(), true, false, default(RGBA_Bytes), (TypeFace)null), out editButton));
			((GuiWidget)editButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				EditMacrosWindow.Show();
			});
			altGroupBox.BorderColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			((GuiWidget)altGroupBox).set_HAnchor((HAnchor)(((GuiWidget)altGroupBox).get_HAnchor() | 5));
			altGroupBox.ClientArea.set_VAnchor((VAnchor)8);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 5.0));
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			presetButtonsContainer = GetMacroButtonContainer();
			((GuiWidget)val).AddChild((GuiWidget)(object)presetButtonsContainer, -1);
			((GuiWidget)altGroupBox).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)altGroupBox, -1);
		}

		private FlowLayoutWidget GetMacroButtonContainer()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Expected O, but got Unknown
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Expected O, but got Unknown
			FLowLeftRightWithWrapping macroContainer = new FLowLeftRightWithWrapping();
			TextWidget noMacrosFound = new TextWidget("No macros are currently set up for this printer.".Localize(), 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			noMacrosFound.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)macroContainer).AddChild((GuiWidget)(object)noMacrosFound, -1);
			((GuiWidget)noMacrosFound).set_Visible(false);
			PrinterSettings instance = ActiveSliceSettings.Instance;
			if (instance == null || !Enumerable.Any<GCodeMacro>(instance.UserMacros()))
			{
				((GuiWidget)noMacrosFound).set_Visible(true);
				return (FlowLayoutWidget)(object)macroContainer;
			}
			foreach (GCodeMacro macro in ActiveSliceSettings.Instance.UserMacros())
			{
				Button val = textImageButtonFactory.Generate(GCodeMacro.FixMacroName(macro.Name));
				((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 5.0, 0.0));
				((GuiWidget)val).add_Click((EventHandler<MouseEventArgs>)delegate
				{
					macro.Run();
				});
				((GuiWidget)macroContainer).AddChild((GuiWidget)(object)val, -1);
			}
			((GuiWidget)macroContainer).get_Children().add_CollectionChanged((NotifyCollectionChangedEventHandler)delegate
			{
				if (!((GuiWidget)this).get_HasBeenClosed())
				{
					((GuiWidget)noMacrosFound).set_Visible(((Collection<GuiWidget>)(object)((GuiWidget)macroContainer).get_Children()).Count == 0);
				}
			});
			return (FlowLayoutWidget)(object)macroContainer;
		}
	}
}
