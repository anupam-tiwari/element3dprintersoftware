using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;

namespace MatterHackers.MatterControl.Replay
{
	public class StartEndJogger : GuiWidget
	{
		public StartEndJogger(Rewinder rewinderToUse)
			: this()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Expected O, but got Unknown
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Expected O, but got Unknown
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Expected O, but got Unknown
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Expected O, but got Unknown
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Expected O, but got Unknown
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Expected O, but got Unknown
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Expected O, but got Unknown
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Expected O, but got Unknown
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Expected O, but got Unknown
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Expected O, but got Unknown
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_VAnchor((VAnchor)8);
			((GuiWidget)val).set_Padding(new BorderDouble(3.0));
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			GuiWidget val2 = new GuiWidget();
			val2.set_HAnchor((HAnchor)5);
			val2.set_VAnchor((VAnchor)8);
			((GuiWidget)val).AddChild(val2, -1);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_VAnchor((VAnchor)8);
			val2.AddChild((GuiWidget)(object)val3, -1);
			TextWidget val4 = new TextWidget("Start Point".Localize(), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val4).set_VAnchor((VAnchor)2);
			((GuiWidget)val4).set_Margin(new BorderDouble(3.0, 0.0));
			TextWidget val5 = val4;
			((GuiWidget)val3).AddChild((GuiWidget)(object)val5, -1);
			GuiWidget val6 = new GuiWidget();
			val6.set_HAnchor((HAnchor)5);
			val6.set_VAnchor((VAnchor)5);
			((GuiWidget)val3).AddChild(val6, -1);
			PointJogger pointJogger = new PointJogger(rewinderToUse);
			((GuiWidget)val3).AddChild((GuiWidget)(object)pointJogger, -1);
			GuiWidget val7 = new GuiWidget();
			val7.set_HAnchor((HAnchor)5);
			val7.set_VAnchor((VAnchor)8);
			((GuiWidget)val).AddChild(val7, -1);
			FlowLayoutWidget val8 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val8).set_HAnchor((HAnchor)5);
			((GuiWidget)val8).set_VAnchor((VAnchor)8);
			val7.AddChild((GuiWidget)(object)val8, -1);
			TextWidget val9 = new TextWidget("End Point".Localize(), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val9).set_VAnchor((VAnchor)2);
			((GuiWidget)val9).set_Margin(new BorderDouble(3.0, 0.0));
			TextWidget val10 = val9;
			((GuiWidget)val8).AddChild((GuiWidget)(object)val10, -1);
			val6 = new GuiWidget();
			val6.set_HAnchor((HAnchor)5);
			val6.set_VAnchor((VAnchor)5);
			((GuiWidget)val8).AddChild(val6, -1);
			PointJogger pointJogger2 = new PointJogger(rewinderToUse);
			((GuiWidget)val8).AddChild((GuiWidget)(object)pointJogger2, -1);
			RGBA_Bytes backgroundColor = default(RGBA_Bytes);
			((RGBA_Bytes)(ref backgroundColor))._002Ector(0, 0, 0, 100);
			GuiWidget startCover = new GuiWidget();
			startCover.AnchorAll();
			startCover.set_BackgroundColor(backgroundColor);
			startCover.set_Visible(false);
			val2.AddChild(startCover, -1);
			GuiWidget endCover = new GuiWidget();
			endCover.AnchorAll();
			endCover.set_BackgroundColor(backgroundColor);
			endCover.set_Visible(true);
			val7.AddChild(endCover, -1);
			startCover.add_Click((EventHandler<MouseEventArgs>)delegate
			{
				startCover.set_Visible(false);
				endCover.set_Visible(true);
				rewinderToUse.SetStartAsCurrent();
				rewinderToUse.MoveToCurrent();
			});
			endCover.add_Click((EventHandler<MouseEventArgs>)delegate
			{
				startCover.set_Visible(true);
				endCover.set_Visible(false);
				rewinderToUse.SetEndAsCurrent();
				rewinderToUse.MoveToCurrent();
			});
		}
	}
}
