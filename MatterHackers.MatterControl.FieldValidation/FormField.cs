using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.FieldValidation
{
	public class FormField
	{
		public delegate ValidationStatus ValidationHandler(string valueToValidate);

		private MHTextEditWidget FieldEditWidget
		{
			get;
			set;
		}

		public TextWidget FieldErrorMessageWidget
		{
			get;
			set;
		}

		private ValidationHandler[] FieldValidationHandlers
		{
			get;
			set;
		}

		public FormField(MHTextEditWidget textEditWidget, TextWidget errorMessageWidget, ValidationHandler[] validationHandlers)
		{
			FieldEditWidget = textEditWidget;
			FieldErrorMessageWidget = errorMessageWidget;
			FieldValidationHandlers = validationHandlers;
		}

		public bool Validate()
		{
			bool flag = true;
			ValidationHandler[] fieldValidationHandlers = FieldValidationHandlers;
			foreach (ValidationHandler validationHandler in fieldValidationHandlers)
			{
				if (flag)
				{
					ValidationStatus validationStatus = validationHandler(((GuiWidget)FieldEditWidget).get_Text());
					if (!validationStatus.IsValid)
					{
						flag = false;
						((GuiWidget)FieldErrorMessageWidget).set_Text(validationStatus.ErrorMessage);
						((GuiWidget)FieldErrorMessageWidget).set_Visible(true);
					}
				}
			}
			return flag;
		}
	}
}
