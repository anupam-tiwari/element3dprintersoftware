using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl
{
	public class WizardControlPage : GuiWidget
	{
		private string stepDescription = "";

		public string StepDescription
		{
			get
			{
				return stepDescription;
			}
			set
			{
				stepDescription = value;
			}
		}

		public WizardControlPage(string stepDescription)
			: this()
		{
			StepDescription = stepDescription;
		}

		public virtual void PageIsBecomingActive()
		{
		}

		public virtual void PageIsBecomingInactive()
		{
		}
	}
}
