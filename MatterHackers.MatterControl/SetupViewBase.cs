using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl
{
	public class SetupViewBase : AltGroupBox
	{
		protected TextImageButtonFactory textImageButtonFactory;

		protected FlowLayoutWidget mainContainer;

		public SetupViewBase(string title)
			: base((GuiWidget)((title != "") ? new TextWidget(title, 0.0, 0.0, 18.0, (Justification)0, ActiveTheme.get_Instance().get_SecondaryAccentColor(), true, false, default(RGBA_Bytes), (TypeFace)null) : ((TextWidget)null)))
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Expected O, but got Unknown
			((GuiWidget)this).set_Margin(new BorderDouble(2.0, 10.0, 2.0, 0.0));
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(6.0, 0.0, 0.0, 6.0));
			mainContainer = val;
			((GuiWidget)this).AddChild((GuiWidget)(object)mainContainer, -1);
		}
	}
}
