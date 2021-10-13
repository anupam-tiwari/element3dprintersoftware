using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class SlicePresetsWindow : SystemWindow
	{
		private static Regex numberMatch = new Regex("\\s*\\(\\d+\\)", (RegexOptions)8);

		private PresetsContext presetsContext;

		private MHTextEditWidget presetNameInput;

		private string initialPresetName;

		private GuiWidget middleRow;

		public SlicePresetsWindow(PresetsContext presetsContext)
			: this(641.0, 481.0)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Expected O, but got Unknown
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Expected O, but got Unknown
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			this.presetsContext = presetsContext;
			((SystemWindow)this).set_AlwaysOnTopOfMain(true);
			((SystemWindow)this).set_Title("Slice Presets Editor".Localize());
			((GuiWidget)this).set_MinimumSize(new Vector2(640.0, 480.0));
			((GuiWidget)this).AnchorAll();
			new LinkButtonFactory
			{
				fontSize = 8.0,
				textColor = ActiveTheme.get_Instance().get_SecondaryAccentColor()
			};
			TextImageButtonFactory buttonFactory = new TextImageButtonFactory
			{
				normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor(),
				hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor(),
				disabledTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor(),
				pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor(),
				borderWidth = 0.0
			};
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_Padding(new BorderDouble(3.0));
			FlowLayoutWidget val2 = val;
			((GuiWidget)val2).AnchorAll();
			middleRow = new GuiWidget();
			middleRow.AnchorAll();
			middleRow.AddChild(CreateSliceSettingsWidget(presetsContext.PersistenceLayer), -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)GetTopRow(), -1);
			((GuiWidget)val2).AddChild(middleRow, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)GetBottomRow(buttonFactory), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
		}

		private FlowLayoutWidget GetTopRow()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Expected O, but got Unknown
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Padding(new BorderDouble(0.0, 3.0));
			TextWidget val2 = new TextWidget("Preset Name:".Localize(), 0.0, 0.0, 14.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_VAnchor((VAnchor)2);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 0.0, 4.0, 0.0));
			((GuiWidget)val).AddChild((GuiWidget)val2, -1);
			initialPresetName = presetsContext.PersistenceLayer.Name;
			MHTextEditWidget mHTextEditWidget = new MHTextEditWidget(initialPresetName);
			((GuiWidget)mHTextEditWidget).set_HAnchor((HAnchor)5);
			presetNameInput = mHTextEditWidget;
			presetNameInput.ActualTextEditWidget.add_EditComplete((EventHandler)delegate
			{
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Expected O, but got Unknown
				ActiveSliceSettings.Instance.SetValue("layer_name", ((GuiWidget)presetNameInput).get_Text(), presetsContext.PersistenceLayer);
				ActiveSliceSettings.SettingChanged.CallEvents((object)null, (EventArgs)new StringEventArgs("layer_name"));
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)presetNameInput, -1);
			return val;
		}

		private GuiWidget CreateSliceSettingsWidget(PrinterSettingsLayer persistenceLayer)
		{
			return (GuiWidget)(object)new SliceSettingsWidget(new List<PrinterSettingsLayer>
			{
				persistenceLayer,
				ActiveSliceSettings.Instance.OemLayer,
				ActiveSliceSettings.Instance.BaseLayer
			}, presetsContext.LayerType)
			{
				ShowControlBar = false
			};
		}

		private string GetNonCollidingName(string profileName, IEnumerable<string> existingNames)
		{
			if (!Enumerable.Contains<string>(existingNames, profileName))
			{
				return profileName;
			}
			int num = 1;
			string text;
			do
			{
				text = $"{profileName} ({num++})";
			}
			while (Enumerable.Contains<string>(existingNames, text));
			return text;
		}

		private FlowLayoutWidget GetBottomRow(TextImageButtonFactory buttonFactory)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 3.0));
			Button val2 = buttonFactory.Generate("Duplicate".Localize());
			((GuiWidget)val2).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					string profileName = numberMatch.Replace(((GuiWidget)presetNameInput).get_Text(), "").Trim();
					string nonCollidingName = GetNonCollidingName(profileName, Enumerable.Select<PrinterSettingsLayer, string>((IEnumerable<PrinterSettingsLayer>)presetsContext.PresetLayers, (Func<PrinterSettingsLayer, string>)((PrinterSettingsLayer preset) => preset.ValueOrDefault("layer_name"))));
					PrinterSettingsLayer printerSettingsLayer = presetsContext.PersistenceLayer.Clone();
					printerSettingsLayer.Name = nonCollidingName;
					((Collection<PrinterSettingsLayer>)(object)presetsContext.PresetLayers).Add(printerSettingsLayer);
					presetsContext.SetAsActive(printerSettingsLayer.LayerID);
					presetsContext.PersistenceLayer = printerSettingsLayer;
					middleRow.CloseAllChildren();
					middleRow.AddChild(CreateSliceSettingsWidget(printerSettingsLayer), -1);
					((GuiWidget)presetNameInput).set_Text(nonCollidingName);
				});
			});
			Button val3 = buttonFactory.Generate("Delete".Localize());
			((GuiWidget)val3).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					presetsContext.DeleteLayer();
					((GuiWidget)this).Close();
				});
			});
			Button val4 = buttonFactory.Generate("Close".Localize());
			((GuiWidget)val4).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				((GuiWidget)this).CloseOnIdle();
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val4, -1);
			return val;
		}
	}
}
