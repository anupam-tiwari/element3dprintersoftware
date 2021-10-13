using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.WellPlate
{
	public class ColIndicator : GuiWidget
	{
		private WellPlate2D wellPlate2D;

		private int col;

		public ColIndicator(WellPlate2D wellPlate2D, int col, double drawSize)
			: this()
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Expected O, but got Unknown
			this.wellPlate2D = wellPlate2D;
			this.col = col;
			((GuiWidget)this).set_Width(drawSize);
			((GuiWidget)this).set_VAnchor((VAnchor)8);
			string text = (col + 1).ToString();
			RGBA_Bytes primaryTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			TextWidget val = new TextWidget(text, 0.0, 0.0, 0.6 * drawSize, (Justification)0, primaryTextColor, true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val).AnchorCenter();
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		public override void OnClick(MouseEventArgs mouseEvent)
		{
			((GuiWidget)this).OnClick(mouseEvent);
			wellPlate2D.SetColSelection(col);
		}
	}
}
