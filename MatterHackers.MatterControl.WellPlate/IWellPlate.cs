namespace MatterHackers.MatterControl.WellPlate
{
	public interface IWellPlate : IWellPlatePerameters
	{
		bool DoHighlighting
		{
			get;
		}

		bool IsPetri
		{
			get;
			set;
		}

		void ParametersUpdated();

		bool WellPartSet(int row, int col);

		void SetWellPart(int row, int col, WellPlatePart wellPlatePart);

		void SelectAllWells();

		void DeselectAllWells();

		void HighlightCross(int row, int col, bool highlight);

		void SetWellSelectionStatus(int row, int col, bool selected);

		void SetRowSelection(int row);

		void SetColSelection(int col);
	}
}
