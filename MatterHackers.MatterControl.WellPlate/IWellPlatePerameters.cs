using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.WellPlate
{
	public interface IWellPlatePerameters
	{
		WellShape WellShape
		{
			get;
			set;
		}

		int HorizontalWellCount
		{
			get;
			set;
		}

		int VerticalWellCount
		{
			get;
			set;
		}

		double HorizontalWellSpacing
		{
			get;
			set;
		}

		double VerticalWellSpacing
		{
			get;
			set;
		}

		double WellWidth
		{
			get;
			set;
		}

		double WellDepth
		{
			get;
			set;
		}

		double ZToWellBottom
		{
			get;
			set;
		}

		Vector2 WellPlateTopLeftOffset
		{
			get;
			set;
		}

		Vector2 WellPlateTopRightOffset
		{
			get;
			set;
		}
	}
}
