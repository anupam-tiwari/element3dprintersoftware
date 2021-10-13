using System;
using System.Collections.Generic;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.PolygonMesh;
using MatterHackers.PolygonMesh.Processors;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.WellPlate
{
	public class WellPlateMeshManipulator
	{
		private Dictionary<Tuple<int, int>, MeshGroup> meshManipulations = new Dictionary<Tuple<int, int>, MeshGroup>();

		private MeshViewerWidget meshViewerWidget;

		private List<MeshGroup> originalMeshGroups = new List<MeshGroup>();

		private List<Matrix4X4> originalMeshGroupTransforms = new List<Matrix4X4>();

		private WellPlateTypeWidget wellPlateTypeWidget;

		private Vector2 xStep;

		private Vector2 yStep;

		public bool CanCommit => meshManipulations.Count > 0;

		public WellPlateMeshManipulator(MeshViewerWidget meshViewer, WellPlateTypeWidget wellPlateType)
		{
			meshViewerWidget = meshViewer;
			wellPlateTypeWidget = wellPlateType;
			meshViewerWidget.LoadDone += delegate
			{
				SetOriginalMesh();
			};
			WellPlateTypeWidget obj = wellPlateTypeWidget;
			obj.OnParametersUpdated = (EventHandler)Delegate.Combine(obj.OnParametersUpdated, (EventHandler)delegate
			{
				SetStep();
			});
			SetStep();
		}

		private void SetOriginalMesh()
		{
			originalMeshGroups.Clear();
			originalMeshGroupTransforms.Clear();
			meshViewerWidget.MeshGroups.ForEach(new Action<MeshGroup>(originalMeshGroups.Add));
			meshViewerWidget.MeshGroupTransforms.ForEach(new Action<Matrix4X4>(originalMeshGroupTransforms.Add));
		}

		private void SetStep()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = wellPlateTypeWidget.WellPlateTopRightOffset - wellPlateTypeWidget.WellPlateTopLeftOffset;
			xStep = val / ((Vector2)(ref val)).get_Length();
			yStep = ((Vector2)(ref xStep)).GetPerpendicularRight();
			xStep *= wellPlateTypeWidget.HorizontalWellSpacing;
			yStep *= wellPlateTypeWidget.VerticalWellSpacing;
		}

		public void UpdateMeshPreview()
		{
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			meshViewerWidget.MeshGroups.Clear();
			meshViewerWidget.MeshGroupTransforms.Clear();
			if (meshManipulations.Count > 0)
			{
				foreach (KeyValuePair<Tuple<int, int>, MeshGroup> meshManipulation in meshManipulations)
				{
					meshViewerWidget.MeshGroups.Add(MeshGroupForManipulation(meshManipulation));
					meshViewerWidget.MeshGroupTransforms.Add(Matrix4X4.Identity);
				}
			}
			else
			{
				originalMeshGroups.ForEach(new Action<MeshGroup>(meshViewerWidget.MeshGroups.Add));
				originalMeshGroupTransforms.ForEach(new Action<Matrix4X4>(meshViewerWidget.MeshGroupTransforms.Add));
			}
		}

		public void QueueMeshToWell(Tuple<int, int> rowCol, MeshGroup mesh)
		{
			if (meshManipulations.ContainsKey(rowCol))
			{
				meshManipulations[rowCol] = mesh;
			}
			else
			{
				meshManipulations.Add(rowCol, mesh);
			}
		}

		public void ClearManipulationForWell(Tuple<int, int> rowCol)
		{
			if (meshManipulations.ContainsKey(rowCol))
			{
				meshManipulations.Remove(rowCol);
			}
		}

		public PrintItemWrapper CommitWellLayoutToNewPrintItemWrapper()
		{
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Expected O, but got Unknown
			List<MeshGroup> list = new List<MeshGroup>();
			foreach (KeyValuePair<Tuple<int, int>, MeshGroup> meshManipulation in meshManipulations)
			{
				list.Add(MeshGroupForManipulation(meshManipulation));
			}
			string[] array = new string[4]
			{
				"Created By",
				"Element",
				"BedPosition",
				"Absolute"
			};
			MeshOutputSettings val = new MeshOutputSettings((OutputType)1, array, (ReportProgressRatio)null);
			string path = Path.ChangeExtension(Path.GetRandomFileName(), ".amf");
			string text = Path.Combine(ApplicationDataStorage.Instance.ApplicationLibraryDataPath, path);
			MeshFileIo.Save(list, text, val, (ReportProgressRatio)null);
			PrintItemWrapper printItemWrapper = new PrintItemWrapper(new PrintItem
			{
				Name = QueueData.Instance.SelectedPrintItem.Name + " - well plate".Localize(),
				FileLocation = text,
				WellPlatePrint = true
			});
			QueueData.Instance.AddItem(printItemWrapper);
			return printItemWrapper;
		}

		private MeshGroup MeshGroupForManipulation(KeyValuePair<Tuple<int, int>, MeshGroup> meshManipulation)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			MeshGroup val = new MeshGroup();
			foreach (Mesh mesh in meshManipulation.Value.get_Meshes())
			{
				val.get_Meshes().Add(Mesh.Copy(mesh, (ReportProgressRatio)null, true));
			}
			AxisAlignedBoundingBox axisAlignedBoundingBox = meshManipulation.Value.GetAxisAlignedBoundingBox();
			double num = ((wellPlateTypeWidget.WellShape != 0) ? Math.Max(axisAlignedBoundingBox.get_Size().x, axisAlignedBoundingBox.get_Size().y) : Math.Sqrt(axisAlignedBoundingBox.get_Size().x * axisAlignedBoundingBox.get_Size().x + axisAlignedBoundingBox.get_Size().y * axisAlignedBoundingBox.get_Size().y));
			val.Transform(Matrix4X4.CreateScale(0.9 * wellPlateTypeWidget.WellWidth / num));
			axisAlignedBoundingBox = val.GetAxisAlignedBoundingBox();
			Vector2 val2 = (double)meshManipulation.Key.Item2 * xStep + (double)meshManipulation.Key.Item1 * yStep;
			Vector2 val3 = new Vector2(wellPlateTypeWidget.WellPlateTopLeftOffset.x, wellPlateTypeWidget.WellPlateTopLeftOffset.y) + val2;
			val.Transform(Matrix4X4.CreateTranslation(val3.x, val3.y, wellPlateTypeWidget.ZToWellBottom - axisAlignedBoundingBox.GetCenter().z + axisAlignedBoundingBox.get_ZSize() / 2.0));
			return val;
		}
	}
}
