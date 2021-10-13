using System;
using System.Collections.Generic;
using System.Text;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class RadialLevlingFunctions : IDisposable
	{
		private Vector3 lastDestinationWithLevelingApplied;

		private EventHandler unregisterEvents;

		public Vector2 BedCenter
		{
			get;
			set;
		}

		public List<Vector3> SampledPositions
		{
			get;
			private set;
		}

		public int NumberOfRadialSamples
		{
			get;
			set;
		}

		public RadialLevlingFunctions(int numberOfRadialSamples, PrintLevelingData levelingData, Vector2 bedCenter)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			SampledPositions = new List<Vector3>(levelingData.SampledPositions);
			BedCenter = bedCenter;
			NumberOfRadialSamples = numberOfRadialSamples;
			PrinterConnectionAndCommunication.Instance.PositionRead.RegisterEvent((EventHandler)PrinterReportedPosition, ref unregisterEvents);
		}

		public void Dispose()
		{
			unregisterEvents?.Invoke(this, null);
		}

		public string DoApplyLeveling(string lineBeingSent, Vector3 currentDestination, PrinterMachineInstruction.MovementTypes movementMode)
		{
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			double readValue = 0.0;
			GCodeFile.GetFirstNumberAfter("E", lineBeingSent, ref readValue);
			double readValue2 = 0.0;
			GCodeFile.GetFirstNumberAfter("F", lineBeingSent, ref readValue2);
			StringBuilder stringBuilder = new StringBuilder("G1 ");
			if (lineBeingSent.Contains("X") || lineBeingSent.Contains("Y") || lineBeingSent.Contains("Z"))
			{
				Vector3 val = GetPositionWithZOffset(currentDestination);
				if (movementMode == PrinterMachineInstruction.MovementTypes.Relative)
				{
					Vector3 val2 = val - lastDestinationWithLevelingApplied;
					lastDestinationWithLevelingApplied = val;
					val = val2;
				}
				else
				{
					lastDestinationWithLevelingApplied = val;
				}
				stringBuilder = stringBuilder.Append($"X{val.x:0.##} Y{val.y:0.##} Z{val.z:0.###}");
			}
			if (readValue != 0.0)
			{
				stringBuilder = stringBuilder.Append($" E{readValue:0.###}");
			}
			if (readValue2 != 0.0)
			{
				stringBuilder = stringBuilder.Append($" F{readValue2:0.##}");
			}
			lineBeingSent = stringBuilder.ToString();
			return lineBeingSent;
		}

		public Vector3 GetPositionWithZOffset(Vector3 currentDestination)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			if (SampledPositions.Count == NumberOfRadialSamples + 1)
			{
				Vector2 val = new Vector2(currentDestination) - BedCenter;
				double num = Math.Atan2(val.y, val.x);
				if (num < 0.0)
				{
					num += 6.2831854820251465;
				}
				double num2 = 6.2831854820251465 / (double)NumberOfRadialSamples;
				int num3 = (int)(num / num2);
				int num4 = num3 + 1;
				if (num4 == NumberOfRadialSamples)
				{
					num4 = 0;
				}
				Plane val2 = default(Plane);
				((Plane)(ref val2))._002Ector(SampledPositions[num3], SampledPositions[num4], SampledPositions[NumberOfRadialSamples]);
				double distanceToIntersection = ((Plane)(ref val2)).GetDistanceToIntersection(new Vector3(currentDestination.x, currentDestination.y, 0.0), Vector3.UnitZ);
				currentDestination.z += distanceToIntersection;
			}
			return currentDestination;
		}

		public Vector2 GetPrintLevelPositionToSample(int index, double radius)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			Vector2 value = ActiveSliceSettings.Instance.GetValue<Vector2>("print_center");
			if (index < NumberOfRadialSamples)
			{
				Vector2 val = default(Vector2);
				((Vector2)(ref val))._002Ector(radius, 0.0);
				((Vector2)(ref val)).Rotate(6.2831854820251465 / (double)NumberOfRadialSamples * (double)index);
				val += value;
				return val;
			}
			return value;
		}

		private void PrinterReportedPosition(object sender, EventArgs e)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			lastDestinationWithLevelingApplied = GetPositionWithZOffset(PrinterConnectionAndCommunication.Instance.LastReportedPosition);
		}
	}
}
