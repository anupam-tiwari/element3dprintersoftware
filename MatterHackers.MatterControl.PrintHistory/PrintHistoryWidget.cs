using System;
using System.Linq.Expressions;
using System.Reflection;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.DataStorage;

namespace MatterHackers.MatterControl.PrintHistory
{
	public class PrintHistoryWidget : GuiWidget
	{
		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private CheckBox showOnlyCompletedCheckbox;

		private CheckBox showTimestampCheckbox;

		private PrintHistoryDataView historyView;

		private TextWidget completedPrintsCount;

		private TextWidget totalPrintTime;

		public PrintHistoryWidget()
			: this()
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Expected O, but got Unknown
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Expected O, but got Unknown
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Expected O, but got Unknown
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Expected O, but got Unknown
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Expected O, but got Unknown
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Expected O, but got Unknown
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Expected O, but got Unknown
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Expected O, but got Unknown
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Expected O, but got Unknown
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			SetDisplayAttributes();
			textImageButtonFactory.borderWidth = 0.0;
			RGBA_Bytes primaryTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Padding(new BorderDouble(6.0, 2.0));
			showOnlyCompletedCheckbox = new CheckBox("Only Show Completed".Localize(), primaryTextColor, 10.0);
			((GuiWidget)showOnlyCompletedCheckbox).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 8.0));
			bool @checked = UserSettings.Instance.get("PrintHistoryFilterShowCompleted") == "true";
			showOnlyCompletedCheckbox.set_Checked(@checked);
			((GuiWidget)showOnlyCompletedCheckbox).set_Width(200.0);
			((GuiWidget)val2).AddChild((GuiWidget)new TextWidget("Completed Prints:".Localize() + " ", 0.0, 0.0, 10.0, (Justification)0, primaryTextColor, true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			completedPrintsCount = new TextWidget(GetCompletedPrints().ToString(), 0.0, 0.0, 14.0, (Justification)0, primaryTextColor, true, false, default(RGBA_Bytes), (TypeFace)null);
			completedPrintsCount.set_AutoExpandBoundsToText(true);
			((GuiWidget)val2).AddChild((GuiWidget)(object)completedPrintsCount, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)showOnlyCompletedCheckbox, -1);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_Padding(new BorderDouble(6.0, 2.0));
			showTimestampCheckbox = new CheckBox("Show Timestamp".Localize(), primaryTextColor, 10.0);
			bool checked2 = UserSettings.Instance.get("PrintHistoryFilterShowTimestamp") == "true";
			showTimestampCheckbox.set_Checked(checked2);
			((GuiWidget)showTimestampCheckbox).set_Width(200.0);
			((GuiWidget)val3).AddChild((GuiWidget)new TextWidget("Total Print Time:".Localize() + " ", 0.0, 0.0, 10.0, (Justification)0, primaryTextColor, true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			totalPrintTime = new TextWidget(GetPrintTimeString(), 0.0, 0.0, 14.0, (Justification)0, primaryTextColor, true, false, default(RGBA_Bytes), (TypeFace)null);
			totalPrintTime.set_AutoExpandBoundsToText(true);
			((GuiWidget)val3).AddChild((GuiWidget)(object)totalPrintTime, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)showTimestampCheckbox, -1);
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val4).set_BackgroundColor(ActiveTheme.get_Instance().get_TransparentDarkOverlay());
			((GuiWidget)val4).set_HAnchor((HAnchor)5);
			((GuiWidget)val4).set_Padding(new BorderDouble(0.0, 6.0, 0.0, 2.0));
			((GuiWidget)val4).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)val3, -1);
			FlowLayoutWidget val5 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val5).set_HAnchor((HAnchor)5);
			((GuiWidget)val5).set_Padding(new BorderDouble(0.0, 3.0));
			((GuiWidget)val5).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val4, -1);
			historyView = new PrintHistoryDataView();
			historyView.DoneLoading += historyView_DoneLoading;
			((GuiWidget)val).AddChild((GuiWidget)(object)historyView, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val5, -1);
			((GuiWidget)val).AnchorAll();
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			AddHandlers();
		}

		private void historyView_DoneLoading(object sender, EventArgs e)
		{
			UpdateCompletedCount();
		}

		private void UpdateCompletedCount()
		{
			((GuiWidget)completedPrintsCount).set_Text(GetCompletedPrints().ToString());
			((GuiWidget)totalPrintTime).set_Text(GetPrintTimeString());
		}

		private void AddHandlers()
		{
			showOnlyCompletedCheckbox.add_CheckedStateChanged((EventHandler)UpdateHistoryFilterShowCompleted);
			showTimestampCheckbox.add_CheckedStateChanged((EventHandler)UpdateHistoryFilterShowTimestamp);
		}

		private void UpdateHistoryFilterShowCompleted(object sender, EventArgs e)
		{
			if (showOnlyCompletedCheckbox.get_Checked())
			{
				UserSettings.Instance.set("PrintHistoryFilterShowCompleted", "true");
			}
			else
			{
				UserSettings.Instance.set("PrintHistoryFilterShowCompleted", "false");
			}
			historyView.LoadHistoryItems();
		}

		private void UpdateHistoryFilterShowTimestamp(object sender, EventArgs e)
		{
			if (showTimestampCheckbox.get_Checked())
			{
				UserSettings.Instance.set("PrintHistoryFilterShowTimestamp", "true");
			}
			else
			{
				UserSettings.Instance.set("PrintHistoryFilterShowTimestamp", "false");
			}
			historyView.ShowTimestamp = showTimestampCheckbox.get_Checked();
			historyView.LoadHistoryItems();
		}

		private int GetCompletedPrints()
		{
			ITableQuery<PrintTask> tableQuery = Datastore.Instance.dbSQLite.Table<PrintTask>();
			ParameterExpression val = Expression.Parameter(typeof(PrintTask), "o");
			return tableQuery.Where(Expression.Lambda<Func<PrintTask, bool>>((Expression)(object)Expression.Equal((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (Expression)(object)Expression.Constant((object)true, typeof(bool))), (ParameterExpression[])(object)new ParameterExpression[1]
			{
				val
			})).Count();
		}

		private int GetTotalPrintSeconds()
		{
			return Datastore.Instance.dbSQLite.ExecuteScalar<int>("SELECT SUM(PrintTimeSeconds) FROM PrintTask", Array.Empty<object>());
		}

		private string GetPrintTimeString()
		{
			int totalPrintSeconds = GetTotalPrintSeconds();
			TimeSpan timeSpan = new TimeSpan(0, 0, totalPrintSeconds);
			if (totalPrintSeconds <= 0)
			{
				return "0min";
			}
			if (totalPrintSeconds > 86400)
			{
				return StringHelper.FormatWith("{0}d {1}hrs {2}min", new object[3]
				{
					timeSpan.Days,
					timeSpan.Hours,
					timeSpan.Minutes
				});
			}
			if (totalPrintSeconds > 3600)
			{
				return StringHelper.FormatWith("{0}hrs {1}min", new object[2]
				{
					timeSpan.Hours,
					timeSpan.Minutes
				});
			}
			return StringHelper.FormatWith("{0}min", new object[1]
			{
				timeSpan.Minutes
			});
		}

		private void SetDisplayAttributes()
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_Padding(new BorderDouble(3.0));
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)this).AnchorAll();
		}
	}
}
