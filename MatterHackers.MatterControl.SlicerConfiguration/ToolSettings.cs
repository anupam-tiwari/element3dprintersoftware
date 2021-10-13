using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class ToolSettings
	{
		public double acceleration;

		public double duty;

		public double infillPercent;

		public string infillType;

		public double infillStartingAngle;

		public double intensity;

		public double jerk;

		public double keepWarm;

		public string name = "";

		public double period;

		public int pin;

		public int position;

		public double psi;

		public int skirt;

		public double speed;

		public double temperature;

		public int temperaturePosition;

		public TOOL_TYPE toolType;

		public double width;

		[JsonIgnore]
		public double Acceleration => acceleration;

		[JsonIgnore]
		public double InfillPercent => infillPercent;

		[JsonIgnore]
		public string InfillType => infillType;

		[JsonIgnore]
		public double InfillStartingAngle => infillStartingAngle;

		[JsonIgnore]
		public double Intensity => intensity;

		[JsonIgnore]
		public double Jerk => jerk;

		[JsonIgnore]
		public double KeepWarm => keepWarm;

		[JsonIgnore]
		public double Period => period;

		[JsonIgnore]
		public double Pin => pin;

		[JsonIgnore]
		public double Position => position;

		[JsonIgnore]
		public double PSI => psi;

		[JsonIgnore]
		public int Skirt => skirt;

		[JsonIgnore]
		public double Speed => speed;

		[JsonIgnore]
		public double Temperature => temperature;

		[JsonIgnore]
		public int TPosition => temperaturePosition;

		[JsonIgnore]
		public double Uptime
		{
			get
			{
				double num = ((period == 0.0) ? 0.0 : (period * duty / 100.0));
				if (!(0.0 < num) || !(num < 0.1))
				{
					return num;
				}
				return 0.1;
			}
		}

		[JsonIgnore]
		public double Width => width;

		public ToolSettings()
		{
			ResetAllSettings();
		}

		public void ResetAllSettings()
		{
			acceleration = 0.0;
			duty = 0.0;
			infillPercent = 40.0;
			infillType = "GRID";
			infillStartingAngle = 45.0;
			intensity = 0.0;
			jerk = 0.0;
			keepWarm = 0.0;
			name = "";
			period = 0.0;
			pin = 0;
			position = 0;
			psi = 0.0;
			skirt = 0;
			speed = 0.0;
			temperature = 0.0;
			temperaturePosition = 0;
			toolType = TOOL_TYPE.NONE;
			width = 0.0;
		}

		public void RemovePosition()
		{
			switch (toolType)
			{
			case TOOL_TYPE.FFF:
				Positions.Instance.ResetFFFPosition(position);
				break;
			case TOOL_TYPE.SYRINGE:
			case TOOL_TYPE.LASER:
			case TOOL_TYPE.IO:
				Positions.Instance.ResetSyringePosition(position);
				break;
			case TOOL_TYPE.TSYRINGE:
				Positions.Instance.ResetLimitedSyringePosition(position, 4);
				break;
			case TOOL_TYPE.MICRO_VALVE:
				Positions.Instance.ResetMicroValvePosition(position);
				break;
			case TOOL_TYPE.LED:
				Positions.Instance.ResetLEDPosition(position);
				break;
			}
		}

		public bool TakePosition(int newPosition)
		{
			switch (toolType)
			{
			case TOOL_TYPE.FFF:
				return Positions.Instance.SetFFFPosition(newPosition);
			case TOOL_TYPE.SYRINGE:
			case TOOL_TYPE.LASER:
			case TOOL_TYPE.IO:
				return Positions.Instance.SetSyringePosition(newPosition);
			case TOOL_TYPE.TSYRINGE:
				return Positions.Instance.SetLimitedSyringePosition(newPosition, 4);
			case TOOL_TYPE.MICRO_VALVE:
				return Positions.Instance.SetMicroValvePosition(newPosition);
			case TOOL_TYPE.LED:
				return Positions.Instance.SetLEDPosition(newPosition);
			default:
				return false;
			}
		}

		public string GetHashString()
		{
			StringBuilder stringBuilder = new StringBuilder(toolType.ToString());
			PropertyInfo[] properties = GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				object value = propertyInfo.GetValue(this);
				stringBuilder.Append(propertyInfo.Name);
				stringBuilder.Append(value.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
