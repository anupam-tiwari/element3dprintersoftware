using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.PrinterControls.PrinterConnections
{
	public class SerialPortIndexRadioButton : RadioButton
	{
		public string PortValue;

		public SerialPortIndexRadioButton(string label, string value)
			: this(label, 12)
		{
			PortValue = value;
			((GuiWidget)this).add_EnabledChanged((EventHandler)onRadioButtonEnabledChanged);
		}

		private void onRadioButtonEnabledChanged(object sender, EventArgs e)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (((GuiWidget)this).get_Enabled())
			{
				((RadioButton)this).set_TextColor(RGBA_Bytes.White);
			}
			((RadioButton)this).set_TextColor(RGBA_Bytes.Gray);
		}
	}
}
