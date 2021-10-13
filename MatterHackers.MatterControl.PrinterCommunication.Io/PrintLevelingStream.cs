using System;
using MatterHackers.MatterControl.ConfigurationPage.PrintLeveling;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrinterCommunication.Io
{
	public class PrintLevelingStream : GCodeStreamProxy
	{
		private bool activePrinting;

		protected PrinterMove lastDestination;

		public PrinterMove LastDestination => lastDestination;

		public static bool Enabled
		{
			get;
			set;
		} = true;


		public PrintLevelingStream(GCodeStream internalStream, bool activePrinting)
			: base(internalStream)
		{
			this.activePrinting = activePrinting;
		}

		public override string ReadLine()
		{
			string text = base.ReadLine();
			if (text != null && Enabled && PrinterConnectionAndCommunication.Instance.ActivePrinter.GetValue<bool>("print_leveling_enabled") && !PrinterConnectionAndCommunication.Instance.ActivePrinter.GetValue<bool>("has_hardware_leveling"))
			{
				if (GCodeStream.LineIsMovement(text))
				{
					PrinterMove position = GCodeStream.GetPosition(text, lastDestination);
					text = RunPrintLevelingTranslations(text, position);
					lastDestination = position;
					return text;
				}
				if (text.StartsWith("G29"))
				{
					text = base.ReadLine();
				}
			}
			return text;
		}

		public override void SetPrinterPosition(PrinterMove position)
		{
			string lineBeingSent = CreateMovementLine(position);
			PrinterMove right = GCodeStream.GetPosition(RunPrintLevelingTranslations(lineBeingSent, position), PrinterMove.Nowhere) - position;
			PrinterMove printerMove = (lastDestination = position - right);
			lastDestination.extrusion = position.extrusion;
			lastDestination.feedRate = position.feedRate;
			internalStream.SetPrinterPosition(lastDestination);
		}

		private string RunPrintLevelingTranslations(string lineBeingSent, PrinterMove currentDestination)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			PrintLevelingData printLevelingData = ActiveSliceSettings.Instance.Helpers.GetPrintLevelingData();
			if (printLevelingData != null)
			{
				lineBeingSent = printLevelingData.CurrentPrinterLevelingSystem switch
				{
					PrintLevelingData.LevelingSystem.Probe3Points => LevelWizard3Point.ApplyLeveling(lineBeingSent, currentDestination.position, PrinterMachineInstruction.MovementTypes.Absolute), 
					PrintLevelingData.LevelingSystem.Probe7PointRadial => LevelWizard7PointRadial.ApplyLeveling(lineBeingSent, currentDestination.position, PrinterMachineInstruction.MovementTypes.Absolute), 
					PrintLevelingData.LevelingSystem.Probe13PointRadial => LevelWizard13PointRadial.ApplyLeveling(lineBeingSent, currentDestination.position, PrinterMachineInstruction.MovementTypes.Absolute), 
					PrintLevelingData.LevelingSystem.Probe3x3Mesh => LevelWizard3x3Mesh.ApplyLeveling(lineBeingSent, currentDestination.position, PrinterMachineInstruction.MovementTypes.Absolute), 
					_ => throw new NotImplementedException(), 
				};
			}
			return lineBeingSent;
		}
	}
}
