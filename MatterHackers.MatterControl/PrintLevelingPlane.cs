using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class PrintLevelingPlane
	{
		private Matrix4X4 bedLevelMatrix = Matrix4X4.Identity;

		private static PrintLevelingPlane instance;

		public static PrintLevelingPlane Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new PrintLevelingPlane();
				}
				return instance;
			}
		}

		private PrintLevelingPlane()
		{
		}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)


		public void SetPrintLevelingEquation(Vector3 position0, Vector3 position1, Vector3 position2, Vector2 bedCenter)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected O, but got Unknown
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			if (!(position0 == position1) && !(position1 == position2) && !(position2 == position0))
			{
				Plane val = default(Plane);
				((Plane)(ref val))._002Ector(position0, position1, position2);
				Ray val2 = new Ray(new Vector3(bedCenter, 0.0), Vector3.UnitZ, 0.0, double.PositiveInfinity, (IntersectionType)1);
				bool flag = default(bool);
				double distanceToIntersection = ((Plane)(ref val)).GetDistanceToIntersection(val2, ref flag);
				Matrix4X4 val3 = Matrix4X4.CreateTranslation(0.0 - bedCenter.x, 0.0 - bedCenter.y, 0.0 - distanceToIntersection);
				val3 *= Matrix4X4.CreateRotation(((Plane)(ref val)).get_PlaneNormal(), Vector3.UnitZ);
				val3 *= Matrix4X4.CreateTranslation(bedCenter.x, bedCenter.y, 0.0);
				bedLevelMatrix = Matrix4X4.Invert(val3);
			}
		}

		public Vector3 ApplyLeveling(Vector3 inPosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.TransformPosition(inPosition, bedLevelMatrix);
		}

		public Vector3 ApplyLevelingRotation(Vector3 inPosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.TransformVector(inPosition, bedLevelMatrix);
		}

		public string ApplyLeveling(Vector3 currentDestination, PrinterMachineInstruction.MovementTypes movementMode, string lineBeingSent)
		{
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			if ((lineBeingSent.StartsWith("G0") || lineBeingSent.StartsWith("G1")) && lineBeingSent.Length > 2 && lineBeingSent[2] == ' ')
			{
				double readValue = 0.0;
				GCodeFile.GetFirstNumberAfter("E", lineBeingSent, ref readValue);
				double readValue2 = 0.0;
				GCodeFile.GetFirstNumberAfter("F", lineBeingSent, ref readValue2);
				string text = "G1 ";
				if (lineBeingSent.Contains("X") || lineBeingSent.Contains("Y") || lineBeingSent.Contains("Z"))
				{
					Vector3 val = Instance.ApplyLeveling(currentDestination);
					if (movementMode == PrinterMachineInstruction.MovementTypes.Relative)
					{
						Vector3 zero = Vector3.Zero;
						GCodeFile.GetFirstNumberAfter("X", lineBeingSent, ref zero.x);
						GCodeFile.GetFirstNumberAfter("Y", lineBeingSent, ref zero.y);
						GCodeFile.GetFirstNumberAfter("Z", lineBeingSent, ref zero.z);
						val = Instance.ApplyLevelingRotation(zero);
					}
					text += $"X{val.x:0.##} Y{val.y:0.##} Z{val.z:0.###}";
				}
				if (readValue != 0.0)
				{
					text += $" E{readValue:0.###}";
				}
				if (readValue2 != 0.0)
				{
					text += $" F{readValue2:0.##}";
				}
				lineBeingSent = text;
			}
			return lineBeingSent;
		}
	}
}
