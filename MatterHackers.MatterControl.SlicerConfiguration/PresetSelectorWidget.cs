using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class PresetSelectorWidget : FlowLayoutWidget
	{
		private string defaultMenuItemText = "- none -".Localize();

		private Button editButton;

		private NamedSettingsLayers layerType;

		private GuiWidget pullDownContainer;

		private int extruderIndex;

		private EventHandler unregisterEvents;

		public PresetSelectorWidget(string label, RGBA_Bytes accentColor, NamedSettingsLayers layerType, int extruderIndex)
			: this((FlowDirection)3)
		{
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected O, but got Unknown
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Expected O, but got Unknown
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Expected O, but got Unknown
			((GuiWidget)this).set_Name(label);
			ActiveSliceSettings.SettingChanged.RegisterEvent((EventHandler)delegate(object s, EventArgs e)
			{
				StringEventArgs val6 = e as StringEventArgs;
				if (val6 != null && val6.get_Data() == "layer_name")
				{
					RebuildDropDownList();
				}
			}, ref unregisterEvents);
			this.extruderIndex = extruderIndex;
			this.layerType = layerType;
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			((GuiWidget)this).set_VAnchor((VAnchor)13);
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			GuiWidget val = new GuiWidget(7.0, 5.0, (SizeLimitsToSet)1);
			val.set_BackgroundColor(accentColor);
			val.set_HAnchor((HAnchor)5);
			GuiWidget val2 = val;
			TextWidget val3 = new TextWidget(label.Localize().ToUpper(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val3.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val3).set_HAnchor((HAnchor)2);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 3.0, 0.0, 6.0));
			TextWidget val4 = val3;
			((GuiWidget)this).AddChild((GuiWidget)(object)val4, -1);
			GuiWidget val5 = new GuiWidget();
			val5.set_HAnchor((HAnchor)5);
			val5.set_VAnchor((VAnchor)8);
			pullDownContainer = val5;
			pullDownContainer.AddChild((GuiWidget)(object)GetPulldownContainer(), -1);
			((GuiWidget)this).AddChild(pullDownContainer, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)new VerticalSpacer(), -1);
			((GuiWidget)this).AddChild(val2, -1);
		}

		public FlowLayoutWidget GetPulldownContainer()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Expected O, but got Unknown
			DropDownList val = CreateDropdown();
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Padding(new BorderDouble(6.0, 0.0));
			editButton = TextImageButtonFactory.GetThemedEditButton();
			((GuiWidget)editButton).set_ToolTipText("Edit Selected Setting".Localize());
			((GuiWidget)editButton).set_Enabled(val.get_SelectedIndex() != -1);
			((GuiWidget)editButton).set_VAnchor((VAnchor)2);
			((GuiWidget)editButton).set_Margin(new BorderDouble(6.0, 0.0, 0.0, 0.0));
			((GuiWidget)editButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				if (layerType == NamedSettingsLayers.Material)
				{
					if (ApplicationController.Instance.EditMaterialPresetsWindow == null)
					{
						string presetsID2 = ActiveSliceSettings.Instance.GetMaterialPresetKey(extruderIndex);
						if (string.IsNullOrEmpty(presetsID2))
						{
							return;
						}
						PrinterSettingsLayer layerToEdit2 = Enumerable.FirstOrDefault<PrinterSettingsLayer>(Enumerable.Where<PrinterSettingsLayer>((IEnumerable<PrinterSettingsLayer>)ActiveSliceSettings.Instance.MaterialLayers, (Func<PrinterSettingsLayer, bool>)((PrinterSettingsLayer layer) => layer.LayerID == presetsID2)));
						PresetsContext presetsContext = new PresetsContext(ActiveSliceSettings.Instance.MaterialLayers, layerToEdit2)
						{
							LayerType = NamedSettingsLayers.Material,
							SetAsActive = delegate(string materialKey)
							{
								ActiveSliceSettings.Instance.SetMaterialPreset(extruderIndex, materialKey);
							},
							DeleteLayer = delegate
							{
								List<string> materialSettingsKeys = ActiveSliceSettings.Instance.MaterialSettingsKeys;
								for (int i = 0; i < materialSettingsKeys.Count; i++)
								{
									if (materialSettingsKeys[i] == presetsID2)
									{
										materialSettingsKeys[i] = "";
									}
								}
								ActiveSliceSettings.Instance.SetMaterialPreset(extruderIndex, "");
								((Collection<PrinterSettingsLayer>)(object)ActiveSliceSettings.Instance.MaterialLayers).Remove(layerToEdit2);
								ActiveSliceSettings.Instance.Save();
								UiThread.RunOnIdle((Action)delegate
								{
									ApplicationController.Instance.ReloadAdvancedControlsPanel();
								});
							}
						};
						ApplicationController.Instance.EditMaterialPresetsWindow = new SlicePresetsWindow(presetsContext);
						((GuiWidget)ApplicationController.Instance.EditMaterialPresetsWindow).add_Closed((EventHandler<ClosedEventArgs>)delegate
						{
							ApplicationController.Instance.EditMaterialPresetsWindow = null;
							ApplicationController.Instance.ReloadAdvancedControlsPanel();
						});
						((SystemWindow)ApplicationController.Instance.EditMaterialPresetsWindow).ShowAsSystemWindow();
					}
					else
					{
						((GuiWidget)ApplicationController.Instance.EditMaterialPresetsWindow).BringToFront();
					}
				}
				if (layerType == NamedSettingsLayers.Quality)
				{
					if (ApplicationController.Instance.EditQualityPresetsWindow == null)
					{
						string presetsID = ActiveSliceSettings.Instance.ActiveQualityKey;
						if (!string.IsNullOrEmpty(presetsID))
						{
							PrinterSettingsLayer layerToEdit = Enumerable.FirstOrDefault<PrinterSettingsLayer>(Enumerable.Where<PrinterSettingsLayer>((IEnumerable<PrinterSettingsLayer>)ActiveSliceSettings.Instance.QualityLayers, (Func<PrinterSettingsLayer, bool>)((PrinterSettingsLayer layer) => layer.LayerID == presetsID)));
							PresetsContext presetsContext2 = new PresetsContext(ActiveSliceSettings.Instance.QualityLayers, layerToEdit)
							{
								LayerType = NamedSettingsLayers.Quality,
								SetAsActive = delegate(string qualityKey)
								{
									ActiveSliceSettings.Instance.ActiveQualityKey = qualityKey;
								},
								DeleteLayer = delegate
								{
									ActiveSliceSettings.Instance.ActiveQualityKey = "";
									((Collection<PrinterSettingsLayer>)(object)ActiveSliceSettings.Instance.QualityLayers).Remove(layerToEdit);
									ActiveSliceSettings.Instance.Save();
									UiThread.RunOnIdle((Action)delegate
									{
										ApplicationController.Instance.ReloadAdvancedControlsPanel();
									});
								}
							};
							ApplicationController.Instance.EditQualityPresetsWindow = new SlicePresetsWindow(presetsContext2);
							((GuiWidget)ApplicationController.Instance.EditQualityPresetsWindow).add_Closed((EventHandler<ClosedEventArgs>)delegate
							{
								ApplicationController.Instance.EditQualityPresetsWindow = null;
								ApplicationController.Instance.ReloadAdvancedControlsPanel();
							});
							((SystemWindow)ApplicationController.Instance.EditQualityPresetsWindow).ShowAsSystemWindow();
						}
					}
					else
					{
						((GuiWidget)ApplicationController.Instance.EditQualityPresetsWindow).BringToFront();
					}
				}
			});
			((GuiWidget)val2).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)editButton, -1);
			return val2;
		}

		private void RebuildDropDownList()
		{
			pullDownContainer.CloseAllChildren();
			pullDownContainer.AddChild((GuiWidget)(object)GetPulldownContainer(), -1);
		}

		private void MenuItem_Selected(object sender, EventArgs e)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			Dictionary<string, string> settingBeforeChange = new Dictionary<string, string>();
			Enumerator<string> enumerator = PrinterSettings.KnownSettings.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.get_Current();
					settingBeforeChange.Add(current, ActiveSliceSettings.Instance.GetValue(current));
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			PrinterSettings instance = ActiveSliceSettings.Instance;
			MenuItem val = (MenuItem)sender;
			if (layerType == NamedSettingsLayers.Material)
			{
				if (instance.GetMaterialPresetKey(extruderIndex) != val.get_Value())
				{
					instance.RestoreConflictingUserOverrides(instance.MaterialLayer);
					instance.SetMaterialPreset(extruderIndex, val.get_Value());
					instance.DeactivateConflictingUserOverrides(instance.MaterialLayer);
				}
			}
			else if (layerType == NamedSettingsLayers.Quality && instance.ActiveQualityKey != val.get_Value())
			{
				instance.RestoreConflictingUserOverrides(instance.QualityLayer);
				instance.ActiveQualityKey = val.get_Value();
				instance.DeactivateConflictingUserOverrides(instance.QualityLayer);
			}
			instance.Save();
			UiThread.RunOnIdle((Action)delegate
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				ApplicationController.Instance.ReloadAdvancedControlsPanel();
				Enumerator<string> enumerator2 = PrinterSettings.KnownSettings.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						string current2 = enumerator2.get_Current();
						if (settingBeforeChange[current2] != ActiveSliceSettings.Instance.GetValue(current2))
						{
							ActiveSliceSettings.OnSettingChanged(current2);
						}
					}
				}
				finally
				{
					((IDisposable)enumerator2).Dispose();
				}
			});
			((GuiWidget)editButton).set_Enabled(((GuiWidget)val).get_Text() != defaultMenuItemText);
		}

		private DropDownList CreateDropdown()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Expected O, but got Unknown
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Expected O, but got Unknown
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Expected O, but got Unknown
			DropDownList val = new DropDownList(defaultMenuItemText, (Direction)1, 300.0, true);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			val.set_MenuItemsPadding(new BorderDouble(10.0, 4.0, 10.0, 6.0));
			DropDownList val2 = val;
			((GuiWidget)val2).set_Name(layerType.ToString());
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 3.0));
			RectangleDouble localBounds = ((GuiWidget)val2).get_LocalBounds();
			double width = ((RectangleDouble)(ref localBounds)).get_Width();
			localBounds = ((GuiWidget)val2).get_LocalBounds();
			((GuiWidget)val2).set_MinimumSize(new Vector2(width, ((RectangleDouble)(ref localBounds)).get_Height()));
			val2.AddItem(defaultMenuItemText, "", 12.0).add_Selected((EventHandler)MenuItem_Selected);
			foreach (PrinterSettingsLayer item in (Collection<PrinterSettingsLayer>)(object)((layerType == NamedSettingsLayers.Material) ? ActiveSliceSettings.Instance.MaterialLayers : ActiveSliceSettings.Instance.QualityLayers))
			{
				MenuItem obj = val2.AddItem(item.Name, item.LayerID, 12.0);
				((GuiWidget)obj).set_Name(item.Name + " Menu");
				obj.add_Selected((EventHandler)MenuItem_Selected);
			}
			val2.AddItem(StaticData.get_Instance().LoadIcon("icon_plus.png", 32, 32), "Add New Setting".Localize() + "...", "new", 12.0).add_Selected((EventHandler)delegate
			{
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ce: Expected O, but got Unknown
				PrinterSettingsLayer printerSettingsLayer = new PrinterSettingsLayer();
				if (layerType == NamedSettingsLayers.Quality)
				{
					printerSettingsLayer.Name = "Quality" + ((Collection<PrinterSettingsLayer>)(object)ActiveSliceSettings.Instance.QualityLayers).Count;
					((Collection<PrinterSettingsLayer>)(object)ActiveSliceSettings.Instance.QualityLayers).Add(printerSettingsLayer);
					ActiveSliceSettings.Instance.ActiveQualityKey = printerSettingsLayer.LayerID;
				}
				else
				{
					printerSettingsLayer.Name = "Material" + ((Collection<PrinterSettingsLayer>)(object)ActiveSliceSettings.Instance.MaterialLayers).Count;
					((Collection<PrinterSettingsLayer>)(object)ActiveSliceSettings.Instance.MaterialLayers).Add(printerSettingsLayer);
					ActiveSliceSettings.Instance.SetMaterialPreset(extruderIndex, printerSettingsLayer.LayerID);
				}
				RebuildDropDownList();
				((ButtonBase)editButton).ClickButton(new MouseEventArgs((MouseButtons)1048576, 1, 0.0, 0.0, 0));
			});
			try
			{
				string text;
				if (layerType == NamedSettingsLayers.Material)
				{
					text = ActiveSliceSettings.Instance.GetMaterialPresetKey(extruderIndex);
					ActiveSliceSettings.Instance.MaterialLayers.add_CollectionChanged(new NotifyCollectionChangedEventHandler(SettingsLayers_CollectionChanged));
					((GuiWidget)val2).add_Closed((EventHandler<ClosedEventArgs>)delegate
					{
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						//IL_001b: Expected O, but got Unknown
						ActiveSliceSettings.Instance.MaterialLayers.remove_CollectionChanged(new NotifyCollectionChangedEventHandler(SettingsLayers_CollectionChanged));
					});
				}
				else
				{
					text = ActiveSliceSettings.Instance.ActiveQualityKey;
					ActiveSliceSettings.Instance.QualityLayers.add_CollectionChanged(new NotifyCollectionChangedEventHandler(SettingsLayers_CollectionChanged));
					((GuiWidget)val2).add_Closed((EventHandler<ClosedEventArgs>)delegate
					{
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						//IL_001b: Expected O, but got Unknown
						ActiveSliceSettings.Instance.QualityLayers.remove_CollectionChanged(new NotifyCollectionChangedEventHandler(SettingsLayers_CollectionChanged));
					});
				}
				if (!string.IsNullOrEmpty(text))
				{
					val2.set_SelectedValue(text);
					return val2;
				}
				return val2;
			}
			catch (Exception)
			{
				return val2;
			}
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((GuiWidget)this).OnClosed(e);
		}

		private void SettingsLayers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RebuildDropDownList();
		}
	}
}
