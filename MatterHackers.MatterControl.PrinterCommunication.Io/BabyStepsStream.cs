using System;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PrinterCommunication.Io
{
	public class BabyStepsStream : GCodeStreamProxy
	{
		private int layerCount = -1;

		private OffsetStream offsetStream;

		private EventHandler unregisterEvents;

		public Vector3 Offset
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return offsetStream.Offset;
			}
			set
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				offsetStream.Offset = value;
			}
		}

		public BabyStepsStream(GCodeStream internalStream)
			: base(null)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			ActiveSliceSettings.SettingChanged.RegisterEvent((EventHandler)delegate(object s, EventArgs e)
			{
				object obj = (object)(e as StringEventArgs);
				if (((obj != null) ? ((StringEventArgs)obj).get_Data() : null) == "baby_step_z_offset")
				{
					OffsetChanged();
				}
			}, ref unregisterEvents);
			offsetStream = new OffsetStream(internalStream, new Vector3(0.0, 0.0, ActiveSliceSettings.Instance.GetValue<double>("baby_step_z_offset")));
			base.internalStream = offsetStream;
		}

		public override void Dispose()
		{
			offsetStream.Dispose();
			unregisterEvents?.Invoke(this, null);
		}

		public override string ReadLine()
		{
			string text = offsetStream.ReadLine();
			if (text != null && layerCount < 1 && GCodeFile.IsLayerChange(text))
			{
				layerCount++;
			}
			return text;
		}

		private void OffsetChanged()
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			offsetStream.Offset = new Vector3(0.0, 0.0, ActiveSliceSettings.Instance.GetValue<double>("baby_step_z_offset"));
		}
	}
}
