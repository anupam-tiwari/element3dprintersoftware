using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Transform;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.RenderOpenGl;
using MatterHackers.RenderOpenGl.OpenGl;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class ViewGcodeWidget : GuiWidget
	{
		public enum ETransformState
		{
			Move,
			Scale
		}

		public ProgressChangedEventHandler LoadingProgressChanged;

		public double FeatureToStartOnRatio0To1;

		public double FeatureToEndOnRatio0To1 = 1.0;

		private BackgroundWorker backgroundWorker;

		private Vector2 lastMousePosition = new Vector2(0.0, 0.0);

		private Vector2 mouseDownPosition = new Vector2(0.0, 0.0);

		private double layerScale = 1.0;

		private int activeLayerIndex;

		private Vector2 gridSizeMm;

		private Vector2 gridCenterMm;

		private bool mouseWasDragged;

		private Vector2 unscaledRenderOffset = new Vector2(0.0, 0.0);

		public string FileNameAndPath;

		public GCodeRenderer gCodeRenderer;

		private bool selectRange;

		private PathStorage grid = new PathStorage();

		private static RGBA_Bytes gridColor = new RGBA_Bytes(190, 190, 190, 255);

		private double startDistanceBetweenPoints = 1.0;

		private double pinchStartScale = 1.0;

		public bool RenderGrid
		{
			get
			{
				string text = UserSettings.Instance.get("GcodeViewerRenderGrid");
				if (text == null)
				{
					RenderGrid = true;
					return true;
				}
				return text == "True";
			}
			set
			{
				UserSettings.Instance.set("GcodeViewerRenderGrid", value.ToString());
				((GuiWidget)this).Invalidate();
			}
		}

		public ETransformState TransformState
		{
			get;
			set;
		}

		public bool RenderMoves
		{
			get
			{
				if (UserSettings.Instance.get("GcodeViewerRenderMoves") == null)
				{
					UserSettings.Instance.set("GcodeViewerRenderMoves", "True");
				}
				return UserSettings.Instance.get("GcodeViewerRenderMoves") == "True";
			}
			set
			{
				UserSettings.Instance.set("GcodeViewerRenderMoves", value.ToString());
				((GuiWidget)this).Invalidate();
			}
		}

		public bool RenderRetractions
		{
			get
			{
				return UserSettings.Instance.get("GcodeViewerRenderRetractions") == "True";
			}
			set
			{
				UserSettings.Instance.set("GcodeViewerRenderRetractions", value.ToString());
				((GuiWidget)this).Invalidate();
			}
		}

		public bool RenderSpeeds
		{
			get
			{
				return UserSettings.Instance.get("GcodeViewerRenderSpeeds") == "True";
			}
			set
			{
				UserSettings.Instance.set("GcodeViewerRenderSpeeds", value.ToString());
				((GuiWidget)this).Invalidate();
			}
		}

		public bool SimulateExtrusion
		{
			get
			{
				return UserSettings.Instance.get("GcodeViewerSimulateExtrusion") == "True";
			}
			set
			{
				UserSettings.Instance.set("GcodeViewerSimulateExtrusion", value.ToString());
				((GuiWidget)this).Invalidate();
			}
		}

		public bool TransparentExtrusion
		{
			get
			{
				return UserSettings.Instance.get("GcodeViewerTransparentExtrusion") == "True";
			}
			set
			{
				UserSettings.Instance.set("GcodeViewerTransparentExtrusion", value.ToString());
				((GuiWidget)this).Invalidate();
			}
		}

		public bool HideExtruderOffsets
		{
			get
			{
				string text = UserSettings.Instance.get("GcodeViewerHideExtruderOffsets");
				if (text == null)
				{
					return true;
				}
				return text == "True";
			}
			set
			{
				UserSettings.Instance.set("GcodeViewerHideExtruderOffsets", value.ToString());
				((GuiWidget)this).Invalidate();
			}
		}

		public GCodeSelectionInfo GCodeSelectionInfo
		{
			get;
			private set;
		} = new GCodeSelectionInfo();


		public HashSet<int> SelectedFeatureInstructionIndices => GCodeSelectionInfo.SelectedFeatureInstructionIndices;

		public int StartLineIndex
		{
			get
			{
				return GCodeSelectionInfo.StartLineIndex;
			}
			set
			{
				GCodeSelectionInfo.StartLineIndex = value;
			}
		}

		public int EndLineIndex
		{
			get
			{
				return GCodeSelectionInfo.EndLineIndex;
			}
			set
			{
				GCodeSelectionInfo.EndLineIndex = value;
			}
		}

		public Vector2 StartBedPosition
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return GCodeSelectionInfo.StartBedPosition;
			}
			set
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				GCodeSelectionInfo.StartBedPosition = value;
			}
		}

		public Vector2 EndBedPosition
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return GCodeSelectionInfo.EndBedPosition;
			}
			set
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				GCodeSelectionInfo.EndBedPosition = value;
			}
		}

		private Affine ScalingTransform => Affine.NewScaling(layerScale, layerScale);

		public Affine TotalTransform => Affine.NewIdentity() * Affine.NewTranslation(unscaledRenderOffset) * ScalingTransform * Affine.NewTranslation(((GuiWidget)this).get_Width() / 2.0, ((GuiWidget)this).get_Height() / 2.0);

		public GCodeFile LoadedGCode
		{
			get;
			set;
		}

		public int ActiveLayerIndex
		{
			get
			{
				return activeLayerIndex;
			}
			set
			{
				if (activeLayerIndex != value)
				{
					activeLayerIndex = value;
					if (gCodeRenderer == null || activeLayerIndex < 0)
					{
						activeLayerIndex = 0;
					}
					else if (activeLayerIndex >= LoadedGCode.NumChangesInZ)
					{
						activeLayerIndex = LoadedGCode.NumChangesInZ - 1;
					}
					GCodeSelectionInfo.ClearSelection();
					((GuiWidget)this).Invalidate();
					this.ActiveLayerChanged?.Invoke(this, null);
				}
			}
		}

		public override RectangleDouble LocalBounds
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return ((GuiWidget)this).get_LocalBounds();
			}
			set
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				double width = ((GuiWidget)this).get_Width();
				((GuiWidget)this).get_Height();
				((GuiWidget)this).set_LocalBounds(value);
				if (width > 0.0)
				{
					layerScale *= ((GuiWidget)this).get_Width() / width;
				}
				else if (gCodeRenderer != null)
				{
					CenterPartInView();
				}
			}
		}

		public event EventHandler DoneLoading;

		public event EventHandler ActiveLayerChanged;

		public ViewGcodeWidget(Vector2 gridSizeMm, Vector2 gridCenterMm)
			: this()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Expected O, but got Unknown
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			this.gridSizeMm = gridSizeMm;
			this.gridCenterMm = gridCenterMm;
			((GuiWidget)this).set_LocalBounds(new RectangleDouble(0.0, 0.0, 100.0, 100.0));
			((GuiWidget)this).AnchorAll();
		}

		public void SetGCodeAfterLoad(GCodeFile loadedGCode)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			LoadedGCode = loadedGCode;
			if (loadedGCode == null)
			{
				TextWidget val = new TextWidget($"Not a valid GCode file.", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 0.0));
				((GuiWidget)val).set_VAnchor((VAnchor)2);
				((GuiWidget)val).set_HAnchor((HAnchor)2);
				((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			}
			else
			{
				SetInitalLayer();
				CenterPartInView();
			}
		}

		private void SetInitalLayer()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			activeLayerIndex = 0;
			if (LoadedGCode.LineCount <= 0)
			{
				return;
			}
			int num = 0;
			Vector3 position = LoadedGCode.Instruction(0).Position;
			double ePosition = LoadedGCode.Instruction(0).EPosition;
			for (int i = 1; i < LoadedGCode.LineCount; i++)
			{
				PrinterMachineInstruction printerMachineInstruction = LoadedGCode.Instruction(i);
				if (printerMachineInstruction.EPosition > ePosition && position != printerMachineInstruction.Position)
				{
					num = i;
					break;
				}
				position = printerMachineInstruction.Position;
			}
			if (num <= 0)
			{
				return;
			}
			for (int j = 0; j < LoadedGCode.NumChangesInZ; j++)
			{
				if (num < LoadedGCode.GetInstructionIndexAtLayer(j))
				{
					activeLayerIndex = Math.Max(0, j - 1);
					break;
				}
			}
		}

		private void initialLoading_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			ProgressChangedEventHandler loadingProgressChanged = LoadingProgressChanged;
			if (loadingProgressChanged != null)
			{
				loadingProgressChanged.Invoke((object)this, e);
			}
		}

		private async void initialLoading_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			SetGCodeAfterLoad((GCodeFile)e.get_Result());
			gCodeRenderer = new GCodeRenderer(LoadedGCode);
			if (ActiveSliceSettings.Instance.PrinterSelected)
			{
				GCodeRenderer.ExtruderWidth = ActiveSliceSettings.Instance.GetValue<double>("nozzle_diameter");
			}
			else
			{
				GCodeRenderer.ExtruderWidth = 0.4;
			}
			await Task.Run(delegate
			{
				DoPostLoadInitialization();
			});
			postLoadInitialization_RunWorkerCompleted();
		}

		public void DoPostLoadInitialization()
		{
			try
			{
				gCodeRenderer.GCodeFileToDraw?.GetFilamentUsedMm(ActiveSliceSettings.Instance.GetValue<double>("filament_diameter"));
			}
			catch (Exception)
			{
			}
			gCodeRenderer.CreateFeaturesForLayerIfRequired(0);
		}

		private void postLoadInitialization_RunWorkerCompleted()
		{
			this.DoneLoading?.Invoke(this, null);
		}

		public override void OnKeyDown(KeyEventArgs keyEvent)
		{
			((GuiWidget)this).OnKeyDown(keyEvent);
			if (keyEvent.get_Control())
			{
				selectRange = true;
			}
		}

		public override void OnKeyUp(KeyEventArgs keyEvent)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).OnKeyUp(keyEvent);
			if ((keyEvent.get_KeyData() & -65536) == 0)
			{
				selectRange = false;
			}
		}

		private void AddInstructionRangeToSelection(int start, int end)
		{
			for (int i = start; i <= end; i++)
			{
				if (LoadedGCode.Instruction(i).Line.Contains("G1"))
				{
					SelectedFeatureInstructionIndices.Add(i);
					gCodeRenderer.Clear3DGCode(LoadedGCode.GetLayerIndex(i));
				}
			}
		}

		private void ProcessReplayClick(MouseEventArgs mouseEvent)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			if (FindPathIndexHitPosotion(mouseEvent.get_Position(), out var instructionFoundIndex) && IsInstructionIndexClickable(instructionFoundIndex, ScreenPositionMapedToBed(mouseEvent.get_Position())))
			{
				if (selectRange)
				{
					List<int> list = new List<int>((IEnumerable<int>)SelectedFeatureInstructionIndices);
					list.Sort();
					int num = -1;
					int num2 = -1;
					foreach (int item in list)
					{
						if (item > instructionFoundIndex)
						{
							num2 = item;
							break;
						}
						if (item < instructionFoundIndex)
						{
							num = item;
						}
					}
					if (num == -1)
					{
						num = 0;
					}
					int num3 = instructionFoundIndex - num;
					int num4 = num2 - instructionFoundIndex;
					if (num3 <= num4 || num2 == -1)
					{
						AddInstructionRangeToSelection(num, instructionFoundIndex);
					}
					else
					{
						AddInstructionRangeToSelection(instructionFoundIndex, num2);
					}
				}
				else if (SelectedFeatureInstructionIndices.Contains(instructionFoundIndex))
				{
					SelectedFeatureInstructionIndices.Remove(instructionFoundIndex);
					gCodeRenderer.Clear3DGCode(LoadedGCode.GetLayerIndex(instructionFoundIndex));
				}
				else
				{
					AddInstructionRangeToSelection(instructionFoundIndex, instructionFoundIndex);
				}
				return;
			}
			Enumerator<int> enumerator2 = SelectedFeatureInstructionIndices.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					int current2 = enumerator2.get_Current();
					gCodeRenderer.Clear3DGCode(LoadedGCode.GetLayerIndex(current2));
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
			SelectedFeatureInstructionIndices.Clear();
		}

		private void ProcessRewindClick(MouseEventArgs mouseEvent)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			if (FindPathIndexHitPosotion(mouseEvent.get_Position(), out var instructionFoundIndex))
			{
				if (IsInstructionIndexClickable(instructionFoundIndex, ScreenPositionMapedToBed(mouseEvent.get_Position())))
				{
					if (StartLineIndex == -1)
					{
						StartLineIndex = instructionFoundIndex;
						StartBedPosition = ScreenPositionMapedToBed(mouseEvent.get_Position());
					}
					else if (gCodeRenderer.GCodeFileToDraw?.GetLayerIndex(StartLineIndex) != gCodeRenderer.GCodeFileToDraw?.GetLayerIndex(instructionFoundIndex))
					{
						StartLineIndex = instructionFoundIndex;
						EndLineIndex = -1;
						StartBedPosition = ScreenPositionMapedToBed(mouseEvent.get_Position());
						EndBedPosition = Vector2.Zero;
					}
					else if (EndLineIndex == -1 && instructionFoundIndex >= StartLineIndex)
					{
						EndLineIndex = instructionFoundIndex;
						EndBedPosition = ScreenPositionMapedToBed(mouseEvent.get_Position());
					}
				}
				else
				{
					StartLineIndex = instructionFoundIndex;
					EndLineIndex = -1;
					StartBedPosition = ScreenPositionMapedToBed(mouseEvent.get_Position());
					EndBedPosition = Vector2.Zero;
				}
			}
			else
			{
				StartLineIndex = -1;
				EndLineIndex = -1;
				StartBedPosition = Vector2.Zero;
				EndBedPosition = Vector2.Zero;
			}
		}

		public override void OnClick(MouseEventArgs mouseEvent)
		{
			if (!mouseWasDragged)
			{
				ProcessRewindClick(mouseEvent);
			}
			((GuiWidget)this).OnClick(mouseEvent);
			mouseWasDragged = false;
			((GuiWidget)this).Invalidate();
		}

		private bool IsInstructionIndexClickable(int instructionIndex, Vector2 clickPositionOnBed)
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			if (LoadedGCode != null && LoadedGCode.GetLayerIndex(StartLineIndex) != LoadedGCode.GetLayerIndex(instructionIndex))
			{
				return true;
			}
			if (StartLineIndex > -1 && StartLineIndex == EndLineIndex && StartLineIndex == instructionIndex)
			{
				RectangleDouble val = default(RectangleDouble);
				((RectangleDouble)(ref val))._002Ector(Math.Min(StartBedPosition.x, EndBedPosition.x), Math.Min(StartBedPosition.y, EndBedPosition.y), Math.Max(StartBedPosition.x, EndBedPosition.x), Math.Max(StartBedPosition.y, EndBedPosition.y));
				return ((RectangleDouble)(ref val)).Contains(clickPositionOnBed);
			}
			if (StartLineIndex > -1 && instructionIndex == StartLineIndex)
			{
				PrinterMachineInstruction printerMachineInstruction = LoadedGCode.Instruction(instructionIndex);
				RectangleDouble val2 = default(RectangleDouble);
				((RectangleDouble)(ref val2))._002Ector(Math.Min(StartBedPosition.x, printerMachineInstruction.X), Math.Min(StartBedPosition.y, printerMachineInstruction.Y), Math.Max(StartBedPosition.x, printerMachineInstruction.X), Math.Max(StartBedPosition.y, printerMachineInstruction.Y));
				return ((RectangleDouble)(ref val2)).Contains(clickPositionOnBed);
			}
			if (EndLineIndex > 0 && instructionIndex == EndLineIndex)
			{
				PrinterMachineInstruction printerMachineInstruction2 = LoadedGCode.Instruction(instructionIndex - 1);
				RectangleDouble val3 = default(RectangleDouble);
				((RectangleDouble)(ref val3))._002Ector(Math.Min(printerMachineInstruction2.X, EndBedPosition.x), Math.Min(printerMachineInstruction2.Y, EndBedPosition.y), Math.Max(printerMachineInstruction2.X, EndBedPosition.x), Math.Max(printerMachineInstruction2.Y, EndBedPosition.y));
				return ((RectangleDouble)(ref val3)).Contains(clickPositionOnBed);
			}
			if (instructionIndex >= StartLineIndex)
			{
				if (EndLineIndex != -1)
				{
					return instructionIndex <= EndLineIndex;
				}
				return true;
			}
			return false;
		}

		private Vector2 ScreenPositionMapedToBed(Vector2 screenPosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			RectangleDouble localBounds = ((GuiWidget)this).get_LocalBounds();
			return (screenPosition - ((RectangleDouble)(ref localBounds)).get_Center() - layerScale * unscaledRenderOffset) / layerScale;
		}

		private bool FindPathIndexHitPosotion(Vector2 screenPosition, out int instructionFoundIndex)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			instructionFoundIndex = -1;
			if (LoadedGCode != null)
			{
				Vector2 val = ScreenPositionMapedToBed(screenPosition);
				int instructionIndexAtLayer = LoadedGCode.GetInstructionIndexAtLayer(activeLayerIndex);
				int num = LoadedGCode.LineCount;
				if (activeLayerIndex < LoadedGCode.NumChangesInZ - 1)
				{
					num = LoadedGCode.GetInstructionIndexAtLayer(activeLayerIndex + 1);
				}
				PrinterMachineInstruction printerMachineInstruction = LoadedGCode.Instruction(0);
				PrinterMachineInstruction printerMachineInstruction2 = printerMachineInstruction;
				Vector2 val2 = default(Vector2);
				RectangleDouble val4 = default(RectangleDouble);
				for (int i = instructionIndexAtLayer; i < num; i++)
				{
					printerMachineInstruction2 = printerMachineInstruction;
					printerMachineInstruction = LoadedGCode.Instruction(i);
					if (printerMachineInstruction2.Position != printerMachineInstruction.Position && printerMachineInstruction.Line.Contains("G1"))
					{
						((Vector2)(ref val2))._002Ector(printerMachineInstruction.Position - printerMachineInstruction2.Position);
						Vector2 val3 = val - new Vector2(printerMachineInstruction2.Position);
						double num2 = Math.Atan2(val2.y, val2.x);
						if (num2 < 0.0)
						{
							num2 = 0.0 - num2;
							val2.y = 0.0 - val2.y;
							val3.y = 0.0 - val3.y;
						}
						double num3 = Math.PI / 2.0 - num2;
						((Vector2)(ref val2)).Rotate(num3);
						((Vector2)(ref val3)).Rotate(num3);
						((RectangleDouble)(ref val4))._002Ector(0.0, 0.0, 0.0, ((Vector2)(ref val2)).get_Length());
						double num4 = ((printerMachineInstruction.ExtruderIndex < LoadedGCode.PrintTools.get_Count()) ? LoadedGCode.PrintTools.get_Item(printerMachineInstruction.ExtruderIndex).get_Width() : GCodeRenderer.ExtruderWidth);
						((RectangleDouble)(ref val4)).Inflate(num4 / 2.0);
						if (((RectangleDouble)(ref val4)).Contains(val3))
						{
							flag = true;
							instructionFoundIndex = i;
							break;
						}
					}
				}
			}
			if (flag && (LoadedGCode.Ratio0to1IntoContainedLayer(instructionFoundIndex) < FeatureToStartOnRatio0To1 || LoadedGCode.Ratio0to1IntoContainedLayer(instructionFoundIndex) > FeatureToEndOnRatio0To1))
			{
				flag = false;
			}
			return flag;
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Expected O, but got Unknown
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			if (LoadedGCode != null)
			{
				Affine totalTransform = TotalTransform;
				if (RenderGrid)
				{
					double num = 0.2 * layerScale;
					Graphics2DOpenGL val = graphics2D as Graphics2DOpenGL;
					if (val != null)
					{
						GlRenderGrid(val, totalTransform, num);
					}
					else
					{
						CreateGrid(totalTransform);
						Stroke val2 = new Stroke((IVertexSource)(object)grid, num);
						graphics2D.Render((IVertexSource)(object)val2, (IColorType)(object)gridColor);
					}
				}
				GCodeRenderInfo renderInfo = new GCodeRenderInfo(activeLayerIndex, activeLayerIndex, totalTransform, layerScale, CreateRenderInfo(), FeatureToStartOnRatio0To1, FeatureToEndOnRatio0To1, (Vector2[])(object)new Vector2[2]
				{
					ActiveSliceSettings.Instance.Helpers.ExtruderOffset(0),
					ActiveSliceSettings.Instance.Helpers.ExtruderOffset(1)
				}, GCodeSelectionInfo);
				gCodeRenderer?.Render(graphics2D, renderInfo);
			}
			((GuiWidget)this).OnDraw(graphics2D);
		}

		private RenderType CreateRenderInfo()
		{
			RenderType renderType = RenderType.Extrusions;
			if (RenderMoves)
			{
				renderType |= RenderType.Moves;
			}
			if (RenderRetractions)
			{
				renderType |= RenderType.Retractions;
			}
			if (RenderSpeeds)
			{
				renderType |= RenderType.SpeedColors;
			}
			if (SimulateExtrusion)
			{
				renderType |= RenderType.SimulateExtrusion;
			}
			if (TransparentExtrusion)
			{
				renderType |= RenderType.TransparentExtrusion;
			}
			if (HideExtruderOffsets)
			{
				renderType |= RenderType.HideExtruderOffsets;
			}
			return renderType;
		}

		private void GlRenderGrid(Graphics2DOpenGL graphics2DGl, Affine transform, double width)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			graphics2DGl.PreRender();
			GL.Begin((BeginMode)4);
			Vector2 val = gridCenterMm - gridSizeMm / 2.0;
			if (gridSizeMm.x > 0.0 && gridSizeMm.y > 0.0)
			{
				grid.remove_all();
				for (int i = 0; (double)i <= gridSizeMm.y; i += 10)
				{
					Vector2 val2 = new Vector2(0.0, (double)i) + val;
					Vector2 val3 = new Vector2(gridSizeMm.x, (double)i) + val;
					((Affine)(ref transform)).transform(ref val2);
					((Affine)(ref transform)).transform(ref val3);
					graphics2DGl.DrawAALine(val2, val3, width, (IColorType)(object)gridColor);
				}
				for (int j = 0; (double)j <= gridSizeMm.x; j += 10)
				{
					Vector2 val4 = new Vector2((double)j, 0.0) + val;
					Vector2 val5 = new Vector2((double)j, gridSizeMm.y) + val;
					((Affine)(ref transform)).transform(ref val4);
					((Affine)(ref transform)).transform(ref val5);
					graphics2DGl.DrawAALine(val4, val5, width, (IColorType)(object)gridColor);
				}
			}
			GL.End();
			graphics2DGl.PopOrthoProjection();
		}

		public void CreateGrid(Affine transform)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = gridCenterMm - gridSizeMm / 2.0;
			if (gridSizeMm.x > 0.0 && gridSizeMm.y > 0.0)
			{
				grid.remove_all();
				for (int i = 0; (double)i <= gridSizeMm.y; i += 10)
				{
					Vector2 val2 = new Vector2(0.0, (double)i) + val;
					Vector2 val3 = new Vector2(gridSizeMm.x, (double)i) + val;
					((Affine)(ref transform)).transform(ref val2);
					((Affine)(ref transform)).transform(ref val3);
					grid.MoveTo(Math.Round(val2.x), Math.Round(val2.y));
					grid.LineTo(Math.Round(val3.x), Math.Round(val3.y));
				}
				for (int j = 0; (double)j <= gridSizeMm.x; j += 10)
				{
					Vector2 val4 = new Vector2((double)j, 0.0) + val;
					Vector2 val5 = new Vector2((double)j, gridSizeMm.y) + val;
					((Affine)(ref transform)).transform(ref val4);
					((Affine)(ref transform)).transform(ref val5);
					grid.MoveTo((double)(int)(val4.x + 0.5) + 0.5, (double)(int)(val4.y + 0.5));
					grid.LineTo((double)(int)(val5.x + 0.5) + 0.5, (double)(int)(val5.y + 0.5));
				}
			}
		}

		public override void OnMouseDown(MouseEventArgs mouseEvent)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).OnMouseDown(mouseEvent);
			if (((GuiWidget)this).get_MouseCaptured())
			{
				if (mouseEvent.get_NumPositions() != 1)
				{
					Vector2 val = (mouseDownPosition = (mouseEvent.GetPosition(1) + mouseEvent.GetPosition(0)) / 2.0);
				}
				else
				{
					mouseDownPosition.x = mouseEvent.get_X();
					mouseDownPosition.y = mouseEvent.get_Y();
				}
				lastMousePosition = mouseDownPosition;
				if (mouseEvent.get_NumPositions() > 1)
				{
					Vector2 val2 = mouseEvent.GetPosition(1) - mouseEvent.GetPosition(0);
					startDistanceBetweenPoints = ((Vector2)(ref val2)).get_Length();
					pinchStartScale = layerScale;
				}
			}
		}

		public override void OnMouseWheel(MouseEventArgs mouseEvent)
		{
			((GuiWidget)this).OnMouseWheel(mouseEvent);
			if (((GuiWidget)this).get_FirstWidgetUnderMouse())
			{
				double num = (double)mouseEvent.get_WheelDelta() / 120.0 * 0.1;
				ScalePartAndFixPosition(mouseEvent, layerScale + layerScale * num);
				((GuiWidget)this).Invalidate();
			}
		}

		private void ScalePartAndFixPosition(MouseEventArgs mouseEvent, double scaleAmount)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = default(Vector2);
			((Vector2)(ref val))._002Ector(mouseEvent.get_X(), mouseEvent.get_Y());
			Affine totalTransform = TotalTransform;
			((Affine)(ref totalTransform)).inverse_transform(ref val);
			layerScale = scaleAmount;
			Vector2 val2 = default(Vector2);
			((Vector2)(ref val2))._002Ector(mouseEvent.get_X(), mouseEvent.get_Y());
			totalTransform = TotalTransform;
			((Affine)(ref totalTransform)).inverse_transform(ref val2);
			unscaledRenderOffset += val2 - val;
		}

		public override void OnMouseMove(MouseEventArgs mouseEvent)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).OnMouseMove(mouseEvent);
			Vector2 val = default(Vector2);
			if (mouseEvent.get_NumPositions() == 1)
			{
				((Vector2)(ref val))._002Ector(mouseEvent.get_X(), mouseEvent.get_Y());
			}
			else
			{
				val = (mouseEvent.GetPosition(1) + mouseEvent.GetPosition(0)) / 2.0;
			}
			if (((GuiWidget)this).get_MouseCaptured())
			{
				Vector2 val2 = val - lastMousePosition;
				Affine val4;
				switch (TransformState)
				{
				case ETransformState.Move:
					val4 = ScalingTransform;
					((Affine)(ref val4)).inverse_transform(ref val2);
					unscaledRenderOffset += val2;
					break;
				case ETransformState.Scale:
				{
					double num = 1.0;
					if (val2.y < 0.0)
					{
						num = 1.0 - -1.0 * val2.y / 100.0;
					}
					else if (val2.y > 0.0)
					{
						num = 1.0 + 1.0 * val2.y / 100.0;
					}
					Vector2 val3 = mouseDownPosition;
					val4 = TotalTransform;
					((Affine)(ref val4)).inverse_transform(ref val3);
					layerScale *= num;
					Vector2 val5 = mouseDownPosition;
					val4 = TotalTransform;
					((Affine)(ref val4)).inverse_transform(ref val5);
					unscaledRenderOffset += val5 - val3;
					break;
				}
				default:
					throw new NotImplementedException();
				}
				mouseWasDragged = true;
				((GuiWidget)this).Invalidate();
			}
			lastMousePosition = val;
			if (TransformState == ETransformState.Move && mouseEvent.get_NumPositions() > 1 && startDistanceBetweenPoints > 0.0)
			{
				Vector2 val6 = mouseEvent.GetPosition(1) - mouseEvent.GetPosition(0);
				double length = ((Vector2)(ref val6)).get_Length();
				double scaleAmount = pinchStartScale * length / startDistanceBetweenPoints;
				ScalePartAndFixPosition(mouseEvent, scaleAmount);
			}
		}

		public void Load(string gcodePathAndFileName)
		{
			LoadedGCode = GCodeFile.Load(gcodePathAndFileName);
			SetInitalLayer();
			CenterPartInView();
		}

		public void LoadInBackground(string gcodePathAndFileName)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected O, but got Unknown
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			FileNameAndPath = gcodePathAndFileName;
			backgroundWorker = new BackgroundWorker();
			backgroundWorker.set_WorkerReportsProgress(true);
			backgroundWorker.set_WorkerSupportsCancellation(true);
			backgroundWorker.add_ProgressChanged(new ProgressChangedEventHandler(initialLoading_ProgressChanged));
			backgroundWorker.add_RunWorkerCompleted(new RunWorkerCompletedEventHandler(initialLoading_RunWorkerCompleted));
			LoadedGCode = null;
			GCodeFileLoaded.LoadInBackground(backgroundWorker, gcodePathAndFileName);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (gCodeRenderer != null)
			{
				gCodeRenderer.Dispose();
			}
			if (backgroundWorker != null)
			{
				backgroundWorker.CancelAsync();
			}
			((GuiWidget)this).OnClosed(e);
		}

		public void CenterPartInView()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			RectangleDouble bounds = LoadedGCode.GetBounds();
			Vector2 weightedCenter = LoadedGCode.GetWeightedCenter();
			unscaledRenderOffset = -weightedCenter;
			layerScale = Math.Min(((GuiWidget)this).get_Height() / ((RectangleDouble)(ref bounds)).get_Height(), ((GuiWidget)this).get_Width() / ((RectangleDouble)(ref bounds)).get_Width());
			((GuiWidget)this).Invalidate();
		}
	}
}
