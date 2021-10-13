using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.PrinterCommunication.Io;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.MatterControl.Queue.OptionsMenu;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.PolygonMesh.Processors;

namespace MatterHackers.MatterControl
{
	public class ExportPrintItemWindow : SystemWindow
	{
		private CheckBox showInFolderAfterSave;

		private CheckBox applyLeveling;

		private PrintItemWrapper printItemWrapper;

		private string gcodePathAndFilenameToSave;

		private string x3gPathAndFilenameToSave;

		private bool partIsGCode;

		private string documentsPath;

		private string applyLevelingDuringExportString = "Apply leveling to G-Code during export".Localize();

		private EventHandler unregisterEvents;

		public ExportPrintItemWindow(PrintItemWrapper printItemWrapper)
			: this(400.0, 300.0)
		{
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			this.printItemWrapper = printItemWrapper;
			if (Path.GetExtension(printItemWrapper.FileLocation)!.ToUpper() == ".GCODE")
			{
				partIsGCode = true;
			}
			string arg = "Element".Localize();
			string arg2 = "Export File".Localize();
			string title = $"{arg}: {arg2}";
			((SystemWindow)this).set_Title(title);
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)this).set_Name("Export Item Window");
			CreateWindowContent();
			PrinterSettings.PrintLevelingEnabledChanged.RegisterEvent((EventHandler)ReloadAfterPrinterProfileChanged, ref unregisterEvents);
		}

		public void CreateWindowContent()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Expected O, but got Unknown
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Expected O, but got Unknown
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Expected O, but got Unknown
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Invalid comparison between Unknown and I4
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Invalid comparison between Unknown and I4
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_045c: Expected O, but got Unknown
			//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Expected O, but got Unknown
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Expected O, but got Unknown
			//IL_0504: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).RemoveAllChildren();
			TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_Padding(new BorderDouble(3.0, 0.0, 3.0, 5.0));
			((GuiWidget)val).AnchorAll();
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 3.0, 0.0, 0.0));
			((GuiWidget)val2).set_Padding(new BorderDouble(0.0, 3.0, 0.0, 3.0));
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			TextWidget val3 = new TextWidget("File export options:".Localize(), 0.0, 0.0, 14.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val3.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_VAnchor((VAnchor)1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val4).set_HAnchor((HAnchor)5);
			((GuiWidget)val4).set_VAnchor((VAnchor)5);
			((GuiWidget)val4).set_Padding(new BorderDouble(5.0));
			((GuiWidget)val4).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			bool flag = !partIsGCode;
			if (flag && printItemWrapper != null && (printItemWrapper.PrintItem.Protected || printItemWrapper.PrintItem.ReadOnly))
			{
				flag = false;
			}
			if (flag)
			{
				string arg = "Export as".Localize();
				string label = $"{arg} STL";
				Button val5 = textImageButtonFactory.Generate(label);
				((GuiWidget)val5).set_Name("Export as STL button");
				((GuiWidget)val5).set_HAnchor((HAnchor)1);
				((GuiWidget)val5).set_Cursor((Cursors)3);
				((GuiWidget)val5).add_Click((EventHandler<MouseEventArgs>)exportSTL_Click);
				((GuiWidget)val4).AddChild((GuiWidget)(object)val5, -1);
				string arg2 = "Export as".Localize();
				string label2 = $"{arg2} AMF";
				Button val6 = textImageButtonFactory.Generate(label2);
				((GuiWidget)val6).set_Name("Export as AMF button");
				((GuiWidget)val6).set_HAnchor((HAnchor)1);
				((GuiWidget)val6).set_Cursor((Cursors)3);
				((GuiWidget)val6).add_Click((EventHandler<MouseEventArgs>)exportAMF_Click);
				((GuiWidget)val4).AddChild((GuiWidget)(object)val6, -1);
			}
			bool flag2 = ActiveSliceSettings.Instance.PrinterSelected || partIsGCode;
			if (flag2)
			{
				string label3 = string.Format("{0} G-Code", "Export as".Localize());
				Button val7 = textImageButtonFactory.Generate(label3);
				((GuiWidget)val7).set_Name("Export as GCode Button");
				((GuiWidget)val7).set_HAnchor((HAnchor)1);
				((GuiWidget)val7).set_Cursor((Cursors)3);
				((GuiWidget)val7).add_Click((EventHandler<MouseEventArgs>)delegate
				{
					UiThread.RunOnIdle((Action)ExportGCode_Click);
				});
				((GuiWidget)val4).AddChild((GuiWidget)(object)val7, -1);
				Exception exception = default(Exception);
				Exception exception2 = default(Exception);
				foreach (ExportGcodePlugin plugin in new PluginFinder<ExportGcodePlugin>((string)null, (IComparer<ExportGcodePlugin>)null).Plugins)
				{
					if (!plugin.EnabledForCurrentPart(printItemWrapper))
					{
						continue;
					}
					string label4 = plugin.GetButtonText().Localize();
					Button val8 = textImageButtonFactory.Generate(label4);
					((GuiWidget)val8).set_HAnchor((HAnchor)1);
					((GuiWidget)val8).set_Cursor((Cursors)3);
					((GuiWidget)val8).add_Click((EventHandler<MouseEventArgs>)delegate
					{
						UiThread.RunOnIdle((Action)delegate
						{
							//IL_0025: Unknown result type (might be due to invalid IL or missing references)
							//IL_002a: Unknown result type (might be due to invalid IL or missing references)
							//IL_0035: Unknown result type (might be due to invalid IL or missing references)
							//IL_004b: Unknown result type (might be due to invalid IL or missing references)
							//IL_007a: Expected O, but got Unknown
							((GuiWidget)this).Close();
							SaveFileDialogParams val12 = new SaveFileDialogParams(plugin.GetExtensionFilter(), "", "", "");
							((FileDialogParams)val12).set_Title("Element: Export File");
							((FileDialogParams)val12).set_FileName(printItemWrapper.Name);
							((FileDialogParams)val12).set_ActionButtonLabel("Export");
							FileDialog.SaveFileDialog(val12, (Action<SaveFileDialogParams>)delegate(SaveFileDialogParams saveParam)
							{
								if (Path.GetExtension(((FileDialogParams)saveParam).get_FileName()) == "")
								{
									SaveFileDialogParams obj = saveParam;
									((FileDialogParams)obj).set_FileName(((FileDialogParams)obj).get_FileName() + plugin.GetFileExtension());
								}
								if (partIsGCode)
								{
									try
									{
										plugin.Generate(printItemWrapper.FileLocation, ((FileDialogParams)saveParam).get_FileName());
									}
									catch (Exception ex)
									{
										exception = ex;
										UiThread.RunOnIdle((Action)delegate
										{
											StyledMessageBox.ShowMessageBox(null, exception.Message, "Couldn't save file".Localize());
										});
									}
								}
								else
								{
									SlicingQueue.Instance.QueuePartForSlicing(printItemWrapper);
									printItemWrapper.SlicingDone += delegate(object printItem, EventArgs eventArgs)
									{
										PrintItemWrapper printItemWrapper = (PrintItemWrapper)printItem;
										if (File.Exists(printItemWrapper.GetGCodePathAndFileName()))
										{
											try
											{
												plugin.Generate(printItemWrapper.GetGCodePathAndFileName(), ((FileDialogParams)saveParam).get_FileName());
											}
											catch (Exception ex2)
											{
												exception2 = ex2;
												UiThread.RunOnIdle((Action)delegate
												{
													StyledMessageBox.ShowMessageBox(null, exception2.Message, "Couldn't save file".Localize());
												});
											}
										}
									};
								}
							});
						});
					});
					((GuiWidget)val4).AddChild((GuiWidget)(object)val8, -1);
				}
			}
			((GuiWidget)val4).AddChild((GuiWidget)(object)new VerticalSpacer(), -1);
			if (flag2 && ActiveSliceSettings.Instance.GetValue<bool>("print_leveling_enabled"))
			{
				applyLeveling = new CheckBox(LocalizedString.Get(applyLevelingDuringExportString), ActiveTheme.get_Instance().get_PrimaryTextColor(), 10.0);
				applyLeveling.set_Checked(true);
				((GuiWidget)applyLeveling).set_HAnchor((HAnchor)1);
				((GuiWidget)applyLeveling).set_Cursor((Cursors)3);
				((GuiWidget)val4).AddChild((GuiWidget)(object)applyLeveling, -1);
			}
			if ((int)OsInformation.get_OperatingSystem() == 1 || (int)OsInformation.get_OperatingSystem() == 3)
			{
				showInFolderAfterSave = new CheckBox("Show file in folder after save".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 10.0);
				((GuiWidget)showInFolderAfterSave).set_HAnchor((HAnchor)1);
				((GuiWidget)showInFolderAfterSave).set_Cursor((Cursors)3);
				((GuiWidget)val4).AddChild((GuiWidget)(object)showInFolderAfterSave, -1);
			}
			if (!flag2)
			{
				string arg3 = "Note".Localize();
				string arg4 = "To enable GCode export, select a printer profile.".Localize();
				TextWidget val9 = new TextWidget($"{arg3}: {arg4}", 0.0, 0.0, 10.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
				((GuiWidget)val9).set_HAnchor((HAnchor)1);
				((GuiWidget)val4).AddChild((GuiWidget)(object)val9, -1);
			}
			FlowLayoutWidget val10 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)val10).set_HAnchor((HAnchor)5);
			((GuiWidget)val10).set_Padding(new BorderDouble(0.0, 3.0));
			Button val11 = textImageButtonFactory.Generate("Cancel".Localize());
			((GuiWidget)val11).set_Name("Export Item Window Cancel Button");
			((GuiWidget)val11).set_Cursor((Cursors)3);
			((GuiWidget)val11).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				((GuiWidget)this).CloseOnIdle();
			});
			((GuiWidget)val10).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val10).AddChild((GuiWidget)(object)val11, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val10, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private string Get8Name(string longName)
		{
			longName.Replace(' ', '_');
			return longName.Substring(0, Math.Min(longName.Length, 8));
		}

		private void ExportGCode_Click()
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Expected O, but got Unknown
			if (!ActiveSliceSettings.Instance.IsValid() && !partIsGCode)
			{
				((GuiWidget)this).Close();
				return;
			}
			SaveFileDialogParams val = new SaveFileDialogParams("Export GCode|*.gcode", "", "Export GCode", "");
			((FileDialogParams)val).set_Title("Element: Export File");
			((FileDialogParams)val).set_ActionButtonLabel("Export");
			((FileDialogParams)val).set_FileName(Path.GetFileNameWithoutExtension(printItemWrapper.Name));
			((GuiWidget)this).Close();
			FileDialog.SaveFileDialog(val, (Action<SaveFileDialogParams>)onExportGcodeFileSelected);
		}

		private void onExportGcodeFileSelected(SaveFileDialogParams saveParams)
		{
			if (!string.IsNullOrEmpty(((FileDialogParams)saveParams).get_FileName()))
			{
				ExportGcodeCommandLineUtility(((FileDialogParams)saveParams).get_FileName());
			}
		}

		public void ExportGcodeCommandLineUtility(string nameOfFile)
		{
			try
			{
				if (!string.IsNullOrEmpty(nameOfFile))
				{
					gcodePathAndFilenameToSave = nameOfFile;
					if (Path.GetExtension(gcodePathAndFilenameToSave) == "")
					{
						File.Delete(gcodePathAndFilenameToSave);
						gcodePathAndFilenameToSave += ".gcode";
					}
					string value = Path.GetExtension(printItemWrapper.FileLocation)!.ToUpper();
					if (MeshFileIo.ValidFileExtensions().Contains(value))
					{
						SlicingQueue.Instance.QueuePartForSlicing(printItemWrapper);
						printItemWrapper.SlicingDone += sliceItem_Done;
					}
					else if (partIsGCode)
					{
						SaveGCodeToNewLocation(printItemWrapper.FileLocation, gcodePathAndFilenameToSave);
					}
				}
			}
			catch
			{
			}
		}

		private void SaveGCodeToNewLocation(string gcodeFilename, string dest)
		{
			try
			{
				GCodeFileStream internalStream = new GCodeFileStream(GCodeFile.Load(gcodeFilename));
				bool num = ActiveSliceSettings.Instance.GetValue<bool>("print_leveling_enabled") && applyLeveling.get_Checked();
				QueuedCommandsStream queuedCommandsStream = new QueuedCommandsStream(internalStream);
				GCodeStream gCodeStream = (num ? new ProcessWriteRegexStream(new PrintLevelingStream(queuedCommandsStream, activePrinting: false), queuedCommandsStream) : new ProcessWriteRegexStream(queuedCommandsStream, queuedCommandsStream));
				using (StreamWriter streamWriter = new StreamWriter(dest))
				{
					for (string text = gCodeStream.ReadLine(); text != null; text = gCodeStream.ReadLine())
					{
						if (text.Trim().Length > 0)
						{
							streamWriter.WriteLine(text);
						}
					}
				}
				ShowFileIfRequested(dest);
			}
			catch (Exception ex)
			{
				Exception e = ex;
				UiThread.RunOnIdle((Action)delegate
				{
					StyledMessageBox.ShowMessageBox(null, e.Message, "Couldn't save file".Localize());
				});
			}
		}

		private void ShowFileIfRequested(string filename)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			if ((int)OsInformation.get_OperatingSystem() == 1 && showInFolderAfterSave.get_Checked())
			{
				WindowsFormsAbstract.ShowFileInFolder(filename);
			}
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			printItemWrapper.SlicingDone -= sliceItem_Done;
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((SystemWindow)this).OnClosed(e);
		}

		private void ReloadAfterPrinterProfileChanged(object sender, EventArgs e)
		{
			CreateWindowContent();
		}

		private void exportAMF_Click(object sender, EventArgs mouseEvent)
		{
			UiThread.RunOnIdle((Action)delegate
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Expected O, but got Unknown
				SaveFileDialogParams val = new SaveFileDialogParams("Save as AMF|*.amf", documentsPath, "", "");
				((FileDialogParams)val).set_Title("Element: Export File");
				((FileDialogParams)val).set_ActionButtonLabel("Export");
				((FileDialogParams)val).set_FileName(printItemWrapper.Name);
				((GuiWidget)this).Close();
				FileDialog.SaveFileDialog(val, (Action<SaveFileDialogParams>)onExportAmfFileSelected);
			});
		}

		private async void onExportAmfFileSelected(SaveFileDialogParams saveParams)
		{
			await Task.Run(delegate
			{
				SaveAmf(saveParams);
			});
		}

		private void SaveAmf(SaveFileDialogParams saveParams)
		{
			try
			{
				if (string.IsNullOrEmpty(((FileDialogParams)saveParams).get_FileName()))
				{
					return;
				}
				string text = ((FileDialogParams)saveParams).get_FileName();
				if (text == null || !(text != ""))
				{
					return;
				}
				if (Path.GetExtension(text) == "")
				{
					File.Delete(text);
					text += ".amf";
				}
				if (Path.GetExtension(printItemWrapper.FileLocation)!.ToUpper() == Path.GetExtension(text)!.ToUpper())
				{
					File.Copy(printItemWrapper.FileLocation, text, true);
				}
				else if (!MeshFileIo.Save(MeshFileIo.Load(printItemWrapper.FileLocation, (ReportProgressRatio)null), text, (MeshOutputSettings)null, (ReportProgressRatio)null))
				{
					UiThread.RunOnIdle((Action)delegate
					{
						StyledMessageBox.ShowMessageBox(null, "STL to AMF conversion failed", "Couldn't save file".Localize());
					});
				}
				ShowFileIfRequested(text);
			}
			catch (Exception ex)
			{
				Exception e = ex;
				UiThread.RunOnIdle((Action)delegate
				{
					StyledMessageBox.ShowMessageBox(null, e.Message, "Couldn't save file".Localize());
				});
			}
		}

		private void exportSTL_Click(object sender, EventArgs mouseEvent)
		{
			UiThread.RunOnIdle((Action)delegate
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Expected O, but got Unknown
				SaveFileDialogParams val = new SaveFileDialogParams("Save as STL|*.stl", "", "", "");
				((FileDialogParams)val).set_Title("Element: Export File");
				((FileDialogParams)val).set_ActionButtonLabel("Export");
				((FileDialogParams)val).set_FileName(printItemWrapper.Name);
				((GuiWidget)this).Close();
				FileDialog.SaveFileDialog(val, (Action<SaveFileDialogParams>)onExportStlFileSelected);
			});
		}

		private async void onExportStlFileSelected(SaveFileDialogParams saveParams)
		{
			await Task.Run(delegate
			{
				SaveStl(saveParams);
			});
		}

		private void SaveStl(SaveFileDialogParams saveParams)
		{
			try
			{
				if (string.IsNullOrEmpty(((FileDialogParams)saveParams).get_FileName()))
				{
					return;
				}
				string text = ((FileDialogParams)saveParams).get_FileName();
				if (text == null || !(text != ""))
				{
					return;
				}
				if (Path.GetExtension(text) == "")
				{
					File.Delete(text);
					text += ".stl";
				}
				if (Path.GetExtension(printItemWrapper.FileLocation)!.ToUpper() == Path.GetExtension(text)!.ToUpper())
				{
					File.Copy(printItemWrapper.FileLocation, text, true);
				}
				else if (!MeshFileIo.Save(MeshFileIo.Load(printItemWrapper.FileLocation, (ReportProgressRatio)null), text, (MeshOutputSettings)null, (ReportProgressRatio)null))
				{
					UiThread.RunOnIdle((Action)delegate
					{
						StyledMessageBox.ShowMessageBox(null, "AMF to STL conversion failed", "Couldn't save file".Localize());
					});
				}
				ShowFileIfRequested(text);
			}
			catch (Exception ex)
			{
				Exception e = ex;
				UiThread.RunOnIdle((Action)delegate
				{
					StyledMessageBox.ShowMessageBox(null, e.Message, "Couldn't save file".Localize());
				});
			}
		}

		private void sliceItem_Done(object sender, EventArgs e)
		{
			PrintItemWrapper printItemWrapper = (PrintItemWrapper)sender;
			this.printItemWrapper.SlicingDone -= sliceItem_Done;
			SaveGCodeToNewLocation(printItemWrapper.GetGCodePathAndFileName(), gcodePathAndFilenameToSave);
		}

		private void x3gItemSlice_Complete(object sender, EventArgs e)
		{
			PrintItemWrapper printItemWrapper = (PrintItemWrapper)sender;
			this.printItemWrapper.SlicingDone -= x3gItemSlice_Complete;
			if (File.Exists(printItemWrapper.GetGCodePathAndFileName()))
			{
				generateX3GfromGcode(printItemWrapper.GetGCodePathAndFileName(), x3gPathAndFilenameToSave);
			}
		}

		private string getGpxExectutablePath()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected I4, but got Unknown
			OSType operatingSystem = OsInformation.get_OperatingSystem();
			switch (operatingSystem - 1)
			{
			case 0:
			{
				string text = Path.Combine("..", "gpx.exe");
				if (!File.Exists(text))
				{
					text = Path.Combine(".", "gpx.exe");
				}
				return Path.GetFullPath(text);
			}
			case 1:
				return Path.Combine(ApplicationDataStorage.Instance.ApplicationPath, "gpx");
			case 2:
				return Path.GetFullPath(Path.Combine(".", "gpx.exe"));
			default:
				throw new NotImplementedException();
			}
		}

		private void generateX3GfromGcode(string gcodeInputPath, string x3gOutputPath, string machineType = "r2")
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			string gpxExectutablePath = getGpxExectutablePath();
			string arguments = string.Format("-p -m {2} \"{0}\" \"{1}\" ", gcodeInputPath, x3gOutputPath, machineType);
			ProcessStartInfo val = new ProcessStartInfo(gpxExectutablePath);
			val.set_WindowStyle((ProcessWindowStyle)1);
			val.set_Arguments(arguments);
			Process.Start(val);
			ShowFileIfRequested(x3gOutputPath);
		}
	}
}
