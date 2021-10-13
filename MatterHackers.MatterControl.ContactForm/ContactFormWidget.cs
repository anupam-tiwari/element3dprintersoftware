using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.FieldValidation;
using MatterHackers.MatterControl.VersionManagement;

namespace MatterHackers.MatterControl.ContactForm
{
	public class ContactFormWidget : GuiWidget
	{
		protected TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		protected TextImageButtonFactory whiteButtonFactory = new TextImageButtonFactory();

		private Button submitButton;

		private Button cancelButton;

		private Button doneButton;

		private FlowLayoutWidget formContainer;

		private FlowLayoutWidget messageContainer;

		private TextWidget submissionStatus;

		private GuiWidget centerContainer;

		private MHTextEditWidget questionInput;

		private TextWidget questionErrorMessage;

		private MHTextEditWidget detailInput;

		private TextWidget detailErrorMessage;

		private MHTextEditWidget emailInput;

		private TextWidget emailErrorMessage;

		private MHTextEditWidget nameInput;

		private TextWidget nameErrorMessage;

		public ContactFormWidget(string subjectText, string bodyText)
			: this()
		{
			SetButtonAttributes();
			((GuiWidget)this).AnchorAll();
			cancelButton = textImageButtonFactory.Generate("Cancel".Localize());
			submitButton = textImageButtonFactory.Generate("Submit".Localize());
			doneButton = textImageButtonFactory.Generate("Done".Localize());
			((GuiWidget)doneButton).set_Visible(false);
			DoLayout(subjectText, bodyText);
			AddButtonHandlers();
		}

		private GuiWidget LabelGenerator(string labelText, int fontSize = 12, int height = 28)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Expected O, but got Unknown
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Expected O, but got Unknown
			GuiWidget val = new GuiWidget();
			val.set_HAnchor((HAnchor)5);
			val.set_Height((double)height * GuiWidget.get_DeviceScale());
			TextWidget val2 = new TextWidget(labelText, 0.0, 0.0, (double)fontSize, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_VAnchor((VAnchor)1);
			((GuiWidget)val2).set_HAnchor((HAnchor)1);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 2.0, 0.0, 0.0));
			val.AddChild((GuiWidget)(object)val2, -1);
			return val;
		}

		private TextWidget ErrorMessageGenerator()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Expected O, but got Unknown
			TextWidget val = new TextWidget("", 0.0, 0.0, 11.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val.set_AutoExpandBoundsToText(true);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 5.0));
			val.set_TextColor(RGBA_Bytes.Red);
			((GuiWidget)val).set_HAnchor((HAnchor)1);
			((GuiWidget)val).set_Visible(false);
			return val;
		}

		private void DoLayout(string subjectText, string bodyText)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Expected O, but got Unknown
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Expected O, but got Unknown
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Expected O, but got Unknown
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Expected O, but got Unknown
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Expected O, but got Unknown
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).AnchorAll();
			GuiWidget val2 = new GuiWidget();
			val2.set_HAnchor((HAnchor)5);
			val2.set_Height(30.0);
			TextWidget val3 = new TextWidget("How can we improve?".Localize(), 0.0, 0.0, 16.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val3.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val3).set_VAnchor((VAnchor)4);
			((GuiWidget)val3).set_HAnchor((HAnchor)1);
			((GuiWidget)val3).set_Margin(new BorderDouble(6.0, 3.0, 6.0, 6.0));
			val2.AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val).AddChild(val2, -1);
			centerContainer = new GuiWidget();
			centerContainer.AnchorAll();
			centerContainer.set_Padding(new BorderDouble(3.0, 0.0, 3.0, 3.0));
			messageContainer = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)messageContainer).AnchorAll();
			((GuiWidget)messageContainer).set_Visible(false);
			((GuiWidget)messageContainer).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((GuiWidget)messageContainer).set_Padding(new BorderDouble(10.0));
			submissionStatus = new TextWidget("Submitting your information...".Localize(), 0.0, 0.0, 13.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			submissionStatus.set_AutoExpandBoundsToText(true);
			((GuiWidget)submissionStatus).set_Margin(new BorderDouble(0.0, 5.0));
			submissionStatus.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)submissionStatus).set_HAnchor((HAnchor)1);
			((GuiWidget)messageContainer).AddChild((GuiWidget)(object)submissionStatus, -1);
			formContainer = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)formContainer).AnchorAll();
			((GuiWidget)formContainer).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((GuiWidget)formContainer).set_Padding(new BorderDouble(10.0));
			((GuiWidget)formContainer).AddChild(LabelGenerator("Subject*".Localize()), -1);
			questionInput = new MHTextEditWidget(subjectText);
			((GuiWidget)questionInput).set_HAnchor((HAnchor)5);
			((GuiWidget)formContainer).AddChild((GuiWidget)(object)questionInput, -1);
			questionErrorMessage = ErrorMessageGenerator();
			((GuiWidget)formContainer).AddChild((GuiWidget)(object)questionErrorMessage, -1);
			((GuiWidget)formContainer).AddChild(LabelGenerator("Message*".Localize()), -1);
			detailInput = new MHTextEditWidget(bodyText, 0.0, 0.0, 12.0, 0.0, 120.0, multiLine: true);
			((GuiWidget)detailInput).set_HAnchor((HAnchor)5);
			((GuiWidget)formContainer).AddChild((GuiWidget)(object)detailInput, -1);
			detailErrorMessage = ErrorMessageGenerator();
			((GuiWidget)formContainer).AddChild((GuiWidget)(object)detailErrorMessage, -1);
			((GuiWidget)formContainer).AddChild(LabelGenerator("Email Address*".Localize()), -1);
			emailInput = new MHTextEditWidget();
			((GuiWidget)emailInput).set_HAnchor((HAnchor)5);
			((GuiWidget)formContainer).AddChild((GuiWidget)(object)emailInput, -1);
			emailErrorMessage = ErrorMessageGenerator();
			((GuiWidget)formContainer).AddChild((GuiWidget)(object)emailErrorMessage, -1);
			((GuiWidget)formContainer).AddChild(LabelGenerator("Name*".Localize()), -1);
			nameInput = new MHTextEditWidget();
			((GuiWidget)nameInput).set_HAnchor((HAnchor)5);
			((GuiWidget)formContainer).AddChild((GuiWidget)(object)nameInput, -1);
			nameErrorMessage = ErrorMessageGenerator();
			((GuiWidget)formContainer).AddChild((GuiWidget)(object)nameErrorMessage, -1);
			centerContainer.AddChild((GuiWidget)(object)formContainer, -1);
			((GuiWidget)val).AddChild(centerContainer, -1);
			FlowLayoutWidget buttonButtonPanel = GetButtonButtonPanel();
			((GuiWidget)buttonButtonPanel).AddChild((GuiWidget)(object)submitButton, -1);
			((GuiWidget)buttonButtonPanel).AddChild((GuiWidget)(object)cancelButton, -1);
			((GuiWidget)buttonButtonPanel).AddChild((GuiWidget)(object)doneButton, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)buttonButtonPanel, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private bool ValidateContactForm()
		{
			ValidationMethods @object = new ValidationMethods();
			List<FormField> list = new List<FormField>();
			FormField.ValidationHandler[] validationHandlers = new FormField.ValidationHandler[1]
			{
				@object.StringIsNotEmpty
			};
			FormField.ValidationHandler[] validationHandlers2 = new FormField.ValidationHandler[2]
			{
				@object.StringIsNotEmpty,
				@object.StringLooksLikeEmail
			};
			list.Add(new FormField(questionInput, questionErrorMessage, validationHandlers));
			list.Add(new FormField(detailInput, detailErrorMessage, validationHandlers));
			list.Add(new FormField(emailInput, emailErrorMessage, validationHandlers2));
			list.Add(new FormField(nameInput, nameErrorMessage, validationHandlers));
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

		private void AddButtonHandlers()
		{
			((GuiWidget)cancelButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)base.Close);
			});
			((GuiWidget)doneButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)base.Close);
			});
			((GuiWidget)submitButton).add_Click((EventHandler<MouseEventArgs>)SubmitContactForm);
		}

		private void SubmitContactForm(object sender, EventArgs mouseEvent)
		{
			if (ValidateContactForm())
			{
				ContactFormRequest contactFormRequest = new ContactFormRequest(((GuiWidget)questionInput).get_Text(), ((GuiWidget)detailInput).get_Text(), ((GuiWidget)emailInput).get_Text(), ((GuiWidget)nameInput).get_Text(), "");
				((GuiWidget)formContainer).set_Visible(false);
				((GuiWidget)messageContainer).set_Visible(true);
				centerContainer.RemoveAllChildren();
				centerContainer.AddChild((GuiWidget)(object)messageContainer, -1);
				((GuiWidget)cancelButton).set_Visible(false);
				((GuiWidget)submitButton).set_Visible(false);
				contactFormRequest.RequestSucceeded += onPostRequestSucceeded;
				contactFormRequest.RequestFailed += onPostRequestFailed;
				contactFormRequest.Request();
			}
		}

		private void onPostRequestSucceeded(object sender, EventArgs e)
		{
			((GuiWidget)submissionStatus).set_Text("Thank you!  Your information has been submitted.".Localize());
			((GuiWidget)doneButton).set_Visible(true);
		}

		private void onPostRequestFailed(object sender, ResponseErrorEventArgs e)
		{
			((GuiWidget)submissionStatus).set_Text("Sorry!  We weren't able to submit your request.".Localize());
			((GuiWidget)doneButton).set_Visible(true);
		}

		private FlowLayoutWidget GetButtonButtonPanel()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Padding(new BorderDouble(3.0, 3.0));
			((GuiWidget)val).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			return val;
		}

		private void SetButtonAttributes()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			textImageButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.disabledTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			whiteButtonFactory.FixedWidth = 138.0 * GuiWidget.get_DeviceScale();
			whiteButtonFactory.normalFillColor = RGBA_Bytes.White;
			whiteButtonFactory.normalTextColor = RGBA_Bytes.Black;
			whiteButtonFactory.hoverTextColor = RGBA_Bytes.Black;
			whiteButtonFactory.hoverFillColor = new RGBA_Bytes(255, 255, 255, 200);
		}
	}
}
