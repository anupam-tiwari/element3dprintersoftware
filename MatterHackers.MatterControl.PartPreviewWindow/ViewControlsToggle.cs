using System;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class ViewControlsToggle : ViewControlsBase
	{
		public RadioButton twoDimensionButton;

		public RadioButton threeDimensionButton;

		private static bool userChangedTo3DThisRun;

		public ViewControlsToggle()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory
			{
				AllowThemeToAdjustImage = false,
				checkedBorderColor = RGBA_Bytes.White
			};
			((GuiWidget)this).set_BackgroundColor(new RGBA_Bytes(0, 0, 0, 120));
			textImageButtonFactory.FixedHeight = (double)buttonHeight * GuiWidget.get_DeviceScale();
			textImageButtonFactory.FixedWidth = (double)buttonHeight * GuiWidget.get_DeviceScale();
			string iconImageName = Path.Combine("ViewTransformControls", "2d.png");
			twoDimensionButton = textImageButtonFactory.GenerateRadioButton("", iconImageName);
			((GuiWidget)twoDimensionButton).set_Margin(new BorderDouble(3.0));
			((GuiWidget)this).AddChild((GuiWidget)(object)twoDimensionButton, -1);
			string iconImageName2 = Path.Combine("ViewTransformControls", "3d.png");
			threeDimensionButton = textImageButtonFactory.GenerateRadioButton("", iconImageName2);
			((GuiWidget)threeDimensionButton).set_Margin(new BorderDouble(3.0));
			if (!UserSettings.Instance.IsTouchScreen)
			{
				((GuiWidget)this).AddChild((GuiWidget)(object)threeDimensionButton, -1);
				if (UserSettings.Instance.get("LayerViewDefault") == "3D Layer" && (UserSettings.Instance.Fields.StartCountDurringExit == UserSettings.Instance.Fields.StartCount - 1 || userChangedTo3DThisRun))
				{
					threeDimensionButton.set_Checked(true);
				}
				else
				{
					twoDimensionButton.set_Checked(true);
				}
			}
			else
			{
				twoDimensionButton.set_Checked(true);
			}
			((GuiWidget)threeDimensionButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				userChangedTo3DThisRun = true;
			});
			((GuiWidget)this).set_Margin(new BorderDouble(5.0, 5.0, 200.0, 5.0));
			((GuiWidget)this).set_HAnchor((HAnchor)(((GuiWidget)this).get_HAnchor() | 4));
			((GuiWidget)this).set_VAnchor((VAnchor)4);
		}
	}
}
