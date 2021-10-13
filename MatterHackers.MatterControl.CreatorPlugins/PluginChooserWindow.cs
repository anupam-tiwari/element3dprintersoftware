using System;
using System.Collections.Generic;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.CreatorPlugins
{
	public class PluginChooserWindow : SystemWindow
	{
		protected TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		protected TextImageButtonFactory unlockButtonFactory = new TextImageButtonFactory();

		private List<GuiWidget> listWithValues = new List<GuiWidget>();

		private EventHandler unregisterEvents;

		private ImageBuffer LoadImage(string imageName)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			string text = Path.Combine("Icons", imageName);
			ImageBuffer val = new ImageBuffer(10, 10);
			StaticData.get_Instance().LoadImage(text, val);
			return val;
		}

		public PluginChooserWindow()
			: this(360.0, 300.0)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			AddElements();
			((SystemWindow)this).ShowAsSystemWindow();
			((GuiWidget)this).set_MinimumSize(new Vector2(360.0, 300.0));
			AddHandlers();
		}

		protected void AddHandlers()
		{
			ActiveTheme.ThemeChanged.RegisterEvent((EventHandler)ThemeChanged, ref unregisterEvents);
		}

		public void ThemeChanged(object sender, EventArgs e)
		{
			UiThread.RunOnIdle((Action)Reload);
		}

		public void TriggerReload(object sender, EventArgs e)
		{
			UiThread.RunOnIdle((Action)Reload);
		}

		public void Reload()
		{
			((GuiWidget)this).RemoveAllChildren();
			AddElements();
		}

		public void AddElements()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Expected O, but got Unknown
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Expected O, but got Unknown
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Expected O, but got Unknown
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Expected O, but got Unknown
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Expected O, but got Unknown
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Expected O, but got Unknown
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Expected O, but got Unknown
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Expected O, but got Unknown
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_060b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Expected O, but got Unknown
			//IL_062e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0638: Unknown result type (might be due to invalid IL or missing references)
			//IL_063f: Expected O, but got Unknown
			((SystemWindow)this).set_Title("Design Add-ons".Localize());
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).AnchorAll();
			((GuiWidget)val).set_Padding(new BorderDouble(3.0, 0.0, 3.0, 5.0));
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 3.0, 0.0, 0.0));
			((GuiWidget)val2).set_Padding(new BorderDouble(0.0, 3.0, 0.0, 3.0));
			string arg = "Select a Design Tool".Localize();
			TextWidget val3 = new TextWidget(string.Format($"{arg}:"), 0.0, 0.0, 14.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val3.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_VAnchor((VAnchor)1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			GuiWidget val4 = new GuiWidget();
			val4.set_HAnchor((HAnchor)5);
			val4.set_VAnchor((VAnchor)5);
			val4.set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			FlowLayoutWidget val5 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val5).AnchorAll();
			val4.AddChild((GuiWidget)(object)val5, -1);
			unlockButtonFactory.Margin = new BorderDouble(10.0, 0.0);
			if (ActiveTheme.get_Instance().get_IsDarkTheme())
			{
				unlockButtonFactory.normalFillColor = new RGBA_Bytes(0, 0, 0, 100);
				unlockButtonFactory.normalBorderColor = new RGBA_Bytes(0, 0, 0, 100);
				unlockButtonFactory.hoverFillColor = new RGBA_Bytes(0, 0, 0, 50);
				unlockButtonFactory.hoverBorderColor = new RGBA_Bytes(0, 0, 0, 50);
			}
			else
			{
				unlockButtonFactory.normalFillColor = new RGBA_Bytes(0, 0, 0, 50);
				unlockButtonFactory.normalBorderColor = new RGBA_Bytes(0, 0, 0, 50);
				unlockButtonFactory.hoverFillColor = new RGBA_Bytes(0, 0, 0, 100);
				unlockButtonFactory.hoverBorderColor = new RGBA_Bytes(0, 0, 0, 100);
			}
			foreach (CreatorInformation creator in RegisteredCreators.Instance.Creators)
			{
				FlowLayoutWidget val6 = new FlowLayoutWidget((FlowDirection)0);
				((GuiWidget)val6).set_HAnchor((HAnchor)5);
				((GuiWidget)val6).set_BackgroundColor(RGBA_Bytes.White);
				((GuiWidget)val6).set_Padding(new BorderDouble(0.0));
				((GuiWidget)val6).set_Margin(new BorderDouble(6.0, 0.0, 6.0, 6.0));
				ClickWidget clickWidget = new ClickWidget();
				((GuiWidget)clickWidget).set_Margin(new BorderDouble(6.0, 0.0, 6.0, 0.0));
				((GuiWidget)clickWidget).set_Height(38.0);
				((GuiWidget)clickWidget).set_HAnchor((HAnchor)5);
				FlowLayoutWidget val7 = new FlowLayoutWidget((FlowDirection)0);
				((GuiWidget)val7).AnchorAll();
				((GuiWidget)val7).set_BackgroundColor(RGBA_Bytes.White);
				if (creator.iconPath != "")
				{
					ImageWidget val8 = new ImageWidget(LoadImage(creator.iconPath));
					((GuiWidget)val8).set_VAnchor((VAnchor)2);
					((GuiWidget)val7).AddChild((GuiWidget)(object)val8, -1);
				}
				bool flag = !creator.paidAddOnFlag || creator.permissionFunction();
				TextWidget val9 = new TextWidget(creator.description, 0.0, 0.0, 14.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				((GuiWidget)val9).set_Margin(new BorderDouble(10.0, 0.0, 0.0, 0.0));
				((GuiWidget)val9).set_VAnchor((VAnchor)2);
				((GuiWidget)val7).AddChild((GuiWidget)(object)val9, -1);
				if (!flag)
				{
					TextWidget val10 = new TextWidget("(" + "demo".Localize() + ")", 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
					((GuiWidget)val10).set_Margin(new BorderDouble(4.0, 0.0, 0.0, 0.0));
					((GuiWidget)val10).set_VAnchor((VAnchor)2);
					((GuiWidget)val7).AddChild((GuiWidget)(object)val10, -1);
				}
				((GuiWidget)val7).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
				CreatorInformation callCorrectFunctionHold = creator;
				clickWidget.Click += delegate
				{
					if (RegisteredCreators.Instance.Creators.Count > 0)
					{
						UiThread.RunOnIdle((Action<object>)CloseOnIdle, (object)callCorrectFunctionHold, 0.0);
					}
					else
					{
						UiThread.RunOnIdle((Action)base.CloseOnIdle);
					}
				};
				((GuiWidget)clickWidget).set_Cursor((Cursors)3);
				((GuiWidget)val7).set_Selectable(false);
				((GuiWidget)clickWidget).AddChild((GuiWidget)(object)val7, -1);
				((GuiWidget)val6).AddChild((GuiWidget)(object)clickWidget, -1);
				if (!flag)
				{
					Button val11 = unlockButtonFactory.Generate("Unlock".Localize());
					((GuiWidget)val11).set_Margin(new BorderDouble(0.0));
					((GuiWidget)val11).set_Cursor((Cursors)3);
					((GuiWidget)val11).add_Click((EventHandler<MouseEventArgs>)delegate
					{
						callCorrectFunctionHold.unlockFunction();
					});
					((GuiWidget)val6).AddChild((GuiWidget)(object)val11, -1);
				}
				((GuiWidget)val5).AddChild((GuiWidget)(object)val6, -1);
				if (callCorrectFunctionHold.unlockRegisterFunction != null)
				{
					callCorrectFunctionHold.unlockRegisterFunction(TriggerReload, ref unregisterEvents);
				}
			}
			((GuiWidget)val).AddChild(val4, -1);
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			Button val12 = textImageButtonFactory.Generate("Cancel".Localize());
			((GuiWidget)val12).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)base.CloseOnIdle);
			});
			FlowLayoutWidget val13 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val13).set_HAnchor((HAnchor)5);
			((GuiWidget)val13).set_Padding(new BorderDouble(0.0, 3.0));
			GuiWidget val14 = new GuiWidget();
			val14.set_HAnchor((HAnchor)5);
			((GuiWidget)val13).AddChild(val14, -1);
			((GuiWidget)val13).AddChild((GuiWidget)(object)val12, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val13, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private void CloseOnIdle(object state)
		{
			((GuiWidget)this).Close();
			CreatorInformation creatorInformation = state as CreatorInformation;
			if (creatorInformation != null)
			{
				UiThread.RunOnIdle(creatorInformation.Show);
			}
		}
	}
}
