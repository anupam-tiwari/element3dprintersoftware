using System.Collections.Generic;
using MatterHackers.Localizations;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class GetFineBedHeight : FindBedHeight
	{
		private static string setZHeightFineInstruction1 = "We will now refine our measurement of the extruder height at this position.".Localize();

		private static string setZHeightFineInstructionTextOne = "Press [Z-] until there is resistance to moving the paper".Localize();

		private static string setZHeightFineInstructionTextTwo = "Press [Z+] once to release the paper".Localize();

		private static string setZHeightFineInstructionTextThree = "Finally click 'Next' to continue.".Localize();

		private static string setZHeightFineInstruction2 = $"\t• {setZHeightFineInstructionTextOne}\n\t• {setZHeightFineInstructionTextTwo}\n\n{setZHeightFineInstructionTextThree}";

		public GetFineBedHeight(WizardControl container, string pageDescription, List<ProbePosition> probePositions, int probePositionsBeingEditedIndex)
			: base(container, pageDescription, setZHeightFineInstruction1, setZHeightFineInstruction2, 0.1, probePositions, probePositionsBeingEditedIndex)
		{
		}
	}
}
