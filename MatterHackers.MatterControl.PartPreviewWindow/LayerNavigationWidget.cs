using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class LayerNavigationWidget : FlowLayoutWidget
	{
		private Button prevLayerButton;

		private Button nextLayerButton;

		private TextWidget layerCountTextWidget;

		private ViewGcodeWidget gcodeViewWidget;

		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		public LayerNavigationWidget(ViewGcodeWidget gcodeViewWidget)
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
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Expected O, but got Unknown
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			this.gcodeViewWidget = gcodeViewWidget;
			textImageButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.disabledTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			prevLayerButton = textImageButtonFactory.Generate("<<");
			((GuiWidget)prevLayerButton).add_Click((EventHandler<MouseEventArgs>)prevLayer_ButtonClick);
			((GuiWidget)this).AddChild((GuiWidget)(object)prevLayerButton, -1);
			layerCountTextWidget = new TextWidget("/1____", 12.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			layerCountTextWidget.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)layerCountTextWidget).set_VAnchor((VAnchor)2);
			layerCountTextWidget.set_AutoExpandBoundsToText(true);
			((GuiWidget)layerCountTextWidget).set_Margin(new BorderDouble(5.0, 0.0));
			((GuiWidget)this).AddChild((GuiWidget)(object)layerCountTextWidget, -1);
			nextLayerButton = textImageButtonFactory.Generate(">>");
			((GuiWidget)nextLayerButton).add_Click((EventHandler<MouseEventArgs>)nextLayer_ButtonClick);
			((GuiWidget)this).AddChild((GuiWidget)(object)nextLayerButton, -1);
		}

		private void nextLayer_ButtonClick(object sender, EventArgs mouseEvent)
		{
			gcodeViewWidget.ActiveLayerIndex += 1;
		}

		private void prevLayer_ButtonClick(object sender, EventArgs mouseEvent)
		{
			gcodeViewWidget.ActiveLayerIndex -= 1;
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			if (gcodeViewWidget.LoadedGCode != null)
			{
				((GuiWidget)layerCountTextWidget).set_Text($"{gcodeViewWidget.ActiveLayerIndex + 1} / {gcodeViewWidget.LoadedGCode.NumChangesInZ.ToString()}");
			}
			((GuiWidget)this).OnDraw(graphics2D);
		}
	}
}
