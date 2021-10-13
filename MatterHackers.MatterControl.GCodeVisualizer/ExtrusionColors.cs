using System.Collections.Generic;
using System.Linq;
using MatterHackers.Agg;

namespace MatterHackers.MatterControl.GCodeVisualizer
{
	public class ExtrusionColors
	{
		private SortedList<float, RGBA_Bytes> speedColorLookup = new SortedList<float, RGBA_Bytes>();

		public RGBA_Bytes GetColorForSpeed(float speed)
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			if (speed > 0f)
			{
				lock (speedColorLookup)
				{
					double num = 223.0 / 360.0;
					double num2 = 0.013888888888888888;
					double num3 = num - num2;
					if (!speedColorLookup.ContainsKey(speed))
					{
						RGBA_Floats val = RGBA_Floats.FromHSL(num, 0.99, 0.49, 1.0);
						RGBA_Bytes asRGBA_Bytes = ((RGBA_Floats)(ref val)).GetAsRGBA_Bytes();
						speedColorLookup.Add(speed, asRGBA_Bytes);
						if (speedColorLookup.get_Count() > 1)
						{
							double num4 = num3 / (double)(speedColorLookup.get_Count() - 1);
							for (int i = 0; i < speedColorLookup.get_Count(); i++)
							{
								double num5 = num4 * (double)i;
								double num6 = num - num5;
								KeyValuePair<float, RGBA_Bytes> keyValuePair = Enumerable.ElementAt<KeyValuePair<float, RGBA_Bytes>>((IEnumerable<KeyValuePair<float, RGBA_Bytes>>)speedColorLookup, i);
								SortedList<float, RGBA_Bytes> obj = speedColorLookup;
								float key = keyValuePair.Key;
								val = RGBA_Floats.FromHSL(num6, 0.99, 0.49, 1.0);
								obj.set_Item(key, ((RGBA_Floats)(ref val)).GetAsRGBA_Bytes());
							}
						}
					}
					return speedColorLookup.get_Item(speed);
				}
			}
			return RGBA_Bytes.Black;
		}
	}
}
