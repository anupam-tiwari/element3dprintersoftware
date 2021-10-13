using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class SliceEngineSelector : DropDownList
	{
		public SliceEngineSelector(string label)
			: this(label, (Direction)1, 0.0, false)
		{
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			foreach (SliceEngineInfo availableSliceEngine in SlicingQueue.AvailableSliceEngines)
			{
				MenuItem obj = ((DropDownList)this).AddItem(availableSliceEngine.Name, (string)null, 12.0);
				((GuiWidget)obj).set_Enabled(ActiveSliceSettings.Instance.GetValue<int>("extruder_count") < 2 || availableSliceEngine.Name == "MatterSlice");
				SlicingEngineTypes itemEngineType = availableSliceEngine.GetSliceEngineType();
				obj.add_Selected((EventHandler)delegate
				{
					if (ActiveSliceSettings.Instance.Helpers.ActiveSliceEngineType() != itemEngineType)
					{
						ActiveSliceSettings.Instance.Helpers.ActiveSliceEngineType(itemEngineType);
						ApplicationController.Instance.ReloadAdvancedControlsPanel();
					}
				});
				if (itemEngineType == ActiveSliceSettings.Instance.Helpers.ActiveSliceEngineType())
				{
					((DropDownList)this).set_SelectedLabel(availableSliceEngine.Name);
				}
			}
			if (((DropDownList)this).get_SelectedLabel() == "")
			{
				try
				{
					((DropDownList)this).set_SelectedLabel(MatterSliceInfo.DisplayName);
				}
				catch (Exception)
				{
					throw new Exception("Unable to find MatterSlice executable");
				}
			}
			RectangleDouble localBounds = ((GuiWidget)this).get_LocalBounds();
			double width = ((RectangleDouble)(ref localBounds)).get_Width();
			localBounds = ((GuiWidget)this).get_LocalBounds();
			((GuiWidget)this).set_MinimumSize(new Vector2(width, ((RectangleDouble)(ref localBounds)).get_Height()));
		}
	}
}
