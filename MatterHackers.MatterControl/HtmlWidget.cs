using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.ContactForm;
using MatterHackers.MatterControl.HtmlParsing;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class HtmlWidget : FlowLayoutWidget
	{
		public class WrappingTextWidget : GuiWidget
		{
			private string unwrappedMessage;

			private TextWidget messageContainer;

			public WrappingTextWidget(string text, double pointSize = 12.0, Justification justification = 0, RGBA_Bytes textColor = default(RGBA_Bytes), bool ellipsisIfClipped = true, bool underline = false, RGBA_Bytes backgroundColor = default(RGBA_Bytes))
				: this()
			{
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Expected O, but got Unknown
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				unwrappedMessage = text;
				messageContainer = new TextWidget(text, 0.0, 0.0, pointSize, justification, textColor, ellipsisIfClipped, underline, default(RGBA_Bytes), (TypeFace)null);
				((GuiWidget)this).set_BackgroundColor(backgroundColor);
				messageContainer.set_AutoExpandBoundsToText(true);
				((GuiWidget)messageContainer).set_HAnchor((HAnchor)1);
				((GuiWidget)messageContainer).set_VAnchor((VAnchor)1);
				((GuiWidget)this).set_HAnchor((HAnchor)5);
				((GuiWidget)this).set_VAnchor((VAnchor)8);
				((GuiWidget)this).AddChild((GuiWidget)(object)messageContainer, -1);
			}

			public override void OnBoundsChanged(EventArgs e)
			{
				AdjustTextWrap();
				((GuiWidget)this).OnBoundsChanged(e);
			}

			private void AdjustTextWrap()
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				if (messageContainer != null)
				{
					double width = ((GuiWidget)this).get_Width();
					BorderDouble padding = ((GuiWidget)this).get_Padding();
					double num = width - ((BorderDouble)(ref padding)).get_Width();
					if (num > 0.0)
					{
						string text = ((TextWrapping)new EnglishTextWrapping(messageContainer.get_Printer().get_TypeFaceStyle().get_EmSizeInPoints())).InsertCRs(unwrappedMessage, num);
						((GuiWidget)messageContainer).set_Text(text);
					}
				}
			}
		}

		private LinkButtonFactory linkButtonFactory = new LinkButtonFactory();

		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private Stack<GuiWidget> elementsUnderConstruction = new Stack<GuiWidget>();

		private HtmlParser htmlParser = new HtmlParser();

		private static readonly Regex replaceMultipleWhiteSpacesWithSingleWhitespaceRegex = new Regex("\\s+", (RegexOptions)8);

		public HtmlWidget(string htmlContent, RGBA_Bytes aboutTextColor)
			: this((FlowDirection)3)
		{
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_Name("HtmlWidget");
			elementsUnderConstruction.Push((GuiWidget)(object)this);
			linkButtonFactory.fontSize = 12.0;
			linkButtonFactory.textColor = aboutTextColor;
			textImageButtonFactory.normalFillColor = RGBA_Bytes.Gray;
			textImageButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			htmlParser.ParseHtml(htmlContent, AddContent, CloseContent);
			((GuiWidget)this).set_VAnchor((VAnchor)13);
			((GuiWidget)this).set_HAnchor((HAnchor)13);
		}

		private void AddContent(HtmlParser htmlParser, string htmlContent)
		{
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Expected O, but got Unknown
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Expected O, but got Unknown
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Expected O, but got Unknown
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Expected O, but got Unknown
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Expected O, but got Unknown
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Expected O, but got Unknown
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Expected O, but got Unknown
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Expected O, but got Unknown
			//IL_0543: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Expected O, but got Unknown
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0581: Expected O, but got Unknown
			//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0700: Unknown result type (might be due to invalid IL or missing references)
			//IL_0741: Unknown result type (might be due to invalid IL or missing references)
			//IL_074a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0750: Unknown result type (might be due to invalid IL or missing references)
			//IL_0753: Unknown result type (might be due to invalid IL or missing references)
			//IL_075a: Expected O, but got Unknown
			//IL_080b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0815: Expected O, but got Unknown
			//IL_0830: Unknown result type (might be due to invalid IL or missing references)
			//IL_0869: Unknown result type (might be due to invalid IL or missing references)
			//IL_086f: Unknown result type (might be due to invalid IL or missing references)
			ElementState elementState = htmlParser.CurrentElementState;
			htmlContent = replaceMultipleWhiteSpacesWithSingleWhitespaceRegex.Replace(htmlContent, " ");
			string text = HtmlParser.UrlDecode(htmlContent);
			switch (elementState.TypeName)
			{
			case "a":
				elementsUnderConstruction.Push((GuiWidget)new FlowLayoutWidget((FlowDirection)0));
				elementsUnderConstruction.Peek().set_Name("a");
				if (text != null && text != "")
				{
					Button val4 = linkButtonFactory.Generate(text.Replace("\r\n", "\n"));
					double descentInPixels = new StyledTypeFace(LiberationSansFont.get_Instance(), elementState.PointSize, false, true).get_DescentInPixels();
					((GuiWidget)val4).set_OriginRelativeParent(new Vector2(((GuiWidget)val4).get_OriginRelativeParent().x, ((GuiWidget)val4).get_OriginRelativeParent().y + descentInPixels));
					((GuiWidget)val4).add_Click((EventHandler<MouseEventArgs>)delegate
					{
						MatterControlApplication.Instance.LaunchBrowser(elementState.Href);
					});
					elementsUnderConstruction.Peek().AddChild((GuiWidget)(object)val4, -1);
				}
				break;
			case "h1":
			case "p":
				elementsUnderConstruction.Push((GuiWidget)new FlowLayoutWidget((FlowDirection)0));
				elementsUnderConstruction.Peek().set_Name("p");
				elementsUnderConstruction.Peek().set_HAnchor((HAnchor)5);
				if (text != null && text != "")
				{
					WrappingTextWidget wrappingTextWidget = new WrappingTextWidget(text, elementState.PointSize, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor());
					elementsUnderConstruction.Peek().AddChild((GuiWidget)(object)wrappingTextWidget, -1);
				}
				break;
			case "div":
				elementsUnderConstruction.Push((GuiWidget)new FlowLayoutWidget((FlowDirection)0));
				elementsUnderConstruction.Peek().set_Name("div");
				if (text != null && text != "")
				{
					TextWidget val7 = new TextWidget(text, 0.0, 0.0, elementState.PointSize, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
					elementsUnderConstruction.Peek().AddChild((GuiWidget)(object)val7, -1);
				}
				break;
			case "img":
			{
				ImageBuffer image = new ImageBuffer(Math.Max(elementState.SizeFixed.x, 1), Math.Max(elementState.SizeFixed.y, 1));
				ImageWidget val8 = new ImageWidget(image);
				((GuiWidget)val8).add_Load((EventHandler)delegate
				{
					//IL_0026: Unknown result type (might be due to invalid IL or missing references)
					ApplicationController.Instance.DownloadToImageAsync(image, elementState.src, elementState.SizeFixed.x != 0);
				});
				if (elementsUnderConstruction.Peek().get_Name() == "a")
				{
					Button val9 = new Button(0.0, 0.0, (GuiWidget)(object)val8);
					((GuiWidget)val9).set_Cursor((Cursors)3);
					((GuiWidget)val9).add_Click((EventHandler<MouseEventArgs>)delegate
					{
						MatterControlApplication.Instance.LaunchBrowser(elementState.Href);
					});
					elementsUnderConstruction.Peek().AddChild((GuiWidget)(object)val9, -1);
				}
				else
				{
					elementsUnderConstruction.Peek().AddChild((GuiWidget)(object)val8, -1);
				}
				break;
			}
			case "limg":
			{
				ImageBuffer val = new ImageBuffer();
				string text2 = Path.Combine(elementState.src.Split(new char[1]
				{
					','
				}));
				StaticData.get_Instance().LoadImage(text2, val);
				ImageWidget val2 = new ImageWidget(val);
				if (elementsUnderConstruction.Peek().get_Name() == "a")
				{
					Button val3 = new Button(0.0, 0.0, (GuiWidget)(object)val2);
					((GuiWidget)val3).set_Cursor((Cursors)3);
					((GuiWidget)val3).add_Click((EventHandler<MouseEventArgs>)delegate
					{
						MatterControlApplication.Instance.LaunchBrowser(elementState.Href);
					});
					elementsUnderConstruction.Peek().AddChild((GuiWidget)(object)val3, -1);
				}
				else
				{
					elementsUnderConstruction.Peek().AddChild((GuiWidget)(object)val2, -1);
				}
				break;
			}
			case "td":
			case "span":
			{
				if (elementState.Classes.Contains("translate"))
				{
					text = text.Localize();
				}
				if (elementState.Classes.Contains("toUpper"))
				{
					text = text.ToUpper();
				}
				if (elementState.Classes.Contains("versionNumber"))
				{
					text = VersionInfo.Instance.ReleaseVersion;
				}
				if (elementState.Classes.Contains("buildNumber"))
				{
					text = VersionInfo.Instance.BuildVersion;
				}
				Button val5 = null;
				GuiWidget val6;
				if (elementState.Classes.Contains("centeredButton"))
				{
					val5 = textImageButtonFactory.Generate(text);
					val6 = (GuiWidget)(object)val5;
				}
				else if (elementState.Classes.Contains("linkButton"))
				{
					double fontSize = linkButtonFactory.fontSize;
					linkButtonFactory.fontSize = elementState.PointSize;
					val5 = linkButtonFactory.Generate(text);
					double descentInPixels2 = new StyledTypeFace(LiberationSansFont.get_Instance(), elementState.PointSize, false, true).get_DescentInPixels();
					((GuiWidget)val5).set_OriginRelativeParent(new Vector2(((GuiWidget)val5).get_OriginRelativeParent().x, ((GuiWidget)val5).get_OriginRelativeParent().y + descentInPixels2));
					val6 = (GuiWidget)(object)val5;
					linkButtonFactory.fontSize = fontSize;
				}
				else
				{
					val6 = (GuiWidget)new TextWidget(text, 0.0, 0.0, elementState.PointSize, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
				}
				if (val5 != null)
				{
					if (elementState.Id == "sendFeedback")
					{
						((GuiWidget)val5).add_Click((EventHandler<MouseEventArgs>)delegate
						{
							ContactFormWindow.Open();
						});
					}
					else if (elementState.Id == "clearCache")
					{
						((GuiWidget)val5).add_Click((EventHandler<MouseEventArgs>)delegate
						{
							AboutWidget.DeleteCacheData(0);
						});
					}
				}
				if (elementState.VerticalAlignment == ElementState.VerticalAlignType.top)
				{
					val6.set_VAnchor((VAnchor)4);
				}
				elementsUnderConstruction.Peek().AddChild(val6, -1);
				break;
			}
			case "tr":
				elementsUnderConstruction.Push((GuiWidget)new FlowLayoutWidget((FlowDirection)0));
				elementsUnderConstruction.Peek().set_Name("tr");
				if (elementState.SizePercent.y == 100)
				{
					elementsUnderConstruction.Peek().set_VAnchor((VAnchor)5);
				}
				if (elementState.Alignment == ElementState.AlignType.center)
				{
					GuiWidget obj = elementsUnderConstruction.Peek();
					obj.set_HAnchor((HAnchor)(obj.get_HAnchor() | 2));
				}
				break;
			default:
				throw new NotImplementedException(StringHelper.FormatWith("Don't know what to do with '{0}'", new object[1]
				{
					elementState.TypeName
				}));
			case "!DOCTYPE":
				break;
			case "body":
				break;
			case "html":
				break;
			case "input":
				break;
			case "table":
				break;
			}
		}

		private void CloseContent(HtmlParser htmlParser, string htmlContent)
		{
			switch (htmlParser.CurrentElementState.TypeName)
			{
			case "a":
			{
				GuiWidget val3 = elementsUnderConstruction.Pop();
				if (val3.get_Name() != "a")
				{
					throw new Exception("Should have been 'a'.");
				}
				elementsUnderConstruction.Peek().AddChild(val3, -1);
				break;
			}
			case "h1":
			case "p":
			{
				GuiWidget val2 = elementsUnderConstruction.Pop();
				if (val2.get_Name() != "p")
				{
					throw new Exception("Should have been 'p'.");
				}
				elementsUnderConstruction.Peek().AddChild(val2, -1);
				break;
			}
			case "div":
			{
				GuiWidget val4 = elementsUnderConstruction.Pop();
				if (val4.get_Name() != "div")
				{
					throw new Exception("Should have been 'div'.");
				}
				elementsUnderConstruction.Peek().AddChild(val4, -1);
				break;
			}
			case "tr":
			{
				GuiWidget val = elementsUnderConstruction.Pop();
				if (val.get_Name() != "tr")
				{
					throw new Exception("Should have been 'tr'.");
				}
				elementsUnderConstruction.Peek().AddChild(val, -1);
				break;
			}
			default:
				throw new NotImplementedException();
			case "body":
				break;
			case "html":
				break;
			case "input":
				break;
			case "table":
				break;
			case "span":
				break;
			case "td":
				break;
			case "img":
				break;
			case "limg":
				break;
			}
		}
	}
}
