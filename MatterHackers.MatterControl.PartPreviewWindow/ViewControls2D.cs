using System;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class ViewControls2D : ViewControlsBase
	{
		private Button resetViewButton;

		public RadioButton translateButton;

		public RadioButton scaleButton;

		public event EventHandler ResetView;

		public ViewControls2D()
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			if (UserSettings.Instance.IsTouchScreen)
			{
				buttonHeight = 40;
			}
			else
			{
				buttonHeight = 0;
			}
			TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory
			{
				AllowThemeToAdjustImage = false,
				checkedBorderColor = RGBA_Bytes.White
			};
			((GuiWidget)this).set_BackgroundColor(new RGBA_Bytes(0, 0, 0, 120));
			textImageButtonFactory.FixedHeight = (double)buttonHeight * GuiWidget.get_DeviceScale();
			textImageButtonFactory.FixedWidth = (double)buttonHeight * GuiWidget.get_DeviceScale();
			string text = Path.Combine("ViewTransformControls", "reset.png");
			resetViewButton = textImageButtonFactory.Generate("", ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon(text, 32, 32)));
			((GuiWidget)resetViewButton).set_ToolTipText("Reset View".Localize());
			((GuiWidget)this).AddChild((GuiWidget)(object)resetViewButton, -1);
			((GuiWidget)resetViewButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					this.ResetView?.Invoke(this, null);
				});
			});
			string text2 = Path.Combine("ViewTransformControls", "translate.png");
			translateButton = textImageButtonFactory.GenerateRadioButton("", StaticData.get_Instance().LoadIcon(text2, 32, 32));
			((GuiWidget)translateButton).set_ToolTipText("Move".Localize());
			((GuiWidget)translateButton).set_Margin(new BorderDouble(3.0));
			((GuiWidget)this).AddChild((GuiWidget)(object)translateButton, -1);
			string text3 = Path.Combine("ViewTransformControls", "scale.png");
			scaleButton = textImageButtonFactory.GenerateRadioButton("", StaticData.get_Instance().LoadIcon(text3, 32, 32));
			((GuiWidget)scaleButton).set_ToolTipText("Zoom".Localize());
			((GuiWidget)scaleButton).set_Margin(new BorderDouble(3.0));
			((GuiWidget)this).AddChild((GuiWidget)(object)scaleButton, -1);
			((GuiWidget)this).set_Margin(new BorderDouble(5.0));
			((GuiWidget)this).set_HAnchor((HAnchor)(((GuiWidget)this).get_HAnchor() | 1));
			((GuiWidget)this).set_VAnchor((VAnchor)4);
			translateButton.set_Checked(true);
		}
	}
}
