using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrinterControls
{
	public class TemperatureControls : ControlWidgetBase
	{
		public List<DisableableWidget> ExtruderWidgetContainers = new List<DisableableWidget>();

		public DisableableWidget BedTemperatureControlWidget;

		public TemperatureControls()
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Expected O, but got Unknown
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			AltGroupBox altGroupBox = new AltGroupBox((GuiWidget)new TextWidget("Temperature".Localize(), 0.0, 0.0, 18.0, (Justification)0, ActiveTheme.get_Instance().get_SecondaryAccentColor(), true, false, default(RGBA_Bytes), (TypeFace)null));
			((GuiWidget)altGroupBox).set_Margin(new BorderDouble(0.0));
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 0.0));
			((GuiWidget)altGroupBox).AddChild((GuiWidget)(object)val, -1);
			RGBA_Bytes backgroundColor = default(RGBA_Bytes);
			((RGBA_Bytes)(ref backgroundColor))._002Ector(ActiveTheme.get_Instance().get_PrimaryTextColor(), 100);
			int num = ActiveSliceSettings.Instance.Helpers.NumberOfHotEnds();
			if (num > 1)
			{
				for (int i = 0; i < num; i++)
				{
					ToolSettings toolSettings = ActiveSliceSettings.Instance.Tools[i];
					if (toolSettings.toolType == TOOL_TYPE.FFF && toolSettings.position != 0)
					{
						DisableableWidget disableableWidget = new DisableableWidget();
						((GuiWidget)disableableWidget).AddChild((GuiWidget)(object)new ExtruderTemperatureControlWidget(toolSettings.position - 1, i), -1);
						((GuiWidget)val).AddChild((GuiWidget)(object)disableableWidget, -1);
						((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalLine(backgroundColor), -1);
						ExtruderWidgetContainers.Add(disableableWidget);
					}
					else if (toolSettings.toolType == TOOL_TYPE.TSYRINGE && toolSettings.temperaturePosition != 0)
					{
						DisableableWidget disableableWidget2 = new DisableableWidget();
						((GuiWidget)disableableWidget2).AddChild((GuiWidget)(object)new ExtruderTemperatureControlWidget(toolSettings.temperaturePosition + 1, i), -1);
						((GuiWidget)val).AddChild((GuiWidget)(object)disableableWidget2, -1);
						((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalLine(backgroundColor), -1);
						ExtruderWidgetContainers.Add(disableableWidget2);
					}
				}
			}
			else if (ActiveSliceSettings.Instance.Tools.Count > 0 && ActiveSliceSettings.Instance.Tools[0].toolType == TOOL_TYPE.FFF && ActiveSliceSettings.Instance.Tools[0].position != 0)
			{
				DisableableWidget disableableWidget3 = new DisableableWidget();
				((GuiWidget)disableableWidget3).AddChild((GuiWidget)(object)new ExtruderTemperatureControlWidget(ActiveSliceSettings.Instance.Tools[0].position - 1), -1);
				((GuiWidget)val).AddChild((GuiWidget)(object)disableableWidget3, -1);
				((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalLine(backgroundColor), -1);
				ExtruderWidgetContainers.Add(disableableWidget3);
			}
			else if (ActiveSliceSettings.Instance.Tools.Count > 0 && ActiveSliceSettings.Instance.Tools[0].toolType == TOOL_TYPE.TSYRINGE && ActiveSliceSettings.Instance.Tools[0].temperaturePosition != 0)
			{
				DisableableWidget disableableWidget4 = new DisableableWidget();
				((GuiWidget)disableableWidget4).AddChild((GuiWidget)(object)new ExtruderTemperatureControlWidget(ActiveSliceSettings.Instance.Tools[0].temperaturePosition + 1), -1);
				((GuiWidget)val).AddChild((GuiWidget)(object)disableableWidget4, -1);
				((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalLine(backgroundColor), -1);
				ExtruderWidgetContainers.Add(disableableWidget4);
			}
			BedTemperatureControlWidget = new DisableableWidget();
			((GuiWidget)BedTemperatureControlWidget).AddChild((GuiWidget)(object)new BedTemperatureControlWidget(), -1);
			if (ActiveSliceSettings.Instance.GetValue<bool>("has_heated_bed"))
			{
				((GuiWidget)val).AddChild((GuiWidget)(object)BedTemperatureControlWidget, -1);
			}
			((GuiWidget)this).AddChild((GuiWidget)(object)altGroupBox, -1);
		}
	}
}
