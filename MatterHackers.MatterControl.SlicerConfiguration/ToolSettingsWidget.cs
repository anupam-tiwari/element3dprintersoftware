using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterSlice;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class ToolSettingsWidget : FlowLayoutWidget
	{
		private int toolNumber;

		private TabIndexKeeper tabIndexKeeper;

		private SettingRowFactory rowFactory;

		private ToolSettings toolSettings;

		private readonly string[] toolNames = new string[7]
		{
			"Filament Extruder",
			"Syringe",
			"Temperature Syringe",
			"Microvalve",
			"Laser",
			"LED",
			"IO"
		};

		private EventHandler periodUpdated;

		private static EventHandler temperaturePositionUpdateHandler;

		private static EventHandler positionUpdateHandler;

		public ToolSettingsWidget(int extruderIndex, TabIndexKeeper indexKeeper)
			: this((FlowDirection)3)
		{
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			toolNumber = extruderIndex;
			tabIndexKeeper = indexKeeper;
			rowFactory = new SettingRowFactory(tabIndexKeeper);
			if (ActiveSliceSettings.Instance.Tools.Count <= toolNumber)
			{
				toolSettings = new ToolSettings();
				ActiveSliceSettings.Instance.Tools.Add(toolSettings);
			}
			else
			{
				toolSettings = ActiveSliceSettings.Instance.Tools[toolNumber];
			}
			ConfigureToolWidget();
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			((GuiWidget)this).OnClosed(e);
			toolSettings.RemovePosition();
		}

		private void ConfigureToolWidget()
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Expected O, but got Unknown
			((GuiWidget)this).CloseAllChildren();
			((GuiWidget)this).RemoveAllChildren();
			FlowLayoutWidget val = rowFactory.NewRow();
			DropDownList val2 = new DropDownList("None".Localize(), (Direction)1, 200.0, false);
			((GuiWidget)val2).set_ToolTipText("Tool type for this extruder & material".Localize());
			((GuiWidget)val2).set_Margin(default(BorderDouble));
			DropDownList val3 = val2;
			string[] array = toolNames;
			foreach (string text in array)
			{
				MenuItem val4 = val3.AddItem(text.Localize(), (string)null, 12.0);
				((GuiWidget)val4).set_Name(text);
				if (((GuiWidget)val4).get_Name() == toolSettings.name)
				{
					val3.set_SelectedLabel(((GuiWidget)val4).get_Text());
				}
				val4.add_Selected((EventHandler)delegate(object sender, EventArgs e)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Expected O, but got Unknown
					MenuItem val5 = (MenuItem)sender;
					Positions.Instance.ResetTemperaturePosition(toolSettings.temperaturePosition);
					toolSettings.RemovePosition();
					toolSettings.ResetAllSettings();
					ConfigureForTool(((GuiWidget)val5).get_Name());
					positionUpdateHandler?.Invoke(this, null);
					temperaturePositionUpdateHandler?.Invoke(this, null);
					UiThread.RunOnIdle((Action)ApplicationController.Instance.ReloadAdvancedControlsPanel);
				});
			}
			((GuiWidget)val).AddChild(rowFactory.MakeSettingName("Tool type"), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			ConfigureForTool();
			ActiveSliceSettings.Instance.Save();
		}

		private void ConfigureForTool(string toolName)
		{
			toolSettings.name = toolName;
			switch (toolName)
			{
			case "Filament Extruder":
				toolSettings.toolType = TOOL_TYPE.FFF;
				toolSettings.temperature = 185.0;
				toolSettings.skirt = 1;
				toolSettings.width = 0.4;
				break;
			case "Syringe":
				toolSettings.toolType = TOOL_TYPE.SYRINGE;
				toolSettings.psi = 1.0;
				toolSettings.width = 0.84;
				break;
			case "Temperature Syringe":
				toolSettings.toolType = TOOL_TYPE.TSYRINGE;
				toolSettings.temperature = 100.0;
				toolSettings.width = 0.84;
				break;
			case "Microvalve":
				toolSettings.toolType = TOOL_TYPE.MICRO_VALVE;
				toolSettings.width = 0.84;
				break;
			case "Laser":
				toolSettings.toolType = TOOL_TYPE.LASER;
				toolSettings.width = 0.075;
				break;
			case "LED":
				toolSettings.toolType = TOOL_TYPE.LED;
				toolSettings.width = 0.5;
				break;
			case "IO":
				toolSettings.toolType = TOOL_TYPE.IO;
				toolSettings.width = 0.5;
				break;
			default:
				toolSettings.toolType = TOOL_TYPE.NONE;
				break;
			}
			ConfigureToolWidget();
		}

		private void ConfigureForTool()
		{
			if (toolSettings.toolType != 0 && (int)toolSettings.toolType < Enum.GetValues(typeof(TOOL_TYPE)).Length)
			{
				MakeInfill();
				MakeSkirt();
				MakePosition();
				MakeSpeed();
				MakeAcceleration();
				MakeJerk();
			}
			switch (toolSettings.toolType)
			{
			case TOOL_TYPE.FFF:
				ConfigureFFF();
				break;
			case TOOL_TYPE.SYRINGE:
				ConfigureSyringe();
				break;
			case TOOL_TYPE.TSYRINGE:
				ConfigureTemperatureSyringe();
				break;
			case TOOL_TYPE.MICRO_VALVE:
				ConfigureMicroValve();
				break;
			case TOOL_TYPE.LASER:
				ConfigureLaser();
				break;
			case TOOL_TYPE.LED:
				ConfigureLED();
				break;
			case TOOL_TYPE.IO:
				ConfigureIO();
				break;
			default:
				ConfigureNoTool();
				break;
			}
		}

		private void ConfigureFFF()
		{
			MakeTemperature();
			MakeKeepWarm();
		}

		private void ConfigureSyringe()
		{
			MakePSI();
			MakeWidth();
		}

		private void ConfigureTemperatureSyringe()
		{
			MakeTemperature();
			MakeTemperaturePosition();
			MakePSI();
			MakeWidth();
		}

		private void ConfigureMicroValve()
		{
			MakePSI();
			MakePeriod();
			MakeDuty();
			MakeWidth();
		}

		private void ConfigureLaser()
		{
			MakeIntensity();
		}

		private void ConfigureLED()
		{
			MakeIntensity();
		}

		private void ConfigureIO()
		{
			MakeIntensity();
			MakePin();
		}

		private void ConfigureNoTool()
		{
			FlowLayoutWidget val = rowFactory.NewRow();
			((GuiWidget)val).AddChild(rowFactory.MakeSettingName("Please select a tool type to configure.".Localize()), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private void MakeAcceleration()
		{
			FlowLayoutWidget val = rowFactory.NewRow();
			MHNumberEdit mHNumberEdit = rowFactory.MakeDoubleEdit(toolSettings.acceleration, "Acceleration when using this tool.".Localize());
			((TextEditWidget)mHNumberEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				NumberEdit val2 = (NumberEdit)sender;
				toolSettings.acceleration = val2.get_Value();
				ActiveSliceSettings.Instance.Save();
			});
			((GuiWidget)val).AddChild(rowFactory.MakeSettingName("Acceleration"), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)mHNumberEdit, -1);
			((GuiWidget)val).AddChild(rowFactory.MakeUnits("mm/s^2"), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private void MakeDuty()
		{
			FlowLayoutWidget val = rowFactory.NewRow();
			string toolTip = "Duty cycle, of the period, for micro valve.".Localize();
			MHTextEditWidget mHTextEditWidget = new MHTextEditWidget(toolSettings.duty + "%", 0.0, 0.0, 12.0, rowFactory.doubleEditWidth - 2, 0.0, multiLine: false, tabIndexKeeper.TabIndex++);
			((GuiWidget)mHTextEditWidget).set_ToolTipText(toolTip);
			mHTextEditWidget.SelectAllOnFocus = true;
			MHTextEditWidget stringEdit = mHTextEditWidget;
			EventHandler updateStyle = delegate
			{
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				string text2 = ((GuiWidget)stringEdit.ActualTextEditWidget).get_Text().Trim();
				if (text2.Contains("%"))
				{
					text2 = text2.Substring(0, text2.IndexOf("%"));
				}
				double.TryParse(text2, out var result2);
				if (result2 == 0.0)
				{
					((GuiWidget)stringEdit.ActualTextEditWidget).set_BackgroundColor(RGBA_Bytes.Red);
					((GuiWidget)stringEdit.ActualTextEditWidget).set_ToolTipText("Duty Cycle must be non-zero".Localize());
				}
				else if (result2 < 0.0 || result2 > 100.0)
				{
					((GuiWidget)stringEdit.ActualTextEditWidget).set_BackgroundColor(RGBA_Bytes.Red);
					((GuiWidget)stringEdit.ActualTextEditWidget).set_ToolTipText("Duty Cycle must be a valid percentage".Localize());
				}
				else
				{
					((GuiWidget)stringEdit.ActualTextEditWidget).set_BackgroundColor(RGBA_Bytes.White);
					((GuiWidget)stringEdit.ActualTextEditWidget).set_ToolTipText(toolTip);
				}
			};
			((GuiWidget)stringEdit.ActualTextEditWidget).add_KeyUp((EventHandler<KeyEventArgs>)delegate(object sender, KeyEventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				TextEditWidget val3 = (TextEditWidget)sender;
				string text = ((GuiWidget)val3).get_Text().Trim();
				if (text.Contains("%"))
				{
					text = text.Substring(0, text.IndexOf("%"));
				}
				double.TryParse(text, out var result);
				text = result.ToString();
				text += "%";
				((GuiWidget)val3).set_Text(text);
				toolSettings.duty = result;
				updateStyle(this, null);
			});
			stringEdit.ActualTextEditWidget.add_EditComplete((EventHandler)delegate
			{
				ActiveSliceSettings.Instance.Save();
			});
			stringEdit.ActualTextEditWidget.get_InternalTextEditWidget().add_AllSelected((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				InternalTextEditWidget val2 = (InternalTextEditWidget)sender;
				int num = ((GuiWidget)val2).get_Text().IndexOf("%");
				if (num != -1)
				{
					val2.SetSelection(0, num - 1);
				}
			});
			updateStyle(this, null);
			((GuiWidget)val).AddChild(rowFactory.MakeSettingName("Duty Cycle"), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)stringEdit, -1);
			((GuiWidget)val).AddChild(rowFactory.MakeUnits("Percent"), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private void MakeInfillPercentage()
		{
			FlowLayoutWidget val = rowFactory.NewRow();
			string toolTip = "The amount of infill material to generate for this tool, expressed as a percentage.".Localize();
			MHTextEditWidget mHTextEditWidget = new MHTextEditWidget(toolSettings.infillPercent + "%", 0.0, 0.0, 12.0, rowFactory.doubleEditWidth - 2, 0.0, multiLine: false, tabIndexKeeper.TabIndex++);
			((GuiWidget)mHTextEditWidget).set_ToolTipText(toolTip);
			mHTextEditWidget.SelectAllOnFocus = true;
			MHTextEditWidget stringEdit = mHTextEditWidget;
			EventHandler updateStyle = delegate
			{
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				string text2 = ((GuiWidget)stringEdit.ActualTextEditWidget).get_Text().Trim();
				if (text2.Contains("%"))
				{
					text2 = text2.Substring(0, text2.IndexOf("%"));
				}
				double.TryParse(text2, out var result2);
				if (result2 < 0.0 || result2 > 100.0)
				{
					((GuiWidget)stringEdit.ActualTextEditWidget).set_BackgroundColor(RGBA_Bytes.Red);
					((GuiWidget)stringEdit.ActualTextEditWidget).set_ToolTipText("Infill amount must be a valid percentage".Localize());
				}
				else
				{
					((GuiWidget)stringEdit.ActualTextEditWidget).set_BackgroundColor(RGBA_Bytes.White);
					((GuiWidget)stringEdit.ActualTextEditWidget).set_ToolTipText(toolTip);
				}
			};
			((GuiWidget)stringEdit.ActualTextEditWidget).add_KeyUp((EventHandler<KeyEventArgs>)delegate(object sender, KeyEventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				TextEditWidget val3 = (TextEditWidget)sender;
				string text = ((GuiWidget)val3).get_Text().Trim();
				if (text.Contains("%"))
				{
					text = text.Substring(0, text.IndexOf("%"));
				}
				double.TryParse(text, out var result);
				text = result.ToString();
				text += "%";
				((GuiWidget)val3).set_Text(text);
				toolSettings.infillPercent = result;
				updateStyle(this, null);
			});
			stringEdit.ActualTextEditWidget.add_EditComplete((EventHandler)delegate
			{
				ActiveSliceSettings.Instance.Save();
			});
			stringEdit.ActualTextEditWidget.get_InternalTextEditWidget().add_AllSelected((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				InternalTextEditWidget val2 = (InternalTextEditWidget)sender;
				int num = ((GuiWidget)val2).get_Text().IndexOf("%");
				if (num != -1)
				{
					val2.SetSelection(0, num - 1);
				}
			});
			updateStyle(this, null);
			((GuiWidget)val).AddChild(rowFactory.MakeSettingName("Infill Amount"), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)stringEdit, -1);
			((GuiWidget)val).AddChild(rowFactory.MakeUnits("Percent"), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private void MakeInfillType()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			FlowLayoutWidget val = rowFactory.NewRow();
			DropDownList val2 = new DropDownList("None".Localize(), (Direction)1, 200.0, false);
			((GuiWidget)val2).set_ToolTipText("The geometric shape of the support structure for the inside of this tool's parts.".Localize());
			DropDownList val3 = val2;
			string[] names = Enum.GetNames(typeof(INFILL_TYPE));
			foreach (string text in names)
			{
				MenuItem obj = val3.AddItem(text, (string)null, 12.0);
				if (((GuiWidget)obj).get_Text() == toolSettings.infillType)
				{
					val3.set_SelectedLabel(toolSettings.infillType);
				}
				obj.add_Selected((EventHandler)delegate(object sender, EventArgs e)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Expected O, but got Unknown
					MenuItem val4 = (MenuItem)sender;
					toolSettings.infillType = ((GuiWidget)val4).get_Text();
					ActiveSliceSettings.Instance.Save();
				});
			}
			((GuiWidget)val).AddChild(rowFactory.MakeSettingName("Infill Type"), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private void MakeInfillStartingAngle()
		{
			FlowLayoutWidget val = rowFactory.NewRow();
			MHNumberEdit mHNumberEdit = rowFactory.MakeDoubleEdit(toolSettings.infillStartingAngle, "The angle of the infill for this tool, measured from the X axis.".Localize());
			((TextEditWidget)mHNumberEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				NumberEdit val2 = (NumberEdit)sender;
				toolSettings.infillStartingAngle = val2.get_Value();
				ActiveSliceSettings.Instance.Save();
			});
			((GuiWidget)val).AddChild(rowFactory.MakeSettingName("Starting Angle"), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)mHNumberEdit, -1);
			((GuiWidget)val).AddChild(rowFactory.MakeUnits("º"), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private void MakeInfill()
		{
			if (ActiveSliceSettings.Instance.GetValue<bool>("tool_specific_infill"))
			{
				MakeInfillPercentage();
				MakeInfillType();
				MakeInfillStartingAngle();
			}
		}

		private void MakeIntensity()
		{
			FlowLayoutWidget val = rowFactory.NewRow();
			MHNumberEdit numberEditWidget = rowFactory.MakeDoubleEdit(toolSettings.intensity);
			Action updateStyle = delegate
			{
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				if (numberEditWidget.ActuallNumberEdit.get_Value() == 0.0)
				{
					((GuiWidget)numberEditWidget.ActuallNumberEdit).set_BackgroundColor(RGBA_Bytes.Red);
					((GuiWidget)numberEditWidget.ActuallNumberEdit).set_ToolTipText("Value must be in 1 to 100 range.".Localize());
				}
				else
				{
					((GuiWidget)numberEditWidget.ActuallNumberEdit).set_BackgroundColor(RGBA_Bytes.White);
					((GuiWidget)numberEditWidget.ActuallNumberEdit).set_ToolTipText("Percent intensity for the tool.".Localize());
				}
			};
			((GuiWidget)numberEditWidget.ActuallNumberEdit).add_KeyUp((EventHandler<KeyEventArgs>)delegate(object sender, KeyEventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				NumberEdit val2 = (NumberEdit)sender;
				toolSettings.intensity = val2.get_Value();
				updateStyle();
			});
			((TextEditWidget)numberEditWidget.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				ActiveSliceSettings.Instance.Save();
			});
			updateStyle();
			((GuiWidget)val).AddChild(rowFactory.MakeSettingName("Intensity"), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)numberEditWidget, -1);
			((GuiWidget)val).AddChild(rowFactory.MakeUnits("Percent"), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private void MakeJerk()
		{
			FlowLayoutWidget val = rowFactory.NewRow();
			MHNumberEdit mHNumberEdit = rowFactory.MakeDoubleEdit(toolSettings.jerk, "Instantanious accelerration for toolhead when using this tool.".Localize());
			((TextEditWidget)mHNumberEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				NumberEdit val2 = (NumberEdit)sender;
				toolSettings.jerk = val2.get_Value();
				ActiveSliceSettings.Instance.Save();
			});
			((GuiWidget)val).AddChild(rowFactory.MakeSettingName("Jerk"), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)mHNumberEdit, -1);
			((GuiWidget)val).AddChild(rowFactory.MakeUnits("mm/s"), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private void MakeKeepWarm()
		{
			FlowLayoutWidget val = rowFactory.NewRow();
			MHNumberEdit mHNumberEdit = rowFactory.MakeDoubleEdit(toolSettings.keepWarm, "Inactive temperature to use with this extruder (zero is disabled).".Localize());
			((TextEditWidget)mHNumberEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				NumberEdit val2 = (NumberEdit)sender;
				toolSettings.keepWarm = val2.get_Value();
				ActiveSliceSettings.Instance.Save();
			});
			((GuiWidget)val).AddChild(rowFactory.MakeSettingName(" Inactive Temperature"), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)mHNumberEdit, -1);
			((GuiWidget)val).AddChild(rowFactory.MakeUnits("ºC"), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private void MakePeriod()
		{
			FlowLayoutWidget val = rowFactory.NewRow();
			MHNumberEdit numberEditWidget = rowFactory.MakeDoubleEdit(toolSettings.period);
			Action updateStyle = delegate
			{
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				if (numberEditWidget.ActuallNumberEdit.get_Value() < 0.1)
				{
					((GuiWidget)numberEditWidget.ActuallNumberEdit).set_BackgroundColor(RGBA_Bytes.Red);
					((GuiWidget)numberEditWidget.ActuallNumberEdit).set_ToolTipText("Value must be greater than 0.1.".Localize());
				}
				else
				{
					((GuiWidget)numberEditWidget.ActuallNumberEdit).set_BackgroundColor(RGBA_Bytes.White);
					((GuiWidget)numberEditWidget.ActuallNumberEdit).set_ToolTipText("PWM period length for micro valve.".Localize());
				}
			};
			((GuiWidget)numberEditWidget.ActuallNumberEdit).add_KeyUp((EventHandler<KeyEventArgs>)delegate(object sender, KeyEventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				NumberEdit val2 = (NumberEdit)sender;
				toolSettings.period = val2.get_Value();
				updateStyle();
				periodUpdated?.Invoke(this, null);
			});
			((TextEditWidget)numberEditWidget.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				ActiveSliceSettings.Instance.Save();
			});
			updateStyle();
			((GuiWidget)val).AddChild(rowFactory.MakeSettingName("Period"), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)numberEditWidget, -1);
			((GuiWidget)val).AddChild(rowFactory.MakeUnits("ms"), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private void MakePin()
		{
			FlowLayoutWidget val = rowFactory.NewRow();
			MHNumberEdit mHNumberEdit = rowFactory.MakeIntEdit(toolSettings.pin, "Pin on PCB for generic IO tool.".Localize());
			((TextEditWidget)mHNumberEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				NumberEdit val2 = (NumberEdit)sender;
				toolSettings.pin = (int)val2.get_Value();
				ActiveSliceSettings.Instance.Save();
			});
			((GuiWidget)val).AddChild(rowFactory.MakeSettingName("Pin"), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)mHNumberEdit, -1);
			((GuiWidget)val).AddChild(rowFactory.MakeUnits(""), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private bool UpdatePositionStyle(NumberEdit numberEdit, int newPosition, bool shouldRemove = true)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			bool result = false;
			if (shouldRemove && toolSettings.position != 0 && ((GuiWidget)numberEdit).get_BackgroundColor() == RGBA_Bytes.White)
			{
				toolSettings.RemovePosition();
			}
			if (newPosition == 0)
			{
				((GuiWidget)numberEdit).set_BackgroundColor(RGBA_Bytes.Red);
				((GuiWidget)numberEdit).set_ToolTipText("Please select a position for this tool on the Aether 1.".Localize());
				result = true;
			}
			else if (!toolSettings.TakePosition(newPosition))
			{
				((GuiWidget)numberEdit).set_BackgroundColor(RGBA_Bytes.Red);
				((GuiWidget)numberEdit).set_ToolTipText("Position invalid or already occupied by another tool.".Localize());
			}
			else
			{
				((GuiWidget)numberEdit).set_BackgroundColor(RGBA_Bytes.White);
				((GuiWidget)numberEdit).set_ToolTipText("The tool position on the Aether 1.".Localize());
				result = true;
			}
			return result;
		}

		private void MakePosition()
		{
			FlowLayoutWidget val = rowFactory.NewRow();
			MHNumberEdit numberEditWidget = rowFactory.MakeIntEdit(toolSettings.position);
			EventHandler theEvent = delegate
			{
				NumberEdit actuallNumberEdit = numberEditWidget.ActuallNumberEdit;
				if (toolSettings.position != (int)actuallNumberEdit.get_Value())
				{
					if (UpdatePositionStyle(actuallNumberEdit, (int)actuallNumberEdit.get_Value()))
					{
						toolSettings.position = (int)actuallNumberEdit.get_Value();
					}
					else
					{
						toolSettings.position = 0;
					}
					ActiveSliceSettings.Instance.Save();
				}
			};
			positionUpdateHandler = (EventHandler)Delegate.Combine(positionUpdateHandler, theEvent);
			((GuiWidget)numberEditWidget).add_Closed((EventHandler<ClosedEventArgs>)delegate
			{
				positionUpdateHandler = (EventHandler)Delegate.Remove(positionUpdateHandler, theEvent);
			});
			((GuiWidget)numberEditWidget.ActuallNumberEdit).add_KeyUp((EventHandler<KeyEventArgs>)delegate(object sender, KeyEventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				NumberEdit val2 = (NumberEdit)sender;
				if (UpdatePositionStyle(val2, (int)val2.get_Value()))
				{
					toolSettings.position = (int)val2.get_Value();
				}
				else
				{
					toolSettings.position = 0;
				}
				positionUpdateHandler = (EventHandler)Delegate.Remove(positionUpdateHandler, theEvent);
				positionUpdateHandler?.Invoke(this, null);
				positionUpdateHandler = (EventHandler)Delegate.Combine(positionUpdateHandler, theEvent);
			});
			((TextEditWidget)numberEditWidget.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				ActiveSliceSettings.Instance.Save();
				if (toolSettings.toolType == TOOL_TYPE.FFF)
				{
					UiThread.RunOnIdle((Action)ApplicationController.Instance.ReloadAdvancedControlsPanel);
				}
			});
			UpdatePositionStyle(numberEditWidget.ActuallNumberEdit, toolSettings.position, shouldRemove: false);
			((GuiWidget)val).AddChild(rowFactory.MakeSettingName("Position"), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)numberEditWidget, -1);
			((GuiWidget)val).AddChild(rowFactory.MakeUnits(""), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private void MakePSI()
		{
			FlowLayoutWidget val = rowFactory.NewRow();
			MHNumberEdit numberEditWidget = rowFactory.MakeDoubleEdit(toolSettings.psi);
			Action updateStyle = delegate
			{
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_012d: Unknown result type (might be due to invalid IL or missing references)
				//IL_015d: Unknown result type (might be due to invalid IL or missing references)
				if (numberEditWidget.ActuallNumberEdit.get_Value() == 0.0 && toolSettings.toolType == TOOL_TYPE.SYRINGE)
				{
					((GuiWidget)numberEditWidget.ActuallNumberEdit).set_BackgroundColor(RGBA_Bytes.White);
					((GuiWidget)numberEditWidget.ActuallNumberEdit).set_ToolTipText("Pressure is disabled for this tool.".Localize());
				}
				else if (numberEditWidget.ActuallNumberEdit.get_Value() < 2.0)
				{
					((GuiWidget)numberEditWidget.ActuallNumberEdit).set_BackgroundColor(RGBA_Bytes.Red);
					((GuiWidget)numberEditWidget.ActuallNumberEdit).set_ToolTipText("PSI value must begreater than 2 PSI.".Localize());
				}
				else if (numberEditWidget.ActuallNumberEdit.get_Value() > 40.0 && toolSettings.toolType == TOOL_TYPE.MICRO_VALVE)
				{
					((GuiWidget)numberEditWidget.ActuallNumberEdit).set_BackgroundColor(RGBA_Bytes.Red);
					((GuiWidget)numberEditWidget.ActuallNumberEdit).set_ToolTipText("Microvalve PSI must beless than 40.".Localize());
				}
				else if (numberEditWidget.ActuallNumberEdit.get_Value() > 100.0)
				{
					((GuiWidget)numberEditWidget.ActuallNumberEdit).set_BackgroundColor(RGBA_Bytes.Red);
					((GuiWidget)numberEditWidget.ActuallNumberEdit).set_ToolTipText("PSI value must beless than 100.".Localize());
				}
				else
				{
					((GuiWidget)numberEditWidget.ActuallNumberEdit).set_BackgroundColor(RGBA_Bytes.White);
					((GuiWidget)numberEditWidget.ActuallNumberEdit).set_ToolTipText("PSI value to use with this tool.".Localize());
				}
			};
			((GuiWidget)numberEditWidget.ActuallNumberEdit).add_KeyUp((EventHandler<KeyEventArgs>)delegate(object sender, KeyEventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				NumberEdit val2 = (NumberEdit)sender;
				toolSettings.psi = val2.get_Value();
				updateStyle();
			});
			((TextEditWidget)numberEditWidget.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				ActiveSliceSettings.Instance.Save();
			});
			updateStyle();
			((GuiWidget)val).AddChild(rowFactory.MakeSettingName("PSI"), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)numberEditWidget, -1);
			((GuiWidget)val).AddChild(rowFactory.MakeUnits("psi"), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private void MakeSkirt()
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Expected O, but got Unknown
			string text = ActiveSliceSettings.Instance.GetValue("skirts");
			if (text.Contains("mm"))
			{
				text = text.Replace("mm", "");
			}
			bool visible = text != "0";
			FlowLayoutWidget val = rowFactory.NewRow();
			((GuiWidget)val).set_Visible(visible);
			CheckBox val2 = new CheckBox("");
			((GuiWidget)val2).set_Name("Skirt Checkbox");
			((GuiWidget)val2).set_ToolTipText("Produce skirt for this tool.");
			((GuiWidget)val2).set_VAnchor((VAnchor)1);
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			val2.set_Checked(toolSettings.skirt == 1);
			CheckBox checkBoxWidget = val2;
			((GuiWidget)checkBoxWidget).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				toolSettings.skirt = (checkBoxWidget.get_Checked() ? 1 : 0);
				ActiveSliceSettings.Instance.Save();
			});
			((GuiWidget)val).AddChild(rowFactory.MakeSettingName("Generate Skirt"), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)checkBoxWidget, -1);
			((GuiWidget)val).AddChild(rowFactory.MakeUnits(""), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private void MakeSpeed()
		{
			FlowLayoutWidget val = rowFactory.NewRow();
			MHNumberEdit mHNumberEdit = rowFactory.MakeDoubleEdit(toolSettings.speed, "Speed for the toolhead to travel at when using this tool.".Localize());
			((TextEditWidget)mHNumberEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				NumberEdit val2 = (NumberEdit)sender;
				toolSettings.speed = val2.get_Value();
				ActiveSliceSettings.Instance.Save();
			});
			((GuiWidget)val).AddChild(rowFactory.MakeSettingName("Speed"), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)mHNumberEdit, -1);
			((GuiWidget)val).AddChild(rowFactory.MakeUnits("mm/s"), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private void MakeTemperature()
		{
			FlowLayoutWidget val = rowFactory.NewRow();
			MHNumberEdit mHNumberEdit = rowFactory.MakeDoubleEdit(toolSettings.temperature, "Temperature to use with this extruder.".Localize());
			((TextEditWidget)mHNumberEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				NumberEdit val2 = (NumberEdit)sender;
				toolSettings.temperature = val2.get_Value();
				ActiveSliceSettings.Instance.Save();
			});
			((GuiWidget)val).AddChild(rowFactory.MakeSettingName("Temperature"), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)mHNumberEdit, -1);
			((GuiWidget)val).AddChild(rowFactory.MakeUnits("ºC"), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private bool UpdateTemperaturePositionStyle(NumberEdit numberEdit, int newPosition, bool shouldRemove = true)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			bool result = false;
			if (shouldRemove && toolSettings.temperaturePosition != 0 && ((GuiWidget)numberEdit).get_BackgroundColor() == RGBA_Bytes.White)
			{
				Positions.Instance.ResetTemperaturePosition(toolSettings.temperaturePosition);
			}
			if (newPosition == 0)
			{
				((GuiWidget)numberEdit).set_BackgroundColor(RGBA_Bytes.Red);
				((GuiWidget)numberEdit).set_ToolTipText("Please select a temperature port for this tool on the Aether 1.".Localize());
				result = true;
			}
			else if (!Positions.Instance.SetTemperaturePosition(newPosition))
			{
				((GuiWidget)numberEdit).set_BackgroundColor(RGBA_Bytes.Red);
				((GuiWidget)numberEdit).set_ToolTipText("Temperature port invalid or already occupied by another tool.".Localize());
			}
			else
			{
				((GuiWidget)numberEdit).set_BackgroundColor(RGBA_Bytes.White);
				((GuiWidget)numberEdit).set_ToolTipText("The temperature port on the Aether 1.".Localize());
				result = true;
			}
			return result;
		}

		private void MakeTemperaturePosition()
		{
			FlowLayoutWidget val = rowFactory.NewRow();
			MHNumberEdit numberEditWidget = rowFactory.MakeIntEdit(toolSettings.temperaturePosition);
			EventHandler theEvent = delegate
			{
				NumberEdit actuallNumberEdit = numberEditWidget.ActuallNumberEdit;
				if (toolSettings.temperaturePosition != (int)actuallNumberEdit.get_Value())
				{
					if (UpdateTemperaturePositionStyle(actuallNumberEdit, (int)actuallNumberEdit.get_Value()))
					{
						toolSettings.temperaturePosition = (int)actuallNumberEdit.get_Value();
					}
					else
					{
						toolSettings.temperaturePosition = 0;
					}
					ActiveSliceSettings.Instance.Save();
				}
			};
			temperaturePositionUpdateHandler = (EventHandler)Delegate.Combine(temperaturePositionUpdateHandler, theEvent);
			((GuiWidget)numberEditWidget).add_Closed((EventHandler<ClosedEventArgs>)delegate
			{
				temperaturePositionUpdateHandler = (EventHandler)Delegate.Remove(temperaturePositionUpdateHandler, theEvent);
			});
			((GuiWidget)this).add_Closed((EventHandler<ClosedEventArgs>)delegate
			{
				Positions.Instance.ResetTemperaturePosition(toolSettings.temperaturePosition);
			});
			((GuiWidget)numberEditWidget.ActuallNumberEdit).add_KeyUp((EventHandler<KeyEventArgs>)delegate(object sender, KeyEventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				NumberEdit val2 = (NumberEdit)sender;
				if (UpdateTemperaturePositionStyle(val2, (int)val2.get_Value()))
				{
					toolSettings.temperaturePosition = (int)val2.get_Value();
				}
				else
				{
					toolSettings.temperaturePosition = 0;
				}
				temperaturePositionUpdateHandler = (EventHandler)Delegate.Remove(temperaturePositionUpdateHandler, theEvent);
				temperaturePositionUpdateHandler?.Invoke(this, null);
				temperaturePositionUpdateHandler = (EventHandler)Delegate.Combine(temperaturePositionUpdateHandler, theEvent);
			});
			((TextEditWidget)numberEditWidget.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				ActiveSliceSettings.Instance.Save();
				if (toolSettings.toolType == TOOL_TYPE.TSYRINGE)
				{
					UiThread.RunOnIdle((Action)ApplicationController.Instance.ReloadAdvancedControlsPanel);
				}
			});
			UpdateTemperaturePositionStyle(numberEditWidget.ActuallNumberEdit, toolSettings.temperaturePosition, shouldRemove: false);
			((GuiWidget)val).AddChild(rowFactory.MakeSettingName("Temperature Port"), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)numberEditWidget, -1);
			((GuiWidget)val).AddChild(rowFactory.MakeUnits(""), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private void MakeWidth()
		{
			FlowLayoutWidget val = rowFactory.NewRow();
			MHNumberEdit mHNumberEdit = rowFactory.MakeDoubleEdit(toolSettings.width, "Width of tool tip.".Localize());
			((TextEditWidget)mHNumberEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				NumberEdit val2 = (NumberEdit)sender;
				toolSettings.width = val2.get_Value();
				ActiveSliceSettings.Instance.Save();
			});
			((GuiWidget)val).AddChild(rowFactory.MakeSettingName("Width"), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)mHNumberEdit, -1);
			((GuiWidget)val).AddChild(rowFactory.MakeUnits("mm"), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}
	}
}
