using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;

namespace MatterHackers.MatterControl
{
	public class WizardControl : GuiWidget
	{
		private double extraTextScaling = 1.0;

		protected TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private FlowLayoutWidget bottomToTopLayout;

		private List<WizardControlPage> pages = new List<WizardControlPage>();

		private int pageIndex;

		public Button backButton;

		public Button nextButton;

		private Button doneButton;

		private Button cancelButton;

		private TextWidget stepDescriptionWidget;

		public string StepDescription
		{
			get
			{
				return ((GuiWidget)stepDescriptionWidget).get_Text();
			}
			set
			{
				((GuiWidget)stepDescriptionWidget).set_Text(value);
			}
		}

		public WizardControl()
			: this()
		{
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Expected O, but got Unknown
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Expected O, but got Unknown
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Expected O, but got Unknown
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Expected O, but got Unknown
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Expected O, but got Unknown
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			if (UserSettings.Instance.IsTouchScreen)
			{
				extraTextScaling = 1.33333;
			}
			textImageButtonFactory.fontSize = extraTextScaling * textImageButtonFactory.fontSize;
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).AnchorAll();
			if (UserSettings.Instance.IsTouchScreen)
			{
				((GuiWidget)val).set_Padding(new BorderDouble(12.0));
			}
			else
			{
				((GuiWidget)val).set_Padding(new BorderDouble(3.0, 0.0, 3.0, 5.0));
			}
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 3.0, 0.0, 0.0));
			((GuiWidget)val2).set_Padding(new BorderDouble(0.0, 3.0, 0.0, 3.0));
			string text = LocalizedString.Get("Title Stuff".Localize());
			stepDescriptionWidget = new TextWidget(text, 0.0, 0.0, 14.0 * extraTextScaling, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			stepDescriptionWidget.set_AutoExpandBoundsToText(true);
			stepDescriptionWidget.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)stepDescriptionWidget).set_HAnchor((HAnchor)5);
			((GuiWidget)stepDescriptionWidget).set_VAnchor((VAnchor)1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)stepDescriptionWidget, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			textImageButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.disabledTextColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 100);
			textImageButtonFactory.disabledFillColor = new RGBA_Bytes(0, 0, 0, 0);
			textImageButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			((GuiWidget)this).AnchorAll();
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			bottomToTopLayout = new FlowLayoutWidget((FlowDirection)1);
			((GuiWidget)bottomToTopLayout).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((GuiWidget)bottomToTopLayout).set_Padding(new BorderDouble(3.0));
			((GuiWidget)val).AddChild((GuiWidget)(object)bottomToTopLayout, -1);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_Padding(new BorderDouble(0.0, 3.0));
			backButton = textImageButtonFactory.Generate("Back".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)backButton).add_Click((EventHandler<MouseEventArgs>)back_Click);
			nextButton = textImageButtonFactory.Generate("Next".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)nextButton).set_Name("Next Button");
			((GuiWidget)nextButton).add_Click((EventHandler<MouseEventArgs>)next_Click);
			doneButton = textImageButtonFactory.Generate("Done".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)doneButton).set_Name("Done Button");
			((GuiWidget)doneButton).add_Click((EventHandler<MouseEventArgs>)done_Click);
			cancelButton = textImageButtonFactory.Generate("Cancel".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)cancelButton).add_Click((EventHandler<MouseEventArgs>)done_Click);
			((GuiWidget)cancelButton).set_Name("Cancel Button");
			((GuiWidget)val3).AddChild((GuiWidget)(object)backButton, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)nextButton, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)cancelButton, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)doneButton, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)bottomToTopLayout).AnchorAll();
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private void done_Click(object sender, EventArgs mouseEvent)
		{
			GuiWidget val = (GuiWidget)(object)this;
			while (val != null && !(val is SystemWindow))
			{
				val = val.get_Parent();
			}
			SystemWindow val2 = val as SystemWindow;
			if (val2 != null)
			{
				((GuiWidget)val2).CloseOnIdle();
			}
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			ApplicationController.Instance.ReloadAll();
			((GuiWidget)this).OnClosed(e);
		}

		private void next_Click(object sender, EventArgs mouseEvent)
		{
			pageIndex = Math.Min(pages.Count - 1, pageIndex + 1);
			SetPageVisibility();
		}

		private void back_Click(object sender, EventArgs mouseEvent)
		{
			pageIndex = Math.Max(0, pageIndex - 1);
			SetPageVisibility();
		}

		private void SetPageVisibility()
		{
			if (pageIndex == 0)
			{
				((GuiWidget)backButton).set_Enabled(false);
				((GuiWidget)nextButton).set_Enabled(true);
				((GuiWidget)doneButton).set_Visible(false);
				((GuiWidget)cancelButton).set_Visible(true);
			}
			else if (pageIndex >= pages.Count - 1)
			{
				((GuiWidget)backButton).set_Enabled(true);
				((GuiWidget)nextButton).set_Enabled(false);
				((GuiWidget)doneButton).set_Visible(true);
				((GuiWidget)cancelButton).set_Visible(false);
			}
			else
			{
				((GuiWidget)backButton).set_Enabled(true);
				((GuiWidget)nextButton).set_Enabled(true);
				((GuiWidget)doneButton).set_Visible(false);
				((GuiWidget)cancelButton).set_Visible(true);
			}
			for (int i = 0; i < pages.Count; i++)
			{
				if (i == pageIndex)
				{
					((GuiWidget)pages[i]).set_Visible(true);
					pages[i].PageIsBecomingActive();
					StepDescription = pages[i].StepDescription;
				}
				else if (((GuiWidget)pages[i]).get_Visible())
				{
					((GuiWidget)pages[i]).set_Visible(false);
					pages[i].PageIsBecomingInactive();
				}
			}
		}

		public void AddPage(WizardControlPage widgetForPage)
		{
			pages.Add(widgetForPage);
			((GuiWidget)pages[pages.Count - 1]).set_Visible(false);
			((GuiWidget)bottomToTopLayout).AddChild((GuiWidget)(object)widgetForPage, -1);
			SetPageVisibility();
		}
	}
}
