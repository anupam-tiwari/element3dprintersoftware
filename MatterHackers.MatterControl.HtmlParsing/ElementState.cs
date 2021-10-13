using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MatterHackers.Agg;

namespace MatterHackers.MatterControl.HtmlParsing
{
	public class ElementState
	{
		public enum AlignType
		{
			none,
			center
		}

		public enum VerticalAlignType
		{
			none,
			top
		}

		internal AlignType alignment;

		internal List<string> classes = new List<string>();

		internal string href;

		internal string id;

		internal double pointSize = 12.0;

		internal Point2D sizeFixed;

		internal Point2D sizePercent;

		internal string src;

		internal string typeName;

		internal VerticalAlignType verticalAlignment;

		private const string getFirstNumber = "[0-9]+";

		private static readonly Regex getFirstNumberRegex = new Regex("[0-9]+", (RegexOptions)8);

		public AlignType Alignment => alignment;

		public List<string> Classes => classes;

		public string Href => href;

		public string Id => id;

		public double PointSize => pointSize;

		public Point2D SizeFixed => sizeFixed;

		public Point2D SizePercent => sizePercent;

		public string TypeName => typeName;

		public VerticalAlignType VerticalAlignment => verticalAlignment;

		internal ElementState()
		{
		}

		internal ElementState(ElementState copy)
		{
			alignment = copy.alignment;
			verticalAlignment = copy.verticalAlignment;
			pointSize = copy.pointSize;
			href = copy.href;
		}

		public void ParseStyleContent(string styleContent)
		{
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			string[] array = styleContent.Split(new char[1]
			{
				';'
			});
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length <= 0 || !array[i].Contains(":"))
				{
					continue;
				}
				string[] array2 = array[i].Split(new char[1]
				{
					':'
				});
				string text = array2[0].Trim();
				string text2 = array2[1];
				switch (text)
				{
				case "float":
					Console.WriteLine("Not Implemented");
					break;
				case "font-size":
					pointSize = GetFirstInt(text2);
					break;
				case "height":
					if (text2.Contains("%"))
					{
						sizePercent = new Point2D(SizePercent.x, GetFirstInt(text2));
					}
					else
					{
						sizeFixed = new Point2D(SizeFixed.x, GetFirstInt(text2));
					}
					break;
				case "width":
					if (text2.Contains("%"))
					{
						sizePercent = new Point2D(GetFirstInt(text2), SizePercent.y);
					}
					else
					{
						sizeFixed = new Point2D(GetFirstInt(text2), SizePercent.y);
					}
					break;
				case "text-align":
					alignment = (AlignType)Enum.Parse(typeof(AlignType), text2);
					break;
				case "vertical-align":
					verticalAlignment = (VerticalAlignType)Enum.Parse(typeof(VerticalAlignType), text2);
					break;
				default:
					throw new NotImplementedException();
				case "cursor":
				case "display":
				case "font-weight":
				case "margin":
				case "margin-right":
				case "margin-left":
				case "text-decoration":
				case "overflow":
				case "padding":
				case "'":
				case "color":
					break;
				}
			}
		}

		private int GetFirstInt(string input)
		{
			Match obj = getFirstNumberRegex.Match(input);
			int index = ((Capture)obj).get_Index();
			int length = ((Capture)obj).get_Length();
			return int.Parse(input.Substring(index, length));
		}
	}
}
