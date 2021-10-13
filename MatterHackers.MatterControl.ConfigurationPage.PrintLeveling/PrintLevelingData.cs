using System;
using System.Collections.Generic;
using System.Linq;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class PrintLevelingData
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public enum LevelingSystem
		{
			Probe3Points,
			Probe7PointRadial,
			Probe13PointRadial,
			Probe3x3Mesh
		}

		private PrinterSettings printerProfile;

		public List<Vector3> SampledPositions = new List<Vector3>
		{
			default(Vector3),
			default(Vector3),
			default(Vector3)
		};

		public LevelingSystem CurrentPrinterLevelingSystem => printerProfile.GetValue("print_leveling_solution") switch
		{
			"7 Point Disk" => LevelingSystem.Probe7PointRadial, 
			"13 Point Disk" => LevelingSystem.Probe13PointRadial, 
			"3x3 Mesh" => LevelingSystem.Probe3x3Mesh, 
			_ => LevelingSystem.Probe3Points, 
		};

		public PrintLevelingData(PrinterSettings printerProfile)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			this.printerProfile = printerProfile;
		}

		internal static PrintLevelingData Create(PrinterSettings printerProfile, string jsonData, string depricatedPositionsCsv3ByXYZ = "")
		{
			if (!string.IsNullOrEmpty(jsonData))
			{
				PrintLevelingData printLevelingData = JsonConvert.DeserializeObject<PrintLevelingData>(jsonData);
				printLevelingData.printerProfile = printerProfile;
				return printLevelingData;
			}
			if (!string.IsNullOrEmpty(depricatedPositionsCsv3ByXYZ))
			{
				PrintLevelingData printLevelingData2 = new PrintLevelingData(ActiveSliceSettings.Instance);
				printLevelingData2.printerProfile = printerProfile;
				printLevelingData2.ParseDepricatedPrintLevelingMeasuredPositions(depricatedPositionsCsv3ByXYZ);
				return printLevelingData2;
			}
			return new PrintLevelingData(ActiveSliceSettings.Instance)
			{
				printerProfile = printerProfile
			};
		}

		private void ParseDepricatedPrintLevelingMeasuredPositions(string depricatedPositionsCsv3ByXYZ)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			SampledPositions = new List<Vector3>(3);
			if (depricatedPositionsCsv3ByXYZ == null)
			{
				return;
			}
			string[] array = depricatedPositionsCsv3ByXYZ.Split(new char[1]
			{
				','
			});
			if (array.Length == 9)
			{
				for (int i = 0; i < 3; i++)
				{
					Vector3 item = default(Vector3);
					item.x = double.Parse(array[i]);
					item.y = double.Parse(array[3 + i]);
					item.z = double.Parse(array[6 + i]);
					SampledPositions.Add(item);
				}
			}
		}

		public bool HasBeenRunAndEnabled()
		{
			if (!ActiveSliceSettings.Instance.GetValue<bool>("print_leveling_enabled"))
			{
				return false;
			}
			foreach (var item in Enumerable.Select((IEnumerable<_003C_003Ef__AnonymousType0<IGrouping<Vector3, Vector3>, int>>)Enumerable.OrderByDescending(Enumerable.Select(Enumerable.GroupBy<Vector3, Vector3>((IEnumerable<Vector3>)SampledPositions, (Func<Vector3, Vector3>)((Vector3 x) => x)), (IGrouping<Vector3, Vector3> g) => new
			{
				g = g,
				count = Enumerable.Count<Vector3>((IEnumerable<Vector3>)g)
			}), _003C_003Eh__TransparentIdentifier0 => _003C_003Eh__TransparentIdentifier0.count), _003C_003Eh__TransparentIdentifier0 => new
			{
				Value = _003C_003Eh__TransparentIdentifier0.g.get_Key(),
				Count = _003C_003Eh__TransparentIdentifier0.count
			}))
			{
				if (item.Count > 1)
				{
					return false;
				}
			}
			switch (CurrentPrinterLevelingSystem)
			{
			case LevelingSystem.Probe3Points:
				if (SampledPositions.Count != 3)
				{
					return false;
				}
				break;
			case LevelingSystem.Probe7PointRadial:
				if (SampledPositions.Count != 7)
				{
					return false;
				}
				break;
			case LevelingSystem.Probe13PointRadial:
				if (SampledPositions.Count != 13)
				{
					return false;
				}
				break;
			case LevelingSystem.Probe3x3Mesh:
				if (SampledPositions.Count != 9)
				{
					return false;
				}
				break;
			default:
				throw new NotImplementedException();
			}
			return true;
		}

		public bool SamplesAreSame(List<Vector3> sampledPositions)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if (sampledPositions.Count == SampledPositions.Count)
			{
				for (int i = 0; i < sampledPositions.Count; i++)
				{
					if (sampledPositions[i] != SampledPositions[i])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
	}
}
