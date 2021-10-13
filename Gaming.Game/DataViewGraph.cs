using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;
using MatterHackers.VectorMath;

namespace Gaming.Game
{
	public class DataViewGraph : WindowWidget
	{
		internal class HistoryData
		{
			private int capacity;

			private List<double> data;

			internal double currentDataSum;

			internal RGBA_Bytes lineColor;

			public int Count => data.Count;

			internal HistoryData(int capacity, IColorType lineColor)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				this.lineColor = lineColor.GetAsRGBA_Bytes();
				this.capacity = capacity;
				data = new List<double>();
				Reset();
			}

			internal void Add(double Value)
			{
				if (data.Count == capacity)
				{
					currentDataSum -= data[0];
					data.RemoveAt(0);
				}
				data.Add(Value);
				currentDataSum += Value;
			}

			internal void Reset()
			{
				currentDataSum = 0.0;
				data.Clear();
			}

			internal double GetItem(int ItemIndex)
			{
				if (ItemIndex < data.Count)
				{
					return data[ItemIndex];
				}
				return 0.0;
			}

			internal double GetMaxValue()
			{
				double num = -9999999999.0;
				for (int i = 0; i < data.Count; i++)
				{
					if (data[i] > num)
					{
						num = data[i];
					}
				}
				return num;
			}

			internal double GetMinValue()
			{
				double num = 9999999999.0;
				for (int i = 0; i < data.Count; i++)
				{
					if (data[i] < num)
					{
						num = data[i];
					}
				}
				return num;
			}

			internal double GetAverageValue()
			{
				return currentDataSum / (double)data.Count;
			}
		}

		private RGBA_Floats SentDataLineColor = new RGBA_Floats(200f, 200f, 0f);

		private RGBA_Floats ReceivedDataLineColor = new RGBA_Floats(0f, 200f, 20f);

		private RGBA_Floats BoxColor = new RGBA_Floats(10f, 25f, 240f);

		private double valueMin;

		private double valueMax;

		private bool dynamiclyScaleRange;

		private uint graphWidth;

		private uint graphHeight;

		private Dictionary<string, HistoryData> dataHistoryArray;

		private int nextLineColorIndex;

		private PathStorage linesToDrawStorage;

		public DataViewGraph()
			: this(80u, 50u, 0.0, 0.0)
		{
			dynamiclyScaleRange = true;
		}

		public DataViewGraph(uint Width, uint Height)
			: this(Width, Height, 0.0, 0.0)
		{
			dynamiclyScaleRange = true;
		}

		public DataViewGraph(uint width, uint height, double valueMin, double valueMax)
			: this(new RectangleDouble(0.0, 0.0, (double)(width + 150), (double)(height + 80)))
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Expected O, but got Unknown
			linesToDrawStorage = new PathStorage();
			dataHistoryArray = new Dictionary<string, HistoryData>();
			graphWidth = width;
			graphHeight = height;
			this.valueMin = valueMin;
			this.valueMax = valueMax;
			if (valueMin == 0.0 && valueMax == 0.0)
			{
				this.valueMax = -999999.0;
				this.valueMin = 999999.0;
			}
			dynamiclyScaleRange = false;
		}

		public double GetAverageValue(string DataType)
		{
			dataHistoryArray.TryGetValue(DataType, out var value);
			return value?.GetAverageValue() ?? 0.0;
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Expected O, but got Unknown
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Expected O, but got Unknown
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Expected O, but got Unknown
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Expected O, but got Unknown
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Expected O, but got Unknown
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			double num = -20.0;
			double num2 = valueMax - valueMin;
			Vector2 val = default(Vector2);
			((Vector2)(ref val))._002Ector(1.0, ((GuiWidget)this).get_Height() - (double)graphHeight - 22.0);
			RoundedRect val2 = new RoundedRect(val.x, val.y - 1.0, val.x + (double)graphWidth, val.y - 1.0 + (double)graphHeight + 2.0, 5.0);
			graphics2D.Render((IVertexSource)(object)val2, (IColorType)(object)new RGBA_Bytes(0.0, 0.0, 0.0, 0.5));
			if (valueMin < 0.0 && valueMax > 0.0)
			{
				linesToDrawStorage.remove_all();
				linesToDrawStorage.MoveTo(val.x, val.y + (0.0 - valueMin) * (double)graphHeight / num2);
				linesToDrawStorage.LineTo(val.x + (double)graphWidth, val.y + (0.0 - valueMin) * (double)graphHeight / num2);
				Stroke val3 = new Stroke((IVertexSource)(object)linesToDrawStorage, 1.0);
				graphics2D.Render((IVertexSource)(object)val3, (IColorType)(object)new RGBA_Bytes(0, 0, 0, 1));
			}
			double val4 = -999999999.0;
			double val5 = 999999999.0;
			double val6 = 0.0;
			foreach (KeyValuePair<string, HistoryData> item in dataHistoryArray)
			{
				HistoryData value = item.Value;
				linesToDrawStorage.remove_all();
				val4 = Math.Max(val4, value.GetMaxValue());
				val5 = Math.Min(val5, value.GetMinValue());
				val6 = Math.Max(val6, value.GetAverageValue());
				for (int i = 0; i < graphWidth - 1; i++)
				{
					if (i == 0)
					{
						linesToDrawStorage.MoveTo(val.x + (double)i, val.y + (value.GetItem(i) - valueMin) * (double)graphHeight / num2);
					}
					else
					{
						linesToDrawStorage.LineTo(val.x + (double)i, val.y + (value.GetItem(i) - valueMin) * (double)graphHeight / num2);
					}
				}
				Stroke val3 = new Stroke((IVertexSource)(object)linesToDrawStorage, 1.0);
				graphics2D.Render((IVertexSource)(object)val3, (IColorType)(object)value.lineColor);
				string text = item.Key + ": Min:" + val5.ToString("0.0") + " Max:" + val4.ToString("0.0") + " Avg:" + val6.ToString("0.0");
				graphics2D.DrawString(text, val.x, val.y + num, 12.0, (Justification)0, (Baseline)3, default(RGBA_Bytes), true, new RGBA_Bytes(RGBA_Bytes.White, 220));
				num -= 20.0;
			}
			Stroke val7 = new Stroke((IVertexSource)new RoundedRect(val.x, val.y - 1.0, val.x + (double)graphWidth, val.y - 1.0 + (double)graphHeight + 2.0, 5.0), 1.0);
			graphics2D.Render((IVertexSource)(object)val7, (IColorType)(object)new RGBA_Bytes(0.0, 0.0, 0.0, 1.0));
			((WindowWidget)this).OnDraw(graphics2D);
		}

		public void AddData(string DataType, double NewData)
		{
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			if (dynamiclyScaleRange)
			{
				valueMax = Math.Max(valueMax, NewData);
				valueMin = Math.Min(valueMin, NewData);
			}
			if (!dataHistoryArray.ContainsKey(DataType))
			{
				RGBA_Bytes val = default(RGBA_Bytes);
				((RGBA_Bytes)(ref val))._002Ector(255, 255, 255);
				switch (nextLineColorIndex++ % 3)
				{
				case 0:
					((RGBA_Bytes)(ref val))._002Ector(255, 55, 55);
					break;
				case 1:
					((RGBA_Bytes)(ref val))._002Ector(55, 255, 55);
					break;
				case 2:
					((RGBA_Bytes)(ref val))._002Ector(55, 55, 255);
					break;
				}
				dataHistoryArray.Add(DataType, new HistoryData((int)graphWidth, (IColorType)(object)val));
			}
			dataHistoryArray[DataType].Add(NewData);
		}

		public void Reset()
		{
			valueMax = 1.0;
			valueMin = 99999.0;
			foreach (KeyValuePair<string, HistoryData> item in dataHistoryArray)
			{
				item.Value.Reset();
			}
		}
	}
}
