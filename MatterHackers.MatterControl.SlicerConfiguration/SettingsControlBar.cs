using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class SettingsControlBar : FlowLayoutWidget
	{
		private class MaterialSettingListControl : ScrollableWidget
		{
			private FlowLayoutWidget topToBottomItemList;

			public MaterialSettingListControl()
				: this(false)
			{
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Expected O, but got Unknown
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				((GuiWidget)this).AnchorAll();
				((ScrollableWidget)this).set_AutoScroll(true);
				ScrollingArea scrollArea = ((ScrollableWidget)this).get_ScrollArea();
				((GuiWidget)scrollArea).set_HAnchor((HAnchor)(((GuiWidget)scrollArea).get_HAnchor() | 5));
				topToBottomItemList = new FlowLayoutWidget((FlowDirection)3);
				((GuiWidget)topToBottomItemList).set_HAnchor((HAnchor)13);
				((GuiWidget)topToBottomItemList).set_Margin(new BorderDouble(3.0, 0.0, 0.0, 0.0));
				((ScrollableWidget)this).AddChild((GuiWidget)(object)topToBottomItemList, -1);
			}

			public override void AddChild(GuiWidget child, int indexInChildrenList = -1)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
				((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 0.0));
				((GuiWidget)val).set_HAnchor((HAnchor)13);
				((GuiWidget)val).AddChild(child, -1);
				((GuiWidget)val).set_VAnchor((VAnchor)8);
				((GuiWidget)topToBottomItemList).AddChild((GuiWidget)(object)val, indexInChildrenList);
			}
		}

		public SettingsControlBar()
			: this((FlowDirection)0)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Expected O, but got Unknown
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Expected O, but got Unknown
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			new FlowLayoutWidget((FlowDirection)0);
			MaterialSettingListControl materialSettingListControl = new MaterialSettingListControl();
			((GuiWidget)materialSettingListControl).AnchorAll();
			((GuiWidget)((ScrollableWidget)materialSettingListControl).get_ScrollArea()).set_HAnchor((HAnchor)5);
			int num = ActiveSliceSettings.Instance.Helpers.NumberOfHotEnds();
			((GuiWidget)this).AddChild((GuiWidget)(object)new PresetSelectorWidget("Quality".Localize(), RGBA_Bytes.Yellow, NamedSettingsLayers.Quality, 0), -1);
			((GuiWidget)this).AddChild(new GuiWidget(8.0, 0.0, (SizeLimitsToSet)1), -1);
			if (num > 1)
			{
				List<RGBA_Bytes> list = new List<RGBA_Bytes>
				{
					RGBA_Bytes.Orange,
					RGBA_Bytes.Violet,
					RGBA_Bytes.YellowGreen,
					RGBA_Bytes.White,
					RGBA_Bytes.Red,
					RGBA_Bytes.Black
				};
				for (int i = 0; i < num; i++)
				{
					if (i > 0)
					{
						((GuiWidget)materialSettingListControl).AddChild(new GuiWidget(0.0, 8.0, (SizeLimitsToSet)1), -1);
					}
					int index = i % list.Count;
					RGBA_Bytes accentColor = list[index];
					((GuiWidget)materialSettingListControl).AddChild((GuiWidget)(object)new PresetSelectorWidget(string.Format("{0} {1}", "Material".Localize(), i + 1), accentColor, NamedSettingsLayers.Material, i), -1);
				}
				((GuiWidget)this).AddChild((GuiWidget)(object)materialSettingListControl, -1);
			}
			else
			{
				((GuiWidget)this).AddChild((GuiWidget)(object)new PresetSelectorWidget("Material".Localize(), RGBA_Bytes.Orange, NamedSettingsLayers.Material, 0), -1);
			}
			((GuiWidget)this).set_Height(60.0 * GuiWidget.get_DeviceScale());
		}
	}
}
