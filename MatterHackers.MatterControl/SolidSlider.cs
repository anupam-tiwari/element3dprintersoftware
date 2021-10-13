using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class SolidSlider : GuiWidget
	{
		private double mouseDownOffsetFromThumbCenter;

		private bool downOnThumb;

		private double position0To1;

		private double thumbHeight;

		private int numTicks;

		private double valueOnMouseDown;

		public SolidSlideView View
		{
			get;
			set;
		}

		public double Position0To1
		{
			get
			{
				return position0To1;
			}
			set
			{
				position0To1 = Math.Max(0.0, Math.Min(value, 1.0));
			}
		}

		public double Value
		{
			get
			{
				return Minimum + (Maximum - Minimum) * Position0To1;
			}
			set
			{
				double num = Minimum;
				if (Maximum - Minimum != 0.0)
				{
					num = Math.Max(0.0, Math.Min((value - Minimum) / (Maximum - Minimum), 1.0));
				}
				if (num != Position0To1)
				{
					Position0To1 = num;
					if (this.ValueChanged != null)
					{
						this.ValueChanged(this, null);
					}
					((GuiWidget)this).Invalidate();
				}
			}
		}

		public double PositionPixelsFromFirstValue
		{
			get
			{
				return ThumbWidth / 2.0 + TrackWidth * Position0To1;
			}
			set
			{
				Position0To1 = (value - ThumbWidth / 2.0) / TrackWidth;
			}
		}

		public Orientation Orientation
		{
			get;
			set;
		}

		public double ThumbWidth
		{
			get;
			set;
		}

		public double ThumbHeight
		{
			get
			{
				return Math.Max(thumbHeight, ThumbWidth);
			}
			set
			{
				thumbHeight = value;
			}
		}

		public double TotalWidthInPixels
		{
			get;
			set;
		}

		public double TrackWidth => TotalWidthInPixels - ThumbWidth;

		public int NumTicks
		{
			get
			{
				return numTicks;
			}
			set
			{
				numTicks = value;
				if (numTicks == 1)
				{
					numTicks = 2;
				}
			}
		}

		public bool SnapToTicks
		{
			get;
			set;
		}

		public double Minimum
		{
			get;
			set;
		}

		public double Maximum
		{
			get;
			set;
		}

		public bool SmallChange
		{
			get;
			set;
		}

		public bool LargeChange
		{
			get;
			set;
		}

		public override RectangleDouble LocalBounds
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return View.GetTotalBounds();
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Invalid comparison between Unknown and I4
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				if ((int)((GuiWidget)this).get_HAnchor() == 5)
				{
					TotalWidthInPixels = value.Right - value.Left;
				}
			}
		}

		public event EventHandler ValueChanged;

		public event EventHandler SliderReleased;

		public SolidSlider(Vector2 positionOfTrackFirstValue, double thumbWidth, double minimum = 0.0, double maximum = 1.0, Orientation orientation = 0)
			: this()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			View = new SolidSlideView(this);
			View.TrackHeight = thumbWidth;
			((GuiWidget)this).set_OriginRelativeParent(positionOfTrackFirstValue);
			Orientation = orientation;
			Minimum = minimum;
			Maximum = maximum;
			ThumbWidth = thumbWidth;
			ThumbHeight = thumbWidth * 1.4;
			((GuiWidget)this).set_MinimumSize(new Vector2(((GuiWidget)this).get_Width(), ((GuiWidget)this).get_Height()));
		}

		public SolidSlider(Vector2 lowerLeft, Vector2 upperRight)
			: this(new Vector2(lowerLeft.x, lowerLeft.y + (upperRight.y - lowerLeft.y) / 2.0), upperRight.x - lowerLeft.x, 0.0, 1.0, (Orientation)0)
		{
		}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)


		public SolidSlider(double lowerLeftX, double lowerLeftY, double upperRightX, double upperRightY)
			: this(new Vector2(lowerLeftX, lowerLeftY + (upperRightY - lowerLeftY) / 2.0), upperRightX - lowerLeftX, 0.0, 1.0, (Orientation)0)
		{
		}//IL_0012: Unknown result type (might be due to invalid IL or missing references)


		public void SetRange(double minimum, double maximum)
		{
			Minimum = minimum;
			Maximum = maximum;
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			View.DoDrawBeforeChildren(graphics2D);
			((GuiWidget)this).OnDraw(graphics2D);
			View.DoDrawAfterChildren(graphics2D);
		}

		public RectangleDouble GetThumbHitBounds()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			if ((int)Orientation == 0)
			{
				return new RectangleDouble((0.0 - ThumbWidth) / 2.0 + PositionPixelsFromFirstValue, (0.0 - ThumbHeight) / 2.0, ThumbWidth / 2.0 + PositionPixelsFromFirstValue, ThumbHeight / 2.0);
			}
			return new RectangleDouble((0.0 - ThumbHeight) / 2.0, (0.0 - ThumbWidth) / 2.0 + PositionPixelsFromFirstValue, ThumbHeight / 2.0, ThumbWidth / 2.0 + PositionPixelsFromFirstValue);
		}

		public double GetPosition0To1FromValue(double value)
		{
			return (value - Minimum) / (Maximum - Minimum);
		}

		public double GetPositionPixelsFromValue(double value)
		{
			return ThumbWidth / 2.0 + TrackWidth * GetPosition0To1FromValue(value);
		}

		public RectangleDouble GetTrackHitBounds()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			if ((int)Orientation == 0)
			{
				return new RectangleDouble(0.0, (0.0 - ThumbHeight) / 2.0, TotalWidthInPixels, ThumbHeight / 2.0);
			}
			return new RectangleDouble((0.0 - ThumbHeight) / 2.0, 0.0, ThumbHeight / 2.0, TotalWidthInPixels);
		}

		public override void OnMouseDown(MouseEventArgs mouseEvent)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			valueOnMouseDown = Value;
			double value = Value;
			Vector2 position = mouseEvent.get_Position();
			RectangleDouble thumbHitBounds = GetThumbHitBounds();
			if (((RectangleDouble)(ref thumbHitBounds)).Contains(position))
			{
				if ((int)Orientation == 0)
				{
					mouseDownOffsetFromThumbCenter = position.x - PositionPixelsFromFirstValue;
				}
				else
				{
					mouseDownOffsetFromThumbCenter = position.y - PositionPixelsFromFirstValue;
				}
				downOnThumb = true;
			}
			else
			{
				RectangleDouble trackHitBounds = GetTrackHitBounds();
				if (((RectangleDouble)(ref trackHitBounds)).Contains(position))
				{
					if ((int)Orientation == 0)
					{
						PositionPixelsFromFirstValue = position.x;
					}
					else
					{
						PositionPixelsFromFirstValue = position.y;
					}
				}
			}
			if (value != Value)
			{
				this.ValueChanged?.Invoke(this, (EventArgs)(object)mouseEvent);
				((GuiWidget)this).Invalidate();
			}
			((GuiWidget)this).OnMouseDown(mouseEvent);
		}

		public override void OnMouseMove(MouseEventArgs mouseEvent)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			Vector2 position = mouseEvent.get_Position();
			if (downOnThumb)
			{
				double value = Value;
				if ((int)Orientation == 0)
				{
					PositionPixelsFromFirstValue = position.x - mouseDownOffsetFromThumbCenter;
				}
				else
				{
					PositionPixelsFromFirstValue = position.y - mouseDownOffsetFromThumbCenter;
				}
				if (value != Value)
				{
					if (this.ValueChanged != null)
					{
						this.ValueChanged(this, (EventArgs)(object)mouseEvent);
					}
					((GuiWidget)this).Invalidate();
				}
			}
			((GuiWidget)this).OnMouseMove(mouseEvent);
		}

		public override void OnMouseUp(MouseEventArgs mouseEvent)
		{
			downOnThumb = false;
			((GuiWidget)this).OnMouseUp(mouseEvent);
			if (valueOnMouseDown != Value)
			{
				this.SliderReleased?.Invoke(this, (EventArgs)(object)mouseEvent);
			}
		}
	}
}
