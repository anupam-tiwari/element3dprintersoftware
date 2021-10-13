using System;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class MHPasswordTextEditWidget : MHTextEditWidget
	{
		private TextEditWidget passwordCoverText;

		public bool Hidden
		{
			get
			{
				return !((GuiWidget)passwordCoverText).get_Visible();
			}
			set
			{
				((GuiWidget)passwordCoverText).set_Visible(!value);
			}
		}

		public MHPasswordTextEditWidget(string text = "", double x = 0.0, double y = 0.0, double pointSize = 12.0, double pixelWidth = 0.0, double pixelHeight = 0.0, bool multiLine = false, int tabIndex = 0, string messageWhenEmptyAndNotSelected = "")
			: base(text, x, y, pointSize, pixelWidth, pixelHeight, multiLine, tabIndex, messageWhenEmptyAndNotSelected)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected O, but got Unknown
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).RemoveChild((GuiWidget)(object)noContentFieldDescription);
			passwordCoverText = new TextEditWidget(text, x, y, pointSize, pixelWidth, pixelHeight, multiLine, 0, (TypeFace)null);
			((GuiWidget)passwordCoverText).set_Selectable(false);
			((GuiWidget)passwordCoverText).set_HAnchor((HAnchor)5);
			((GuiWidget)passwordCoverText).set_MinimumSize(new Vector2(Math.Max(((GuiWidget)passwordCoverText).get_MinimumSize().x, pixelWidth), Math.Max(((GuiWidget)passwordCoverText).get_MinimumSize().y, pixelHeight)));
			((GuiWidget)passwordCoverText).set_VAnchor((VAnchor)1);
			((GuiWidget)this).AddChild((GuiWidget)(object)passwordCoverText, -1);
			((GuiWidget)actuallTextEditWidget).add_TextChanged((EventHandler)delegate
			{
				((GuiWidget)passwordCoverText).set_Text(new string('●', ((GuiWidget)actuallTextEditWidget).get_Text().Length));
			});
			((GuiWidget)noContentFieldDescription).ClearRemovedFlag();
			((GuiWidget)this).AddChild((GuiWidget)(object)noContentFieldDescription, -1);
		}
	}
}
