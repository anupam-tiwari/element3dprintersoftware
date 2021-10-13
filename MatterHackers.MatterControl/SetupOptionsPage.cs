using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;

namespace MatterHackers.MatterControl
{
	public class SetupOptionsPage : WizardPage
	{
		private EventHandler unregisterEvents;

		public SetupOptionsPage()
			: base("Done")
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)headerLabel).set_Text("Setup Options".Localize());
			textImageButtonFactory.borderWidth = 1.0;
			textImageButtonFactory.normalBorderColor = RGBA_Bytes.White;
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)new SetupPrinterView(textImageButtonFactory)
			{
				WizardPage = this
			}, -1);
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)new SetupAccountView(textImageButtonFactory), -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)cancelButton, -1);
			((GuiWidget)cancelButton).set_Text("Back".Localize());
		}
	}
}
