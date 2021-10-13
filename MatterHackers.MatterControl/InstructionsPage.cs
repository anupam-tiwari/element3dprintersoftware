using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl
{
	public class InstructionsPage : WizardControlPage
	{
		private double extraTextScaling = 1.0;

		protected FlowLayoutWidget topToBottomControls;

		public InstructionsPage(string pageDescription, string instructionsText)
			: base(pageDescription)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			if (UserSettings.Instance.IsTouchScreen)
			{
				extraTextScaling = 1.33333;
			}
			topToBottomControls = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)topToBottomControls).set_Padding(new BorderDouble(3.0));
			FlowLayoutWidget obj = topToBottomControls;
			((GuiWidget)obj).set_HAnchor((HAnchor)(((GuiWidget)obj).get_HAnchor() | 1));
			FlowLayoutWidget obj2 = topToBottomControls;
			((GuiWidget)obj2).set_VAnchor((VAnchor)(((GuiWidget)obj2).get_VAnchor() | 4));
			AddTextField(instructionsText, 10);
			((GuiWidget)this).AddChild((GuiWidget)(object)topToBottomControls, -1);
			((GuiWidget)this).AnchorAll();
		}

		public void AddTextField(string instructionsText, int pixelsFromLast)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected O, but got Unknown
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Expected O, but got Unknown
			GuiWidget val = new GuiWidget(10.0, (double)pixelsFromLast, (SizeLimitsToSet)1);
			((GuiWidget)topToBottomControls).AddChild(val, -1);
			string text = ((TextWrapping)new EnglishTextWrapping(12.0)).InsertCRs(instructionsText, 400.0).Replace("\t", "    ");
			RGBA_Bytes primaryTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			TextWidget val2 = new TextWidget(text, 0.0, 0.0, 12.0 * extraTextScaling, (Justification)0, primaryTextColor, true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val2).set_HAnchor((HAnchor)1);
			((GuiWidget)topToBottomControls).AddChild((GuiWidget)(object)val2, -1);
		}
	}
}
