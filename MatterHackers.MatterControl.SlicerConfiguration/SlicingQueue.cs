using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.MatterControl.SettingsManagement;
using MatterHackers.MatterSlice;
using MatterHackers.PolygonMesh;
using MatterHackers.PolygonMesh.Processors;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class SlicingQueue
	{
		private static Thread slicePartThread = null;

		private static List<PrintItemWrapper> listOfSlicingItems = new List<PrintItemWrapper>();

		private static bool haltSlicingThread = false;

		private static List<SliceEngineInfo> availableSliceEngines;

		private static SlicingQueue instance;

		public static List<bool> extrudersUsed = new List<bool>();

		public static bool runInProcess = false;

		private static Process slicerProcess = null;

		private static PrintItemWrapper itemCurrentlySlicing;

		public static List<SliceEngineInfo> AvailableSliceEngines
		{
			get
			{
				if (availableSliceEngines == null)
				{
					availableSliceEngines = new List<SliceEngineInfo>();
					Slic3rInfo slic3rInfo = new Slic3rInfo();
					if (slic3rInfo.Exists())
					{
						availableSliceEngines.Add(slic3rInfo);
					}
					CuraEngineInfo curaEngineInfo = new CuraEngineInfo();
					if (curaEngineInfo.Exists())
					{
						availableSliceEngines.Add(curaEngineInfo);
					}
					MatterSliceInfo matterSliceInfo = new MatterSliceInfo();
					if (matterSliceInfo.Exists())
					{
						availableSliceEngines.Add(matterSliceInfo);
					}
				}
				return availableSliceEngines;
			}
		}

		public static SlicingQueue Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new SlicingQueue();
				}
				return instance;
			}
		}

		private static SliceEngineInfo getSliceEngineInfoByType(SlicingEngineTypes engineType)
		{
			foreach (SliceEngineInfo availableSliceEngine in AvailableSliceEngines)
			{
				if (availableSliceEngine.GetSliceEngineType() == engineType)
				{
					return availableSliceEngine;
				}
			}
			return null;
		}

		private SlicingQueue()
		{
			if (slicePartThread == null)
			{
				slicePartThread = new Thread(new ThreadStart(CreateSlicedPartsThread));
				slicePartThread.Name = "slicePartThread";
				slicePartThread.IsBackground = true;
				slicePartThread.Start();
			}
		}

		public void QueuePartForSlicing(PrintItemWrapper itemToQueue)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			itemToQueue.DoneSlicing = false;
			string arg = "Preparing to slice model".Localize();
			string text = $"{arg}...";
			itemToQueue.OnSlicingOutputMessage((EventArgs)new StringEventArgs(text));
			lock (listOfSlicingItems)
			{
				listOfSlicingItems.Add(itemToQueue);
			}
		}

		public void ShutDownSlicingThread()
		{
			haltSlicingThread = true;
		}

		private static string macQuotes(string textLine)
		{
			if (textLine.StartsWith("\"") && textLine.EndsWith("\""))
			{
				return textLine;
			}
			return "\"" + textLine.Replace("\"", "\\\"") + "\"";
		}

		private static string getSlicerFullPath()
		{
			return getSliceEngineInfoByType(ActiveSliceSettings.Instance.Helpers.ActiveSliceEngineType())?.GetEnginePath();
		}

		public static string[] GetStlFileLocations(string fileToSlice, bool doMergeInSlicer, ref string mergeRules)
		{
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Expected O, but got Unknown
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			extrudersUsed.Clear();
			int value = ActiveSliceSettings.Instance.GetValue<int>("extruder_count");
			for (int i = 0; i < value; i++)
			{
				extrudersUsed.Add(item: false);
			}
			if (ActiveSliceSettings.Instance.GetValue<bool>("support_material") && ActiveSliceSettings.Instance.GetValue<int>("support_material_extruder") != 0)
			{
				int index = Math.Max(0, Math.Min(ActiveSliceSettings.Instance.GetValue<int>("extruder_count") - 1, ActiveSliceSettings.Instance.GetValue<int>("support_material_extruder") - 1));
				extrudersUsed[index] = true;
			}
			if (ActiveSliceSettings.Instance.GetValue<bool>("create_raft") && ActiveSliceSettings.Instance.GetValue<int>("raft_extruder") != 0)
			{
				int index2 = Math.Max(0, Math.Min(ActiveSliceSettings.Instance.GetValue<int>("extruder_count") - 1, ActiveSliceSettings.Instance.GetValue<int>("raft_extruder") - 1));
				extrudersUsed[index2] = true;
			}
			switch (Path.GetExtension(fileToSlice)!.ToUpper())
			{
			case ".STL":
			case ".GCODE":
				extrudersUsed[0] = true;
				return new string[1]
				{
					fileToSlice
				};
			case ".AMF":
			{
				List<MeshGroup> list = MeshFileIo.Load(fileToSlice, (ReportProgressRatio)null);
				if (list != null)
				{
					AxisAlignedBoundingBox val = null;
					foreach (MeshGroup item in list)
					{
						val = ((val != null) ? (val + item.GetAxisAlignedBoundingBox()) : item.GetAxisAlignedBoundingBox());
					}
					List<MeshGroup> list2 = new List<MeshGroup>();
					for (int j = 0; j < value; j++)
					{
						list2.Add(new MeshGroup());
					}
					int num = 0;
					foreach (MeshGroup item2 in list)
					{
						foreach (Mesh mesh in item2.get_Meshes())
						{
							MeshMaterialData val2 = MeshMaterialData.Get(mesh);
							int num2 = Math.Max(0, val2.MaterialIndex - 1);
							num = Math.Max(num, num2);
							if (num2 >= value)
							{
								extrudersUsed[0] = true;
								list2[0].get_Meshes().Add(mesh);
							}
							else
							{
								extrudersUsed[num2] = true;
								list2[num2].get_Meshes().Add(mesh);
							}
						}
					}
					int num3 = 0;
					List<string> list3 = new List<string>();
					for (int k = 0; k < list2.Count; k++)
					{
						MeshGroup val3 = list2[k];
						List<int> list4 = new List<int>();
						list4.Add(k + 1);
						if (k == 0)
						{
							for (int l = value + 1; l < num + 2; l++)
							{
								list4.Add(l);
							}
						}
						if (doMergeInSlicer)
						{
							int count = val3.get_Meshes().Count;
							if (count > 0)
							{
								for (int m = 0; m < count; m++)
								{
									Mesh val4 = val3.get_Meshes()[m];
									if (m % 2 == 0)
									{
										mergeRules += StringHelper.FormatWith("({0}", new object[1]
										{
											num3
										});
									}
									else if (m < count - 1)
									{
										mergeRules += StringHelper.FormatWith(",({0}", new object[1]
										{
											num3
										});
									}
									else
									{
										mergeRules += StringHelper.FormatWith(",{0}", new object[1]
										{
											num3
										});
									}
									int materialIndex = MeshMaterialData.Get(val4).MaterialIndex;
									if (list4.Contains(materialIndex))
									{
										list3.Add(SaveAndGetFilenameForMesh(val4));
									}
									num3++;
								}
								for (int n = 0; n < count; n++)
								{
									mergeRules += ")";
								}
							}
							else
							{
								int num4 = 0;
								for (int num5 = k + 1; num5 < list2.Count; num5++)
								{
									num4 += list2[num5].get_Meshes().Count;
								}
								if (num4 > 0)
								{
									mergeRules += $"({num3})";
								}
								Mesh val5 = PlatonicSolids.CreateCube(0.001, 0.001, 0.001);
								val5.Translate(new Vector3(0.0, 0.0, val.minXYZ.z));
								list3.Add(SaveAndGetFilenameForMesh(val5));
								num3++;
							}
						}
						else
						{
							list3.Add(SaveAndGetFilenameForMaterial(val3, list4));
						}
					}
					return list3.ToArray();
				}
				return new string[1]
				{
					""
				};
			}
			default:
				return new string[1]
				{
					""
				};
			}
		}

		private static string SaveAndGetFilenameForMeshGroup(MeshGroup meshGroupToSave)
		{
			string path = Path.ChangeExtension(Path.GetRandomFileName(), ".amf");
			string text = Path.Combine(ApplicationDataStorage.ApplicationUserDataPath, "data", "temp", "group_seperation");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			string text2 = Path.Combine(text, path);
			MeshFileIo.Save(meshGroupToSave, text2, (MeshOutputSettings)null);
			return text2;
		}

		private static string SaveAndGetFilenameForMesh(Mesh meshToSave)
		{
			string path = Path.ChangeExtension(Path.GetRandomFileName(), ".stl");
			string text = Path.Combine(ApplicationDataStorage.ApplicationUserDataPath, "data", "temp", "amf_to_stl");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			string text2 = Path.Combine(text, path);
			MeshFileIo.Save(meshToSave, text2, (MeshOutputSettings)null);
			return text2;
		}

		private static string SaveAndGetFilenameForMaterial(MeshGroup extruderMeshGroup, List<int> materialIndexsToSaveInThisSTL)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			string path = Path.ChangeExtension(Path.GetRandomFileName(), ".stl");
			string text = Path.Combine(ApplicationDataStorage.ApplicationUserDataPath, "data", "temp", "amf_to_stl");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			MeshOutputSettings val = new MeshOutputSettings();
			val.MaterialIndexsToSave = materialIndexsToSaveInThisSTL;
			string text2 = Path.Combine(text, path);
			MeshFileIo.Save(extruderMeshGroup, text2, val);
			return text2;
		}

		private static void CreateSlicedPartsThread()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			while (!haltSlicingThread)
			{
				if (listOfSlicingItems.Count > 0)
				{
					PrintItemWrapper itemToSlice = listOfSlicingItems[0];
					string gCodePathAndFileName = itemToSlice.GetGCodePathAndFileName();
					if (!ActiveSliceSettings.Instance.GetValue<bool>("export_one_at_a_time") || Path.GetExtension(itemToSlice.FileLocation)!.ToUpper() != ".AMF")
					{
						ActuallySlicePart(itemToSlice, itemToSlice.FileLocation, gCodePathAndFileName);
					}
					else
					{
						List<MeshGroup> list = MeshFileIo.Load(itemToSlice.FileLocation, (ReportProgressRatio)null);
						if (list != null)
						{
							List<string> list2 = new List<string>();
							string value = ActiveSliceSettings.Instance.GetValue("start_gcode");
							string value2 = ActiveSliceSettings.Instance.GetValue("end_gcode");
							ActiveSliceSettings.Instance.SetValue("end_gcode", "");
							bool flag = true;
							for (int i = 0; i < list.Count; i++)
							{
								if (i == list.Count - 1)
								{
									ActiveSliceSettings.Instance.SetValue("end_gcode", value2);
								}
								string text = Path.Combine(Path.GetDirectoryName(gCodePathAndFileName), Path.GetFileNameWithoutExtension(gCodePathAndFileName) + "-" + i + ".gcode");
								list2.Add(text);
								string fileLocation = SaveAndGetFilenameForMeshGroup(list[i]);
								ActuallySlicePart(itemToSlice, fileLocation, text);
								if (flag)
								{
									ActiveSliceSettings.Instance.SetValue("start_gcode", "");
									flag = false;
								}
							}
							ActiveSliceSettings.Instance.SetValue("start_gcode", value);
							using StreamWriter streamWriter = new StreamWriter(gCodePathAndFileName, append: false);
							for (int j = 0; j < list2.Count; j++)
							{
								streamWriter.WriteLine(File.ReadAllText(list2[j]));
							}
						}
					}
					try
					{
						if (File.Exists(gCodePathAndFileName))
						{
							string text2 = Path.Combine(ApplicationDataStorage.Instance.GCodeOutputPath, "config_" + ActiveSliceSettings.Instance.GetLongHashCode() + ".ini");
							EngineMappingsMatterSlice.WriteSliceSettingsFile(text2);
							bool flag2 = false;
							int num = 32000;
							using (Stream stream = File.OpenRead(gCodePathAndFileName))
							{
								byte[] array = new byte[num];
								stream.Seek(Math.Max(0L, stream.Length - num), SeekOrigin.Begin);
								stream.Read(array, 0, num);
								if (Encoding.UTF8.GetString(array).Contains("GCode settings used"))
								{
									flag2 = true;
								}
							}
							if (!flag2)
							{
								using StreamWriter streamWriter2 = File.AppendText(gCodePathAndFileName);
								string text3 = "Element";
								if (OemSettings.Instance.WindowTitleExtra != null && OemSettings.Instance.WindowTitleExtra.Trim().Length > 0)
								{
									text3 += StringHelper.FormatWith(" - {0}", new object[1]
									{
										OemSettings.Instance.WindowTitleExtra
									});
								}
								streamWriter2.WriteLine(StringHelper.FormatWith("; {0} Version {1} Build {2} : GCode settings used", new object[3]
								{
									text3,
									VersionInfo.Instance.ReleaseVersion,
									VersionInfo.Instance.BuildVersion
								}));
								streamWriter2.WriteLine(StringHelper.FormatWith("; Date {0} Time {1}:{2:00}", new object[3]
								{
									DateTime.Now.Date,
									DateTime.Now.Hour,
									DateTime.Now.Minute
								}));
								foreach (string item in File.ReadLines(text2))
								{
									streamWriter2.WriteLine(StringHelper.FormatWith("; {0}", new object[1]
									{
										item
									}));
								}
							}
							string[] array2 = File.ReadAllLines(gCodePathAndFileName);
							using StreamWriter streamWriter3 = File.CreateText(gCodePathAndFileName);
							string[] array3 = array2;
							foreach (string text4 in array3)
							{
								if (text4.Trim().Length > 0)
								{
									streamWriter3.WriteLine(text4);
								}
							}
						}
					}
					catch (Exception)
					{
					}
					UiThread.RunOnIdle((Action)delegate
					{
						itemToSlice.CurrentlySlicing = false;
						itemToSlice.DoneSlicing = true;
					});
					lock (listOfSlicingItems)
					{
						listOfSlicingItems.RemoveAt(0);
					}
				}
				Thread.Sleep(100);
			}
		}

		private static void ActuallySlicePart(PrintItemWrapper itemToSlice, string fileLocation, string gcodePathAndFileName)
		{
			string mergeRules = "";
			string[] stlFileLocations = GetStlFileLocations(fileLocation, doMergeInSlicer: true, ref mergeRules);
			if (!File.Exists(stlFileLocations[0]))
			{
				return;
			}
			itemToSlice.CurrentlySlicing = true;
			string text = Path.Combine(ApplicationDataStorage.Instance.GCodeOutputPath, "config_" + ActiveSliceSettings.Instance.GetLongHashCode() + ".ini");
			bool flag = itemToSlice.IsGCodeFileComplete(gcodePathAndFileName);
			if (!File.Exists(gcodePathAndFileName) || !flag)
			{
				string text2 = "";
				EngineMappingsMatterSlice.WriteSliceSettingsFile(text);
				text2 = ((!(mergeRules == "")) ? (StringHelper.FormatWith("-b {0} -v -o \"", new object[1]
				{
					mergeRules
				}) + gcodePathAndFileName + "\" -c \"" + text + "\"") : ("-v -o \"" + gcodePathAndFileName + "\" -c \"" + text + "\""));
				string[] array = stlFileLocations;
				foreach (string str in array)
				{
					text2 = text2 + " \"" + str + "\"";
				}
				itemCurrentlySlicing = itemToSlice;
				LogOutput.GetLogWrites = (EventHandler)Delegate.Combine(LogOutput.GetLogWrites, new EventHandler(SendProgressToItem));
				MatterSlice.ProcessArgs(text2);
				LogOutput.GetLogWrites = (EventHandler)Delegate.Remove(LogOutput.GetLogWrites, new EventHandler(SendProgressToItem));
				itemCurrentlySlicing = null;
			}
		}

		private static void SendProgressToItem(object sender, EventArgs args)
		{
			string message = sender as string;
			if (message == null)
			{
				return;
			}
			message = message.Replace("=>", "").Trim();
			if (message.Contains(".gcode"))
			{
				message = "Saving intermediate file";
			}
			message += "...";
			UiThread.RunOnIdle((Action)delegate
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Expected O, but got Unknown
				if (itemCurrentlySlicing != null)
				{
					itemCurrentlySlicing.OnSlicingOutputMessage((EventArgs)new StringEventArgs(message));
				}
			});
		}

		internal void CancelCurrentSlicing()
		{
			if (slicerProcess != null)
			{
				lock (slicerProcess)
				{
					if (slicerProcess != null && !slicerProcess.get_HasExited())
					{
						slicerProcess.Kill();
					}
				}
			}
			else
			{
				MatterSlice.Stop();
			}
		}
	}
}
