using System.Collections.Generic;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class InjectGCodeCommands : UnescapeNewlineCharacters
	{
		public InjectGCodeCommands(string canonicalSettingsName, string exportedName)
			: base(canonicalSettingsName, exportedName)
		{
		}

		protected void AddDefaultIfNotPresent(List<string> linesAdded, string commandToAdd, string[] linesToCheckIfAlreadyPresent, string comment)
		{
			string value = commandToAdd.Split(new char[1]
			{
				' '
			})[0].Trim();
			bool flag = false;
			for (int i = 0; i < linesToCheckIfAlreadyPresent.Length; i++)
			{
				if (linesToCheckIfAlreadyPresent[i].StartsWith(value))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				linesAdded.Add($"{commandToAdd} ; {comment}");
			}
		}
	}
}
