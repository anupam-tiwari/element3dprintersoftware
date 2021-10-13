using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl
{
	public class SetupWizardTroubleshooting : WizardPage
	{
		private class CriteriaRow : FlowLayoutWidget
		{
			private static bool stillSuccessful = true;

			private static int criteriaCount = 0;

			private static RGBA_Bytes disabledTextColor = new RGBA_Bytes(0.35, 0.35, 0.35);

			private static RGBA_Bytes disabledBackColor = new RGBA_Bytes(0.22, 0.22, 0.22);

			private static RGBA_Bytes toggleColor = new RGBA_Bytes(RGBA_Bytes.Gray.red + 2, RGBA_Bytes.Gray.green + 2, RGBA_Bytes.Gray.blue + 2);

			public static CriteriaRow ActiveErrorItem
			{
				get;
				private set;
			}

			public string ErrorText
			{
				get;
				private set;
			}

			public CriteriaRow(string itemText, string fixitText, string errorText, bool succeeded, Action fixAction)
				: this((FlowDirection)0)
			{
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Expected O, but got Unknown
				//IL_010f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0144: Unknown result type (might be due to invalid IL or missing references)
				//IL_014b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0158: Unknown result type (might be due to invalid IL or missing references)
				((GuiWidget)this).set_HAnchor((HAnchor)5);
				((GuiWidget)this).set_VAnchor((VAnchor)0);
				TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();
				ErrorText = errorText;
				((GuiWidget)this).set_Height(40.0);
				TextWidget val = new TextWidget($"  {criteriaCount + 1}. {itemText}", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				val.set_TextColor(stillSuccessful ? RGBA_Bytes.White : disabledTextColor);
				((GuiWidget)val).set_VAnchor((VAnchor)2);
				((GuiWidget)this).AddChild((GuiWidget)val, -1);
				if (stillSuccessful && !succeeded)
				{
					ActiveErrorItem = this;
				}
				((GuiWidget)this).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
				if (stillSuccessful)
				{
					if (succeeded)
					{
						AddSuccessIcon();
					}
					else
					{
						Button val2 = textImageButtonFactory.Generate(LocalizedString.Get(fixitText), (string)null, (string)null, (string)null, (string)null, centerText: true);
						((GuiWidget)val2).set_VAnchor((VAnchor)2);
						((GuiWidget)val2).set_Padding(new BorderDouble(3.0, 8.0));
						((GuiWidget)val2).add_Click((EventHandler<MouseEventArgs>)delegate
						{
							fixAction();
						});
						((GuiWidget)this).AddChild((GuiWidget)(object)val2, -1);
					}
				}
				if (stillSuccessful)
				{
					((GuiWidget)this).set_BackgroundColor((criteriaCount % 2 == 0) ? RGBA_Bytes.Gray : toggleColor);
				}
				else
				{
					((GuiWidget)this).set_BackgroundColor(disabledBackColor);
				}
				stillSuccessful &= succeeded;
				criteriaCount++;
			}

			public void SetSuccessful()
			{
				((GuiWidget)this).RemoveChild(Enumerable.Last<GuiWidget>((IEnumerable<GuiWidget>)((GuiWidget)this).get_Children()));
				ActiveErrorItem = null;
				AddSuccessIcon();
			}

			public static void ResetAll()
			{
				criteriaCount = 0;
				stillSuccessful = true;
				ActiveErrorItem = null;
			}

			private void AddSuccessIcon()
			{
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Expected O, but got Unknown
				ImageWidget val = new ImageWidget(StaticData.get_Instance().LoadImage(Path.Combine("Icons", "426.png")));
				((GuiWidget)val).set_VAnchor((VAnchor)2);
				((GuiWidget)this).AddChild((GuiWidget)val, -1);
			}
		}

		private class UsbStatus
		{
			public bool HasUsbDevice
			{
				get;
				set;
			}

			public bool IsDriverLoadable
			{
				get;
				set;
			}

			public string Summary
			{
				get;
				set;
			}

			public bool HasUsbPermission
			{
				get;
				set;
			}

			public bool AnyUsbDeviceExists
			{
				get;
				set;
			}

			public UsbDeviceDetails UsbDetails
			{
				get;
				set;
			}
		}

		private class UsbDeviceDetails
		{
			public int VendorID
			{
				get;
				set;
			}

			public int ProductID
			{
				get;
				set;
			}

			public string DriverClass
			{
				get;
				set;
			}
		}

		private Button nextButton;

		private EventHandler unregisterEvents;

		private CriteriaRow connectToPrinterRow;

		private Timer checkForPermissionTimer;

		public SetupWizardTroubleshooting()
		{
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Expected O, but got Unknown
			RefreshStatus();
			cancelButton = whiteImageButtonFactory.Generate("Cancel".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)cancelButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)WizardWindow.ChangeToPage<AndroidConnectDevicePage>);
			});
			nextButton = textImageButtonFactory.Generate("Continue".Localize());
			((GuiWidget)nextButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)((GuiWidget)WizardWindow).Close);
			});
			((GuiWidget)nextButton).set_Visible(false);
			FlowLayoutWidget footerRow = base.footerRow;
			GuiWidget val = new GuiWidget();
			val.set_HAnchor((HAnchor)5);
			((GuiWidget)footerRow).AddChild(val, -1);
			((GuiWidget)base.footerRow).AddChild((GuiWidget)(object)cancelButton, -1);
			((GuiWidget)base.footerRow).AddChild((GuiWidget)(object)nextButton, -1);
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)ConnectionStatusChanged, ref unregisterEvents);
		}

		public void ConnectionStatusChanged(object test, EventArgs args)
		{
			if (PrinterConnectionAndCommunication.Instance.CommunicationState == PrinterConnectionAndCommunication.CommunicationStates.Connected && connectToPrinterRow != null)
			{
				connectToPrinterRow.SetSuccessful();
				((GuiWidget)nextButton).set_Visible(true);
			}
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (checkForPermissionTimer != null)
			{
				checkForPermissionTimer.Dispose();
			}
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		private void RefreshStatus()
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Expected O, but got Unknown
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Expected O, but got Unknown
			CriteriaRow.ResetAll();
			((GuiWidget)contentRow).CloseAllChildren();
			TextWidget val = new TextWidget(string.Format("{0}:", "Connection Troubleshooting".Localize()), 0.0, 0.0, labelFontSize, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 10.0, 0.0, 0.0));
			connectToPrinterRow = new CriteriaRow("Connect to Printer", "Connect", "Click the 'Connect' button to retry the original connection attempt", succeeded: false, delegate
			{
				PrinterConnectionAndCommunication.Instance.ConnectToActivePrinter();
			});
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)connectToPrinterRow, -1);
			if (CriteriaRow.ActiveErrorItem != null)
			{
				FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
				((GuiWidget)val2).set_Padding(new BorderDouble(0.0, 15.0));
				FlowLayoutWidget val3 = val2;
				TextWidget val4 = new TextWidget(CriteriaRow.ActiveErrorItem.ErrorText, 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				val4.set_TextColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
				((GuiWidget)val3).AddChild((GuiWidget)val4, -1);
				((GuiWidget)contentRow).AddChild((GuiWidget)(object)val3, -1);
			}
		}
	}
}
