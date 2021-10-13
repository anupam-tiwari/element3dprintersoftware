using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl;
using MatterHackers.MatterControl.CustomWidgets;

public class LicenseAgreementPage : WizardPage
{
	public LicenseAgreementPage()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		string text = StaticData.get_Instance().ReadAllText("Element EULA.txt").Replace("\r\n", "\n");
		ScrollableWidget val = new ScrollableWidget(true);
		((GuiWidget)val).AnchorAll();
		((GuiWidget)val.get_ScrollArea()).set_HAnchor((HAnchor)5);
		((GuiWidget)contentRow).AddChild((GuiWidget)(object)val, -1);
		WrappedTextWidget val2 = new WrappedTextWidget(text, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), false);
		val2.set_DrawFromHintedCache(true);
		((GuiWidget)val2).set_Name("LicenseAgreementPage");
		WrappedTextWidget val3 = val2;
		((GuiWidget)val.get_ScrollArea()).set_Margin(new BorderDouble(0.0, 0.0, 15.0, 0.0));
		((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
		Button val4 = textImageButtonFactory.Generate("Accept".Localize());
		((GuiWidget)val4).add_Click((EventHandler<MouseEventArgs>)delegate
		{
			UserSettings.Instance.set("SoftwareLicenseAccepted", "true");
			UiThread.RunOnIdle((Action)((GuiWidget)WizardWindow).Close);
		});
		((GuiWidget)val4).set_Visible(true);
		((GuiWidget)cancelButton).set_Visible(true);
		((GuiWidget)cancelButton).add_Click((EventHandler<MouseEventArgs>)delegate
		{
			UiThread.RunOnIdle((Action)((GuiWidget)MatterControlApplication.Instance).Close);
		});
		((GuiWidget)footerRow).AddChild((GuiWidget)(object)val4, -1);
		((GuiWidget)footerRow).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
		((GuiWidget)footerRow).AddChild((GuiWidget)(object)cancelButton, -1);
		((GuiWidget)footerRow).set_Visible(true);
		UiThread.RunOnIdle((Action)MakeFrontWindow, 0.2);
	}

	private void MakeFrontWindow()
	{
		((GuiWidget)WizardWindow).BringToFront();
	}
}
