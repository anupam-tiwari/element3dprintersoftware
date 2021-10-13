using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl.EeProm
{
	public class EePromMarlinWindow : CloseOnDisconnectWindow
	{
		private EePromMarlinSettings currentEePromSettings;

		private MHNumberEdit stepsPerMmX;

		private MHNumberEdit stepsPerMmY;

		private MHNumberEdit stepsPerMmZ;

		private MHNumberEdit stepsPerMmE;

		private MHNumberEdit maxFeedrateMmPerSX;

		private MHNumberEdit maxFeedrateMmPerSY;

		private MHNumberEdit maxFeedrateMmPerSZ;

		private MHNumberEdit maxFeedrateMmPerSE;

		private MHNumberEdit maxAccelerationMmPerSSqrdX;

		private MHNumberEdit maxAccelerationMmPerSSqrdY;

		private MHNumberEdit maxAccelerationMmPerSSqrdZ;

		private MHNumberEdit maxAccelerationMmPerSSqrdE;

		private MHNumberEdit acceleration;

		private MHNumberEdit retractAcceleration;

		private MHNumberEdit pidP;

		private MHNumberEdit pidI;

		private MHNumberEdit pidD;

		private MHNumberEdit homingOffsetX;

		private MHNumberEdit homingOffsetY;

		private MHNumberEdit homingOffsetZ;

		private MHNumberEdit minFeedrate;

		private MHNumberEdit minTravelFeedrate;

		private MHNumberEdit minSegmentTime;

		private MHNumberEdit maxXYJerk;

		private MHNumberEdit maxZJerk;

		private EventHandler unregisterEvents;

		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private double maxWidthOfLeftStuff;

		private List<GuiWidget> leftStuffToSize = new List<GuiWidget>();

		private int currentTabIndex;

		public EePromMarlinWindow()
			: base(650.0 * GuiWidget.get_DeviceScale(), 480.0 * GuiWidget.get_DeviceScale())
		{
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Expected O, but got Unknown
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Expected O, but got Unknown
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Expected O, but got Unknown
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Expected O, but got Unknown
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Expected O, but got Unknown
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Expected O, but got Unknown
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0584: Unknown result type (might be due to invalid IL or missing references)
			((SystemWindow)this).set_AlwaysOnTopOfMain(true);
			((SystemWindow)this).set_Title("Marlin Firmware EEPROM Settings".Localize());
			currentEePromSettings = new EePromMarlinSettings();
			currentEePromSettings.eventAdded += SetUiToPrinterSettings;
			GuiWidget val = new GuiWidget();
			val.AnchorAll();
			val.set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			val.set_Padding(new BorderDouble(3.0, 0.0));
			GuiWidget val2 = new GuiWidget(0.0, 500.0, (SizeLimitsToSet)1);
			val2.set_VAnchor((VAnchor)1);
			val2.set_HAnchor((HAnchor)5);
			val2.set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			val2.set_Padding(new BorderDouble(0.0, 0.0, 0.0, 3.0));
			val.AddChild(val2, -1);
			double num = 0.0;
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_VAnchor((VAnchor)12);
			((GuiWidget)val3).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 3.0));
			Button val4 = textImageButtonFactory.Generate("Reset to Factory Defaults".Localize());
			((GuiWidget)val3).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)val4).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				currentEePromSettings.SetPrinterToFactorySettings();
				currentEePromSettings.Update();
			});
			val.AddChild((GuiWidget)(object)val3, -1);
			num = ((GuiWidget)val3).get_Height();
			FlowLayoutWidget val5 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val5).set_VAnchor((VAnchor)12);
			((GuiWidget)val5).set_HAnchor((HAnchor)5);
			((GuiWidget)val5).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((GuiWidget)val5).set_Padding(new BorderDouble(0.0, 0.0, 0.0, 3.0));
			((GuiWidget)val5).set_Margin(new BorderDouble(0.0, 0.0, 0.0, num));
			((GuiWidget)val5).AddChild(Create4FieldSet("Steps per mm:".Localize(), "X:", ref stepsPerMmX, "Y:", ref stepsPerMmY, "Z:", ref stepsPerMmZ, "E:", ref stepsPerMmE), -1);
			((GuiWidget)val5).AddChild(Create4FieldSet("Maximum feedrates [mm/s]:".Localize(), "X:", ref maxFeedrateMmPerSX, "Y:", ref maxFeedrateMmPerSY, "Z:", ref maxFeedrateMmPerSZ, "E:", ref maxFeedrateMmPerSE), -1);
			((GuiWidget)val5).AddChild(Create4FieldSet("Maximum Acceleration [mm/s²]:".Localize(), "X:", ref maxAccelerationMmPerSSqrdX, "Y:", ref maxAccelerationMmPerSSqrdY, "Z:", ref maxAccelerationMmPerSSqrdZ, "E:", ref maxAccelerationMmPerSSqrdE), -1);
			((GuiWidget)val5).AddChild(CreateField("Acceleration:".Localize(), ref acceleration), -1);
			((GuiWidget)val5).AddChild(CreateField("Retract Acceleration:".Localize(), ref retractAcceleration), -1);
			((GuiWidget)val5).AddChild(Create3FieldSet("PID settings:".Localize(), "P:", ref pidP, "I:", ref pidI, "D:", ref pidD), -1);
			((GuiWidget)val5).AddChild(Create3FieldSet("Homing Offset:".Localize(), "X:", ref homingOffsetX, "Y:", ref homingOffsetY, "Z:", ref homingOffsetZ), -1);
			((GuiWidget)val5).AddChild(CreateField("Min feedrate [mm/s]:".Localize(), ref minFeedrate), -1);
			((GuiWidget)val5).AddChild(CreateField("Min travel feedrate [mm/s]:".Localize(), ref minTravelFeedrate), -1);
			((GuiWidget)val5).AddChild(CreateField("Minimum segment time [ms]:".Localize(), ref minSegmentTime), -1);
			((GuiWidget)val5).AddChild(CreateField("Maximum X-Y jerk [mm/s]:".Localize(), ref maxXYJerk), -1);
			((GuiWidget)val5).AddChild(CreateField("Maximum Z jerk [mm/s]:".Localize(), ref maxZJerk), -1);
			GuiWidget val6 = new GuiWidget(1.0, 1.0, (SizeLimitsToSet)1);
			val6.set_VAnchor((VAnchor)5);
			((GuiWidget)val5).AddChild(val6, -1);
			val.AddChild((GuiWidget)(object)val5, -1);
			FlowLayoutWidget val7 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val7).set_HAnchor((HAnchor)13);
			((GuiWidget)val7).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)val7).set_Margin(new BorderDouble(0.0, 3.0));
			Button val8 = textImageButtonFactory.Generate("Save to EEProm".Localize());
			((GuiWidget)val7).AddChild((GuiWidget)(object)val8, -1);
			((GuiWidget)val8).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					SaveSettingsToActive();
					currentEePromSettings.SaveToEeProm();
					((GuiWidget)this).Close();
				});
			});
			CreateSpacer(val7);
			Button val9 = textImageButtonFactory.Generate("Import".Localize() + "...");
			((GuiWidget)val9).set_Margin(new BorderDouble(0.0, 3.0));
			((GuiWidget)val9).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_004b: Expected O, but got Unknown
					OpenFileDialogParams val13 = new OpenFileDialogParams("EEPROM Settings|*.ini", "", false, "", "");
					((FileDialogParams)val13).set_ActionButtonLabel("Import EEPROM Settings".Localize());
					((FileDialogParams)val13).set_Title("Import EEPROM".Localize());
					FileDialog.OpenFileDialog(val13, (Action<OpenFileDialogParams>)delegate(OpenFileDialogParams openParams)
					{
						if (!string.IsNullOrEmpty(((FileDialogParams)openParams).get_FileName()))
						{
							currentEePromSettings.Import(((FileDialogParams)openParams).get_FileName());
							SetUiToPrinterSettings(null, null);
						}
					});
				});
			});
			((GuiWidget)val7).AddChild((GuiWidget)(object)val9, -1);
			Button val10 = textImageButtonFactory.Generate("Export".Localize() + "...");
			((GuiWidget)val10).set_Margin(new BorderDouble(0.0, 3.0));
			((GuiWidget)val10).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					//IL_002c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0031: Unknown result type (might be due to invalid IL or missing references)
					//IL_0041: Unknown result type (might be due to invalid IL or missing references)
					//IL_0051: Unknown result type (might be due to invalid IL or missing references)
					//IL_006e: Expected O, but got Unknown
					string defaultFileNameNoPath = "eeprom_settings.ini";
					SaveFileDialogParams val12 = new SaveFileDialogParams("EEPROM Settings|*.ini", "", "", "");
					((FileDialogParams)val12).set_ActionButtonLabel("Export EEPROM Settings".Localize());
					((FileDialogParams)val12).set_Title("Export EEPROM".Localize());
					((FileDialogParams)val12).set_FileName(defaultFileNameNoPath);
					FileDialog.SaveFileDialog(val12, (Action<SaveFileDialogParams>)delegate(SaveFileDialogParams saveParams)
					{
						if (!string.IsNullOrEmpty(((FileDialogParams)saveParams).get_FileName()) && ((FileDialogParams)saveParams).get_FileName() != defaultFileNameNoPath)
						{
							currentEePromSettings.Export(((FileDialogParams)saveParams).get_FileName());
						}
					});
				});
			});
			((GuiWidget)val7).AddChild((GuiWidget)(object)val10, -1);
			Button val11 = textImageButtonFactory.Generate("Close".Localize());
			((GuiWidget)val7).AddChild((GuiWidget)(object)val11, -1);
			((GuiWidget)val11).add_Click((EventHandler<MouseEventArgs>)buttonAbort_Click);
			val.AddChild((GuiWidget)(object)val7, -1);
			PrinterConnectionAndCommunication.Instance.CommunicationUnconditionalFromPrinter.RegisterEvent((EventHandler)currentEePromSettings.Add, ref unregisterEvents);
			((GuiWidget)this).AddChild(val, -1);
			((SystemWindow)this).ShowAsSystemWindow();
			currentEePromSettings.Update();
			foreach (GuiWidget item in leftStuffToSize)
			{
				item.set_Width(maxWidthOfLeftStuff);
			}
		}

		private GuiWidget CreateMHNumEdit(ref MHNumberEdit numberEditToCreate)
		{
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			numberEditToCreate = new MHNumberEdit(0.0, 0.0, 0.0, 12.0, 80.0, 0.0, allowNegatives: true, allowDecimals: true);
			numberEditToCreate.SelectAllOnFocus = true;
			((GuiWidget)numberEditToCreate).set_VAnchor((VAnchor)2);
			((GuiWidget)numberEditToCreate).set_Margin(new BorderDouble(3.0, 0.0));
			return (GuiWidget)(object)numberEditToCreate;
		}

		private GuiWidget CreateField(string label, ref MHNumberEdit field1)
		{
			MHNumberEdit field2 = null;
			return Create4FieldSet(label, "", ref field1, null, ref field2, null, ref field2, null, ref field2);
		}

		private GuiWidget Create3FieldSet(string label, string field1Label, ref MHNumberEdit field1, string field2Label, ref MHNumberEdit field2, string field3Label, ref MHNumberEdit field3)
		{
			MHNumberEdit field4 = null;
			return Create4FieldSet(label, field1Label, ref field1, field2Label, ref field2, field3Label, ref field3, null, ref field4);
		}

		private GuiWidget CreateTextField(string label)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Expected O, but got Unknown
			GuiWidget val = (GuiWidget)new TextWidget(label, 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			val.set_VAnchor((VAnchor)2);
			val.set_HAnchor((HAnchor)4);
			GuiWidget val2 = new GuiWidget(val.get_Height(), 24.0, (SizeLimitsToSet)1);
			val2.AddChild(val, -1);
			return val2;
		}

		private GuiWidget Create4FieldSet(string label, string field1Label, ref MHNumberEdit field1, string field2Label, ref MHNumberEdit field2, string field3Label, ref MHNumberEdit field3, string field4Label, ref MHNumberEdit field4)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Expected O, but got Unknown
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_Margin(new BorderDouble(3.0));
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			TextWidget val2 = new TextWidget(label, 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val2).set_VAnchor((VAnchor)2);
			maxWidthOfLeftStuff = Math.Max(maxWidthOfLeftStuff, ((GuiWidget)val2).get_Width());
			GuiWidget val3 = new GuiWidget(((GuiWidget)val2).get_Width(), ((GuiWidget)val2).get_Height(), (SizeLimitsToSet)1);
			val3.set_Margin(new BorderDouble(3.0, 0.0));
			val3.AddChild((GuiWidget)(object)val2, -1);
			leftStuffToSize.Add(val3);
			((GuiWidget)val).AddChild(val3, -1);
			((GuiWidget)val).AddChild(CreateTextField(field1Label), -1);
			GuiWidget val4 = CreateMHNumEdit(ref field1);
			val4.set_TabIndex(GetNextTabIndex());
			((GuiWidget)val).AddChild(val4, -1);
			if (field2Label != null)
			{
				((GuiWidget)val).AddChild(CreateTextField(field2Label), -1);
				GuiWidget val5 = CreateMHNumEdit(ref field2);
				val5.set_TabIndex(GetNextTabIndex());
				((GuiWidget)val).AddChild(val5, -1);
			}
			if (field3Label != null)
			{
				((GuiWidget)val).AddChild(CreateTextField(field3Label), -1);
				GuiWidget val6 = CreateMHNumEdit(ref field3);
				val6.set_TabIndex(GetNextTabIndex());
				((GuiWidget)val).AddChild(val6, -1);
			}
			if (field4Label != null)
			{
				((GuiWidget)val).AddChild(CreateTextField(field4Label), -1);
				GuiWidget val7 = CreateMHNumEdit(ref field4);
				val7.set_TabIndex(GetNextTabIndex());
				((GuiWidget)val).AddChild(val7, -1);
			}
			return (GuiWidget)(object)val;
		}

		private int GetNextTabIndex()
		{
			return currentTabIndex++;
		}

		private static void CreateSpacer(FlowLayoutWidget buttonBar)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			GuiWidget val = new GuiWidget(1.0, 1.0, (SizeLimitsToSet)1);
			val.set_HAnchor((HAnchor)5);
			((GuiWidget)buttonBar).AddChild(val, -1);
		}

		private void buttonAbort_Click(object sender, EventArgs e)
		{
			UiThread.RunOnIdle((Action)base.Close);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			base.OnClosed(e);
		}

		private void SetUiToPrinterSettings(object sender, EventArgs e)
		{
			((GuiWidget)stepsPerMmX).set_Text(currentEePromSettings.SX);
			((GuiWidget)stepsPerMmY).set_Text(currentEePromSettings.SY);
			((GuiWidget)stepsPerMmZ).set_Text(currentEePromSettings.SZ);
			((GuiWidget)stepsPerMmE).set_Text(currentEePromSettings.SE);
			((GuiWidget)maxFeedrateMmPerSX).set_Text(currentEePromSettings.FX);
			((GuiWidget)maxFeedrateMmPerSY).set_Text(currentEePromSettings.FY);
			((GuiWidget)maxFeedrateMmPerSZ).set_Text(currentEePromSettings.FZ);
			((GuiWidget)maxFeedrateMmPerSE).set_Text(currentEePromSettings.FE);
			((GuiWidget)maxAccelerationMmPerSSqrdX).set_Text(currentEePromSettings.AX);
			((GuiWidget)maxAccelerationMmPerSSqrdY).set_Text(currentEePromSettings.AY);
			((GuiWidget)maxAccelerationMmPerSSqrdZ).set_Text(currentEePromSettings.AZ);
			((GuiWidget)maxAccelerationMmPerSSqrdE).set_Text(currentEePromSettings.AE);
			((GuiWidget)acceleration).set_Text(currentEePromSettings.ACC);
			((GuiWidget)retractAcceleration).set_Text(currentEePromSettings.RACC);
			((GuiWidget)minFeedrate).set_Text(currentEePromSettings.AVS);
			((GuiWidget)minTravelFeedrate).set_Text(currentEePromSettings.AVT);
			((GuiWidget)minSegmentTime).set_Text(currentEePromSettings.AVB);
			((GuiWidget)maxXYJerk).set_Text(currentEePromSettings.AVX);
			((GuiWidget)maxZJerk).set_Text(currentEePromSettings.AVZ);
			MHNumberEdit mHNumberEdit = pidP;
			MHNumberEdit mHNumberEdit2 = pidI;
			bool hasPID;
			((GuiWidget)pidD).set_Enabled(hasPID = currentEePromSettings.hasPID);
			bool enabled;
			((GuiWidget)mHNumberEdit2).set_Enabled(enabled = hasPID);
			((GuiWidget)mHNumberEdit).set_Enabled(enabled);
			((GuiWidget)pidP).set_Text(currentEePromSettings.PPID);
			((GuiWidget)pidI).set_Text(currentEePromSettings.IPID);
			((GuiWidget)pidD).set_Text(currentEePromSettings.DPID);
			((GuiWidget)homingOffsetX).set_Text(currentEePromSettings.hox);
			((GuiWidget)homingOffsetY).set_Text(currentEePromSettings.hoy);
			((GuiWidget)homingOffsetZ).set_Text(currentEePromSettings.hoz);
		}

		private void SaveSettingsToActive()
		{
			currentEePromSettings.SX = ((GuiWidget)stepsPerMmX).get_Text();
			currentEePromSettings.SY = ((GuiWidget)stepsPerMmY).get_Text();
			currentEePromSettings.SZ = ((GuiWidget)stepsPerMmZ).get_Text();
			currentEePromSettings.SE = ((GuiWidget)stepsPerMmE).get_Text();
			currentEePromSettings.FX = ((GuiWidget)maxFeedrateMmPerSX).get_Text();
			currentEePromSettings.FY = ((GuiWidget)maxFeedrateMmPerSY).get_Text();
			currentEePromSettings.FZ = ((GuiWidget)maxFeedrateMmPerSZ).get_Text();
			currentEePromSettings.FE = ((GuiWidget)maxFeedrateMmPerSE).get_Text();
			currentEePromSettings.AX = ((GuiWidget)maxAccelerationMmPerSSqrdX).get_Text();
			currentEePromSettings.AY = ((GuiWidget)maxAccelerationMmPerSSqrdY).get_Text();
			currentEePromSettings.AZ = ((GuiWidget)maxAccelerationMmPerSSqrdZ).get_Text();
			currentEePromSettings.AE = ((GuiWidget)maxAccelerationMmPerSSqrdE).get_Text();
			currentEePromSettings.ACC = ((GuiWidget)acceleration).get_Text();
			currentEePromSettings.RACC = ((GuiWidget)retractAcceleration).get_Text();
			currentEePromSettings.AVS = ((GuiWidget)minFeedrate).get_Text();
			currentEePromSettings.AVT = ((GuiWidget)minTravelFeedrate).get_Text();
			currentEePromSettings.AVB = ((GuiWidget)minSegmentTime).get_Text();
			currentEePromSettings.AVX = ((GuiWidget)maxXYJerk).get_Text();
			currentEePromSettings.AVZ = ((GuiWidget)maxZJerk).get_Text();
			currentEePromSettings.PPID = ((GuiWidget)pidP).get_Text();
			currentEePromSettings.IPID = ((GuiWidget)pidI).get_Text();
			currentEePromSettings.DPID = ((GuiWidget)pidD).get_Text();
			currentEePromSettings.HOX = ((GuiWidget)homingOffsetX).get_Text();
			currentEePromSettings.HOY = ((GuiWidget)homingOffsetY).get_Text();
			currentEePromSettings.HOZ = ((GuiWidget)homingOffsetZ).get_Text();
			currentEePromSettings.Save();
		}
	}
}
