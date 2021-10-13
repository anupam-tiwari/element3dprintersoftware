using System;
using System.Threading;
using System.Threading.Tasks;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.Replay
{
	public class RewindWidget : GuiWidget
	{
		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private Rewinder rewinder;

		private StartEndJogger startEndJogger;

		public RewindWidget(GCodeFile fileToReplayFrom, GCodeSelectionInfo gCodeSelectionInfo)
			: this()
		{
			RewindWidget rewindWidget = this;
			rewinder = new Rewinder(fileToReplayFrom, gCodeSelectionInfo);
			rewinder.SetStartAsCurrent();
			rewinder.MoveToCurrent();
			CreateAndAddChildren();
			((GuiWidget)this).add_Closed((EventHandler<ClosedEventArgs>)delegate
			{
				gCodeSelectionInfo.ClearSelection();
				rewindWidget.rewinder.CancelRewind();
			});
		}

		private void CreateAndAddChildren()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Expected O, but got Unknown
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).AnchorAll();
			((GuiWidget)val).set_Padding(new BorderDouble(5.0));
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			startEndJogger = new StartEndJogger(rewinder);
			((GuiWidget)startEndJogger).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((GuiWidget)startEndJogger).set_HAnchor((HAnchor)5);
			((GuiWidget)startEndJogger).set_VAnchor((VAnchor)8);
			((GuiWidget)val).AddChild((GuiWidget)(object)startEndJogger, -1);
			((GuiWidget)val).AddChild(MakeSliderAndContainer(), -1);
			GuiWidget val2 = new GuiWidget();
			val2.AnchorAll();
			((GuiWidget)val).AddChild(val2, -1);
			Button val3 = new TextImageButtonFactory
			{
				normalFillColor = RGBA_Bytes.White,
				normalTextColor = RGBA_Bytes.Black,
				hoverTextColor = RGBA_Bytes.Black,
				hoverFillColor = new RGBA_Bytes(255, 255, 255, 200)
			}.Generate("Start".Localize());
			((GuiWidget)val3).set_HAnchor((HAnchor)2);
			((GuiWidget)val3).add_Click((EventHandler<MouseEventArgs>)StartButton_Click);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
		}

		private GuiWidget MakeSliderAndContainer()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Expected O, but got Unknown
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Expected O, but got Unknown
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Expected O, but got Unknown
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Expected O, but got Unknown
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Expected O, but got Unknown
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Expected O, but got Unknown
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Expected O, but got Unknown
			//IL_02a9: Expected O, but got Unknown
			GuiWidget val = new GuiWidget();
			val.set_Width(200.0);
			val.set_HAnchor((HAnchor)2);
			val.set_VAnchor((VAnchor)8);
			val.set_Padding(new BorderDouble(0.0, 6.0));
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			val.AddChild((GuiWidget)(object)val2, -1);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val3, -1);
			TextWidget val4 = new TextWidget("Step Distance".Localize(), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val4).set_VAnchor((VAnchor)2);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val4, -1);
			GuiWidget val5 = new GuiWidget();
			val5.set_HAnchor((HAnchor)5);
			((GuiWidget)val3).AddChild(val5, -1);
			MHNumberEdit stepAmountBox = new MHNumberEdit(rewinder.StepAmount, 0.0, 0.0, 10.0, 32.0, 0.0, allowNegatives: false, allowDecimals: true, 0.25);
			((GuiWidget)stepAmountBox).set_VAnchor((VAnchor)2);
			((GuiWidget)val3).AddChild((GuiWidget)(object)stepAmountBox, -1);
			((GuiWidget)val3).AddChild(new GuiWidget(2.0, 2.0, (SizeLimitsToSet)1), -1);
			TextWidget val6 = new TextWidget("mm".Localize(), 0.0, 0.0, 8.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val6).set_VAnchor((VAnchor)2);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val6, -1);
			((GuiWidget)val2).AddChild(new GuiWidget(2.0, 2.0, (SizeLimitsToSet)1), -1);
			SolidSlider stepAmountSlider = new SolidSlider(default(Vector2), 10.0, 0.25, 20.0, (Orientation)0);
			((GuiWidget)stepAmountSlider).set_HAnchor((HAnchor)5);
			stepAmountSlider.Value = rewinder.StepAmount;
			((GuiWidget)val2).AddChild((GuiWidget)(object)stepAmountSlider, -1);
			((GuiWidget)val2).AddChild(new GuiWidget(4.0, 4.0, (SizeLimitsToSet)1), -1);
			EventHandler sliderValueChanged = delegate
			{
				stepAmountBox.ActuallNumberEdit.set_Value(stepAmountSlider.Value);
				rewinder.StepAmount = stepAmountSlider.Value;
			};
			stepAmountSlider.ValueChanged += sliderValueChanged;
			((TextEditWidget)stepAmountBox.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				stepAmountSlider.ValueChanged -= sliderValueChanged;
				stepAmountSlider.Value = stepAmountBox.ActuallNumberEdit.get_Value();
				stepAmountSlider.ValueChanged += sliderValueChanged;
				rewinder.StepAmount = stepAmountBox.ActuallNumberEdit.get_Value();
			});
			return val;
		}

		private async void StartButton_Click(object sender, EventArgs e)
		{
			GuiWidget cover = new GuiWidget();
			cover.AnchorAll();
			cover.set_BackgroundColor(new RGBA_Bytes(0, 0, 0, 200));
			cover.set_BackgroundColor(RGBA_Bytes.Black);
			((GuiWidget)this).AddChild(cover, -1);
			TextWidget processingMessage2 = new TextWidget("Rewind in progress...".Localize(), 0.0, 0.0, 20.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)processingMessage2).AnchorCenter();
			cover.AddChild((GuiWidget)(object)processingMessage2, -1);
			bool canceled = false;
			Button cancelButton = textImageButtonFactory.Generate("Cancel".Localize());
			((GuiWidget)cancelButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				canceled = true;
				rewinder.CancelRewind();
			});
			((GuiWidget)cancelButton).set_HAnchor((HAnchor)4);
			((GuiWidget)cancelButton).set_VAnchor((VAnchor)1);
			((GuiWidget)cancelButton).set_Margin(new BorderDouble(10.0, 4.0));
			cover.AddChild((GuiWidget)(object)cancelButton, -1);
			await Task.Run(delegate
			{
				rewinder.DoRewindNow();
			});
			if (!canceled)
			{
				cover.RemoveChild((GuiWidget)(object)processingMessage2);
				cover.RemoveChild((GuiWidget)(object)cancelButton);
				processingMessage2 = new TextWidget("Rewind complete".Localize(), 0.0, 0.0, 20.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
				((GuiWidget)processingMessage2).AnchorCenter();
				cover.AddChild((GuiWidget)(object)processingMessage2, -1);
				if (PrinterConnectionAndCommunication.Instance.PrinterIsPaused)
				{
					Button val = textImageButtonFactory.Generate("Resume Print".Localize());
					((GuiWidget)val).set_HAnchor((HAnchor)1);
					((GuiWidget)val).set_VAnchor((VAnchor)1);
					((GuiWidget)val).set_Margin(new BorderDouble(10.0, 4.0));
					((GuiWidget)val).add_Click((EventHandler<MouseEventArgs>)delegate
					{
						PrinterConnectionAndCommunication.Instance.Resume();
						((GuiWidget)this).Close();
					});
					cover.AddChild((GuiWidget)(object)val, -1);
				}
				bool continueAfterFinished = false;
				Button val2 = textImageButtonFactory.Generate("OK".Localize());
				((GuiWidget)val2).add_Click((EventHandler<MouseEventArgs>)delegate
				{
					continueAfterFinished = true;
				});
				((GuiWidget)val2).set_HAnchor((HAnchor)4);
				((GuiWidget)val2).set_VAnchor((VAnchor)1);
				((GuiWidget)val2).set_Margin(new BorderDouble(10.0, 4.0));
				cover.AddChild((GuiWidget)(object)val2, -1);
				await Task.Run(delegate
				{
					while (!continueAfterFinished && !((GuiWidget)this).get_HasBeenClosed())
					{
						Thread.Sleep(0);
					}
				});
			}
			((GuiWidget)this).RemoveChild(cover);
		}
	}
}
