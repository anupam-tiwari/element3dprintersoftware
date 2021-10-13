using System;
using System.Collections.Generic;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl
{
	public class TerminalWidget : GuiWidget
	{
		private Button sendCommand;

		private CheckBox filterOutput;

		private CheckBox autoUppercase;

		private MHTextEditWidget manualCommandTextEdit;

		private TextScrollWidget textScrollWidget;

		private RGBA_Bytes backgroundColor = ActiveTheme.get_Instance().get_PrimaryBackgroundColor();

		private RGBA_Bytes textColor = ActiveTheme.get_Instance().get_PrimaryTextColor();

		private TextImageButtonFactory controlButtonFactory = new TextImageButtonFactory();

		private static readonly string TerminalFilterOutputKey = "TerminalFilterOutput";

		private static readonly string TerminalAutoUppercaseKey = "TerminalAutoUppercase";

		private string writeFaildeWaring = "WARNING: Write Failed!".Localize();

		private string cantAccessPath = "Can't access '{0}'.".Localize();

		private List<string> commandHistory = new List<string>();

		private int commandHistoryIndex;

		public TerminalWidget(bool showInWindow)
			: this()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Expected O, but got Unknown
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Expected O, but got Unknown
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Expected O, but got Unknown
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Expected O, but got Unknown
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Expected O, but got Unknown
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Expected O, but got Unknown
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Expected O, but got Unknown
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Expected O, but got Unknown
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0522: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Expected O, but got Unknown
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_Name("TerminalWidget");
			((GuiWidget)this).set_BackgroundColor(backgroundColor);
			((GuiWidget)this).set_Padding(new BorderDouble(5.0, 0.0));
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).AnchorAll();
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val2).set_VAnchor((VAnchor)(((GuiWidget)val2).get_VAnchor() | 4));
			((GuiWidget)val2).set_Padding(new BorderDouble(0.0, 0.0, 0.0, 8.0));
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_HAnchor((HAnchor)(((GuiWidget)val3).get_HAnchor() | 1));
			CheckBox val4 = new CheckBox("Filter Output".Localize());
			((GuiWidget)val4).set_Margin(new BorderDouble(5.0, 5.0, 5.0, 2.0));
			val4.set_TextColor(textColor);
			((GuiWidget)val4).set_VAnchor((VAnchor)1);
			val4.set_Checked(true);
			filterOutput = val4;
			filterOutput.add_CheckedStateChanged((EventHandler)delegate
			{
				if (filterOutput.get_Checked())
				{
					textScrollWidget.SetLineStartFilter(new string[3]
					{
						"<-wait",
						"<-ok",
						"<-T"
					});
				}
				else
				{
					textScrollWidget.SetLineStartFilter(null);
				}
				UserSettings.Instance.Fields.SetBool(TerminalFilterOutputKey, filterOutput.get_Checked());
			});
			((GuiWidget)val3).AddChild((GuiWidget)(object)filterOutput, -1);
			autoUppercase = new CheckBox("Auto Uppercase".Localize());
			((GuiWidget)autoUppercase).set_Margin(new BorderDouble(5.0, 5.0, 5.0, 2.0));
			autoUppercase.set_Checked(UserSettings.Instance.Fields.GetBool(TerminalAutoUppercaseKey, defaultValue: true));
			autoUppercase.set_TextColor(textColor);
			((GuiWidget)autoUppercase).set_VAnchor((VAnchor)1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)autoUppercase, -1);
			autoUppercase.add_CheckedStateChanged((EventHandler)delegate
			{
				UserSettings.Instance.Fields.SetBool(TerminalAutoUppercaseKey, autoUppercase.get_Checked());
			});
			((GuiWidget)val2).AddChild((GuiWidget)(object)val3, -1);
			FlowLayoutWidget val5 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val5).AnchorAll();
			textScrollWidget = new TextScrollWidget(PrinterOutputCache.Instance.PrinterLines);
			((GuiWidget)textScrollWidget).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			textScrollWidget.TextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			((GuiWidget)textScrollWidget).set_HAnchor((HAnchor)5);
			((GuiWidget)textScrollWidget).set_VAnchor((VAnchor)5);
			((GuiWidget)textScrollWidget).set_Margin(new BorderDouble(0.0, 5.0));
			((GuiWidget)textScrollWidget).set_Padding(new BorderDouble(3.0, 0.0));
			textScrollWidget.SetLineStartFilter(new string[3]
			{
				"<-wait",
				"<-ok",
				"<-T"
			});
			((GuiWidget)val5).AddChild((GuiWidget)(object)textScrollWidget, -1);
			TextScrollBar textScrollBar = new TextScrollBar(textScrollWidget, 15);
			((GuiWidget)val5).AddChild((GuiWidget)(object)textScrollBar, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val5, -1);
			FlowLayoutWidget val6 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val6).set_BackgroundColor(backgroundColor);
			((GuiWidget)val6).set_HAnchor((HAnchor)5);
			manualCommandTextEdit = new MHTextEditWidget("", 0.0, 0.0, 12.0, 0.0, 0.0, multiLine: false, 0, "", ApplicationController.MonoSpacedTypeFace);
			((GuiWidget)manualCommandTextEdit).set_Margin(new BorderDouble(0.0, 0.0, 3.0, 0.0));
			((GuiWidget)manualCommandTextEdit).set_HAnchor((HAnchor)5);
			((GuiWidget)manualCommandTextEdit).set_VAnchor((VAnchor)1);
			manualCommandTextEdit.ActualTextEditWidget.add_EnterPressed(new KeyEventHandler(manualCommandTextEdit_EnterPressed));
			((GuiWidget)manualCommandTextEdit.ActualTextEditWidget).add_KeyDown((EventHandler<KeyEventArgs>)manualCommandTextEdit_KeyDown);
			((GuiWidget)val6).AddChild((GuiWidget)(object)manualCommandTextEdit, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val6, -1);
			Button val7 = controlButtonFactory.Generate("Clear".Localize());
			((GuiWidget)val7).set_Margin(new BorderDouble(0.0));
			((GuiWidget)val7).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				PrinterOutputCache.Instance.Clear();
			});
			Button val8 = controlButtonFactory.Generate("Export".Localize() + "...");
			((GuiWidget)val8).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)DoExportExportLog_Click);
			});
			Button val9 = controlButtonFactory.Generate("Close".Localize());
			((GuiWidget)val9).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)CloseWindow);
			});
			sendCommand = controlButtonFactory.Generate("Send".Localize());
			((GuiWidget)sendCommand).add_Click((EventHandler<MouseEventArgs>)sendManualCommandToPrinter_Click);
			FlowLayoutWidget val10 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val10).set_HAnchor((HAnchor)5);
			((GuiWidget)val10).set_Margin(new BorderDouble(0.0, 3.0));
			((GuiWidget)val10).AddChild((GuiWidget)(object)sendCommand, -1);
			((GuiWidget)val10).AddChild((GuiWidget)(object)val7, -1);
			((GuiWidget)val10).AddChild((GuiWidget)(object)val8, -1);
			((GuiWidget)val10).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			if (showInWindow)
			{
				((GuiWidget)val10).AddChild((GuiWidget)(object)val9, -1);
			}
			((GuiWidget)val2).AddChild((GuiWidget)(object)val10, -1);
			((GuiWidget)val2).AnchorAll();
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)this).AnchorAll();
		}

		private void DoExportExportLog_Click()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Expected O, but got Unknown
			SaveFileDialogParams val = new SaveFileDialogParams("Save as Text|*.txt", "", "", "");
			((FileDialogParams)val).set_Title("Element: Terminal Log");
			((FileDialogParams)val).set_ActionButtonLabel("Export");
			((FileDialogParams)val).set_FileName("print_log.txt");
			FileDialog.SaveFileDialog(val, (Action<SaveFileDialogParams>)onExportLogFileSelected);
		}

		public override void OnLoad(EventArgs args)
		{
			filterOutput.set_Checked(UserSettings.Instance.Fields.GetBool(TerminalFilterOutputKey, defaultValue: false));
			UiThread.RunOnIdle((Action)((GuiWidget)manualCommandTextEdit).Focus);
			((GuiWidget)this).OnLoad(args);
		}

		private void onExportLogFileSelected(SaveFileDialogParams saveParams)
		{
			if (string.IsNullOrEmpty(((FileDialogParams)saveParams).get_FileName()))
			{
				return;
			}
			string fileName = ((FileDialogParams)saveParams).get_FileName();
			if (fileName == null || !(fileName != ""))
			{
				return;
			}
			try
			{
				textScrollWidget.WriteToFile(fileName);
			}
			catch (UnauthorizedAccessException ex)
			{
				UnauthorizedAccessException e = ex;
				PrinterOutputCache.Instance.PrinterLines.Add("");
				PrinterOutputCache.Instance.PrinterLines.Add(writeFaildeWaring);
				PrinterOutputCache.Instance.PrinterLines.Add(StringHelper.FormatWith(cantAccessPath, new object[1]
				{
					fileName
				}));
				PrinterOutputCache.Instance.PrinterLines.Add("");
				UiThread.RunOnIdle((Action)delegate
				{
					StyledMessageBox.ShowMessageBox(null, e.Message, "Couldn't save file".Localize());
				});
			}
		}

		private void CloseWindow()
		{
			SystemWindow obj = Enumerable.FirstOrDefault<SystemWindow>(ExtensionMethods.Parents<SystemWindow>((GuiWidget)(object)this));
			if (obj != null)
			{
				((GuiWidget)obj).Close();
			}
		}

		private void manualCommandTextEdit_KeyDown(object sender, KeyEventArgs keyEvent)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Invalid comparison between Unknown and I4
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Invalid comparison between Unknown and I4
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Invalid comparison between Unknown and I4
			bool flag = false;
			if ((int)keyEvent.get_KeyCode() == 38)
			{
				commandHistoryIndex--;
				if (commandHistoryIndex < 0)
				{
					commandHistoryIndex = 0;
				}
				flag = true;
			}
			else if ((int)keyEvent.get_KeyCode() == 40)
			{
				commandHistoryIndex++;
				if (commandHistoryIndex > commandHistory.Count - 1)
				{
					commandHistoryIndex = commandHistory.Count - 1;
				}
				else
				{
					flag = true;
				}
			}
			else if ((int)keyEvent.get_KeyCode() == 27)
			{
				((GuiWidget)manualCommandTextEdit).set_Text("");
			}
			if (flag && commandHistory.Count > 0)
			{
				((GuiWidget)manualCommandTextEdit).set_Text(commandHistory[commandHistoryIndex]);
			}
		}

		private void manualCommandTextEdit_EnterPressed(object sender, KeyEventArgs keyEvent)
		{
			sendManualCommandToPrinter_Click(null, null);
		}

		private void sendManualCommandToPrinter_Click(object sender, EventArgs mouseEvent)
		{
			string text = ((GuiWidget)manualCommandTextEdit).get_Text().Trim();
			if (autoUppercase.get_Checked())
			{
				text = text.ToUpper();
			}
			if (text == "" && commandHistory.Count != 0)
			{
				text = Enumerable.Last<string>((IEnumerable<string>)commandHistory);
			}
			else
			{
				commandHistory.Add(text);
				commandHistoryIndex = commandHistory.Count;
			}
			PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow(text);
			((GuiWidget)manualCommandTextEdit).set_Text("");
		}
	}
}
