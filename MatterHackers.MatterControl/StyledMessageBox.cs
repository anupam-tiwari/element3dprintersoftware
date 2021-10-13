using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;

namespace MatterHackers.MatterControl
{
	public class StyledMessageBox : SystemWindow
	{
		public enum MessageType
		{
			OK,
			YES_NO
		}

		private string unwrappedMessage;

		private TextWidget messageContainer;

		private FlowLayoutWidget middleRowContainer;

		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private Action<bool> responseCallback;

		private double extraTextScaling = 1.0;

		public static void ShowMessageBox(Action<bool> callback, string message, string caption, MessageType messageType = MessageType.OK, string yesOk = "", string noCancel = "")
		{
			ShowMessageBox(callback, message, caption, null, messageType, yesOk, noCancel);
		}

		public static void ShowMessageBox(Action<bool> callback, string message, string caption, GuiWidget[] extraWidgetsToAdd, MessageType messageType, string yesOk = "", string noCancel = "")
		{
			StyledMessageBox styledMessageBox = new StyledMessageBox(callback, message, caption, messageType, extraWidgetsToAdd, 400.0, 300.0, yesOk, noCancel);
			((SystemWindow)styledMessageBox).set_CenterInParent(true);
			((SystemWindow)styledMessageBox).ShowAsSystemWindow();
		}

		public StyledMessageBox(Action<bool> callback, string message, string windowTitle, MessageType messageType, GuiWidget[] extraWidgetsToAdd, double width, double height, string yesOk, string noCancel)
			: this(width, height)
		{
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Expected O, but got Unknown
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Expected O, but got Unknown
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Expected O, but got Unknown
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Expected O, but got Unknown
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Expected O, but got Unknown
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Expected O, but got Unknown
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			if (UserSettings.Instance.IsTouchScreen)
			{
				extraTextScaling = 1.33333;
			}
			textImageButtonFactory.fontSize = extraTextScaling * textImageButtonFactory.fontSize;
			if (yesOk == "")
			{
				yesOk = ((messageType != 0) ? "Yes".Localize() : "Ok".Localize());
			}
			if (noCancel == "")
			{
				noCancel = "No".Localize();
			}
			responseCallback = callback;
			unwrappedMessage = message;
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).AnchorAll();
			if (UserSettings.Instance.IsTouchScreen)
			{
				((GuiWidget)val).set_Padding(new BorderDouble(12.0, 12.0, 13.0, 8.0));
			}
			else
			{
				((GuiWidget)val).set_Padding(new BorderDouble(3.0, 0.0, 3.0, 5.0));
			}
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 3.0, 0.0, 0.0));
			((GuiWidget)val2).set_Padding(new BorderDouble(0.0, 3.0, 0.0, 3.0));
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			TextWidget val3 = new TextWidget(windowTitle, 0.0, 0.0, 14.0 * extraTextScaling, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val3.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_VAnchor((VAnchor)1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			middleRowContainer = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)middleRowContainer).set_HAnchor((HAnchor)5);
			((GuiWidget)middleRowContainer).set_VAnchor((VAnchor)5);
			((GuiWidget)middleRowContainer).set_Padding(new BorderDouble(5.0, 5.0, 5.0, 15.0));
			((GuiWidget)middleRowContainer).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			RGBA_Bytes primaryTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			messageContainer = new TextWidget(message, 0.0, 0.0, 12.0 * extraTextScaling, (Justification)0, primaryTextColor, true, false, default(RGBA_Bytes), (TypeFace)null);
			messageContainer.set_AutoExpandBoundsToText(true);
			((GuiWidget)messageContainer).set_HAnchor((HAnchor)1);
			((GuiWidget)middleRowContainer).AddChild((GuiWidget)(object)messageContainer, -1);
			if (extraWidgetsToAdd != null)
			{
				foreach (GuiWidget val4 in extraWidgetsToAdd)
				{
					((GuiWidget)middleRowContainer).AddChild(val4, -1);
				}
			}
			((GuiWidget)val).AddChild((GuiWidget)(object)middleRowContainer, -1);
			FlowLayoutWidget val5 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)val5).set_HAnchor((HAnchor)5);
			((GuiWidget)val5).set_Padding(new BorderDouble(0.0, 3.0));
			switch (messageType)
			{
			case MessageType.YES_NO:
			{
				((GuiWidget)val5).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
				((SystemWindow)this).set_Title("Element - " + "Please Confirm".Localize());
				Button val7 = textImageButtonFactory.Generate(noCancel, (string)null, (string)null, (string)null, (string)null, centerText: true);
				((GuiWidget)val7).set_Name("No Button");
				((GuiWidget)val7).add_Click((EventHandler<MouseEventArgs>)noButton_Click);
				((GuiWidget)val7).set_Cursor((Cursors)3);
				((GuiWidget)val5).AddChild((GuiWidget)(object)val7, -1);
				Button val8 = textImageButtonFactory.Generate(yesOk, (string)null, (string)null, (string)null, (string)null, centerText: true);
				((GuiWidget)val8).set_Name("Yes Button");
				((GuiWidget)val8).add_Click((EventHandler<MouseEventArgs>)okButton_Click);
				((GuiWidget)val8).set_Cursor((Cursors)3);
				((GuiWidget)val5).AddChild((GuiWidget)(object)val8, -1);
				break;
			}
			case MessageType.OK:
			{
				((SystemWindow)this).set_Title("Element - " + "Alert".Localize());
				Button val6 = textImageButtonFactory.Generate(yesOk, (string)null, (string)null, (string)null, (string)null, centerText: true);
				((GuiWidget)val6).set_Name("Ok Button");
				((GuiWidget)val6).set_Cursor((Cursors)3);
				((GuiWidget)val6).add_Click((EventHandler<MouseEventArgs>)okButton_Click);
				((GuiWidget)val5).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
				((GuiWidget)val5).AddChild((GuiWidget)(object)val6, -1);
				break;
			}
			default:
				throw new NotImplementedException();
			}
			((GuiWidget)val).AddChild((GuiWidget)(object)val5, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			((SystemWindow)this).set_IsModal(true);
			AdjustTextWrap();
		}

		public override void OnBoundsChanged(EventArgs e)
		{
			AdjustTextWrap();
			((GuiWidget)this).OnBoundsChanged(e);
		}

		private void AdjustTextWrap()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			if (messageContainer != null)
			{
				double width = ((GuiWidget)middleRowContainer).get_Width();
				BorderDouble val = ((GuiWidget)middleRowContainer).get_Padding();
				double width2 = ((BorderDouble)(ref val)).get_Width();
				val = ((GuiWidget)messageContainer).get_Margin();
				double num = width - (width2 + ((BorderDouble)(ref val)).get_Width());
				if (num > 0.0)
				{
					string text = ((TextWrapping)new EnglishTextWrapping(12.0 * extraTextScaling * GuiWidget.get_DeviceScale())).InsertCRs(unwrappedMessage, num);
					((GuiWidget)messageContainer).set_Text(text);
				}
			}
		}

		private void noButton_Click(object sender, EventArgs mouseEvent)
		{
			UiThread.RunOnIdle((Action)base.Close);
			if (responseCallback != null)
			{
				responseCallback(obj: false);
			}
		}

		private void okButton_Click(object sender, EventArgs mouseEvent)
		{
			UiThread.RunOnIdle((Action)base.Close);
			if (responseCallback != null)
			{
				responseCallback(obj: true);
			}
		}
	}
}
