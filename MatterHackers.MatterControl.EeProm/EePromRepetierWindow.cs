using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl.EeProm
{
	public class EePromRepetierWindow : CloseOnDisconnectWindow
	{
		protected TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private EePromRepetierStorage currentEePromSettings;

		private FlowLayoutWidget settingsColmun;

		private EventHandler unregisterEvents;

		private bool waitingForUiUpdate;

		private int currentTabIndex;

		public EePromRepetierWindow()
			: base(650.0 * GuiWidget.get_DeviceScale(), 480.0 * GuiWidget.get_DeviceScale())
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Expected O, but got Unknown
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Expected O, but got Unknown
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Expected O, but got Unknown
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Expected O, but got Unknown
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Expected O, but got Unknown
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			((SystemWindow)this).set_AlwaysOnTopOfMain(true);
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			currentEePromSettings = new EePromRepetierStorage();
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)val).set_Padding(new BorderDouble(3.0, 0.0));
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			GuiWidget val3 = AddDescription("Description".Localize());
			val3.set_Margin(new BorderDouble(3.0, 0.0, 0.0, 0.0));
			((GuiWidget)val2).AddChild(val3, -1);
			CreateSpacer(val2);
			GuiWidget val4 = (GuiWidget)new TextWidget("Value".Localize(), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			val4.set_VAnchor((VAnchor)2);
			val4.set_Margin(new BorderDouble(5.0, 0.0, 60.0, 0.0));
			((GuiWidget)val2).AddChild(val4, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			ScrollableWidget val5 = new ScrollableWidget(true);
			ScrollingArea scrollArea = val5.get_ScrollArea();
			((GuiWidget)scrollArea).set_HAnchor((HAnchor)(((GuiWidget)scrollArea).get_HAnchor() | 5));
			((GuiWidget)val5).AnchorAll();
			((GuiWidget)val5).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((GuiWidget)val).AddChild((GuiWidget)(object)val5, -1);
			settingsColmun = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)settingsColmun).set_HAnchor((HAnchor)13);
			((GuiWidget)val5).AddChild((GuiWidget)(object)settingsColmun, -1);
			FlowLayoutWidget val6 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val6).set_HAnchor((HAnchor)13);
			((GuiWidget)val6).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			Button val7 = textImageButtonFactory.Generate("Save To EEPROM".Localize());
			((GuiWidget)val7).set_Margin(new BorderDouble(0.0, 3.0));
			((GuiWidget)val7).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					currentEePromSettings.Save();
					currentEePromSettings.Clear();
					currentEePromSettings.eventAdded -= NewSettingReadFromPrinter;
					((GuiWidget)this).Close();
				});
			});
			((GuiWidget)val6).AddChild((GuiWidget)(object)val7, -1);
			CreateSpacer(val6);
			Button val8 = textImageButtonFactory.Generate("Import".Localize() + "...");
			((GuiWidget)val8).set_Margin(new BorderDouble(0.0, 3.0));
			((GuiWidget)val8).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_004b: Expected O, but got Unknown
					OpenFileDialogParams val12 = new OpenFileDialogParams("EEPROM Settings|*.ini", "", false, "", "");
					((FileDialogParams)val12).set_ActionButtonLabel("Import EEPROM Settings".Localize());
					((FileDialogParams)val12).set_Title("Import EEPROM".Localize());
					FileDialog.OpenFileDialog(val12, (Action<OpenFileDialogParams>)delegate(OpenFileDialogParams openParams)
					{
						if (!string.IsNullOrEmpty(((FileDialogParams)openParams).get_FileName()))
						{
							currentEePromSettings.Import(((FileDialogParams)openParams).get_FileName());
							RebuildUi();
						}
					});
				});
			});
			((GuiWidget)val6).AddChild((GuiWidget)(object)val8, -1);
			Button val9 = textImageButtonFactory.Generate("Export".Localize() + "...");
			((GuiWidget)val9).set_Margin(new BorderDouble(0.0, 3.0));
			((GuiWidget)val9).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					//IL_0014: Unknown result type (might be due to invalid IL or missing references)
					//IL_0019: Unknown result type (might be due to invalid IL or missing references)
					//IL_0029: Unknown result type (might be due to invalid IL or missing references)
					//IL_0039: Unknown result type (might be due to invalid IL or missing references)
					//IL_0055: Expected O, but got Unknown
					SaveFileDialogParams val11 = new SaveFileDialogParams("EEPROM Settings|*.ini", "", "", "");
					((FileDialogParams)val11).set_ActionButtonLabel("Export EEPROM Settings".Localize());
					((FileDialogParams)val11).set_Title("Export EEPROM".Localize());
					((FileDialogParams)val11).set_FileName("eeprom_settings.ini");
					FileDialog.SaveFileDialog(val11, (Action<SaveFileDialogParams>)delegate(SaveFileDialogParams saveParams)
					{
						if (!string.IsNullOrEmpty(((FileDialogParams)saveParams).get_FileName()))
						{
							currentEePromSettings.Export(((FileDialogParams)saveParams).get_FileName());
						}
					});
				});
			});
			((GuiWidget)val6).AddChild((GuiWidget)(object)val9, -1);
			Button val10 = textImageButtonFactory.Generate("Close".Localize());
			((GuiWidget)val10).set_Margin(new BorderDouble(10.0, 3.0, 0.0, 3.0));
			((GuiWidget)val10).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					currentEePromSettings.Clear();
					currentEePromSettings.eventAdded -= NewSettingReadFromPrinter;
					((GuiWidget)this).Close();
				});
			});
			((GuiWidget)val6).AddChild((GuiWidget)(object)val10, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val6, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			((SystemWindow)this).set_Title("Firmware EEPROM Settings".Localize());
			((SystemWindow)this).ShowAsSystemWindow();
			currentEePromSettings.Clear();
			PrinterConnectionAndCommunication.Instance.CommunicationUnconditionalFromPrinter.RegisterEvent((EventHandler)currentEePromSettings.Add, ref unregisterEvents);
			currentEePromSettings.eventAdded += NewSettingReadFromPrinter;
			currentEePromSettings.AskPrinterForSettings();
		}

		private static void CreateSpacer(FlowLayoutWidget buttonBar)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			GuiWidget val = new GuiWidget(1.0, 1.0, (SizeLimitsToSet)1);
			val.set_HAnchor((HAnchor)5);
			((GuiWidget)buttonBar).AddChild(val, -1);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			base.OnClosed(e);
		}

		private void NewSettingReadFromPrinter(object sender, EventArgs e)
		{
			if (e is EePromRepetierParameter && !waitingForUiUpdate)
			{
				waitingForUiUpdate = true;
				UiThread.RunOnIdle((Action)RebuildUi, 1.0);
			}
		}

		private void RebuildUi()
		{
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Expected O, but got Unknown
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			List<EePromRepetierParameter> list = new List<EePromRepetierParameter>();
			lock (currentEePromSettings.eePromSettingsList)
			{
				foreach (KeyValuePair<int, EePromRepetierParameter> eePromSettings in currentEePromSettings.eePromSettingsList)
				{
					list.Add(eePromSettings.Value);
				}
			}
			((GuiWidget)settingsColmun).CloseAllChildren();
			foreach (EePromRepetierParameter newSetting in list)
			{
				if (newSetting != null)
				{
					FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
					((GuiWidget)val).set_HAnchor((HAnchor)13);
					((GuiWidget)val).AddChild(AddDescription(newSetting.Description), -1);
					((GuiWidget)val).set_Padding(new BorderDouble(5.0, 0.0));
					if (((Collection<GuiWidget>)(object)((GuiWidget)settingsColmun).get_Children()).Count % 2 == 1)
					{
						((GuiWidget)val).set_BackgroundColor(new RGBA_Bytes(0, 0, 0, 30));
					}
					CreateSpacer(val);
					double.TryParse(newSetting.Value, out var result);
					MHNumberEdit valueEdit = new MHNumberEdit(result, 0.0, 0.0, 12.0, 80.0 * GuiWidget.get_DeviceScale(), 0.0, allowNegatives: true, allowDecimals: true);
					valueEdit.SelectAllOnFocus = true;
					((GuiWidget)valueEdit).set_TabIndex(currentTabIndex++);
					((GuiWidget)valueEdit).set_VAnchor((VAnchor)2);
					((TextEditWidget)valueEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
					{
						newSetting.Value = valueEdit.ActuallNumberEdit.get_Value().ToString();
					});
					((GuiWidget)val).AddChild((GuiWidget)(object)valueEdit, -1);
					((GuiWidget)settingsColmun).AddChild((GuiWidget)(object)val, -1);
				}
			}
			waitingForUiUpdate = false;
		}

		private GuiWidget AddDescription(string description)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Expected O, but got Unknown
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Expected O, but got Unknown
			GuiWidget val = new GuiWidget(340.0, 40.0, (SizeLimitsToSet)1);
			TextWidget val2 = new TextWidget(description, 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val2).set_VAnchor((VAnchor)2);
			val.AddChild((GuiWidget)(object)val2, -1);
			return val;
		}
	}
}
