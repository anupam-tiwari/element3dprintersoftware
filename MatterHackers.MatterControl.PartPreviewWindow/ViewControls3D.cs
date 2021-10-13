using System;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class ViewControls3D : FlowLayoutWidget
	{
		private GuiWidget partSelectSeparator;

		private MeshViewerWidget meshViewerWidget;

		private Button resetViewButton;

		private RadioButton translateButton;

		private RadioButton rotateButton;

		private RadioButton scaleButton;

		private RadioButton partSelectButton;

		private int buttonHeight;

		private ViewControls3DButtons activeTransformState;

		private EventHandler unregisterEvents;

		public bool PartSelectVisible
		{
			get
			{
				return partSelectSeparator.get_Visible();
			}
			set
			{
				partSelectSeparator.set_Visible(value);
				((GuiWidget)partSelectButton).set_Visible(value);
			}
		}

		public ViewControls3DButtons ActiveButton
		{
			get
			{
				return activeTransformState;
			}
			set
			{
				activeTransformState = value;
				switch (value)
				{
				case ViewControls3DButtons.Rotate:
					meshViewerWidget.TrackballTumbleWidget.TransformState = (MouseDownType)2;
					rotateButton.set_Checked(true);
					break;
				case ViewControls3DButtons.Translate:
					meshViewerWidget.TrackballTumbleWidget.TransformState = (MouseDownType)1;
					translateButton.set_Checked(true);
					break;
				case ViewControls3DButtons.Scale:
					meshViewerWidget.TrackballTumbleWidget.TransformState = (MouseDownType)3;
					scaleButton.set_Checked(true);
					break;
				case ViewControls3DButtons.PartSelect:
					meshViewerWidget.TrackballTumbleWidget.TransformState = (MouseDownType)0;
					partSelectButton.set_Checked(true);
					break;
				}
			}
		}

		public event EventHandler ResetView;

		public ViewControls3D(MeshViewerWidget meshViewerWidget)
			: this((FlowDirection)0)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Expected O, but got Unknown
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Expected O, but got Unknown
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			if (UserSettings.Instance.IsTouchScreen)
			{
				buttonHeight = 40;
			}
			else
			{
				buttonHeight = 0;
			}
			this.meshViewerWidget = meshViewerWidget;
			TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory
			{
				normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor(),
				hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor(),
				disabledTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor(),
				pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor()
			};
			((GuiWidget)this).set_BackgroundColor(new RGBA_Bytes(0, 0, 0, 120));
			textImageButtonFactory.FixedHeight = (double)buttonHeight * GuiWidget.get_DeviceScale();
			textImageButtonFactory.FixedWidth = (double)buttonHeight * GuiWidget.get_DeviceScale();
			textImageButtonFactory.AllowThemeToAdjustImage = false;
			textImageButtonFactory.checkedBorderColor = RGBA_Bytes.White;
			string text = Path.Combine("ViewTransformControls", "reset.png");
			resetViewButton = textImageButtonFactory.Generate("", ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon(text, 32, 32)));
			((GuiWidget)resetViewButton).set_ToolTipText("Reset View".Localize());
			((GuiWidget)this).AddChild((GuiWidget)(object)resetViewButton, -1);
			((GuiWidget)resetViewButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				this.ResetView?.Invoke(this, null);
			});
			string text2 = Path.Combine("ViewTransformControls", "rotate.png");
			rotateButton = textImageButtonFactory.GenerateRadioButton("", StaticData.get_Instance().LoadIcon(text2, 32, 32));
			((GuiWidget)rotateButton).set_ToolTipText("Rotate (Alt + Left Mouse)".Localize());
			((GuiWidget)rotateButton).set_Margin(new BorderDouble(3.0));
			((GuiWidget)this).AddChild((GuiWidget)(object)rotateButton, -1);
			((GuiWidget)rotateButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				ActiveButton = ViewControls3DButtons.Rotate;
			});
			string text3 = Path.Combine("ViewTransformControls", "translate.png");
			translateButton = textImageButtonFactory.GenerateRadioButton("", StaticData.get_Instance().LoadIcon(text3, 32, 32));
			((GuiWidget)translateButton).set_ToolTipText("Move (Shift + Left Mouse)".Localize());
			((GuiWidget)translateButton).set_Margin(new BorderDouble(3.0));
			((GuiWidget)this).AddChild((GuiWidget)(object)translateButton, -1);
			((GuiWidget)translateButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				ActiveButton = ViewControls3DButtons.Translate;
			});
			string text4 = Path.Combine("ViewTransformControls", "scale.png");
			scaleButton = textImageButtonFactory.GenerateRadioButton("", StaticData.get_Instance().LoadIcon(text4, 32, 32));
			((GuiWidget)scaleButton).set_ToolTipText("Zoom (Ctrl + Left Mouse)".Localize());
			((GuiWidget)scaleButton).set_Margin(new BorderDouble(3.0));
			((GuiWidget)this).AddChild((GuiWidget)(object)scaleButton, -1);
			((GuiWidget)scaleButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				ActiveButton = ViewControls3DButtons.Scale;
			});
			partSelectSeparator = new GuiWidget(2.0, 32.0, (SizeLimitsToSet)1);
			partSelectSeparator.set_BackgroundColor(RGBA_Bytes.White);
			partSelectSeparator.set_Margin(new BorderDouble(3.0));
			((GuiWidget)this).AddChild(partSelectSeparator, -1);
			string text5 = Path.Combine("ViewTransformControls", "partSelect.png");
			partSelectButton = textImageButtonFactory.GenerateRadioButton("", StaticData.get_Instance().LoadIcon(text5, 32, 32));
			((GuiWidget)partSelectButton).set_ToolTipText("Select Part".Localize());
			((GuiWidget)partSelectButton).set_Margin(new BorderDouble(3.0));
			((GuiWidget)this).AddChild((GuiWidget)(object)partSelectButton, -1);
			((GuiWidget)partSelectButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				ActiveButton = ViewControls3DButtons.PartSelect;
			});
			GuiWidget val = new GuiWidget(2.0, 32.0, (SizeLimitsToSet)1);
			val.set_BackgroundColor(RGBA_Bytes.White);
			val.set_Margin(new BorderDouble(3.0));
			((GuiWidget)this).AddChild(val, -1);
			string text6 = Path.Combine("ViewTransformControls", "grid.png");
			ImageBuffer val2 = StaticData.get_Instance().LoadIcon(text6, 32, 32);
			string text7 = Path.Combine("ViewTransformControls", "grid-click.png");
			ImageBuffer val3 = StaticData.get_Instance().LoadIcon(text7, 32, 32);
			if (ActiveTheme.get_Instance().get_IsDarkTheme())
			{
				ExtensionMethods.InvertLightness(val2);
				ExtensionMethods.InvertLightness(val3);
			}
			CheckBox gridToggleButton = textImageButtonFactory.GenerateCheckBoxButton("", val2, val3);
			((GuiWidget)gridToggleButton).set_ToolTipText("Toggle Bed Grid".Localize());
			((GuiWidget)gridToggleButton).set_Margin(new BorderDouble(3.0));
			((GuiWidget)this).AddChild((GuiWidget)(object)gridToggleButton, -1);
			gridToggleButton.set_Checked(MeshViewerWidget.RenderGrid);
			gridToggleButton.add_CheckedStateChanged((EventHandler)delegate
			{
				MeshViewerWidget.RenderGrid = gridToggleButton.get_Checked();
			});
			((GuiWidget)this).set_Margin(new BorderDouble(5.0));
			((GuiWidget)this).set_HAnchor((HAnchor)(((GuiWidget)this).get_HAnchor() | 1));
			((GuiWidget)this).set_VAnchor((VAnchor)4);
			rotateButton.set_Checked(true);
			SetMeshViewerDisplayTheme();
			partSelectButton.add_CheckedStateChanged((EventHandler)SetMeshViewerDisplayTheme);
			ActiveTheme.ThemeChanged.RegisterEvent((EventHandler)ThemeChanged, ref unregisterEvents);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((GuiWidget)this).OnClosed(e);
		}

		public void ThemeChanged(object sender, EventArgs e)
		{
			SetMeshViewerDisplayTheme();
		}

		protected void SetMeshViewerDisplayTheme(object sender = null, EventArgs e = null)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			meshViewerWidget.TrackballTumbleWidget.RotationHelperCircleColor = ActiveTheme.get_Instance().get_PrimaryBackgroundColor();
			MeshViewerWidget obj = meshViewerWidget;
			RGBA_Bytes primaryAccentColor = ActiveTheme.get_Instance().get_PrimaryAccentColor();
			int red0To = ((RGBA_Bytes)(ref primaryAccentColor)).get_Red0To255();
			primaryAccentColor = ActiveTheme.get_Instance().get_PrimaryAccentColor();
			int green0To = ((RGBA_Bytes)(ref primaryAccentColor)).get_Green0To255();
			primaryAccentColor = ActiveTheme.get_Instance().get_PrimaryAccentColor();
			obj.BuildVolumeColor = new RGBA_Bytes(red0To, green0To, ((RGBA_Bytes)(ref primaryAccentColor)).get_Blue0To255(), 50);
		}
	}
}
