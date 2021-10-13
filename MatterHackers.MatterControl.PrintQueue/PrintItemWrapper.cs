using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrintLibrary.Provider;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrintQueue
{
	public class PrintItemWrapper
	{
		public static RootedObjectEventHandler FileHasChanged = new RootedObjectEventHandler();

		private string fileNotFound = "File Not Found\n'{0}'".Localize();

		private string readyToPrint = "Ready to Print".Localize();

		private string slicingError = "Slicing Error".Localize();

		private bool doneSlicing;

		private long fileHashCode;

		private string fileType;

		private bool slicingHadError;

		private long writeTime;

		private PrintItem printItem;

		public bool CurrentlySlicing
		{
			get;
			set;
		}

		public bool DoneSlicing
		{
			get
			{
				return doneSlicing;
			}
			set
			{
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Expected O, but got Unknown
				if (value == doneSlicing)
				{
					return;
				}
				doneSlicing = value;
				if (!doneSlicing)
				{
					return;
				}
				string text = slicingError;
				slicingHadError = true;
				if (File.Exists(FileLocation))
				{
					string gCodePathAndFileName = GetGCodePathAndFileName();
					if (gCodePathAndFileName != "" && File.Exists(gCodePathAndFileName) && new FileInfo(gCodePathAndFileName).get_Length() > 10)
					{
						text = readyToPrint;
						slicingHadError = false;
					}
				}
				else
				{
					text = string.Format(fileNotFound, FileLocation);
				}
				OnSlicingOutputMessage((EventArgs)new StringEventArgs(text));
				if (this.SlicingDone != null)
				{
					this.SlicingDone(this, null);
				}
			}
		}

		public long FileHashCode
		{
			get
			{
				if (File.Exists(FileLocation))
				{
					long num = File.GetLastWriteTime(FileLocation).ToBinary();
					if (fileHashCode == 0L || writeTime != num)
					{
						writeTime = num;
						try
						{
							using FileStream fileStream = new FileStream(FileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
							long length = fileStream.Length;
							int num2 = 65536;
							byte[] array = new byte[Math.Max(64, num2 * 3)];
							fileStream.Read(array, num2, num2);
							fileStream.Seek(length / 2, SeekOrigin.Begin);
							fileStream.Read(array, num2, num2);
							fileStream.Seek(Math.Max(0L, length - num2), SeekOrigin.Begin);
							fileStream.Read(array, num2 * 2, num2);
							byte[] bytes = BitConverter.GetBytes(length);
							for (int i = 0; i < bytes.Length; i++)
							{
								array[i] = bytes[i];
							}
							byte[] bytes2 = BitConverter.GetBytes(num);
							for (int j = 0; j < bytes2.Length; j++)
							{
								array[bytes.Length + j] = bytes[j];
							}
							fileHashCode = agg_basics.ComputeHash(array);
						}
						catch (Exception)
						{
							fileHashCode = 0L;
						}
					}
				}
				else
				{
					fileHashCode = 0L;
				}
				return fileHashCode;
			}
		}

		public string FileLocation
		{
			get
			{
				return PrintItem.FileLocation;
			}
			set
			{
				PrintItem.FileLocation = value;
			}
		}

		public string Name
		{
			get
			{
				return PrintItem.Name;
			}
			set
			{
				PrintItem.Name = value;
			}
		}

		public PrintItem PrintItem
		{
			get
			{
				return printItem;
			}
			set
			{
				printItem = value;
			}
		}

		public bool SlicingHadError => slicingHadError;

		public List<ProviderLocatorNode> SourceLibraryProviderLocator
		{
			get;
			private set;
		}

		public bool UseIncrementedNameDuringTypeChange
		{
			get;
			internal set;
		}

		public event EventHandler SlicingDone;

		public event EventHandler<StringEventArgs> SlicingOutputMessage;

		public PrintItemWrapper(PrintItem printItem, List<ProviderLocatorNode> sourceLibraryProviderLocator = null)
		{
			PrintItem = printItem;
			if (FileLocation != null)
			{
				fileType = Path.GetExtension(FileLocation)!.ToUpper();
			}
			SourceLibraryProviderLocator = sourceLibraryProviderLocator;
		}

		public PrintItemWrapper(int printItemId)
		{
			_003C_003Ec__DisplayClass16_0 _003C_003Ec__DisplayClass16_ = new _003C_003Ec__DisplayClass16_0();
			_003C_003Ec__DisplayClass16_.printItemId = printItemId;
			base._002Ector();
			ITableQuery<PrintItem> tableQuery = Datastore.Instance.dbSQLite.Table<PrintItem>();
			ParameterExpression val = Expression.Parameter(typeof(PrintItem), "v");
			PrintItem = tableQuery.Where(Expression.Lambda<Func<PrintItem, bool>>((Expression)(object)Expression.Equal((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (Expression)(object)Expression.Field((Expression)(object)Expression.Constant((object)_003C_003Ec__DisplayClass16_, typeof(_003C_003Ec__DisplayClass16_0)), FieldInfo.GetFieldFromHandle((RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/))), (ParameterExpression[])(object)new ParameterExpression[1]
			{
				val
			})).Take(1).FirstOrDefault();
			try
			{
				fileType = Path.GetExtension(FileLocation)!.ToUpper();
			}
			catch (Exception)
			{
			}
		}

		public string GetFileExtension()
		{
			return Path.GetExtension(PrintItem.FileLocation);
		}

		public string GetFileNameWithoutExtension()
		{
			return Path.GetFileNameWithoutExtension(PrintItem.FileLocation);
		}

		public void ReportFileChange()
		{
			FileHasChanged.CallEvents((object)this, (EventArgs)null);
		}

		public void Delete()
		{
			PrintItem.Delete();
			PrintItem.Id = 0;
		}

		public string GetGCodePathAndFileName()
		{
			if (FileLocation.Trim() != "")
			{
				if (Path.GetExtension(FileLocation)!.ToUpper() == ".GCODE")
				{
					return FileLocation;
				}
				string text = ((int)ActiveSliceSettings.Instance.Helpers.ActiveSliceEngineType()).ToString();
				string str = FileHashCode + "_" + text + "_" + ActiveSliceSettings.Instance.GetLongHashCode();
				return Path.Combine(ApplicationDataStorage.Instance.GCodeOutputPath, str + ".gcode");
			}
			return null;
		}

		public bool IsGCodeFileComplete(string gcodePathAndFileName)
		{
			if (Path.GetExtension(FileLocation)!.ToUpper() == ".GCODE")
			{
				return true;
			}
			bool result = false;
			if (File.Exists(gcodePathAndFileName))
			{
				string text = "";
				using (FileStream stream = new FileStream(gcodePathAndFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					using StreamReader streamReader = new StreamReader(stream);
					text = streamReader.ReadToEnd();
				}
				SlicingEngineTypes slicingEngineTypes = ActiveSliceSettings.Instance.Helpers.ActiveSliceEngineType();
				if ((uint)slicingEngineTypes > 2u)
				{
					throw new NotImplementedException();
				}
				if (text.Contains("filament used ="))
				{
					result = true;
				}
			}
			return result;
		}

		public void OnSlicingOutputMessage(EventArgs e)
		{
			StringEventArgs e2 = e as StringEventArgs;
			if (this.SlicingOutputMessage != null)
			{
				this.SlicingOutputMessage(this, e2);
			}
		}
	}
}
