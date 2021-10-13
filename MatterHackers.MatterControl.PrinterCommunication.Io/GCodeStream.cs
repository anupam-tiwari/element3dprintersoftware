using System;
using System.Text;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrinterCommunication.Io
{
	public abstract class GCodeStream : IDisposable
	{
		private bool useG0ForMovement;

		public abstract string ReadLine();

		public abstract void SetPrinterPosition(PrinterMove position);

		public abstract void Dispose();

		public GCodeStream()
		{
			useG0ForMovement = ActiveSliceSettings.Instance.GetValue<bool>("g0");
		}

		public string CreateMovementLine(PrinterMove currentDestination)
		{
			return CreateMovementLine(currentDestination, PrinterMove.Nowhere);
		}

		public string CreateMovementLine(PrinterMove destination, PrinterMove start)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			bool flag = destination.extrusion != start.extrusion;
			StringBuilder stringBuilder = new StringBuilder((useG0ForMovement && !flag) ? "G0 " : "G1 ");
			if (destination.position.x != start.position.x)
			{
				stringBuilder.AppendFormat("X{0:0.##} ", destination.position.x);
			}
			if (destination.position.y != start.position.y)
			{
				stringBuilder.AppendFormat("Y{0:0.##} ", destination.position.y);
			}
			if (destination.position.z != start.position.z)
			{
				stringBuilder.AppendFormat("Z{0:0.###} ", destination.position.z);
			}
			if (flag)
			{
				stringBuilder.AppendFormat("E{0:0.###} ", destination.extrusion);
			}
			if (destination.feedRate != start.feedRate)
			{
				stringBuilder.AppendFormat("F{0:0.##}", destination.feedRate);
			}
			return stringBuilder.ToString().Trim();
		}

		public static PrinterMove GetPosition(string lineBeingSent, PrinterMove startPositionPosition)
		{
			PrinterMove result = startPositionPosition;
			GCodeFile.GetFirstNumberAfter("X", lineBeingSent, ref result.position.x);
			GCodeFile.GetFirstNumberAfter("Y", lineBeingSent, ref result.position.y);
			GCodeFile.GetFirstNumberAfter("Z", lineBeingSent, ref result.position.z);
			GCodeFile.GetFirstNumberAfter("E", lineBeingSent, ref result.extrusion);
			GCodeFile.GetFirstNumberAfter("F", lineBeingSent, ref result.feedRate);
			return result;
		}

		public static bool LineIsMovement(string lineBeingSent)
		{
			if (lineBeingSent.StartsWith("G0 ") || lineBeingSent.StartsWith("G1 "))
			{
				return true;
			}
			return false;
		}
	}
}
