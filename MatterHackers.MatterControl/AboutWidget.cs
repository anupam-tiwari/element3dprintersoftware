using System;
using System.Collections.Generic;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrintLibrary.Provider;
using MatterHackers.MatterControl.PrintQueue;

namespace MatterHackers.MatterControl
{
	public class AboutWidget : GuiWidget
	{
		private static HashSet<string> folderNamesToPreserve;

		public AboutWidget()
			: this()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Expected O, but got Unknown
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			((GuiWidget)this).set_VAnchor((VAnchor)4);
			((GuiWidget)this).set_Padding(new BorderDouble(5.0));
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_Name("AboutPageCustomInfo");
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_VAnchor((VAnchor)13);
			((GuiWidget)val).set_Padding(new BorderDouble(5.0, 10.0, 5.0, 0.0));
			if (UserSettings.Instance.IsTouchScreen)
			{
				((GuiWidget)val).AddChild((GuiWidget)(object)new UpdateControlView(), -1);
			}
			((GuiWidget)val).AddChild(new GuiWidget(1.0, 10.0, (SizeLimitsToSet)1), -1);
			string text = Path.Combine("OEMSettings", "AboutPage.html");
			HtmlWidget htmlWidget = new HtmlWidget(StaticData.get_Instance().ReadAllText(text), ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val).AddChild((GuiWidget)(object)htmlWidget, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		public static void DeleteCacheData(int daysOldToDelete)
		{
			if (LibraryProviderSQLite.PreloadingCalibrationFiles)
			{
				return;
			}
			HashSet<string> val = new HashSet<string>();
			CleanDirectory(ApplicationDataStorage.Instance.GCodeOutputPath, val, daysOldToDelete);
			string applicationUserDataPath = ApplicationDataStorage.ApplicationUserDataPath;
			RemoveDirectory(Path.Combine(applicationUserDataPath, "updates"));
			foreach (PrintItemWrapper printItem in QueueData.Instance.PrintItems)
			{
				string fileLocation = printItem.FileLocation;
				if (!val.Contains(fileLocation))
				{
					val.Add(fileLocation);
					val.Add(PartThumbnailWidget.GetImageFileName(printItem));
				}
			}
			foreach (PrintItem item2 in LibraryProviderSQLite.GetAllPrintItemsRecursive())
			{
				PrintItemWrapper item = new PrintItemWrapper(item2);
				string fileLocation2 = item2.FileLocation;
				if (!val.Contains(fileLocation2))
				{
					val.Add(fileLocation2);
					val.Add(PartThumbnailWidget.GetImageFileName(item));
				}
			}
			if (val.get_Count() > 0)
			{
				CleanDirectory(applicationUserDataPath, val, daysOldToDelete);
			}
		}

		public string CreateCenteredButton(string content)
		{
			throw new NotImplementedException();
		}

		public string CreateLinkButton(string content)
		{
			throw new NotImplementedException();
		}

		public string DoToUpper(string content)
		{
			throw new NotImplementedException();
		}

		public string DoTranslate(string content)
		{
			throw new NotImplementedException();
		}

		public string GetBuildString(string content)
		{
			return VersionInfo.Instance.BuildVersion;
		}

		public string GetVersionString(string content)
		{
			return VersionInfo.Instance.ReleaseVersion;
		}

		private static int CleanDirectory(string path, HashSet<string> referencedFilePaths, int daysOldToDelete)
		{
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			foreach (string item in Directory.EnumerateDirectories(path))
			{
				if (CleanDirectory(item, referencedFilePaths, daysOldToDelete) == 0 && !folderNamesToPreserve.Contains(Path.GetFileName(item)))
				{
					try
					{
						Directory.Delete(item);
					}
					catch (Exception)
					{
					}
				}
				else
				{
					num++;
				}
			}
			foreach (string item2 in Directory.EnumerateFiles(path, "*.*"))
			{
				bool flag = ((FileSystemInfo)new FileInfo(item2)).get_LastAccessTime() > DateTime.Now.AddDays(-daysOldToDelete);
				switch (Path.GetExtension(item2)!.ToUpper())
				{
				case ".STL":
				case ".AMF":
				case ".GCODE":
				case ".PNG":
				case ".TGA":
					if (referencedFilePaths.Contains(item2) || LibraryProviderSQLite.PreloadingCalibrationFiles || flag)
					{
						num++;
						break;
					}
					try
					{
						File.Delete(item2);
					}
					catch (Exception)
					{
					}
					break;
				case ".JSON":
					num++;
					break;
				default:
					num++;
					break;
				}
			}
			return num;
		}

		private static void RemoveDirectory(string directoryToRemove)
		{
			try
			{
				if (Directory.Exists(directoryToRemove))
				{
					Directory.Delete(directoryToRemove, true);
				}
			}
			catch (Exception)
			{
			}
		}

		static AboutWidget()
		{
			HashSet<string> obj = new HashSet<string>();
			obj.Add("profiles");
			folderNamesToPreserve = obj;
		}
	}
}
