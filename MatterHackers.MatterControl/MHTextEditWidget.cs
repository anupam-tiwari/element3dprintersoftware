using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class MHTextEditWidget : GuiWidget
	{
		protected TextEditWidget actuallTextEditWidget;

		protected TextWidget noContentFieldDescription;

		public TextEditWidget ActualTextEditWidget => actuallTextEditWidget;

		public override string Text
		{
			get
			{
				return ((GuiWidget)actuallTextEditWidget).get_Text();
			}
			set
			{
				((GuiWidget)actuallTextEditWidget).set_Text(value);
			}
		}

		public bool SelectAllOnFocus
		{
			get
			{
				return actuallTextEditWidget.get_InternalTextEditWidget().get_SelectAllOnFocus();
			}
			set
			{
				actuallTextEditWidget.get_InternalTextEditWidget().set_SelectAllOnFocus(value);
			}
		}

		public MHTextEditWidget(string text = "", double x = 0.0, double y = 0.0, double pointSize = 12.0, double pixelWidth = 0.0, double pixelHeight = 0.0, bool multiLine = false, int tabIndex = 0, string messageWhenEmptyAndNotSelected = "", TypeFace typeFace = null)
			: this()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Expected O, but got Unknown
			((GuiWidget)this).set_Padding(new BorderDouble(3.0));
			actuallTextEditWidget = new TextEditWidget(text, x, y, pointSize, pixelWidth, pixelHeight, multiLine, tabIndex, typeFace);
			((GuiWidget)actuallTextEditWidget).set_HAnchor((HAnchor)5);
			((GuiWidget)actuallTextEditWidget).set_MinimumSize(new Vector2(Math.Max(((GuiWidget)actuallTextEditWidget).get_MinimumSize().x, pixelWidth), Math.Max(((GuiWidget)actuallTextEditWidget).get_MinimumSize().y, pixelHeight)));
			((GuiWidget)actuallTextEditWidget).set_VAnchor((VAnchor)1);
			((GuiWidget)this).AddChild((GuiWidget)(object)actuallTextEditWidget, -1);
			((GuiWidget)this).set_BackgroundColor(RGBA_Bytes.White);
			((GuiWidget)this).set_HAnchor((HAnchor)8);
			((GuiWidget)this).set_VAnchor((VAnchor)8);
			noContentFieldDescription = new TextWidget(messageWhenEmptyAndNotSelected, 0.0, 0.0, 12.0, (Justification)0, RGBA_Bytes.Gray, true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)noContentFieldDescription).set_VAnchor((VAnchor)1);
			noContentFieldDescription.set_AutoExpandBoundsToText(true);
			((GuiWidget)this).AddChild((GuiWidget)(object)noContentFieldDescription, -1);
			SetNoContentFieldDescriptionVisibility();
		}

		private void SetNoContentFieldDescriptionVisibility()
		{
			if (noContentFieldDescription != null)
			{
				if (((GuiWidget)this).get_Text() == "")
				{
					((GuiWidget)noContentFieldDescription).set_Visible(true);
				}
				else
				{
					((GuiWidget)noContentFieldDescription).set_Visible(false);
				}
			}
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			SetNoContentFieldDescriptionVisibility();
			((GuiWidget)this).OnDraw(graphics2D);
			if (((GuiWidget)this).get_ContainsFocus())
			{
				graphics2D.Rectangle(((GuiWidget)this).get_LocalBounds(), RGBA_Bytes.Orange, 1.0);
			}
		}

		public override void Focus()
		{
			((GuiWidget)actuallTextEditWidget).Focus();
		}

		public void DrawFromHintedCache()
		{
			ActualTextEditWidget.get_Printer().set_DrawFromHintedCache(true);
			ActualTextEditWidget.set_DoubleBuffer(false);
		}
	}
}
