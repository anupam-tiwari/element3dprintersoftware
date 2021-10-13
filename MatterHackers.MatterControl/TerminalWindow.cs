using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class TerminalWindow : SystemWindow
	{
		private static readonly Vector2 minSize = new Vector2(400.0, 300.0);

		private static readonly string TerminalWindowLeftOpen = "TerminalWindowLeftOpen";

		private static readonly string TerminalWindowSizeKey = "TerminalWindowSize";

		private static readonly string TerminalWindowPositionKey = "TerminalWindowPosition";

		private static TerminalWindow connectionWindow = null;

		private static bool terminalWasOpenOnAppClose = false;

		public static void Show()
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			if (connectionWindow == null)
			{
				terminalWasOpenOnAppClose = false;
				string text = UserSettings.Instance.get(TerminalWindowSizeKey);
				int width = 400;
				int height = 300;
				if (text != null && text != "")
				{
					string[] array = text.Split(new char[1]
					{
						','
					});
					width = Math.Max(int.Parse(array[0]), (int)minSize.x);
					height = Math.Max(int.Parse(array[1]), (int)minSize.y);
				}
				connectionWindow = new TerminalWindow(width, height);
				((GuiWidget)connectionWindow).add_Closed((EventHandler<ClosedEventArgs>)delegate
				{
					connectionWindow = null;
				});
				UserSettings.Instance.Fields.SetBool(TerminalWindowLeftOpen, value: true);
			}
			else
			{
				((GuiWidget)connectionWindow).BringToFront();
			}
		}

		public static void ShowIfLeftOpen()
		{
			if (UserSettings.Instance.Fields.GetBool(TerminalWindowLeftOpen, defaultValue: false))
			{
				Show();
			}
		}

		public static void CloseIfOpen()
		{
			if (connectionWindow != null)
			{
				terminalWasOpenOnAppClose = true;
				((GuiWidget)connectionWindow).Close();
			}
		}

		private TerminalWindow(int width, int height)
			: this((double)width, (double)height)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			((SystemWindow)this).set_AlwaysOnTopOfMain(true);
			((GuiWidget)this).AddChild((GuiWidget)(object)new TerminalWidget(showInWindow: true), -1);
			((SystemWindow)this).set_Title("Element - Terminal".Localize());
			((SystemWindow)this).ShowAsSystemWindow();
			((GuiWidget)this).set_MinimumSize(minSize);
			((GuiWidget)this).set_Name("Gcode Terminal");
			string text = UserSettings.Instance.get(TerminalWindowPositionKey);
			if (text != null && text != "")
			{
				string[] array = text.Split(new char[1]
				{
					','
				});
				int num = Math.Max(int.Parse(array[0]), -10);
				int num2 = Math.Max(int.Parse(array[1]), -10);
				((SystemWindow)this).set_DesktopPosition(new Point2D(num, num2));
			}
		}

		private void SaveOnClosing()
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			UserSettings.Instance.set(TerminalWindowSizeKey, $"{((GuiWidget)this).get_Width()},{((GuiWidget)this).get_Height()}");
			UserSettings.Instance.set(TerminalWindowPositionKey, $"{((SystemWindow)this).get_DesktopPosition().x},{((SystemWindow)this).get_DesktopPosition().y}");
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			SaveOnClosing();
			UserSettings.Instance.Fields.SetBool(TerminalWindowLeftOpen, terminalWasOpenOnAppClose);
			((SystemWindow)this).OnClosed(e);
		}
	}
}
