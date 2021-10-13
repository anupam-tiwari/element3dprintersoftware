using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.WellPlate
{
	public class WellPlate2D : GuiWidget, IWellPlate, IWellPlatePerameters
	{
		private MeshViewerWidget meshViewerWidget;

		private WellPlateWidget wellPlateWidget;

		private EventHandler unregisterEvents;

		private Well2D[,] wellWidgets;

		private RectangleDouble wellPlateOutline;

		private RectangleDouble totalOutline;

		private double totalScale;

		private int lastHorizontalWellCount = -1;

		private int lastVerticalWellCount = -1;

		public WellShape WellShape
		{
			get;
			set;
		}

		public int HorizontalWellCount
		{
			get;
			set;
		}

		public int VerticalWellCount
		{
			get;
			set;
		}

		public double HorizontalWellSpacing
		{
			get;
			set;
		}

		public double VerticalWellSpacing
		{
			get;
			set;
		}

		public double WellWidth
		{
			get;
			set;
		}

		public double WellDepth
		{
			get;
			set;
		}

		public double ZToWellBottom
		{
			get;
			set;
		}

		public Vector2 WellPlateTopLeftOffset
		{
			get;
			set;
		}

		public Vector2 WellPlateTopRightOffset
		{
			get;
			set;
		}

		public bool DoHighlighting => wellPlateWidget.DoHighlighting;

		public bool IsPetri
		{
			get;
			set;
		}

		public WellPlate2D(WellPlateWidget wellPlateWidget, MeshViewerWidget meshViewer)
			: this()
		{
			this.wellPlateWidget = wellPlateWidget;
			meshViewerWidget = meshViewer;
			((GuiWidget)this).add_BoundsChanged((EventHandler)delegate
			{
				ParametersUpdated();
			});
		}

		public void ParametersUpdated()
		{
			if (HorizontalWellCount <= 0 || VerticalWellCount <= 0 || !(HorizontalWellSpacing > 0.0) || !(VerticalWellSpacing > 0.0) || !(WellWidth > 0.0) || !(((GuiWidget)this).get_Width() > 0.0) || !(((GuiWidget)this).get_Height() > 0.0))
			{
				return;
			}
			if (lastHorizontalWellCount != HorizontalWellCount || lastVerticalWellCount != VerticalWellCount || wellWidgets == null)
			{
				lastHorizontalWellCount = HorizontalWellCount;
				lastVerticalWellCount = VerticalWellCount;
				RemoveAllWells();
				wellWidgets = new Well2D[VerticalWellCount, HorizontalWellCount];
			}
			List<GuiWidget> list = new List<GuiWidget>();
			foreach (GuiWidget item in (Collection<GuiWidget>)(object)((GuiWidget)this).get_Children())
			{
				if (item is RowIndicator || item is ColIndicator)
				{
					list.Add(item);
				}
			}
			list.ForEach(new Action<GuiWidget>(((GuiWidget)this).RemoveChild));
			if (!IsPetri)
			{
				ConfigureWellPlate();
			}
			else
			{
				ConfigurePetri();
			}
			((GuiWidget)this).Invalidate();
		}

		private void ConfigureWellPlate()
		{
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			double num = (double)HorizontalWellCount * HorizontalWellSpacing - (HorizontalWellSpacing - WellWidth);
			double num2 = (double)VerticalWellCount * VerticalWellSpacing - (VerticalWellSpacing - WellWidth);
			int length = RowName.RowIndexToName(VerticalWellCount - 1).Length;
			double num3 = num + (double)(length + 1) * WellWidth;
			double num4 = num2 + 2.0 * WellWidth;
			double num5 = ((GuiWidget)this).get_Width() / num3;
			double num6 = ((GuiWidget)this).get_Height() / num4;
			totalScale = ((num5 * num4 > ((GuiWidget)this).get_Height()) ? num6 : num5);
			totalScale *= 0.9;
			double num7 = totalScale * num3;
			double num8 = totalScale * num4;
			Vector2 val = new Vector2(((GuiWidget)this).get_Width() - num7, ((GuiWidget)this).get_Height() - num8) / 2.0;
			totalOutline = default(RectangleDouble);
			totalOutline.Right = num7;
			totalOutline.Top = num8;
			((RectangleDouble)(ref totalOutline)).Offset(val);
			double num9 = totalScale * num;
			double top = totalScale * num2;
			Vector2 val2 = val + new Vector2(num7 - num9 - totalScale * WellWidth / 2.0, totalScale * WellWidth / 2.0);
			wellPlateOutline = default(RectangleDouble);
			wellPlateOutline.Right = num9;
			wellPlateOutline.Top = top;
			((RectangleDouble)(ref wellPlateOutline)).Offset(val2);
			((RectangleDouble)(ref wellPlateOutline)).Inflate(totalScale * WellWidth / 2.0);
			Vector2 val3 = default(Vector2);
			for (int i = 0; i < VerticalWellCount; i++)
			{
				double num10 = (double)(VerticalWellCount - i - 1) * VerticalWellSpacing;
				num10 *= totalScale;
				num10 += val2.y;
				for (int j = 0; j < HorizontalWellCount; j++)
				{
					double num11 = (double)j * HorizontalWellSpacing;
					num11 *= totalScale;
					num11 += val2.x;
					if (wellWidgets[i, j] != null)
					{
						((Vector2)(ref val3))._002Ector(totalScale * WellWidth, totalScale * WellWidth);
						Well2D well2D = wellWidgets[i, j];
						Vector2 maximumSize;
						((GuiWidget)wellWidgets[i, j]).set_MinimumSize(maximumSize = val3);
						((GuiWidget)well2D).set_MaximumSize(maximumSize);
						Well2D well2D2 = wellWidgets[i, j];
						double width;
						((GuiWidget)wellWidgets[i, j]).set_Height(width = totalScale * WellWidth);
						((GuiWidget)well2D2).set_Width(width);
						((GuiWidget)wellWidgets[i, j]).set_OriginRelativeParent(new Vector2(num11, num10));
						wellWidgets[i, j].WellShape = WellShape;
					}
					else
					{
						Well2D[,] array = wellWidgets;
						int num12 = i;
						int num13 = j;
						Well2D well2D3 = new Well2D(this, i, j, WellShape, totalScale * WellWidth, RGBA_Bytes.Gray);
						((GuiWidget)well2D3).set_HAnchor((HAnchor)0);
						((GuiWidget)well2D3).set_VAnchor((VAnchor)0);
						((GuiWidget)well2D3).set_OriginRelativeParent(new Vector2(num11, num10));
						array[num12, num13] = well2D3;
						((GuiWidget)this).AddChild((GuiWidget)(object)wellWidgets[i, j], -1);
					}
					if (i == 0)
					{
						ColIndicator colIndicator = new ColIndicator(this, j, totalScale * WellWidth);
						((GuiWidget)colIndicator).set_HAnchor((HAnchor)0);
						((GuiWidget)colIndicator).set_VAnchor((VAnchor)0);
						((GuiWidget)colIndicator).set_OriginRelativeParent(new Vector2(num11, num10 + 1.7 * totalScale * WellWidth));
						((GuiWidget)this).AddChild((GuiWidget)(object)colIndicator, -1);
					}
					if (j == 0)
					{
						RowIndicator rowIndicator = new RowIndicator(this, i, totalScale * WellWidth);
						((GuiWidget)rowIndicator).set_HAnchor((HAnchor)0);
						((GuiWidget)rowIndicator).set_VAnchor((VAnchor)0);
						((GuiWidget)rowIndicator).set_OriginRelativeParent(new Vector2(num11 - totalScale * WellWidth / 2.0 - (((GuiWidget)rowIndicator).get_Width() + totalScale * WellWidth) / 2.0, num10));
						((GuiWidget)this).AddChild((GuiWidget)(object)rowIndicator, -1);
					}
				}
			}
		}

		private void ConfigurePetri()
		{
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			double num = (double)HorizontalWellCount * HorizontalWellSpacing - (HorizontalWellSpacing - WellWidth);
			double num2 = (double)VerticalWellCount * VerticalWellSpacing - (VerticalWellSpacing - WellWidth);
			double num3 = ((GuiWidget)this).get_Width() / num;
			double num4 = ((GuiWidget)this).get_Height() / num2;
			totalScale = ((num3 * num2 > ((GuiWidget)this).get_Height()) ? num4 : num3);
			totalScale *= 0.9;
			double num5 = totalScale * num;
			double num6 = totalScale * num2;
			Vector2 originRelativeParent = new Vector2(((GuiWidget)this).get_Width() - num5, ((GuiWidget)this).get_Height() - num6) / 2.0;
			if (wellWidgets[0, 0] != null)
			{
				Vector2 val = default(Vector2);
				((Vector2)(ref val))._002Ector(totalScale * WellWidth, totalScale * WellWidth);
				Well2D well2D = wellWidgets[0, 0];
				Vector2 maximumSize;
				((GuiWidget)wellWidgets[0, 0]).set_MinimumSize(maximumSize = val);
				((GuiWidget)well2D).set_MaximumSize(maximumSize);
				Well2D well2D2 = wellWidgets[0, 0];
				double width;
				((GuiWidget)wellWidgets[0, 0]).set_Height(width = totalScale * WellWidth);
				((GuiWidget)well2D2).set_Width(width);
				((GuiWidget)wellWidgets[0, 0]).set_OriginRelativeParent(originRelativeParent);
			}
			else
			{
				Well2D[,] array = wellWidgets;
				Well2D well2D3 = new Well2D(this, 0, 0, WellShape.CIRCLE, totalScale * WellWidth, RGBA_Bytes.Gray);
				((GuiWidget)well2D3).set_HAnchor((HAnchor)0);
				((GuiWidget)well2D3).set_VAnchor((VAnchor)0);
				((GuiWidget)well2D3).set_OriginRelativeParent(originRelativeParent);
				array[0, 0] = well2D3;
				((GuiWidget)this).AddChild((GuiWidget)(object)wellWidgets[0, 0], -1);
			}
		}

		private void RemoveAllWells()
		{
			if (wellWidgets == null)
			{
				return;
			}
			Well2D[,] array = wellWidgets;
			foreach (Well2D well2D in array)
			{
				((GuiWidget)this).RemoveChild((GuiWidget)(object)well2D);
			}
		}

		public bool WellPartSet(int row, int col)
		{
			if (wellWidgets != null)
			{
				return wellWidgets[row, col].PartSet;
			}
			return false;
		}

		public void SetWellPart(int row, int col, WellPlatePart wellPlatePart)
		{
			if (wellWidgets != null)
			{
				wellWidgets[row, col].SetPart(wellPlatePart);
			}
		}

		public void SelectAllWells()
		{
			if (wellWidgets == null)
			{
				return;
			}
			Well2D[,] array = wellWidgets;
			int upperBound = array.GetUpperBound(0);
			int upperBound2 = array.GetUpperBound(1);
			for (int i = array.GetLowerBound(0); i <= upperBound; i++)
			{
				for (int j = array.GetLowerBound(1); j <= upperBound2; j++)
				{
					array[i, j].Selected = true;
				}
			}
		}

		public void DeselectAllWells()
		{
			if (wellWidgets == null)
			{
				return;
			}
			Well2D[,] array = wellWidgets;
			int upperBound = array.GetUpperBound(0);
			int upperBound2 = array.GetUpperBound(1);
			for (int i = array.GetLowerBound(0); i <= upperBound; i++)
			{
				for (int j = array.GetLowerBound(1); j <= upperBound2; j++)
				{
					array[i, j].ClearSelection();
				}
			}
		}

		public void HighlightCross(int row, int col, bool highlight)
		{
			if (wellWidgets != null)
			{
				for (int i = 0; i < VerticalWellCount; i++)
				{
					wellWidgets[i, col].Highlighted = highlight;
				}
				for (int j = 0; j < HorizontalWellCount; j++)
				{
					wellWidgets[row, j].Highlighted = highlight;
				}
			}
		}

		public void SetWellSelectionStatus(int row, int col, bool selected)
		{
			wellPlateWidget.SetWellSelectionStatus(row, col, selected);
		}

		public void SetRowSelection(int row)
		{
			if (wellWidgets != null && row < VerticalWellCount)
			{
				wellPlateWidget.ShouldCheckForSelectioinPairs = false;
				bool flag = true;
				for (int i = 0; i < HorizontalWellCount && flag; i++)
				{
					flag &= wellWidgets[row, i].Selected;
				}
				for (int j = 0; j < HorizontalWellCount; j++)
				{
					wellWidgets[row, j].Selected = !flag;
				}
				wellPlateWidget.ShouldCheckForSelectioinPairs = true;
			}
		}

		public void SetColSelection(int col)
		{
			if (wellWidgets != null && col < HorizontalWellCount)
			{
				wellPlateWidget.ShouldCheckForSelectioinPairs = false;
				bool flag = true;
				for (int i = 0; i < VerticalWellCount && flag; i++)
				{
					flag &= wellWidgets[i, col].Selected;
				}
				for (int j = 0; j < VerticalWellCount; j++)
				{
					wellWidgets[j, col].Selected = !flag;
				}
				wellPlateWidget.ShouldCheckForSelectioinPairs = true;
			}
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).OnDraw(graphics2D);
			if (!IsPetri)
			{
				graphics2D.Rectangle(wellPlateOutline, RGBA_Bytes.Gray, 1.0);
			}
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
		}
	}
}
