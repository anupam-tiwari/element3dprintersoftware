using MatterHackers.Agg;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.PrinterControls;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PrinterCommunication.Io
{
	internal class PrintRecoveryStream : GCodeStream
	{
		private enum RecoveryState
		{
			RemoveHeating,
			Raising,
			Homing,
			FindingRecoveryLayer,
			SkippingGCode,
			PrimingAndMovingToStart,
			PrintingSlow,
			PrintingToEnd
		}

		private GCodeFileStream internalStream;

		private double percentDone;

		private double recoverFeedRate;

		private PrinterMove lastDestination;

		private QueuedCommandsStream queuedCommands;

		private RectangleDouble boundsOfSkippedLayers = RectangleDouble.ZeroIntersection;

		private RecoveryState recoveryState;

		public PrintRecoveryStream(GCodeFileStream internalStream, double percentDone)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			this.internalStream = internalStream;
			this.percentDone = percentDone;
			recoverFeedRate = ActiveSliceSettings.Instance.GetValue<double>("recover_first_layer_speed");
			if (recoverFeedRate == 0.0)
			{
				recoverFeedRate = 10.0;
			}
			recoverFeedRate *= 60.0;
			queuedCommands = new QueuedCommandsStream(null);
		}

		public override void Dispose()
		{
		}

		public override void SetPrinterPosition(PrinterMove position)
		{
			lastDestination = position;
			internalStream.SetPrinterPosition(lastDestination);
		}

		public override string ReadLine()
		{
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			string text = queuedCommands.ReadLine();
			if (text != null)
			{
				return text;
			}
			switch (recoveryState)
			{
			case RecoveryState.RemoveHeating:
			{
				queuedCommands.Add("G21; set units to millimeters");
				queuedCommands.Add("M107; fan off");
				queuedCommands.Add("T0; set the active extruder to 0");
				queuedCommands.Add("G90; use absolute coordinates");
				queuedCommands.Add("G92 E0; reset the expected extruder position");
				queuedCommands.Add("M82; use absolute distance for extrusion");
				bool value4 = ActiveSliceSettings.Instance.GetValue<bool>("has_heated_bed");
				double value5 = ActiveSliceSettings.Instance.GetValue<double>("bed_temperature");
				if (value4 && value5 > 0.0)
				{
					queuedCommands.Add($"M140 S{value5}");
				}
				queuedCommands.Add(StringHelper.FormatWith("M109 S{0}", new object[1]
				{
					ActiveSliceSettings.Instance.Helpers.ExtruderTemperature(0)
				}));
				if (value4 && value5 > 0.0)
				{
					queuedCommands.Add($"M190 S{value5}");
				}
				recoveryState = RecoveryState.Raising;
				return "";
			}
			case RecoveryState.Raising:
				PrintLevelingStream.Enabled = false;
				queuedCommands.Add("M114 ; get current position");
				queuedCommands.Add("G91 ; move relative");
				queuedCommands.Add(StringHelper.FormatWith("G1 Z10 F{0}", new object[1]
				{
					MovementControls.ZSpeed
				}));
				queuedCommands.Add("G90 ; move absolute");
				recoveryState = RecoveryState.Homing;
				return "";
			case RecoveryState.Homing:
				if (ActiveSliceSettings.Instance.GetValue<bool>("z_homes_to_max"))
				{
					queuedCommands.Add("G28");
				}
				else
				{
					queuedCommands.Add("G28 X0");
					queuedCommands.Add("G28 Y0");
					Vector2 value3 = ActiveSliceSettings.Instance.GetValue<Vector2>("recover_position_before_z_home");
					queuedCommands.Add(StringHelper.FormatWith("G1 X{0:0.###}Y{1:0.###}F{2}", new object[3]
					{
						value3.x,
						value3.y,
						MovementControls.XSpeed
					}));
					queuedCommands.Add("G28 Z0");
				}
				PrintLevelingStream.Enabled = true;
				recoveryState = RecoveryState.FindingRecoveryLayer;
				return "";
			case RecoveryState.FindingRecoveryLayer:
				recoveryState = RecoveryState.SkippingGCode;
				goto case RecoveryState.SkippingGCode;
			case RecoveryState.SkippingGCode:
			{
				int num = 0;
				boundsOfSkippedLayers = RectangleDouble.ZeroIntersection;
				while (internalStream.FileStreaming.PercentComplete(internalStream.LineIndex) < percentDone)
				{
					string text3 = internalStream.ReadLine();
					if (text3 == null)
					{
						break;
					}
					num++;
					if (text3.Contains(";"))
					{
						text3 = text3.Split(new char[1]
						{
							';'
						})[0];
					}
					lastDestination = GCodeStream.GetPosition(text3, lastDestination);
					if (num > 100)
					{
						((RectangleDouble)(ref boundsOfSkippedLayers)).ExpandToInclude(((Vector3)(ref lastDestination.position)).get_Xy());
						_ = boundsOfSkippedLayers.Bottom;
						_ = 10.0;
					}
					if (text3.StartsWith("M109") || text3.StartsWith("M104") || text3.StartsWith("M190") || text3.StartsWith("M140") || text3.StartsWith("T") || text3.StartsWith("M106") || text3.StartsWith("M107") || text3.StartsWith("G92"))
					{
						return text3;
					}
				}
				recoveryState = RecoveryState.PrimingAndMovingToStart;
				((RectangleDouble)(ref boundsOfSkippedLayers)).ExpandToInclude(((Vector3)(ref lastDestination.position)).get_Xy());
				return "";
			}
			case RecoveryState.PrimingAndMovingToStart:
			{
				if (ActiveSliceSettings.Instance.GetValue("z_homes_to_max") == "0")
				{
					Vector2 value = ActiveSliceSettings.Instance.GetValue<Vector2>("recover_position_before_z_home");
					queuedCommands.Add(CreateMovementLine(new PrinterMove(new Vector3(value.x, value.y, lastDestination.position.z), 0.0, MovementControls.ZSpeed)));
				}
				double value2 = ActiveSliceSettings.Instance.GetValue<double>("nozzle_diameter");
				queuedCommands.Add(CreateMovementLine(new PrinterMove(new Vector3(boundsOfSkippedLayers.Left - value2 * 2.0, boundsOfSkippedLayers.Bottom + ((RectangleDouble)(ref boundsOfSkippedLayers)).get_Height() / 2.0, lastDestination.position.z), 0.0, MovementControls.XSpeed)));
				queuedCommands.Add(StringHelper.FormatWith("G1 E10 F{0}", new object[1]
				{
					MovementControls.EFeedRate(0)
				}));
				queuedCommands.Add("G1 E9");
				queuedCommands.Add(CreateMovementLine(new PrinterMove(lastDestination.position, 0.0, MovementControls.XSpeed)));
				queuedCommands.Add(StringHelper.FormatWith("G92 E{0}", new object[1]
				{
					lastDestination.extrusion
				}));
				recoveryState = RecoveryState.PrintingSlow;
				return "";
			}
			case RecoveryState.PrintingSlow:
			{
				string text2 = internalStream.ReadLine();
				if (text2 == null)
				{
					return null;
				}
				if (!GCodeFile.IsLayerChange(text2))
				{
					if (GCodeStream.LineIsMovement(text2))
					{
						PrinterMove position = GCodeStream.GetPosition(text2, lastDestination);
						PrinterMove destination = position;
						destination.feedRate = recoverFeedRate;
						text2 = CreateMovementLine(destination, lastDestination);
						lastDestination = position;
						return text2;
					}
					return text2;
				}
				recoveryState = RecoveryState.PrintingToEnd;
				return "";
			}
			case RecoveryState.PrintingToEnd:
				return internalStream.ReadLine();
			default:
				return null;
			}
		}
	}
}
