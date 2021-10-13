using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.PolygonMesh;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class MirrorControls : FlowLayoutWidget
	{
		private CheckBox expandMirrorOptions;

		private FlowLayoutWidget mirrorOptionContainer;

		private View3DWidget view3DWidget;

		public MirrorControls(View3DWidget view3DWidget)
			: this((FlowDirection)3)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Expected O, but got Unknown
			this.view3DWidget = view3DWidget;
			expandMirrorOptions = view3DWidget.ExpandMenuOptionFactory.GenerateCheckBoxButton("Mirror".Localize().ToUpper(), View3DWidget.ArrowRight, View3DWidget.ArrowDown);
			((GuiWidget)expandMirrorOptions).set_Margin(new BorderDouble(0.0, 2.0, 0.0, 0.0));
			((GuiWidget)this).AddChild((GuiWidget)(object)expandMirrorOptions, -1);
			mirrorOptionContainer = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)mirrorOptionContainer).set_HAnchor((HAnchor)5);
			((GuiWidget)mirrorOptionContainer).set_Visible(false);
			((GuiWidget)this).AddChild((GuiWidget)(object)mirrorOptionContainer, -1);
			AddMirrorControls(mirrorOptionContainer);
			expandMirrorOptions.add_CheckedStateChanged((EventHandler)expandMirrorOptions_CheckedStateChanged);
		}

		private void AddMirrorControls(FlowLayoutWidget buttonPanel)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			List<GuiWidget> list = new List<GuiWidget>();
			double fixedWidth = view3DWidget.textImageButtonFactory.FixedWidth;
			view3DWidget.textImageButtonFactory.FixedWidth = view3DWidget.EditButtonHeight;
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			Button val2 = view3DWidget.textImageButtonFactory.Generate("X", (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			list.Add((GuiWidget)(object)val2);
			((GuiWidget)val2).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Expected O, but got Unknown
				if (view3DWidget.HaveSelection)
				{
					view3DWidget.UndoBuffer.AddAndDo((IUndoRedoCommand)new UndoRedoActions((Action)delegate
					{
						MirrorOnAxis(0);
					}, (Action)delegate
					{
						MirrorOnAxis(0);
					}));
				}
			});
			Button val3 = view3DWidget.textImageButtonFactory.Generate("Y", (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			list.Add((GuiWidget)(object)val3);
			((GuiWidget)val3).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Expected O, but got Unknown
				if (view3DWidget.HaveSelection)
				{
					view3DWidget.UndoBuffer.AddAndDo((IUndoRedoCommand)new UndoRedoActions((Action)delegate
					{
						MirrorOnAxis(1);
					}, (Action)delegate
					{
						MirrorOnAxis(1);
					}));
				}
			});
			Button val4 = view3DWidget.textImageButtonFactory.Generate("Z", (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)val).AddChild((GuiWidget)(object)val4, -1);
			list.Add((GuiWidget)(object)val4);
			((GuiWidget)val4).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Expected O, but got Unknown
				if (view3DWidget.HaveSelection)
				{
					view3DWidget.UndoBuffer.AddAndDo((IUndoRedoCommand)new UndoRedoActions((Action)delegate
					{
						MirrorOnAxis(2);
					}, (Action)delegate
					{
						MirrorOnAxis(2);
					}));
				}
			});
			((GuiWidget)buttonPanel).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)buttonPanel).AddChild(view3DWidget.GenerateHorizontalRule(), -1);
			view3DWidget.textImageButtonFactory.FixedWidth = fixedWidth;
		}

		private void MirrorOnAxis(int axisIndex)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			Vector3 one = Vector3.One;
			((Vector3)(ref one)).set_Item(axisIndex, -1.0);
			foreach (int selectedMeshGroupIndex in view3DWidget.SelectedMeshGroupIndices)
			{
				view3DWidget.MeshGroups[selectedMeshGroupIndex].ReverseFaceEdges();
				view3DWidget.MeshGroupTransforms[selectedMeshGroupIndex] = PlatingHelper.ApplyAtCenter((IHasAABB)(object)view3DWidget.MeshGroups[selectedMeshGroupIndex], view3DWidget.MeshGroupTransforms[selectedMeshGroupIndex], Matrix4X4.CreateScale(one));
			}
			view3DWidget.PartHasBeenChanged();
			((GuiWidget)this).Invalidate();
		}

		private void expandMirrorOptions_CheckedStateChanged(object sender, EventArgs e)
		{
			if (((GuiWidget)mirrorOptionContainer).get_Visible() != expandMirrorOptions.get_Checked())
			{
				((GuiWidget)mirrorOptionContainer).set_Visible(expandMirrorOptions.get_Checked());
			}
		}
	}
}
