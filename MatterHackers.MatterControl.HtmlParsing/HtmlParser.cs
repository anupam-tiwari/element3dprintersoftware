using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MatterHackers.MatterControl.HtmlParsing
{
	public class HtmlParser
	{
		private const string typeNameEnd = "[ >]";

		private static readonly Regex typeNameEndRegex = new Regex("[ >]", (RegexOptions)8);

		private static List<string> voidElements = new List<string>
		{
			"area",
			"base",
			"br",
			"col",
			"command",
			"embed",
			"hr",
			"img",
			"input",
			"keygen",
			"link",
			"meta",
			"param",
			"source",
			"track",
			"wbr"
		};

		private Stack<ElementState> elementQueue = new Stack<ElementState>();

		public ElementState CurrentElementState => elementQueue.Peek();

		public static string UrlDecode(string htmlContent)
		{
			return htmlContent.Replace("&trade;", "™").Replace("&nbsp;", " ").Replace("&copy;", "©");
		}

		public void ParseHtml(string htmlContent, Action<HtmlParser, string> addContentFunction, Action<HtmlParser, string> closeContentFunction)
		{
			elementQueue.Push(new ElementState());
			int num = 0;
			while (num < htmlContent.Length)
			{
				int num2 = htmlContent.IndexOf('<', num);
				if (num2 == -1)
				{
					break;
				}
				int num3 = htmlContent.IndexOf('>', num2);
				if (htmlContent[num2 + 1] == '/')
				{
					closeContentFunction(this, null);
					elementQueue.Pop();
					int num4 = htmlContent.IndexOf('<', num3);
					if (num4 > num3 + 1)
					{
						string arg = htmlContent.Substring(num3 + 1, num4 - (num3 + 1));
						addContentFunction(this, arg);
					}
				}
				else
				{
					ParseTypeContent(num2, num3, htmlContent);
					int index = ((Capture)typeNameEndRegex.Match(htmlContent, num2 + 1)).get_Index();
					elementQueue.Peek().typeName = htmlContent.Substring(num2 + 1, index - (num2 + 1));
					int num5 = htmlContent.IndexOf('<', num3);
					if (num3 + 1 < htmlContent.Length && num5 != -1)
					{
						string arg2 = htmlContent.Substring(num3 + 1, num5 - num3 - 1);
						addContentFunction(this, arg2);
					}
					if (voidElements.Contains(elementQueue.Peek().typeName))
					{
						closeContentFunction(this, null);
						elementQueue.Pop();
						int num6 = htmlContent.IndexOf('<', num3);
						if (num6 > num3 + 1)
						{
							string arg3 = htmlContent.Substring(num3 + 1, num6 - (num3 + 1));
							addContentFunction(this, arg3);
						}
					}
				}
				num = num3 + 1;
			}
		}

		private static int SetEndAtCharacter(string content, int currentEndIndex, char characterToSkipTo)
		{
			int num = content.IndexOf(characterToSkipTo, currentEndIndex);
			currentEndIndex = ((num != -1) ? Math.Min(num + 1, content.Length - 1) : (content.Length - 1));
			return currentEndIndex;
		}

		private static string[] SplitOnSpacesNotInQuotes(string content)
		{
			List<string> list = new List<string>();
			int num = 0;
			int startIndex = 0;
			int num2 = content.IndexOf(' ', startIndex);
			bool flag = num2 != -1;
			while (flag)
			{
				int num3 = content.IndexOf('\'', startIndex);
				int num4 = content.IndexOf('"', startIndex);
				if ((num3 != -1 && num3 < num2) || (num4 != -1 && num4 < num2))
				{
					startIndex = ((num4 != -1 && (num3 == -1 || num3 >= num4)) ? SetEndAtCharacter(content, num4 + 1, '"') : SetEndAtCharacter(content, num3 + 1, '\''));
				}
				else
				{
					list.Add(content.Substring(num, num2 - num));
					num = num2 + 1;
					startIndex = num;
				}
				num2 = content.IndexOf(' ', startIndex);
				flag = num2 != -1;
			}
			if (num < content.Length)
			{
				list.Add(content.Substring(num, content.Length - num));
			}
			return list.ToArray();
		}

		private void ParseTypeContent(int openPosition, int closePosition, string htmlContent)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			htmlContent.Substring(openPosition, closePosition - openPosition);
			ElementState elementState = new ElementState(elementQueue.Peek());
			int index = ((Capture)typeNameEndRegex.Match(htmlContent, openPosition)).get_Index();
			if (index < closePosition)
			{
				string[] array = SplitOnSpacesNotInQuotes(htmlContent.Substring(index, closePosition - index).Trim());
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = new Regex("=").Split(array[i]);
					string text = array2[0];
					string text2 = "";
					if (array2.Length > 1)
					{
						text2 = RemoveOuterQuotes(array2[1]);
					}
					switch (text)
					{
					case "style":
						elementState.ParseStyleContent(text2);
						break;
					case "class":
					{
						string[] array3 = text2.Split(new char[1]
						{
							' '
						});
						foreach (string item in array3)
						{
							elementState.classes.Add(item);
						}
						break;
					}
					case "href":
						elementState.href = text2;
						break;
					case "src":
						elementState.src = text2;
						break;
					case "id":
						elementState.id = text2;
						break;
					}
				}
			}
			elementQueue.Push(elementState);
		}

		private string RemoveOuterQuotes(string inputString)
		{
			int num = inputString.IndexOf('\'');
			int num2 = inputString.IndexOf('"');
			if (num2 == -1 || (num != -1 && num < num2))
			{
				int num3 = inputString.LastIndexOf('\'');
				if (num3 != -1 && num3 != num)
				{
					return inputString.Substring(num + 1, num3 - num - 1);
				}
				return inputString.Substring(num + 1);
			}
			if (num2 != -1)
			{
				int num4 = inputString.LastIndexOf('"');
				if (num4 != -1 && num4 != num2)
				{
					return inputString.Substring(num2 + 1, num4 - num2 - 1);
				}
				return inputString.Substring(num2 + 1);
			}
			return inputString;
		}
	}
}
