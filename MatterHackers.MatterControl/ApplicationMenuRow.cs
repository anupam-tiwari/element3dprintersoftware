using System;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl
{
	public class ApplicationMenuRow : FlowLayoutWidget
	{
		public delegate void AddRightElementDelegate(FlowLayoutWidget iconContainer);

		private FlowLayoutWidget rightElement;

		private LinkButtonFactory linkButtonFactory = new LinkButtonFactory();

		private EventHandler unregisterEvents;

		public static bool AlwaysShowUpdateStatus
		{
			get;
			set;
		}

		public static event AddRightElementDelegate AddRightElement;

		public ApplicationMenuRow()
			: this((FlowDirection)0)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Expected O, but got Unknown
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			linkButtonFactory.textColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			linkButtonFactory.fontSize = 8.0;
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)this).AddChild((GuiWidget)(object)new MenuOptionFile(), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)new MenuOptionSettings(), -1);
			PrinterSettings instance = ActiveSliceSettings.Instance;
			if (instance != null && Enumerable.Any<GCodeMacro>(instance.ActionMacros()))
			{
				((GuiWidget)this).AddChild((GuiWidget)(object)new MenuOptionAction(), -1);
			}
			((GuiWidget)this).AddChild((GuiWidget)(object)new MenuOptionHelp(), -1);
			linkButtonFactory.fontSize = 10.0;
			((GuiWidget)this).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			rightElement = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)rightElement).set_VAnchor((VAnchor)8);
			((GuiWidget)this).AddChild((GuiWidget)(object)rightElement, -1);
			((GuiWidget)this).set_Padding(new BorderDouble(0.0));
			ApplicationMenuRow.AddRightElement?.Invoke(rightElement);
			ApplicationController.Instance.PluginsLoaded.RegisterEvent((EventHandler)delegate
			{
				ApplicationMenuRow.AddRightElement?.Invoke(rightElement);
			}, ref unregisterEvents);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}
	}
}
