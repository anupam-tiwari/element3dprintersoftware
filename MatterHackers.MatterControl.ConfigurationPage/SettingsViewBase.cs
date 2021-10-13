using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.ConfigurationPage
{
	public class SettingsViewBase : AltGroupBox
	{
		protected readonly int TallButtonHeight = (int)(25.0 * GuiWidget.get_DeviceScale() + 0.5);

		protected TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		protected LinkButtonFactory linkButtonFactory = new LinkButtonFactory();

		protected RGBA_Bytes separatorLineColor;

		protected FlowLayoutWidget mainContainer;

		public SettingsViewBase(string title)
			: base((GuiWidget)new TextWidget(title, 0.0, 0.0, 18.0, (Justification)0, ActiveTheme.get_Instance().get_SecondaryAccentColor(), true, false, default(RGBA_Bytes), (TypeFace)null))
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Expected O, but got Unknown
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Expected O, but got Unknown
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			SetDisplayAttributes();
			mainContainer = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)mainContainer).set_HAnchor((HAnchor)5);
			((GuiWidget)mainContainer).set_Margin(new BorderDouble(6.0, 0.0, 0.0, 0.0));
		}

		private void SetDisplayAttributes()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			separatorLineColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 100);
			((GuiWidget)this).set_Margin(new BorderDouble(2.0, 4.0, 2.0, 0.0));
			textImageButtonFactory.normalFillColor = RGBA_Bytes.Transparent;
			textImageButtonFactory.normalBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);
			textImageButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_SecondaryTextColor();
			textImageButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.hoverBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);
			textImageButtonFactory.disabledFillColor = RGBA_Bytes.Transparent;
			textImageButtonFactory.disabledBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 100);
			textImageButtonFactory.disabledTextColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 100);
			textImageButtonFactory.FixedHeight = TallButtonHeight;
			textImageButtonFactory.fontSize = 11.0;
			textImageButtonFactory.borderWidth = 1.0;
			linkButtonFactory.fontSize = 11.0;
		}
	}
}
