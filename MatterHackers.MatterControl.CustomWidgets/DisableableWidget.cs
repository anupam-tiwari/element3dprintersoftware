using System;
using System.Collections.ObjectModel;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.CustomWidgets
{
	public class DisableableWidget : GuiWidget
	{
		public enum EnableLevel
		{
			Disabled,
			ConfigOnly,
			Enabled
		}

		public GuiWidget disableOverlay;

		public DisableableWidget()
			: this()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected O, but got Unknown
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			((GuiWidget)this).set_VAnchor((VAnchor)8);
			((GuiWidget)this).set_Margin(new BorderDouble(3.0));
			disableOverlay = new GuiWidget(0.0, 0.0, (SizeLimitsToSet)1);
			((GuiWidget)this).add_BoundsChanged((EventHandler)delegate
			{
				EnsureCorrectBounds();
			});
			((GuiWidget)this).add_ParentChanged((EventHandler)delegate
			{
				EnsureCorrectBounds();
			});
			disableOverlay.set_Visible(false);
			((GuiWidget)this).AddChild(disableOverlay, -1);
		}

		private void EnsureCorrectBounds()
		{
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			if (((GuiWidget)this).get_Parent() != null && ((GuiWidget)this).get_Parent().get_Visible() && ((GuiWidget)this).get_Parent().get_Width() > 0.0 && ((GuiWidget)this).get_Parent().get_Height() > 0.0 && ((Collection<GuiWidget>)(object)((GuiWidget)this).get_Parent().get_Children()).Count > 1)
			{
				if (((Collection<GuiWidget>)(object)((GuiWidget)this).get_Children()).IndexOf(disableOverlay) != ((Collection<GuiWidget>)(object)((GuiWidget)this).get_Children()).Count - 1)
				{
					((Collection<GuiWidget>)(object)((GuiWidget)this).get_Children()).RemoveAt(((Collection<GuiWidget>)(object)((GuiWidget)this).get_Children()).IndexOf(disableOverlay));
					disableOverlay.ClearRemovedFlag();
					((Collection<GuiWidget>)(object)((GuiWidget)this).get_Children()).Add(disableOverlay);
				}
				RectangleDouble childrenBoundsIncludingMargins = ((GuiWidget)this).GetChildrenBoundsIncludingMargins(false, (Func<GuiWidget, GuiWidget, bool>)((GuiWidget parent, GuiWidget child) => (child != disableOverlay) ? true : false));
				if (childrenBoundsIncludingMargins != RectangleDouble.ZeroIntersection)
				{
					disableOverlay.set_LocalBounds(new RectangleDouble(childrenBoundsIncludingMargins.Left, childrenBoundsIncludingMargins.Bottom, childrenBoundsIncludingMargins.Right, childrenBoundsIncludingMargins.Top - disableOverlay.get_Margin().Top));
				}
			}
		}

		public void SetEnableLevel(EnableLevel enabledLevel)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			disableOverlay.set_BackgroundColor(new RGBA_Bytes(ActiveTheme.get_Instance().get_TertiaryBackgroundColor(), 160));
			switch (enabledLevel)
			{
			case EnableLevel.Disabled:
				disableOverlay.set_Margin(new BorderDouble(0.0));
				disableOverlay.set_Visible(true);
				break;
			case EnableLevel.ConfigOnly:
				disableOverlay.set_Margin(new BorderDouble(0.0, 0.0, 0.0, 26.0));
				disableOverlay.set_Visible(true);
				break;
			case EnableLevel.Enabled:
				disableOverlay.set_Visible(false);
				break;
			}
		}
	}
}
