using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl.EeProm
{
	public class EePromMarlinSettings : EventArgs
	{
		public string sx = "0";

		public string sy = "0";

		public string sz = "0";

		public string se = "0";

		public string fx = "0";

		public string fy = "0";

		public string fz = "0";

		public string fe = "0";

		public string ax = "0";

		public string ay = "0";

		public string az = "0";

		public string ae = "0";

		public string acc = "0";

		public string racc = "0";

		public string avs = "0";

		public string avt = "0";

		public string avb = "0";

		public string avx = "0";

		public string avz = "0";

		public string ppid = "0";

		public string ipid = "0";

		public string dpid = "0";

		public string hox = "0";

		public string hoy = "0";

		public string hoz = "0";

		public bool hasPID;

		private bool changed;

		public string SX
		{
			get
			{
				return sx;
			}
			set
			{
				if (!sx.Equals(value))
				{
					sx = value;
					changed = true;
				}
			}
		}

		public string SY
		{
			get
			{
				return sy;
			}
			set
			{
				if (!sy.Equals(value))
				{
					sy = value;
					changed = true;
				}
			}
		}

		public string SZ
		{
			get
			{
				return sz;
			}
			set
			{
				if (!sz.Equals(value))
				{
					sz = value;
					changed = true;
				}
			}
		}

		public string SE
		{
			get
			{
				return se;
			}
			set
			{
				if (!se.Equals(value))
				{
					se = value;
					changed = true;
				}
			}
		}

		public string FX
		{
			get
			{
				return fx;
			}
			set
			{
				if (!fx.Equals(value))
				{
					fx = value;
					changed = true;
				}
			}
		}

		public string FY
		{
			get
			{
				return fy;
			}
			set
			{
				if (!fy.Equals(value))
				{
					fy = value;
					changed = true;
				}
			}
		}

		public string FZ
		{
			get
			{
				return fz;
			}
			set
			{
				if (!fz.Equals(value))
				{
					fz = value;
					changed = true;
				}
			}
		}

		public string FE
		{
			get
			{
				return fe;
			}
			set
			{
				if (!fe.Equals(value))
				{
					fe = value;
					changed = true;
				}
			}
		}

		public string AX
		{
			get
			{
				return ax;
			}
			set
			{
				if (!ax.Equals(value))
				{
					ax = value;
					changed = true;
				}
			}
		}

		public string AY
		{
			get
			{
				return ay;
			}
			set
			{
				if (!ay.Equals(value))
				{
					ay = value;
					changed = true;
				}
			}
		}

		public string AZ
		{
			get
			{
				return az;
			}
			set
			{
				if (!az.Equals(value))
				{
					az = value;
					changed = true;
				}
			}
		}

		public string AE
		{
			get
			{
				return ae;
			}
			set
			{
				if (!ae.Equals(value))
				{
					ae = value;
					changed = true;
				}
			}
		}

		public string ACC
		{
			get
			{
				return acc;
			}
			set
			{
				if (!acc.Equals(value))
				{
					acc = value;
					changed = true;
				}
			}
		}

		public string RACC
		{
			get
			{
				return racc;
			}
			set
			{
				if (!racc.Equals(value))
				{
					racc = value;
					changed = true;
				}
			}
		}

		public string AVS
		{
			get
			{
				return avs;
			}
			set
			{
				if (!avs.Equals(value))
				{
					avs = value;
					changed = true;
				}
			}
		}

		public string AVT
		{
			get
			{
				return avt;
			}
			set
			{
				if (!avt.Equals(value))
				{
					avt = value;
					changed = true;
				}
			}
		}

		public string AVB
		{
			get
			{
				return avb;
			}
			set
			{
				if (!avb.Equals(value))
				{
					avb = value;
					changed = true;
				}
			}
		}

		public string AVX
		{
			get
			{
				return avx;
			}
			set
			{
				if (!avx.Equals(value))
				{
					avx = value;
					changed = true;
				}
			}
		}

		public string AVZ
		{
			get
			{
				return avz;
			}
			set
			{
				if (!avz.Equals(value))
				{
					avz = value;
					changed = true;
				}
			}
		}

		public string PPID
		{
			get
			{
				return ppid;
			}
			set
			{
				if (!ppid.Equals(value))
				{
					ppid = value;
					changed = true;
				}
			}
		}

		public string IPID
		{
			get
			{
				return ipid;
			}
			set
			{
				if (!ipid.Equals(value))
				{
					ipid = value;
					changed = true;
				}
			}
		}

		public string DPID
		{
			get
			{
				return dpid;
			}
			set
			{
				if (!dpid.Equals(value))
				{
					dpid = value;
					changed = true;
				}
			}
		}

		public string HOX
		{
			get
			{
				return hox;
			}
			set
			{
				if (!hox.Equals(value))
				{
					hox = value;
					changed = true;
				}
			}
		}

		public string HOY
		{
			get
			{
				return hoy;
			}
			set
			{
				if (!hoy.Equals(value))
				{
					hoy = value;
					changed = true;
				}
			}
		}

		public string HOZ
		{
			get
			{
				return hoz;
			}
			set
			{
				if (!hoz.Equals(value))
				{
					hoz = value;
					changed = true;
				}
			}
		}

		public event EventHandler eventAdded;

		public bool update(string line)
		{
			bool result = false;
			string[] array = line.Split(new char[1]
			{
				' '
			});
			string a = "";
			bool flag = false;
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (string.IsNullOrWhiteSpace(text))
				{
					continue;
				}
				if (text == "M92" || a == "M92")
				{
					result = true;
					if (a != "M92")
					{
						flag = false;
					}
					a = "M92";
					if (text[0] == 'X')
					{
						sx = text.Substring(1);
					}
					if (text[0] == 'Y')
					{
						sy = text.Substring(1);
					}
					if (text[0] == 'Z')
					{
						sz = text.Substring(1);
					}
					if (text[0] == 'E' && !flag)
					{
						flag = true;
						se = text.Substring(1);
					}
				}
				if (text == "M203" || a == "M203")
				{
					result = true;
					a = "M203";
					if (text[0] == 'X')
					{
						fx = text.Substring(1);
					}
					if (text[0] == 'Y')
					{
						fy = text.Substring(1);
					}
					if (text[0] == 'Z')
					{
						fz = text.Substring(1);
					}
					if (text[0] == 'E')
					{
						fe = text.Substring(1);
					}
				}
				if (text == "M201" || a == "M201")
				{
					result = true;
					a = "M201";
					if (text[0] == 'X')
					{
						ax = text.Substring(1);
					}
					if (text[0] == 'Y')
					{
						ay = text.Substring(1);
					}
					if (text[0] == 'Z')
					{
						az = text.Substring(1);
					}
					if (text[0] == 'E')
					{
						ae = text.Substring(1);
					}
				}
				if (text == "M204" || a == "M204")
				{
					result = true;
					a = "M204";
					if (text[0] == 'S')
					{
						acc = text.Substring(1);
					}
					if (text[0] == 'T')
					{
						racc = text.Substring(1);
					}
				}
				if (text == "M205" || a == "M205")
				{
					result = true;
					a = "M205";
					if (text[0] == 'S')
					{
						avs = text.Substring(1);
					}
					if (text[0] == 'T')
					{
						avt = text.Substring(1);
					}
					if (text[0] == 'B')
					{
						avb = text.Substring(1);
					}
					if (text[0] == 'X')
					{
						avx = text.Substring(1);
					}
					if (text[0] == 'Z')
					{
						avz = text.Substring(1);
					}
				}
				if (text == "M301" || a == "M301")
				{
					result = true;
					a = "M301";
					hasPID = true;
					if (text[0] == 'P')
					{
						ppid = text.Substring(1);
					}
					if (text[0] == 'I')
					{
						ipid = text.Substring(1);
					}
					if (text[0] == 'D')
					{
						dpid = text.Substring(1);
					}
				}
				if (text == "M206" || a == "M206")
				{
					result = true;
					a = "M206";
					hasPID = true;
					if (text[0] == 'X')
					{
						hox = text.Substring(1);
					}
					if (text[0] == 'Y')
					{
						hoy = text.Substring(1);
					}
					if (text[0] == 'Z')
					{
						hoz = text.Substring(1);
					}
				}
			}
			changed = false;
			return result;
		}

		public void Save()
		{
			if (changed)
			{
				string lineToWrite = "M92 X" + sx + " Y" + sy + " Z" + sz + " E" + se;
				string lineToWrite2 = "M203 X" + fx + " Y" + fy + " Z" + fz + " E" + fe;
				string lineToWrite3 = "M201 X" + ax + " Y" + ay + " Z" + az + " E" + ae;
				string lineToWrite4 = "M204 S" + acc + " T" + racc;
				string lineToWrite5 = "M205 S" + avs + " T" + avt + " B" + avb + " X" + avx + " Z" + avz;
				string lineToWrite6 = "M206 X" + hox + " Y" + hoy + " Z" + hoz;
				string lineToWrite7 = "M301 P" + ppid + " I" + ipid + " D" + dpid;
				PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow(lineToWrite);
				PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow(lineToWrite2);
				PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow(lineToWrite3);
				PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow(lineToWrite4);
				PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow(lineToWrite5);
				PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow(lineToWrite6);
				if (hasPID)
				{
					PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow(lineToWrite7);
				}
				changed = false;
			}
		}

		internal void Import(string fileName)
		{
			string[] array = File.ReadAllLines(fileName);
			foreach (string text in array)
			{
				if (text.Contains("|"))
				{
					string[] array2 = text.Split(new char[1]
					{
						'|'
					});
					if (array2.Length == 2)
					{
						SetSetting(array2[0], array2[1]);
					}
				}
			}
			changed = true;
		}

		private void SetSetting(string keyToSet, string valueToSetTo)
		{
			valueToSetTo = valueToSetTo.Replace("\"", "").Trim();
			new List<string>();
			FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (new List<string>
				{
					fieldInfo.Name
				}.Contains(keyToSet))
				{
					_ = fieldInfo.Name;
					fieldInfo.GetValue(this);
					switch (fieldInfo.FieldType.Name)
					{
					case "Int32":
						fieldInfo.SetValue(this, (int)double.Parse(valueToSetTo));
						break;
					case "Double":
						fieldInfo.SetValue(this, double.Parse(valueToSetTo));
						break;
					case "Boolean":
						fieldInfo.SetValue(this, bool.Parse(valueToSetTo));
						break;
					case "String":
						fieldInfo.SetValue(this, valueToSetTo.Replace("\\n", "\n"));
						break;
					default:
						throw new NotImplementedException("unknown type");
					}
				}
			}
		}

		internal void Export(string fileName)
		{
			using StreamWriter streamWriter = new StreamWriter(fileName);
			FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
			foreach (FieldInfo obj in fields)
			{
				string name = obj.Name;
				object value = obj.GetValue(this);
				switch (obj.FieldType.Name)
				{
				case "Int32":
				case "Double":
				case "Boolean":
				case "FMatrix3x3":
				case "String":
					streamWriter.WriteLine(StringHelper.FormatWith("{0}|{1}", new object[2]
					{
						name,
						value
					}));
					break;
				}
			}
		}

		public void SaveToEeProm()
		{
			PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow("M500");
		}

		public void SetPrinterToFactorySettings()
		{
			hasPID = false;
			PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow("M502");
		}

		public void Add(object sender, EventArgs e)
		{
			StringEventArgs val = e as StringEventArgs;
			if (e != null && update(val.get_Data()) && this.eventAdded != null)
			{
				UiThread.RunOnIdle((Action<object>)CallEventAdded, (object)val, 0.0);
			}
		}

		private void CallEventAdded(object state)
		{
			StringEventArgs val = state as StringEventArgs;
			if (val != null)
			{
				this.eventAdded(this, (EventArgs)(object)val);
			}
		}

		public void Update()
		{
			hasPID = false;
			PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow("M503");
		}
	}
}
