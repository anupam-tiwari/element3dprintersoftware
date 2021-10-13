using System;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrinterControls.PrinterConnections
{
	public class ShowAuthPanel : ConnectionWizardPage
	{
		public ShowAuthPanel()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Expected O, but got Unknown
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Expected O, but got Unknown
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			WrappedTextWidget val = new WrappedTextWidget("Sign in to access your cloud printer profiles.\n\nOnce signed in you will be able to access:".Localize(), 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), true);
			val.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			WrappedTextWidget val2 = val;
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)val2, -1);
			AddBulletPointAndDescription(contentRow, "Cloud Library".Localize(), "Save your designs to the cloud and access them from anywhere in the world. You can also share them any time with with anyone you want.".Localize());
			AddBulletPointAndDescription(contentRow, "Cloud Printer Profiles".Localize(), "Create your machine settings once, and have them available anywhere you want to print. All your changes appear on all your devices.".Localize());
			AddBulletPointAndDescription(contentRow, "Remote Monitoring".Localize(), "Check on your prints from anywhere. With cloud monitoring, you have access to your printer no matter where you go.".Localize());
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)new VerticalSpacer(), -1);
			CheckBox rememberChoice = new CheckBox("Don't remind me again".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12.0);
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)rememberChoice, -1);
			rememberChoice.add_CheckedStateChanged((EventHandler)delegate
			{
				ApplicationSettings.Instance.set("SuppressAuthPanel", rememberChoice.get_Checked().ToString());
			});
			Button val3 = textImageButtonFactory.Generate("Skip".Localize());
			((GuiWidget)val3).set_Name("Connection Wizard Skip Sign In Button");
			((GuiWidget)val3).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				if (!Enumerable.Any<PrinterInfo>(ProfileManager.Instance.ActiveProfiles))
				{
					UiThread.RunOnIdle((Action)WizardWindow.ChangeToPage<SetupStepMakeModelName>);
				}
				else
				{
					UiThread.RunOnIdle((Action)((GuiWidget)WizardWindow).Close);
				}
			});
			Button val4 = textImageButtonFactory.Generate("Create Account".Localize());
			((GuiWidget)val4).set_Name("Create Account From Connection Wizard Button");
			((GuiWidget)val4).set_Margin(new BorderDouble(0.0, 0.0, 5.0, 0.0));
			((GuiWidget)val4).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					((GuiWidget)WizardWindow).Close();
					WizardWindow.ChangeToAccountCreate();
				});
			});
			Button val5 = textImageButtonFactory.Generate("Sign In".Localize());
			((GuiWidget)val5).set_Name("Sign In From Connection Wizard Button");
			((GuiWidget)val5).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					((GuiWidget)WizardWindow).Close();
					WizardWindow.ShowAuthDialog?.Invoke();
				});
			});
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)val5, -1);
		}

		private void AddBulletPointAndDescription(FlowLayoutWidget contentRow, string v1, string v2)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Expected O, but got Unknown
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Expected O, but got Unknown
			TextWidget val = new TextWidget("• " + v1, 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val).set_HAnchor((HAnchor)1);
			val.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 10.0));
			((GuiWidget)contentRow).AddChild((GuiWidget)val, -1);
			WrappedTextWidget val2 = new WrappedTextWidget(v2, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), true);
			val2.set_TextColor(ActiveTheme.get_Instance().get_SecondaryTextColor());
			((GuiWidget)val2).set_Margin(new BorderDouble(20.0, 5.0, 5.0, 5.0));
			((GuiWidget)contentRow).AddChild((GuiWidget)val2, -1);
		}
	}
}
