using System;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class Positions
	{
		private static Positions instance;

		private static int NUM_FFF = 2;

		private static int NUM_SYRINGE = 8;

		private static int NUM_MICROVALVE = 14;

		private static int NUM_LED = 2;

		private static int NUM_TPOS = 4;

		private readonly bool[] fffPositions;

		private readonly bool[] syringePositions;

		private readonly bool[] microValvePositions;

		private readonly bool[] ledPositions;

		private readonly bool[] temperaturePositions;

		public static Positions Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new Positions();
				}
				return instance;
			}
		}

		private Positions()
		{
			fffPositions = new bool[NUM_FFF];
			syringePositions = new bool[NUM_SYRINGE];
			microValvePositions = new bool[NUM_MICROVALVE];
			ledPositions = new bool[NUM_LED];
			temperaturePositions = new bool[NUM_TPOS];
		}

		public bool SetFFFPosition(int position)
		{
			bool result = false;
			if (0 < position && position <= NUM_FFF && !fffPositions[position - 1])
			{
				result = (fffPositions[position - 1] = true);
			}
			return result;
		}

		public bool SetSyringePosition(int position)
		{
			bool result = false;
			if (0 < position && position <= NUM_SYRINGE && !syringePositions[position - 1])
			{
				result = (syringePositions[position - 1] = true);
			}
			return result;
		}

		public bool SetLimitedSyringePosition(int position, int limit)
		{
			bool result = false;
			int num = Math.Min(limit, NUM_SYRINGE);
			if (0 < position && position <= num && !syringePositions[position - 1])
			{
				result = (syringePositions[position - 1] = true);
			}
			return result;
		}

		public bool SetMicroValvePosition(int position)
		{
			bool result = false;
			if (0 < position && position <= NUM_MICROVALVE && !microValvePositions[position - 1])
			{
				result = (microValvePositions[position - 1] = true);
			}
			return result;
		}

		public bool SetLEDPosition(int position)
		{
			bool flag = true;
			if (0 < position && position <= NUM_LED)
			{
				int num = (position - 1) * NUM_MICROVALVE / NUM_LED;
				int num2 = position * NUM_MICROVALVE / NUM_LED;
				for (int i = num; i < num2; i++)
				{
					flag &= !microValvePositions[i];
				}
				if (flag)
				{
					ledPositions[position - 1] = true;
					for (int j = num; j < num2; j++)
					{
						microValvePositions[j] = true;
					}
				}
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public bool SetTemperaturePosition(int position)
		{
			bool result = false;
			if (0 < position && position <= NUM_TPOS && !temperaturePositions[position - 1])
			{
				result = (temperaturePositions[position - 1] = true);
			}
			return result;
		}

		public bool ResetFFFPosition(int position)
		{
			bool result = false;
			if (0 < position && position <= NUM_FFF)
			{
				fffPositions[position - 1] = false;
				result = true;
			}
			return result;
		}

		public bool ResetSyringePosition(int position)
		{
			bool result = false;
			if (0 < position && position <= NUM_SYRINGE)
			{
				syringePositions[position - 1] = false;
				result = true;
			}
			return result;
		}

		public bool ResetLimitedSyringePosition(int position, int limit)
		{
			bool result = false;
			int num = Math.Min(limit, NUM_SYRINGE);
			if (0 < position && position <= num)
			{
				syringePositions[position - 1] = false;
				result = true;
			}
			return result;
		}

		public bool ResetMicroValvePosition(int position)
		{
			int num = (position - 1) * NUM_LED / NUM_MICROVALVE;
			bool result = false;
			if (0 < position && position <= NUM_MICROVALVE && !ledPositions[num])
			{
				microValvePositions[position - 1] = false;
				result = true;
			}
			return result;
		}

		public bool ResetLEDPosition(int position)
		{
			bool result = false;
			if (0 < position && position <= NUM_LED && ledPositions[position - 1])
			{
				ledPositions[position - 1] = false;
				int num = (position - 1) * NUM_MICROVALVE / NUM_LED;
				int num2 = position * NUM_MICROVALVE / NUM_LED;
				for (int i = num; i < num2; i++)
				{
					microValvePositions[i] = false;
				}
				result = true;
			}
			return result;
		}

		public bool ResetTemperaturePosition(int position)
		{
			bool result = false;
			if (0 < position && position <= NUM_TPOS)
			{
				temperaturePositions[position - 1] = false;
				result = true;
			}
			return result;
		}

		public void ResetAllPositions()
		{
			Array.Clear(fffPositions, 0, NUM_FFF);
			Array.Clear(syringePositions, 0, NUM_SYRINGE);
			Array.Clear(microValvePositions, 0, NUM_MICROVALVE);
			Array.Clear(ledPositions, 0, NUM_LED);
			Array.Clear(temperaturePositions, 0, NUM_TPOS);
		}
	}
}
