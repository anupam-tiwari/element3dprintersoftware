using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class SetLayerWidget : FlowLayoutWidget
	{
		private NumberEdit editCurrentLayerIndex;

		private Button setLayerButton;

		private ViewGcodeWidget gcodeViewWidget;

		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		public SetLayerWidget(ViewGcodeWidget gcodeViewWidget)
			: this((FlowDirection)0)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Expected O, but got Unknown
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			this.gcodeViewWidget = gcodeViewWidget;
			textImageButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.disabledTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			editCurrentLayerIndex = new NumberEdit(1.0, 0.0, 0.0, 12.0, 40.0, 0.0, false, false, -2147483648.0, 2147483647.0, 1.0, 0);
			((GuiWidget)editCurrentLayerIndex).set_VAnchor((VAnchor)2);
			((GuiWidget)editCurrentLayerIndex).set_Margin(new BorderDouble(5.0, 0.0));
			((TextEditWidget)editCurrentLayerIndex).add_EditComplete((EventHandler)editCurrentLayerIndex_EditComplete);
			((GuiWidget)editCurrentLayerIndex).set_Name("Current GCode Layer Edit");
			((GuiWidget)this).AddChild((GuiWidget)(object)editCurrentLayerIndex, -1);
			gcodeViewWidget.ActiveLayerChanged += gcodeViewWidget_ActiveLayerChanged;
			setLayerButton = textImageButtonFactory.Generate("Go".Localize());
			((GuiWidget)setLayerButton).set_VAnchor((VAnchor)2);
			((GuiWidget)setLayerButton).add_Click((EventHandler<MouseEventArgs>)layerCountTextWidget_EditComplete);
			((GuiWidget)this).AddChild((GuiWidget)(object)setLayerButton, -1);
		}

		private void gcodeViewWidget_ActiveLayerChanged(object sender, EventArgs e)
		{
			editCurrentLayerIndex.set_Value((double)(gcodeViewWidget.ActiveLayerIndex + 1));
		}

		private void editCurrentLayerIndex_EditComplete(object sender, EventArgs e)
		{
			gcodeViewWidget.ActiveLayerIndex = (int)editCurrentLayerIndex.get_Value() - 1;
			editCurrentLayerIndex.set_Value((double)(gcodeViewWidget.ActiveLayerIndex + 1));
		}

		private void layerCountTextWidget_EditComplete(object sender, EventArgs e)
		{
			gcodeViewWidget.ActiveLayerIndex = (int)editCurrentLayerIndex.get_Value() - 1;
		}
	}
}
