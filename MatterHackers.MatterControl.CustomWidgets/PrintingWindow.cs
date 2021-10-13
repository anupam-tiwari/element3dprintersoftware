using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.CustomWidgets
{
	public class PrintingWindow : SystemWindow
	{
		protected EventHandler unregisterEvents;

		private static PrintingWindow instance;

		private TextImageButtonFactory buttonFactory = new TextImageButtonFactory
		{
			fontSize = 15.0,
			invertImageLocation = false,
			normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor(),
			hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor(),
			disabledTextColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 100),
			disabledFillColor = RGBA_Bytes.Transparent,
			pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor()
		};

		private AverageMillisecondTimer millisecondTimer = new AverageMillisecondTimer();

		private Stopwatch totalDrawTime = new Stopwatch();

		private GuiWidget bodyContainer;

		private BasicBody basicBody;

		public static bool IsShowing
		{
			get
			{
				if (instance != null)
				{
					return true;
				}
				return false;
			}
		}

		public PrintingWindow()
			: this(1280.0, 750.0)
		{
		}//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown


		public override void OnLoad(EventArgs args)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Expected O, but got Unknown
			bool smallScreen = ((GuiWidget)this).get_Parent().get_Width() <= 1180.0;
			((SystemWindow)this).set_AlwaysOnTopOfMain(true);
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((SystemWindow)this).set_Title("Print Monitor".Localize());
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			FlowLayoutWidget val2 = val;
			((GuiWidget)this).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val2).AddChild(CreateActionBar(smallScreen), -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)CreateHorizontalLine(), -1);
			((GuiWidget)val2).AddChild(CreateDropShadow(), -1);
			basicBody = new BasicBody();
			GuiWidget val3 = new GuiWidget();
			val3.set_VAnchor((VAnchor)5);
			val3.set_HAnchor((HAnchor)5);
			bodyContainer = val3;
			bodyContainer.AddChild((GuiWidget)(object)basicBody, -1);
			((GuiWidget)val2).AddChild(bodyContainer, -1);
			((GuiWidget)this).OnLoad(args);
		}

		private GuiWidget CreateActionBar(bool smallScreen)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Expected O, but got Unknown
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_VAnchor((VAnchor)12);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			ImageBuffer val2 = StaticData.get_Instance().LoadImage(Path.Combine("Images", "Screensaver", "logo.png"));
			if (!ActiveTheme.get_Instance().get_IsDarkTheme())
			{
				ExtensionMethods.InvertLightness(val2);
			}
			((GuiWidget)val).AddChild((GuiWidget)new ImageWidget(val2), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			Button pauseButton = CreateButton("Pause".Localize().ToUpper(), smallScreen);
			((GuiWidget)pauseButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					PrinterConnectionAndCommunication.Instance.RequestPause();
				});
			});
			((GuiWidget)pauseButton).set_Enabled(PrinterConnectionAndCommunication.Instance.PrinterIsPrinting && !PrinterConnectionAndCommunication.Instance.PrinterIsPaused);
			((GuiWidget)val).AddChild((GuiWidget)(object)pauseButton, -1);
			Button resumeButton = CreateButton("Resume".Localize().ToUpper(), smallScreen);
			((GuiWidget)resumeButton).set_Visible(false);
			((GuiWidget)resumeButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					if (PrinterConnectionAndCommunication.Instance.PrinterIsPaused)
					{
						PrinterConnectionAndCommunication.Instance.Resume();
					}
				});
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)resumeButton, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)CreateVerticalLine(), -1);
			Button cancelButton = CreateButton("Cancel".Localize().ToUpper(), smallScreen);
			((GuiWidget)cancelButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				if (ApplicationController.Instance.ConditionalCancelPrint())
				{
					((GuiWidget)this).Close();
				}
			});
			((GuiWidget)cancelButton).set_Enabled(PrinterConnectionAndCommunication.Instance.PrinterIsPrinting || PrinterConnectionAndCommunication.Instance.PrinterIsPaused);
			((GuiWidget)val).AddChild((GuiWidget)(object)cancelButton, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)CreateVerticalLine(), -1);
			Button val3 = CreateButton("Reset".Localize().ToUpper(), smallScreen, centerText: true, StaticData.get_Instance().LoadIcon("e_stop4.png", 32, 32));
			((GuiWidget)val3).set_Visible(ActiveSliceSettings.Instance.GetValue<bool>("show_reset_connection"));
			((GuiWidget)val3).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)PrinterConnectionAndCommunication.Instance.RebootBoard);
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)CreateVerticalLine(), -1);
			Button advancedButton = CreateButton("Advanced".Localize().ToUpper(), smallScreen);
			((GuiWidget)val).AddChild((GuiWidget)(object)advancedButton, -1);
			((GuiWidget)advancedButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				if (((Collection<GuiWidget>)(object)bodyContainer.get_Children())[0] is BasicBody)
				{
					bodyContainer.RemoveChild((GuiWidget)(object)basicBody);
					GuiWidget obj = bodyContainer;
					ManualPrinterControls manualPrinterControls = new ManualPrinterControls();
					((GuiWidget)manualPrinterControls).set_VAnchor((VAnchor)5);
					((GuiWidget)manualPrinterControls).set_HAnchor((HAnchor)5);
					obj.AddChild((GuiWidget)(object)manualPrinterControls, -1);
					((GuiWidget)advancedButton).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
				}
				else
				{
					bodyContainer.CloseAllChildren();
					((GuiWidget)basicBody).ClearRemovedFlag();
					bodyContainer.AddChild((GuiWidget)(object)basicBody, -1);
					((GuiWidget)advancedButton).set_BackgroundColor(RGBA_Bytes.Transparent);
				}
			});
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)delegate
			{
				((GuiWidget)pauseButton).set_Enabled(PrinterConnectionAndCommunication.Instance.PrinterIsPrinting && !PrinterConnectionAndCommunication.Instance.PrinterIsPaused);
				if (PrinterConnectionAndCommunication.Instance.PrinterIsPaused)
				{
					((GuiWidget)resumeButton).set_Visible(true);
					((GuiWidget)pauseButton).set_Visible(false);
				}
				else
				{
					((GuiWidget)resumeButton).set_Visible(false);
					((GuiWidget)pauseButton).set_Visible(true);
				}
				PrinterConnectionAndCommunication.CommunicationStates communicationState = PrinterConnectionAndCommunication.Instance.CommunicationState;
				if ((uint)(communicationState - 4) > 1u && communicationState != PrinterConnectionAndCommunication.CommunicationStates.Paused)
				{
					((GuiWidget)this).CloseOnIdle();
				}
			}, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)delegate
			{
				((GuiWidget)cancelButton).set_Enabled(PrinterConnectionAndCommunication.Instance.PrinterIsPrinting || PrinterConnectionAndCommunication.Instance.PrinterIsPaused);
			}, ref unregisterEvents);
			return (GuiWidget)val;
		}

		public static void Show()
		{
			if (instance == null)
			{
				instance = new PrintingWindow();
				((SystemWindow)instance).ShowAsSystemWindow();
			}
			else
			{
				((GuiWidget)instance).BringToFront();
			}
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			BasicBody obj = basicBody;
			if (obj != null)
			{
				((GuiWidget)obj).Close();
			}
			unregisterEvents?.Invoke(this, null);
			instance = null;
			((SystemWindow)this).OnClosed(e);
		}

		private Button CreateButton(string localizedText, bool smallScreen, bool centerText = true, ImageBuffer icon = null)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			Button val = null;
			val = ((!(icon == (ImageBuffer)null)) ? buttonFactory.GenerateTooltipButton(localizedText, icon) : buttonFactory.Generate(localizedText, (string)null, (string)null, (string)null, (string)null, centerText));
			RectangleDouble localBounds = ((GuiWidget)val).get_LocalBounds();
			if (smallScreen)
			{
				((RectangleDouble)(ref localBounds)).Inflate(new BorderDouble(10.0, 10.0));
			}
			else
			{
				((RectangleDouble)(ref localBounds)).Inflate(new BorderDouble(40.0, 10.0));
			}
			((GuiWidget)val).set_LocalBounds(localBounds);
			((GuiWidget)val).set_Cursor((Cursors)3);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0));
			foreach (GuiWidget item in (Collection<GuiWidget>)(object)((GuiWidget)val).get_Children())
			{
				item.set_VAnchor((VAnchor)2);
			}
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			return val;
		}

		private GuiWidget CreateDropShadow()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			GuiWidget val = new GuiWidget();
			val.set_HAnchor((HAnchor)5);
			val.set_Height(12.0 * GuiWidget.get_DeviceScale());
			val.set_DoubleBuffer(true);
			GuiWidget dropShadowWidget = val;
			dropShadowWidget.add_AfterDraw((EventHandler<DrawEventArgs>)delegate
			{
				byte[] buffer = dropShadowWidget.get_BackBuffer().GetBuffer();
				for (int i = 0; (double)i < dropShadowWidget.get_Height(); i++)
				{
					int bufferOffsetY = dropShadowWidget.get_BackBuffer().GetBufferOffsetY(i);
					byte b = (byte)((double)i / dropShadowWidget.get_Height() * 100.0);
					for (int j = 0; (double)j < dropShadowWidget.get_Width(); j++)
					{
						buffer[bufferOffsetY + j * 4] = 0;
						buffer[bufferOffsetY + j * 4 + 1] = 0;
						buffer[bufferOffsetY + j * 4 + 2] = 0;
						buffer[bufferOffsetY + j * 4 + 3] = b;
					}
				}
			});
			return dropShadowWidget;
		}

		public static HorizontalLine CreateHorizontalLine()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			HorizontalLine horizontalLine = new HorizontalLine();
			((GuiWidget)horizontalLine).set_BackgroundColor(new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 50));
			return horizontalLine;
		}

		public static VerticalLine CreateVerticalLine()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			VerticalLine verticalLine = new VerticalLine();
			((GuiWidget)verticalLine).set_BackgroundColor(new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 50));
			return verticalLine;
		}
	}
}
