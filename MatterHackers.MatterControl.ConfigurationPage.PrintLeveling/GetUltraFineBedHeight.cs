using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class GetUltraFineBedHeight : FindBedHeight
	{
		private static string setZHeightFineInstruction1 = "We will now finalize our measurement of the extruder height at this position.".Localize();

		private static string setHeightFineInstructionTextOne = "Press [Z-] one click PAST the first hint of resistance".Localize();

		private static string setHeightFineInstructionTextTwo = "Finally click 'Next' to continue.".Localize();

		private static string setZHeightFineInstruction2 = $"\t• {setHeightFineInstructionTextOne}\n\n\n{setHeightFineInstructionTextTwo}";

		private bool haveDrawn;

		public GetUltraFineBedHeight(WizardControl container, string pageDescription, List<ProbePosition> probePositions, int probePositionsBeingEditedIndex)
			: base(container, pageDescription, setZHeightFineInstruction1, setZHeightFineInstruction2, 0.02, probePositions, probePositionsBeingEditedIndex)
		{
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			haveDrawn = true;
			((GuiWidget)this).OnDraw(graphics2D);
		}

		public override void PageIsBecomingInactive()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if (haveDrawn)
			{
				PrinterConnectionAndCommunication.Instance.MoveRelative(PrinterConnectionAndCommunication.Axis.Z, 2.0, ActiveSliceSettings.Instance.Helpers.ManualMovementSpeeds().z);
			}
			base.PageIsBecomingInactive();
		}
	}
}
