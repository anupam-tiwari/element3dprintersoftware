using System;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.MatterSlice;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.GCodeVisualizer
{
	public abstract class GCodeFile
	{
		private static readonly Vector4 MaxAccelerationMmPerS2 = new Vector4(1000.0, 1000.0, 100.0, 5000.0);

		private static readonly Vector4 MaxVelocityMmPerS = new Vector4(500.0, 500.0, 5.0, 25.0);

		private static readonly Vector4 VelocitySameAsStopMmPerS = new Vector4(8.0, 8.0, 0.4, 5.0);

		protected const int Max32BitFileSize = 100000000;

		public abstract int LineCount
		{
			get;
		}

		public abstract int NumChangesInZ
		{
			get;
		}

		public abstract double TotalSecondsInPrint
		{
			get;
		}

		public abstract Tools PrintTools
		{
			get;
		}

		public static void AssertDebugNotDefined()
		{
		}

		public abstract void Clear();

		public abstract RectangleDouble GetBounds();

		public abstract double GetFilamentCubicMm(double filamentDiameter);

		public abstract double GetFilamentDiameter();

		public abstract double GetFilamentUsedMm(double filamentDiameter);

		public abstract double GetFilamentWeightGrams(double filamentDiameterMm, double density);

		public abstract double GetFirstLayerHeight();

		public abstract int GetInstructionIndexAtLayer(int layerIndex);

		public abstract double GetLayerHeight();

		public abstract int GetLayerIndex(int instructionIndex);

		public abstract Vector2 GetWeightedCenter();

		public abstract PrinterMachineInstruction Instruction(int i);

		public abstract bool IsExtruding(int instructionIndexToCheck);

		public abstract double PercentComplete(int instructionIndex);

		public abstract double Ratio0to1IntoContainedLayer(int instructionIndex);

		public static int CalculateChecksum(string commandToGetChecksumFor)
		{
			int num = 0;
			if (commandToGetChecksumFor.Length > 0)
			{
				num = commandToGetChecksumFor[0];
				for (int i = 1; i < commandToGetChecksumFor.Length; i++)
				{
					num ^= commandToGetChecksumFor[i];
				}
			}
			return num;
		}

		public static bool IsLayerChange(string lineString)
		{
			if (!lineString.StartsWith("; LAYER:"))
			{
				return lineString.StartsWith(";LAYER:");
			}
			return true;
		}

		public static bool FileTooBigToLoad(string fileName)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			if (File.Exists(fileName) && RunningIn32Bit() && new FileInfo(fileName).get_Length() > 100000000)
			{
				return true;
			}
			return false;
		}

		public static bool GetFirstNumberAfter(string stringToCheckAfter, string stringWithNumber, ref int readValue, int startIndex = 0, string stopCheckingString = ";")
		{
			double readValue2 = readValue;
			if (GetFirstNumberAfter(stringToCheckAfter, stringWithNumber, ref readValue2, startIndex, stopCheckingString))
			{
				readValue = (int)readValue2;
				return true;
			}
			return false;
		}

		public static bool GetFirstNumberAfter(string stringToCheckAfter, string stringWithNumber, ref double readValue, int startIndex = 0, string stopCheckingString = ";")
		{
			int num = stringWithNumber.IndexOf(stringToCheckAfter, startIndex);
			int num2 = stringWithNumber.IndexOf(stopCheckingString);
			if (num != -1 && (num2 == -1 || num < num2))
			{
				num += stringToCheckAfter.Length;
				readValue = agg_basics.ParseDouble(stringWithNumber, ref num, true);
				return true;
			}
			return false;
		}

		public static bool GetFirstStringAfter(string stringToCheckAfter, string fullStringToLookIn, string separatorString, ref string nextString, int startIndex = 0)
		{
			int num = fullStringToLookIn.IndexOf(stringToCheckAfter, startIndex);
			if (num != -1)
			{
				int num2 = fullStringToLookIn.IndexOf(separatorString, num);
				if (num2 != -1)
				{
					nextString = fullStringToLookIn.Substring(num + stringToCheckAfter.Length, num2 - (num + stringToCheckAfter.Length));
					return true;
				}
			}
			return false;
		}

		public static GCodeFile Load(string fileName)
		{
			if (FileTooBigToLoad(fileName))
			{
				return new GCodeFileStreamed(fileName);
			}
			return new GCodeFileLoaded(fileName);
		}

		public static string ReplaceNumberAfter(char charToReplaceAfter, string stringWithNumber, double numberToPutIn)
		{
			int num = stringWithNumber.IndexOf(charToReplaceAfter);
			if (num != -1)
			{
				int num2 = stringWithNumber.IndexOf(" ", num);
				if (num2 == -1)
				{
					return $"{stringWithNumber.Substring(0, num + 1)}{numberToPutIn:0.#####}";
				}
				return $"{stringWithNumber.Substring(0, num + 1)}{numberToPutIn:0.#####}{stringWithNumber.Substring(num2)}";
			}
			return stringWithNumber;
		}

		protected static double GetSecondsThisLine(Vector3 deltaPositionThisLine, double deltaEPositionThisLine, double feedRateMmPerMin)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			double x = VelocitySameAsStopMmPerS.x;
			_ = VelocitySameAsStopMmPerS;
			double num = Math.Min(feedRateMmPerMin / 60.0, MaxVelocityMmPerS.x);
			double x2 = MaxAccelerationMmPerS2.x;
			double num2 = Math.Max(((Vector3)(ref deltaPositionThisLine)).get_Length(), deltaEPositionThisLine);
			double distanceToReachEndingVelocity = GetDistanceToReachEndingVelocity(x, num, x2);
			if (distanceToReachEndingVelocity <= num2 / 2.0)
			{
				double num3 = GetTimeToAccelerateDistance(x, distanceToReachEndingVelocity, x2) * 2.0;
				double num4 = (num2 - distanceToReachEndingVelocity * 2.0) / num;
				return num3 + num4;
			}
			return GetTimeToAccelerateDistance(x, num2 / 2.0, x2) * 2.0;
		}

		private static double GetDistanceToReachEndingVelocity(double startingVelocityMmPerS, double endingVelocityMmPerS, double accelerationMmPerS2)
		{
			double num = endingVelocityMmPerS * endingVelocityMmPerS;
			double num2 = startingVelocityMmPerS * startingVelocityMmPerS;
			return (num - num2) / (2.0 * accelerationMmPerS2);
		}

		private static double GetTimeToAccelerateDistance(double startingVelocityMmPerS, double distanceMm, double accelerationMmPerS2)
		{
			double num = startingVelocityMmPerS * startingVelocityMmPerS;
			double num2 = 2.0 * accelerationMmPerS2 * distanceMm;
			return (Math.Sqrt(num + num2) - startingVelocityMmPerS) / accelerationMmPerS2;
		}

		private static bool RunningIn32Bit()
		{
			if (IntPtr.Size == 4)
			{
				return true;
			}
			return false;
		}
	}
}
