using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;

namespace MatterHackers.MatterControl
{
	public class WizardPage : GuiWidget
	{
		protected FlowLayoutWidget headerRow;

		protected FlowLayoutWidget contentRow;

		protected FlowLayoutWidget footerRow;

		protected WrappedTextWidget headerLabel;

		protected Button cancelButton;

		protected TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory
		{
			fontSize = 16.0
		};

		protected TextImageButtonFactory whiteImageButtonFactory;

		protected LinkButtonFactory linkButtonFactory = new LinkButtonFactory();

		protected double labelFontSize = 12.0 * GuiWidget.get_DeviceScale();

		protected double errorFontSize = 10.0 * GuiWidget.get_DeviceScale();

		public WizardWindow WizardWindow;

		protected GuiWidget mainContainer;

		public WizardPage(string unlocalizedTextForCancelButton = "Cancel", string unlocalizedTextForTitle = "Setup Wizard")
			: this()
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Expected O, but got Unknown
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Expected O, but got Unknown
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Expected O, but got Unknown
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Expected O, but got Unknown
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Expected O, but got Unknown
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			whiteImageButtonFactory = new TextImageButtonFactory
			{
				normalFillColor = RGBA_Bytes.White,
				disabledFillColor = RGBA_Bytes.White,
				fontSize = 16.0,
				borderWidth = 1.0,
				normalBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200),
				hoverBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200),
				disabledTextColor = RGBA_Bytes.DarkGray,
				hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor(),
				normalTextColor = RGBA_Bytes.Black,
				pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor(),
				FixedWidth = 200.0
			};
			if (!UserSettings.Instance.IsTouchScreen)
			{
				textImageButtonFactory = new TextImageButtonFactory
				{
					normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor(),
					hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor(),
					disabledTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor(),
					pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor(),
					borderWidth = 0.0
				};
				linkButtonFactory.textColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
				linkButtonFactory.fontSize = 10.0;
				((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
				((GuiWidget)this).set_Padding(new BorderDouble(0.0));
			}
			((GuiWidget)this).AnchorAll();
			cancelButton = textImageButtonFactory.Generate(unlocalizedTextForCancelButton.Localize());
			((GuiWidget)cancelButton).set_Name("Cancel Wizard Button");
			((GuiWidget)cancelButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					WizardWindow wizardWindow = WizardWindow;
					if (wizardWindow != null)
					{
						((GuiWidget)wizardWindow).Close();
					}
				});
			});
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_Padding(new BorderDouble(12.0, 12.0, 12.0, 0.0));
			((GuiWidget)val).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			mainContainer = (GuiWidget)val;
			mainContainer.AnchorAll();
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 3.0, 0.0, 0.0));
			((GuiWidget)val2).set_Padding(new BorderDouble(0.0, 12.0));
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			headerRow = val2;
			WrappedTextWidget val3 = new WrappedTextWidget(unlocalizedTextForTitle.Localize(), 24.0, (Justification)0, ActiveTheme.get_Instance().get_SecondaryAccentColor(), true, false, default(RGBA_Bytes), true);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			headerLabel = val3;
			((GuiWidget)headerRow).AddChild((GuiWidget)(object)headerLabel, -1);
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val4).set_Padding(new BorderDouble(10.0));
			((GuiWidget)val4).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((GuiWidget)val4).set_HAnchor((HAnchor)5);
			((GuiWidget)val4).set_VAnchor((VAnchor)5);
			contentRow = val4;
			FlowLayoutWidget val5 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val5).set_HAnchor((HAnchor)5);
			((GuiWidget)val5).set_Margin(new BorderDouble(0.0, 6.0));
			footerRow = val5;
			mainContainer.AddChild((GuiWidget)(object)headerRow, -1);
			mainContainer.AddChild((GuiWidget)(object)contentRow, -1);
			mainContainer.AddChild((GuiWidget)(object)footerRow, -1);
			if (!UserSettings.Instance.IsTouchScreen)
			{
				mainContainer.set_Padding(new BorderDouble(3.0, 5.0, 3.0, 5.0));
				((GuiWidget)headerRow).set_Padding(new BorderDouble(0.0, 3.0, 0.0, 3.0));
				headerLabel.get_TextWidget().set_PointSize(14.0);
				headerLabel.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
				((GuiWidget)contentRow).set_Padding(new BorderDouble(5.0));
				((GuiWidget)footerRow).set_Margin(new BorderDouble(0.0, 3.0));
			}
			((GuiWidget)this).AddChild(mainContainer, -1);
		}

		public virtual void PageIsBecomingActive()
		{
		}

		public virtual void PageIsBecomingInactive()
		{
		}
	}
}
