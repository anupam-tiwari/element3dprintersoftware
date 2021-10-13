using System;
using System.Collections.Generic;
using System.Linq;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class AutoProbeFeedback : InstructionsPage
	{
		private Vector3 lastReportedPosition;

		private List<ProbePosition> probePositions;

		private int probePositionsBeingEditedIndex;

		private EventHandler unregisterEvents;

		protected Vector3 probeStartPosition;

		protected WizardControl container;

		private List<double> samples = new List<double>();

		public AutoProbeFeedback(WizardControl container, Vector3 probeStartPosition, string pageDescription, List<ProbePosition> probePositions, int probePositionsBeingEditedIndex)
			: base(pageDescription, pageDescription)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			this.container = container;
			this.probeStartPosition = probeStartPosition;
			this.probePositions = probePositions;
			lastReportedPosition = PrinterConnectionAndCommunication.Instance.LastReportedPosition;
			this.probePositionsBeingEditedIndex = probePositionsBeingEditedIndex;
			GuiWidget val = new GuiWidget(15.0, 15.0, (SizeLimitsToSet)1);
			((GuiWidget)topToBottomControls).AddChild(val, -1);
			new FlowLayoutWidget((FlowDirection)3);
		}

		private void GetZProbeHeight(object sender, EventArgs e)
		{
			StringEventArgs val = e as StringEventArgs;
			if (val == null)
			{
				return;
			}
			double readValue = double.MinValue;
			if (val.get_Data().StartsWith("Bed"))
			{
				probePositions[probePositionsBeingEditedIndex].position.x = probeStartPosition.x;
				probePositions[probePositionsBeingEditedIndex].position.y = probeStartPosition.y;
				GCodeFile.GetFirstNumberAfter("Z:", val.get_Data(), ref readValue);
			}
			else if (val.get_Data().StartsWith("Z:"))
			{
				probePositions[probePositionsBeingEditedIndex].position.x = probeStartPosition.x;
				probePositions[probePositionsBeingEditedIndex].position.y = probeStartPosition.y;
				double readValue2 = 0.0;
				GCodeFile.GetFirstNumberAfter("Z:", val.get_Data(), ref readValue2);
				readValue = probeStartPosition.z - readValue2;
			}
			if (readValue == double.MinValue)
			{
				return;
			}
			samples.Add(readValue);
			int value = ActiveSliceSettings.Instance.GetValue<int>("z_probe_samples");
			if (samples.Count == value)
			{
				samples.Sort();
				if (samples.Count > 3)
				{
					samples.RemoveAt(0);
					samples.RemoveAt(samples.Count - 1);
				}
				probePositions[probePositionsBeingEditedIndex].position.z = Math.Round(Enumerable.Average((IEnumerable<double>)samples), 2);
				UiThread.RunOnIdle((Action)delegate
				{
					((ButtonBase)container.nextButton).ClickButton((MouseEventArgs)null);
				});
			}
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((GuiWidget)this).OnClosed(e);
		}

		public override void PageIsBecomingActive()
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			ActiveSliceSettings.Instance.Helpers.DoPrintLeveling(doLeveling: false);
			base.PageIsBecomingActive();
			if (ActiveSliceSettings.Instance.GetValue<bool>("has_z_probe") && ActiveSliceSettings.Instance.GetValue<bool>("use_z_probe") && ActiveSliceSettings.Instance.GetValue<bool>("has_z_servo"))
			{
				double value = ActiveSliceSettings.Instance.GetValue<double>("z_servo_depolyed_angle");
				PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow($"M280 P0 S{value}");
			}
			Vector3 val = ActiveSliceSettings.Instance.Helpers.ManualMovementSpeeds();
			Vector3 val2 = probeStartPosition;
			Vector2 value2 = ActiveSliceSettings.Instance.GetValue<Vector2>("z_probe_xy_offset");
			val2 -= new Vector3(value2, 0.0);
			PrinterConnectionAndCommunication.Instance.MoveAbsolute(PrinterConnectionAndCommunication.Axis.Z, probeStartPosition.z, val.z);
			PrinterConnectionAndCommunication.Instance.MoveAbsolute(val2, val.x);
			int value3 = ActiveSliceSettings.Instance.GetValue<int>("z_probe_samples");
			for (int i = 0; i < value3; i++)
			{
				PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow("G30");
				PrinterConnectionAndCommunication.Instance.MoveAbsolute(val2, val.x);
			}
			((GuiWidget)container.backButton).set_Enabled(false);
			((GuiWidget)container.nextButton).set_Enabled(false);
			if (PrinterConnectionAndCommunication.Instance.PrinterIsConnected && !PrinterConnectionAndCommunication.Instance.PrinterIsPrinting && !PrinterConnectionAndCommunication.Instance.PrinterIsPaused)
			{
				PrinterConnectionAndCommunication.Instance.ReadLine.RegisterEvent((EventHandler)GetZProbeHeight, ref unregisterEvents);
			}
		}

		public override void PageIsBecomingInactive()
		{
			PrinterConnectionAndCommunication.Instance.ReadLine.UnregisterEvent((EventHandler)GetZProbeHeight, ref unregisterEvents);
			base.PageIsBecomingInactive();
		}
	}
}
