using System;

namespace MatterHackers.MatterControl
{
	public class ConditionalClickWidget : ClickWidget
	{
		private Func<bool> enabledCallback;

		public override bool Enabled
		{
			get
			{
				return enabledCallback();
			}
			set
			{
				Console.WriteLine("Attempted to set readonly Enabled property on ConditionalClickWidget");
			}
		}

		public ConditionalClickWidget(Func<bool> enabledCallback)
		{
			this.enabledCallback = enabledCallback;
		}
	}
}
