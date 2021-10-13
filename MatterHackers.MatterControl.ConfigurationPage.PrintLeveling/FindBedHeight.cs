using System;
using System.Collections.Generic;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class FindBedHeight : InstructionsPage
	{
		private Vector3 lastReportedPosition;

		private List<ProbePosition> probePositions;

		private int probePositionsBeingEditedIndex;

		private double moveAmount;

		protected JogControls.MoveButton zPlusControl;

		protected JogControls.MoveButton zMinusControl;

		protected WizardControl container;

		public FindBedHeight(WizardControl container, string pageDescription, string setZHeightCoarseInstruction1, string setZHeightCoarseInstruction2, double moveDistance, List<ProbePosition> probePositions, int probePositionsBeingEditedIndex)
			: base(pageDescription, setZHeightCoarseInstruction1)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Expected O, but got Unknown
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Expected O, but got Unknown
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Expected O, but got Unknown
			this.container = container;
			this.probePositions = probePositions;
			moveAmount = moveDistance;
			lastReportedPosition = PrinterConnectionAndCommunication.Instance.LastReportedPosition;
			this.probePositionsBeingEditedIndex = probePositionsBeingEditedIndex;
			GuiWidget val = new GuiWidget(15.0, 15.0, (SizeLimitsToSet)1);
			((GuiWidget)topToBottomControls).AddChild(val, -1);
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)(((GuiWidget)val2).get_HAnchor() | 2));
			FlowLayoutWidget val3 = CreateZButtons();
			((GuiWidget)val2).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val2).AddChild(new GuiWidget(15.0, 10.0, (SizeLimitsToSet)1), -1);
			TextWidget val4 = new TextWidget("Z: 0.0      ", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val4).set_VAnchor((VAnchor)2);
			((GuiWidget)val4).set_Margin(new BorderDouble(10.0, 0.0));
			TextWidget zPosition = val4;
			Action<TextWidget> updateUntilClose = null;
			updateUntilClose = delegate
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				Vector3 currentDestination = PrinterConnectionAndCommunication.Instance.CurrentDestination;
				((GuiWidget)zPosition).set_Text(StringHelper.FormatWith("Z: {0:0.00}", new object[1]
				{
					currentDestination.z
				}));
				UiThread.RunOnIdle((Action)delegate
				{
					updateUntilClose(zPosition);
				}, 0.3);
			};
			updateUntilClose(zPosition);
			((GuiWidget)val2).AddChild((GuiWidget)(object)zPosition, -1);
			((GuiWidget)topToBottomControls).AddChild((GuiWidget)(object)val2, -1);
			AddTextField(setZHeightCoarseInstruction2, 10);
		}

		public override void PageIsBecomingActive()
		{
			ActiveSliceSettings.Instance.Helpers.DoPrintLeveling(doLeveling: false);
			base.PageIsBecomingActive();
			((GuiWidget)Enumerable.First<SystemWindow>(ExtensionMethods.Parents<SystemWindow>((GuiWidget)(object)this))).add_KeyDown((EventHandler<KeyEventArgs>)TopWindowKeyDown);
		}

		public override void PageIsBecomingInactive()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)Enumerable.First<SystemWindow>(ExtensionMethods.Parents<SystemWindow>((GuiWidget)(object)this))).remove_KeyDown((EventHandler<KeyEventArgs>)TopWindowKeyDown);
			probePositions[probePositionsBeingEditedIndex].position = PrinterConnectionAndCommunication.Instance.LastReportedPosition;
			base.PageIsBecomingInactive();
		}

		private FlowLayoutWidget CreateZButtons()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget result = JogControls.CreateZButtons(RGBA_Bytes.White, 4.0, out zPlusControl, out zMinusControl, levelingButtons: true);
			zPlusControl.MoveAmount = 0.0;
			zMinusControl.MoveAmount = 0.0;
			((GuiWidget)zPlusControl).add_Click((EventHandler<MouseEventArgs>)zPlusControl_Click);
			((GuiWidget)zMinusControl).add_Click((EventHandler<MouseEventArgs>)zMinusControl_Click);
			return result;
		}

		public void TopWindowKeyDown(object s, KeyEventArgs keyEvent)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected I4, but got Unknown
			Keys keyCode = keyEvent.get_KeyCode();
			switch (keyCode - 37)
			{
			case 1:
				zPlusControl_Click(null, null);
				((GuiWidget)container.nextButton).set_Enabled(true);
				break;
			case 3:
				zMinusControl_Click(null, null);
				((GuiWidget)container.nextButton).set_Enabled(true);
				break;
			case 2:
				if (((GuiWidget)container.nextButton).get_Enabled())
				{
					UiThread.RunOnIdle((Action)delegate
					{
						((ButtonBase)container.nextButton).ClickButton((MouseEventArgs)null);
					});
				}
				break;
			case 0:
				if (((GuiWidget)container.backButton).get_Enabled())
				{
					UiThread.RunOnIdle((Action)delegate
					{
						((ButtonBase)container.backButton).ClickButton((MouseEventArgs)null);
					});
				}
				break;
			}
			((GuiWidget)this).OnKeyDown(keyEvent);
		}

		private void zMinusControl_Click(object sender, EventArgs mouseEvent)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			PrinterConnectionAndCommunication.Instance.MoveRelative(PrinterConnectionAndCommunication.Axis.Z, 0.0 - moveAmount, ActiveSliceSettings.Instance.Helpers.ManualMovementSpeeds().z);
			PrinterConnectionAndCommunication.Instance.ReadPosition();
		}

		private void zPlusControl_Click(object sender, EventArgs mouseEvent)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			PrinterConnectionAndCommunication.Instance.MoveRelative(PrinterConnectionAndCommunication.Axis.Z, moveAmount, ActiveSliceSettings.Instance.Helpers.ManualMovementSpeeds().z);
			PrinterConnectionAndCommunication.Instance.ReadPosition();
		}
	}
}
