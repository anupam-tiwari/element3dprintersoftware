using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class EditableNumberDisplay : FlowLayoutWidget
	{
		protected ClickWidget clickableValueContainer;

		protected Button setButton;

		protected MHNumberEdit numberInputField;

		protected TextWidget valueDisplay;

		public event EventHandler EditComplete;

		public event EventHandler EditEnabled;

		public EditableNumberDisplay(TextImageButtonFactory textImageButtonFactory, string startingValue, string largestPossibleValue)
			: this((FlowDirection)0)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Expected O, but got Unknown
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Expected O, but got Unknown
			((GuiWidget)this).set_Margin(new BorderDouble(3.0, 0.0));
			((GuiWidget)this).set_VAnchor((VAnchor)2);
			clickableValueContainer = new ClickWidget();
			((GuiWidget)clickableValueContainer).set_VAnchor((VAnchor)5);
			((GuiWidget)clickableValueContainer).set_Cursor((Cursors)3);
			clickableValueContainer.BorderWidth = 1;
			clickableValueContainer.BorderColor = new RGBA_Bytes(255, 255, 255, 140);
			((GuiWidget)clickableValueContainer).add_MouseEnterBounds((EventHandler)delegate
			{
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				clickableValueContainer.BorderWidth = 2;
				clickableValueContainer.BorderColor = new RGBA_Bytes(255, 255, 255, 255);
			});
			((GuiWidget)clickableValueContainer).add_MouseLeaveBounds((EventHandler)delegate
			{
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				clickableValueContainer.BorderWidth = 1;
				clickableValueContainer.BorderColor = new RGBA_Bytes(255, 255, 255, 140);
			});
			valueDisplay = new TextWidget(largestPossibleValue, 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)valueDisplay).set_VAnchor((VAnchor)2);
			((GuiWidget)valueDisplay).set_HAnchor((HAnchor)1);
			valueDisplay.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)valueDisplay).set_Margin(new BorderDouble(6.0));
			clickableValueContainer.Click += editField_Click;
			((GuiWidget)clickableValueContainer).AddChild((GuiWidget)(object)valueDisplay, -1);
			((GuiWidget)clickableValueContainer).SetBoundsToEncloseChildren();
			((GuiWidget)valueDisplay).set_Text(startingValue);
			numberInputField = new MHNumberEdit(0.0, 0.0, 0.0, 12.0, 40.0, 0.0, allowNegatives: false, allowDecimals: true);
			((GuiWidget)numberInputField).set_VAnchor((VAnchor)2);
			((GuiWidget)numberInputField).set_Margin(new BorderDouble(6.0, 0.0, 0.0, 0.0));
			((GuiWidget)numberInputField).set_Visible(false);
			((GuiWidget)this).set_MinimumSize(new Vector2(0.0, ((GuiWidget)numberInputField).get_Height()));
			setButton = textImageButtonFactory.Generate("SET".Localize());
			((GuiWidget)setButton).set_VAnchor((VAnchor)2);
			((GuiWidget)setButton).set_Margin(new BorderDouble(6.0, 0.0, 0.0, 0.0));
			((GuiWidget)setButton).set_Visible(false);
			((TextEditWidget)numberInputField.ActuallNumberEdit).add_EnterPressed(new KeyEventHandler(ActuallNumberEdit_EnterPressed));
			((GuiWidget)numberInputField).add_KeyDown((EventHandler<KeyEventArgs>)delegate(object sender, KeyEventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Invalid comparison between Unknown and I4
				if ((int)e.get_KeyCode() == 27)
				{
					((GuiWidget)clickableValueContainer).set_Visible(true);
					((GuiWidget)numberInputField).set_Visible(false);
					((GuiWidget)setButton).set_Visible(false);
				}
			});
			((GuiWidget)setButton).add_Click((EventHandler<MouseEventArgs>)setButton_Click);
			((GuiWidget)this).AddChild((GuiWidget)(object)clickableValueContainer, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)numberInputField, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)setButton, -1);
		}

		private void editField_Click(object sender, EventArgs mouseEvent)
		{
			((GuiWidget)clickableValueContainer).set_Visible(false);
			((GuiWidget)numberInputField).set_Visible(true);
			string text = ((GuiWidget)valueDisplay).get_Text().Split(new char[1]
			{
				'.'
			})[0];
			if (text != null && text != "")
			{
				double.TryParse(text, out var result);
				numberInputField.ActuallNumberEdit.set_Value(result);
			}
			((GuiWidget)numberInputField.ActuallNumberEdit.get_InternalNumberEdit()).Focus();
			((InternalTextEditWidget)numberInputField.ActuallNumberEdit.get_InternalNumberEdit()).SelectAll();
			((GuiWidget)setButton).set_Visible(true);
			OnEditEnabled();
		}

		public void OnEditEnabled()
		{
			if (this.EditEnabled != null)
			{
				this.EditEnabled(this, null);
			}
		}

		public void OnEditComplete()
		{
			if (this.EditComplete != null)
			{
				this.EditComplete(this, null);
			}
		}

		private void setButton_Click(object sender, EventArgs mouseEvent)
		{
			OnEditComplete();
		}

		private void ActuallNumberEdit_EnterPressed(object sender, KeyEventArgs keyEvent)
		{
			OnEditComplete();
		}

		public void SetDisplayString(string displayString)
		{
			((GuiWidget)valueDisplay).set_Text(displayString);
			((GuiWidget)clickableValueContainer).set_Visible(true);
			((GuiWidget)numberInputField).set_Visible(false);
			((GuiWidget)setButton).set_Visible(false);
		}

		public double GetValue()
		{
			return numberInputField.ActuallNumberEdit.get_Value();
		}
	}
}
