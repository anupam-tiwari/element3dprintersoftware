using System;
using System.Collections.Generic;
using System.IO;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.ConfigurationPage.PrintLeveling;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.PolygonMesh.Processors;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PrintQueue
{
	public class ExportToFolderProcess
	{
		private List<PrintItem> allFilesToExport;

		private List<string> savedGCodeFileNames;

		private int itemCountBeingWorkedOn;

		private string exportPath;

		public int ItemCountBeingWorkedOn => itemCountBeingWorkedOn;

		public string ItemNameBeingWorkedOn
		{
			get
			{
				if (ItemCountBeingWorkedOn < allFilesToExport.Count)
				{
					return allFilesToExport[ItemCountBeingWorkedOn].Name;
				}
				return "";
			}
		}

		public int CountOfParts => allFilesToExport.Count;

		public event EventHandler UpdatePartStatus;

		public event EventHandler StartingNextPart;

		public event EventHandler DoneSaving;

		public ExportToFolderProcess(List<PrintItem> list, string exportPath)
		{
			allFilesToExport = list;
			this.exportPath = exportPath;
			itemCountBeingWorkedOn = 0;
		}

		public void Start()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			if (allFilesToExport.Count <= 0)
			{
				return;
			}
			if (this.StartingNextPart != null)
			{
				this.StartingNextPart(this, (EventArgs)new StringEventArgs(ItemNameBeingWorkedOn));
			}
			savedGCodeFileNames = new List<string>();
			foreach (PrintItem item in allFilesToExport)
			{
				PrintItemWrapper printItemWrapper = new PrintItemWrapper(item);
				string text = Path.GetExtension(printItemWrapper.FileLocation)!.ToUpper();
				if (text != "" && MeshFileIo.ValidFileExtensions().Contains(text))
				{
					SlicingQueue.Instance.QueuePartForSlicing(printItemWrapper);
					printItemWrapper.SlicingDone += sliceItem_Done;
					printItemWrapper.SlicingOutputMessage += printItemWrapper_SlicingOutputMessage;
				}
				else if (Path.GetExtension(printItemWrapper.FileLocation)!.ToUpper() == ".GCODE")
				{
					sliceItem_Done(printItemWrapper, null);
				}
			}
		}

		private void printItemWrapper_SlicingOutputMessage(object sender, EventArgs e)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			StringEventArgs e2 = (StringEventArgs)e;
			if (this.UpdatePartStatus != null)
			{
				this.UpdatePartStatus(this, (EventArgs)(object)e2);
			}
		}

		private void sliceItem_Done(object sender, EventArgs e)
		{
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Expected O, but got Unknown
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Expected O, but got Unknown
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Expected O, but got Unknown
			PrintItemWrapper printItemWrapper = (PrintItemWrapper)sender;
			printItemWrapper.SlicingDone -= sliceItem_Done;
			printItemWrapper.SlicingOutputMessage -= printItemWrapper_SlicingOutputMessage;
			if (File.Exists(printItemWrapper.FileLocation))
			{
				savedGCodeFileNames.Add(printItemWrapper.GetGCodePathAndFileName());
			}
			itemCountBeingWorkedOn++;
			if (itemCountBeingWorkedOn < allFilesToExport.Count)
			{
				if (this.StartingNextPart != null)
				{
					this.StartingNextPart(this, (EventArgs)new StringEventArgs(ItemNameBeingWorkedOn));
				}
				return;
			}
			if (this.UpdatePartStatus != null)
			{
				this.UpdatePartStatus(this, (EventArgs)new StringEventArgs("Calculating Total filament mm..."));
			}
			if (savedGCodeFileNames.Count <= 0)
			{
				return;
			}
			double num = 0.0;
			foreach (string savedGCodeFileName in savedGCodeFileNames)
			{
				string text = File.ReadAllText(savedGCodeFileName);
				if (text.Length <= 0)
				{
					continue;
				}
				string text2 = "filament used =";
				int num2 = text.IndexOf(text2);
				if (num2 > 0)
				{
					int num3 = Math.Min(text.IndexOf("\n", num2), text.IndexOf("mm", num2));
					if (num3 > 0 && double.TryParse(text.Substring(num2 + text2.Length, num3 - num2 - text2.Length), out var result))
					{
						num += result;
					}
				}
			}
			PrintLevelingData printLevelingData = ActiveSliceSettings.Instance.Helpers.GetPrintLevelingData();
			for (int i = 0; i < savedGCodeFileNames.Count; i++)
			{
				string text3 = savedGCodeFileNames[i];
				string path = Path.ChangeExtension(Path.GetFileName(allFilesToExport[i].Name), ".gcode");
				string text4 = Path.Combine(exportPath, path);
				if (ActiveSliceSettings.Instance.GetValue<bool>("print_leveling_enabled"))
				{
					GCodeFileLoaded gCodeFileLoaded = new GCodeFileLoaded(text3);
					for (int j = 0; j < gCodeFileLoaded.LineCount; j++)
					{
						PrinterMachineInstruction printerMachineInstruction = gCodeFileLoaded.Instruction(j);
						Vector3 position = printerMachineInstruction.Position;
						switch (printLevelingData.CurrentPrinterLevelingSystem)
						{
						case PrintLevelingData.LevelingSystem.Probe3Points:
							printerMachineInstruction.Line = LevelWizard3Point.ApplyLeveling(printerMachineInstruction.Line, position, printerMachineInstruction.movementType);
							break;
						case PrintLevelingData.LevelingSystem.Probe7PointRadial:
							printerMachineInstruction.Line = LevelWizard7PointRadial.ApplyLeveling(printerMachineInstruction.Line, position, printerMachineInstruction.movementType);
							break;
						case PrintLevelingData.LevelingSystem.Probe13PointRadial:
							printerMachineInstruction.Line = LevelWizard13PointRadial.ApplyLeveling(printerMachineInstruction.Line, position, printerMachineInstruction.movementType);
							break;
						default:
							throw new NotImplementedException();
						}
					}
					gCodeFileLoaded.Save(text4);
				}
				else
				{
					File.Copy(text3, text4, true);
				}
			}
			if (this.DoneSaving != null)
			{
				this.DoneSaving(this, (EventArgs)new StringEventArgs($"{num:0.0}"));
			}
		}
	}
}
