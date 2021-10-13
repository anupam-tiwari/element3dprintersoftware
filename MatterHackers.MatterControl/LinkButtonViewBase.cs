using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;

namespace MatterHackers.MatterControl
{
	public class LinkButtonViewBase : GuiWidget
	{
		protected RGBA_Bytes fillColor = new RGBA_Bytes(0, 0, 0, 0);

		protected RGBA_Bytes borderColor = new RGBA_Bytes(0, 0, 0, 0);

		protected double borderWidth;

		protected double borderRadius;

		protected double padding;

		protected bool isUnderlined;

		private TextWidget buttonText;

		public RGBA_Bytes TextColor
		{
			get;
			set;
		}

		public LinkButtonViewBase(string label, double textHeight, double padding, RGBA_Bytes textColor, bool isUnderlined = false)
			: this()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Expected O, but got Unknown
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			this.padding = padding;
			TextColor = textColor;
			this.isUnderlined = isUnderlined;
			buttonText = new TextWidget(label, 0.0, 0.0, textHeight, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)buttonText).set_VAnchor((VAnchor)2);
			((GuiWidget)buttonText).set_HAnchor((HAnchor)2);
			buttonText.set_TextColor(TextColor);
			((GuiWidget)this).AddChild((GuiWidget)(object)buttonText, -1);
			((GuiWidget)this).set_HAnchor((HAnchor)8);
			((GuiWidget)this).set_VAnchor((VAnchor)8);
		}

		public override void SendToChildren(object objectToRout)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			ChangeTextColorEventArgs changeTextColorEventArgs = objectToRout as ChangeTextColorEventArgs;
			if (changeTextColorEventArgs != null)
			{
				buttonText.set_TextColor(changeTextColorEventArgs.color);
			}
			((GuiWidget)this).SendToChildren(objectToRout);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			RectangleDouble localBounds = ((GuiWidget)this).get_LocalBounds();
			RoundedRect val = new RoundedRect(localBounds, borderRadius);
			graphics2D.Render((IVertexSource)(object)val, (IColorType)(object)borderColor);
			RectangleDouble val2 = localBounds;
			((RectangleDouble)(ref val2)).Inflate(0.0 - borderWidth);
			RoundedRect val3 = new RoundedRect(val2, Math.Max(borderRadius - borderWidth, 0.0));
			graphics2D.Render((IVertexSource)(object)val3, (IColorType)(object)fillColor);
			if (isUnderlined)
			{
				RectangleDouble val4 = default(RectangleDouble);
				((RectangleDouble)(ref val4))._002Ector(((GuiWidget)this).get_LocalBounds().Left, ((GuiWidget)this).get_LocalBounds().Bottom, ((GuiWidget)this).get_LocalBounds().Right, ((GuiWidget)this).get_LocalBounds().Bottom);
				graphics2D.Rectangle(val4, buttonText.get_TextColor(), 1.0);
			}
			((GuiWidget)this).OnDraw(graphics2D);
		}
	}
}
