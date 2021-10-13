using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.PolygonMesh;
using MatterHackers.PolygonMesh.Processors;

namespace MatterHackers.MatterControl.Queue.OptionsMenu
{
	public class MyMenuExtension : PrintItemMenuExtension
	{
		public override IEnumerable<PrintItemAction> GetMenuItems()
		{
			return new List<PrintItemAction>
			{
				new PrintItemAction
				{
					SingleItemOnly = false,
					Title = "Merge".Localize() + "...",
					Action = delegate(IEnumerable<QueueRowItem> items, QueueDataWidget queueDataWidget)
					{
						List<QueueRowItem> allRowItems = new List<QueueRowItem>(items);
						if (allRowItems.Count > 1)
						{
							RenameItemWindow renameItemWindow = new RenameItemWindow(allRowItems[0].PrintItemWrapper.Name, delegate(RenameItemWindow.RenameItemReturnInfo returnInfo)
							{
								Task.Run(delegate
								{
									//IL_007e: Unknown result type (might be due to invalid IL or missing references)
									//IL_0084: Expected O, but got Unknown
									List<MeshGroup> list = new List<MeshGroup>();
									foreach (QueueRowItem item2 in allRowItems)
									{
										List<MeshGroup> collection = MeshFileIo.Load(item2.PrintItemWrapper.FileLocation, (ReportProgressRatio)null);
										list.AddRange(collection);
									}
									string[] array = new string[4]
									{
										"Created By",
										"Element",
										"BedPosition",
										"Absolute"
									};
									MeshOutputSettings val = new MeshOutputSettings((OutputType)1, array, (ReportProgressRatio)null);
									string applicationLibraryDataPath = ApplicationDataStorage.Instance.ApplicationLibraryDataPath;
									if (!Directory.Exists(applicationLibraryDataPath))
									{
										Directory.CreateDirectory(applicationLibraryDataPath);
									}
									string text = Path.Combine(applicationLibraryDataPath, Path.ChangeExtension(Path.GetRandomFileName(), "amf"));
									if (MeshFileIo.Save(list, text, val, (ReportProgressRatio)null) && File.Exists(text))
									{
										PrintItemWrapper item = new PrintItemWrapper(new PrintItem(returnInfo.newName, text));
										QueueData.Instance.AddItem(item, 0);
										QueueData.Instance.SelectedIndex = 0;
										queueDataWidget.LeaveEditMode();
									}
								});
							}, "Set Name".Localize());
							((SystemWindow)renameItemWindow).set_Title("Element - Set Name".Localize());
							renameItemWindow.ElementHeader = "Set Name".Localize();
						}
					}
				}
			};
		}
	}
}
