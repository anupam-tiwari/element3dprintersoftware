namespace MatterHackers.MatterControl.FieldValidation
{
	public class ValidationStatus
	{
		public bool IsValid
		{
			get;
			set;
		}

		public string ErrorMessage
		{
			get;
			set;
		}

		public ValidationStatus()
		{
			IsValid = true;
			ErrorMessage = "";
		}
	}
}
