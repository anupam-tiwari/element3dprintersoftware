using System.Text.RegularExpressions;
using MatterHackers.Localizations;

namespace MatterHackers.MatterControl.FieldValidation
{
	public class ValidationMethods
	{
		private static Regex digitsOnly = new Regex("[^\\d]");

		public ValidationStatus StringIsNotEmpty(string value)
		{
			ValidationStatus validationStatus = new ValidationStatus();
			if (value.Trim() == "")
			{
				validationStatus.IsValid = false;
				validationStatus.ErrorMessage = "Oops! Field cannot be left blank".Localize();
			}
			return validationStatus;
		}

		public ValidationStatus StringHasNoSpecialChars(string value)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			ValidationStatus validationStatus = new ValidationStatus();
			if (!new Regex("^[a-zA-Z0-9 ]*$").IsMatch(value))
			{
				validationStatus.IsValid = false;
				validationStatus.ErrorMessage = "Oops! Field cannot have special characters".Localize();
			}
			return validationStatus;
		}

		public ValidationStatus StringLooksLikePhoneNumber(string value)
		{
			ValidationStatus validationStatus = new ValidationStatus();
			value = digitsOnly.Replace(value, "");
			if (value.Length == 10)
			{
				validationStatus.IsValid = true;
			}
			else if (value.Length == 11 && value[0] == '1')
			{
				validationStatus.IsValid = true;
			}
			else
			{
				validationStatus.IsValid = false;
				validationStatus.ErrorMessage = "Sorry!  Must be a valid U.S. or Canadian phone number.".Localize();
			}
			return validationStatus;
		}

		public ValidationStatus StringLooksLikeEmail(string value)
		{
			ValidationStatus validationStatus = new ValidationStatus();
			int num = value.IndexOf("@");
			int num2 = value.LastIndexOf(".");
			if (num < num2 && num > 0 && value.IndexOf("@@") == -1 && num2 > 2 && value.Length - num2 > 2)
			{
				validationStatus.IsValid = true;
			}
			else
			{
				validationStatus.IsValid = false;
				validationStatus.ErrorMessage = "Sorry!  Must be a valid email address.".Localize();
			}
			return validationStatus;
		}
	}
}
