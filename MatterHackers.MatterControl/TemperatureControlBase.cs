using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public abstract class TemperatureControlBase : FlowLayoutWidget
	{
		protected TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		protected TextWidget actualTempIndicator;

		protected FlowLayoutWidget presetButtonsContainer;

		protected EditableNumberDisplay targetTemperatureDisplay;

		protected string label;

		protected string editWindowLabel;

		protected int extruderIndex0Based;

		protected FlowLayoutWidget tempEditContainer;

		private EditTemperaturePresetsWindow editSettingsWindow;

		protected abstract string HelpText
		{
			get;
		}

		public override RectangleDouble LocalBounds
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return ((GuiWidget)this).get_LocalBounds();
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				((GuiWidget)this).set_LocalBounds(value);
			}
		}

		private double MaxTemp
		{
			get
			{
				string[] array = GetTemperaturePresets().Split(new char[1]
				{
					','
				});
				double num = 0.0;
				string[] array2 = array;
				foreach (string s in array2)
				{
					double result = 0.0;
					double.TryParse(s, out result);
					num = Math.Max(num, result);
				}
				return num;
			}
		}

		protected TemperatureControlBase(int extruderIndex0Based, string label, string editWindowLabel)
			: this((FlowDirection)3)
		{
			this.extruderIndex0Based = extruderIndex0Based;
			this.label = label;
			this.editWindowLabel = editWindowLabel;
			SetDisplayAttributes();
		}

		protected abstract double GetActualTemperature();

		protected abstract double GetTargetTemperature();

		protected abstract void SetTargetTemperature(double targetTemp);

		protected abstract string GetTemperaturePresets();

		protected abstract double GetPreheatTemperature();

		protected abstract void SetTemperaturePresets(object sender, EventArgs stringEvent);

		private void SetDisplayAttributes()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			textImageButtonFactory.normalFillColor = RGBA_Bytes.Transparent;
			textImageButtonFactory.FixedWidth = 38.0 * GuiWidget.get_DeviceScale();
			textImageButtonFactory.FixedHeight = 20.0 * GuiWidget.get_DeviceScale();
			textImageButtonFactory.fontSize = 10.0;
			textImageButtonFactory.borderWidth = 1.0;
			textImageButtonFactory.normalBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);
			textImageButtonFactory.hoverBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);
			textImageButtonFactory.disabledTextColor = RGBA_Bytes.Gray;
			textImageButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_SecondaryTextColor();
			textImageButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			((GuiWidget)this).set_HAnchor((HAnchor)5);
		}

		protected void AddChildElements()
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Expected O, but got Unknown
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Expected O, but got Unknown
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Expected O, but got Unknown
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Expected O, but got Unknown
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Expected O, but got Unknown
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Expected O, but got Unknown
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Expected O, but got Unknown
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			Button editButton;
			AltGroupBox altGroupBox = new AltGroupBox(textImageButtonFactory.GenerateGroupBoxLabelWithEdit(label, out editButton));
			((GuiWidget)editButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				if (editSettingsWindow == null)
				{
					editSettingsWindow = new EditTemperaturePresetsWindow(editWindowLabel, GetTemperaturePresets(), SetTemperaturePresets);
					((GuiWidget)editSettingsWindow).add_Closed((EventHandler<ClosedEventArgs>)delegate
					{
						editSettingsWindow = null;
					});
				}
				else
				{
					((GuiWidget)editSettingsWindow).BringToFront();
				}
			});
			altGroupBox.TextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			altGroupBox.BorderColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			((GuiWidget)altGroupBox).set_HAnchor((HAnchor)(((GuiWidget)altGroupBox).get_HAnchor() | 5));
			altGroupBox.ClientArea.set_VAnchor((VAnchor)8);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 2.0));
			((GuiWidget)val).set_HAnchor((HAnchor)(((GuiWidget)val).get_HAnchor() | 5));
			tempEditContainer = new FlowLayoutWidget((FlowDirection)3);
			GuiWidget sliderLabels = GetSliderLabels();
			((GuiWidget)tempEditContainer).set_HAnchor((HAnchor)5);
			((GuiWidget)tempEditContainer).AddChild(sliderLabels, -1);
			((GuiWidget)tempEditContainer).set_Visible(false);
			new GuiWidget(0.0, 10.0, (SizeLimitsToSet)1).set_HAnchor((HAnchor)5);
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 0.0));
			((GuiWidget)val2).set_Padding(new BorderDouble(0.0, 3.0));
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_Margin(new BorderDouble(3.0, 0.0));
			TextWidget val4 = new TextWidget("Actual".Localize() + ": ", 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val4.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val4).set_VAnchor((VAnchor)2);
			actualTempIndicator = new TextWidget($"{GetActualTemperature():0.0}°C", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			actualTempIndicator.set_AutoExpandBoundsToText(true);
			actualTempIndicator.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)actualTempIndicator).set_VAnchor((VAnchor)2);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)actualTempIndicator, -1);
			TextWidget val5 = new TextWidget("Target".Localize() + ": ", 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val5).set_Margin(new BorderDouble(10.0, 0.0, 0.0, 0.0));
			val5.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val5).set_VAnchor((VAnchor)2);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val5, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)GetTargetTemperatureDisplay(), -1);
			FlowLayoutWidget helperTextWidget = GetHelpTextWidget();
			((GuiWidget)new LinkButtonFactory
			{
				textColor = ActiveTheme.get_Instance().get_PrimaryTextColor(),
				fontSize = 10.0
			}.Generate("?")).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				((GuiWidget)helperTextWidget).set_Visible(!((GuiWidget)helperTextWidget).get_Visible());
			});
			presetButtonsContainer = GetPresetsContainer();
			((GuiWidget)val2).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)presetButtonsContainer, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)altGroupBox).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)altGroupBox, -1);
		}

		private FlowLayoutWidget GetPresetsContainer()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_Margin(new BorderDouble(3.0, 0.0));
			string arg = "Presets".Localize();
			TextWidget val2 = new TextWidget($"{arg}: ", 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 0.0, 5.0, 0.0));
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_VAnchor((VAnchor)2);
			Enumerator<double, string> enumerator = GetTemperaturePresetLabels().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<double, string> current = enumerator.get_Current();
					Button val3 = textImageButtonFactory.Generate(current.Value);
					((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 0.0, 5.0, 0.0));
					((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
					double temp = current.Key;
					((GuiWidget)val3).add_Click((EventHandler<MouseEventArgs>)delegate
					{
						UiThread.RunOnIdle((Action)delegate
						{
							SetTargetTemperature(temp);
							((GuiWidget)tempEditContainer).set_Visible(false);
						});
					});
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			textImageButtonFactory.FixedWidth = 76.0 * GuiWidget.get_DeviceScale();
			Button val4 = textImageButtonFactory.Generate("Preheat".Localize().ToUpper());
			((GuiWidget)val4).set_Margin(new BorderDouble(0.0, 0.0, 5.0, 0.0));
			((GuiWidget)val).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)val4).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					SetTargetTemperature(GetPreheatTemperature());
					((GuiWidget)tempEditContainer).set_Visible(false);
				});
			});
			textImageButtonFactory.FixedWidth = 38.0 * GuiWidget.get_DeviceScale();
			return val;
		}

		private EditableNumberDisplay GetTargetTemperatureDisplay()
		{
			targetTemperatureDisplay = new EditableNumberDisplay(textImageButtonFactory, $"{GetTargetTemperature():0.0}°C", $"{240.2:0.0}°C");
			targetTemperatureDisplay.EditEnabled += delegate
			{
				((GuiWidget)tempEditContainer).set_Visible(true);
			};
			targetTemperatureDisplay.EditComplete += delegate
			{
				SetTargetTemperature(targetTemperatureDisplay.GetValue());
			};
			return targetTemperatureDisplay;
		}

		private FlowLayoutWidget GetHelpTextWidget()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Expected O, but got Unknown
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			double num = 260.0 * GuiWidget.get_DeviceScale();
			((GuiWidget)val).set_Margin(new BorderDouble(3.0));
			((GuiWidget)val).set_Padding(new BorderDouble(3.0));
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_BackgroundColor(ActiveTheme.get_Instance().get_TransparentDarkOverlay());
			double num2 = 10.0;
			EnglishTextWrapping val2 = new EnglishTextWrapping(num2);
			string helpText = HelpText;
			BorderDouble padding = ((GuiWidget)val).get_Padding();
			foreach (string item in ((TextWrapping)val2).WrapText(helpText, num - ((BorderDouble)(ref padding)).get_Width()))
			{
				GuiWidget val3 = (GuiWidget)new TextWidget(item, 0.0, 0.0, num2, (Justification)0, RGBA_Bytes.White, true, false, default(RGBA_Bytes), (TypeFace)null);
				((GuiWidget)val).AddChild(val3, -1);
			}
			((GuiWidget)val).set_MinimumSize(new Vector2(num, ((GuiWidget)val).get_MinimumSize().y));
			((GuiWidget)val).set_Visible(false);
			return val;
		}

		protected SortedDictionary<double, string> GetTemperaturePresetLabels()
		{
			string temperaturePresets = GetTemperaturePresets();
			SortedDictionary<double, string> val = new SortedDictionary<double, string>();
			val.Add(0.0, "OFF");
			string[] array = temperaturePresets.Split(new char[1]
			{
				','
			});
			for (int i = 0; i < array.Length / 2; i++)
			{
				string text = array[i * 2];
				double.TryParse(array[i * 2 + 1], out var result);
				if (result > 0.0 && !val.ContainsKey(result))
				{
					val.Add(result, text);
				}
			}
			return val;
		}

		protected GuiWidget GetSliderLabels()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			GuiWidget val = new GuiWidget();
			val.set_HAnchor((HAnchor)5);
			val.set_Height(20.0 * GuiWidget.get_DeviceScale());
			double num = -10.0 * GuiWidget.get_DeviceScale();
			SortedDictionary<double, string> temperaturePresetLabels = GetTemperaturePresetLabels();
			bool flag = true;
			double num2 = 0.0;
			Enumerator<double, string> enumerator = temperaturePresetLabels.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<double, string> current = enumerator.get_Current();
					if (flag)
					{
						num2 = num;
						flag = false;
					}
					else
					{
						double val2 = num;
						num2 = Math.Max(num2 + textImageButtonFactory.FixedWidth + 3.0, val2);
					}
					Button val3 = textImageButtonFactory.Generate(current.Value);
					((GuiWidget)val3).set_OriginRelativeParent(new Vector2(num2, 0.0));
					val.AddChild((GuiWidget)(object)val3, -1);
					double temp = current.Key;
					((GuiWidget)val3).add_Click((EventHandler<MouseEventArgs>)delegate
					{
						SetTargetTemperature(temp);
						((GuiWidget)tempEditContainer).set_Visible(false);
					});
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			val.set_HAnchor((HAnchor)8);
			val.set_VAnchor((VAnchor)8);
			val.set_MinimumSize(new Vector2(val.get_Width(), val.get_Height()));
			return val;
		}

		protected void onTemperatureRead(object sender, EventArgs e)
		{
			TemperatureEventArgs temperatureEventArgs = e as TemperatureEventArgs;
			if (temperatureEventArgs != null && temperatureEventArgs.Index0Based == extruderIndex0Based)
			{
				((GuiWidget)actualTempIndicator).set_Text($"{temperatureEventArgs.Temperature:0.0}°C");
			}
		}

		protected void ExtruderTemperatureSet(object sender, EventArgs e)
		{
			TemperatureEventArgs temperatureEventArgs = e as TemperatureEventArgs;
			if (temperatureEventArgs != null && temperatureEventArgs.Index0Based == extruderIndex0Based)
			{
				string displayString = $"{temperatureEventArgs.Temperature:0.0}°C";
				targetTemperatureDisplay.SetDisplayString(displayString);
			}
		}

		protected void BedTemperatureSet(object sender, EventArgs e)
		{
			TemperatureEventArgs temperatureEventArgs = e as TemperatureEventArgs;
			if (temperatureEventArgs != null)
			{
				string displayString = $"{temperatureEventArgs.Temperature:0.0}°C";
				targetTemperatureDisplay.SetDisplayString(displayString);
			}
		}
	}
}
