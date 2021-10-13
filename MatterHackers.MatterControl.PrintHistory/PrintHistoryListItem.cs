using System;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PartPreviewWindow;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.PrintQueue;

namespace MatterHackers.MatterControl.PrintHistory
{
	public class PrintHistoryListItem : GuiWidget
	{
		public PrintTask printTask;

		public RGBA_Bytes WidgetTextColor;

		public RGBA_Bytes WidgetBackgroundColor;

		public bool isActivePrint;

		public bool isSelectedItem;

		public bool isHoverItem;

		private bool showTimestamp;

		private TextWidget partLabel;

		public CheckBox selectionCheckBox;

		private float pointSizeFactor = 1f;

		private static int rightOverlayWidth = 304;

		private int actionButtonSize = rightOverlayWidth / 3;

		private SlideWidget rightButtonOverlay;

		private LinkButtonFactory linkButtonFactory = new LinkButtonFactory();

		private EventHandler unregisterEvents;

		private PrintItemWrapper itemToRemove;

		private PartPreviewMainWindow partPreviewWindow;

		public PrintHistoryListItem(PrintTask printTask, bool showTimestamp)
			: this()
		{
			this.printTask = printTask;
			this.showTimestamp = showTimestamp;
			SetDisplayAttributes();
			AddChildElements();
			AddHandlers();
		}

		private void AddChildElements()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Expected O, but got Unknown
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Expected O, but got Unknown
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Expected O, but got Unknown
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Expected O, but got Unknown
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Expected O, but got Unknown
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Expected O, but got Unknown
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Expected O, but got Unknown
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Expected O, but got Unknown
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Expected O, but got Unknown
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04df: Expected O, but got Unknown
			//IL_054c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0553: Expected O, but got Unknown
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_0589: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0595: Unknown result type (might be due to invalid IL or missing references)
			//IL_0598: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bb: Expected O, but got Unknown
			//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0616: Unknown result type (might be due to invalid IL or missing references)
			//IL_061b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0621: Unknown result type (might be due to invalid IL or missing references)
			//IL_062b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0638: Expected O, but got Unknown
			//IL_0660: Unknown result type (might be due to invalid IL or missing references)
			//IL_0666: Unknown result type (might be due to invalid IL or missing references)
			//IL_066c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0672: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_067a: Unknown result type (might be due to invalid IL or missing references)
			//IL_067b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0685: Unknown result type (might be due to invalid IL or missing references)
			//IL_068c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0698: Expected O, but got Unknown
			//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0716: Unknown result type (might be due to invalid IL or missing references)
			//IL_071b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0721: Unknown result type (might be due to invalid IL or missing references)
			//IL_072b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0738: Expected O, but got Unknown
			//IL_0760: Unknown result type (might be due to invalid IL or missing references)
			//IL_0766: Unknown result type (might be due to invalid IL or missing references)
			//IL_076c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0772: Unknown result type (might be due to invalid IL or missing references)
			//IL_0775: Unknown result type (might be due to invalid IL or missing references)
			//IL_077a: Unknown result type (might be due to invalid IL or missing references)
			//IL_077b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0785: Unknown result type (might be due to invalid IL or missing references)
			//IL_078c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0798: Expected O, but got Unknown
			//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ff: Expected O, but got Unknown
			//IL_0809: Unknown result type (might be due to invalid IL or missing references)
			//IL_0827: Unknown result type (might be due to invalid IL or missing references)
			//IL_0832: Unknown result type (might be due to invalid IL or missing references)
			//IL_0839: Expected O, but got Unknown
			//IL_0855: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08af: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bf: Expected O, but got Unknown
			//IL_08c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0925: Unknown result type (might be due to invalid IL or missing references)
			//IL_092b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0931: Unknown result type (might be due to invalid IL or missing references)
			//IL_0937: Unknown result type (might be due to invalid IL or missing references)
			//IL_093a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0941: Expected O, but got Unknown
			//IL_0943: Unknown result type (might be due to invalid IL or missing references)
			//IL_096b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0972: Expected O, but got Unknown
			//IL_098e: Unknown result type (might be due to invalid IL or missing references)
			//IL_09dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f8: Expected O, but got Unknown
			//IL_09fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a87: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a93: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa3: Expected O, but got Unknown
			//IL_0aa5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2b: Unknown result type (might be due to invalid IL or missing references)
			GuiWidget val = new GuiWidget();
			val.set_HAnchor((HAnchor)5);
			val.set_VAnchor((VAnchor)5);
			TextInfo textInfo = new CultureInfo("en-US", useUserOverride: false).TextInfo;
			GuiWidget val2 = new GuiWidget();
			val2.set_VAnchor((VAnchor)5);
			val2.set_Width(15.0);
			if (printTask.PrintComplete)
			{
				val2.set_BackgroundColor(new RGBA_Bytes(38, 147, 51, 180));
			}
			else
			{
				val2.set_BackgroundColor(new RGBA_Bytes(252, 209, 22, 180));
			}
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_Padding(new BorderDouble(6.0, 3.0));
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val4).set_HAnchor((HAnchor)5);
			string text = textInfo.ToTitleCase(printTask.PrintName);
			text = text.Replace('_', ' ');
			partLabel = new TextWidget(text, 0.0, 0.0, (double)(15f * pointSizeFactor), (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			partLabel.set_TextColor(WidgetTextColor);
			((GuiWidget)val4).AddChild((GuiWidget)(object)partLabel, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val4, -1);
			RGBA_Bytes textColor = default(RGBA_Bytes);
			((RGBA_Bytes)(ref textColor))._002Ector(34, 34, 34);
			FlowLayoutWidget val5 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val5).set_Margin(new BorderDouble(0.0));
			((GuiWidget)val5).set_HAnchor((HAnchor)5);
			((GuiWidget)new TextWidget("Status: Completed".Localize(), 0.0, 0.0, (double)(8f * pointSizeFactor), (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null)).set_Margin(new BorderDouble(0.0, 0.0, 3.0, 0.0));
			string arg = "Time".Localize().ToUpper();
			TextWidget val6 = new TextWidget($"{arg}: ", 0.0, 0.0, (double)(8f * pointSizeFactor), (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val6.set_TextColor(textColor);
			int printTimeMinutes = printTask.PrintTimeMinutes;
			TextWidget val7 = ((printTimeMinutes < 0) ? new TextWidget("Unknown".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null) : ((printTimeMinutes <= 60) ? new TextWidget($"{printTask.PrintTimeMinutes}min", 0.0, 0.0, (double)(12f * pointSizeFactor), (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null) : new TextWidget(StringHelper.FormatWith("{0}hrs {1}min", new object[2]
			{
				printTask.PrintTimeMinutes / 60,
				printTask.PrintTimeMinutes % 60
			}), 0.0, 0.0, (double)(12f * pointSizeFactor), (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null)));
			if (printTask.PercentDone > 0.0)
			{
				val7.set_AutoExpandBoundsToText(true);
				((GuiWidget)val7).set_Text(((GuiWidget)val7).get_Text() + $" ({printTask.PercentDone:0.0}%)");
				if (printTask.RecoveryCount > 0.0)
				{
					if (printTask.RecoveryCount == 1.0)
					{
						((GuiWidget)val7).set_Text(((GuiWidget)val7).get_Text() + " - " + "recovered once".Localize());
					}
					else
					{
						((GuiWidget)val7).set_Text(((GuiWidget)val7).get_Text() + " - " + StringHelper.FormatWith("recovered {0} times", new object[1]
						{
							printTask.RecoveryCount
						}));
					}
				}
			}
			((GuiWidget)val7).set_Margin(new BorderDouble(0.0, 0.0, 6.0, 0.0));
			val7.set_TextColor(textColor);
			((GuiWidget)val5).AddChild((GuiWidget)(object)val6, -1);
			((GuiWidget)val5).AddChild((GuiWidget)(object)val7, -1);
			((GuiWidget)val5).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val5, -1);
			GuiWidget val8 = new GuiWidget();
			val8.set_HAnchor((HAnchor)5);
			val8.set_VAnchor((VAnchor)5);
			FlowLayoutWidget val9 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val9).set_HAnchor((HAnchor)5);
			((GuiWidget)val9).set_VAnchor((VAnchor)5);
			((GuiWidget)val9).AddChild(val2, -1);
			((GuiWidget)val9).AddChild((GuiWidget)(object)val3, -1);
			val8.AddChild((GuiWidget)(object)val9, -1);
			rightButtonOverlay = new SlideWidget();
			((GuiWidget)rightButtonOverlay).set_VAnchor((VAnchor)5);
			((GuiWidget)rightButtonOverlay).set_HAnchor((HAnchor)4);
			((GuiWidget)rightButtonOverlay).set_Width((double)rightOverlayWidth);
			((GuiWidget)rightButtonOverlay).set_Visible(false);
			FlowLayoutWidget val10 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val10).set_VAnchor((VAnchor)5);
			TextWidget val11 = new TextWidget("View".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val11.set_TextColor(RGBA_Bytes.White);
			((GuiWidget)val11).set_VAnchor((VAnchor)2);
			((GuiWidget)val11).set_HAnchor((HAnchor)2);
			FatFlatClickWidget fatFlatClickWidget = new FatFlatClickWidget(val11);
			((GuiWidget)fatFlatClickWidget).set_VAnchor((VAnchor)5);
			((GuiWidget)fatFlatClickWidget).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryAccentColor());
			((GuiWidget)fatFlatClickWidget).set_Width((double)actionButtonSize);
			fatFlatClickWidget.Click += ViewButton_Click;
			((GuiWidget)val10).AddChild((GuiWidget)(object)fatFlatClickWidget, -1);
			GuiWidget val12 = new GuiWidget(2.0, 2.0, (SizeLimitsToSet)1);
			val12.set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			val12.set_VAnchor((VAnchor)5);
			((GuiWidget)val10).AddChild(val12, -1);
			TextWidget val13 = new TextWidget("Enqueue".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val13.set_TextColor(RGBA_Bytes.White);
			((GuiWidget)val13).set_VAnchor((VAnchor)2);
			((GuiWidget)val13).set_HAnchor((HAnchor)2);
			FatFlatClickWidget fatFlatClickWidget2 = new FatFlatClickWidget(val13);
			((GuiWidget)fatFlatClickWidget2).set_Name("Row Item " + ((GuiWidget)partLabel).get_Text() + " Enqueue Button");
			((GuiWidget)fatFlatClickWidget2).set_VAnchor((VAnchor)5);
			((GuiWidget)fatFlatClickWidget2).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryAccentColor());
			((GuiWidget)fatFlatClickWidget2).set_Width(100.0);
			fatFlatClickWidget2.Click += delegate
			{
				QueueData.Instance.AddItem(new PrintItemWrapper(printTask.PrintItemId));
				((GuiWidget)this).Invalidate();
			};
			((GuiWidget)val10).AddChild((GuiWidget)(object)fatFlatClickWidget2, -1);
			GuiWidget val14 = new GuiWidget(2.0, 2.0, (SizeLimitsToSet)1);
			val14.set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			val14.set_VAnchor((VAnchor)5);
			((GuiWidget)val10).AddChild(val14, -1);
			TextWidget val15 = new TextWidget("Print".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val15.set_TextColor(RGBA_Bytes.White);
			((GuiWidget)val15).set_VAnchor((VAnchor)2);
			((GuiWidget)val15).set_HAnchor((HAnchor)2);
			FatFlatClickWidget fatFlatClickWidget3 = new FatFlatClickWidget(val15);
			((GuiWidget)fatFlatClickWidget3).set_VAnchor((VAnchor)5);
			((GuiWidget)fatFlatClickWidget3).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryAccentColor());
			((GuiWidget)fatFlatClickWidget3).set_Width((double)actionButtonSize);
			fatFlatClickWidget3.Click += delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					if (!PrinterConnectionAndCommunication.Instance.PrintIsActive)
					{
						QueueData.Instance.AddItem(new PrintItemWrapper(printTask.PrintItemId), 0);
						QueueData.Instance.SelectedIndex = 0;
						PrinterConnectionAndCommunication.Instance.PrintActivePartIfPossible();
					}
					else
					{
						QueueData.Instance.AddItem(new PrintItemWrapper(printTask.PrintItemId));
					}
					rightButtonOverlay.SlideOut();
				});
			};
			((GuiWidget)val10).AddChild((GuiWidget)(object)fatFlatClickWidget3, -1);
			((GuiWidget)rightButtonOverlay).AddChild((GuiWidget)(object)val10, -1);
			if (showTimestamp)
			{
				FlowLayoutWidget val16 = new FlowLayoutWidget((FlowDirection)3);
				((GuiWidget)val16).set_VAnchor((VAnchor)5);
				((GuiWidget)val16).set_BackgroundColor(RGBA_Bytes.LightGray);
				((GuiWidget)val16).set_Padding(new BorderDouble(6.0, 0.0));
				FlowLayoutWidget val17 = new FlowLayoutWidget((FlowDirection)0);
				((GuiWidget)val17).set_HAnchor((HAnchor)5);
				((GuiWidget)val17).set_Padding(new BorderDouble(0.0, 3.0));
				TextWidget val18 = new TextWidget(StringHelper.FormatWith("{0}:", new object[1]
				{
					"Start".Localize().ToUpper()
				}), 0.0, 0.0, (double)(8f * pointSizeFactor), (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				val18.set_TextColor(textColor);
				TextWidget val19 = new TextWidget(printTask.PrintStart.ToString("MMM d yyyy h:mm ") + printTask.PrintStart.ToString("tt").ToLower(), 0.0, 0.0, (double)(12f * pointSizeFactor), (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				val19.set_TextColor(textColor);
				((GuiWidget)val17).AddChild((GuiWidget)(object)val18, -1);
				((GuiWidget)val17).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
				((GuiWidget)val17).AddChild((GuiWidget)(object)val19, -1);
				FlowLayoutWidget val20 = new FlowLayoutWidget((FlowDirection)0);
				((GuiWidget)val20).set_HAnchor((HAnchor)5);
				((GuiWidget)val20).set_Padding(new BorderDouble(0.0, 3.0));
				TextWidget val21 = new TextWidget(StringHelper.FormatWith("{0}:", new object[1]
				{
					"End".Localize().ToUpper()
				}), 0.0, 0.0, (double)(8f * pointSizeFactor), (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				val21.set_TextColor(textColor);
				string text2 = ((!(printTask.PrintEnd != DateTime.MinValue)) ? "Unknown".Localize() : (printTask.PrintEnd.ToString("MMM d yyyy h:mm ") + printTask.PrintEnd.ToString("tt").ToLower()));
				TextWidget val22 = new TextWidget(text2, 0.0, 0.0, (double)(12f * pointSizeFactor), (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				val22.set_TextColor(textColor);
				((GuiWidget)val20).AddChild((GuiWidget)(object)val21, -1);
				((GuiWidget)val20).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
				((GuiWidget)val20).AddChild((GuiWidget)(object)val22, -1);
				HorizontalLine horizontalLine = new HorizontalLine();
				((GuiWidget)horizontalLine).set_BackgroundColor(RGBA_Bytes.Gray);
				((GuiWidget)val16).AddChild((GuiWidget)(object)val20, -1);
				((GuiWidget)val16).AddChild((GuiWidget)(object)horizontalLine, -1);
				((GuiWidget)val16).AddChild((GuiWidget)(object)val17, -1);
				((GuiWidget)val16).set_HAnchor((HAnchor)5);
				((GuiWidget)val16).set_Padding(new BorderDouble(5.0, 0.0, 15.0, 0.0));
				((GuiWidget)val9).AddChild((GuiWidget)(object)val16, -1);
			}
			val.AddChild(val8, -1);
			val.AddChild((GuiWidget)(object)rightButtonOverlay, -1);
			((GuiWidget)this).AddChild(val, -1);
		}

		private void SetDisplayAttributes()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			linkButtonFactory.fontSize = 10.0;
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			((GuiWidget)this).set_Height(50.0);
			((GuiWidget)this).set_BackgroundColor(WidgetBackgroundColor);
			((GuiWidget)this).set_Padding(new BorderDouble(0.0));
			((GuiWidget)this).set_Margin(new BorderDouble(6.0, 0.0, 6.0, 6.0));
		}

		private void AddHandlers()
		{
			((GuiWidget)this).add_MouseEnterBounds((EventHandler)HistoryItem_MouseEnterBounds);
			((GuiWidget)this).add_MouseLeaveBounds((EventHandler)HistoryItem_MouseLeaveBounds);
		}

		private void ViewButton_Click(object sender, EventArgs e)
		{
			rightButtonOverlay.SlideOut();
			ITableQuery<PrintItem> tableQuery = Datastore.Instance.dbSQLite.Table<PrintItem>();
			ParameterExpression val = Expression.Parameter(typeof(PrintItem), "v");
			PrintItem printItem = tableQuery.Where(Expression.Lambda<Func<PrintItem, bool>>((Expression)(object)Expression.Equal((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (Expression)(object)Expression.Property((Expression)(object)Expression.Field((Expression)(object)Expression.Constant((object)this, typeof(PrintHistoryListItem)), FieldInfo.GetFieldFromHandle((RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/)), (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/))), (ParameterExpression[])(object)new ParameterExpression[1]
			{
				val
			})).Take(1).FirstOrDefault();
			if (printItem == null)
			{
				return;
			}
			if (File.Exists(printItem.FileLocation))
			{
				if (Keyboard.IsKeyDown((Keys)16))
				{
					OpenPartPreviewWindow(printItem, View3DWidget.AutoRotate.Disabled);
				}
				else
				{
					OpenPartPreviewWindow(printItem, View3DWidget.AutoRotate.Enabled);
				}
			}
			else
			{
				PrintItemWrapper printItemWrapper = new PrintItemWrapper(printItem);
				ShowCantFindFileMessage(printItemWrapper);
			}
		}

		public void ShowCantFindFileMessage(PrintItemWrapper printItemWrapper)
		{
			itemToRemove = printItemWrapper;
			UiThread.RunOnIdle((Action)delegate
			{
				string text = printItemWrapper.FileLocation;
				int num = 43;
				if (text.Length > num)
				{
					string text2 = text.Substring(0, 15) + "...";
					int num2 = num - text2.Length;
					string str = text.Substring(text.Length - num2, num2);
					text = text2 + str;
				}
				string text3 = "Oops! Could not find this file:".Localize();
				string message = StringHelper.FormatWith("{0}:\n'{1}'", new object[2]
				{
					text3,
					text
				});
				string caption = "Item not Found".Localize();
				StyledMessageBox.ShowMessageBox(onConfirmRemove, message, caption);
			});
		}

		private void onConfirmRemove(bool messageBoxResponse)
		{
			if (messageBoxResponse)
			{
				QueueData.Instance.RemoveIndexOnIdle(QueueData.Instance.GetIndex(itemToRemove));
			}
		}

		private void OpenPartPreviewWindow(PrintItem printItem, View3DWidget.AutoRotate autoRotate)
		{
			PrintItemWrapper printItem2 = new PrintItemWrapper(printItem.Id);
			if (partPreviewWindow == null)
			{
				partPreviewWindow = new PartPreviewMainWindow(printItem2, autoRotate);
				((GuiWidget)partPreviewWindow).add_Closed((EventHandler<ClosedEventArgs>)PartPreviewWindow_Closed);
			}
			else
			{
				((GuiWidget)partPreviewWindow).BringToFront();
			}
		}

		private void PartPreviewWindow_Closed(object sender, ClosedEventArgs e)
		{
			partPreviewWindow = null;
		}

		private void HistoryItem_MouseLeaveBounds(object sender, EventArgs e)
		{
			rightButtonOverlay.SlideOut();
		}

		private void HistoryItem_MouseEnterBounds(object sender, EventArgs e)
		{
			rightButtonOverlay.SlideIn();
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((GuiWidget)this).OnClosed(e);
		}

		public void ThemeChanged(object sender, EventArgs e)
		{
			((GuiWidget)this).Invalidate();
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected O, but got Unknown
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Expected O, but got Unknown
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).OnDraw(graphics2D);
			if (isSelectedItem)
			{
				((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
				partLabel.set_TextColor(RGBA_Bytes.White);
				selectionCheckBox.set_TextColor(RGBA_Bytes.White);
			}
			else if (isHoverItem)
			{
				RoundedRect val = new RoundedRect(((GuiWidget)this).get_LocalBounds(), 0.0);
				((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryAccentColor());
				partLabel.set_TextColor(RGBA_Bytes.White);
				selectionCheckBox.set_TextColor(RGBA_Bytes.White);
				graphics2D.Render((IVertexSource)new Stroke((IVertexSource)(object)val, 3.0), (IColorType)(object)ActiveTheme.get_Instance().get_PrimaryAccentColor());
			}
			else
			{
				((GuiWidget)this).set_BackgroundColor(new RGBA_Bytes(255, 255, 255, 255));
				partLabel.set_TextColor(RGBA_Bytes.Black);
			}
		}
	}
}
