using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.WellPlate
{
	public class RowIndicator : GuiWidget
	{
		private WellPlate2D wellPlate2D;

		private int row;

		public RowIndicator(WellPlate2D wellPlate2D, int row, double drawSize)
			: this()
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Expected O, but got Unknown
			this.wellPlate2D = wellPlate2D;
			this.row = row;
			((GuiWidget)this).set_HAnchor((HAnchor)8);
			((GuiWidget)this).set_Height(drawSize);
			string text = RowName.RowIndexToName(row);
			RGBA_Bytes primaryTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			TextWidget val = new TextWidget(text, 0.0, 0.0, 0.6 * drawSize, (Justification)0, primaryTextColor, true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val).AnchorCenter();
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		public override void OnClick(MouseEventArgs mouseEvent)
		{
			((GuiWidget)this).OnClick(mouseEvent);
			wellPlate2D.SetRowSelection(row);
		}
	}
}
