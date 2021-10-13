using System;
using System.Collections.Generic;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class GetCoarseBedHeight : FindBedHeight
	{
		private static string setZHeightCoarseInstruction1 = "Using the [Z] controls on this screen, we will now take a coarse measurement of the extruder height at this position.".Localize();

		private static string setZHeightCourseInstructTextOne = "Place the paper under the extruder".Localize();

		private static string setZHeightCourseInstructTextTwo = "Using the above controls".Localize();

		private static string setZHeightCourseInstructTextThree = "Press [Z-] until there is resistance to moving the paper".Localize();

		private static string setZHeightCourseInstructTextFour = "Press [Z+] once to release the paper".Localize();

		private static string setZHeightCourseInstructTextFive = "Finally click 'Next' to continue.".Localize();

		private static string setZHeightCoarseInstruction2 = $"\t• {setZHeightCourseInstructTextOne}\n\t• {setZHeightCourseInstructTextTwo}\n\t• {setZHeightCourseInstructTextThree}\n\t• {setZHeightCourseInstructTextFour}\n\n{setZHeightCourseInstructTextFive}";

		protected Vector3 probeStartPosition;

		public GetCoarseBedHeight(WizardControl container, Vector3 probeStartPosition, string pageDescription, List<ProbePosition> probePositions, int probePositionsBeingEditedIndex)
			: base(container, pageDescription, setZHeightCoarseInstruction1, setZHeightCoarseInstruction2, 1.0, probePositions, probePositionsBeingEditedIndex)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			this.probeStartPosition = probeStartPosition;
		}

		public override void PageIsBecomingActive()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			base.PageIsBecomingActive();
			Vector3 val = ActiveSliceSettings.Instance.Helpers.ManualMovementSpeeds();
			PrinterConnectionAndCommunication.Instance.MoveAbsolute(PrinterConnectionAndCommunication.Axis.Z, probeStartPosition.z, val.z);
			PrinterConnectionAndCommunication.Instance.MoveAbsolute(probeStartPosition, val.x);
			PrinterConnectionAndCommunication.Instance.ReadPosition();
			((GuiWidget)container.backButton).set_Enabled(false);
			((GuiWidget)container.nextButton).set_Enabled(false);
			((GuiWidget)zPlusControl).add_Click((EventHandler<MouseEventArgs>)zControl_Click);
			((GuiWidget)zMinusControl).add_Click((EventHandler<MouseEventArgs>)zControl_Click);
		}

		protected void zControl_Click(object sender, EventArgs mouseEvent)
		{
			((GuiWidget)container.nextButton).set_Enabled(true);
		}

		public override void PageIsBecomingInactive()
		{
			((GuiWidget)container.backButton).set_Enabled(true);
			((GuiWidget)container.nextButton).set_Enabled(true);
			base.PageIsBecomingInactive();
		}
	}
}
