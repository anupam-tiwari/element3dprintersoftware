using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class DoubleSolidSlider : GuiWidget
	{
		private double mouseDownOffsetFromFirstThumbCenter;

		private double mouseDownOffsetFromSecondThumbCenter;

		private bool downOnFirstThumb;

		private bool downOnSecondThumb;

		private double firstPosition0To1;

		private double secondPosition0To1;

		private double thumbHeight;

		private int numTicks;

		private double firstValueOnMouseDown;

		private double secondValueOnMouseDown;

		public DoubleSolidSlideView View
		{
			get;
			set;
		}

		public double SecondPosition0To1
		{
			get
			{
				return secondPosition0To1;
			}
			set
			{
				secondPosition0To1 = Math.Max(0.0, Math.Min(value, 1.0));
			}
		}

		public double FirstPosition0To1
		{
			get
			{
				return firstPosition0To1;
			}
			set
			{
				firstPosition0To1 = Math.Max(0.0, Math.Min(value, 1.0));
			}
		}

		public double FirstValue
		{
			get
			{
				return Minimum + (Maximum - Minimum) * FirstPosition0To1;
			}
			set
			{
				double num = Math.Max(0.0, Math.Min((value - Minimum) / (Maximum - Minimum), 1.0));
				if (num != FirstPosition0To1)
				{
					FirstPosition0To1 = num;
					if (this.FirstValueChanged != null)
					{
						this.FirstValueChanged(this, null);
					}
					((GuiWidget)this).Invalidate();
				}
			}
		}

		public double SecondValue
		{
			get
			{
				return Minimum + (Maximum - Minimum) * SecondPosition0To1;
			}
			set
			{
				double num = Math.Max(0.0, Math.Min((value - Minimum) / (Maximum - Minimum), 1.0));
				if (num != SecondPosition0To1)
				{
					SecondPosition0To1 = num;
					if (this.SecondValueChanged != null)
					{
						this.SecondValueChanged(this, null);
					}
					((GuiWidget)this).Invalidate();
				}
			}
		}

		public double PositionPixelsFromSecondValue
		{
			get
			{
				return ThumbWidth / 2.0 + TrackWidth * SecondPosition0To1;
			}
			set
			{
				SecondPosition0To1 = (value - ThumbWidth / 2.0) / TrackWidth;
			}
		}

		public double PositionPixelsFromFirstValue
		{
			get
			{
				return ThumbWidth / 2.0 + TrackWidth * FirstPosition0To1;
			}
			set
			{
				FirstPosition0To1 = (value - ThumbWidth / 2.0) / TrackWidth;
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
			}
		}

		public event EventHandler FirstValueChanged;

		public event EventHandler SecondValueChanged;

		public event EventHandler FirstSliderReleased;

		public event EventHandler SecondSliderReleased;

		public DoubleSolidSlider(Vector2 positionOfTrackFirstValue, double widthInPixels, double minimum = 0.0, double maximum = 1.0, Orientation orientation = 0)
			: this()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			View = new DoubleSolidSlideView(this);
			View.TrackHeight = widthInPixels;
			((GuiWidget)this).set_OriginRelativeParent(positionOfTrackFirstValue);
			TotalWidthInPixels = widthInPixels;
			Orientation = orientation;
			Minimum = minimum;
			Maximum = maximum;
			ThumbWidth = widthInPixels;
			ThumbHeight = widthInPixels * 1.4;
			((GuiWidget)this).set_MinimumSize(new Vector2(((GuiWidget)this).get_Width(), ((GuiWidget)this).get_Height()));
		}

		public DoubleSolidSlider(Vector2 lowerLeft, Vector2 upperRight)
			: this(new Vector2(lowerLeft.x, lowerLeft.y + (upperRight.y - lowerLeft.y) / 2.0), upperRight.x - lowerLeft.x, 0.0, 1.0, (Orientation)0)
		{
		}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)


		public DoubleSolidSlider(double lowerLeftX, double lowerLeftY, double upperRightX, double upperRightY)
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

		public RectangleDouble GetSecondThumbHitBounds()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			if ((int)Orientation == 0)
			{
				return new RectangleDouble((0.0 - ThumbWidth) / 2.0 + PositionPixelsFromSecondValue, (0.0 - ThumbHeight) / 2.0, ThumbWidth / 2.0 + PositionPixelsFromSecondValue, ThumbHeight / 2.0);
			}
			return new RectangleDouble((0.0 - ThumbHeight) / 2.0, (0.0 - ThumbWidth) / 2.0 + PositionPixelsFromSecondValue, ThumbHeight / 2.0, ThumbWidth / 2.0 + PositionPixelsFromSecondValue);
		}

		public RectangleDouble GetFirstThumbHitBounds()
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

		public double GetPosition0To1FromFirstValue(double value)
		{
			return (value - Minimum) / (Maximum - Minimum);
		}

		public double GetPositionPixelsFromFirstValue(double value)
		{
			return ThumbWidth / 2.0 + TrackWidth * GetPosition0To1FromFirstValue(value);
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
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			firstValueOnMouseDown = FirstValue;
			secondValueOnMouseDown = SecondValue;
			Vector2 position = mouseEvent.get_Position();
			RectangleDouble firstThumbHitBounds = GetFirstThumbHitBounds();
			RectangleDouble secondThumbHitBounds = GetSecondThumbHitBounds();
			if (((RectangleDouble)(ref firstThumbHitBounds)).Contains(position))
			{
				if ((int)Orientation == 0)
				{
					mouseDownOffsetFromFirstThumbCenter = position.x - PositionPixelsFromFirstValue;
				}
				else
				{
					mouseDownOffsetFromFirstThumbCenter = position.y - PositionPixelsFromFirstValue;
				}
				downOnFirstThumb = true;
			}
			else if (((RectangleDouble)(ref secondThumbHitBounds)).Contains(position))
			{
				if ((int)Orientation == 0)
				{
					mouseDownOffsetFromSecondThumbCenter = position.x - PositionPixelsFromSecondValue;
				}
				else
				{
					mouseDownOffsetFromSecondThumbCenter = position.y - PositionPixelsFromSecondValue;
				}
				downOnSecondThumb = true;
			}
			((GuiWidget)this).OnMouseDown(mouseEvent);
		}

		public override void OnMouseMove(MouseEventArgs mouseEvent)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			Vector2 position = mouseEvent.get_Position();
			if (downOnFirstThumb)
			{
				double firstValue = FirstValue;
				if ((int)Orientation == 0)
				{
					PositionPixelsFromFirstValue = Math.Min(position.x - mouseDownOffsetFromFirstThumbCenter, PositionPixelsFromSecondValue - ThumbWidth - 2.0);
				}
				else
				{
					PositionPixelsFromFirstValue = Math.Min(position.y - mouseDownOffsetFromFirstThumbCenter, PositionPixelsFromSecondValue - ThumbWidth - 2.0);
				}
				if (firstValue != FirstValue)
				{
					if (this.FirstValueChanged != null)
					{
						this.FirstValueChanged(this, (EventArgs)(object)mouseEvent);
					}
					((GuiWidget)this).Invalidate();
				}
			}
			else if (downOnSecondThumb)
			{
				double secondValue = SecondValue;
				if ((int)Orientation == 0)
				{
					PositionPixelsFromSecondValue = Math.Max(position.x - mouseDownOffsetFromSecondThumbCenter, PositionPixelsFromFirstValue + ThumbWidth + 2.0);
				}
				else
				{
					PositionPixelsFromSecondValue = Math.Max(position.y - mouseDownOffsetFromSecondThumbCenter, PositionPixelsFromFirstValue + ThumbWidth + 2.0);
				}
				if (secondValue != SecondValue)
				{
					if (this.SecondValueChanged != null)
					{
						this.SecondValueChanged(this, (EventArgs)(object)mouseEvent);
					}
					((GuiWidget)this).Invalidate();
				}
			}
			((GuiWidget)this).OnMouseMove(mouseEvent);
		}

		public override void OnMouseUp(MouseEventArgs mouseEvent)
		{
			downOnFirstThumb = false;
			downOnSecondThumb = false;
			((GuiWidget)this).OnMouseUp(mouseEvent);
			if (downOnFirstThumb)
			{
				if (firstValueOnMouseDown != FirstValue && this.FirstSliderReleased != null)
				{
					this.FirstSliderReleased(this, (EventArgs)(object)mouseEvent);
				}
			}
			else if (downOnSecondThumb && secondValueOnMouseDown != SecondValue && this.SecondSliderReleased != null)
			{
				this.SecondSliderReleased(this, (EventArgs)(object)mouseEvent);
			}
		}
	}
}
