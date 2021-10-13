namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class ScaledSingleNumber : MapFirstValue
	{
		internal double scale;

		public override string Value
		{
			get
			{
				double num = 0.0;
				if (base.Value.Contains("%"))
				{
					string textValue = base.Value.Replace("%", "");
					num = ParseDouble(textValue) / 100.0;
				}
				else
				{
					num = ParseDouble(base.Value);
				}
				return (num * scale).ToString();
			}
		}

		internal ScaledSingleNumber(string matterControlName, string exportedName, double scale = 1.0)
			: base(matterControlName, exportedName)
		{
			this.scale = scale;
		}
	}
}
