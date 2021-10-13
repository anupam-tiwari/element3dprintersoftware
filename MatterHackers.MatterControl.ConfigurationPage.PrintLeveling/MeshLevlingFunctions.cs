using System;
using System.Collections.Generic;
using System.Text;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class MeshLevlingFunctions : IDisposable
	{
		public class Region
		{
			public Vector3 LeftBottom
			{
				get;
				set;
			}

			public Vector3 LeftTop
			{
				get;
				set;
			}

			public Vector3 RightBottom
			{
				get;
				set;
			}

			public Vector3 RightTop
			{
				get;
				set;
			}

			internal Vector3 Center
			{
				get;
				private set;
			}

			internal Vector3 LeftBottomCenter
			{
				get;
				private set;
			}

			internal Vector3 RightTopCenter
			{
				get;
				private set;
			}

			internal Plane LeftBottomPlane
			{
				get;
				private set;
			}

			internal Plane RightTopPlane
			{
				get;
				private set;
			}

			internal Vector3 GetPositionWithZOffset(Vector3 currentDestination)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				Plane val = LeftBottomPlane;
				if (((Plane)(ref val)).get_PlaneNormal() == Vector3.Zero)
				{
					InitializePlanes();
				}
				Vector3 val2 = default(Vector3);
				((Vector3)(ref val2))._002Ector(currentDestination.x, currentDestination.y, 0.0);
				Vector3 val3 = LeftBottomCenter - val2;
				double lengthSquared = ((Vector3)(ref val3)).get_LengthSquared();
				val3 = RightTopCenter - val2;
				if (lengthSquared < ((Vector3)(ref val3)).get_LengthSquared())
				{
					val = LeftBottomPlane;
					double distanceToIntersection = ((Plane)(ref val)).GetDistanceToIntersection(val2, Vector3.UnitZ);
					currentDestination.z += distanceToIntersection;
				}
				else
				{
					val = RightTopPlane;
					double distanceToIntersection2 = ((Plane)(ref val)).GetDistanceToIntersection(val2, Vector3.UnitZ);
					currentDestination.z += distanceToIntersection2;
				}
				return currentDestination;
			}

			private void InitializePlanes()
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				LeftBottomPlane = new Plane(LeftBottom, RightBottom, LeftTop);
				LeftBottomCenter = (LeftBottom + RightBottom + LeftTop) / 3.0;
				RightTopPlane = new Plane(RightBottom, RightTop, LeftTop);
				RightTopCenter = (RightBottom + RightTop + LeftTop) / 3.0;
				Center = (LeftBottomCenter + RightTopCenter) / 2.0;
			}
		}

		private Vector3 lastDestinationWithLevelingApplied;

		private EventHandler unregisterEvents;

		public List<Vector3> SampledPositions
		{
			get;
			private set;
		}

		public List<Region> Regions
		{
			get;
			private set;
		} = new List<Region>();


		public MeshLevlingFunctions(int gridWidth, int gridHeight, PrintLevelingData levelingData)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			SampledPositions = new List<Vector3>(levelingData.SampledPositions);
			PrinterConnectionAndCommunication.Instance.PositionRead.RegisterEvent((EventHandler)PrinterReportedPosition, ref unregisterEvents);
			for (int i = 0; i < gridHeight - 1; i++)
			{
				for (int j = 0; j < gridWidth - 1; j++)
				{
					Regions.Add(new Region
					{
						LeftBottom = levelingData.SampledPositions[i * gridWidth + j],
						RightBottom = levelingData.SampledPositions[i * gridWidth + j + 1],
						LeftTop = levelingData.SampledPositions[(i + 1) * gridWidth + j],
						RightTop = levelingData.SampledPositions[(i + 1) * gridWidth + j + 1]
					});
				}
			}
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
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return GetCorrectRegion(currentDestination).GetPositionWithZOffset(currentDestination);
		}

		public Vector2 GetPrintLevelPositionToSample(int index, int gridWidth, int gridHeight)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			List<Vector2> manualPositions = LevelWizardBase.GetManualPositions(ActiveSliceSettings.Instance.GetValue("leveling_manual_positions"), gridWidth * gridHeight);
			if (manualPositions != null)
			{
				return manualPositions[index];
			}
			Vector2 value = ActiveSliceSettings.Instance.GetValue<Vector2>("bed_size");
			Vector2 value2 = ActiveSliceSettings.Instance.GetValue<Vector2>("print_center");
			BedShape value3 = ActiveSliceSettings.Instance.GetValue<BedShape>("bed_shape");
			if (value3 != 0 && value3 == BedShape.Circular)
			{
				Vector2 val = default(Vector2);
				((Vector2)(ref val))._002Ector(value2.x, value2.y + value.y / 2.0 * 0.5);
				return (Vector2)(index switch
				{
					0 => val, 
					1 => Vector2.Rotate(val, 2.094395160675049), 
					2 => Vector2.Rotate(val, 4.188790321350098), 
					_ => throw new IndexOutOfRangeException(), 
				});
			}
			return (Vector2)(index switch
			{
				0 => new Vector2(value2.x, value2.y + value.y / 2.0 * 0.8), 
				1 => new Vector2(value2.x - value.x / 2.0 * 0.8, value2.y - value.y / 2.0 * 0.8), 
				2 => new Vector2(value2.x + value.x / 2.0 * 0.8, value2.y - value.y / 2.0 * 0.8), 
				_ => throw new IndexOutOfRangeException(), 
			});
		}

		private Region GetCorrectRegion(Vector3 currentDestination)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			int index = 0;
			double num = double.PositiveInfinity;
			currentDestination.z = 0.0;
			for (int i = 0; i < Regions.Count; i++)
			{
				Vector3 val = Regions[i].Center - currentDestination;
				double lengthSquared = ((Vector3)(ref val)).get_LengthSquared();
				if (lengthSquared < num)
				{
					index = i;
					num = lengthSquared;
				}
			}
			return Regions[index];
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
