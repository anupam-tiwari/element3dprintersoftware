using System;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.ContactForm
{
	public class ContactFormWindow : SystemWindow
	{
		private static ContactFormWindow contactFormWindow;

		private static bool contactFormIsOpen;

		private ContactFormWidget contactFormWidget;

		private EventHandler unregisterEvents;

		public static void Open(string subject = "", string bodyText = "")
		{
			if (!contactFormIsOpen)
			{
				contactFormWindow = new ContactFormWindow(subject, bodyText);
				contactFormIsOpen = true;
				((GuiWidget)contactFormWindow).add_Closed((EventHandler<ClosedEventArgs>)delegate
				{
					contactFormIsOpen = false;
				});
			}
			else if (contactFormWindow != null)
			{
				((GuiWidget)contactFormWindow).BringToFront();
			}
		}

		private ContactFormWindow(string subject = "", string bodyText = "")
			: this(500.0, 550.0)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			((SystemWindow)this).set_AlwaysOnTopOfMain(true);
			((SystemWindow)this).set_Title("MatterControl: Submit Feedback".Localize());
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			contactFormWidget = new ContactFormWidget(subject, bodyText);
			((GuiWidget)this).AddChild((GuiWidget)(object)contactFormWidget, -1);
			AddHandlers();
			((SystemWindow)this).ShowAsSystemWindow();
			((GuiWidget)this).set_MinimumSize(new Vector2(500.0, 550.0));
		}

		private void AddHandlers()
		{
			ActiveTheme.ThemeChanged.RegisterEvent((EventHandler)ThemeChanged, ref unregisterEvents);
			((GuiWidget)contactFormWidget).add_Closed((EventHandler<ClosedEventArgs>)delegate
			{
				((GuiWidget)this).Close();
			});
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((SystemWindow)this).OnClosed(e);
		}

		public void ThemeChanged(object sender, EventArgs e)
		{
			((GuiWidget)this).Invalidate();
		}
	}
}
