using System;
using System.Collections.Generic;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl.EeProm
{
	public class EePromRepetierStorage
	{
		public Dictionary<int, EePromRepetierParameter> eePromSettingsList;

		public event EventHandler eventAdded;

		public EePromRepetierStorage()
		{
			eePromSettingsList = new Dictionary<int, EePromRepetierParameter>();
		}

		public void Clear()
		{
			lock (eePromSettingsList)
			{
				eePromSettingsList.Clear();
			}
		}

		public void Save()
		{
			lock (eePromSettingsList)
			{
				foreach (EePromRepetierParameter value in eePromSettingsList.Values)
				{
					value.Save();
				}
			}
		}

		public void Add(object sender, EventArgs e)
		{
			StringEventArgs val = e as StringEventArgs;
			if (e == null)
			{
				return;
			}
			string data = val.get_Data();
			if (!data.StartsWith("EPR:"))
			{
				return;
			}
			EePromRepetierParameter eePromRepetierParameter = new EePromRepetierParameter(data);
			lock (eePromSettingsList)
			{
				if (eePromSettingsList.ContainsKey(eePromRepetierParameter.position))
				{
					eePromSettingsList.Remove(eePromRepetierParameter.position);
				}
				eePromSettingsList.Add(eePromRepetierParameter.position, eePromRepetierParameter);
			}
			this.eventAdded(this, eePromRepetierParameter);
		}

		public void AskPrinterForSettings()
		{
			PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow("M205");
		}

		internal void Export(string fileName)
		{
			using StreamWriter streamWriter = new StreamWriter(fileName);
			lock (eePromSettingsList)
			{
				foreach (EePromRepetierParameter value2 in eePromSettingsList.Values)
				{
					string value = StringHelper.FormatWith("{0}|{1}", new object[2]
					{
						value2.description,
						value2.value
					});
					streamWriter.WriteLine(value);
				}
			}
		}

		internal void Import(string fileName)
		{
			string[] array = File.ReadAllLines(fileName);
			foreach (string text in array)
			{
				if (!text.Contains("|"))
				{
					continue;
				}
				string[] array2 = text.Split(new char[1]
				{
					'|'
				});
				if (array2.Length != 2)
				{
					continue;
				}
				foreach (KeyValuePair<int, EePromRepetierParameter> eePromSettings in eePromSettingsList)
				{
					if (eePromSettings.Value.Description == array2[0] && eePromSettings.Value.Value != array2[1])
					{
						eePromSettings.Value.Value = array2[1];
						eePromSettings.Value.MarkChanged();
						break;
					}
				}
			}
		}
	}
}
