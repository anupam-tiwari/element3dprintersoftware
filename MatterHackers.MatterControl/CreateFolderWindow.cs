using System;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.DataStorage;

namespace MatterHackers.MatterControl
{
	public class CreateFolderWindow : SystemWindow
	{
		public class CreateFolderReturnInfo
		{
			public string newName;

			public CreateFolderReturnInfo(string newName)
			{
				this.newName = newName;
			}
		}

		private Action<CreateFolderReturnInfo> functionToCallToCreateNamedFolder;

		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private MHTextEditWidget folderNameWidget;

		public CreateFolderWindow(Action<CreateFolderReturnInfo> functionToCallToCreateNamedFolder)
			: this(480.0, 180.0)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Expected O, but got Unknown
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Expected O, but got Unknown
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Expected O, but got Unknown
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Expected O, but got Unknown
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Expected O, but got Unknown
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Expected O, but got Unknown
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Expected O, but got Unknown
			((SystemWindow)this).set_Title("Element - Create Folder");
			((SystemWindow)this).set_AlwaysOnTopOfMain(true);
			this.functionToCallToCreateNamedFolder = functionToCallToCreateNamedFolder;
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).AnchorAll();
			((GuiWidget)val).set_Padding(new BorderDouble(3.0, 0.0, 3.0, 5.0));
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 3.0, 0.0, 0.0));
			((GuiWidget)val2).set_Padding(new BorderDouble(0.0, 3.0, 0.0, 3.0));
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			TextWidget val3 = new TextWidget("Create New Folder:".Localize(), 0.0, 0.0, 14.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val3.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_VAnchor((VAnchor)1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val4).set_HAnchor((HAnchor)5);
			((GuiWidget)val4).set_VAnchor((VAnchor)5);
			((GuiWidget)val4).set_Padding(new BorderDouble(5.0));
			((GuiWidget)val4).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			TextWidget val5 = new TextWidget("Folder Name".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val5.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val5).set_Margin(new BorderDouble(5.0));
			((GuiWidget)val5).set_HAnchor((HAnchor)1);
			folderNameWidget = new MHTextEditWidget("", 0.0, 0.0, 12.0, 300.0, 0.0, multiLine: false, 0, "Enter a Folder Name Here".Localize());
			((GuiWidget)folderNameWidget).set_Name("Create Folder - Text Input");
			((GuiWidget)folderNameWidget).set_HAnchor((HAnchor)5);
			((GuiWidget)folderNameWidget).set_Margin(new BorderDouble(5.0));
			((GuiWidget)val4).AddChild((GuiWidget)(object)val5, -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)folderNameWidget, -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val4, -1);
			FlowLayoutWidget val6 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)val6).set_HAnchor((HAnchor)5);
			((GuiWidget)val6).set_Padding(new BorderDouble(0.0, 3.0));
			((GuiWidget)val6).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			Button val7 = textImageButtonFactory.Generate("Cancel".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)val7).set_Visible(true);
			((GuiWidget)val7).set_Cursor((Cursors)3);
			((GuiWidget)val6).AddChild((GuiWidget)(object)val7, -1);
			((GuiWidget)val7).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				((GuiWidget)this).CloseOnIdle();
			});
			Button val8 = textImageButtonFactory.Generate("Create".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)val8).set_Name("Create Folder Button");
			((GuiWidget)val8).set_Visible(true);
			((GuiWidget)val8).set_Cursor((Cursors)3);
			((GuiWidget)val6).AddChild((GuiWidget)(object)val8, -1);
			((GuiWidget)val8).add_Click((EventHandler<MouseEventArgs>)createFolderButton_Click);
			folderNameWidget.ActualTextEditWidget.add_EnterPressed(new KeyEventHandler(ActualTextEditWidget_EnterPressed));
			((GuiWidget)val).AddChild((GuiWidget)(object)val6, -1);
			((SystemWindow)this).ShowAsSystemWindow();
		}

		public override void OnLoad(EventArgs args)
		{
			UiThread.RunOnIdle((Action)((GuiWidget)folderNameWidget).Focus);
			((GuiWidget)this).OnLoad(args);
		}

		private void ActualTextEditWidget_EnterPressed(object sender, KeyEventArgs keyEvent)
		{
			SubmitForm();
		}

		private void createFolderButton_Click(object sender, EventArgs mouseEvent)
		{
			SubmitForm();
		}

		private void SubmitForm()
		{
			string text = ((GuiWidget)folderNameWidget.ActualTextEditWidget).get_Text();
			if (text != "")
			{
				string path = Path.ChangeExtension(Path.GetRandomFileName(), ".amf");
				Path.Combine(ApplicationDataStorage.Instance.ApplicationLibraryDataPath, path);
				CreateFolderReturnInfo obj = new CreateFolderReturnInfo(text);
				functionToCallToCreateNamedFolder(obj);
				((GuiWidget)this).CloseOnIdle();
			}
		}
	}
}
