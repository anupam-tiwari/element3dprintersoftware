using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl
{
	public class AltGroupBox : FlowLayoutWidget
	{
		private GuiWidget groupBoxLabel;

		private RGBA_Bytes borderColor = RGBA_Bytes.Black;

		private GuiWidget clientArea;

		public RGBA_Bytes TextColor
		{
			get
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				GuiWidget obj = groupBoxLabel;
				TextWidget val = obj as TextWidget;
				if (val != null)
				{
					return val.get_TextColor();
				}
				return RGBA_Bytes.White;
			}
			set
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				GuiWidget obj = groupBoxLabel;
				TextWidget val = obj as TextWidget;
				if (val != null)
				{
					val.set_TextColor(value);
				}
			}
		}

		public RGBA_Bytes BorderColor
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return borderColor;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				borderColor = value;
			}
		}

		public GuiWidget ClientArea => clientArea;

		public override string Text
		{
			get
			{
				if (groupBoxLabel != null)
				{
					return groupBoxLabel.get_Text();
				}
				return "";
			}
			set
			{
				if (groupBoxLabel != null)
				{
					groupBoxLabel.set_Text(value);
				}
			}
		}

		public AltGroupBox()
			: this("")
		{
		}

		public AltGroupBox(GuiWidget groupBoxLabel)
			: this((FlowDirection)3)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Expected O, but got Unknown
			((GuiWidget)this).set_Padding(new BorderDouble(5.0));
			((GuiWidget)this).set_Margin(new BorderDouble(0.0));
			this.groupBoxLabel = groupBoxLabel;
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_TertiaryBackgroundColor());
			if (groupBoxLabel != null)
			{
				groupBoxLabel.set_Margin(new BorderDouble(0.0));
				groupBoxLabel.set_HAnchor((HAnchor)5);
				((GuiWidget)this).AddChild(groupBoxLabel, -1);
			}
			GuiWidget val = new GuiWidget();
			val.set_HAnchor((HAnchor)5);
			val.set_VAnchor((VAnchor)8);
			clientArea = val;
			((GuiWidget)this).AddChild(clientArea, -1);
		}

		public AltGroupBox(string title)
			: this((GuiWidget)new TextWidget(title, 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null))
		{
		}//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown


		public override void AddChild(GuiWidget childToAdd, int indexInChildrenList = -1)
		{
			clientArea.AddChild(childToAdd, indexInChildrenList);
		}
	}
}
