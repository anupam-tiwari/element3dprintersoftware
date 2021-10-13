using System;
using System.Collections.Generic;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.GCodeVisualizer;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class ColorGradientWidget : FlowLayoutWidget
	{
		public class ColorToSpeedWidget : FlowLayoutWidget
		{
			public string layerSpeed;

			public ColorToSpeedWidget(GuiWidget colorWidget, double speed)
				: this((FlowDirection)0)
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a6: Expected O, but got Unknown
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				((GuiWidget)this).set_Margin(new BorderDouble(2.0));
				layerSpeed = StringHelper.FormatWith("{0} mm/s", new object[1]
				{
					speed
				});
				colorWidget.set_Margin(new BorderDouble(2.0, 0.0, 0.0, 0.0));
				TextWidget val = new TextWidget(layerSpeed, 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				val.set_TextColor(RGBA_Bytes.White);
				((GuiWidget)val).set_VAnchor((VAnchor)2);
				((GuiWidget)val).set_Margin(new BorderDouble(5.0, 0.0));
				((GuiWidget)this).AddChild(colorWidget, -1);
				((GuiWidget)this).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
				((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
				((GuiWidget)this).set_HAnchor((HAnchor)(((GuiWidget)this).get_HAnchor() | 5));
			}
		}

		public ColorGradientWidget(GCodeFile gcodeFileTest)
			: this((FlowDirection)3)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Expected O, but got Unknown
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_BackgroundColor(new RGBA_Bytes(0, 0, 0, 120));
			HashSet<float> val = new HashSet<float>();
			PrinterMachineInstruction printerMachineInstruction = gcodeFileTest.Instruction(0);
			for (int j = 1; j < gcodeFileTest.LineCount; j++)
			{
				PrinterMachineInstruction printerMachineInstruction2 = gcodeFileTest.Instruction(j);
				if (printerMachineInstruction2.EPosition > printerMachineInstruction.EPosition && (printerMachineInstruction2.Line.IndexOf('X') != -1 || printerMachineInstruction2.Line.IndexOf('Y') != -1))
				{
					val.Add((float)printerMachineInstruction2.FeedRate);
				}
				printerMachineInstruction = printerMachineInstruction2;
			}
			ExtrusionColors extrusionColors = new ExtrusionColors();
			Enumerable.ToArray<RGBA_Bytes>(Enumerable.Select<float, RGBA_Bytes>((IEnumerable<float>)val, (Func<float, RGBA_Bytes>)((float speed) => extrusionColors.GetColorForSpeed(speed))));
			if (val.get_Count() > 0)
			{
				float min = Enumerable.Min((IEnumerable<float>)val);
				float num = Enumerable.Max((IEnumerable<float>)val);
				int num2 = Math.Min(7, Enumerable.Count<float>((IEnumerable<float>)val));
				int num3 = num2 - 1;
				float increment = (num - min) / (float)num3;
				int index = 0;
				int[] array = ((val.get_Count() >= 8) ? Enumerable.ToArray<int>(Enumerable.Select<int, int>(Enumerable.Range(0, num2), (Func<int, int>)((int x) => (int)(min + increment * (float)index++)))) : Enumerable.ToArray<int>((IEnumerable<int>)Enumerable.OrderBy<int, int>(Enumerable.Select<float, int>((IEnumerable<float>)val, (Func<float, int>)((float s) => (int)s)), (Func<int, int>)((int i) => i))));
				RGBA_Bytes[] array2 = Enumerable.ToArray<RGBA_Bytes>(Enumerable.Select<int, RGBA_Bytes>((IEnumerable<int>)Enumerable.OrderBy<int, int>((IEnumerable<int>)array, (Func<int, int>)((int s) => s)), (Func<int, RGBA_Bytes>)((int speed) => extrusionColors.GetColorForSpeed(speed))));
				for (int k = 0; k < array2.Length; k++)
				{
					RGBA_Bytes backgroundColor = array2[k];
					int num4 = array[k];
					GuiWidget val2 = new GuiWidget();
					val2.set_Width(20.0);
					val2.set_Height(20.0);
					val2.set_BackgroundColor(backgroundColor);
					val2.set_Margin(new BorderDouble(2.0));
					double speed2 = num4 / 60;
					ColorToSpeedWidget colorToSpeedWidget = new ColorToSpeedWidget(val2, speed2);
					((GuiWidget)this).AddChild((GuiWidget)(object)colorToSpeedWidget, -1);
				}
				((GuiWidget)this).set_Margin(new BorderDouble(5.0, 5.0, 200.0, 50.0));
				((GuiWidget)this).set_HAnchor((HAnchor)(((GuiWidget)this).get_HAnchor() | 1));
				((GuiWidget)this).set_VAnchor((VAnchor)4);
			}
		}
	}
}
