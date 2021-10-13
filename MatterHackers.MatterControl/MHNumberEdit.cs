using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl
{
	public class MHNumberEdit : GuiWidget
	{
		private NumberEdit actuallNumberEdit;

		public NumberEdit ActuallNumberEdit => actuallNumberEdit;

		public override int TabIndex
		{
			get
			{
				return ((GuiWidget)this).get_TabIndex();
			}
			set
			{
				((GuiWidget)actuallNumberEdit).set_TabIndex(value);
			}
		}

		public override string Text
		{
			get
			{
				return ((GuiWidget)actuallNumberEdit).get_Text();
			}
			set
			{
				((GuiWidget)actuallNumberEdit).set_Text(value);
			}
		}

		public bool SelectAllOnFocus
		{
			get
			{
				return ((InternalTextEditWidget)ActuallNumberEdit.get_InternalNumberEdit()).get_SelectAllOnFocus();
			}
			set
			{
				((InternalTextEditWidget)ActuallNumberEdit.get_InternalNumberEdit()).set_SelectAllOnFocus(value);
			}
		}

		public MHNumberEdit(double startingValue, double x = 0.0, double y = 0.0, double pointSize = 12.0, double pixelWidth = 0.0, double pixelHeight = 0.0, bool allowNegatives = false, bool allowDecimals = false, double minValue = -2147483648.0, double maxValue = 2147483647.0, double increment = 1.0, int tabIndex = 0)
			: this()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected O, but got Unknown
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_Padding(new BorderDouble(3.0));
			actuallNumberEdit = new NumberEdit(startingValue, x, y, pointSize, pixelWidth, pixelHeight, allowNegatives, allowDecimals, minValue, maxValue, increment, tabIndex);
			((GuiWidget)actuallNumberEdit).set_VAnchor((VAnchor)1);
			((GuiWidget)this).AddChild((GuiWidget)(object)actuallNumberEdit, -1);
			((GuiWidget)this).set_BackgroundColor(RGBA_Bytes.White);
			((GuiWidget)this).set_HAnchor((HAnchor)8);
			((GuiWidget)this).set_VAnchor((VAnchor)8);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).OnDraw(graphics2D);
			if (((GuiWidget)this).get_ContainsFocus())
			{
				graphics2D.Rectangle(((GuiWidget)this).get_LocalBounds(), RGBA_Bytes.Orange, 1.0);
			}
		}
	}
}
