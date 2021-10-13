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
	public class RenameItemWindow : SystemWindow
	{
		public class RenameItemReturnInfo
		{
			public string newName;

			public RenameItemReturnInfo(string newName)
			{
				this.newName = newName;
			}
		}

		public class CreateFolderReturnInfo
		{
			public string newName;

			public CreateFolderReturnInfo(string newName)
			{
				this.newName = newName;
			}
		}

		private Action<RenameItemReturnInfo> functionToCallToCreateNamedFolder;

		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private MHTextEditWidget saveAsNameWidget;

		private TextWidget elementHeader;

		private Button renameItemButton;

		public string ElementHeader
		{
			get
			{
				return ((GuiWidget)elementHeader).get_Text();
			}
			set
			{
				((GuiWidget)elementHeader).set_Text(value);
			}
		}

		public RenameItemWindow(string currentItemName, Action<RenameItemReturnInfo> functionToCallToRenameItem, string renameButtonString = null)
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
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Expected O, but got Unknown
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Expected O, but got Unknown
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Expected O, but got Unknown
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Expected O, but got Unknown
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Expected O, but got Unknown
			((SystemWindow)this).set_Title("Element - Rename Item");
			((SystemWindow)this).set_AlwaysOnTopOfMain(true);
			functionToCallToCreateNamedFolder = functionToCallToRenameItem;
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).AnchorAll();
			((GuiWidget)val).set_Padding(new BorderDouble(3.0, 0.0, 3.0, 5.0));
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 3.0, 0.0, 0.0));
			((GuiWidget)val2).set_Padding(new BorderDouble(0.0, 3.0, 0.0, 3.0));
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			string text = "Rename Item:".Localize();
			elementHeader = new TextWidget(text, 0.0, 0.0, 14.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			elementHeader.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)elementHeader).set_HAnchor((HAnchor)5);
			((GuiWidget)elementHeader).set_VAnchor((VAnchor)1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)elementHeader, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_VAnchor((VAnchor)5);
			((GuiWidget)val3).set_Padding(new BorderDouble(5.0));
			((GuiWidget)val3).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			TextWidget val4 = new TextWidget("New Name".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val4.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val4).set_Margin(new BorderDouble(5.0));
			((GuiWidget)val4).set_HAnchor((HAnchor)1);
			saveAsNameWidget = new MHTextEditWidget(currentItemName, 0.0, 0.0, 12.0, 300.0, 0.0, multiLine: false, 0, "Enter New Name Here".Localize());
			((GuiWidget)saveAsNameWidget).set_HAnchor((HAnchor)5);
			((GuiWidget)saveAsNameWidget).set_Margin(new BorderDouble(5.0));
			((GuiWidget)val3).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)saveAsNameWidget, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			FlowLayoutWidget val5 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)val5).set_HAnchor((HAnchor)5);
			((GuiWidget)val5).set_Padding(new BorderDouble(0.0, 3.0));
			((GuiWidget)val5).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			Button val6 = textImageButtonFactory.Generate("Cancel".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)val6).set_Visible(true);
			((GuiWidget)val6).set_Cursor((Cursors)3);
			((GuiWidget)val5).AddChild((GuiWidget)(object)val6, -1);
			((GuiWidget)val6).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				((GuiWidget)this).CloseOnIdle();
			});
			if (renameButtonString == null)
			{
				renameButtonString = "Rename".Localize();
			}
			renameItemButton = textImageButtonFactory.Generate(renameButtonString, (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)renameItemButton).set_Name("Rename Button");
			((GuiWidget)renameItemButton).set_Visible(true);
			((GuiWidget)renameItemButton).set_Cursor((Cursors)3);
			((GuiWidget)val5).AddChild((GuiWidget)(object)renameItemButton, -1);
			((GuiWidget)renameItemButton).add_Click((EventHandler<MouseEventArgs>)renameItemButton_Click);
			saveAsNameWidget.ActualTextEditWidget.add_EnterPressed(new KeyEventHandler(ActualTextEditWidget_EnterPressed));
			((GuiWidget)val).AddChild((GuiWidget)(object)val5, -1);
			((SystemWindow)this).ShowAsSystemWindow();
		}

		public override void OnLoad(EventArgs args)
		{
			UiThread.RunOnIdle((Action)delegate
			{
				((GuiWidget)saveAsNameWidget).Focus();
				saveAsNameWidget.ActualTextEditWidget.get_InternalTextEditWidget().SelectAll();
			});
			((GuiWidget)this).OnLoad(args);
		}

		private void ActualTextEditWidget_EnterPressed(object sender, KeyEventArgs keyEvent)
		{
			SubmitForm();
		}

		private void renameItemButton_Click(object sender, EventArgs mouseEvent)
		{
			SubmitForm();
		}

		private void SubmitForm()
		{
			string text = ((GuiWidget)saveAsNameWidget.ActualTextEditWidget).get_Text();
			if (text != "")
			{
				string path = Path.ChangeExtension(Path.GetRandomFileName(), ".amf");
				Path.Combine(ApplicationDataStorage.Instance.ApplicationLibraryDataPath, path);
				RenameItemReturnInfo obj = new RenameItemReturnInfo(text);
				functionToCallToCreateNamedFolder(obj);
				((GuiWidget)this).CloseOnIdle();
			}
		}
	}
}
