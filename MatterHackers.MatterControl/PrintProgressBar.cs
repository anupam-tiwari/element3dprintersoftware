using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class PrintProgressBar : GuiWidget
	{
		private double currentPercent;

		private RGBA_Bytes completeColor = new RGBA_Bytes(255, 255, 255);

		private TextWidget printTimeRemaining;

		private TextWidget printTimeElapsed;

		private EventHandler unregisterEvents;

		private bool widgetIsExtended;

		private ImageBuffer upImageBuffer;

		private ImageBuffer downImageBuffer;

		private ImageWidget indicatorWidget;

		public bool WidgetIsExtended
		{
			get
			{
				return widgetIsExtended;
			}
			set
			{
				widgetIsExtended = value;
				ToggleExtendedDisplayProperties();
			}
		}

		private void ToggleExtendedDisplayProperties()
		{
			if (!WidgetIsExtended)
			{
				indicatorWidget.set_Image(downImageBuffer);
			}
			else
			{
				indicatorWidget.set_Image(upImageBuffer);
			}
		}

		public PrintProgressBar(bool widgetIsExtended = true)
			: this()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Expected O, but got Unknown
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Expected O, but got Unknown
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Expected O, but got Unknown
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Expected O, but got Unknown
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Expected O, but got Unknown
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Expected O, but got Unknown
			((GuiWidget)this).set_MinimumSize(new Vector2(0.0, 24.0));
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryAccentColor());
			((GuiWidget)this).set_Margin(new BorderDouble(0.0));
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).AnchorAll();
			((GuiWidget)val).set_Padding(new BorderDouble(6.0, 0.0));
			printTimeElapsed = new TextWidget("", 0.0, 0.0, 11.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			printTimeElapsed.get_Printer().set_DrawFromHintedCache(true);
			printTimeElapsed.set_AutoExpandBoundsToText(true);
			((GuiWidget)printTimeElapsed).set_VAnchor((VAnchor)2);
			printTimeRemaining = new TextWidget("", 0.0, 0.0, 11.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			printTimeRemaining.get_Printer().set_DrawFromHintedCache(true);
			printTimeRemaining.set_AutoExpandBoundsToText(true);
			((GuiWidget)printTimeRemaining).set_VAnchor((VAnchor)2);
			((GuiWidget)val).AddChild((GuiWidget)(object)printTimeElapsed, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)printTimeRemaining, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			if (UserSettings.Instance.IsTouchScreen)
			{
				upImageBuffer = StaticData.get_Instance().LoadIcon("TouchScreen/arrow_up_32x24.png");
				downImageBuffer = StaticData.get_Instance().LoadIcon("TouchScreen/arrow_down_32x24.png");
				indicatorWidget = new ImageWidget(upImageBuffer);
				((GuiWidget)indicatorWidget).set_HAnchor((HAnchor)2);
				((GuiWidget)indicatorWidget).set_VAnchor((VAnchor)2);
				WidgetIsExtended = widgetIsExtended;
				GuiWidget val2 = new GuiWidget();
				val2.AnchorAll();
				val2.AddChild((GuiWidget)(object)indicatorWidget, -1);
				((GuiWidget)this).AddChild(val2, -1);
			}
			GuiWidget val3 = new GuiWidget();
			val3.AnchorAll();
			val3.add_Click((EventHandler<MouseEventArgs>)delegate
			{
				ApplicationView mainView = ApplicationController.Instance.MainView;
				if (mainView is TouchscreenView)
				{
					((TouchscreenView)mainView).ToggleTopContainer();
				}
			});
			((GuiWidget)this).AddChild(val3, -1);
			PrinterConnectionAndCommunication.Instance.ActivePrintItemChanged.RegisterEvent((EventHandler)Instance_PrintItemChanged, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)Instance_PrintItemChanged, ref unregisterEvents);
			SetThemedColors();
			if (!UserSettings.Instance.IsTouchScreen)
			{
				UiThread.RunOnIdle((Action)OnIdle);
				UpdatePrintStatus();
			}
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		private void SetThemedColors()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			printTimeElapsed.set_TextColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			printTimeRemaining.set_TextColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryAccentColor());
		}

		public void ThemeChanged(object sender, EventArgs e)
		{
			SetThemedColors();
			((GuiWidget)this).Invalidate();
		}

		private void Instance_PrintItemChanged(object sender, EventArgs e)
		{
			UpdatePrintStatus();
		}

		private void OnIdle()
		{
			currentPercent = PrinterConnectionAndCommunication.Instance.PercentComplete;
			UpdatePrintStatus();
			if (!((GuiWidget)this).get_HasBeenClosed())
			{
				UiThread.RunOnIdle((Action)OnIdle, 1.0);
			}
		}

		private void UpdatePrintStatus()
		{
			if (PrinterConnectionAndCommunication.Instance.ActivePrintItem == null)
			{
				((GuiWidget)printTimeElapsed).set_Text(string.Format(""));
				((GuiWidget)printTimeRemaining).set_Text(string.Format(""));
			}
			else
			{
				int secondsPrinted = PrinterConnectionAndCommunication.Instance.SecondsPrinted;
				int num = secondsPrinted / 3600;
				int num2 = secondsPrinted / 60 - num * 60;
				secondsPrinted %= 60;
				if (secondsPrinted > 0)
				{
					if (num > 0)
					{
						((GuiWidget)printTimeElapsed).set_Text($"{num}:{num2:00}:{secondsPrinted:00}");
					}
					else
					{
						((GuiWidget)printTimeElapsed).set_Text($"{num2}:{secondsPrinted:00}");
					}
				}
				else
				{
					((GuiWidget)printTimeElapsed).set_Text(string.Format(""));
				}
				string text = $"{currentPercent:0.0}%";
				if (PrinterConnectionAndCommunication.Instance.PrinterIsPrinting || PrinterConnectionAndCommunication.Instance.PrinterIsPaused)
				{
					((GuiWidget)printTimeRemaining).set_Text(text);
				}
				else if (PrinterConnectionAndCommunication.Instance.PrintIsFinished)
				{
					((GuiWidget)printTimeRemaining).set_Text("Done!");
				}
				else
				{
					((GuiWidget)printTimeRemaining).set_Text(string.Format(""));
				}
			}
			((GuiWidget)this).Invalidate();
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			graphics2D.FillRectangle(0.0, 0.0, ((GuiWidget)this).get_Width() * currentPercent / 100.0, ((GuiWidget)this).get_Height(), (IColorType)(object)completeColor);
			((GuiWidget)this).OnDraw(graphics2D);
		}
	}
}
