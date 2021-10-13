using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl
{
	public class PrinterOutputCache
	{
		private static PrinterOutputCache instance;

		public List<string> PrinterLines = new List<string>();

		public RootedObjectEventHandler HasChanged = new RootedObjectEventHandler();

		private int maxLinesToBuffer = 2147483646;

		private EventHandler unregisterEvents;

		public static PrinterOutputCache Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new PrinterOutputCache();
				}
				return instance;
			}
		}

		private static bool Is32Bit()
		{
			if (IntPtr.Size == 4)
			{
				return true;
			}
			return false;
		}

		private PrinterOutputCache()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			PrinterConnectionAndCommunication.Instance.ConnectionFailed.RegisterEvent((EventHandler)Instance_ConnectionFailed, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.CommunicationUnconditionalFromPrinter.RegisterEvent((EventHandler)FromPrinter, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.CommunicationUnconditionalToPrinter.RegisterEvent((EventHandler)ToPrinter, ref unregisterEvents);
			if (Is32Bit())
			{
				maxLinesToBuffer = 450000;
			}
		}

		private void OnHasChanged(EventArgs e)
		{
			HasChanged.CallEvents((object)this, e);
			if (PrinterLines.Count > maxLinesToBuffer)
			{
				Clear();
			}
		}

		private void FromPrinter(object sender, EventArgs e)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			StringEventArgs val = e as StringEventArgs;
			StringEventArgs val2 = new StringEventArgs("<-" + val.get_Data());
			PrinterLines.Add(val2.get_Data());
			OnHasChanged((EventArgs)(object)val2);
		}

		private void ToPrinter(object sender, EventArgs e)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			StringEventArgs val = e as StringEventArgs;
			StringEventArgs val2 = new StringEventArgs("->" + val.get_Data());
			PrinterLines.Add(val2.get_Data());
			OnHasChanged((EventArgs)(object)val2);
		}

		public void WriteLine(string line)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			StringEventArgs val = new StringEventArgs(line);
			PrinterLines.Add(val.get_Data());
			OnHasChanged((EventArgs)(object)val);
		}

		private void Instance_ConnectionFailed(object sender, EventArgs e)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected O, but got Unknown
			OnHasChanged(null);
			StringEventArgs val = new StringEventArgs("Lost connection to printer.");
			PrinterLines.Add(val.get_Data());
			OnHasChanged((EventArgs)(object)val);
		}

		public void Clear()
		{
			lock (PrinterLines)
			{
				PrinterLines.Clear();
			}
			OnHasChanged(null);
		}
	}
}
