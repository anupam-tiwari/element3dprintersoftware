using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;

namespace MatterHackers.MatterControl
{
	public class SetupWizardWifi : WizardPage
	{
		public SetupWizardWifi()
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Expected O, but got Unknown
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Expected O, but got Unknown
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Expected O, but got Unknown
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Expected O, but got Unknown
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget contentRow = base.contentRow;
			TextWidget val = new TextWidget("Wifi Setup".Localize() + ":", 0.0, 0.0, labelFontSize, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 10.0, 0.0, 0.0));
			((GuiWidget)contentRow).AddChild((GuiWidget)val, -1);
			((GuiWidget)base.contentRow).AddChild((GuiWidget)new TextWidget("Some features may require an internet connection.".Localize(), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			((GuiWidget)base.contentRow).AddChild((GuiWidget)new TextWidget("Would you like to setup Wifi?".Localize(), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 6.0));
			FlowLayoutWidget val3 = val2;
			Button skipButton = whiteImageButtonFactory.Generate("Skip".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)skipButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					WizardWindow.ChangeToSetupPrinterForm();
				});
			});
			Button nextButton = textImageButtonFactory.Generate("Continue".Localize());
			((GuiWidget)nextButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					WizardWindow.ChangeToSetupPrinterForm();
				});
			});
			((GuiWidget)nextButton).set_Visible(false);
			Button configureButton = whiteImageButtonFactory.Generate("Configure".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)configureButton).set_Margin(new BorderDouble(0.0, 0.0, 10.0, 0.0));
			((GuiWidget)configureButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					((GuiWidget)nextButton).set_Visible(true);
					((GuiWidget)skipButton).set_Visible(false);
					((GuiWidget)configureButton).set_Visible(false);
					MatterControlApplication.Instance.ConfigureWifi();
				});
			});
			((GuiWidget)val3).AddChild((GuiWidget)(object)configureButton, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)skipButton, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)base.contentRow).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)nextButton, -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)cancelButton, -1);
		}
	}
}
