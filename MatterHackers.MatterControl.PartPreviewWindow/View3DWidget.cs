using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.PrintLibrary.Provider;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.PolygonMesh;
using MatterHackers.PolygonMesh.Processors;
using MatterHackers.RayTracer;
using MatterHackers.RayTracer.Traceable;
using MatterHackers.RenderOpenGl;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class View3DWidget : PartPreview3DWidget
	{
		public enum AutoRotate
		{
			Enabled,
			Disabled
		}

		public enum OpenMode
		{
			Viewing,
			Editing
		}

		public enum WindowMode
		{
			Embeded,
			StandAlone
		}

		private enum TraceInfoOpperation
		{
			DONT_COPY,
			DO_COPY
		}

		private string PartsNotPrintableMessage = "Parts are not on the bed or outside the print area.\n\nWould you like to center them on the bed?".Localize();

		private string PartsNotPrintableTitle = "Parts not in print area".Localize();

		public readonly int EditButtonHeight = 44;

		private Action afterSaveCallback;

		private List<MeshGroup> asyncMeshGroups = new List<MeshGroup>();

		private List<Matrix4X4> asyncMeshGroupTransforms = new List<Matrix4X4>();

		private List<PlatingMeshGroupData> asyncPlatingDatas = new List<PlatingMeshGroupData>();

		private FlowLayoutWidget doEdittingButtonsContainer;

		private bool editorThatRequestedSave;

		private FlowLayoutWidget enterEditButtonsContainer;

		private CheckBox expandMaterialOptions;

		private CheckBox expandRotateOptions;

		private CheckBox expandViewOptions;

		private ExportPrintItemWindow exportingWindow;

		private ObservableCollection<GuiWidget> extruderButtons = new ObservableCollection<GuiWidget>();

		private bool hasDrawn;

		private FlowLayoutWidget materialOptionContainer;

		private OpenMode openMode;

		private bool partHasBeenEdited;

		private List<string> pendingPartsToLoad = new List<string>();

		private PrintItemWrapper printItemWrapper;

		private ProgressControl processingProgressControl;

		private FlowLayoutWidget rotateOptionContainer;

		private SaveAsWindow saveAsWindow;

		private SplitButton saveButtons;

		private bool saveSucceded = true;

		private EventHandler SelectionChanged;

		private RGBA_Bytes[] SelectionColors = (RGBA_Bytes[])(object)new RGBA_Bytes[5]
		{
			new RGBA_Bytes(131, 4, 66),
			new RGBA_Bytes(227, 31, 61),
			new RGBA_Bytes(255, 148, 1),
			new RGBA_Bytes(247, 224, 23),
			new RGBA_Bytes(143, 212, 1)
		};

		private Stopwatch timeSinceLastSpin = new Stopwatch();

		private Stopwatch timeSinceReported = new Stopwatch();

		private List<Matrix4X4> transformsOnMouseDown = new List<Matrix4X4>();

		private bool selectMultiple;

		private EventHandler unregisterEvents;

		private bool viewIsInEditModePreLock;

		private FlowLayoutWidget viewOptionContainer;

		private bool wasInSelectMode;

		private ViewControls3DButtons? activeButtonBeforeMouseOverride;

		private ViewControls3DButtons? activeButtonBeforeKeyOverride;

		private IPrimitive allObjects;

		public static Regex fileNameNumberMatch = new Regex("\\(\\d+\\)", (RegexOptions)8);

		public UndoBuffer UndoBuffer
		{
			get;
			private set;
		} = new UndoBuffer();


		public List<PlatingMeshGroupData> MeshGroupExtraData
		{
			get;
			private set;
		}

		public MeshSelectInfo CurrentSelectInfo
		{
			get;
			private set;
		} = new MeshSelectInfo();


		public PrintItemWrapper ActivePrintItem => printItemWrapper;

		public static ImageBuffer ArrowRight
		{
			get
			{
				if (ActiveTheme.get_Instance().get_IsDarkTheme())
				{
					return ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon("icon_arrow_right_no_border_32x32.png", 32, 32));
				}
				return StaticData.get_Instance().LoadIcon("icon_arrow_right_no_border_32x32.png", 32, 32);
			}
		}

		public static ImageBuffer ArrowDown
		{
			get
			{
				if (ActiveTheme.get_Instance().get_IsDarkTheme())
				{
					return ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon("icon_arrow_down_no_border_32x32.png", 32, 32));
				}
				return StaticData.get_Instance().LoadIcon("icon_arrow_down_no_border_32x32.png", 32, 32);
			}
		}

		public bool ShouldBeSaved
		{
			get
			{
				if (InEditMode)
				{
					return ((GuiWidget)saveButtons).get_Visible();
				}
				return false;
			}
		}

		public bool DragingPart => CurrentSelectInfo.DownOnPart;

		public bool DisplayAllValueData
		{
			get;
			set;
		}

		public override int SelectedMeshGroupIndex
		{
			get
			{
				return meshViewerWidget.SelectedMeshGroupIndex;
			}
			set
			{
				if (base.SelectedMeshGroupIndices.Count > 1)
				{
					base.SelectedMeshGroupIndices.Clear();
				}
				if (value != SelectedMeshGroupIndex)
				{
					meshViewerWidget.SelectedMeshGroupIndex = value;
					OnSelectionChanged(null);
					((GuiWidget)this).Invalidate();
				}
			}
		}

		public Matrix4X4 SelectedMeshGroupTransform
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return base.SelectedMeshTransform;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				base.SelectedMeshTransform = value;
			}
		}

		public MeshViewerWidget MeshViewer => meshViewerWidget;

		public WindowMode windowType
		{
			get;
			set;
		}

		private bool DoAddFileAfterCreatingEditData
		{
			get;
			set;
		}

		public Vector3 LastHitPosition
		{
			get;
			private set;
		}

		public override bool InEditMode => ((GuiWidget)buttonRightPanel).get_Visible();

		public event EventHandler SelectedTransformChanged;

		private void CreateSelectionData()
		{
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Expected O, but got Unknown
			processingProgressControl.set_ProcessType("Preparing Meshes".Localize() + ":");
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			PushMeshGroupDataToAsynchLists(TraceInfoOpperation.DONT_COPY);
			asyncPlatingDatas.Clear();
			double num = 1.0 / (double)asyncMeshGroups.Count;
			double num2 = 0.0;
			for (int i = 0; i < asyncMeshGroups.Count; i++)
			{
				asyncPlatingDatas.Add(new PlatingMeshGroupData());
				PlatingHelper.CreateITraceableForMeshGroup(asyncPlatingDatas, asyncMeshGroups, i, (ReportProgressRatio)delegate(double progress0To1, string processingState, out bool continueProcessing)
				{
					ReportProgressChanged(progress0To1, processingState, out continueProcessing);
				});
				num2 += num;
			}
			ReportProgressChanged(1.0, "Creating GL Data", out var _);
			meshViewerWidget.CreateGlDataForMeshes(asyncMeshGroups);
		}

		private async void EnterEditAndCreateSelectionData()
		{
			if (((GuiWidget)enterEditButtonsContainer).get_Visible())
			{
				((GuiWidget)enterEditButtonsContainer).set_Visible(false);
			}
			viewControls3D.ActiveButton = ViewControls3DButtons.PartSelect;
			if (base.MeshGroups.Count <= 0)
			{
				return;
			}
			((GuiWidget)processingProgressControl).set_Visible(true);
			LockEditControls();
			viewIsInEditModePreLock = true;
			await Task.Run(new Action(CreateSelectionData));
			if (((GuiWidget)this).get_HasBeenClosed())
			{
				return;
			}
			PullMeshGroupDataFromAsynchLists();
			SelectedMeshGroupIndex = 0;
			((GuiWidget)buttonRightPanel).set_Visible(true);
			UnlockEditControls();
			viewControls3D.ActiveButton = ViewControls3DButtons.PartSelect;
			((GuiWidget)this).Invalidate();
			if (DoAddFileAfterCreatingEditData)
			{
				FileDialog.OpenFileDialog(new OpenFileDialogParams(ApplicationSettings.OpenDesignFileParams, "", true, "", ""), (Action<OpenFileDialogParams>)delegate(OpenFileDialogParams openParams)
				{
					LoadAndAddPartsToPlate(((FileDialogParams)openParams).get_FileNames());
				});
				DoAddFileAfterCreatingEditData = false;
			}
			else if (pendingPartsToLoad.Count > 0)
			{
				LoadAndAddPartsToPlate(pendingPartsToLoad.ToArray());
				pendingPartsToLoad.Clear();
			}
			else
			{
				if (PartsAreInPrintVolume())
				{
					return;
				}
				UiThread.RunOnIdle((Action)delegate
				{
					StyledMessageBox.ShowMessageBox(delegate(bool doCentering)
					{
						if (doCentering)
						{
							AutoArrangePartsInBackground();
						}
					}, PartsNotPrintableMessage, PartsNotPrintableTitle, StyledMessageBox.MessageType.YES_NO, "Center on Bed".Localize(), "Cancel".Localize());
				});
			}
		}

		private void AlignSelected()
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Expected O, but got Unknown
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			PushMeshGroupDataToAsynchLists(TraceInfoOpperation.DO_COPY);
			Vector3 center = asyncMeshGroups[SelectedMeshGroupIndex].GetAxisAlignedBoundingBox().get_Center();
			Vector3 center2 = asyncMeshGroups[SelectedMeshGroupIndex].GetAxisAlignedBoundingBox(asyncMeshGroupTransforms[SelectedMeshGroupIndex]).get_Center();
			foreach (int selectedMeshGroupIndex in base.SelectedMeshGroupIndices)
			{
				MeshGroup val = asyncMeshGroups[selectedMeshGroupIndex];
				if (val != asyncMeshGroups[SelectedMeshGroupIndex])
				{
					Vector3 center3 = val.GetAxisAlignedBoundingBox().get_Center();
					Vector3 center4 = val.GetAxisAlignedBoundingBox(asyncMeshGroupTransforms[selectedMeshGroupIndex]).get_Center();
					Vector3 val2 = center3 - center;
					Vector3 val3 = center4 - center2;
					Vector3 val4 = val2 - val3;
					if (((Vector3)(ref val4)).get_Length() > 0.0001)
					{
						List<Matrix4X4> list = asyncMeshGroupTransforms;
						int index = selectedMeshGroupIndex;
						list[index] *= Matrix4X4.CreateTranslation(val4);
						PartHasBeenChanged();
					}
				}
			}
			asyncPlatingDatas.Clear();
			double num = 1.0 / (double)asyncMeshGroups.Count;
			double num2 = 0.0;
			for (int i = 0; i < asyncMeshGroups.Count; i++)
			{
				PlatingMeshGroupData item = new PlatingMeshGroupData();
				asyncPlatingDatas.Add(item);
				_ = asyncMeshGroups[i];
				PlatingHelper.CreateITraceableForMeshGroup(asyncPlatingDatas, asyncMeshGroups, i, (ReportProgressRatio)delegate(double progress0To1, string processingState, out bool continueProcessing)
				{
					ReportProgressChanged(progress0To1, processingState, out continueProcessing);
				});
				num2 += num;
			}
		}

		private async void AlignToSelectedMeshGroup()
		{
			if (base.MeshGroups.Count <= 0)
			{
				return;
			}
			processingProgressControl.set_PercentComplete(0);
			((GuiWidget)processingProgressControl).set_Visible(true);
			string arg = "Aligning".Localize();
			string processType = $"{arg}:";
			processingProgressControl.set_ProcessType(processType);
			LockEditControls();
			viewIsInEditModePreLock = true;
			if (base.SelectedMeshGroupIndices.Count < 2)
			{
				base.SelectedMeshGroupIndices.Clear();
				for (int i = 0; i < base.MeshGroups.Count; i++)
				{
					base.SelectedMeshGroupIndices.Add(i);
				}
			}
			List<Matrix4X4> undoTransforms = new List<Matrix4X4>(base.SelectedMeshGroupTransforms);
			await Task.Run(new Action(AlignSelected));
			if (!((GuiWidget)this).get_HasBeenClosed())
			{
				PullMeshGroupDataFromAsynchLists();
				UndoBuffer.Add((IUndoRedoCommand)(object)new TransformUndoCommand(this, base.SelectedMeshGroupIndices, undoTransforms, base.SelectedMeshGroupTransforms));
				UnlockEditControls();
				((GuiWidget)this).Invalidate();
			}
		}

		private async void AutoArrangePartsInBackground()
		{
			if (base.MeshGroups.Count <= 0)
			{
				return;
			}
			string arg = "Arranging Parts".Localize();
			string processType = $"{arg}:";
			processingProgressControl.set_ProcessType(processType);
			((GuiWidget)processingProgressControl).set_Visible(true);
			processingProgressControl.set_PercentComplete(0);
			LockEditControls();
			List<Matrix4X4> preArrangeTarnsforms = new List<Matrix4X4>(base.MeshGroupTransforms);
			await Task.Run(delegate
			{
				Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
				PushMeshGroupDataToAsynchLists(TraceInfoOpperation.DONT_COPY);
				PlatingHelper.ArrangeMeshGroups(asyncMeshGroups, asyncMeshGroupTransforms, asyncPlatingDatas, ReportProgressChanged);
			});
			if (!((GuiWidget)this).get_HasBeenClosed())
			{
				for (int i = 0; i < asyncMeshGroups.Count; i++)
				{
					List<Matrix4X4> list = asyncMeshGroupTransforms;
					int index = i;
					list[index] *= Matrix4X4.CreateTranslation(new Vector3(ActiveSliceSettings.Instance.GetValue<Vector2>("print_center"), 0.0));
				}
				PartHasBeenChanged();
				PullMeshGroupDataFromAsynchLists();
				List<Matrix4X4> postArrangeTarnsforms = new List<Matrix4X4>(base.MeshGroupTransforms);
				UndoBuffer.Add((IUndoRedoCommand)(object)new ArangeUndoCommand(this, preArrangeTarnsforms, postArrangeTarnsforms));
				UnlockEditControls();
			}
		}

		private void CopyGroup()
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected O, but got Unknown
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Expected O, but got Unknown
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			PushMeshGroupDataToAsynchLists(TraceInfoOpperation.DO_COPY);
			foreach (int selectedMeshGroupIndex in base.SelectedMeshGroupIndices)
			{
				MeshGroup obj = asyncMeshGroups[selectedMeshGroupIndex];
				MeshGroup val = new MeshGroup();
				double num = obj.get_Meshes().Count;
				for (int i = 0; (double)i < num; i++)
				{
					Mesh val2 = asyncMeshGroups[selectedMeshGroupIndex].get_Meshes()[i];
					val.get_Meshes().Add(Mesh.Copy(val2, (ReportProgressRatio)delegate(double progress0To1, string processingState, out bool continueProcessing)
					{
						ReportProgressChanged(progress0To1, processingState, out continueProcessing);
					}, true));
				}
				PlatingHelper.FindPositionForGroupAndAddToPlate(val, base.MeshGroupTransforms[selectedMeshGroupIndex], asyncPlatingDatas, asyncMeshGroups, asyncMeshGroupTransforms);
				PlatingHelper.CreateITraceableForMeshGroup(asyncPlatingDatas, asyncMeshGroups, asyncMeshGroups.Count - 1, null);
			}
			ReportProgressChanged(0.95, "", out var _);
		}

		private async void MakeCopyOfGroup()
		{
			if (base.MeshGroups.Count <= 0 || !base.HaveSelection)
			{
				return;
			}
			string arg = "Making Copy".Localize();
			string processType = $"{arg}:";
			processingProgressControl.set_ProcessType(processType);
			((GuiWidget)processingProgressControl).set_Visible(true);
			processingProgressControl.set_PercentComplete(0);
			LockEditControls();
			await Task.Run(new Action(CopyGroup));
			if (!((GuiWidget)this).get_HasBeenClosed())
			{
				UnlockEditControls();
				PullMeshGroupDataFromAsynchLists();
				PartHasBeenChanged();
				int num = base.MeshGroups.Count - 1;
				int count = base.SelectedMeshGroupIndices.Count;
				UndoBuffer.Add((IUndoRedoCommand)(object)new CopyUndoCommand(this, num, base.SelectedMeshGroupIndices));
				base.SelectedMeshGroupIndices.Clear();
				for (int i = num - count + 1; i <= num; i++)
				{
					base.SelectedMeshGroupIndices.Add(i);
				}
			}
		}

		private void GroupSelected()
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Expected O, but got Unknown
			string arg = "Grouping".Localize();
			string processType = $"{arg}:";
			processingProgressControl.set_ProcessType(processType);
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			PushMeshGroupDataToAsynchLists(TraceInfoOpperation.DO_COPY);
			for (int i = 0; i < asyncMeshGroups.Count; i++)
			{
				asyncMeshGroups[i].Transform(asyncMeshGroupTransforms[i]);
				ReportProgressChanged((double)(i + 1) * 0.4 / (double)asyncMeshGroups.Count, "", out var _);
			}
			base.SelectedMeshGroupIndices.Sort();
			MeshGroup val = asyncMeshGroups[base.SelectedMeshGroupIndices[0]];
			base.SelectedMeshGroupIndices.Reverse();
			foreach (int selectedMeshGroupIndex in base.SelectedMeshGroupIndices)
			{
				MeshGroup val2 = asyncMeshGroups[selectedMeshGroupIndex];
				if (val2 != val)
				{
					for (int j = 0; j < val2.get_Meshes().Count; j++)
					{
						Mesh item = val2.get_Meshes()[j];
						val.get_Meshes().Add(item);
					}
					asyncMeshGroups.RemoveAt(selectedMeshGroupIndex);
					asyncMeshGroupTransforms.RemoveAt(selectedMeshGroupIndex);
				}
				else
				{
					asyncMeshGroupTransforms[selectedMeshGroupIndex] = Matrix4X4.Identity;
				}
			}
			asyncPlatingDatas.Clear();
			double num = 1.0 / (double)asyncMeshGroups.Count;
			double num2 = 0.0;
			for (int k = 0; k < asyncMeshGroups.Count; k++)
			{
				PlatingMeshGroupData item2 = new PlatingMeshGroupData();
				asyncPlatingDatas.Add(item2);
				_ = asyncMeshGroups[k];
				PlatingHelper.CreateITraceableForMeshGroup(asyncPlatingDatas, asyncMeshGroups, k, (ReportProgressRatio)delegate(double progress0To1, string processingState, out bool continueProcessing)
				{
					ReportProgressChanged(progress0To1, processingState, out continueProcessing);
				});
				num2 += num;
			}
		}

		private async void GroupSelectedMeshs()
		{
			if (base.MeshGroups.Count <= 0)
			{
				return;
			}
			processingProgressControl.set_PercentComplete(0);
			((GuiWidget)processingProgressControl).set_Visible(true);
			LockEditControls();
			viewIsInEditModePreLock = true;
			Tuple<List<int>, List<MeshGroup>, List<Matrix4X4>, List<PlatingMeshGroupData>> undoData = MakeCopyOfAllMeshData();
			if (base.SelectedMeshGroupIndices.Count < 2)
			{
				base.SelectedMeshGroupIndices.Clear();
				for (int i = 0; i < base.MeshGroups.Count; i++)
				{
					base.SelectedMeshGroupIndices.Add(i);
				}
			}
			int keepIndex = Enumerable.Min((IEnumerable<int>)base.SelectedMeshGroupIndices);
			await Task.Run(new Action(GroupSelected));
			if (!((GuiWidget)this).get_HasBeenClosed())
			{
				PullMeshGroupDataFromAsynchLists();
				base.SelectedMeshGroupIndices.Clear();
				base.SelectedMeshGroupIndices.Add(keepIndex);
				UndoBuffer.Add((IUndoRedoCommand)(object)new MeshGroupUndoCommand(this, undoData));
				UnlockEditControls();
				PartHasBeenChanged();
				((GuiWidget)this).Invalidate();
			}
		}

		private void UngroupSelected()
		{
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Expected O, but got Unknown
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Expected O, but got Unknown
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Expected O, but got Unknown
			//IL_01f7: Expected O, but got Unknown
			string arg = "Ungrouping".Localize();
			string processType = $"{arg}:";
			processingProgressControl.set_ProcessType(processType);
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			PushMeshGroupDataToAsynchLists(TraceInfoOpperation.DO_COPY);
			base.SelectedMeshGroupIndices.Sort();
			base.SelectedMeshGroupIndices.Reverse();
			foreach (int selectedMeshGroupIndex in base.SelectedMeshGroupIndices)
			{
				List<Mesh> list = new List<Mesh>();
				asyncMeshGroups[selectedMeshGroupIndex].Transform(asyncMeshGroupTransforms[selectedMeshGroupIndex]);
				if (asyncMeshGroups[selectedMeshGroupIndex].get_Meshes().Count > 1)
				{
					foreach (Mesh mesh in asyncMeshGroups[selectedMeshGroupIndex].get_Meshes())
					{
						list.Add(mesh);
					}
				}
				else
				{
					list = CreateDiscreteMeshes.SplitConnectedIntoMeshes(asyncMeshGroups[selectedMeshGroupIndex], (ReportProgressRatio)delegate(double progress0To1, string processingState, out bool continueProcessing)
					{
						ReportProgressChanged(progress0To1 * 0.5, processingState, out continueProcessing);
					});
				}
				asyncMeshGroups.RemoveAt(selectedMeshGroupIndex);
				asyncPlatingDatas.RemoveAt(selectedMeshGroupIndex);
				asyncMeshGroupTransforms.RemoveAt(selectedMeshGroupIndex);
				double num = 1.0 / (double)list.Count;
				double currentRatioDone = 0.0;
				ReportProgressRatio val = default(ReportProgressRatio);
				for (int i = 0; i < list.Count; i++)
				{
					PlatingMeshGroupData item = new PlatingMeshGroupData();
					asyncPlatingDatas.Add(item);
					asyncMeshGroups.Add(new MeshGroup(list[i]));
					int num2 = asyncMeshGroups.Count - 1;
					_ = asyncMeshGroups[num2];
					Matrix4X4 identity = Matrix4X4.Identity;
					asyncMeshGroupTransforms.Add(identity);
					List<PlatingMeshGroupData> perMeshGroupInfo = asyncPlatingDatas;
					List<MeshGroup> meshGroups = asyncMeshGroups;
					ReportProgressRatio obj = val;
					if (obj == null)
					{
						ReportProgressRatio val2 = delegate(double progress0To1, string processingState, out bool continueProcessing)
						{
							ReportProgressChanged(0.5 + progress0To1 * 0.5 * currentRatioDone, processingState, out continueProcessing);
						};
						ReportProgressRatio val3 = val2;
						val = val2;
						obj = val3;
					}
					PlatingHelper.CreateITraceableForMeshGroup(perMeshGroupInfo, meshGroups, num2, obj);
					currentRatioDone += num;
				}
			}
		}

		private async void UngroupSelectedMeshGroup()
		{
			if (base.MeshGroups.Count <= 0)
			{
				return;
			}
			processingProgressControl.set_PercentComplete(0);
			((GuiWidget)processingProgressControl).set_Visible(true);
			LockEditControls();
			viewIsInEditModePreLock = true;
			Tuple<List<int>, List<MeshGroup>, List<Matrix4X4>, List<PlatingMeshGroupData>> undoData = MakeCopyOfAllMeshData();
			if (!base.HaveSelection)
			{
				base.SelectedMeshGroupIndices.Clear();
				for (int i = 0; i < base.MeshGroups.Count; i++)
				{
					base.SelectedMeshGroupIndices.Add(i);
				}
			}
			await Task.Run(new Action(UngroupSelected));
			if (!((GuiWidget)this).get_HasBeenClosed())
			{
				PullMeshGroupDataFromAsynchLists();
				base.SelectedMeshGroupIndices.Clear();
				UndoBuffer.Add((IUndoRedoCommand)(object)new MeshGroupUndoCommand(this, undoData));
				UnlockEditControls();
				PartHasBeenChanged();
				((GuiWidget)this).Invalidate();
			}
		}

		public View3DWidget(PrintItemWrapper printItemWrapper, Vector3 viewerVolume, Vector2 bedCenter, BedShape bedShape, WindowMode windowType, AutoRotate autoRotate, OpenMode openMode = OpenMode.Viewing)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Expected O, but got Unknown
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Expected O, but got Unknown
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Expected O, but got Unknown
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Expected O, but got Unknown
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Expected O, but got Unknown
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Expected O, but got Unknown
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Expected O, but got Unknown
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Expected O, but got Unknown
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Expected O, but got Unknown
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Expected O, but got Unknown
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Expected O, but got Unknown
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0705: Unknown result type (might be due to invalid IL or missing references)
			//IL_070c: Expected O, but got Unknown
			//IL_0713: Unknown result type (might be due to invalid IL or missing references)
			//IL_0731: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fd: Expected O, but got Unknown
			//IL_0804: Unknown result type (might be due to invalid IL or missing references)
			//IL_0822: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c8: Expected O, but got Unknown
			//IL_0937: Unknown result type (might be due to invalid IL or missing references)
			//IL_093c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0943: Unknown result type (might be due to invalid IL or missing references)
			//IL_094f: Expected O, but got Unknown
			//IL_095a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0964: Unknown result type (might be due to invalid IL or missing references)
			//IL_0992: Unknown result type (might be due to invalid IL or missing references)
			//IL_0999: Expected O, but got Unknown
			this.openMode = openMode;
			this.windowType = windowType;
			allowAutoRotate = autoRotate == AutoRotate.Enabled;
			autoRotating = allowAutoRotate;
			MeshGroupExtraData = new List<PlatingMeshGroupData>();
			MeshGroupExtraData.Add(new PlatingMeshGroupData());
			this.printItemWrapper = printItemWrapper;
			((GuiWidget)this).set_Name("View3DWidget");
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)13);
			((GuiWidget)val).set_VAnchor((VAnchor)13);
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_Name("centerPartPreviewAndControls");
			((GuiWidget)val2).AnchorAll();
			GuiWidget val3 = new GuiWidget();
			val3.set_BackgroundColor(RGBA_Bytes.Black);
			val3.AnchorAll();
			base.meshViewerWidget = new MeshViewerWidget(viewerVolume, bedCenter, bedShape, "Press 'Add' to select an item.".Localize());
			MeshViewerWidget meshViewerWidget = base.meshViewerWidget;
			meshViewerWidget.SelectionChanged = (EventHandler)Delegate.Combine(meshViewerWidget.SelectionChanged, (EventHandler)delegate(object sender, EventArgs e)
			{
				OnSelectionChanged(e);
			});
			PutOemImageOnBed();
			((GuiWidget)base.meshViewerWidget).AnchorAll();
			val3.AddChild((GuiWidget)(object)base.meshViewerWidget, -1);
			((GuiWidget)val2).AddChild(val3, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val4).set_HAnchor((HAnchor)5);
			((GuiWidget)val4).set_Padding(new BorderDouble(3.0, 3.0));
			((GuiWidget)val4).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			buttonRightPanel = CreateRightButtonPanel(viewerVolume.y);
			((GuiWidget)buttonRightPanel).set_Name("buttonRightPanel");
			((GuiWidget)buttonRightPanel).set_Visible(false);
			CreateOptionsContent();
			FlowLayoutWidget val5 = new FlowLayoutWidget((FlowDirection)0);
			string text = "Entering Editor".Localize();
			string text2 = StringHelper.FormatWith("{0}:", new object[1]
			{
				text
			});
			processingProgressControl = new ProgressControl(text2, ActiveTheme.get_Instance().get_PrimaryTextColor(), ActiveTheme.get_Instance().get_PrimaryAccentColor(), 80, 15, 5);
			((GuiWidget)processingProgressControl).set_VAnchor((VAnchor)2);
			((GuiWidget)val5).AddChild((GuiWidget)(object)processingProgressControl, -1);
			((GuiWidget)val5).set_VAnchor((VAnchor)(((GuiWidget)val5).get_VAnchor() | 2));
			((GuiWidget)processingProgressControl).set_Visible(false);
			enterEditButtonsContainer = new FlowLayoutWidget((FlowDirection)0);
			Button val6 = textImageButtonFactory.Generate("Insert".Localize(), "icon_insert_32x32.png");
			((GuiWidget)val6).set_ToolTipText("Insert an .stl, .amf or .zip file".Localize());
			((GuiWidget)val6).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 0.0));
			((GuiWidget)enterEditButtonsContainer).AddChild((GuiWidget)(object)val6, -1);
			((GuiWidget)val6).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					DoAddFileAfterCreatingEditData = true;
					EnterEditAndCreateSelectionData();
				});
			});
			if (printItemWrapper != null && printItemWrapper.PrintItem.ReadOnly)
			{
				((GuiWidget)val6).set_Enabled(false);
			}
			ImageBuffer normalImage = StaticData.get_Instance().LoadIcon("icon_edit.png", 14, 14);
			Button val7 = textImageButtonFactory.Generate("Edit".Localize(), normalImage);
			((GuiWidget)val7).set_Name("3D View Edit");
			((GuiWidget)val7).set_Margin(new BorderDouble(0.0, 0.0, 4.0, 0.0));
			((GuiWidget)val7).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				EnterEditAndCreateSelectionData();
			});
			if (printItemWrapper != null && printItemWrapper.PrintItem.ReadOnly)
			{
				((GuiWidget)val7).set_Enabled(false);
			}
			Button val8 = textImageButtonFactory.Generate("Export".Localize() + "...");
			((GuiWidget)val8).set_Margin(new BorderDouble(0.0, 0.0, 10.0, 0.0));
			((GuiWidget)val8).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					OpenExportWindow();
				});
			});
			((GuiWidget)enterEditButtonsContainer).AddChild((GuiWidget)(object)val7, -1);
			((GuiWidget)enterEditButtonsContainer).AddChild((GuiWidget)(object)val8, -1);
			((GuiWidget)val5).AddChild((GuiWidget)(object)enterEditButtonsContainer, -1);
			doEdittingButtonsContainer = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)doEdittingButtonsContainer).set_Visible(false);
			Button val9 = textImageButtonFactory.Generate("Insert".Localize(), "icon_insert_32x32.png");
			((GuiWidget)val9).set_Margin(new BorderDouble(0.0, 0.0, 10.0, 0.0));
			((GuiWidget)doEdittingButtonsContainer).AddChild((GuiWidget)(object)val9, -1);
			((GuiWidget)val9).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					//IL_002b: Expected O, but got Unknown
					FileDialog.OpenFileDialog(new OpenFileDialogParams(ApplicationSettings.OpenDesignFileParams, "", true, "", ""), (Action<OpenFileDialogParams>)delegate(OpenFileDialogParams openParams)
					{
						LoadAndAddPartsToPlate(((FileDialogParams)openParams).get_FileNames());
					});
				});
			});
			GuiWidget val10 = new GuiWidget(1.0, 2.0, (SizeLimitsToSet)1);
			val10.set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			val10.set_Margin(new BorderDouble(4.0, 2.0));
			val10.set_VAnchor((VAnchor)5);
			((GuiWidget)doEdittingButtonsContainer).AddChild(val10, -1);
			Button val11 = textImageButtonFactory.Generate("Ungroup".Localize());
			((GuiWidget)val11).set_Name("3D View Ungroup");
			((GuiWidget)doEdittingButtonsContainer).AddChild((GuiWidget)(object)val11, -1);
			((GuiWidget)val11).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UngroupSelectedMeshGroup();
			});
			Button val12 = textImageButtonFactory.Generate("Group".Localize());
			((GuiWidget)val12).set_Name("3D View Group");
			((GuiWidget)doEdittingButtonsContainer).AddChild((GuiWidget)(object)val12, -1);
			((GuiWidget)val12).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				GroupSelectedMeshs();
			});
			Button val13 = textImageButtonFactory.Generate("Align".Localize());
			((GuiWidget)doEdittingButtonsContainer).AddChild((GuiWidget)(object)val13, -1);
			((GuiWidget)val13).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				AlignToSelectedMeshGroup();
			});
			Button val14 = textImageButtonFactory.Generate("Arrange".Localize());
			((GuiWidget)doEdittingButtonsContainer).AddChild((GuiWidget)(object)val14, -1);
			((GuiWidget)val14).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				AutoArrangePartsInBackground();
			});
			GuiWidget val15 = new GuiWidget(1.0, 2.0, (SizeLimitsToSet)1);
			val15.set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			val15.set_Margin(new BorderDouble(4.0, 2.0));
			val15.set_VAnchor((VAnchor)5);
			((GuiWidget)doEdittingButtonsContainer).AddChild(val15, -1);
			Button val16 = textImageButtonFactory.Generate("Copy".Localize());
			((GuiWidget)val16).set_Name("3D View Copy");
			((GuiWidget)doEdittingButtonsContainer).AddChild((GuiWidget)(object)val16, -1);
			((GuiWidget)val16).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				MakeCopyOfGroup();
			});
			Button val17 = textImageButtonFactory.Generate("Remove".Localize());
			((GuiWidget)val17).set_Name("3D View Remove");
			((GuiWidget)doEdittingButtonsContainer).AddChild((GuiWidget)(object)val17, -1);
			((GuiWidget)val17).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				DeleteSelectedMesh();
			});
			GuiWidget val18 = new GuiWidget(1.0, 2.0, (SizeLimitsToSet)1);
			val18.set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			val18.set_Margin(new BorderDouble(4.0, 1.0));
			val18.set_VAnchor((VAnchor)5);
			((GuiWidget)doEdittingButtonsContainer).AddChild(val18, -1);
			Button val19 = textImageButtonFactory.Generate("Cancel".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)val19).set_Name("3D View Cancel");
			((GuiWidget)val19).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					if (((GuiWidget)saveButtons).get_Visible())
					{
						StyledMessageBox.ShowMessageBox(ExitEditingAndSaveIfRequired, "Would you like to save your changes before exiting the editor?".Localize(), "Save Changes".Localize(), StyledMessageBox.MessageType.YES_NO, "Save Changed".Localize(), "Discard Changes".Localize());
					}
					else if (partHasBeenEdited)
					{
						ExitEditingAndSaveIfRequired(response: false);
					}
					else
					{
						SwitchStateToNotEditing();
					}
				});
			});
			((GuiWidget)doEdittingButtonsContainer).AddChild((GuiWidget)(object)val19, -1);
			AddSaveAndSaveAs(doEdittingButtonsContainer);
			((GuiWidget)val5).AddChild((GuiWidget)(object)doEdittingButtonsContainer, -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)val5, -1);
			GuiWidget val20 = new GuiWidget();
			val20.set_HAnchor((HAnchor)8);
			val20.set_VAnchor((VAnchor)5);
			GuiWidget buttonRightPanelHolder = val20;
			buttonRightPanelHolder.set_Name("buttonRightPanelHolder");
			((GuiWidget)val2).AddChild(buttonRightPanelHolder, -1);
			buttonRightPanelHolder.AddChild((GuiWidget)(object)buttonRightPanel, -1);
			((GuiWidget)buttonRightPanel).add_VisibleChanged((EventHandler)delegate
			{
				buttonRightPanelHolder.set_Visible(((GuiWidget)buttonRightPanel).get_Visible());
			});
			viewControls3D = new ViewControls3D(base.meshViewerWidget);
			viewControls3D.ResetView += delegate
			{
				base.meshViewerWidget.ResetView();
			};
			GuiWidget val21 = new GuiWidget();
			val21.set_HAnchor((HAnchor)5);
			val21.set_VAnchor((VAnchor)5);
			buttonRightPanelDisabledCover = val21;
			buttonRightPanelDisabledCover.set_BackgroundColor(new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryBackgroundColor(), 150));
			buttonRightPanelHolder.AddChild(buttonRightPanelDisabledCover, -1);
			viewControls3D.PartSelectVisible = false;
			LockEditControls();
			GuiWidget val22 = new GuiWidget();
			val22.set_HAnchor((HAnchor)5);
			((GuiWidget)val4).AddChild(val22, -1);
			if (windowType == WindowMode.StandAlone)
			{
				Button val23 = textImageButtonFactory.Generate("Close".Localize());
				((GuiWidget)val4).AddChild((GuiWidget)(object)val23, -1);
				((GuiWidget)val23).add_Click((EventHandler<MouseEventArgs>)delegate
				{
					((GuiWidget)this).CloseOnIdle();
				});
			}
			((GuiWidget)val).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)this).AnchorAll();
			base.meshViewerWidget.TrackballTumbleWidget.TransformState = (MouseDownType)2;
			((GuiWidget)this).AddChild((GuiWidget)(object)viewControls3D, -1);
			UiThread.RunOnIdle((Action)AutoSpin);
			if (printItemWrapper == null && windowType == WindowMode.Embeded)
			{
				((GuiWidget)enterEditButtonsContainer).set_Visible(false);
			}
			if (windowType == WindowMode.Embeded)
			{
				PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)SetEditControlsBasedOnPrinterState, ref unregisterEvents);
				if (windowType == WindowMode.Embeded)
				{
					PrinterConnectionAndCommunication.CommunicationStates communicationState = PrinterConnectionAndCommunication.Instance.CommunicationState;
					if (communicationState == PrinterConnectionAndCommunication.CommunicationStates.Printing || communicationState == PrinterConnectionAndCommunication.CommunicationStates.Paused)
					{
						LockEditControls();
					}
				}
			}
			ActiveTheme.ThemeChanged.RegisterEvent((EventHandler)ThemeChanged, ref unregisterEvents);
			base.meshViewerWidget.interactionVolumes.Add(new UpArrow3D(this));
			base.meshViewerWidget.interactionVolumes.Add(new SelectionShadow(this));
			base.meshViewerWidget.interactionVolumes.Add(new SnappingIndicators(this));
			foreach (InteractionVolumePlugin plugin in new PluginFinder<InteractionVolumePlugin>((string)null, (IComparer<InteractionVolumePlugin>)null).Plugins)
			{
				base.meshViewerWidget.interactionVolumes.Add(plugin.CreateInteractionVolume(this));
			}
			ThemeChanged(this, null);
			((GuiWidget)saveButtons).add_VisibleChanged((EventHandler)delegate
			{
				partHasBeenEdited = true;
			});
			base.meshViewerWidget.ResetView();
			ClearBedAndLoadPrintItemWrapper(printItemWrapper);
			base.meshViewerWidget.TrackballTumbleWidget.DrawGlContent += trackballTumbleWidget_DrawGlContent;
		}

		private void trackballTumbleWidget_DrawGlContent(object sender, EventArgs e)
		{
			_ = allObjects;
		}

		public override void OnKeyDown(KeyEventArgs keyEvent)
		{
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Invalid comparison between Unknown and I4
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Invalid comparison between Unknown and I4
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Invalid comparison between Unknown and I4
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Invalid comparison between Unknown and I4
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Invalid comparison between Unknown and I4
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Invalid comparison between Unknown and I4
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Invalid comparison between Unknown and I4
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			if (!(Enumerable.FirstOrDefault<GuiWidget>(Enumerable.Where<GuiWidget>(ExtensionMethods.ChildrenRecursive<GuiWidget>((GuiWidget)(object)this), (Func<GuiWidget, bool>)((GuiWidget x) => x.get_Focused()))) is InternalTextEditWidget))
			{
				if (!activeButtonBeforeKeyOverride.HasValue)
				{
					activeButtonBeforeKeyOverride = viewControls3D.ActiveButton;
					if (keyEvent.get_Alt())
					{
						viewControls3D.ActiveButton = ViewControls3DButtons.Rotate;
					}
					else if (keyEvent.get_Shift())
					{
						viewControls3D.ActiveButton = ViewControls3DButtons.Translate;
					}
				}
				if ((keyEvent.get_KeyData() & -65536) == 131072)
				{
					selectMultiple = true;
				}
				Keys keyCode = keyEvent.get_KeyCode();
				if ((int)keyCode <= 27)
				{
					if ((int)keyCode == 8)
					{
						goto IL_011d;
					}
					if ((int)keyCode == 27 && CurrentSelectInfo.DownOnPart)
					{
						CurrentSelectInfo.DownOnPart = false;
						foreach (int selectedMeshGroupIndex in base.SelectedMeshGroupIndices)
						{
							base.MeshGroupTransforms[selectedMeshGroupIndex] = transformsOnMouseDown[selectedMeshGroupIndex];
						}
						((GuiWidget)this).Invalidate();
					}
				}
				else
				{
					if ((int)keyCode == 46)
					{
						goto IL_011d;
					}
					if ((int)keyCode != 89)
					{
						if ((int)keyCode == 90 && keyEvent.get_Control())
						{
							UndoBuffer.Undo(1);
							keyEvent.set_Handled(true);
							keyEvent.set_SuppressKeyPress(true);
						}
					}
					else if (keyEvent.get_Control())
					{
						UndoBuffer.Redo(1);
						keyEvent.set_Handled(true);
						keyEvent.set_SuppressKeyPress(true);
					}
				}
			}
			goto IL_018b;
			IL_018b:
			((GuiWidget)this).OnKeyDown(keyEvent);
			return;
			IL_011d:
			DeleteSelectedMesh();
			goto IL_018b;
		}

		private void AddGridSnapSettings(GuiWidget widgetToAddTo)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Expected O, but got Unknown
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_Margin(new BorderDouble(5.0, 0.0));
			FlowLayoutWidget val2 = val;
			TextWidget val3 = new TextWidget("Snap Grid".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val3.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val3).set_VAnchor((VAnchor)2);
			((GuiWidget)val3).set_Margin(new BorderDouble(3.0, 0.0, 0.0, 0.0));
			TextWidget val4 = val3;
			((GuiWidget)val2).AddChild((GuiWidget)(object)val4, -1);
			DropDownList val5 = new DropDownList("Custom", (Direction)0, 0.0, false);
			((GuiWidget)val5).set_VAnchor((VAnchor)10);
			DropDownList val6 = val5;
			foreach (KeyValuePair<double, string> snapSetting in new Dictionary<double, string>
			{
				{
					0.0,
					"Off"
				},
				{
					0.1,
					"0.1"
				},
				{
					0.25,
					"0.25"
				},
				{
					0.5,
					"0.5"
				},
				{
					1.0,
					"1"
				},
				{
					2.0,
					"2"
				},
				{
					5.0,
					"5"
				}
			})
			{
				double key = snapSetting.Key;
				MenuItem obj = val6.AddItem(snapSetting.Value, (string)null, 12.0);
				if (meshViewerWidget.SnapGridDistance == key)
				{
					val6.set_SelectedLabel(snapSetting.Value);
				}
				obj.add_Selected((EventHandler)delegate
				{
					meshViewerWidget.SnapGridDistance = snapSetting.Key;
				});
			}
			((GuiWidget)val2).AddChild((GuiWidget)(object)val6, -1);
			widgetToAddTo.AddChild((GuiWidget)(object)val2, -1);
		}

		public Tuple<List<int>, List<MeshGroup>, List<Matrix4X4>, List<PlatingMeshGroupData>> MakeCopyOfAllMeshData()
		{
			List<int> item = new List<int>(base.SelectedMeshGroupIndices);
			List<MeshGroup> item2 = new List<MeshGroup>(base.MeshGroups);
			List<Matrix4X4> item3 = new List<Matrix4X4>(base.MeshGroupTransforms);
			List<PlatingMeshGroupData> item4 = new List<PlatingMeshGroupData>(MeshGroupExtraData);
			return Tuple.Create(item, item2, item3, item4);
		}

		public void OnSelectionChanged(EventArgs e)
		{
			SelectionChanged?.Invoke(this, e);
			this.SelectedTransformChanged?.Invoke(this, null);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			base.OnClosed(e);
		}

		public override void OnDragDrop(FileDropEventArgs fileDropEventArgs)
		{
			if (AllowDragDrop())
			{
				pendingPartsToLoad.Clear();
				foreach (string droppedFile in fileDropEventArgs.DroppedFiles)
				{
					string text = Path.GetExtension(droppedFile)!.ToLower();
					if (text != "" && ApplicationSettings.OpenDesignFileParams.Contains(text))
					{
						pendingPartsToLoad.Add(droppedFile);
					}
				}
				if (pendingPartsToLoad.Count > 0)
				{
					if (((GuiWidget)enterEditButtonsContainer).get_Visible())
					{
						EnterEditAndCreateSelectionData();
					}
					else
					{
						LoadAndAddPartsToPlate(pendingPartsToLoad.ToArray());
						pendingPartsToLoad.Clear();
					}
				}
			}
			((GuiWidget)this).OnDragDrop(fileDropEventArgs);
		}

		public override void OnDragEnter(FileDropEventArgs fileDropEventArgs)
		{
			if (AllowDragDrop())
			{
				foreach (string droppedFile in fileDropEventArgs.DroppedFiles)
				{
					string text = Path.GetExtension(droppedFile)!.ToLower();
					if (text != "" && ApplicationSettings.OpenDesignFileParams.Contains(text))
					{
						fileDropEventArgs.set_AcceptDrop(true);
					}
				}
			}
			((GuiWidget)this).OnDragEnter(fileDropEventArgs);
		}

		public override void OnDragOver(FileDropEventArgs fileDropEventArgs)
		{
			if (AllowDragDrop())
			{
				foreach (string droppedFile in fileDropEventArgs.DroppedFiles)
				{
					string text = Path.GetExtension(droppedFile)!.ToLower();
					if (text != "" && ApplicationSettings.OpenDesignFileParams.Contains(text))
					{
						fileDropEventArgs.set_AcceptDrop(true);
					}
				}
			}
			((GuiWidget)this).OnDragOver(fileDropEventArgs);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			if (base.HaveSelection)
			{
				foreach (InteractionVolume interactionVolume in meshViewerWidget.interactionVolumes)
				{
					interactionVolume.SetPosition();
				}
			}
			hasDrawn = true;
			base.OnDraw(graphics2D);
		}

		public override void OnKeyUp(KeyEventArgs keyEvent)
		{
			if (activeButtonBeforeKeyOverride.HasValue)
			{
				viewControls3D.ActiveButton = activeButtonBeforeKeyOverride.Value;
				activeButtonBeforeKeyOverride = null;
			}
			if (!keyEvent.get_Control())
			{
				selectMultiple = false;
			}
			((GuiWidget)this).OnKeyUp(keyEvent);
		}

		public override void OnMouseDown(MouseEventArgs mouseEvent)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Invalid comparison between Unknown and I4
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Invalid comparison between Unknown and I4
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Invalid comparison between Unknown and I4
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Invalid comparison between Unknown and I4
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Invalid comparison between Unknown and I4
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Expected O, but got Unknown
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Expected O, but got Unknown
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			if (!activeButtonBeforeMouseOverride.HasValue && (int)mouseEvent.get_Button() == 2097152)
			{
				activeButtonBeforeMouseOverride = viewControls3D.ActiveButton;
				viewControls3D.ActiveButton = ViewControls3DButtons.Rotate;
			}
			else if (!activeButtonBeforeMouseOverride.HasValue && (int)mouseEvent.get_Button() == 4194304)
			{
				activeButtonBeforeMouseOverride = viewControls3D.ActiveButton;
				viewControls3D.ActiveButton = ViewControls3DButtons.Translate;
			}
			autoRotating = false;
			((GuiWidget)this).OnMouseDown(mouseEvent);
			if ((int)((GuiWidget)meshViewerWidget.TrackballTumbleWidget).get_UnderMouseState() != 2 || (int)meshViewerWidget.TrackballTumbleWidget.TransformState != 0 || (int)mouseEvent.get_Button() != 1048576 || (int)((GuiWidget)this).get_ModifierKeys() == 65536 || meshViewerWidget.MouseDownOnInteractionVolume)
			{
				return;
			}
			IntersectInfo info = new IntersectInfo();
			if (FindMeshGroupHitPosition(mouseEvent.get_Position(), out var meshHitIndex, ref info))
			{
				CurrentSelectInfo.HitPlane = new PlaneShape(Vector3.UnitZ, CurrentSelectInfo.PlaneDownHitPos.z, (MaterialAbstract)null);
				if (selectMultiple)
				{
					if (base.SelectedMeshGroupIndices.Contains(meshHitIndex))
					{
						base.SelectedMeshGroupIndices.Remove(meshHitIndex);
					}
					else
					{
						base.SelectedMeshGroupIndices.Add(meshHitIndex);
					}
				}
				else if (!base.SelectedMeshGroupIndices.Contains(meshHitIndex))
				{
					base.SelectedMeshGroupIndices.Clear();
					base.SelectedMeshGroupIndices.Add(meshHitIndex);
				}
				transformsOnMouseDown = base.SelectedMeshGroupTransforms;
				((GuiWidget)this).Invalidate();
				CurrentSelectInfo.DownOnPart = true;
				AxisAlignedBoundingBox boundsForSelection = meshViewerWidget.GetBoundsForSelection();
				if (info.hitPosition.x < boundsForSelection.get_Center().x)
				{
					if (info.hitPosition.y < boundsForSelection.get_Center().y)
					{
						CurrentSelectInfo.HitQuadrant = HitQuadrant.LB;
					}
					else
					{
						CurrentSelectInfo.HitQuadrant = HitQuadrant.LT;
					}
				}
				else if (info.hitPosition.y < boundsForSelection.get_Center().y)
				{
					CurrentSelectInfo.HitQuadrant = HitQuadrant.RB;
				}
				else
				{
					CurrentSelectInfo.HitQuadrant = HitQuadrant.RT;
				}
			}
			else
			{
				base.SelectedMeshGroupIndices.Clear();
			}
			this.SelectedTransformChanged?.Invoke(this, null);
		}

		public override void OnMouseMove(MouseEventArgs mouseEvent)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			if ((int)meshViewerWidget.TrackballTumbleWidget.TransformState == 0 && CurrentSelectInfo.DownOnPart)
			{
				Vector2 screenPosition = ((GuiWidget)meshViewerWidget).TransformFromParentSpace((GuiWidget)(object)this, new Vector2(mouseEvent.get_X(), mouseEvent.get_Y()));
				Ray rayFromScreen = meshViewerWidget.TrackballTumbleWidget.GetRayFromScreen(screenPosition);
				IntersectInfo closestIntersection = ((BaseShape)CurrentSelectInfo.HitPlane).GetClosestIntersection(rayFromScreen);
				if (closestIntersection != null)
				{
					foreach (int selectedMeshGroupIndex in base.SelectedMeshGroupIndices)
					{
						Matrix4X4 val = Matrix4X4.CreateTranslation(new Vector3(-CurrentSelectInfo.LastMoveDelta));
						List<Matrix4X4> meshGroupTransforms = base.MeshGroupTransforms;
						int index = selectedMeshGroupIndex;
						meshGroupTransforms[index] *= val;
					}
					Vector3 val2 = closestIntersection.hitPosition - CurrentSelectInfo.PlaneDownHitPos;
					double snapGridDistance = meshViewerWidget.SnapGridDistance;
					if (snapGridDistance > 0.0)
					{
						AxisAlignedBoundingBox boundsForSelection = meshViewerWidget.GetBoundsForSelection();
						double x = boundsForSelection.minXYZ.x;
						if (CurrentSelectInfo.HitQuadrant == HitQuadrant.RB || CurrentSelectInfo.HitQuadrant == HitQuadrant.RT)
						{
							x = boundsForSelection.maxXYZ.x;
						}
						double num = Math.Round((x + val2.x) / snapGridDistance) * snapGridDistance;
						val2.x = num - x;
						double y = boundsForSelection.minXYZ.y;
						if (CurrentSelectInfo.HitQuadrant == HitQuadrant.LT || CurrentSelectInfo.HitQuadrant == HitQuadrant.RT)
						{
							y = boundsForSelection.maxXYZ.y;
						}
						double num2 = Math.Round((y + val2.y) / snapGridDistance) * snapGridDistance;
						val2.y = num2 - y;
					}
					foreach (int selectedMeshGroupIndex2 in base.SelectedMeshGroupIndices)
					{
						Matrix4X4 val3 = Matrix4X4.CreateTranslation(new Vector3(val2));
						List<Matrix4X4> meshGroupTransforms = base.MeshGroupTransforms;
						int index = selectedMeshGroupIndex2;
						meshGroupTransforms[index] *= val3;
						CurrentSelectInfo.LastMoveDelta = val2;
					}
					LastHitPosition = closestIntersection.hitPosition;
					((GuiWidget)this).Invalidate();
				}
			}
			((GuiWidget)this).OnMouseMove(mouseEvent);
		}

		public void AddUndoForSelectedMeshGroupTransform(Matrix4X4 undoTransform)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			if (undoTransform != SelectedMeshGroupTransform)
			{
				UndoBuffer.Add((IUndoRedoCommand)(object)new TransformUndoCommand(this, SelectedMeshGroupIndex, undoTransform, SelectedMeshGroupTransform));
			}
		}

		public void AddUndoForSelectedMeshGroupTransforms(List<Matrix4X4> undoTransforms)
		{
			if (!Enumerable.SequenceEqual<Matrix4X4>((IEnumerable<Matrix4X4>)undoTransforms, (IEnumerable<Matrix4X4>)base.SelectedMeshGroupTransforms))
			{
				UndoBuffer.Add((IUndoRedoCommand)(object)new TransformUndoCommand(this, base.SelectedMeshGroupIndices, undoTransforms, base.SelectedMeshGroupTransforms));
			}
		}

		public override void OnMouseUp(MouseEventArgs mouseEvent)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if ((int)meshViewerWidget.TrackballTumbleWidget.TransformState == 0 && CurrentSelectInfo.DownOnPart && CurrentSelectInfo.LastMoveDelta != Vector3.Zero && !Enumerable.SequenceEqual<Matrix4X4>((IEnumerable<Matrix4X4>)base.SelectedMeshGroupTransforms, (IEnumerable<Matrix4X4>)transformsOnMouseDown))
			{
				AddUndoForSelectedMeshGroupTransforms(transformsOnMouseDown);
				PartHasBeenChanged();
			}
			CurrentSelectInfo.DownOnPart = false;
			if (activeButtonBeforeMouseOverride.HasValue)
			{
				viewControls3D.ActiveButton = activeButtonBeforeMouseOverride.Value;
				activeButtonBeforeMouseOverride = null;
			}
			((GuiWidget)this).OnMouseUp(mouseEvent);
		}

		public void PartHasBeenChanged()
		{
			((GuiWidget)saveButtons).set_Visible(true);
			this.SelectedTransformChanged?.Invoke(this, null);
			((GuiWidget)this).Invalidate();
		}

		public void ThemeChanged(object sender, EventArgs e)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			processingProgressControl.set_FillColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
		}

		private void AddMaterialControls(FlowLayoutWidget buttonPanel)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Expected O, but got Unknown
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Expected O, but got Unknown
			((Collection<GuiWidget>)(object)extruderButtons).Clear();
			for (int i = 0; i < ActiveSliceSettings.Instance.GetValue<int>("extruder_count"); i++)
			{
				FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
				((GuiWidget)val).set_HAnchor((HAnchor)5);
				((GuiWidget)val).set_Padding(new BorderDouble(5.0));
				RadioButton val2 = new RadioButton(string.Format("{0} {1}", "Tool".Localize(), i + 1), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12);
				((Collection<GuiWidget>)(object)extruderButtons).Add((GuiWidget)(object)val2);
				val2.set_SiblingRadioButtonList(extruderButtons);
				((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
				((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
				Button val3 = textImageButtonFactory.Generate("", StaticData.get_Instance().LoadIcon("palette.png", 24, 24));
				((GuiWidget)val3).set_VAnchor((VAnchor)2);
				int materialIndex = i + 1;
				((GuiWidget)val3).add_Click((EventHandler<MouseEventArgs>)delegate
				{
					((SystemWindow)new MeshViewerColorPicker(materialIndex)).ShowAsSystemWindow();
				});
				((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
				int extruderIndexLocal = i;
				((GuiWidget)val2).add_Click((EventHandler<MouseEventArgs>)delegate
				{
					if (base.HaveSelection)
					{
						foreach (MeshGroup selectedMeshGroup in base.SelectedMeshGroups)
						{
							foreach (Mesh mesh in selectedMeshGroup.get_Meshes())
							{
								MeshMaterialData val5 = MeshMaterialData.Get(mesh);
								if (val5.MaterialIndex != extruderIndexLocal + 1)
								{
									val5.MaterialIndex = extruderIndexLocal + 1;
									PartHasBeenChanged();
								}
							}
						}
					}
				});
				SelectionChanged = (EventHandler)Delegate.Combine(SelectionChanged, (EventHandler)delegate
				{
					//IL_003a: Unknown result type (might be due to invalid IL or missing references)
					if (base.HaveSelection)
					{
						MeshMaterialData val4 = MeshMaterialData.Get(base.SelectedMeshGroup.get_Meshes()[0]);
						for (int j = 0; j < ((Collection<GuiWidget>)(object)extruderButtons).Count; j++)
						{
							if (val4.MaterialIndex - 1 == j)
							{
								((RadioButton)((Collection<GuiWidget>)(object)extruderButtons)[j]).set_Checked(true);
							}
						}
					}
				});
				((GuiWidget)buttonPanel).AddChild((GuiWidget)(object)val, -1);
			}
		}

		private void AddRotateControls(FlowLayoutWidget buttonPanel)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Expected O, but got Unknown
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Expected O, but got Unknown
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Expected O, but got Unknown
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Expected O, but got Unknown
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Expected O, but got Unknown
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			List<GuiWidget> list = new List<GuiWidget>();
			textImageButtonFactory.FixedWidth = EditButtonHeight;
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Padding(new BorderDouble(5.0));
			string text = "Degrees".Localize();
			StringHelper.FormatWith("{0}:", new object[1]
			{
				text
			});
			TextWidget val2 = new TextWidget(text, 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			MHNumberEdit degreesControl = new MHNumberEdit(45.0, 0.0, 0.0, 12.0, 40.0, 0.0, allowNegatives: true, allowDecimals: true, -360.0, 360.0, 5.0);
			((GuiWidget)degreesControl).set_VAnchor((VAnchor)4);
			((GuiWidget)val).AddChild((GuiWidget)(object)degreesControl, -1);
			list.Add((GuiWidget)(object)degreesControl);
			((GuiWidget)buttonPanel).AddChild((GuiWidget)(object)val, -1);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			ImageBuffer normalImage = StaticData.get_Instance().LoadIcon("icon_rotate_32x32.png", 32, 32);
			Button val4 = textImageButtonFactory.Generate("", normalImage);
			TextWidget val5 = new TextWidget("X", 0.0, 0.0, 10.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val5).set_Margin(new BorderDouble(3.0, 0.0, 0.0, 0.0));
			((GuiWidget)val5).AnchorCenter();
			((GuiWidget)val4).AddChild((GuiWidget)(object)val5, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val4, -1);
			list.Add((GuiWidget)(object)val4);
			((GuiWidget)val4).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				if (base.HaveSelection)
				{
					Matrix4X4 transformToApply3 = Matrix4X4.CreateRotationX(MathHelper.DegreesToRadians(degreesControl.ActuallNumberEdit.get_Value()));
					List<Matrix4X4> list5 = new List<Matrix4X4>();
					foreach (int selectedMeshGroupIndex in base.SelectedMeshGroupIndices)
					{
						Matrix4X4 item3 = base.MeshGroupTransforms[selectedMeshGroupIndex];
						base.MeshGroupTransforms[selectedMeshGroupIndex] = PlatingHelper.ApplyAtCenter((IHasAABB)(object)base.MeshGroups[selectedMeshGroupIndex], base.MeshGroupTransforms[selectedMeshGroupIndex], transformToApply3);
						PlatingHelper.PlaceMeshGroupOnBed(base.MeshGroups, base.MeshGroupTransforms, selectedMeshGroupIndex);
						list5.Add(item3);
					}
					UndoBuffer.Add((IUndoRedoCommand)(object)new TransformUndoCommand(this, base.SelectedMeshGroupIndices, list5, base.SelectedMeshGroupTransforms));
					PartHasBeenChanged();
					((GuiWidget)this).Invalidate();
				}
			});
			Button val6 = textImageButtonFactory.Generate("", normalImage);
			TextWidget val7 = new TextWidget("Y", 0.0, 0.0, 10.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val7).set_Margin(new BorderDouble(3.0, 0.0, 0.0, 0.0));
			((GuiWidget)val7).AnchorCenter();
			((GuiWidget)val6).AddChild((GuiWidget)(object)val7, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val6, -1);
			list.Add((GuiWidget)(object)val6);
			((GuiWidget)val6).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				if (base.HaveSelection)
				{
					Matrix4X4 transformToApply2 = Matrix4X4.CreateRotationY(MathHelper.DegreesToRadians(degreesControl.ActuallNumberEdit.get_Value()));
					List<Matrix4X4> list4 = new List<Matrix4X4>();
					foreach (int selectedMeshGroupIndex2 in base.SelectedMeshGroupIndices)
					{
						Matrix4X4 item2 = base.MeshGroupTransforms[selectedMeshGroupIndex2];
						base.MeshGroupTransforms[selectedMeshGroupIndex2] = PlatingHelper.ApplyAtCenter((IHasAABB)(object)base.MeshGroups[selectedMeshGroupIndex2], base.MeshGroupTransforms[selectedMeshGroupIndex2], transformToApply2);
						PlatingHelper.PlaceMeshGroupOnBed(base.MeshGroups, base.MeshGroupTransforms, selectedMeshGroupIndex2);
						list4.Add(item2);
					}
					UndoBuffer.Add((IUndoRedoCommand)(object)new TransformUndoCommand(this, base.SelectedMeshGroupIndices, list4, base.SelectedMeshGroupTransforms));
					PartHasBeenChanged();
					((GuiWidget)this).Invalidate();
				}
			});
			Button val8 = textImageButtonFactory.Generate("", normalImage);
			TextWidget val9 = new TextWidget("Z", 0.0, 0.0, 10.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val9).set_Margin(new BorderDouble(3.0, 0.0, 0.0, 0.0));
			((GuiWidget)val9).AnchorCenter();
			((GuiWidget)val8).AddChild((GuiWidget)(object)val9, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val8, -1);
			list.Add((GuiWidget)(object)val8);
			((GuiWidget)val8).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				if (base.HaveSelection)
				{
					Matrix4X4 transformToApply = Matrix4X4.CreateRotationZ(MathHelper.DegreesToRadians(degreesControl.ActuallNumberEdit.get_Value()));
					List<Matrix4X4> list3 = new List<Matrix4X4>();
					foreach (int selectedMeshGroupIndex3 in base.SelectedMeshGroupIndices)
					{
						Matrix4X4 item = base.MeshGroupTransforms[selectedMeshGroupIndex3];
						base.MeshGroupTransforms[selectedMeshGroupIndex3] = PlatingHelper.ApplyAtCenter((IHasAABB)(object)base.MeshGroups[selectedMeshGroupIndex3], base.MeshGroupTransforms[selectedMeshGroupIndex3], transformToApply);
						PlatingHelper.PlaceMeshGroupOnBed(base.MeshGroups, base.MeshGroupTransforms, selectedMeshGroupIndex3);
						list3.Add(item);
					}
					UndoBuffer.Add((IUndoRedoCommand)(object)new TransformUndoCommand(this, base.SelectedMeshGroupIndices, list3, base.SelectedMeshGroupTransforms));
					PartHasBeenChanged();
					((GuiWidget)this).Invalidate();
				}
			});
			((GuiWidget)buttonPanel).AddChild((GuiWidget)(object)val3, -1);
			Button val10 = WhiteButtonFactory.Generate("Align to Bed".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)val10).set_Cursor((Cursors)3);
			((GuiWidget)buttonPanel).AddChild((GuiWidget)(object)val10, -1);
			((GuiWidget)val10).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				if (base.HaveSelection)
				{
					List<Matrix4X4> list2 = new List<Matrix4X4>();
					foreach (int selectedMeshGroupIndex4 in base.SelectedMeshGroupIndices)
					{
						list2.Add(base.MeshGroupTransforms[selectedMeshGroupIndex4]);
						MakeLowestFaceFlat(selectedMeshGroupIndex4);
						PlatingHelper.PlaceMeshGroupOnBed(base.MeshGroups, base.MeshGroupTransforms, selectedMeshGroupIndex4);
					}
					UndoBuffer.Add((IUndoRedoCommand)(object)new TransformUndoCommand(this, base.SelectedMeshGroupIndices, list2, base.SelectedMeshGroupTransforms));
					PartHasBeenChanged();
					((GuiWidget)this).Invalidate();
				}
			});
			((GuiWidget)buttonPanel).AddChild(GenerateHorizontalRule(), -1);
			textImageButtonFactory.FixedWidth = 0.0;
		}

		private void AddSaveAndSaveAs(FlowLayoutWidget flowToAddTo)
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			TupleList<string, Func<bool>> tupleList = new TupleList<string, Func<bool>>();
			tupleList.Add("Save".Localize(), delegate
			{
				MergeAndSavePartsToCurrentMeshFile();
				return true;
			});
			tupleList.Add("Save As".Localize(), delegate
			{
				UiThread.RunOnIdle((Action)OpenSaveAsWindow);
				return true;
			});
			SplitButtonFactory splitButtonFactory = new SplitButtonFactory();
			splitButtonFactory.FixedHeight = 40.0 * GuiWidget.get_DeviceScale();
			saveButtons = splitButtonFactory.Generate(tupleList, (Direction)0, "icon_save_32x32.png");
			((GuiWidget)saveButtons).set_Visible(false);
			((GuiWidget)saveButtons).set_Margin(default(BorderDouble));
			SplitButton splitButton = saveButtons;
			((GuiWidget)splitButton).set_VAnchor((VAnchor)(((GuiWidget)splitButton).get_VAnchor() | 2));
			((GuiWidget)flowToAddTo).AddChild((GuiWidget)(object)saveButtons, -1);
		}

		private bool AllowDragDrop()
		{
			if ((!((GuiWidget)enterEditButtonsContainer).get_Visible() && !((GuiWidget)doEdittingButtonsContainer).get_Visible()) || printItemWrapper == null || printItemWrapper.PrintItem.ReadOnly)
			{
				return false;
			}
			return true;
		}

		private void AutoSpin()
		{
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			if (!((GuiWidget)this).get_HasBeenClosed() && autoRotating)
			{
				UiThread.RunOnIdle((Action)AutoSpin, 0.04);
				if ((!timeSinceLastSpin.get_IsRunning() || timeSinceLastSpin.get_ElapsedMilliseconds() > 50) && hasDrawn)
				{
					hasDrawn = false;
					timeSinceLastSpin.Restart();
					Matrix4X4 currentRotation = meshViewerWidget.TrackballTumbleWidget.TrackBallController.get_CurrentRotation();
					Quaternion rotation = ((Matrix4X4)(ref currentRotation)).GetRotation();
					Quaternion val = Quaternion.Invert(rotation);
					Quaternion val2 = Quaternion.FromEulerAngles(new Vector3(0.0, 0.0, 0.01));
					val2 = val * val2 * rotation;
					meshViewerWidget.TrackballTumbleWidget.TrackBallController.Rotate(val2);
					((GuiWidget)this).Invalidate();
				}
			}
		}

		private void ReportProgressChanged(double progress0To1, string processingState)
		{
			ReportProgressChanged(progress0To1, processingState, out var _);
		}

		private void ReportProgressChanged(double progress0To1, string processingState, out bool continueProcessing)
		{
			if (!timeSinceReported.get_IsRunning() || timeSinceReported.get_ElapsedMilliseconds() > 100 || processingState != processingProgressControl.get_ProgressMessage())
			{
				UiThread.RunOnIdle((Action)delegate
				{
					processingProgressControl.set_RatioComplete(progress0To1);
					processingProgressControl.set_ProgressMessage(processingState);
				});
				timeSinceReported.Restart();
			}
			continueProcessing = true;
		}

		private void ClearBedAndLoadPrintItemWrapper(PrintItemWrapper printItemWrapper)
		{
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			SwitchStateToNotEditing();
			base.MeshGroups.Clear();
			MeshGroupExtraData.Clear();
			base.MeshGroupTransforms.Clear();
			if (printItemWrapper != null)
			{
				PrintItemWrapper.FileHasChanged.UnregisterEvent((EventHandler)ReloadMeshIfChangeExternaly, ref unregisterEvents);
				PrintItemWrapper.FileHasChanged.RegisterEvent((EventHandler)ReloadMeshIfChangeExternaly, ref unregisterEvents);
				meshViewerWidget.LoadDone += meshViewerWidget_LoadDone;
				Vector2 bedCenter = default(Vector2);
				MeshViewerWidget.CenterPartAfterLoad centerPart = MeshViewerWidget.CenterPartAfterLoad.DONT;
				if (ActiveSliceSettings.Instance?.GetValue<bool>("center_part_on_bed") ?? false)
				{
					centerPart = MeshViewerWidget.CenterPartAfterLoad.DO;
					bedCenter = ActiveSliceSettings.Instance.GetValue<Vector2>("print_center");
				}
				meshViewerWidget.LoadMesh(printItemWrapper.FileLocation, centerPart, bedCenter);
			}
			partHasBeenEdited = false;
		}

		private void CreateOptionsContent()
		{
			AddRotateControls(rotateOptionContainer);
		}

		private void CreateRenderTypeRadioButtons(FlowLayoutWidget viewOptionContainer)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Expected O, but got Unknown
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Invalid comparison between Unknown and I4
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Expected O, but got Unknown
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Invalid comparison between Unknown and I4
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Expected O, but got Unknown
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Invalid comparison between Unknown and I4
			string text = UserSettings.Instance.get("defaultRenderSetting");
			if (text == null)
			{
				text = ((!UserSettings.Instance.IsTouchScreen) ? "Outlines" : "Shaded");
				UserSettings.Instance.set("defaultRenderSetting", text);
			}
			if (Enum.TryParse<RenderTypes>(text, out RenderTypes result))
			{
				meshViewerWidget.RenderType = result;
			}
			RadioButton val = new RadioButton("Shaded".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12);
			val.set_Checked((int)meshViewerWidget.RenderType == 1);
			val.add_CheckedStateChanged((EventHandler)delegate
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				meshViewerWidget.RenderType = (RenderTypes)1;
				UserSettings instance3 = UserSettings.Instance;
				RenderTypes renderType3 = meshViewerWidget.RenderType;
				instance3.set("defaultRenderSetting", ((object)(RenderTypes)(ref renderType3)).ToString());
			});
			((GuiWidget)viewOptionContainer).AddChild((GuiWidget)(object)val, -1);
			RadioButton val2 = new RadioButton("Outlines".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12);
			val2.set_Checked((int)meshViewerWidget.RenderType == 2);
			val2.add_CheckedStateChanged((EventHandler)delegate
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				meshViewerWidget.RenderType = (RenderTypes)2;
				UserSettings instance2 = UserSettings.Instance;
				RenderTypes renderType2 = meshViewerWidget.RenderType;
				instance2.set("defaultRenderSetting", ((object)(RenderTypes)(ref renderType2)).ToString());
			});
			((GuiWidget)viewOptionContainer).AddChild((GuiWidget)(object)val2, -1);
			RadioButton val3 = new RadioButton("Polygons".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12);
			val3.set_Checked((int)meshViewerWidget.RenderType == 3);
			val3.add_CheckedStateChanged((EventHandler)delegate
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				meshViewerWidget.RenderType = (RenderTypes)3;
				UserSettings instance = UserSettings.Instance;
				RenderTypes renderType = meshViewerWidget.RenderType;
				instance.set("defaultRenderSetting", ((object)(RenderTypes)(ref renderType)).ToString());
			});
			((GuiWidget)viewOptionContainer).AddChild((GuiWidget)(object)val3, -1);
		}

		private FlowLayoutWidget CreateRightButtonPanel(double buildHeight)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Expected O, but got Unknown
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Expected O, but got Unknown
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Expected O, but got Unknown
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Expected O, but got Unknown
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Expected O, but got Unknown
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Expected O, but got Unknown
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Expected O, but got Unknown
			//IL_055d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Expected O, but got Unknown
			//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0608: Unknown result type (might be due to invalid IL or missing references)
			//IL_0616: Unknown result type (might be due to invalid IL or missing references)
			//IL_0620: Expected O, but got Unknown
			//IL_0689: Unknown result type (might be due to invalid IL or missing references)
			//IL_0697: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a1: Expected O, but got Unknown
			//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0726: Unknown result type (might be due to invalid IL or missing references)
			//IL_0765: Unknown result type (might be due to invalid IL or missing references)
			//IL_0770: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_Width(200.0);
			((GuiWidget)val).set_HAnchor((HAnchor)8);
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			((GuiWidget)val).set_Padding(new BorderDouble(6.0, 6.0));
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 1.0));
			((GuiWidget)val).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_VAnchor((VAnchor)12);
			((GuiWidget)val2).set_HAnchor((HAnchor)10);
			FlowLayoutWidget val3 = val2;
			double fixedWidth = WhiteButtonFactory.FixedWidth;
			WhiteButtonFactory.FixedWidth /= 2.0;
			Button undoButton = WhiteButtonFactory.Generate("Undo".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)undoButton).set_Name("3D View Undo");
			((GuiWidget)undoButton).set_Enabled(false);
			((GuiWidget)undoButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UndoBuffer.Undo(1);
			});
			((GuiWidget)val3).AddChild((GuiWidget)(object)undoButton, -1);
			Button redoButton = WhiteButtonFactory.Generate("Redo".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)redoButton).set_Name("3D View Redo");
			((GuiWidget)redoButton).set_Enabled(false);
			((GuiWidget)redoButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UndoBuffer.Redo(1);
			});
			((GuiWidget)val3).AddChild((GuiWidget)(object)redoButton, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			UndoBuffer.add_Changed((EventHandler)delegate
			{
				((GuiWidget)undoButton).set_Enabled(UndoBuffer.get_UndoCount() > 0);
				((GuiWidget)redoButton).set_Enabled(UndoBuffer.get_RedoCount() > 0);
			});
			WhiteButtonFactory.FixedWidth = fixedWidth;
			FlowLayoutWidget buttonRightPanel = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)buttonRightPanel).set_HAnchor((HAnchor)5);
			ScrollableWidget rightPanelScroll = new ScrollableWidget(false);
			((GuiWidget)rightPanelScroll).AnchorAll();
			ScrollableWidget obj = rightPanelScroll;
			((GuiWidget)obj).set_HAnchor((HAnchor)(((GuiWidget)obj).get_HAnchor() | 8));
			rightPanelScroll.set_AutoScroll(true);
			ScrollingArea scrollArea = rightPanelScroll.get_ScrollArea();
			((GuiWidget)scrollArea).set_HAnchor((HAnchor)(((GuiWidget)scrollArea).get_HAnchor() | 5));
			((GuiWidget)rightPanelScroll).add_BoundsChanged((EventHandler)delegate
			{
				((GuiWidget)buttonRightPanel).set_Width(((GuiWidget)rightPanelScroll.get_ScrollArea()).get_Width() - ((GuiWidget)rightPanelScroll.get_VerticalScrollBar()).get_Width());
			});
			((GuiWidget)rightPanelScroll.get_ScrollArea()).add_BoundsChanged((EventHandler)delegate
			{
				((GuiWidget)buttonRightPanel).set_Width(((GuiWidget)rightPanelScroll.get_ScrollArea()).get_Width() - ((GuiWidget)rightPanelScroll.get_VerticalScrollBar()).get_Width());
			});
			rightPanelScroll.add_ScrollPositionChanged((EventHandler)delegate
			{
				((GuiWidget)buttonRightPanel).set_Width(((GuiWidget)rightPanelScroll.get_ScrollArea()).get_Width() - ((GuiWidget)rightPanelScroll.get_VerticalScrollBar()).get_Width());
			});
			((GuiWidget)rightPanelScroll.get_ScrollArea()).add_MarginChanged((EventHandler)delegate
			{
				((GuiWidget)buttonRightPanel).set_Width(((GuiWidget)rightPanelScroll.get_ScrollArea()).get_Width() - ((GuiWidget)rightPanelScroll.get_VerticalScrollBar()).get_Width());
			});
			((GuiWidget)rightPanelScroll).AddChild((GuiWidget)(object)buttonRightPanel, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)rightPanelScroll, -1);
			int num = 16;
			((GuiWidget)buttonRightPanel).AddChild(new GuiWidget((double)num, (double)num, (SizeLimitsToSet)1), -1);
			new BorderDouble(0.0, 0.0, 0.0, 3.0);
			expandRotateOptions = ExpandMenuOptionFactory.GenerateCheckBoxButton("Rotate".Localize().ToUpper(), ArrowRight, ArrowDown);
			((GuiWidget)expandRotateOptions).set_Margin(new BorderDouble(0.0, 2.0, 0.0, 0.0));
			((GuiWidget)buttonRightPanel).AddChild((GuiWidget)(object)expandRotateOptions, -1);
			expandRotateOptions.add_CheckedStateChanged((EventHandler)expandRotateOptions_CheckedStateChanged);
			rotateOptionContainer = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)rotateOptionContainer).set_HAnchor((HAnchor)5);
			((GuiWidget)rotateOptionContainer).set_Visible(false);
			((GuiWidget)buttonRightPanel).AddChild((GuiWidget)(object)rotateOptionContainer, -1);
			((GuiWidget)buttonRightPanel).AddChild((GuiWidget)(object)new ScaleControls(this), -1);
			((GuiWidget)buttonRightPanel).AddChild((GuiWidget)(object)new MirrorControls(this), -1);
			foreach (SideBarPlugin plugin in new PluginFinder<SideBarPlugin>((string)null, (IComparer<SideBarPlugin>)null).Plugins)
			{
				((GuiWidget)buttonRightPanel).AddChild(plugin.CreateSideBarTool(this), -1);
			}
			ActiveSliceSettings.Instance.GetValue<int>("extruder_count");
			expandMaterialOptions = ExpandMenuOptionFactory.GenerateCheckBoxButton("Materials".Localize().ToUpper(), ArrowRight, ArrowDown);
			((GuiWidget)expandMaterialOptions).set_Margin(new BorderDouble(0.0, 2.0, 0.0, 0.0));
			expandMaterialOptions.add_CheckedStateChanged((EventHandler)expandMaterialOptions_CheckedStateChanged);
			((GuiWidget)buttonRightPanel).AddChild((GuiWidget)(object)expandMaterialOptions, -1);
			materialOptionContainer = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)materialOptionContainer).set_HAnchor((HAnchor)5);
			((GuiWidget)materialOptionContainer).set_Visible(false);
			((GuiWidget)buttonRightPanel).AddChild((GuiWidget)(object)materialOptionContainer, -1);
			AddMaterialControls(materialOptionContainer);
			expandViewOptions = ExpandMenuOptionFactory.GenerateCheckBoxButton("Display".Localize().ToUpper(), ArrowRight, ArrowDown);
			((GuiWidget)expandViewOptions).set_Margin(new BorderDouble(0.0, 2.0, 0.0, 0.0));
			((GuiWidget)buttonRightPanel).AddChild((GuiWidget)(object)expandViewOptions, -1);
			expandViewOptions.add_CheckedStateChanged((EventHandler)expandViewOptions_CheckedStateChanged);
			viewOptionContainer = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)viewOptionContainer).set_HAnchor((HAnchor)5);
			((GuiWidget)viewOptionContainer).set_Padding(new BorderDouble(4.0, 0.0, 0.0, 0.0));
			((GuiWidget)viewOptionContainer).set_Visible(false);
			CheckBox showBedCheckBox = new CheckBox("Show Print Bed".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12.0);
			showBedCheckBox.set_Checked(true);
			showBedCheckBox.add_CheckedStateChanged((EventHandler)delegate
			{
				meshViewerWidget.RenderBed = showBedCheckBox.get_Checked();
			});
			((GuiWidget)viewOptionContainer).AddChild((GuiWidget)(object)showBedCheckBox, -1);
			if (buildHeight > 0.0)
			{
				CheckBox showBuildVolumeCheckBox = new CheckBox("Show Print Area".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12.0);
				showBuildVolumeCheckBox.set_Checked(false);
				((GuiWidget)showBuildVolumeCheckBox).set_Margin(new BorderDouble(0.0, 5.0, 0.0, 0.0));
				showBuildVolumeCheckBox.add_CheckedStateChanged((EventHandler)delegate
				{
					meshViewerWidget.RenderBuildVolume = showBuildVolumeCheckBox.get_Checked();
				});
				((GuiWidget)viewOptionContainer).AddChild((GuiWidget)(object)showBuildVolumeCheckBox, -1);
			}
			if (UserSettings.Instance.IsTouchScreen)
			{
				UserSettings instance = UserSettings.Instance;
				RenderTypes val4 = (RenderTypes)1;
				instance.set("defaultRenderSetting", ((object)(RenderTypes)(ref val4)).ToString());
			}
			else
			{
				CreateRenderTypeRadioButtons(viewOptionContainer);
			}
			((GuiWidget)buttonRightPanel).AddChild((GuiWidget)(object)viewOptionContainer, -1);
			((GuiWidget)buttonRightPanel).AddChild(new GuiWidget((double)num, (double)num, (SizeLimitsToSet)1), -1);
			AddGridSnapSettings((GuiWidget)(object)val);
			return val;
		}

		private void DeleteSelectedMesh()
		{
			if (!base.HaveSelection || base.MeshGroups.Count - base.SelectedMeshGroupIndices.Count <= 0)
			{
				return;
			}
			List<int> list = new List<int>(base.SelectedMeshGroupIndices);
			list.Sort();
			list.Reverse();
			UndoBuffer.Add((IUndoRedoCommand)(object)new DeleteUndoCommand(this, list));
			foreach (int item in list)
			{
				base.MeshGroups.RemoveAt(item);
				MeshGroupExtraData.RemoveAt(item);
				base.MeshGroupTransforms.RemoveAt(item);
			}
			base.SelectedMeshGroupIndices.Clear();
			PartHasBeenChanged();
		}

		private void ExitEditingAndSaveIfRequired(bool response)
		{
			if (response)
			{
				MergeAndSavePartsToCurrentMeshFile(SwitchStateToNotEditing);
				return;
			}
			SwitchStateToNotEditing();
			ClearBedAndLoadPrintItemWrapper(printItemWrapper);
		}

		private void expandMaterialOptions_CheckedStateChanged(object sender, EventArgs e)
		{
			if (((GuiWidget)materialOptionContainer).get_Visible() != expandMaterialOptions.get_Checked())
			{
				((GuiWidget)materialOptionContainer).set_Visible(expandMaterialOptions.get_Checked());
			}
		}

		private void expandRotateOptions_CheckedStateChanged(object sender, EventArgs e)
		{
			if (((GuiWidget)rotateOptionContainer).get_Visible() != expandRotateOptions.get_Checked())
			{
				((GuiWidget)rotateOptionContainer).set_Visible(expandRotateOptions.get_Checked());
			}
		}

		private void expandViewOptions_CheckedStateChanged(object sender, EventArgs e)
		{
			if (((GuiWidget)viewOptionContainer).get_Visible() != expandViewOptions.get_Checked())
			{
				((GuiWidget)viewOptionContainer).set_Visible(expandViewOptions.get_Checked());
			}
		}

		private bool FindMeshGroupHitPosition(Vector2 screenPosition, out int meshHitIndex, ref IntersectInfo info)
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Expected O, but got Unknown
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			meshHitIndex = 0;
			if (MeshGroupExtraData.Count == 0 || MeshGroupExtraData[0].meshTraceableData == null)
			{
				return false;
			}
			List<IPrimitive> list = new List<IPrimitive>();
			for (int i = 0; i < MeshGroupExtraData.Count; i++)
			{
				foreach (IPrimitive meshTraceableDatum in MeshGroupExtraData[i].meshTraceableData)
				{
					list.Add((IPrimitive)new Transform(meshTraceableDatum, base.MeshGroupTransforms[i]));
				}
			}
			allObjects = BoundingVolumeHierarchy.CreateNewHierachy(list, 0, 0, (SortingAccelerator)null);
			Vector2 screenPosition2 = ((GuiWidget)meshViewerWidget).TransformFromParentSpace((GuiWidget)(object)this, screenPosition);
			Ray rayFromScreen = meshViewerWidget.TrackballTumbleWidget.GetRayFromScreen(screenPosition2);
			info = allObjects.GetClosestIntersection(rayFromScreen);
			if (info != null)
			{
				CurrentSelectInfo.PlaneDownHitPos = info.hitPosition;
				CurrentSelectInfo.LastMoveDelta = default(Vector3);
				for (int j = 0; j < MeshGroupExtraData.Count; j++)
				{
					List<IPrimitive> list2 = new List<IPrimitive>();
					foreach (IPrimitive meshTraceableDatum2 in MeshGroupExtraData[j].meshTraceableData)
					{
						meshTraceableDatum2.GetContained(list2, info.closestHitObject.GetAxisAlignedBoundingBox());
					}
					if (list2.Contains(info.closestHitObject))
					{
						meshHitIndex = j;
						return true;
					}
				}
			}
			return false;
		}

		public GuiWidget GenerateHorizontalRule()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Expected O, but got Unknown
			GuiWidget val = new GuiWidget();
			val.set_Height(1.0);
			val.set_Margin(new BorderDouble(0.0, 1.0, 0.0, 3.0));
			val.set_HAnchor((HAnchor)5);
			val.set_BackgroundColor(new RGBA_Bytes(255, 255, 255, 200));
			return val;
		}

		private async void LoadAndAddPartsToPlate(string[] filesToLoad)
		{
			if (base.MeshGroups.Count <= 0 || filesToLoad == null || filesToLoad.Length == 0)
			{
				return;
			}
			string text = "Loading Parts".Localize();
			string processType = StringHelper.FormatWith("{0}:", new object[1]
			{
				text
			});
			processingProgressControl.set_ProcessType(processType);
			((GuiWidget)processingProgressControl).set_Visible(true);
			processingProgressControl.set_PercentComplete(0);
			LockEditControls();
			PushMeshGroupDataToAsynchLists(TraceInfoOpperation.DO_COPY);
			await Task.Run(delegate
			{
				loadAndAddPartsToPlate(filesToLoad);
			});
			if (((GuiWidget)this).get_HasBeenClosed())
			{
				return;
			}
			UnlockEditControls();
			PartHasBeenChanged();
			bool flag = asyncMeshGroups.Count == base.MeshGroups.Count + 1;
			if (base.MeshGroups.Count > 0)
			{
				PullMeshGroupDataFromAsynchLists();
				if (flag)
				{
					SelectedMeshGroupIndex = asyncMeshGroups.Count - 1;
				}
			}
		}

		private void loadAndAddPartsToPlate(string[] filesToLoadIncludingZips)
		{
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Expected O, but got Unknown
			//IL_013d: Expected O, but got Unknown
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Expected O, but got Unknown
			//IL_0202: Expected O, but got Unknown
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			List<string> list = new List<string>();
			if (filesToLoadIncludingZips == null || filesToLoadIncludingZips.Length == 0)
			{
				return;
			}
			foreach (string text in filesToLoadIncludingZips)
			{
				string text2 = Path.GetExtension(text)!.ToUpper();
				if (text2 != "" && MeshFileIo.ValidFileExtensions().Contains(text2))
				{
					list.Add(text);
				}
				else
				{
					if (!(text2 == ".ZIP"))
					{
						continue;
					}
					List<PrintItem> list2 = new ProjectFileHandler(null).ImportFromProjectArchive(text);
					if (list2 == null)
					{
						continue;
					}
					foreach (PrintItem item in list2)
					{
						list.Add(item.FileLocation);
					}
				}
			}
			string progressMessage = "Loading Parts...".Localize();
			double ratioPerFile = 1.0 / (double)list.Count;
			double currentRatioDone = 0.0;
			ReportProgressRatio val = default(ReportProgressRatio);
			for (int j = 0; j < list.Count; j++)
			{
				string fullPath = Path.GetFullPath(list[j]);
				ReportProgressRatio obj = val;
				if (obj == null)
				{
					ReportProgressRatio val2 = delegate(double progress0To1, string processingState, out bool continueProcessing)
					{
						continueProcessing = !((GuiWidget)this).get_HasBeenClosed();
						double num2 = ratioPerFile * 0.5;
						double progress0To3 = currentRatioDone + progress0To1 * num2;
						ReportProgressChanged(progress0To3, progressMessage, out continueProcessing);
					};
					ReportProgressRatio val3 = val2;
					val = val2;
					obj = val3;
				}
				List<MeshGroup> list3 = MeshFileIo.Load(fullPath, obj);
				if (((GuiWidget)this).get_HasBeenClosed())
				{
					break;
				}
				if (list3 != null)
				{
					double ratioPerSubMesh = ratioPerFile / (double)list3.Count;
					double subMeshRatioDone = 0.0;
					ReportProgressRatio val4 = default(ReportProgressRatio);
					for (int k = 0; k < list3.Count; k++)
					{
						PlatingHelper.FindPositionForGroupAndAddToPlate(list3[k], Matrix4X4.Identity, asyncPlatingDatas, asyncMeshGroups, asyncMeshGroupTransforms);
						if (((GuiWidget)this).get_HasBeenClosed())
						{
							return;
						}
						List<PlatingMeshGroupData> perMeshGroupInfo = asyncPlatingDatas;
						List<MeshGroup> meshGroups = asyncMeshGroups;
						int meshGroupIndex = asyncMeshGroups.Count - 1;
						ReportProgressRatio obj2 = val4;
						if (obj2 == null)
						{
							ReportProgressRatio val5 = delegate(double progress0To1, string processingState, out bool continueProcessing)
							{
								continueProcessing = !((GuiWidget)this).get_HasBeenClosed();
								double num = ratioPerFile * 0.5;
								double progress0To2 = currentRatioDone + subMeshRatioDone + num + progress0To1 * ratioPerSubMesh;
								ReportProgressChanged(progress0To2, progressMessage, out continueProcessing);
							};
							ReportProgressRatio val3 = val5;
							val4 = val5;
							obj2 = val3;
						}
						PlatingHelper.CreateITraceableForMeshGroup(perMeshGroupInfo, meshGroups, meshGroupIndex, obj2);
						subMeshRatioDone += ratioPerSubMesh;
					}
				}
				currentRatioDone += ratioPerFile;
			}
		}

		private void LockEditControls()
		{
			viewIsInEditModePreLock = ((GuiWidget)doEdittingButtonsContainer).get_Visible();
			((GuiWidget)enterEditButtonsContainer).set_Visible(false);
			((GuiWidget)doEdittingButtonsContainer).set_Visible(false);
			buttonRightPanelDisabledCover.set_Visible(true);
			if (viewControls3D.PartSelectVisible)
			{
				viewControls3D.PartSelectVisible = false;
				if (viewControls3D.ActiveButton == ViewControls3DButtons.PartSelect)
				{
					wasInSelectMode = true;
					viewControls3D.ActiveButton = ViewControls3DButtons.Rotate;
				}
			}
		}

		private void MakeLowestFaceFlat(int indexToLayFlat)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			Vertex val = base.MeshGroups[indexToLayFlat].get_Meshes()[0].get_Vertices().get_Item(0);
			Vector3 val2 = Vector3.Transform(val.get_Position(), base.MeshGroupTransforms[indexToLayFlat]);
			foreach (Mesh mesh in base.MeshGroups[indexToLayFlat].get_Meshes())
			{
				for (int i = 1; i < mesh.get_Vertices().get_Count(); i++)
				{
					Vector3 val3 = Vector3.Transform(mesh.get_Vertices().get_Item(i).get_Position(), base.MeshGroupTransforms[indexToLayFlat]);
					if (val3.z < val2.z)
					{
						val = mesh.get_Vertices().get_Item(i);
						val2 = val3;
					}
				}
			}
			Face val4 = null;
			double num = double.MaxValue;
			foreach (Face item in val.ConnectedFaces())
			{
				double num2 = double.MinValue;
				foreach (Vertex item2 in item.Vertices())
				{
					if (item2 != val)
					{
						Vector3 val5 = Vector3.Transform(item2.get_Position(), base.MeshGroupTransforms[indexToLayFlat]) - val2;
						Vector2 val6 = new Vector2(val5.x, val5.y);
						double length = ((Vector2)(ref val6)).get_Length();
						double num3 = Math.Atan2(val5.z, length);
						if (num3 > num2)
						{
							num2 = num3;
						}
					}
				}
				if (num2 < num)
				{
					num = num2;
					val4 = item;
				}
			}
			double num4 = 0.0;
			List<Vector3> list = new List<Vector3>();
			foreach (Vertex item3 in val4.Vertices())
			{
				Vector3 val7 = Vector3.Transform(item3.get_Position(), base.MeshGroupTransforms[indexToLayFlat]);
				list.Add(val7);
				num4 = Math.Max(num4, val7.z - val2.z);
			}
			if (num4 > 0.001)
			{
				Vector3 val8 = list[1] - list[0];
				Vector3 normal = ((Vector3)(ref val8)).GetNormal();
				val8 = list[2] - list[0];
				Vector3 normal2 = ((Vector3)(ref val8)).GetNormal();
				val8 = Vector3.Cross(normal, normal2);
				Vector3 normal3 = ((Vector3)(ref val8)).GetNormal();
				Matrix4X4 transformToApply = Matrix4X4.CreateRotation(new Quaternion(normal3, new Vector3(0.0, 0.0, -1.0)));
				base.MeshGroupTransforms[indexToLayFlat] = PlatingHelper.ApplyAtCenter((IHasAABB)(object)base.MeshGroups[indexToLayFlat], base.MeshGroupTransforms[indexToLayFlat], transformToApply);
				PartHasBeenChanged();
				((GuiWidget)this).Invalidate();
			}
		}

		private void MergeAndSavePartsDoWork(SaveAsWindow.SaveAsReturnInfo returnInfo)
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Expected O, but got Unknown
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Expected O, but got Unknown
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Expected O, but got Unknown
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Expected O, but got Unknown
			if (returnInfo != null)
			{
				PrintItem printItem = new PrintItem();
				printItem.Name = returnInfo.newName;
				printItem.FileLocation = Path.GetFullPath(returnInfo.fileNameAndPath);
				printItemWrapper = new PrintItemWrapper(printItem, returnInfo.destinationLibraryProvider.GetProviderLocator());
			}
			PushMeshGroupDataToAsynchLists(TraceInfoOpperation.DO_COPY);
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			try
			{
				for (int i = 0; i < asyncMeshGroups.Count; i++)
				{
					asyncMeshGroups[i].Transform(asyncMeshGroupTransforms[i]);
					ReportProgressChanged((double)(i + 1) * 0.4 / (double)asyncMeshGroups.Count, "", out var _);
				}
				string[] array = new string[4]
				{
					"Created By",
					"Element",
					"BedPosition",
					"Absolute"
				};
				MeshOutputSettings val = new MeshOutputSettings((OutputType)1, array, (ReportProgressRatio)null);
				if (returnInfo == null)
				{
					FileInfo val2 = new FileInfo(printItemWrapper.FileLocation);
					bool flag = !((FileSystemInfo)val2).get_Extension().Equals(".amf", StringComparison.OrdinalIgnoreCase);
					if (flag && !printItemWrapper.UseIncrementedNameDuringTypeChange)
					{
						printItemWrapper.FileLocation = Path.ChangeExtension(printItemWrapper.FileLocation, ".amf");
					}
					else if (flag)
					{
						string text = Path.GetFileNameWithoutExtension(((FileSystemInfo)val2).get_Name());
						if (text.Contains("("))
						{
							text = fileNameNumberMatch.Replace(text, "").Trim();
						}
						int num = 0;
						string str;
						string fileLocation;
						do
						{
							str = $"{text} ({++num})";
							fileLocation = Path.Combine(val2.get_DirectoryName(), str + ".amf");
						}
						while (Enumerable.Any<string>((IEnumerable<string>)Directory.GetFiles(val2.get_DirectoryName(), str + ".*")));
						printItemWrapper.FileLocation = fileLocation;
					}
					try
					{
						string tempFileName = ApplicationDataStorage.Instance.GetTempFileName("amf");
						if (MeshFileIo.Save(asyncMeshGroups, tempFileName, val, new ReportProgressRatio(ReportProgressChanged)) && File.Exists(tempFileName))
						{
							if (File.Exists(printItemWrapper.FileLocation))
							{
								File.Delete(printItemWrapper.FileLocation);
							}
							File.Move(tempFileName, printItemWrapper.FileLocation);
							printItemWrapper.PrintItem.Commit();
						}
					}
					catch (Exception ex)
					{
						Trace.WriteLine("Error saving file: ", ex.Message);
					}
				}
				else
				{
					MeshFileIo.Save(asyncMeshGroups, printItemWrapper.FileLocation, val, new ReportProgressRatio(ReportProgressChanged));
				}
				UiThread.RunOnIdle((Action)printItemWrapper.ReportFileChange, 3.0);
				if (returnInfo != null && returnInfo.destinationLibraryProvider != null)
				{
					LibraryProvider destinationLibraryProvider = returnInfo.destinationLibraryProvider;
					if (destinationLibraryProvider != null)
					{
						destinationLibraryProvider.AddItem(printItemWrapper);
						destinationLibraryProvider.Dispose();
					}
				}
				saveSucceded = true;
			}
			catch (UnauthorizedAccessException)
			{
				saveSucceded = false;
				UiThread.RunOnIdle((Action)delegate
				{
					StyledMessageBox.ShowMessageBox(null, "Oops! Unable to save changes.", "Unable to save");
				});
			}
			catch (Exception)
			{
				saveSucceded = false;
				UiThread.RunOnIdle((Action)delegate
				{
					StyledMessageBox.ShowMessageBox(null, "Oops! Unable to save changes.", "Unable to save");
				});
			}
		}

		private void MergeAndSavePartsDoCompleted()
		{
			if (!((GuiWidget)this).get_HasBeenClosed())
			{
				UnlockEditControls();
				if (saveSucceded)
				{
					((GuiWidget)saveButtons).set_Visible(false);
				}
				if (afterSaveCallback != null)
				{
					afterSaveCallback();
				}
			}
		}

		public void SaveYourselfNow(Action eventToCallAfterSave = null)
		{
			MergeAndSavePartsToCurrentMeshFile(eventToCallAfterSave);
		}

		private async void MergeAndSavePartsToCurrentMeshFile(Action eventToCallAfterSave = null)
		{
			editorThatRequestedSave = true;
			afterSaveCallback = eventToCallAfterSave;
			if (base.MeshGroups.Count > 0)
			{
				string text = "Saving".Localize();
				string processType = StringHelper.FormatWith("{0}:", new object[1]
				{
					text
				});
				processingProgressControl.set_ProcessType(processType);
				((GuiWidget)processingProgressControl).set_Visible(true);
				processingProgressControl.set_PercentComplete(0);
				LockEditControls();
				await Task.Run(delegate
				{
					MergeAndSavePartsDoWork(null);
				});
				MergeAndSavePartsDoCompleted();
			}
		}

		private async void MergeAndSavePartsToNewMeshFile(SaveAsWindow.SaveAsReturnInfo returnInfo)
		{
			editorThatRequestedSave = true;
			if (base.MeshGroups.Count > 0)
			{
				string text = "Saving".Localize();
				string processType = StringHelper.FormatWith("{0}:", new object[1]
				{
					text
				});
				processingProgressControl.set_ProcessType(processType);
				((GuiWidget)processingProgressControl).set_Visible(true);
				processingProgressControl.set_PercentComplete(0);
				LockEditControls();
				await Task.Run(delegate
				{
					MergeAndSavePartsDoWork(returnInfo);
				});
				MergeAndSavePartsDoCompleted();
			}
		}

		private void meshViewerWidget_LoadDone(object sender, EventArgs e)
		{
			if (windowType == WindowMode.Embeded)
			{
				PrinterConnectionAndCommunication.CommunicationStates communicationState = PrinterConnectionAndCommunication.Instance.CommunicationState;
				if (communicationState != PrinterConnectionAndCommunication.CommunicationStates.Printing && communicationState != PrinterConnectionAndCommunication.CommunicationStates.Paused)
				{
					UnlockEditControls();
				}
			}
			else
			{
				UnlockEditControls();
			}
			OnSelectionChanged(null);
			if (openMode == OpenMode.Editing)
			{
				UiThread.RunOnIdle((Action)EnterEditAndCreateSelectionData);
			}
			meshViewerWidget.ResetView();
		}

		private bool PartsAreInPrintVolume()
		{
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			PrinterSettings instance = ActiveSliceSettings.Instance;
			if (instance != null && !instance.GetValue<bool>("center_part_on_bed"))
			{
				AxisAlignedBoundingBox axisAlignedBoundingBox = MeshViewerWidget.GetAxisAlignedBoundingBox(base.MeshGroups);
				bool num = axisAlignedBoundingBox.minXYZ.z > -0.001 && axisAlignedBoundingBox.minXYZ.z < 0.001;
				RectangleDouble val = default(RectangleDouble);
				((RectangleDouble)(ref val))._002Ector(0.0, 0.0, ActiveSliceSettings.Instance.GetValue<Vector2>("bed_size").x, ActiveSliceSettings.Instance.GetValue<Vector2>("bed_size").y);
				((RectangleDouble)(ref val)).Offset(ActiveSliceSettings.Instance.GetValue<Vector2>("print_center") - ActiveSliceSettings.Instance.GetValue<Vector2>("bed_size") / 2.0);
				bool flag = ((RectangleDouble)(ref val)).Contains(new Vector2(axisAlignedBoundingBox.minXYZ)) && ((RectangleDouble)(ref val)).Contains(new Vector2(axisAlignedBoundingBox.maxXYZ));
				return num && flag;
			}
			return true;
		}

		private void OpenExportWindow()
		{
			if (exportingWindow == null)
			{
				exportingWindow = new ExportPrintItemWindow(printItemWrapper);
				((GuiWidget)exportingWindow).add_Closed((EventHandler<ClosedEventArgs>)delegate
				{
					exportingWindow = null;
				});
				((SystemWindow)exportingWindow).ShowAsSystemWindow();
			}
			else
			{
				((GuiWidget)exportingWindow).BringToFront();
			}
		}

		private void OpenSaveAsWindow()
		{
			if (saveAsWindow == null)
			{
				List<ProviderLocatorNode> providerLocator = null;
				if (printItemWrapper.SourceLibraryProviderLocator != null)
				{
					providerLocator = printItemWrapper.SourceLibraryProviderLocator;
				}
				saveAsWindow = new SaveAsWindow(MergeAndSavePartsToNewMeshFile, providerLocator, showQueue: true, getNewName: true);
				((GuiWidget)saveAsWindow).add_Closed((EventHandler<ClosedEventArgs>)SaveAsWindow_Closed);
			}
			else
			{
				((GuiWidget)saveAsWindow).BringToFront();
			}
		}

		private void PullMeshGroupDataFromAsynchLists()
		{
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			if (base.MeshGroups.Count != asyncMeshGroups.Count)
			{
				PartHasBeenChanged();
			}
			base.MeshGroups.Clear();
			foreach (MeshGroup asyncMeshGroup in asyncMeshGroups)
			{
				base.MeshGroups.Add(asyncMeshGroup);
			}
			base.MeshGroupTransforms.Clear();
			foreach (Matrix4X4 asyncMeshGroupTransform in asyncMeshGroupTransforms)
			{
				base.MeshGroupTransforms.Add(asyncMeshGroupTransform);
			}
			MeshGroupExtraData.Clear();
			foreach (PlatingMeshGroupData asyncPlatingData in asyncPlatingDatas)
			{
				MeshGroupExtraData.Add(asyncPlatingData);
			}
			if (base.MeshGroups.Count != base.MeshGroupTransforms.Count || base.MeshGroups.Count != MeshGroupExtraData.Count)
			{
				throw new Exception("These all need to remain in sync.");
			}
		}

		private void PushMeshGroupDataToAsynchLists(TraceInfoOpperation traceInfoOpperation, ReportProgressRatio reportProgress = null)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			UiThread.RunOnIdle((Action)delegate
			{
				processingProgressControl.set_ProgressMessage("Async Copy");
			});
			asyncMeshGroups.Clear();
			asyncMeshGroupTransforms.Clear();
			for (int i = 0; i < base.MeshGroups.Count; i++)
			{
				MeshGroup val = base.MeshGroups[i];
				MeshGroup val2 = new MeshGroup();
				for (int j = 0; j < val.get_Meshes().Count; j++)
				{
					Mesh val3 = val.get_Meshes()[j];
					val2.get_Meshes().Add(Mesh.Copy(val3, (ReportProgressRatio)null, true));
				}
				asyncMeshGroups.Add(val2);
				asyncMeshGroupTransforms.Add(base.MeshGroupTransforms[i]);
			}
			asyncPlatingDatas.Clear();
			for (int k = 0; k < MeshGroupExtraData.Count; k++)
			{
				PlatingMeshGroupData platingMeshGroupData = new PlatingMeshGroupData();
				_ = base.MeshGroups[k];
				if (traceInfoOpperation == TraceInfoOpperation.DO_COPY)
				{
					platingMeshGroupData.meshTraceableData.AddRange(MeshGroupExtraData[k].meshTraceableData);
				}
				asyncPlatingDatas.Add(platingMeshGroupData);
			}
			UiThread.RunOnIdle((Action)delegate
			{
				processingProgressControl.set_ProgressMessage("");
			});
		}

		private void ReloadMeshIfChangeExternaly(object sender, EventArgs e)
		{
			PrintItemWrapper printItemWrapper = sender as PrintItemWrapper;
			if (printItemWrapper != null && printItemWrapper.FileLocation == this.printItemWrapper.FileLocation)
			{
				if (!editorThatRequestedSave)
				{
					ClearBedAndLoadPrintItemWrapper(this.printItemWrapper);
				}
				editorThatRequestedSave = false;
			}
		}

		private bool rotateQueueMenu_Click()
		{
			return true;
		}

		private void SaveAsWindow_Closed(object sender, ClosedEventArgs e)
		{
			saveAsWindow = null;
		}

		private bool scaleQueueMenu_Click()
		{
			return true;
		}

		private void SetEditControlsBasedOnPrinterState(object sender, EventArgs e)
		{
			if (windowType == WindowMode.Embeded)
			{
				PrinterConnectionAndCommunication.CommunicationStates communicationState = PrinterConnectionAndCommunication.Instance.CommunicationState;
				if (communicationState == PrinterConnectionAndCommunication.CommunicationStates.Printing || communicationState == PrinterConnectionAndCommunication.CommunicationStates.Paused)
				{
					LockEditControls();
				}
				else
				{
					UnlockEditControls();
				}
			}
		}

		private void SwitchStateToNotEditing()
		{
			if (!((GuiWidget)enterEditButtonsContainer).get_Visible())
			{
				((GuiWidget)enterEditButtonsContainer).set_Visible(true);
				((GuiWidget)processingProgressControl).set_Visible(false);
				((GuiWidget)buttonRightPanel).set_Visible(false);
				((GuiWidget)doEdittingButtonsContainer).set_Visible(false);
				viewControls3D.PartSelectVisible = false;
				if (viewControls3D.ActiveButton == ViewControls3DButtons.PartSelect)
				{
					viewControls3D.ActiveButton = ViewControls3DButtons.Rotate;
				}
				base.SelectedMeshGroupIndices.Clear();
			}
		}

		private void UnlockEditControls()
		{
			buttonRightPanelDisabledCover.set_Visible(false);
			((GuiWidget)processingProgressControl).set_Visible(false);
			if (viewIsInEditModePreLock)
			{
				if (!((GuiWidget)enterEditButtonsContainer).get_Visible())
				{
					viewControls3D.PartSelectVisible = true;
					((GuiWidget)doEdittingButtonsContainer).set_Visible(true);
				}
			}
			else
			{
				((GuiWidget)enterEditButtonsContainer).set_Visible(true);
			}
			if (wasInSelectMode)
			{
				viewControls3D.ActiveButton = ViewControls3DButtons.PartSelect;
				wasInSelectMode = false;
			}
			this.SelectedTransformChanged?.Invoke(this, null);
		}
	}
}
