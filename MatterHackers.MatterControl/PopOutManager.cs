using System;
using System.Collections.ObjectModel;
using System.Globalization;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class PopOutManager
	{
		private static readonly string PositionSufix = "_WindowPosition";

		private static readonly string WindowLeftOpenSufix = "_WindowLeftOpen";

		private static readonly string WindowSizeSufix = "_WindowSize";

		private string dataBaseKeyPrefix;

		private Vector2 minSize;

		private SystemWindow systemWindowWithPopContent;

		private string PositionKey;

		private GuiWidget widgetWithPopContent;

		private string WindowLeftOpenKey;

		private string WindowSizeKey;

		private string windowTitle;

		public static bool SaveIfClosed
		{
			get;
			set;
		}

		public PopOutManager(GuiWidget widgetWithPopContent, Vector2 minSize, string windowTitle, string dataBaseKeyPrefix)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			string str = new CultureInfo("en-US", useUserOverride: false).TextInfo.ToTitleCase(windowTitle.ToLower());
			this.windowTitle = "Element - " + str;
			this.minSize = minSize;
			this.dataBaseKeyPrefix = dataBaseKeyPrefix;
			this.widgetWithPopContent = widgetWithPopContent;
			UiThread.RunOnIdle((Action)delegate
			{
				((GuiWidget)ApplicationController.Instance.MainView).add_AfterDraw((EventHandler<DrawEventArgs>)ShowOnNextMatterControlDraw);
			});
			widgetWithPopContent.add_Closed((EventHandler<ClosedEventArgs>)delegate
			{
				WidgetWithPopContentIsClosing();
			});
			WindowLeftOpenKey = dataBaseKeyPrefix + WindowLeftOpenSufix;
			WindowSizeKey = dataBaseKeyPrefix + WindowSizeSufix;
			PositionKey = dataBaseKeyPrefix + PositionSufix;
		}

		public void ShowContentInWindow()
		{
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Expected O, but got Unknown
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			if (widgetWithPopContent.get_HasBeenClosed())
			{
				if (systemWindowWithPopContent != null)
				{
					((GuiWidget)systemWindowWithPopContent).Close();
				}
			}
			else if (systemWindowWithPopContent == null)
			{
				UserSettings.Instance.Fields.SetBool(WindowLeftOpenKey, value: true);
				string text = UserSettings.Instance.get(WindowSizeKey);
				int num = 600;
				int num2 = 400;
				if (text != null && text != "")
				{
					string[] array = text.Split(new char[1]
					{
						','
					});
					num = Math.Max(int.Parse(array[0]), (int)minSize.x);
					num2 = Math.Max(int.Parse(array[1]), (int)minSize.y);
				}
				systemWindowWithPopContent = new SystemWindow((double)num, (double)num2);
				((GuiWidget)systemWindowWithPopContent).set_Padding(new BorderDouble(3.0));
				systemWindowWithPopContent.set_Title(windowTitle);
				systemWindowWithPopContent.set_AlwaysOnTopOfMain(true);
				((GuiWidget)systemWindowWithPopContent).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
				((GuiWidget)systemWindowWithPopContent).add_Closing((EventHandler<ClosingEventArgs>)SystemWindow_Closing);
				if (((Collection<GuiWidget>)(object)widgetWithPopContent.get_Children()).Count == 1)
				{
					GuiWidget val = ((Collection<GuiWidget>)(object)widgetWithPopContent.get_Children())[0];
					widgetWithPopContent.RemoveChild(val);
					val.ClearRemovedFlag();
					widgetWithPopContent.AddChild(CreateContentForEmptyControl(), -1);
					((GuiWidget)systemWindowWithPopContent).AddChild(val, -1);
				}
				systemWindowWithPopContent.ShowAsSystemWindow();
				((GuiWidget)systemWindowWithPopContent).set_MinimumSize(minSize);
				string text2 = UserSettings.Instance.get(PositionKey);
				if (text2 != null && text2 != "")
				{
					string[] array2 = text2.Split(new char[1]
					{
						','
					});
					int num3 = Math.Max(int.Parse(array2[0]), -10);
					int num4 = Math.Max(int.Parse(array2[1]), -10);
					systemWindowWithPopContent.set_DesktopPosition(new Point2D(num3, num4));
				}
			}
			else
			{
				((GuiWidget)systemWindowWithPopContent).BringToFront();
			}
		}

		private static void SetPopOutState(string dataBaseKeyPrefix, bool poppedOut)
		{
			string keyToSet = dataBaseKeyPrefix + WindowLeftOpenSufix;
			UserSettings.Instance.Fields.SetBool(keyToSet, poppedOut);
		}

		private static void SetStates(string dataBaseKeyPrefix, bool poppedOut, double width, double height, double positionX, double positionY)
		{
			string keyToSet = dataBaseKeyPrefix + WindowLeftOpenSufix;
			string key = dataBaseKeyPrefix + WindowSizeSufix;
			string key2 = dataBaseKeyPrefix + PositionSufix;
			UserSettings.Instance.Fields.SetBool(keyToSet, poppedOut);
			UserSettings.Instance.set(key, $"{width},{height}");
			UserSettings.Instance.set(key2, $"{positionX},{positionY}");
		}

		private GuiWidget CreateContentForEmptyControl()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected O, but got Unknown
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Expected O, but got Unknown
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Expected O, but got Unknown
			GuiWidget val = new GuiWidget();
			val.set_HAnchor((HAnchor)5);
			val.set_VAnchor((VAnchor)5);
			val.set_Padding(new BorderDouble(5.0, 10.0, 5.0, 10.0));
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_BackgroundColor(ActiveTheme.get_Instance().get_TransparentDarkOverlay());
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_VAnchor((VAnchor)4);
			((GuiWidget)val2).set_Padding(new BorderDouble(10.0, 0.0));
			((GuiWidget)val2).set_Height(60.0);
			Button val3 = new TextImageButtonFactory
			{
				normalFillColor = RGBA_Bytes.Gray,
				normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor()
			}.Generate("Restore".Localize());
			((GuiWidget)val3).set_ToolTipText("Bring the Window back into this Tab".Localize());
			((GuiWidget)val3).set_VAnchor((VAnchor)2);
			((GuiWidget)val3).set_Cursor((Cursors)3);
			((GuiWidget)val3).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					SaveWindowShouldStartClosed();
					SystemWindow obj = systemWindowWithPopContent;
					SystemWindow_Closing(null, null);
					((GuiWidget)obj).Close();
				});
			});
			TextWidget val4 = new TextWidget("WINDOWED MODE: This tab has been moved to a separate window.".Localize(), 0.0, 0.0, 10.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val4).set_VAnchor((VAnchor)2);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val3, -1);
			val.AddChild((GuiWidget)(object)val2, -1);
			return val;
		}

		private void SaveSizeAndPosition()
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			if (systemWindowWithPopContent != null)
			{
				UserSettings.Instance.set(WindowSizeKey, $"{((GuiWidget)systemWindowWithPopContent).get_Width()},{((GuiWidget)systemWindowWithPopContent).get_Height()}");
				UserSettings.Instance.set(PositionKey, $"{systemWindowWithPopContent.get_DesktopPosition().x},{systemWindowWithPopContent.get_DesktopPosition().y}");
			}
		}

		private void SaveWindowShouldStartClosed()
		{
			if (!((GuiWidget)MatterControlApplication.Instance).get_HasBeenClosed() && SaveIfClosed)
			{
				UserSettings.Instance.Fields.SetBool(WindowLeftOpenKey, value: false);
			}
		}

		private void ShowOnNextMatterControlDraw(object drawingWidget, DrawEventArgs e)
		{
			if (((Collection<GuiWidget>)(object)widgetWithPopContent.get_Children()).Count > 0)
			{
				UiThread.RunOnIdle((Action)delegate
				{
					if (UserSettings.Instance.Fields.GetBool(WindowLeftOpenKey, defaultValue: false))
					{
						ShowContentInWindow();
					}
				});
			}
			((GuiWidget)ApplicationController.Instance.MainView).remove_AfterDraw((EventHandler<DrawEventArgs>)ShowOnNextMatterControlDraw);
		}

		private void SystemWindow_Closing(object sender, ClosingEventArgs closingEvent)
		{
			if (systemWindowWithPopContent != null)
			{
				SaveSizeAndPosition();
				SaveWindowShouldStartClosed();
				if (((Collection<GuiWidget>)(object)((GuiWidget)systemWindowWithPopContent).get_Children()).Count == 1)
				{
					GuiWidget val = ((Collection<GuiWidget>)(object)((GuiWidget)systemWindowWithPopContent).get_Children())[0];
					((GuiWidget)systemWindowWithPopContent).RemoveChild(val);
					val.ClearRemovedFlag();
					widgetWithPopContent.RemoveAllChildren();
					widgetWithPopContent.AddChild(val, -1);
				}
				systemWindowWithPopContent = null;
			}
		}

		private void WidgetWithPopContentIsClosing()
		{
			if (systemWindowWithPopContent != null)
			{
				SaveSizeAndPosition();
				((GuiWidget)systemWindowWithPopContent).CloseAllChildren();
				((GuiWidget)systemWindowWithPopContent).Close();
			}
		}
	}
}
