using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.PolygonMesh;

namespace MatterHackers.MatterControl.WellPlate
{
	public class WellPlatePartPicker : GuiWidget
	{
		private MeshViewerWidget meshViewerWidget;

		private WellPlateWidget wellPlateWidget;

		private FlowLayoutWidget thumbsTopToBottom;

		private EventHandler unregisterEvents;

		public WellPlatePartPicker(WellPlateWidget wellPlate, MeshViewerWidget meshViewer)
			: this()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Expected O, but got Unknown
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Expected O, but got Unknown
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Expected O, but got Unknown
			wellPlateWidget = wellPlate;
			meshViewerWidget = meshViewer;
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)10);
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			TextWidget val2 = new TextWidget("List Of Models".Localize(), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val2).set_HAnchor((HAnchor)2);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			ScrollableWidget val3 = new ScrollableWidget(false);
			val3.set_AutoScroll(true);
			((GuiWidget)val3).set_HAnchor((HAnchor)8);
			((GuiWidget)val3).set_VAnchor((VAnchor)5);
			((GuiWidget)val3.get_ScrollArea()).set_HAnchor((HAnchor)8);
			val3.get_VerticalScrollBar().set_Show((ShowState)0);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			thumbsTopToBottom = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)thumbsTopToBottom).set_HAnchor((HAnchor)8);
			((GuiWidget)thumbsTopToBottom).set_VAnchor((VAnchor)8);
			((GuiWidget)val3).AddChild((GuiWidget)(object)thumbsTopToBottom, -1);
			meshViewerWidget.LoadDone += MeshGroupsUpdated;
		}

		private void MeshGroupsUpdated(object sender, EventArgs e)
		{
			((GuiWidget)thumbsTopToBottom).RemoveAllChildren();
			WellPlatePart wellPlatePart = null;
			foreach (MeshGroup meshGroup in meshViewerWidget.MeshGroups)
			{
				WellPlatePart wellPlatePart2 = new WellPlatePart(wellPlateWidget, meshGroup);
				((GuiWidget)wellPlatePart2).set_HAnchor((HAnchor)2);
				WellPlatePart wellPlatePart3 = wellPlatePart2;
				if (wellPlatePart != null)
				{
					WellPlatePart wellPlatePart4 = wellPlatePart;
					wellPlatePart4.ThumbnailGenerationDone = (Action)Delegate.Combine(wellPlatePart4.ThumbnailGenerationDone, new Action(wellPlatePart3.MakeThumbnailNow));
				}
				((GuiWidget)thumbsTopToBottom).AddChild((GuiWidget)(object)wellPlatePart3, -1);
				wellPlatePart = wellPlatePart3;
			}
			if (((Collection<GuiWidget>)(object)((GuiWidget)thumbsTopToBottom).get_Children()).Count > 0)
			{
				Task.Run(delegate
				{
					(((Collection<GuiWidget>)(object)((GuiWidget)thumbsTopToBottom).get_Children())[0] as WellPlatePart).MakeThumbnailNow();
				});
			}
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			meshViewerWidget.LoadDone -= MeshGroupsUpdated;
		}
	}
}
