using System.Collections.Generic;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class LastPagelInstructions : InstructionsPage
	{
		protected WizardControl container;

		private List<ProbePosition> probePositions;

		public LastPagelInstructions(WizardControl container, string pageDescription, string instructionsText, List<ProbePosition> probePositions)
			: base(pageDescription, instructionsText)
		{
			this.probePositions = probePositions;
			this.container = container;
		}

		public override void PageIsBecomingActive()
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			PrintLevelingData printLevelingData = ActiveSliceSettings.Instance.Helpers.GetPrintLevelingData();
			printLevelingData.SampledPositions.Clear();
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(0.0, 0.0, ActiveSliceSettings.Instance.GetValue<double>("z_probe_z_offset"));
			for (int i = 0; i < probePositions.Count; i++)
			{
				printLevelingData.SampledPositions.Add(probePositions[i].position - val);
			}
			ActiveSliceSettings.Instance.Helpers.SetPrintLevelingData(printLevelingData, clearUserZOffset: true);
			ActiveSliceSettings.Instance.Helpers.DoPrintLeveling(doLeveling: true);
			if (ActiveSliceSettings.Instance.GetValue<bool>("z_homes_to_max"))
			{
				PrinterConnectionAndCommunication.Instance.HomeAxis(PrinterConnectionAndCommunication.Axis.XYZ);
			}
			((GuiWidget)container.backButton).set_Enabled(false);
			base.PageIsBecomingActive();
		}
	}
}
