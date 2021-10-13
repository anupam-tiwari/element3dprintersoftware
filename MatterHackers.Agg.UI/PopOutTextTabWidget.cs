using System;
using System.Collections.ObjectModel;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Localizations;
using MatterHackers.MatterControl;
using MatterHackers.VectorMath;

namespace MatterHackers.Agg.UI
{
	public class PopOutTextTabWidget : Tab
	{
		private PopOutManager popOutManager;

		private Button popOutButton;

		public TextWidget tabTitle;

		private FlowLayoutWidget leftToRight;

		public PopOutTextTabWidget(TabPage tabPageControledByTab, string internalTabName, Vector2 minSize)
			: this(tabPageControledByTab, internalTabName, minSize, 12.0)
		{
		}//IL_0003: Unknown result type (might be due to invalid IL or missing references)


		public PopOutTextTabWidget(TabPage tabPageControledByTab, string internalTabName, Vector2 minSize, double pointSize)
			: this(internalTabName, new GuiWidget(), new GuiWidget(), new GuiWidget(), tabPageControledByTab)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			//IL_0017: Expected O, but got Unknown
			//IL_0017: Expected O, but got Unknown
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			RGBA_Bytes primaryTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			RGBA_Bytes backgroundColor = default(RGBA_Bytes);
			RGBA_Bytes tabLabelUnselected = ActiveTheme.get_Instance().get_TabLabelUnselected();
			RGBA_Bytes backgroundColor2 = default(RGBA_Bytes);
			AddText(((GuiWidget)tabPageControledByTab).get_Text(), base.selectedWidget, primaryTextColor, backgroundColor, pointSize);
			AddText(((GuiWidget)tabPageControledByTab).get_Text(), base.normalWidget, tabLabelUnselected, backgroundColor2, pointSize);
			((GuiWidget)tabPageControledByTab).add_TextChanged((EventHandler)tabPageControledByTab_TextChanged);
			((GuiWidget)this).SetBoundsToEncloseChildren();
			popOutManager = new PopOutManager((GuiWidget)(object)((Tab)this).get_TabPage(), minSize, ((GuiWidget)tabPageControledByTab).get_Text(), internalTabName);
		}

		public void ShowInWindow()
		{
			popOutManager.ShowContentInWindow();
		}

		public override void OnMouseDown(MouseEventArgs mouseEvent)
		{
			if (!((GuiWidget)popOutButton).get_FirstWidgetUnderMouse())
			{
				((Tab)this).OnSelected((EventArgs)(object)mouseEvent);
			}
			((GuiWidget)this).OnMouseDown(mouseEvent);
		}

		private void tabPageControledByTab_TextChanged(object sender, EventArgs e)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			((Collection<GuiWidget>)(object)base.normalWidget.get_Children())[0].set_Text(((GuiWidget)sender).get_Text());
			base.normalWidget.SetBoundsToEncloseChildren();
			((Collection<GuiWidget>)(object)base.selectedWidget.get_Children())[0].set_Text(((GuiWidget)sender).get_Text());
			base.selectedWidget.SetBoundsToEncloseChildren();
			((GuiWidget)this).SetBoundsToEncloseChildren();
		}

		private void AddText(string tabText, GuiWidget widgetState, RGBA_Bytes textColor, RGBA_Bytes backgroundColor, double pointSize)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected O, but got Unknown
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Expected O, but got Unknown
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Expected O, but got Unknown
			//IL_00e0: Expected O, but got Unknown
			//IL_00e0: Expected O, but got Unknown
			//IL_00e0: Expected O, but got Unknown
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Expected O, but got Unknown
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Expected O, but got Unknown
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			leftToRight = new FlowLayoutWidget((FlowDirection)0);
			tabTitle = new TextWidget(tabText, 0.0, 0.0, pointSize, (Justification)0, textColor, true, false, default(RGBA_Bytes), (TypeFace)null);
			tabTitle.set_AutoExpandBoundsToText(true);
			((GuiWidget)leftToRight).AddChild((GuiWidget)(object)tabTitle, -1);
			ImageBuffer val = StaticData.get_Instance().LoadIcon("icon_pop_out_32x32.png", 16, 16);
			if (ActiveTheme.get_Instance().get_IsDarkTheme())
			{
				ExtensionMethods.InvertLightness(val);
			}
			ImageBuffer val2 = new ImageBuffer(val);
			byte[] buffer = val2.GetBuffer();
			for (int i = 0; i < buffer.Length; i++)
			{
				if ((i & 3) != 3)
				{
					buffer[i] = textColor.red;
				}
			}
			popOutButton = new Button(0.0, 0.0, (GuiWidget)new ButtonViewStates((GuiWidget)new ImageWidget(val2), (GuiWidget)new ImageWidget(val2), (GuiWidget)new ImageWidget(val), (GuiWidget)new ImageWidget(val)));
			((GuiWidget)popOutButton).set_ToolTipText("Pop This Tab out into its own Window".Localize());
			((GuiWidget)popOutButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				popOutManager.ShowContentInWindow();
			});
			((GuiWidget)popOutButton).set_Margin(new BorderDouble(3.0, 0.0));
			((GuiWidget)popOutButton).set_VAnchor((VAnchor)4);
			((GuiWidget)leftToRight).AddChild((GuiWidget)(object)popOutButton, -1);
			widgetState.AddChild((GuiWidget)(object)leftToRight, -1);
			widgetState.SetBoundsToEncloseChildren();
			widgetState.set_BackgroundColor(backgroundColor);
		}
	}
}
