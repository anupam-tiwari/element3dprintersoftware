using System.Text;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.GCodeVisualizer
{
	public class PrinterMachineInstruction
	{
		public enum MovementTypes
		{
			Absolute,
			Relative
		}

		public byte[] byteLine;

		private Vector3Float xyzPosition;

		private float ePosition;

		private float feedRate;

		public MovementTypes movementType;

		public float secondsThisLine;

		public float secondsToEndFromHere;

		public bool clientInsertion;

		public string Line
		{
			get
			{
				return Encoding.Default.GetString(byteLine);
			}
			set
			{
				byteLine = Encoding.Default.GetBytes(value);
			}
		}

		public int ExtruderIndex
		{
			get;
			set;
		}

		public Vector3 Position => new Vector3(xyzPosition);

		public double X
		{
			get
			{
				return xyzPosition.x;
			}
			set
			{
				if (movementType == MovementTypes.Absolute)
				{
					xyzPosition.x = (float)value;
				}
				else
				{
					xyzPosition.x += (float)value;
				}
			}
		}

		public double Y
		{
			get
			{
				return xyzPosition.y;
			}
			set
			{
				if (movementType == MovementTypes.Absolute)
				{
					xyzPosition.y = (float)value;
				}
				else
				{
					xyzPosition.y += (float)value;
				}
			}
		}

		public double Z
		{
			get
			{
				return xyzPosition.z;
			}
			set
			{
				if (movementType == MovementTypes.Absolute)
				{
					xyzPosition.z = (float)value;
				}
				else
				{
					xyzPosition.z += (float)value;
				}
			}
		}

		public double EPosition
		{
			get
			{
				return ePosition;
			}
			set
			{
				ePosition = (float)value;
			}
		}

		public double FeedRate
		{
			get
			{
				return feedRate;
			}
			set
			{
				feedRate = (float)value;
			}
		}

		public PrinterMachineInstruction(string Line)
		{
			this.Line = Line;
		}

		public PrinterMachineInstruction(string Line, PrinterMachineInstruction copy, bool clientInsertion = false)
			: this(Line)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			xyzPosition = copy.xyzPosition;
			feedRate = copy.feedRate;
			ePosition = copy.ePosition;
			movementType = copy.movementType;
			secondsToEndFromHere = copy.secondsToEndFromHere;
			ExtruderIndex = copy.ExtruderIndex;
			this.clientInsertion = clientInsertion;
		}
	}
}
