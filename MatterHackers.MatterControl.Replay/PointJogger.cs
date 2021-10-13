using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;

namespace MatterHackers.MatterControl.Replay
{
	public class PointJogger : GuiWidget
	{
		public PointJogger(Rewinder rewinderToUse)
			: this()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Expected O, but got Unknown
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected O, but got Unknown
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Expected O, but got Unknown
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_HAnchor((HAnchor)8);
			((GuiWidget)this).set_VAnchor((VAnchor)8);
			BorderDouble margin = default(BorderDouble);
			((BorderDouble)(ref margin))._002Ector(3.0);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)8);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			ImageBuffer val2 = new ImageBuffer();
			ImageBuffer val3 = new ImageBuffer();
			ImageBuffer val4 = new ImageBuffer();
			ImageBuffer val5 = new ImageBuffer();
			StaticData.get_Instance().LoadIcon("before_one_40x40.png", val2);
			StaticData.get_Instance().LoadIcon("before_two_40x40.png", val3);
			StaticData.get_Instance().LoadIcon("next_one_40x40.png", val4);
			StaticData.get_Instance().LoadIcon("next_two_40x40.png", val5);
			TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();
			Button val6 = textImageButtonFactory.Generate("", val3);
			((GuiWidget)val6).set_ToolTipText("Go to beginning of line, or previous one".Localize());
			((GuiWidget)val6).set_Margin(margin);
			((GuiWidget)val6).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				rewinderToUse.SkipCurrentLine(forward: false);
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)val6, -1);
			Button val7 = textImageButtonFactory.Generate("", val2);
			((GuiWidget)val7).set_ToolTipText("Step print head backwards by the step distance".Localize());
			((GuiWidget)val7).set_Margin(margin);
			((GuiWidget)val7).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				rewinderToUse.JogCurrent(forward: false);
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)val7, -1);
			Button val8 = textImageButtonFactory.Generate("", val4);
			((GuiWidget)val8).set_ToolTipText("Step print head forwards by the step distance".Localize());
			((GuiWidget)val8).set_Margin(margin);
			((GuiWidget)val8).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				rewinderToUse.JogCurrent();
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)val8, -1);
			Button val9 = textImageButtonFactory.Generate("", val5);
			((GuiWidget)val9).set_ToolTipText("Go to end of line, or next one".Localize());
			((GuiWidget)val9).set_Margin(margin);
			((GuiWidget)val9).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				rewinderToUse.SkipCurrentLine();
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)val9, -1);
		}
	}
}
