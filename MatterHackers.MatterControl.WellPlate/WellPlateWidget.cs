using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.MatterControl.PartPreviewWindow;
using MatterHackers.MatterControl.PrintQueue;

namespace MatterHackers.MatterControl.WellPlate
{
	public class WellPlateWidget : FlowLayoutWidget
	{
		public RootedObjectEventHandler PartPicked = new RootedObjectEventHandler();

		public RootedObjectEventHandler WellPicked = new RootedObjectEventHandler();

		public RootedObjectEventHandler WellPlateChanged = new RootedObjectEventHandler();

		private MeshViewerWidget meshViewerWidget;

		private WellPlateTypeWidget wellPlateTypeWidget;

		private WellPlate2D wellPlate2D;

		private Button commitChangesButton;

		private Button clearAllButton;

		private Button selectAllWellsButton;

		private Button clearSelectedWellParts;

		private Button clearSelectionButton;

		private WellPlateMeshManipulator meshManipulator;

		private List<Tuple<int, int>> selectedWells = new List<Tuple<int, int>>();

		private WellPlatePart selectedWellPlatePart;

		private bool shouldCheckForSelectioinPairs;

		public bool ShouldCheckForSelectioinPairs
		{
			get
			{
				return shouldCheckForSelectioinPairs;
			}
			set
			{
				shouldCheckForSelectioinPairs = value;
				if (shouldCheckForSelectioinPairs)
				{
					CheckForSelectionPairs();
				}
			}
		}

		public bool DoHighlighting
		{
			get;
			set;
		} = true;


		public WellPlateWidget(MeshViewerWidget meshViewer)
			: this((FlowDirection)0)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			meshViewerWidget = meshViewer;
			shouldCheckForSelectioinPairs = true;
			CreateAndAddChildren();
			meshManipulator = new WellPlateMeshManipulator(meshViewerWidget, wellPlateTypeWidget);
		}

		private void CreateAndAddChildren()
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Expected O, but got Unknown
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Expected O, but got Unknown
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Expected O, but got Unknown
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Expected O, but got Unknown
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			BorderDouble margin = default(BorderDouble);
			((BorderDouble)(ref margin))._002Ector(2.0);
			BorderDouble padding = default(BorderDouble);
			((BorderDouble)(ref padding))._002Ector(4.0);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((GuiWidget)val).set_Margin(margin);
			((GuiWidget)val).set_Padding(padding);
			FlowLayoutWidget val2 = val;
			((GuiWidget)val2).AnchorAll();
			wellPlate2D = new WellPlate2D(this, meshViewerWidget);
			((GuiWidget)wellPlate2D).AnchorAll();
			((GuiWidget)val2).AddChild((GuiWidget)(object)wellPlate2D, -1);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_VAnchor((VAnchor)9);
			FlowLayoutWidget val4 = val3;
			((GuiWidget)val2).AddChild((GuiWidget)(object)val4, -1);
			TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();
			clearAllButton = textImageButtonFactory.Generate("Clear All".Localize());
			((GuiWidget)clearAllButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				ClearAll();
			});
			((GuiWidget)val4).AddChild((GuiWidget)(object)clearAllButton, -1);
			((GuiWidget)clearAllButton).set_Enabled(false);
			selectAllWellsButton = textImageButtonFactory.Generate("Select All Wells".Localize());
			((GuiWidget)selectAllWellsButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				SelectAllWells();
			});
			((GuiWidget)val4).AddChild((GuiWidget)(object)selectAllWellsButton, -1);
			GuiWidget val5 = new GuiWidget();
			val5.set_HAnchor((HAnchor)5);
			((GuiWidget)val4).AddChild(val5, -1);
			clearSelectedWellParts = textImageButtonFactory.Generate("Clear Parts In Selected".Localize());
			((GuiWidget)clearSelectedWellParts).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				ClearSelectedWellParts();
			});
			((GuiWidget)val4).AddChild((GuiWidget)(object)clearSelectedWellParts, -1);
			((GuiWidget)clearSelectedWellParts).set_Visible(false);
			clearSelectionButton = textImageButtonFactory.Generate("Clear Selection".Localize());
			((GuiWidget)clearSelectionButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				DeselectAllWells();
			});
			((GuiWidget)val4).AddChild((GuiWidget)(object)clearSelectionButton, -1);
			((GuiWidget)clearSelectionButton).set_Visible(false);
			FlowLayoutWidget val6 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val6).set_HAnchor((HAnchor)8);
			((GuiWidget)val6).set_VAnchor((VAnchor)5);
			FlowLayoutWidget val7 = val6;
			WellPlateTypeWidget obj = new WellPlateTypeWidget(this, wellPlate2D);
			((GuiWidget)obj).set_HAnchor((HAnchor)13);
			((GuiWidget)obj).set_VAnchor((VAnchor)8);
			((GuiWidget)obj).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((GuiWidget)obj).set_Margin(margin);
			((GuiWidget)obj).set_Padding(padding);
			wellPlateTypeWidget = obj;
			wellPlateTypeWidget.AddWellPlate(wellPlate2D);
			int horizontalWellCount = wellPlateTypeWidget.HorizontalWellCount;
			int verticalWellCount = wellPlateTypeWidget.VerticalWellCount;
			WellPlateTypeWidget obj2 = wellPlateTypeWidget;
			obj2.OnParametersUpdated = (EventHandler)Delegate.Combine(obj2.OnParametersUpdated, (EventHandler)delegate
			{
				if (horizontalWellCount != wellPlateTypeWidget.HorizontalWellCount || verticalWellCount != wellPlateTypeWidget.VerticalWellCount)
				{
					horizontalWellCount = wellPlateTypeWidget.HorizontalWellCount;
					verticalWellCount = wellPlateTypeWidget.VerticalWellCount;
					ClearAll();
				}
				meshManipulator.UpdateMeshPreview();
			});
			((GuiWidget)val7).AddChild((GuiWidget)(object)wellPlateTypeWidget, -1);
			WellPlatePartPicker wellPlatePartPicker = new WellPlatePartPicker(this, meshViewerWidget);
			((GuiWidget)wellPlatePartPicker).set_HAnchor((HAnchor)13);
			((GuiWidget)wellPlatePartPicker).set_VAnchor((VAnchor)5);
			((GuiWidget)wellPlatePartPicker).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((GuiWidget)wellPlatePartPicker).set_Margin(margin);
			((GuiWidget)wellPlatePartPicker).set_Padding(padding);
			WellPlatePartPicker wellPlatePartPicker2 = wellPlatePartPicker;
			((GuiWidget)val7).AddChild((GuiWidget)(object)wellPlatePartPicker2, -1);
			commitChangesButton = textImageButtonFactory.Generate("Save Layout".Localize());
			((GuiWidget)commitChangesButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				CommitWellPlateLayout();
			});
			((GuiWidget)commitChangesButton).set_HAnchor((HAnchor)2);
			((GuiWidget)val7).AddChild((GuiWidget)(object)commitChangesButton, -1);
			((GuiWidget)commitChangesButton).set_Visible(false);
			((GuiWidget)this).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val7, -1);
		}

		private void CommitWellPlateLayout()
		{
			QueueData.Instance.SelectedPrintItem = meshManipulator.CommitWellLayoutToNewPrintItemWrapper();
			ClearAll();
			using IEnumerator<PartPreviewContent> enumerator = ExtensionMethods.Parents<PartPreviewContent>((GuiWidget)(object)this).GetEnumerator();
			if (enumerator.MoveNext())
			{
				enumerator.Current.SwitchTo3dView();
			}
		}

		public void ClearAll()
		{
			ClearSelection();
			ClearAllWellParts();
		}

		public void SelectAllWells()
		{
			wellPlate2D.SelectAllWells();
			UpdateButtons();
		}

		public void DeselectAllWells()
		{
			selectedWells.Clear();
			wellPlate2D.DeselectAllWells();
			UpdateButtons();
		}

		public void ClearSelection()
		{
			DeselectAllWells();
			ClearPartSelection();
		}

		public void ClearPartSelection()
		{
			SetSelectedWellPlatePart(null);
		}

		public void SetWellSelectionStatus(int row, int col, bool selected)
		{
			Tuple<int, int> item = Tuple.Create(row, col);
			bool flag = selectedWells.Contains(item);
			if (selected && !flag)
			{
				selectedWells.Add(item);
			}
			else if (!selected && flag)
			{
				selectedWells.Remove(item);
			}
			if (shouldCheckForSelectioinPairs)
			{
				CheckForSelectionPairs();
			}
		}

		public void SetSelectedWellPlatePart(WellPlatePart wellPlatePart)
		{
			selectedWellPlatePart?.ClearSelection();
			selectedWellPlatePart = wellPlatePart;
			if (shouldCheckForSelectioinPairs)
			{
				CheckForSelectionPairs();
			}
		}

		private void CheckForSelectionPairs()
		{
			if (selectedWells.Count > 0 && selectedWellPlatePart != null)
			{
				foreach (Tuple<int, int> selectedWell in selectedWells)
				{
					meshManipulator.QueueMeshToWell(selectedWell, selectedWellPlatePart.MeshGroup);
					wellPlate2D.SetWellPart(selectedWell.Item1, selectedWell.Item2, selectedWellPlatePart);
				}
				ClearSelection();
				meshManipulator.UpdateMeshPreview();
			}
			UpdateButtons();
		}

		private void ClearSelectedWellParts()
		{
			foreach (Tuple<int, int> selectedWell in selectedWells)
			{
				meshManipulator.ClearManipulationForWell(selectedWell);
				wellPlate2D.SetWellPart(selectedWell.Item1, selectedWell.Item2, null);
			}
			meshManipulator.UpdateMeshPreview();
			UpdateButtons();
		}

		private void ClearAllWellParts()
		{
			for (int i = 0; i < wellPlateTypeWidget.VerticalWellCount; i++)
			{
				for (int j = 0; j < wellPlateTypeWidget.HorizontalWellCount; j++)
				{
					meshManipulator.ClearManipulationForWell(Tuple.Create(i, j));
					wellPlate2D.SetWellPart(i, j, null);
				}
			}
			meshManipulator.UpdateMeshPreview();
			UpdateButtons();
		}

		private void UpdateButtons()
		{
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < wellPlateTypeWidget.VerticalWellCount; i++)
			{
				if (flag && flag2)
				{
					break;
				}
				for (int j = 0; j < wellPlateTypeWidget.HorizontalWellCount; j++)
				{
					if (flag && flag2)
					{
						break;
					}
					if (wellPlate2D.WellPartSet(i, j))
					{
						flag = true;
						if (selectedWells.Contains(Tuple.Create(i, j)))
						{
							flag2 = true;
						}
					}
				}
			}
			if (meshManipulator.CanCommit)
			{
				((GuiWidget)commitChangesButton).set_Visible(true);
			}
			else
			{
				((GuiWidget)commitChangesButton).set_Visible(false);
			}
			if (selectedWells.Count > 0 || flag)
			{
				((GuiWidget)clearAllButton).set_Enabled(true);
			}
			else
			{
				((GuiWidget)clearAllButton).set_Enabled(false);
			}
			if (selectedWells.Count > 0)
			{
				((GuiWidget)clearSelectionButton).set_Visible(true);
				if (flag2)
				{
					((GuiWidget)clearSelectedWellParts).set_Visible(true);
				}
				else
				{
					((GuiWidget)clearSelectedWellParts).set_Visible(false);
				}
			}
			else
			{
				((GuiWidget)clearSelectedWellParts).set_Visible(false);
				((GuiWidget)clearSelectionButton).set_Visible(false);
			}
			if (selectedWells.Count == wellPlateTypeWidget.HorizontalWellCount * wellPlateTypeWidget.VerticalWellCount)
			{
				((GuiWidget)selectAllWellsButton).set_Visible(false);
			}
			else
			{
				((GuiWidget)selectAllWellsButton).set_Visible(true);
			}
		}
	}
}
