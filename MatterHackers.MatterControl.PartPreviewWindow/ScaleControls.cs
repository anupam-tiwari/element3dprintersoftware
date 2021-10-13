using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.PolygonMesh;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class ScaleControls : FlowLayoutWidget
	{
		private Button applyScaleButton;

		private CheckBox expandScaleOptions;

		private FlowLayoutWidget scaleOptionContainer;

		private MHNumberEdit scaleRatioControl;

		private EditableNumberDisplay[] sizeDisplay = new EditableNumberDisplay[3];

		private CheckBox uniformScale;

		private View3DWidget view3DWidget;

		public ScaleControls(View3DWidget view3DWidget)
			: this((FlowDirection)3)
		{
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Expected O, but got Unknown
			this.view3DWidget = view3DWidget;
			expandScaleOptions = view3DWidget.ExpandMenuOptionFactory.GenerateCheckBoxButton("Scale".Localize().ToUpper(), View3DWidget.ArrowRight, View3DWidget.ArrowDown);
			((GuiWidget)expandScaleOptions).set_Margin(new BorderDouble(0.0, 2.0, 0.0, 0.0));
			((GuiWidget)this).AddChild((GuiWidget)(object)expandScaleOptions, -1);
			scaleOptionContainer = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)scaleOptionContainer).set_HAnchor((HAnchor)5);
			((GuiWidget)scaleOptionContainer).set_Visible(false);
			((GuiWidget)this).AddChild((GuiWidget)(object)scaleOptionContainer, -1);
			AddScaleControls(scaleOptionContainer);
			expandScaleOptions.add_CheckedStateChanged((EventHandler)expandScaleOptions_CheckedStateChanged);
			view3DWidget.SelectedTransformChanged += OnSelectedTransformChanged;
		}

		private void AddScaleControls(FlowLayoutWidget buttonPanel)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Expected O, but got Unknown
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Expected O, but got Unknown
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Expected O, but got Unknown
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Expected O, but got Unknown
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			List<GuiWidget> list = new List<GuiWidget>();
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Padding(new BorderDouble(5.0));
			string text = "Ratio".Localize();
			TextWidget val2 = new TextWidget(StringHelper.FormatWith("{0}:", new object[1]
			{
				text
			}), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 0.0, 3.0, 0.0));
			((GuiWidget)val2).set_VAnchor((VAnchor)2);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			scaleRatioControl = new MHNumberEdit(1.0, 0.0, 0.0, 12.0, 50.0 * GuiWidget.get_DeviceScale(), 0.0, allowNegatives: false, allowDecimals: true, -2147483648.0, 2147483647.0, 0.05);
			scaleRatioControl.SelectAllOnFocus = true;
			((GuiWidget)scaleRatioControl).set_VAnchor((VAnchor)2);
			((GuiWidget)val).AddChild((GuiWidget)(object)scaleRatioControl, -1);
			((GuiWidget)scaleRatioControl.ActuallNumberEdit).add_KeyPressed((EventHandler<KeyPressEventArgs>)delegate
			{
				OnSelectedTransformChanged(this, null);
			});
			((GuiWidget)scaleRatioControl.ActuallNumberEdit).add_KeyDown((EventHandler<KeyEventArgs>)delegate
			{
				OnSelectedTransformChanged(this, null);
			});
			((TextEditWidget)scaleRatioControl.ActuallNumberEdit).add_EnterPressed((KeyEventHandler)delegate
			{
				ApplyScaleFromEditField();
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)CreateScaleDropDownMenu(), -1);
			((GuiWidget)buttonPanel).AddChild((GuiWidget)(object)val, -1);
			list.Add((GuiWidget)(object)scaleRatioControl);
			applyScaleButton = view3DWidget.WhiteButtonFactory.Generate("Apply Scale".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)applyScaleButton).set_Cursor((Cursors)3);
			((GuiWidget)buttonPanel).AddChild((GuiWidget)(object)applyScaleButton, -1);
			list.Add((GuiWidget)(object)applyScaleButton);
			((GuiWidget)applyScaleButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				ApplyScaleFromEditField();
			});
			((GuiWidget)buttonPanel).AddChild(CreateAxisScalingControl("x".ToUpper(), 0), -1);
			((GuiWidget)buttonPanel).AddChild(CreateAxisScalingControl("y".ToUpper(), 1), -1);
			((GuiWidget)buttonPanel).AddChild(CreateAxisScalingControl("z".ToUpper(), 2), -1);
			uniformScale = new CheckBox("Lock Ratio".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12.0);
			uniformScale.set_Checked(true);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_Padding(new BorderDouble(5.0, 3.0));
			((GuiWidget)val3).AddChild((GuiWidget)(object)uniformScale, -1);
			((GuiWidget)buttonPanel).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)buttonPanel).AddChild(view3DWidget.GenerateHorizontalRule(), -1);
		}

		private void ApplyScaleFromEditField()
		{
			if (view3DWidget.HaveSelection)
			{
				List<Matrix4X4> selectedMeshGroupTransforms = view3DWidget.SelectedMeshGroupTransforms;
				double value = scaleRatioControl.ActuallNumberEdit.get_Value();
				if (value > 0.0)
				{
					ScaleAxis(value, 0);
					ScaleAxis(value, 1);
					ScaleAxis(value, 2);
				}
				view3DWidget.AddUndoForSelectedMeshGroupTransforms(selectedMeshGroupTransforms);
			}
		}

		private GuiWidget CreateAxisScalingControl(string axis, int axisIndex)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_Padding(new BorderDouble(5.0, 3.0));
			TextWidget val2 = new TextWidget(StringHelper.FormatWith("{0}:", new object[1]
			{
				axis
			}), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val2).set_VAnchor((VAnchor)2);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			sizeDisplay[axisIndex] = new EditableNumberDisplay(view3DWidget.textImageButtonFactory, "100", "1000.00");
			sizeDisplay[axisIndex].EditComplete += delegate
			{
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				if (view3DWidget.HaveSelection)
				{
					List<Matrix4X4> selectedMeshGroupTransforms = view3DWidget.SelectedMeshGroupTransforms;
					SetNewModelSize(sizeDisplay[axisIndex].GetValue(), axisIndex);
					AxisAlignedBoundingBox boundsForSelection = view3DWidget.MeshViewer.GetBoundsForSelection();
					for (int i = 0; i <= 2; i++)
					{
						EditableNumberDisplay obj = sizeDisplay[i];
						object[] array = new object[1];
						Vector3 size = boundsForSelection.get_Size();
						array[0] = ((Vector3)(ref size)).get_Item(i);
						obj.SetDisplayString(StringHelper.FormatWith("{0:0.00}", array));
					}
					OnSelectedTransformChanged(null, null);
					view3DWidget.AddUndoForSelectedMeshGroupTransforms(selectedMeshGroupTransforms);
				}
				else
				{
					sizeDisplay[axisIndex].SetDisplayString("---");
				}
			};
			((GuiWidget)val).AddChild((GuiWidget)(object)sizeDisplay[axisIndex], -1);
			return (GuiWidget)(object)val;
		}

		private DropDownMenu CreateScaleDropDownMenu()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			DropDownMenu presetScaleMenu = new DropDownMenu("", (Direction)1);
			presetScaleMenu.NormalArrowColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			presetScaleMenu.HoverArrowColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			presetScaleMenu.MenuAsWideAsItems = false;
			((Menu)presetScaleMenu).set_AlignToRightEdge(true);
			((GuiWidget)presetScaleMenu).set_HAnchor((HAnchor)0);
			((GuiWidget)presetScaleMenu).set_VAnchor((VAnchor)0);
			((GuiWidget)presetScaleMenu).set_Width(25.0);
			((GuiWidget)presetScaleMenu).set_Height(((GuiWidget)scaleRatioControl).get_Height() + 2.0);
			presetScaleMenu.AddItem("mm to in (.0393)");
			presetScaleMenu.AddItem("in to mm (25.4)");
			presetScaleMenu.AddItem("mm to cm (.1)");
			presetScaleMenu.AddItem("cm to mm (10)");
			string text = "none".Localize();
			string name = StringHelper.FormatWith("{0} (1)", new object[1]
			{
				text
			});
			presetScaleMenu.AddItem(name);
			presetScaleMenu.SelectionChanged += delegate
			{
				double value = 1.0;
				switch (presetScaleMenu.SelectedIndex)
				{
				case 0:
					value = 0.03937007874015748;
					break;
				case 1:
					value = 25.4;
					break;
				case 2:
					value = 0.1;
					break;
				case 3:
					value = 10.0;
					break;
				case 4:
					value = 1.0;
					break;
				}
				scaleRatioControl.ActuallNumberEdit.set_Value(value);
			};
			return presetScaleMenu;
		}

		private void expandScaleOptions_CheckedStateChanged(object sender, EventArgs e)
		{
			if (((GuiWidget)scaleOptionContainer).get_Visible() != expandScaleOptions.get_Checked())
			{
				((GuiWidget)scaleOptionContainer).set_Visible(expandScaleOptions.get_Checked());
			}
		}

		private void ScaleAxis(double scaleIn, int axis)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			Vector3 one = Vector3.One;
			((Vector3)(ref one)).set_Item(axis, scaleIn);
			Matrix4X4 transformToApply = Matrix4X4.CreateScale(one);
			foreach (int selectedMeshGroupIndex in view3DWidget.SelectedMeshGroupIndices)
			{
				view3DWidget.MeshGroupTransforms[selectedMeshGroupIndex] = PlatingHelper.ApplyAtCenter((IHasAABB)(object)view3DWidget.MeshGroups[selectedMeshGroupIndex], view3DWidget.MeshGroupTransforms[selectedMeshGroupIndex], transformToApply);
				AxisAlignedBoundingBox axisAlignedBoundingBox = view3DWidget.MeshGroups[selectedMeshGroupIndex].GetAxisAlignedBoundingBox(view3DWidget.MeshGroupTransforms[selectedMeshGroupIndex]);
				PlatingHelper.PlaceMeshAtHeight(view3DWidget.MeshGroups, view3DWidget.MeshGroupTransforms, selectedMeshGroupIndex, axisAlignedBoundingBox.minXYZ.z);
				PlatingHelper.PlaceMeshGroupOnBed(view3DWidget.MeshGroups, view3DWidget.MeshGroupTransforms, selectedMeshGroupIndex);
			}
			view3DWidget.PartHasBeenChanged();
			((GuiWidget)this).Invalidate();
			OnSelectedTransformChanged(this, null);
		}

		private void SetNewModelSize(double sizeInMm, int axis)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (view3DWidget.HaveSelection)
			{
				Vector3 size = view3DWidget.MeshViewer.GetBoundsForSelection().get_Size();
				double num = ((Vector3)(ref size)).get_Item(axis);
				double value = sizeDisplay[axis].GetValue();
				double num2 = 1.0;
				if (num != 0.0)
				{
					num2 = value / num;
				}
				if (uniformScale.get_Checked())
				{
					scaleRatioControl.ActuallNumberEdit.set_Value(num2);
					ApplyScaleFromEditField();
				}
				else
				{
					ScaleAxis(num2, axis);
				}
			}
		}

		private void OnSelectedTransformChanged(object sender, EventArgs e)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			if (sizeDisplay[0] != null && view3DWidget.HaveSelection)
			{
				AxisAlignedBoundingBox boundsForSelection = view3DWidget.MeshViewer.GetBoundsForSelection();
				EditableNumberDisplay obj = sizeDisplay[0];
				object[] array = new object[1];
				Vector3 size = boundsForSelection.get_Size();
				array[0] = ((Vector3)(ref size)).get_Item(0);
				obj.SetDisplayString(StringHelper.FormatWith("{0:0.00}", array));
				EditableNumberDisplay obj2 = sizeDisplay[1];
				object[] array2 = new object[1];
				size = boundsForSelection.get_Size();
				array2[0] = ((Vector3)(ref size)).get_Item(1);
				obj2.SetDisplayString(StringHelper.FormatWith("{0:0.00}", array2));
				EditableNumberDisplay obj3 = sizeDisplay[2];
				object[] array3 = new object[1];
				size = boundsForSelection.get_Size();
				array3[0] = ((Vector3)(ref size)).get_Item(2);
				obj3.SetDisplayString(StringHelper.FormatWith("{0:0.00}", array3));
			}
			else
			{
				sizeDisplay[0].SetDisplayString("---");
				sizeDisplay[1].SetDisplayString("---");
				sizeDisplay[2].SetDisplayString("---");
			}
		}
	}
}
