using System;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class GettingThirdPointFor2PointCalibration : InstructionsPage
	{
		protected Vector3 probeStartPosition;

		private ProbePosition probePosition;

		protected WizardControl container;

		private EventHandler unregisterEvents;

		public GettingThirdPointFor2PointCalibration(WizardControl container, string pageDescription, Vector3 probeStartPosition, string instructionsText, ProbePosition probePosition)
			: base(pageDescription, instructionsText)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			this.probeStartPosition = probeStartPosition;
			this.probePosition = probePosition;
			this.container = container;
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		public override void PageIsBecomingActive()
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			PrinterConnectionAndCommunication.Instance.ReadLine.UnregisterEvent((EventHandler)FinishedProbe, ref unregisterEvents);
			Vector3 val = ActiveSliceSettings.Instance.Helpers.ManualMovementSpeeds();
			PrinterConnectionAndCommunication.Instance.MoveAbsolute(PrinterConnectionAndCommunication.Axis.Z, probeStartPosition.z, val.z);
			PrinterConnectionAndCommunication.Instance.MoveAbsolute(probeStartPosition, val.x);
			PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow("G30");
			PrinterConnectionAndCommunication.Instance.ReadLine.RegisterEvent((EventHandler)FinishedProbe, ref unregisterEvents);
			base.PageIsBecomingActive();
			((GuiWidget)container.nextButton).set_Enabled(false);
			((GuiWidget)container.backButton).set_Enabled(false);
		}

		private void FinishedProbe(object sender, EventArgs e)
		{
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Expected O, but got Unknown
			StringEventArgs val = e as StringEventArgs;
			if (val != null && val.get_Data().Contains("endstops hit"))
			{
				PrinterConnectionAndCommunication.Instance.ReadLine.UnregisterEvent((EventHandler)FinishedProbe, ref unregisterEvents);
				int num = val.get_Data().LastIndexOf("Z:");
				string s = val.get_Data().Substring(num + 2);
				probePosition.position = new Vector3(probeStartPosition.x, probeStartPosition.y, double.Parse(s));
				PrinterConnectionAndCommunication.Instance.MoveAbsolute(probeStartPosition, ActiveSliceSettings.Instance.Helpers.ManualMovementSpeeds().z);
				PrinterConnectionAndCommunication.Instance.ReadPosition();
				((ButtonBase)container.nextButton).ClickButton(new MouseEventArgs((MouseButtons)1048576, 1, 0.0, 0.0, 0));
			}
		}
	}
}
