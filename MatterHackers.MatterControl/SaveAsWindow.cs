using System;
using System.Collections.Generic;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.CustomWidgets.LibrarySelector;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrintLibrary.Provider;

namespace MatterHackers.MatterControl
{
	public class SaveAsWindow : SystemWindow
	{
		public class SaveAsReturnInfo
		{
			public string fileNameAndPath;

			public string newName;

			public LibraryProvider destinationLibraryProvider;

			public SaveAsReturnInfo(string newName, string fileNameAndPath, LibraryProvider destinationLibraryProvider)
			{
				this.destinationLibraryProvider = destinationLibraryProvider;
				this.newName = newName;
				this.fileNameAndPath = fileNameAndPath;
			}
		}

		private Action<SaveAsReturnInfo> functionToCallOnSaveAs;

		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private MHTextEditWidget textToAddWidget;

		private LibrarySelectorWidget librarySelectorWidget;

		private Button saveAsButton;

		public SaveAsWindow(Action<SaveAsReturnInfo> functionToCallOnSaveAs, List<ProviderLocatorNode> providerLocator, bool showQueue, bool getNewName)
			: this(480.0, 500.0)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Expected O, but got Unknown
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Expected O, but got Unknown
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Expected O, but got Unknown
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Expected O, but got Unknown
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Expected O, but got Unknown
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Expected O, but got Unknown
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Expected O, but got Unknown
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Expected O, but got Unknown
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			textImageButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.disabledTextColor = ActiveTheme.get_Instance().get_TabLabelUnselected();
			textImageButtonFactory.disabledFillColor = default(RGBA_Bytes);
			((SystemWindow)this).set_Title("Element - " + "Save As".Localize());
			((SystemWindow)this).set_AlwaysOnTopOfMain(true);
			((GuiWidget)this).set_Name("Save As Window");
			this.functionToCallOnSaveAs = functionToCallOnSaveAs;
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).AnchorAll();
			((GuiWidget)val).set_Padding(new BorderDouble(3.0, 0.0, 3.0, 5.0));
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 3.0, 0.0, 0.0));
			((GuiWidget)val2).set_Padding(new BorderDouble(0.0, 3.0, 0.0, 3.0));
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			TextWidget val3 = new TextWidget("Save New Design".Localize() + ":", 0.0, 0.0, 14.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
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
			librarySelectorWidget = new LibrarySelectorWidget(showQueue);
			FolderBreadCrumbWidget folderBreadCrumbWidget = new FolderBreadCrumbWidget(librarySelectorWidget.SetCurrentLibraryProvider, librarySelectorWidget.CurrentLibraryProvider);
			((GuiWidget)val4).AddChild((GuiWidget)(object)folderBreadCrumbWidget, -1);
			librarySelectorWidget.ChangedCurrentLibraryProvider += folderBreadCrumbWidget.SetBreadCrumbs;
			GuiWidget val5 = new GuiWidget(10.0, 30.0, (SizeLimitsToSet)1);
			val5.set_HAnchor((HAnchor)5);
			val5.set_VAnchor((VAnchor)5);
			val5.set_Margin(new BorderDouble(5.0));
			val5.set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			val5.set_Padding(new BorderDouble(3.0));
			val5.AddChild((GuiWidget)(object)librarySelectorWidget, -1);
			((GuiWidget)val4).AddChild(val5, -1);
			if (getNewName)
			{
				TextWidget val6 = new TextWidget("Design Name".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				val6.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
				((GuiWidget)val6).set_Margin(new BorderDouble(5.0));
				((GuiWidget)val6).set_HAnchor((HAnchor)1);
				textToAddWidget = new MHTextEditWidget("", 0.0, 0.0, 12.0, 300.0, 0.0, multiLine: false, 0, "Enter a Design Name Here".Localize());
				((GuiWidget)textToAddWidget).set_HAnchor((HAnchor)5);
				((GuiWidget)textToAddWidget).set_Margin(new BorderDouble(5.0));
				textToAddWidget.ActualTextEditWidget.add_EnterPressed(new KeyEventHandler(ActualTextEditWidget_EnterPressed));
				((GuiWidget)val4).AddChild((GuiWidget)(object)val6, -1);
				((GuiWidget)val4).AddChild((GuiWidget)(object)textToAddWidget, -1);
			}
			((GuiWidget)val4).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val4, -1);
			FlowLayoutWidget val7 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)val7).set_HAnchor((HAnchor)5);
			((GuiWidget)val7).set_Padding(new BorderDouble(0.0, 3.0));
			((GuiWidget)val7).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			Button val8 = textImageButtonFactory.Generate("Cancel".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)val8).set_Visible(true);
			((GuiWidget)val8).set_Cursor((Cursors)3);
			((GuiWidget)val7).AddChild((GuiWidget)(object)val8, -1);
			((GuiWidget)val8).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				((GuiWidget)this).CloseOnIdle();
			});
			saveAsButton = textImageButtonFactory.Generate("Save".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)saveAsButton).set_Name("Save As Save Button");
			((GuiWidget)saveAsButton).set_Enabled(false);
			((GuiWidget)saveAsButton).set_Cursor((Cursors)3);
			((GuiWidget)val7).AddChild((GuiWidget)(object)saveAsButton, -1);
			librarySelectorWidget.ChangedCurrentLibraryProvider += EnableSaveAsButtonOnChangedLibraryProvider;
			((GuiWidget)saveAsButton).add_Click((EventHandler<MouseEventArgs>)saveAsButton_Click);
			((GuiWidget)val).AddChild((GuiWidget)(object)val7, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			((SystemWindow)this).ShowAsSystemWindow();
		}

		private void EnableSaveAsButtonOnChangedLibraryProvider(LibraryProvider arg1, LibraryProvider arg2)
		{
			((GuiWidget)saveAsButton).set_Enabled(true);
		}

		public override void OnLoad(EventArgs args)
		{
			if (textToAddWidget != null && !UserSettings.Instance.IsTouchScreen)
			{
				UiThread.RunOnIdle((Action)((GuiWidget)textToAddWidget).Focus);
			}
			((GuiWidget)this).OnLoad(args);
		}

		private void ActualTextEditWidget_EnterPressed(object sender, KeyEventArgs keyEvent)
		{
			SubmitForm();
		}

		private void saveAsButton_Click(object sender, EventArgs mouseEvent)
		{
			SubmitForm();
		}

		private void SubmitForm()
		{
			string text = "none";
			if (textToAddWidget != null)
			{
				text = ((GuiWidget)textToAddWidget.ActualTextEditWidget).get_Text();
			}
			if (text != "")
			{
				string path = Path.ChangeExtension(Path.GetRandomFileName(), ".amf");
				string fileNameAndPath = Path.Combine(ApplicationDataStorage.Instance.ApplicationLibraryDataPath, path);
				SaveAsReturnInfo obj = new SaveAsReturnInfo(text, fileNameAndPath, librarySelectorWidget.CurrentLibraryProvider);
				functionToCallOnSaveAs(obj);
				((GuiWidget)this).CloseOnIdle();
			}
		}
	}
}
