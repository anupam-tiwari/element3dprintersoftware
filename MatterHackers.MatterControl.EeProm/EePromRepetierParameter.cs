using System;
using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl.EeProm
{
	public class EePromRepetierParameter : EventArgs
	{
		public string description = "";

		public int type;

		public int position;

		private bool changed;

		public string value
		{
			get;
			private set;
		} = "";


		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		public string Value
		{
			get
			{
				return value;
			}
			set
			{
				value = value.Replace(',', '.').Trim();
				if (!this.value.Equals(value))
				{
					this.value = value;
					MarkChanged();
				}
			}
		}

		public EePromRepetierParameter(string line)
		{
			update(line);
		}

		public void update(string line)
		{
			if (line.Length <= 4)
			{
				return;
			}
			string[] array = line.Substring(4).Split(new char[1]
			{
				' '
			});
			if (array.Length > 2)
			{
				int.TryParse(array[0], out type);
				int.TryParse(array[1], out position);
				value = array[2];
				int num = 7 + array[0].Length + array[1].Length + array[2].Length;
				if (line.Length > num)
				{
					description = line.Substring(num);
				}
				changed = false;
			}
		}

		public void Save()
		{
			if (changed)
			{
				string str = "M206 T" + type + " P" + position + " ";
				str = ((type != 3) ? (str + "S" + value) : (str + "X" + value));
				PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow(str);
				changed = false;
			}
		}

		internal void MarkChanged()
		{
			changed = true;
		}
	}
}
