namespace MatterHackers.MatterControl.WellPlate
{
	public interface IWell
	{
		bool Highlighted
		{
			get;
			set;
		}

		bool Selected
		{
			get;
			set;
		}

		bool PartSet
		{
			get;
		}

		void SetPart(WellPlatePart wellPlatePart);

		void ClearSelection();
	}
}
