using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl
{
	public class MenuOptionAction : MenuBase
	{
		private EventHandler unregisterEvents;

		public MenuOptionAction()
			: base("Actions".Localize())
		{
			((GuiWidget)this).set_Name("Actions Menu");
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)delegate
			{
				SetEnabledState();
			}, ref unregisterEvents);
		}

		public override void OnLoad(EventArgs args)
		{
			SetEnabledState();
			((GuiWidget)this).OnLoad(args);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		private void SetEnabledState()
		{
			for (int i = 0; i < ((Collection<MenuItem>)(object)((Menu)MenuDropList).MenuItems).Count; i++)
			{
				((GuiWidget)((Collection<MenuItem>)(object)((Menu)MenuDropList).MenuItems)[i]).set_Enabled(ActiveSliceSettings.Instance.PrinterSelected && PrinterConnectionAndCommunication.Instance.PrinterIsConnected && !PrinterConnectionAndCommunication.Instance.PrinterIsPrinting);
			}
		}

		protected override IEnumerable<MenuItemAction> GetMenuActions()
		{
			List<MenuItemAction> list = new List<MenuItemAction>();
			if (Enumerable.Any<GCodeMacro>(ActiveSliceSettings.Instance.ActionMacros()))
			{
				foreach (GCodeMacro item in ActiveSliceSettings.Instance.ActionMacros())
				{
					list.Add(new MenuItemAction(GCodeMacro.FixMacroName(item.Name), item.Run));
				}
				return list;
			}
			return list;
		}
	}
}
