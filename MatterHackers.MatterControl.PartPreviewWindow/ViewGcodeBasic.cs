using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.MatterControl.Replay;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class ViewGcodeBasic : PartPreview3DWidget
	{
		public enum WindowMode
		{
			Embeded,
			StandAlone
		}

		public delegate Vector2 GetSizeFunction();

		public SolidSlider selectLayerSlider;

		private SetLayerWidget setLayerWidget;

		private LayerNavigationWidget navigationWidget;

		public DoubleSolidSlider layerRenderRatioSlider;

		private TextWidget gcodeProcessingStateInfoText;

		private ViewGcodeWidget gcodeViewWidget;

		private PrintItemWrapper printItem;

		private bool startedSliceFromGenerateButton;

		private Button generateGCodeButton;

		private Button rewindButton;

		private Button printSelectedButton;

		private FlowLayoutWidget buttonBottomPanel;

		private FlowLayoutWidget layerSelectionButtonsPanel;

		private FlowLayoutWidget modelOptionsContainer;

		private FlowLayoutWidget displayOptionsContainer;

		private ViewControlsToggle viewControlsToggle;

		private CheckBox expandModelOptions;

		private CheckBox expandDisplayOptions;

		private CheckBox syncToPrint;

		private CheckBox showSpeeds;

		private GuiWidget gcodeDisplayWidget;

		private ColorGradientWidget gradientWidget;

		private EventHandler unregisterEvents;

		private WindowMode windowMode;

		private string slicingErrorMessage = "Slicing Error.\nPlease review your slice settings.".Localize();

		private string pressGenerateMessage = "Press 'generate' to view layers".Localize();

		private string fileNotFoundMessage = "File not found on disk.".Localize();

		private string fileTooBigToLoad = "GCode file too big to preview ({0}).".Localize();

		private Vector2 bedCenter;

		private Vector3 viewerVolume;

		private BedShape bedShape;

		private int sliderWidth;

		private TextWidget massTextWidget;

		private FlowLayoutWidget estimatedCostInfo;

		private TextWidget costTextWidget;

		private string partToStartLoadingOnFirstDraw;

		private GuiWidget widgetThatHasKeyDownHooked;

		private double totalMass
		{
			get
			{
				double value = ActiveSliceSettings.Instance.GetValue<double>("filament_diameter");
				double value2 = ActiveSliceSettings.Instance.GetValue<double>("filament_density");
				return gcodeViewWidget.LoadedGCode.GetFilamentWeightGrams(value, value2);
			}
		}

		private double totalCost
		{
			get
			{
				double value = ActiveSliceSettings.Instance.GetValue<double>("filament_cost");
				return totalMass / 1000.0 * value;
			}
		}

		public ViewGcodeBasic(PrintItemWrapper printItem, Vector3 viewerVolume, Vector2 bedCenter, BedShape bedShape, WindowMode windowMode)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			this.viewerVolume = viewerVolume;
			this.bedShape = bedShape;
			this.bedCenter = bedCenter;
			this.windowMode = windowMode;
			this.printItem = printItem;
			if (UserSettings.Instance.IsTouchScreen)
			{
				sliderWidth = 20;
			}
			else
			{
				sliderWidth = 10;
			}
			CreateAndAddChildren();
			ActiveSliceSettings.SettingChanged.RegisterEvent((EventHandler)CheckSettingChanged, ref unregisterEvents);
			ApplicationController.Instance.AdvancedControlsPanelReloading.RegisterEvent((EventHandler)delegate
			{
				ClearGCode();
			}, ref unregisterEvents);
		}

		private void CheckSettingChanged(object sender, EventArgs e)
		{
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			StringEventArgs val = e as StringEventArgs;
			if (val == null)
			{
				return;
			}
			if (gcodeViewWidget?.LoadedGCode != null && (val.get_Data() == "filament_cost" || val.get_Data() == "filament_diameter" || val.get_Data() == "filament_density"))
			{
				UpdateMassText();
				UpdateEstimatedCost();
			}
			if (val.get_Data() == "bed_size" || val.get_Data() == "print_center" || val.get_Data() == "build_height" || val.get_Data() == "bed_shape" || val.get_Data() == "center_part_on_bed")
			{
				viewerVolume = new Vector3(ActiveSliceSettings.Instance.GetValue<Vector2>("bed_size"), ActiveSliceSettings.Instance.GetValue<double>("build_height"));
				bedShape = ActiveSliceSettings.Instance.GetValue<BedShape>("bed_shape");
				bedCenter = ActiveSliceSettings.Instance.GetValue<Vector2>("print_center");
				ActiveSliceSettings.Instance.GetValue<double>("build_height");
				UiThread.RunOnIdle((Action)delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					meshViewerWidget.CreatePrintBed(viewerVolume, bedCenter, bedShape);
				});
			}
			else if (val.get_Data() == "extruder_offset")
			{
				ClearGCode();
			}
		}

		private void ClearGCode()
		{
			if (gcodeViewWidget != null && gcodeViewWidget.gCodeRenderer != null)
			{
				((GuiWidget)rewindButton).set_Visible(false);
				((GuiWidget)printSelectedButton).set_Visible(false);
				gcodeViewWidget.GCodeSelectionInfo.ClearSelection();
				gcodeViewWidget.gCodeRenderer.Clear3DGCode();
				((GuiWidget)gcodeViewWidget).Invalidate();
			}
		}

		private void CreateAndAddChildren()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Expected O, but got Unknown
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Expected O, but got Unknown
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Expected O, but got Unknown
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Expected O, but got Unknown
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Expected O, but got Unknown
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Expected O, but got Unknown
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Expected O, but got Unknown
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();
			textImageButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.disabledTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			((GuiWidget)this).CloseAllChildren();
			gcodeViewWidget = null;
			gcodeProcessingStateInfoText = null;
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)13);
			((GuiWidget)val).set_VAnchor((VAnchor)13);
			buttonBottomPanel = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)buttonBottomPanel).set_HAnchor((HAnchor)5);
			((GuiWidget)buttonBottomPanel).set_Padding(new BorderDouble(3.0, 3.0));
			((GuiWidget)buttonBottomPanel).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			generateGCodeButton = textImageButtonFactory.Generate("Generate".Localize());
			((GuiWidget)generateGCodeButton).set_Name("Generate Gcode Button");
			((GuiWidget)generateGCodeButton).add_Click((EventHandler<MouseEventArgs>)generateButton_Click);
			((GuiWidget)buttonBottomPanel).AddChild((GuiWidget)(object)generateGCodeButton, -1);
			layerSelectionButtonsPanel = new FlowLayoutWidget((FlowDirection)2);
			((GuiWidget)layerSelectionButtonsPanel).set_HAnchor((HAnchor)5);
			((GuiWidget)layerSelectionButtonsPanel).set_Padding(new BorderDouble(0.0));
			GuiWidget val2 = new GuiWidget(1.0, ((GuiWidget)generateGCodeButton).get_Height(), (SizeLimitsToSet)1);
			((GuiWidget)layerSelectionButtonsPanel).AddChild(val2, -1);
			if (windowMode == WindowMode.StandAlone)
			{
				Button val3 = textImageButtonFactory.Generate("Close".Localize());
				((GuiWidget)layerSelectionButtonsPanel).AddChild((GuiWidget)(object)val3, -1);
				((GuiWidget)val3).add_Click((EventHandler<MouseEventArgs>)delegate
				{
					((GuiWidget)this).CloseOnIdle();
				});
			}
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val4).AnchorAll();
			GuiWidget val5 = new GuiWidget();
			val5.set_BackgroundColor(RGBA_Bytes.Black);
			val5.set_HAnchor((HAnchor)5);
			val5.set_VAnchor((VAnchor)5);
			gcodeDisplayWidget = val5;
			string processingMessage = "Press 'Add' to select an item.".Localize();
			if (printItem != null)
			{
				processingMessage = "Loading G-Code...".Localize();
				if (Path.GetExtension(printItem.FileLocation)!.ToUpper() == ".GCODE")
				{
					gcodeDisplayWidget.AddChild(CreateGCodeViewWidget(printItem.FileLocation), -1);
				}
				else if (File.Exists(printItem.FileLocation))
				{
					string gCodePathAndFileName = printItem.GetGCodePathAndFileName();
					bool flag = printItem.IsGCodeFileComplete(gCodePathAndFileName);
					processingMessage = ((!printItem.SlicingHadError) ? pressGenerateMessage : slicingErrorMessage);
					if (File.Exists(gCodePathAndFileName) && flag)
					{
						gcodeDisplayWidget.AddChild(CreateGCodeViewWidget(gCodePathAndFileName), -1);
					}
					printItem.SlicingOutputMessage += sliceItem_SlicingOutputMessage;
					printItem.SlicingDone += sliceItem_Done;
				}
				else
				{
					processingMessage = $"{fileNotFoundMessage}\n'{printItem.Name}'";
				}
			}
			else
			{
				((GuiWidget)generateGCodeButton).set_Visible(false);
			}
			SetProcessingMessage(processingMessage);
			((GuiWidget)val4).AddChild(gcodeDisplayWidget, -1);
			buttonRightPanel = CreateRightButtonPanel();
			((GuiWidget)buttonRightPanel).set_Visible(false);
			((GuiWidget)val4).AddChild((GuiWidget)(object)buttonRightPanel, -1);
			if (gcodeViewWidget != null)
			{
				EventHandler setReplayButtonVisibility = delegate
				{
					if (!PrinterConnectionAndCommunication.Instance.PrinterIsConnected)
					{
						((GuiWidget)rewindButton).set_Visible(false);
						((GuiWidget)printSelectedButton).set_Visible(false);
					}
					else if (gcodeViewWidget.gCodeRenderer != null)
					{
						((GuiWidget)rewindButton).set_Visible(gcodeViewWidget.StartLineIndex != -1 && gcodeViewWidget.EndLineIndex != -1);
						((GuiWidget)printSelectedButton).set_Visible(gcodeViewWidget.SelectedFeatureInstructionIndices.get_Count() > 0);
					}
					else
					{
						((GuiWidget)rewindButton).set_Visible(false);
						((GuiWidget)printSelectedButton).set_Visible(false);
					}
				};
				PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent(setReplayButtonVisibility, ref unregisterEvents);
				((GuiWidget)gcodeViewWidget).add_Click((EventHandler<MouseEventArgs>)delegate(object sender, MouseEventArgs e)
				{
					setReplayButtonVisibility(sender, (EventArgs)(object)e);
				});
			}
			FlowLayoutWidget obj = layerSelectionButtonsPanel;
			GuiWidget val6 = new GuiWidget();
			val6.set_HAnchor((HAnchor)5);
			((GuiWidget)obj).AddChild(val6, -1);
			((GuiWidget)buttonBottomPanel).AddChild((GuiWidget)(object)layerSelectionButtonsPanel, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)buttonBottomPanel, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			meshViewerWidget = new MeshViewerWidget(viewerVolume, bedCenter, bedShape, "".Localize());
			((GuiWidget)meshViewerWidget).AnchorAll();
			meshViewerWidget.AllowBedRenderingWhenEmpty = true;
			gcodeDisplayWidget.AddChild((GuiWidget)(object)meshViewerWidget, -1);
			((GuiWidget)meshViewerWidget).set_Visible(false);
			meshViewerWidget.TrackballTumbleWidget.DrawGlContent += TrackballTumbleWidget_DrawGlContent;
			viewControls2D = new ViewControls2D();
			((GuiWidget)this).AddChild((GuiWidget)(object)viewControls2D, -1);
			viewControls2D.ResetView += delegate
			{
				SetDefaultView2D();
			};
			viewControls3D = new ViewControls3D(meshViewerWidget);
			viewControls3D.PartSelectVisible = false;
			((GuiWidget)this).AddChild((GuiWidget)(object)viewControls3D, -1);
			viewControls3D.ResetView += delegate
			{
				meshViewerWidget.ResetView();
			};
			viewControls3D.ActiveButton = ViewControls3DButtons.Rotate;
			((GuiWidget)viewControls3D).set_Visible(false);
			viewControlsToggle = new ViewControlsToggle();
			((GuiWidget)viewControlsToggle).set_HAnchor((HAnchor)4);
			((GuiWidget)this).AddChild((GuiWidget)(object)viewControlsToggle, -1);
			((GuiWidget)viewControlsToggle).set_Visible(false);
			meshViewerWidget.ResetView();
			((GuiWidget)viewControls2D.translateButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				gcodeViewWidget.TransformState = ViewGcodeWidget.ETransformState.Move;
			});
			((GuiWidget)viewControls2D.scaleButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				gcodeViewWidget.TransformState = ViewGcodeWidget.ETransformState.Scale;
			});
			AddHandlers();
		}

		private void SetDefaultView2D()
		{
			gcodeViewWidget.CenterPartInView();
		}

		private RenderType GetRenderType()
		{
			RenderType renderType = RenderType.Extrusions;
			if (gcodeViewWidget.RenderMoves)
			{
				renderType |= RenderType.Moves;
			}
			if (gcodeViewWidget.RenderRetractions)
			{
				renderType |= RenderType.Retractions;
			}
			if (gcodeViewWidget.RenderSpeeds)
			{
				renderType |= RenderType.SpeedColors;
			}
			if (gcodeViewWidget.SimulateExtrusion)
			{
				renderType |= RenderType.SimulateExtrusion;
			}
			if (gcodeViewWidget.TransparentExtrusion)
			{
				renderType |= RenderType.TransparentExtrusion;
			}
			if (gcodeViewWidget.HideExtruderOffsets)
			{
				renderType |= RenderType.HideExtruderOffsets;
			}
			return renderType;
		}

		private void TrackballTumbleWidget_DrawGlContent(object sender, EventArgs e)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			GCodeRenderer.ExtrusionColor = ActiveTheme.get_Instance().get_PrimaryAccentColor();
			GCodeRenderInfo renderInfo = new GCodeRenderInfo(0, Math.Min(gcodeViewWidget.ActiveLayerIndex + 1, gcodeViewWidget.LoadedGCode.NumChangesInZ), gcodeViewWidget.TotalTransform, 1.0, GetRenderType(), gcodeViewWidget.FeatureToStartOnRatio0To1, gcodeViewWidget.FeatureToEndOnRatio0To1, (Vector2[])(object)new Vector2[2]
			{
				ActiveSliceSettings.Instance.Helpers.ExtruderOffset(0),
				ActiveSliceSettings.Instance.Helpers.ExtruderOffset(1)
			}, gcodeViewWidget.GCodeSelectionInfo);
			gcodeViewWidget.gCodeRenderer.Render3D(renderInfo);
		}

		private void SetAnimationPosition()
		{
			int currentlyPrintingLayer = PrinterConnectionAndCommunication.Instance.CurrentlyPrintingLayer;
			if (currentlyPrintingLayer < 0)
			{
				selectLayerSlider.Value = 0.0;
				layerRenderRatioSlider.SecondValue = 0.0;
				layerRenderRatioSlider.FirstValue = 0.0;
			}
			else
			{
				selectLayerSlider.Value = currentlyPrintingLayer - 1;
				layerRenderRatioSlider.SecondValue = PrinterConnectionAndCommunication.Instance.RatioIntoCurrentLayer;
				layerRenderRatioSlider.FirstValue = 0.0;
			}
			if (layerRenderRatioSlider.SecondValue < 0.0005 && currentlyPrintingLayer > 1)
			{
				selectLayerSlider.Value = currentlyPrintingLayer - 2;
				layerRenderRatioSlider.SecondValue = 1.0;
			}
		}

		private FlowLayoutWidget CreateRightButtonPanel()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Expected O, but got Unknown
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Expected O, but got Unknown
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Expected O, but got Unknown
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_Width(200.0);
			string label = "Model".Localize().ToUpper();
			expandModelOptions = ExpandMenuOptionFactory.GenerateCheckBoxButton(label, View3DWidget.ArrowRight, View3DWidget.ArrowDown);
			((GuiWidget)expandModelOptions).set_Margin(new BorderDouble(0.0, 2.0, 0.0, 0.0));
			((GuiWidget)val).AddChild((GuiWidget)(object)expandModelOptions, -1);
			expandModelOptions.set_Checked(true);
			modelOptionsContainer = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)modelOptionsContainer).set_HAnchor((HAnchor)5);
			((GuiWidget)val).AddChild((GuiWidget)(object)modelOptionsContainer, -1);
			expandDisplayOptions = ExpandMenuOptionFactory.GenerateCheckBoxButton("Display".Localize().ToUpper(), View3DWidget.ArrowRight, View3DWidget.ArrowDown);
			((GuiWidget)expandDisplayOptions).set_Name("Display Checkbox");
			((GuiWidget)expandDisplayOptions).set_Margin(new BorderDouble(0.0, 2.0, 0.0, 0.0));
			((GuiWidget)val).AddChild((GuiWidget)(object)expandDisplayOptions, -1);
			expandDisplayOptions.set_Checked(false);
			displayOptionsContainer = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)displayOptionsContainer).set_HAnchor((HAnchor)5);
			((GuiWidget)displayOptionsContainer).set_Padding(new BorderDouble(6.0, 0.0, 0.0, 0.0));
			((GuiWidget)displayOptionsContainer).set_Visible(false);
			((GuiWidget)val).AddChild((GuiWidget)(object)displayOptionsContainer, -1);
			GuiWidget val2 = new GuiWidget();
			val2.set_VAnchor((VAnchor)5);
			((GuiWidget)val).AddChild(val2, -1);
			rewindButton = WhiteButtonFactory.Generate("Start Rewind".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)rewindButton).add_Click((EventHandler<MouseEventArgs>)RewindButton_Click);
			((GuiWidget)rewindButton).set_Visible(false);
			((GuiWidget)val).AddChild((GuiWidget)(object)rewindButton, -1);
			printSelectedButton = WhiteButtonFactory.Generate("Print Selected".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)printSelectedButton).add_Click((EventHandler<MouseEventArgs>)PrintSelectedButton_Click);
			((GuiWidget)printSelectedButton).set_Visible(false);
			((GuiWidget)val).AddChild((GuiWidget)(object)printSelectedButton, -1);
			((GuiWidget)val).set_Padding(new BorderDouble(6.0, 6.0));
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 1.0));
			((GuiWidget)val).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			return val;
		}

		private void CreateOptionsContent()
		{
			AddModelInfo(modelOptionsContainer);
			AddDisplayControls(displayOptionsContainer);
		}

		private void AddModelInfo(FlowLayoutWidget buttonPanel)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Expected O, but got Unknown
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Expected O, but got Unknown
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Expected O, but got Unknown
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Expected O, but got Unknown
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Expected O, but got Unknown
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Expected O, but got Unknown
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)buttonPanel).CloseAllChildren();
			double fixedWidth = textImageButtonFactory.FixedWidth;
			textImageButtonFactory.FixedWidth = 44.0 * GuiWidget.get_DeviceScale();
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Padding(new BorderDouble(5.0));
			string arg = "Print Time".Localize();
			string text = $"{arg}:";
			((GuiWidget)val).AddChild((GuiWidget)new TextWidget(text, 0.0, 0.0, 9.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			string arg2 = "---";
			if (gcodeViewWidget != null && gcodeViewWidget.LoadedGCode != null)
			{
				int num = (int)gcodeViewWidget.LoadedGCode.Instruction(0).secondsToEndFromHere;
				int num2 = num / 3600;
				int num3 = (num + 30) / 60 - num2 * 60;
				_ = num % 60;
				arg2 = ((num2 <= 0) ? $"{num3} min" : $"{num2} h, {num3} min");
			}
			GuiWidget val2 = (GuiWidget)new TextWidget($"{arg2}", 0.0, 0.0, 14.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_Margin(new BorderDouble(0.0, 9.0, 0.0, 3.0));
			((GuiWidget)val).AddChild(val2, -1);
			string arg3 = "Filament Length".Localize();
			string text2 = $"{arg3}:";
			((GuiWidget)val).AddChild((GuiWidget)new TextWidget(text2, 0.0, 0.0, 9.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			double filamentUsedMm = gcodeViewWidget.LoadedGCode.GetFilamentUsedMm(ActiveSliceSettings.Instance.GetValue<double>("filament_diameter"));
			GuiWidget val3 = (GuiWidget)new TextWidget($"{filamentUsedMm:0.0} mm", 0.0, 0.0, 14.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			val3.set_Margin(new BorderDouble(0.0, 9.0, 0.0, 3.0));
			((GuiWidget)val).AddChild(val3, -1);
			string arg4 = "Filament Volume".Localize();
			string text3 = $"{arg4}:";
			((GuiWidget)val).AddChild((GuiWidget)new TextWidget(text3, 0.0, 0.0, 9.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			double filamentCubicMm = gcodeViewWidget.LoadedGCode.GetFilamentCubicMm(ActiveSliceSettings.Instance.GetValue<double>("filament_diameter"));
			GuiWidget val4 = (GuiWidget)new TextWidget($"{filamentCubicMm / 1000.0:0.00} cm³", 0.0, 0.0, 14.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			val4.set_Margin(new BorderDouble(0.0, 9.0, 0.0, 3.0));
			((GuiWidget)val).AddChild(val4, -1);
			((GuiWidget)val).AddChild(GetEstimatedMassInfo(), -1);
			((GuiWidget)val).AddChild(GetEstimatedCostInfo(), -1);
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)HookUpGCodeMessagesWhenDonePrinting, ref unregisterEvents);
			((GuiWidget)buttonPanel).AddChild((GuiWidget)(object)val, -1);
			textImageButtonFactory.FixedWidth = fixedWidth;
		}

		private void UpdateMassText()
		{
			if (totalMass != 0.0)
			{
				((GuiWidget)massTextWidget).set_Text($"{totalMass:0.00} g");
			}
			else
			{
				((GuiWidget)massTextWidget).set_Text("Unknown");
			}
		}

		private GuiWidget GetEstimatedMassInfo()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected O, but got Unknown
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Expected O, but got Unknown
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			string arg = "Estimated Mass".Localize();
			string text = $"{arg}:";
			((GuiWidget)val).AddChild((GuiWidget)new TextWidget(text, 0.0, 0.0, 9.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			TextWidget val2 = new TextWidget("", 0.0, 0.0, 14.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_AutoExpandBoundsToText(true);
			massTextWidget = val2;
			((GuiWidget)massTextWidget).set_Margin(new BorderDouble(0.0, 9.0, 0.0, 3.0));
			((GuiWidget)val).AddChild((GuiWidget)(object)massTextWidget, -1);
			UpdateMassText();
			return (GuiWidget)val;
		}

		private void UpdateEstimatedCost()
		{
			((GuiWidget)costTextWidget).set_Text($"${totalCost:0.00}");
			if (totalCost == 0.0)
			{
				((GuiWidget)estimatedCostInfo).set_Visible(false);
			}
			else
			{
				((GuiWidget)estimatedCostInfo).set_Visible(true);
			}
		}

		private GuiWidget GetEstimatedCostInfo()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Expected O, but got Unknown
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			estimatedCostInfo = new FlowLayoutWidget((FlowDirection)3);
			string arg = "Estimated Cost".Localize();
			string text = $"{arg}:";
			((GuiWidget)estimatedCostInfo).AddChild((GuiWidget)new TextWidget(text, 0.0, 0.0, 9.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			TextWidget val = new TextWidget("", 0.0, 0.0, 14.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			val.set_AutoExpandBoundsToText(true);
			costTextWidget = val;
			((GuiWidget)costTextWidget).set_Margin(new BorderDouble(0.0, 9.0, 0.0, 3.0));
			((GuiWidget)estimatedCostInfo).AddChild((GuiWidget)(object)costTextWidget, -1);
			UpdateEstimatedCost();
			return (GuiWidget)(object)estimatedCostInfo;
		}

		private void AddLayerInfo(FlowLayoutWidget buttonPanel)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Expected O, but got Unknown
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Expected O, but got Unknown
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Expected O, but got Unknown
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Expected O, but got Unknown
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Expected O, but got Unknown
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Expected O, but got Unknown
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Expected O, but got Unknown
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Expected O, but got Unknown
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Expected O, but got Unknown
			double fixedWidth = textImageButtonFactory.FixedWidth;
			textImageButtonFactory.FixedWidth = 44.0 * GuiWidget.get_DeviceScale();
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Padding(new BorderDouble(5.0));
			((GuiWidget)val).AddChild((GuiWidget)new TextWidget("Layer Number:", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			((GuiWidget)val).AddChild((GuiWidget)new TextWidget("Layer Height:", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			((GuiWidget)val).AddChild((GuiWidget)new TextWidget("Num GCodes:", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			((GuiWidget)val).AddChild((GuiWidget)new TextWidget("Filament Used:", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			((GuiWidget)val).AddChild((GuiWidget)new TextWidget("Weight:", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			((GuiWidget)val).AddChild((GuiWidget)new TextWidget("Print Time:", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			((GuiWidget)val).AddChild((GuiWidget)new TextWidget("Extrude Speeds:", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			((GuiWidget)val).AddChild((GuiWidget)new TextWidget("Move Speeds:", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			((GuiWidget)val).AddChild((GuiWidget)new TextWidget("Retract Speeds:", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			((GuiWidget)buttonPanel).AddChild((GuiWidget)(object)val, -1);
			textImageButtonFactory.FixedWidth = fixedWidth;
		}

		private void AddDisplayControls(FlowLayoutWidget buttonPanel)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Expected O, but got Unknown
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Expected O, but got Unknown
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Expected O, but got Unknown
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Expected O, but got Unknown
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Expected O, but got Unknown
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Expected O, but got Unknown
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Expected O, but got Unknown
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Expected O, but got Unknown
			((GuiWidget)buttonPanel).CloseAllChildren();
			double fixedWidth = textImageButtonFactory.FixedWidth;
			textImageButtonFactory.FixedWidth = 44.0 * GuiWidget.get_DeviceScale();
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Padding(new BorderDouble(5.0));
			CheckBox showGrid = new CheckBox("Print Bed".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12.0);
			showGrid.set_Checked(gcodeViewWidget.RenderGrid);
			meshViewerWidget.RenderBed = showGrid.get_Checked();
			showGrid.add_CheckedStateChanged((EventHandler)delegate
			{
				gcodeViewWidget.RenderGrid = showGrid.get_Checked();
				meshViewerWidget.RenderBed = showGrid.get_Checked();
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)showGrid, -1);
			CheckBox showMoves = new CheckBox("Moves".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12.0);
			showMoves.set_Checked(gcodeViewWidget.RenderMoves);
			showMoves.add_CheckedStateChanged((EventHandler)delegate
			{
				gcodeViewWidget.RenderMoves = showMoves.get_Checked();
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)showMoves, -1);
			CheckBox showRetractions = new CheckBox("Retractions".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12.0);
			showRetractions.set_Checked(gcodeViewWidget.RenderRetractions);
			showRetractions.add_CheckedStateChanged((EventHandler)delegate
			{
				gcodeViewWidget.RenderRetractions = showRetractions.get_Checked();
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)showRetractions, -1);
			showSpeeds = new CheckBox("Speeds".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12.0);
			showSpeeds.set_Checked(gcodeViewWidget.RenderSpeeds);
			showSpeeds.add_CheckedStateChanged((EventHandler)delegate
			{
				((GuiWidget)gradientWidget).set_Visible(showSpeeds.get_Checked());
				gcodeViewWidget.RenderSpeeds = showSpeeds.get_Checked();
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)showSpeeds, -1);
			CheckBox simulateExtrusion = new CheckBox("Extrusion".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12.0);
			simulateExtrusion.set_Checked(gcodeViewWidget.SimulateExtrusion);
			simulateExtrusion.add_CheckedStateChanged((EventHandler)delegate
			{
				gcodeViewWidget.SimulateExtrusion = simulateExtrusion.get_Checked();
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)simulateExtrusion, -1);
			CheckBox val2 = new CheckBox("Transparent".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12.0);
			val2.set_Checked(gcodeViewWidget.TransparentExtrusion);
			((GuiWidget)val2).set_Margin(new BorderDouble(5.0, 0.0, 0.0, 0.0));
			((GuiWidget)val2).set_HAnchor((HAnchor)1);
			CheckBox transparentExtrusion = val2;
			transparentExtrusion.add_CheckedStateChanged((EventHandler)delegate
			{
				gcodeViewWidget.TransparentExtrusion = transparentExtrusion.get_Checked();
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)transparentExtrusion, -1);
			if (ActiveSliceSettings.Instance.GetValue<int>("extruder_count") > 1)
			{
				CheckBox hideExtruderOffsets = new CheckBox("Hide Offsets", ActiveTheme.get_Instance().get_PrimaryTextColor(), 12.0);
				hideExtruderOffsets.set_Checked(gcodeViewWidget.HideExtruderOffsets);
				hideExtruderOffsets.add_CheckedStateChanged((EventHandler)delegate
				{
					gcodeViewWidget.HideExtruderOffsets = hideExtruderOffsets.get_Checked();
				});
				((GuiWidget)val).AddChild((GuiWidget)(object)hideExtruderOffsets, -1);
			}
			viewControlsToggle.twoDimensionButton.add_CheckedStateChanged((EventHandler)delegate
			{
				SetLayerViewType();
			});
			viewControlsToggle.threeDimensionButton.add_CheckedStateChanged((EventHandler)delegate
			{
				SetLayerViewType();
			});
			SetLayerViewType();
			if (windowMode == WindowMode.Embeded)
			{
				syncToPrint = new CheckBox("Sync To Print".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12.0);
				syncToPrint.set_Checked(UserSettings.Instance.get("LayerViewSyncToPrint") == "True");
				((GuiWidget)syncToPrint).set_Name("Sync To Print Checkbox");
				syncToPrint.add_CheckedStateChanged((EventHandler)delegate
				{
					UserSettings.Instance.set("LayerViewSyncToPrint", syncToPrint.get_Checked().ToString());
					SetSyncToPrintVisibility();
				});
				((GuiWidget)val).AddChild((GuiWidget)(object)syncToPrint, -1);
				if (PrinterConnectionAndCommunication.Instance.PrinterIsPrinting && PrinterConnectionAndCommunication.Instance.ActivePrintItem == printItem)
				{
					printItem.SlicingOutputMessage -= sliceItem_SlicingOutputMessage;
					printItem.SlicingDone -= sliceItem_Done;
					((GuiWidget)generateGCodeButton).set_Visible(false);
					PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)HookUpGCodeMessagesWhenDonePrinting, ref unregisterEvents);
					UiThread.RunOnIdle((Action)SetSyncToPrintVisibility);
				}
			}
			((GuiWidget)buttonPanel).AddChild((GuiWidget)(object)val, -1);
			textImageButtonFactory.FixedWidth = fixedWidth;
		}

		private void SetSyncToPrintVisibility()
		{
			if (windowMode != 0)
			{
				return;
			}
			bool flag = PrinterConnectionAndCommunication.Instance.PrinterIsPaused || PrinterConnectionAndCommunication.Instance.PrinterIsPrinting;
			if (syncToPrint.get_Checked() && flag)
			{
				SetAnimationPosition();
				((GuiWidget)layerRenderRatioSlider).set_Visible(false);
				((GuiWidget)selectLayerSlider).set_Visible(false);
				return;
			}
			if (layerRenderRatioSlider != null)
			{
				layerRenderRatioSlider.FirstValue = 0.0;
				layerRenderRatioSlider.SecondValue = 1.0;
			}
			((GuiWidget)navigationWidget).set_Visible(true);
			((GuiWidget)setLayerWidget).set_Visible(true);
			((GuiWidget)layerRenderRatioSlider).set_Visible(true);
			((GuiWidget)selectLayerSlider).set_Visible(true);
		}

		private void SetLayerViewType()
		{
			if (viewControlsToggle.threeDimensionButton.get_Checked())
			{
				UserSettings.Instance.set("LayerViewDefault", "3D Layer");
				((GuiWidget)viewControls2D).set_Visible(false);
				((GuiWidget)gcodeViewWidget).set_Visible(false);
				((GuiWidget)viewControls3D).set_Visible(true);
				((GuiWidget)meshViewerWidget).set_Visible(true);
			}
			else
			{
				UserSettings.Instance.set("LayerViewDefault", "2D Layer");
				((GuiWidget)viewControls2D).set_Visible(true);
				((GuiWidget)gcodeViewWidget).set_Visible(true);
				((GuiWidget)viewControls3D).set_Visible(false);
				((GuiWidget)meshViewerWidget).set_Visible(false);
			}
		}

		private void HookUpGCodeMessagesWhenDonePrinting(object sender, EventArgs e)
		{
			if (!PrinterConnectionAndCommunication.Instance.PrinterIsPaused && !PrinterConnectionAndCommunication.Instance.PrinterIsPrinting)
			{
				printItem.SlicingOutputMessage -= sliceItem_SlicingOutputMessage;
				printItem.SlicingDone -= sliceItem_Done;
				printItem.SlicingOutputMessage += sliceItem_SlicingOutputMessage;
				printItem.SlicingDone += sliceItem_Done;
				((GuiWidget)generateGCodeButton).set_Visible(true);
			}
			SetSyncToPrintVisibility();
		}

		private GuiWidget CreateGCodeViewWidget(string pathAndFileName)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Expected O, but got Unknown
			gcodeViewWidget = new ViewGcodeWidget(new Vector2(viewerVolume.x, viewerVolume.y), bedCenter);
			gcodeViewWidget.DoneLoading += DoneLoadingGCode;
			ViewGcodeWidget viewGcodeWidget = gcodeViewWidget;
			viewGcodeWidget.LoadingProgressChanged = (ProgressChangedEventHandler)Delegate.Combine((Delegate?)(object)viewGcodeWidget.LoadingProgressChanged, (Delegate?)new ProgressChangedEventHandler(LoadingProgressChanged));
			partToStartLoadingOnFirstDraw = pathAndFileName;
			return (GuiWidget)(object)gcodeViewWidget;
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			bool flag = PrinterConnectionAndCommunication.Instance.PrinterIsPaused || PrinterConnectionAndCommunication.Instance.PrinterIsPrinting;
			if (syncToPrint != null && syncToPrint.get_Checked() && flag)
			{
				SetAnimationPosition();
			}
			EnsureKeyDownHooked();
			if (partToStartLoadingOnFirstDraw != null)
			{
				gcodeViewWidget.LoadInBackground(partToStartLoadingOnFirstDraw);
				partToStartLoadingOnFirstDraw = null;
			}
			base.OnDraw(graphics2D);
		}

		private void EnsureKeyDownHooked()
		{
			if (widgetThatHasKeyDownHooked != null)
			{
				GuiWidget parent = ((GuiWidget)this).get_Parent();
				while (!(parent is SystemWindow))
				{
					parent = parent.get_Parent();
				}
				if (parent != widgetThatHasKeyDownHooked)
				{
					widgetThatHasKeyDownHooked.remove_KeyDown((EventHandler<KeyEventArgs>)Parent_KeyDown);
					widgetThatHasKeyDownHooked = null;
				}
			}
			if (widgetThatHasKeyDownHooked == null)
			{
				GuiWidget parent2 = ((GuiWidget)this).get_Parent();
				while (!(parent2 is SystemWindow))
				{
					parent2 = parent2.get_Parent();
				}
				UnHookWidgetThatHasKeyDownHooked();
				parent2.add_KeyDown((EventHandler<KeyEventArgs>)Parent_KeyDown);
				widgetThatHasKeyDownHooked = parent2;
			}
		}

		private void UnHookWidgetThatHasKeyDownHooked()
		{
			if (widgetThatHasKeyDownHooked != null)
			{
				widgetThatHasKeyDownHooked.remove_KeyDown((EventHandler<KeyEventArgs>)Parent_KeyDown);
			}
		}

		private void Parent_KeyDown(object sender, KeyEventArgs keyEvent)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Invalid comparison between Unknown and I4
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Invalid comparison between Unknown and I4
			if ((int)keyEvent.get_KeyCode() == 38)
			{
				if (gcodeViewWidget != null)
				{
					gcodeViewWidget.ActiveLayerIndex += 1;
				}
			}
			else if ((int)keyEvent.get_KeyCode() == 40 && gcodeViewWidget != null)
			{
				gcodeViewWidget.ActiveLayerIndex -= 1;
			}
		}

		private void SetProcessingMessage(string message)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Expected O, but got Unknown
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Expected O, but got Unknown
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			if (gcodeProcessingStateInfoText == null)
			{
				gcodeProcessingStateInfoText = new TextWidget(message, 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				((GuiWidget)gcodeProcessingStateInfoText).set_HAnchor((HAnchor)2);
				((GuiWidget)gcodeProcessingStateInfoText).set_VAnchor((VAnchor)2);
				gcodeProcessingStateInfoText.set_AutoExpandBoundsToText(true);
				GuiWidget val = new GuiWidget();
				val.AnchorAll();
				val.AddChild((GuiWidget)(object)gcodeProcessingStateInfoText, -1);
				val.set_Selectable(false);
				gcodeDisplayWidget.AddChild(val, -1);
			}
			if (message == "")
			{
				((GuiWidget)gcodeProcessingStateInfoText).set_BackgroundColor(default(RGBA_Bytes));
			}
			else
			{
				((GuiWidget)gcodeProcessingStateInfoText).set_BackgroundColor(RGBA_Bytes.White);
			}
			((GuiWidget)gcodeProcessingStateInfoText).set_Text(message);
		}

		private void LoadingProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			SetProcessingMessage(string.Format("{0} {1}%...", "Loading G-Code".Localize(), e.get_ProgressPercentage()));
		}

		private void CloseIfNotNull(GuiWidget widget)
		{
			if (widget != null)
			{
				widget.Close();
			}
		}

		private static bool RunningIn32Bit()
		{
			if (IntPtr.Size == 4)
			{
				return true;
			}
			return false;
		}

		private void DoneLoadingGCode(object sender, EventArgs e)
		{
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			SetProcessingMessage("");
			if (gcodeViewWidget != null && gcodeViewWidget.LoadedGCode == null)
			{
				if (File.Exists(printItem.FileLocation))
				{
					SetProcessingMessage(string.Format(fileTooBigToLoad, printItem.Name));
				}
				else
				{
					SetProcessingMessage($"{fileNotFoundMessage}\n'{Path.GetFileName(printItem.FileLocation)}'");
				}
			}
			if (gcodeViewWidget != null && gcodeViewWidget.LoadedGCode != null && gcodeViewWidget.LoadedGCode.LineCount > 0)
			{
				CloseIfNotNull((GuiWidget)(object)gradientWidget);
				gradientWidget = new ColorGradientWidget(gcodeViewWidget.LoadedGCode);
				((GuiWidget)this).AddChild((GuiWidget)(object)gradientWidget, -1);
				((GuiWidget)gradientWidget).set_Visible(false);
				CreateOptionsContent();
				setGradientVisibility();
				((GuiWidget)buttonRightPanel).set_Visible(true);
				((GuiWidget)viewControlsToggle).set_Visible(true);
				CloseIfNotNull((GuiWidget)(object)setLayerWidget);
				setLayerWidget = new SetLayerWidget(gcodeViewWidget);
				((GuiWidget)setLayerWidget).set_VAnchor((VAnchor)4);
				((GuiWidget)layerSelectionButtonsPanel).AddChild((GuiWidget)(object)setLayerWidget, -1);
				CloseIfNotNull((GuiWidget)(object)navigationWidget);
				navigationWidget = new LayerNavigationWidget(gcodeViewWidget);
				((GuiWidget)navigationWidget).set_Margin(new BorderDouble(0.0, 0.0, 20.0, 0.0));
				((GuiWidget)layerSelectionButtonsPanel).AddChild((GuiWidget)(object)navigationWidget, -1);
				CloseIfNotNull((GuiWidget)(object)selectLayerSlider);
				selectLayerSlider = new SolidSlider(default(Vector2), sliderWidth, 0.0, gcodeViewWidget.LoadedGCode.NumChangesInZ - 1, (Orientation)1);
				selectLayerSlider.ValueChanged += selectLayerSlider_ValueChanged;
				gcodeViewWidget.ActiveLayerChanged += gcodeViewWidget_ActiveLayerChanged;
				((GuiWidget)this).AddChild((GuiWidget)(object)selectLayerSlider, -1);
				CloseIfNotNull((GuiWidget)(object)layerRenderRatioSlider);
				layerRenderRatioSlider = new DoubleSolidSlider(default(Vector2), sliderWidth, 0.0, 1.0, (Orientation)0);
				layerRenderRatioSlider.FirstValue = 0.0;
				layerRenderRatioSlider.FirstValueChanged += layerStartRenderRatioSlider_ValueChanged;
				layerRenderRatioSlider.SecondValue = 1.0;
				layerRenderRatioSlider.SecondValueChanged += layerEndRenderRatioSlider_ValueChanged;
				((GuiWidget)this).AddChild((GuiWidget)(object)layerRenderRatioSlider, -1);
				SetSliderSizes();
				gcodeViewWidget.ActiveLayerIndex += 1;
				gcodeViewWidget.ActiveLayerIndex -= 1;
				((GuiWidget)this).add_BoundsChanged((EventHandler)PartPreviewGCode_BoundsChanged);
				((GuiWidget)meshViewerWidget.partProcessingInfo).set_Visible(false);
			}
		}

		private void setGradientVisibility()
		{
			if (showSpeeds.get_Checked())
			{
				((GuiWidget)gradientWidget).set_Visible(true);
			}
			else
			{
				((GuiWidget)gradientWidget).set_Visible(false);
			}
		}

		private void layerStartRenderRatioSlider_ValueChanged(object sender, EventArgs e)
		{
			gcodeViewWidget.FeatureToStartOnRatio0To1 = layerRenderRatioSlider.FirstValue;
			gcodeViewWidget.FeatureToEndOnRatio0To1 = layerRenderRatioSlider.SecondValue;
			((GuiWidget)gcodeViewWidget).Invalidate();
		}

		private void layerEndRenderRatioSlider_ValueChanged(object sender, EventArgs e)
		{
			gcodeViewWidget.FeatureToStartOnRatio0To1 = layerRenderRatioSlider.FirstValue;
			gcodeViewWidget.FeatureToEndOnRatio0To1 = layerRenderRatioSlider.SecondValue;
			((GuiWidget)gcodeViewWidget).Invalidate();
		}

		private void gcodeViewWidget_ActiveLayerChanged(object sender, EventArgs e)
		{
			if (gcodeViewWidget.ActiveLayerIndex != (int)(selectLayerSlider.Value + 0.5))
			{
				selectLayerSlider.Value = gcodeViewWidget.ActiveLayerIndex;
			}
		}

		private void selectLayerSlider_ValueChanged(object sender, EventArgs e)
		{
			gcodeViewWidget.ActiveLayerIndex = (int)(selectLayerSlider.Value + 0.5);
		}

		private void PartPreviewGCode_BoundsChanged(object sender, EventArgs e)
		{
			SetSliderSizes();
		}

		private void SetSliderSizes()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)selectLayerSlider).set_OriginRelativeParent(new Vector2(gcodeDisplayWidget.get_Width() - 20.0, 70.0));
			selectLayerSlider.TotalWidthInPixels = gcodeDisplayWidget.get_Height() - 80.0;
			((GuiWidget)layerRenderRatioSlider).set_OriginRelativeParent(new Vector2(60.0, 70.0));
			layerRenderRatioSlider.TotalWidthInPixels = gcodeDisplayWidget.get_Width() - 100.0;
		}

		private void AddHandlers()
		{
			expandModelOptions.add_CheckedStateChanged((EventHandler)expandModelOptions_CheckedStateChanged);
			expandDisplayOptions.add_CheckedStateChanged((EventHandler)expandDisplayOptions_CheckedStateChanged);
		}

		private void expandModelOptions_CheckedStateChanged(object sender, EventArgs e)
		{
			bool @checked;
			((GuiWidget)modelOptionsContainer).set_Visible(@checked = expandModelOptions.get_Checked());
			if (@checked)
			{
				if (expandModelOptions.get_Checked())
				{
					expandDisplayOptions.set_Checked(false);
				}
				((GuiWidget)modelOptionsContainer).set_Visible(expandModelOptions.get_Checked());
			}
		}

		private void expandDisplayOptions_CheckedStateChanged(object sender, EventArgs e)
		{
			if (((GuiWidget)displayOptionsContainer).get_Visible() != expandDisplayOptions.get_Checked())
			{
				if (expandDisplayOptions.get_Checked())
				{
					expandModelOptions.set_Checked(false);
				}
				((GuiWidget)displayOptionsContainer).set_Visible(expandDisplayOptions.get_Checked());
			}
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			UnHookWidgetThatHasKeyDownHooked();
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			if (printItem != null)
			{
				printItem.SlicingOutputMessage -= sliceItem_SlicingOutputMessage;
				printItem.SlicingDone -= sliceItem_Done;
				if (startedSliceFromGenerateButton && printItem.CurrentlySlicing)
				{
					SlicingQueue.Instance.CancelCurrentSlicing();
				}
			}
			base.OnClosed(e);
		}

		private bool PauseIfNecessary()
		{
			bool doPrintSelected = true;
			if (PrinterConnectionAndCommunication.Instance.PrinterIsPrinting)
			{
				string englishString = "Print Must Be Paused";
				string englishString2 = "The print must be paused to use replay.\n\nPause the print?";
				StyledMessageBox.ShowMessageBox(delegate(bool shouldPause)
				{
					doPrintSelected = shouldPause;
					if (shouldPause)
					{
						PrinterConnectionAndCommunication.Instance.RequestPause();
					}
				}, englishString2.Localize(), englishString.Localize(), StyledMessageBox.MessageType.YES_NO);
			}
			return doPrintSelected;
		}

		private void RewindButton_Click(object sender, EventArgs e)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			if (PauseIfNecessary())
			{
				RewindWidget rewindWidget = new RewindWidget(gcodeViewWidget.LoadedGCode, gcodeViewWidget.GCodeSelectionInfo);
				ModalTermWithTab rewindWindow = new ModalTermWithTab((GuiWidget)(object)rewindWidget, "Rewind Configuration".Localize(), new Vector2(400.0, 264.0));
				((GuiWidget)rewindWidget).add_Closed((EventHandler<ClosedEventArgs>)delegate
				{
					((GuiWidget)rewindWindow).Close();
				});
				((SystemWindow)rewindWindow).ShowAsSystemWindow();
			}
		}

		private void PrintSelectedButton_Click(object sender, EventArgs e)
		{
			if (PauseIfNecessary())
			{
				((SystemWindow)new ModalTermWithTab((GuiWidget)(object)new ReplayWidget(gcodeViewWidget.LoadedGCode, new List<int>((IEnumerable<int>)gcodeViewWidget.SelectedFeatureInstructionIndices)), "Replay Configuration".Localize())).ShowAsSystemWindow();
			}
		}

		private void generateButton_Click(object sender, EventArgs mouseEvent)
		{
			UiThread.RunOnIdle((Action<object>)DoGenerateButton_Click, sender, 0.0);
		}

		private void DoGenerateButton_Click(object state)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if (PrinterConnectionAndCommunication.Instance.ActivePrinter != null)
			{
				if (ActiveSliceSettings.Instance.IsValid() && printItem != null)
				{
					((GuiWidget)(Button)state).set_Visible(false);
					SlicingQueue.Instance.QueuePartForSlicing(printItem);
					startedSliceFromGenerateButton = true;
				}
			}
			else
			{
				StyledMessageBox.ShowMessageBox(null, "Oops! Please select a printer in order to continue slicing.", "Select Printer");
			}
		}

		private void sliceItem_SlicingOutputMessage(object sender, EventArgs e)
		{
			StringEventArgs val = e as StringEventArgs;
			if (val != null && val.get_Data() != null)
			{
				SetProcessingMessage(val.get_Data());
			}
			else
			{
				SetProcessingMessage("");
			}
		}

		private void sliceItem_Done(object sender, EventArgs e)
		{
			printItem.SlicingOutputMessage -= sliceItem_SlicingOutputMessage;
			printItem.SlicingDone -= sliceItem_Done;
			UiThread.RunOnIdle((Action)CreateAndAddChildren);
			startedSliceFromGenerateButton = false;
		}
	}
}
