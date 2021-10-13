using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;

namespace MatterHackers.MatterControl
{
	public class ExportToFolderFeedbackWindow : SystemWindow
	{
		private int totalParts;

		private int count;

		private FlowLayoutWidget feedback = new FlowLayoutWidget((FlowDirection)3);

		private TextWidget nextLine;

		public ExportToFolderFeedbackWindow(int totalParts, string firstPartName, RGBA_Bytes backgroundColor)
			: this(300.0, 500.0)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_BackgroundColor(backgroundColor);
			string arg = "Element".Localize();
			string arg2 = "Exporting to Folder or SD Card".Localize();
			((SystemWindow)this).set_Title($"{arg} - {arg2}");
			this.totalParts = totalParts;
			((GuiWidget)feedback).set_Padding(new BorderDouble(5.0, 5.0));
			((GuiWidget)feedback).AnchorAll();
			((GuiWidget)this).AddChild((GuiWidget)(object)feedback, -1);
			nextLine = CreateNextLine("");
			((GuiWidget)feedback).AddChild((GuiWidget)(object)nextLine, -1);
		}

		private TextWidget CreateNextLine(string startText)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Expected O, but got Unknown
			TextWidget val = new TextWidget(startText, 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 2.0));
			((GuiWidget)val).set_HAnchor((HAnchor)1);
			val.set_AutoExpandBoundsToText(true);
			return val;
		}

		public void StartingNextPart(object sender, EventArgs e)
		{
			count++;
			StringEventArgs val = e as StringEventArgs;
			if (val != null)
			{
				string text = $"{count}/{totalParts} '{val.get_Data()}'";
				((GuiWidget)nextLine).set_Text(text);
				nextLine = CreateNextLine("");
				((GuiWidget)feedback).AddChild((GuiWidget)(object)nextLine, -1);
			}
		}

		public void DoneSaving(object sender, EventArgs e)
		{
			StringEventArgs val = e as StringEventArgs;
			if (val != null)
			{
				((GuiWidget)nextLine).set_Text("");
				((GuiWidget)feedback).AddChild((GuiWidget)(object)CreateNextLine($"Filament length = {val.get_Data()} mm"), -1);
			}
		}

		public void UpdatePartStatus(object sender, EventArgs e)
		{
			StringEventArgs val = e as StringEventArgs;
			if (val != null)
			{
				((GuiWidget)nextLine).set_Text("   " + val.get_Data());
			}
		}
	}
}
