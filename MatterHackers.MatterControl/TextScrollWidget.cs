using System;
using System.Collections.Generic;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class TextScrollWidget : GuiWidget
	{
		private object locker = new object();

		private string[] StartLineStringFilters;

		private EventHandler unregisterEvents;

		private List<string> allSourceLines;

		private List<string> visibleLines;

		private TypeFacePrinter printer;

		public RGBA_Bytes TextColor = new RGBA_Bytes(102, 102, 102);

		private int forceStartLine = -1;

		public double Position0To1
		{
			get
			{
				if (forceStartLine == -1)
				{
					return 0.0;
				}
				return ((double)visibleLines.Count - (double)forceStartLine) / (double)visibleLines.Count;
			}
			set
			{
				forceStartLine = (int)((double)visibleLines.Count * (1.0 - value)) - 1;
				forceStartLine = Math.Max(0, forceStartLine);
				forceStartLine = Math.Min(visibleLines.Count - 1, forceStartLine);
				if (forceStartLine > visibleLines.Count - NumVisibleLines)
				{
					forceStartLine = -1;
				}
				((GuiWidget)this).Invalidate();
			}
		}

		public int NumVisibleLines => (int)Math.Ceiling(((GuiWidget)this).get_Height() / printer.get_TypeFaceStyle().get_EmSizeInPixels());

		public TextScrollWidget(List<string> sourceLines)
			: this()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected O, but got Unknown
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected O, but got Unknown
			printer = new TypeFacePrinter("", new StyledTypeFace(ApplicationController.MonoSpacedTypeFace, 12.0, false, true), default(Vector2), (Justification)0, (Baseline)3);
			printer.set_DrawFromHintedCache(true);
			allSourceLines = sourceLines;
			visibleLines = sourceLines;
			PrinterOutputCache.Instance.HasChanged.RegisterEvent((EventHandler)RecievedNewLine, ref unregisterEvents);
		}

		private void ConditionalyAddToVisible(string line)
		{
			if (StartLineStringFilters == null || StartLineStringFilters.Length == 0)
			{
				return;
			}
			bool flag = true;
			string[] startLineStringFilters = StartLineStringFilters;
			foreach (string value in startLineStringFilters)
			{
				if (line == null || line.Contains("M105") || line.Length < 3 || line.StartsWith(value))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				visibleLines.Add(line);
			}
		}

		private void RecievedNewLine(object sender, EventArgs e)
		{
			StringEventArgs val = e as StringEventArgs;
			if (val != null)
			{
				ConditionalyAddToVisible(val.get_Data());
			}
			else if (StartLineStringFilters != null && StartLineStringFilters.Length != 0)
			{
				CreateFilteredList();
			}
			((GuiWidget)this).Invalidate();
		}

		private void CreateFilteredList()
		{
			lock (locker)
			{
				visibleLines = new List<string>();
				string[] array = allSourceLines.ToArray();
				foreach (string line in array)
				{
					ConditionalyAddToVisible(line);
				}
			}
		}

		public void SetLineStartFilter(string[] startLineStringsToFilter)
		{
			if (startLineStringsToFilter != null && startLineStringsToFilter.Length != 0)
			{
				StartLineStringFilters = startLineStringsToFilter;
				CreateFilteredList();
			}
			else
			{
				visibleLines = allSourceLines;
			}
		}

		public void WriteToFile(string filePath)
		{
			string[] array = allSourceLines.ToArray();
			File.WriteAllLines(filePath, array);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((GuiWidget)this).OnClosed(e);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			RectangleDouble localBounds = ((GuiWidget)this).get_LocalBounds();
			int numVisibleLines = NumVisibleLines;
			double num = ((GuiWidget)this).get_LocalBounds().Bottom + printer.get_TypeFaceStyle().get_EmSizeInPixels() * (double)numVisibleLines;
			lock (visibleLines)
			{
				lock (locker)
				{
					int num2 = visibleLines.Count - numVisibleLines;
					if (forceStartLine != -1)
					{
						num = ((GuiWidget)this).get_LocalBounds().Top;
						if (forceStartLine > visibleLines.Count - numVisibleLines)
						{
							forceStartLine = -1;
						}
						else
						{
							num2 = Math.Min(forceStartLine, num2);
						}
					}
					int count = visibleLines.Count;
					for (int i = num2; i < count; i++)
					{
						if (i >= 0 && visibleLines[i] != null)
						{
							printer.set_Text(visibleLines[i]);
							printer.set_Origin(new Vector2(localBounds.Left + 2.0, num));
							printer.Render(graphics2D, TextColor);
						}
						num -= printer.get_TypeFaceStyle().get_EmSizeInPixels();
						if (num < 0.0 - printer.get_TypeFaceStyle().get_EmSizeInPixels())
						{
							break;
						}
					}
				}
			}
			((GuiWidget)this).OnDraw(graphics2D);
		}

		public override void OnMouseWheel(MouseEventArgs mouseEvent)
		{
			((GuiWidget)this).OnMouseWheel(mouseEvent);
			double num = (double)mouseEvent.get_WheelDelta() / ((double)visibleLines.Count * 60.0);
			if (num < 0.0)
			{
				num *= 2.0;
			}
			else if (Position0To1 == 0.0)
			{
				num = (double)NumVisibleLines / (double)visibleLines.Count;
			}
			double num2 = Position0To1 + num;
			if (num2 > 1.0)
			{
				num2 = 1.0;
			}
			else if (num2 < 0.0)
			{
				num2 = 0.0;
			}
			Position0To1 = num2;
		}
	}
}
