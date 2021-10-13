using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class PartPreviewWidget : GuiWidget
	{
		protected readonly int ShortButtonHeight = 25;

		protected int SideBarButtonWidth;

		public TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		protected TextImageButtonFactory checkboxButtonFactory = new TextImageButtonFactory();

		public TextImageButtonFactory ExpandMenuOptionFactory = new TextImageButtonFactory();

		public TextImageButtonFactory WhiteButtonFactory = new TextImageButtonFactory();

		protected ViewControls2D viewControls2D;

		protected GuiWidget buttonRightPanelDisabledCover;

		protected FlowLayoutWidget buttonRightPanel;

		public PartPreviewWidget()
			: this()
		{
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			if (UserSettings.Instance.IsTouchScreen)
			{
				SideBarButtonWidth = 180;
				ShortButtonHeight = 40;
			}
			else
			{
				SideBarButtonWidth = 138;
				ShortButtonHeight = 30;
			}
			textImageButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.disabledTextColor = ActiveTheme.get_Instance().get_TabLabelUnselected();
			textImageButtonFactory.disabledFillColor = default(RGBA_Bytes);
			WhiteButtonFactory.FixedWidth = SideBarButtonWidth;
			WhiteButtonFactory.FixedHeight = ShortButtonHeight;
			WhiteButtonFactory.normalFillColor = RGBA_Bytes.White;
			WhiteButtonFactory.normalTextColor = RGBA_Bytes.Black;
			WhiteButtonFactory.hoverTextColor = RGBA_Bytes.Black;
			WhiteButtonFactory.hoverFillColor = new RGBA_Bytes(255, 255, 255, 200);
			WhiteButtonFactory.borderWidth = 1.0;
			WhiteButtonFactory.normalBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);
			WhiteButtonFactory.hoverBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);
			ExpandMenuOptionFactory.FixedWidth = SideBarButtonWidth;
			ExpandMenuOptionFactory.normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			ExpandMenuOptionFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			ExpandMenuOptionFactory.disabledTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			ExpandMenuOptionFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			ExpandMenuOptionFactory.hoverFillColor = new RGBA_Bytes(255, 255, 255, 50);
			ExpandMenuOptionFactory.pressedFillColor = new RGBA_Bytes(255, 255, 255, 50);
			ExpandMenuOptionFactory.disabledFillColor = new RGBA_Bytes(255, 255, 255, 50);
			checkboxButtonFactory.fontSize = 11.0;
			checkboxButtonFactory.FixedWidth = SideBarButtonWidth;
			checkboxButtonFactory.borderWidth = 3.0;
			checkboxButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			checkboxButtonFactory.normalBorderColor = new RGBA_Bytes(0, 0, 0, 0);
			checkboxButtonFactory.normalFillColor = ActiveTheme.get_Instance().get_PrimaryBackgroundColor();
			checkboxButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			checkboxButtonFactory.hoverBorderColor = new RGBA_Bytes(0, 0, 0, 50);
			checkboxButtonFactory.hoverFillColor = new RGBA_Bytes(0, 0, 0, 50);
			checkboxButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			checkboxButtonFactory.pressedBorderColor = new RGBA_Bytes(0, 0, 0, 50);
			checkboxButtonFactory.disabledTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			((GuiWidget)this).set_BackgroundColor(RGBA_Bytes.White);
		}
	}
}
