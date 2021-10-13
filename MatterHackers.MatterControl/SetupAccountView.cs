using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;

namespace MatterHackers.MatterControl
{
	public class SetupAccountView : SetupViewBase
	{
		private EventHandler unregisterEvents;

		private Button signInButton;

		private Button signOutButton;

		private TextWidget statusMessage;

		private TextWidget connectionStatus;

		public static string AuthenticationString
		{
			private get;
			set;
		} = "";


		internal void RefreshStatus()
		{
			((GuiWidget)connectionStatus).set_Text(AuthenticationString);
			if (!((GuiWidget)this).get_HasBeenClosed())
			{
				UiThread.RunOnIdle((Action)RefreshStatus, 1.0);
			}
		}

		public SetupAccountView(TextImageButtonFactory textImageButtonFactory)
			: base("My Account")
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Expected O, but got Unknown
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Expected O, but got Unknown
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Expected O, but got Unknown
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Expected O, but got Unknown
			base.textImageButtonFactory = textImageButtonFactory;
			bool flag = true;
			string text = AuthenticationData.Instance.ActiveSessionUsername;
			if (text == null)
			{
				flag = false;
				text = "Not Signed In";
			}
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).AddChild((GuiWidget)new TextWidget(text, 0.0, 0.0, 16.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			TextWidget val2 = new TextWidget(AuthenticationString, 0.0, 0.0, 8.0, (Justification)0, ActiveTheme.get_Instance().get_SecondaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val2).set_Margin(new BorderDouble(5.0, 0.0, 0.0, 0.0));
			val2.set_AutoExpandBoundsToText(true);
			connectionStatus = val2;
			if (flag)
			{
				((GuiWidget)val).AddChild((GuiWidget)(object)connectionStatus, -1);
			}
			((GuiWidget)mainContainer).AddChild((GuiWidget)(object)val, -1);
			RefreshStatus();
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 14.0));
			signInButton = textImageButtonFactory.Generate("Sign In".Localize());
			((GuiWidget)signInButton).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 0.0));
			((GuiWidget)signInButton).set_VAnchor((VAnchor)2);
			((GuiWidget)signInButton).set_Visible(!flag);
			((GuiWidget)signInButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)ApplicationController.Instance.StartSignIn);
			});
			((GuiWidget)val3).AddChild((GuiWidget)(object)signInButton, -1);
			signOutButton = textImageButtonFactory.Generate("Sign Out".Localize());
			((GuiWidget)signOutButton).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 0.0));
			((GuiWidget)signOutButton).set_VAnchor((VAnchor)2);
			((GuiWidget)signOutButton).set_Visible(flag);
			((GuiWidget)signOutButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)ApplicationController.Instance.StartSignOut);
			});
			((GuiWidget)val3).AddChild((GuiWidget)(object)signOutButton, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			textImageButtonFactory.disabledTextColor = new RGBA_Bytes(textImageButtonFactory.normalTextColor, 100);
			Button val4 = textImageButtonFactory.Generate("Redeem Purchase".Localize());
			((GuiWidget)val4).set_Enabled(true);
			((GuiWidget)val4).set_Name("Redeem Code Button");
			((GuiWidget)val4).set_Margin(new BorderDouble(0.0, 0.0, 10.0, 0.0));
			((GuiWidget)val4).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				ApplicationController.Instance.RedeemDesignCode?.Invoke();
			});
			((GuiWidget)val3).AddChild((GuiWidget)(object)val4, -1);
			Button val5 = textImageButtonFactory.Generate("Enter Share Code".Localize());
			((GuiWidget)val5).set_Enabled(true);
			((GuiWidget)val5).set_Name("Enter Share Code");
			((GuiWidget)val5).set_Margin(new BorderDouble(0.0, 0.0, 10.0, 0.0));
			((GuiWidget)val5).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				ApplicationController.Instance.EnterShareCode?.Invoke();
			});
			if (!flag)
			{
				((GuiWidget)val4).set_Enabled(false);
				((GuiWidget)val5).set_Enabled(false);
			}
			((GuiWidget)val3).AddChild((GuiWidget)(object)val5, -1);
			statusMessage = new TextWidget("Please wait...", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_SecondaryAccentColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)statusMessage).set_Visible(false);
			((GuiWidget)val3).AddChild((GuiWidget)(object)statusMessage, -1);
			((GuiWidget)mainContainer).AddChild((GuiWidget)(object)val3, -1);
			ApplicationController.Instance.DoneReloadingAll.RegisterEvent((EventHandler)RemoveAndNewControl, ref unregisterEvents);
		}

		private void RemoveAndNewControl(object sender, EventArgs e)
		{
			GuiWidget parent = ((GuiWidget)this).get_Parent();
			int childIndex = parent.GetChildIndex((GuiWidget)(object)this);
			parent.RemoveChild((GuiWidget)(object)this);
			parent.AddChild((GuiWidget)(object)new SetupAccountView(textImageButtonFactory), childIndex);
			((GuiWidget)this).Close();
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}
	}
}
