using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.FrostedSerial;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class SliceSettingsWidget : GuiWidget
	{
		internal class ExtraSettingTextWidget : MHTextEditWidget
		{
			internal string itemKey
			{
				get;
				set;
			}

			internal ExtraSettingTextWidget(string itemKey, string itemValue)
				: base(itemValue)
			{
				this.itemKey = itemKey;
			}
		}

		private class SettingsRow : FlowLayoutWidget
		{
			private EventHandler unregisterEvents;

			public string SettingsKey
			{
				get;
				set;
			}

			public string SettingsValue
			{
				get;
				set;
			}

			public Action<string> ValueChanged
			{
				get;
				set;
			}

			public Action UpdateStyle
			{
				get;
				set;
			}

			public SettingsRow(IEnumerable<PrinterSettingsLayer> layerCascade)
				: this((FlowDirection)0)
			{
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				SettingsRow settingsRow = this;
				((GuiWidget)this).set_Margin(new BorderDouble(0.0, 2.0));
				((GuiWidget)this).set_Padding(new BorderDouble(3.0));
				((GuiWidget)this).set_HAnchor((HAnchor)5);
				ActiveSliceSettings.SettingChanged.RegisterEvent((EventHandler)delegate(object s, EventArgs e)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					if (((StringEventArgs)e).get_Data() == settingsRow.SettingsKey)
					{
						string value = ActiveSliceSettings.Instance.GetValue(settingsRow.SettingsKey, layerCascade);
						if (settingsRow.SettingsValue != value || settingsRow.SettingsKey == "com_port")
						{
							settingsRow.SettingsValue = value;
							settingsRow.ValueChanged?.Invoke(value);
						}
						settingsRow.UpdateStyle?.Invoke();
					}
				}, ref unregisterEvents);
			}

			public override void OnClosed(ClosedEventArgs e)
			{
				unregisterEvents?.Invoke(this, null);
				((GuiWidget)this).OnClosed(e);
			}

			public void RefreshValue(IEnumerable<PrinterSettingsLayer> layerFilters)
			{
				string obj = (SettingsValue = GetActiveValue(SettingsKey, layerFilters));
				UpdateStyle?.Invoke();
				ValueChanged?.Invoke(obj);
			}
		}

		private TextImageButtonFactory buttonFactory = new TextImageButtonFactory();

		private SliceSettingsDetailControl sliceSettingsDetailControl;

		private TabControl topCategoryTabs;

		private AltGroupBox noConnectionMessageContainer;

		private TextImageButtonFactory textImageButtonFactory;

		private List<PrinterSettingsLayer> layerCascade;

		private PrinterSettingsLayer persistenceLayer;

		private NamedSettingsLayers viewFilter;

		internal static ImageBuffer restoreNormal;

		internal static ImageBuffer restoreHover;

		internal static ImageBuffer restorePressed;

		private bool showControlBar = true;

		private EventHandler unregisterEvents;

		private int tabIndexForItem;

		private static readonly RGBA_Bytes materialSettingBackgroundColor;

		private static readonly RGBA_Bytes qualitySettingBackgroundColor;

		public static readonly RGBA_Bytes userSettingBackgroundColor;

		public string UserLevel
		{
			get
			{
				if (SliceSettingsOrganizer.Instance.UserLevels.ContainsKey(sliceSettingsDetailControl.SelectedValue))
				{
					return sliceSettingsDetailControl.SelectedValue;
				}
				return "Simple";
			}
		}

		public bool ShowControlBar
		{
			get
			{
				return showControlBar;
			}
			set
			{
				showControlBar = value;
			}
		}

		static SliceSettingsWidget()
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Invalid comparison between Unknown and I4
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			materialSettingBackgroundColor = new RGBA_Bytes(255, 127, 0, 108);
			qualitySettingBackgroundColor = new RGBA_Bytes(255, 255, 0, 108);
			userSettingBackgroundColor = new RGBA_Bytes(68, 95, 220, 108);
			int size = (int)(16.0 * GuiWidget.get_DeviceScale());
			if ((int)OsInformation.get_OperatingSystem() == 5)
			{
				restoreNormal = ColorCircle(size, new RGBA_Bytes(200, 0, 0));
			}
			else
			{
				restoreNormal = ColorCircle(size, new RGBA_Bytes(128, 128, 128));
			}
			restoreHover = ColorCircle(size, new RGBA_Bytes(200, 0, 0));
			restorePressed = ColorCircle(size, new RGBA_Bytes(255, 0, 0));
		}

		public SliceSettingsWidget(List<PrinterSettingsLayer> layerCascade = null, NamedSettingsLayers viewFilter = NamedSettingsLayers.All)
			: this()
		{
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Expected O, but got Unknown
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Expected O, but got Unknown
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Expected O, but got Unknown
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Expected O, but got Unknown
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Expected O, but got Unknown
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Expected O, but got Unknown
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_051f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			SliceSettingsWidget sliceSettingsWidget = this;
			this.layerCascade = layerCascade;
			this.viewFilter = viewFilter;
			List<PrinterSettingsLayer> list = layerCascade;
			persistenceLayer = ((list != null) ? Enumerable.First<PrinterSettingsLayer>((IEnumerable<PrinterSettingsLayer>)list) : null) ?? ActiveSliceSettings.Instance.UserLayer;
			textImageButtonFactory = new TextImageButtonFactory();
			textImageButtonFactory.normalFillColor = RGBA_Bytes.Transparent;
			textImageButtonFactory.FixedHeight = 15.0 * GuiWidget.get_DeviceScale();
			textImageButtonFactory.fontSize = 8.0;
			textImageButtonFactory.borderWidth = 1.0;
			textImageButtonFactory.normalBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);
			textImageButtonFactory.hoverBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);
			textImageButtonFactory.disabledTextColor = RGBA_Bytes.Gray;
			textImageButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_SecondaryTextColor();
			textImageButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			buttonFactory.FixedHeight = 20.0 * GuiWidget.get_DeviceScale();
			buttonFactory.fontSize = 10.0;
			buttonFactory.normalFillColor = RGBA_Bytes.White;
			buttonFactory.normalTextColor = RGBA_Bytes.DarkGray;
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_VAnchor((VAnchor)4);
			FlowLayoutWidget val2 = val;
			((GuiWidget)val2).AnchorAll();
			((GuiWidget)val2).set_Padding(new BorderDouble(3.0, 0.0));
			((GuiWidget)this).AddChild((GuiWidget)(object)val2, -1);
			noConnectionMessageContainer = new AltGroupBox((GuiWidget)new TextWidget("No Printer Selected".Localize(), 0.0, 0.0, 18.0, (Justification)0, ActiveTheme.get_Instance().get_SecondaryAccentColor(), true, false, default(RGBA_Bytes), (TypeFace)null));
			((GuiWidget)noConnectionMessageContainer).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 10.0));
			noConnectionMessageContainer.BorderColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			((GuiWidget)noConnectionMessageContainer).set_HAnchor((HAnchor)5);
			((GuiWidget)noConnectionMessageContainer).set_Height(90.0);
			TextWidget val3 = new TextWidget("No printer is currently selected. Please select a printer to edit slice settings.".Localize() + "\n\n" + "NOTE: You need to select a printer, but do not need to connect to it.".Localize(), 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val3).set_Margin(new BorderDouble(5.0));
			val3.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val3).set_VAnchor((VAnchor)2);
			((GuiWidget)noConnectionMessageContainer).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)noConnectionMessageContainer, -1);
			topCategoryTabs = new TabControl((Orientation)0);
			topCategoryTabs.get_TabBar().set_BorderColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)topCategoryTabs).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 8.0));
			((GuiWidget)topCategoryTabs).AnchorAll();
			sliceSettingsDetailControl = new SliceSettingsDetailControl(layerCascade);
			List<TabBar> list2 = new List<TabBar>();
			for (int i = 0; i < SliceSettingsOrganizer.Instance.UserLevels[UserLevel].CategoriesList.Count; i++)
			{
				OrganizerCategory organizerCategory = SliceSettingsOrganizer.Instance.UserLevels[UserLevel].CategoriesList[i];
				TabPage val4 = new TabPage(organizerCategory.Name.Localize());
				SimpleTextTabWidget val5 = new SimpleTextTabWidget(val4, organizerCategory.Name + " Tab", 16.0, ActiveTheme.get_Instance().get_TabLabelSelected(), default(RGBA_Bytes), ActiveTheme.get_Instance().get_TabLabelUnselected(), default(RGBA_Bytes));
				((GuiWidget)val4).AnchorAll();
				topCategoryTabs.AddTab((Tab)(object)val5);
				TabControl val6 = CreateSideTabsAndPages(organizerCategory);
				list2.Add(val6.get_TabBar());
				((GuiWidget)val4).AddChild((GuiWidget)(object)val6, -1);
			}
			((GuiWidget)topCategoryTabs.get_TabBar()).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)topCategoryTabs.get_TabBar()).AddChild((GuiWidget)(object)sliceSettingsDetailControl, -1);
			double num = 0.0;
			foreach (TabBar item in list2)
			{
				num = Math.Max(num, ((GuiWidget)item).get_Width());
			}
			foreach (TabBar item2 in list2)
			{
				((GuiWidget)item2).set_MinimumSize(new Vector2(num, ((GuiWidget)item2).get_MinimumSize().y));
			}
			foreach (TabBar item3 in list2)
			{
				if (((GuiWidget)item3).CountVisibleChildren() == 1)
				{
					((GuiWidget)item3).set_MinimumSize(new Vector2(0.0, 0.0));
					((GuiWidget)item3).set_Width(0.0);
				}
			}
			((GuiWidget)val2).AddChild((GuiWidget)(object)topCategoryTabs, -1);
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)onPrinterStatusChanged, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.EnableChanged.RegisterEvent((EventHandler)onPrinterStatusChanged, ref unregisterEvents);
			SetVisibleControls();
			string settingsName = "SliceSettingsWidget_CurrentTab";
			string text = UserSettings.Instance.get(settingsName);
			topCategoryTabs.SelectTab(text);
			topCategoryTabs.get_TabBar().add_TabIndexChanged((EventHandler)delegate
			{
				string selectedTabName = sliceSettingsWidget.topCategoryTabs.get_TabBar().get_SelectedTabName();
				if (!string.IsNullOrEmpty(selectedTabName) && layerCascade == null)
				{
					UserSettings.Instance.set(settingsName, selectedTabName);
				}
			});
			((GuiWidget)this).AnchorAll();
		}

		public void CurrentlyActiveCategory(out int index, out string name)
		{
			index = topCategoryTabs.get_SelectedTabIndex();
			name = topCategoryTabs.get_SelectedTabName();
		}

		public void CurrentlyActiveGroup(out int index, out string name)
		{
			index = 0;
			name = "";
			TabPage activePage = topCategoryTabs.GetActivePage();
			TabControl val = null;
			if (((Collection<GuiWidget>)(object)((GuiWidget)activePage).get_Children()).Count > 0)
			{
				GuiWidget obj = ((Collection<GuiWidget>)(object)((GuiWidget)activePage).get_Children())[0];
				val = obj as TabControl;
			}
			if (val != null)
			{
				index = val.get_SelectedTabIndex();
				name = val.get_SelectedTabName();
			}
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		private void onPrinterStatusChanged(object sender, EventArgs e)
		{
			SetVisibleControls();
			((GuiWidget)this).Invalidate();
		}

		private void APP_onPrinterStatusChanged(object sender, EventArgs e)
		{
			SetVisibleControls();
			((GuiWidget)this).Invalidate();
		}

		private void SetVisibleControls()
		{
			if (ActiveSliceSettings.Instance.PrinterSelected)
			{
				((GuiWidget)topCategoryTabs).set_Visible(true);
				((GuiWidget)noConnectionMessageContainer).set_Visible(false);
			}
			else
			{
				((GuiWidget)topCategoryTabs).set_Visible(false);
				((GuiWidget)noConnectionMessageContainer).set_Visible(true);
			}
		}

		private TabControl CreateSideTabsAndPages(OrganizerCategory category)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Expected O, but got Unknown
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Expected O, but got Unknown
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Expected O, but got Unknown
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Expected O, but got Unknown
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			TabControl leftSideGroupTabs = new TabControl((Orientation)1);
			((GuiWidget)leftSideGroupTabs).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 5.0));
			leftSideGroupTabs.get_TabBar().set_BorderColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			foreach (OrganizerGroup groups in category.GroupsList)
			{
				tabIndexForItem = 0;
				TabPage val = new TabPage(groups.Name.Localize());
				((GuiWidget)val).set_HAnchor((HAnchor)5);
				SimpleTextTabWidget val2 = new SimpleTextTabWidget(val, groups.Name + " Tab", 14.0, ActiveTheme.get_Instance().get_TabLabelSelected(), default(RGBA_Bytes), ActiveTheme.get_Instance().get_TabLabelUnselected(), default(RGBA_Bytes));
				((GuiWidget)val2).set_HAnchor((HAnchor)5);
				FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)3);
				((GuiWidget)val3).AnchorAll();
				bool flag = false;
				foreach (OrganizerSubGroup subGroups in groups.SubGroupsList)
				{
					string englishString = subGroups.Name;
					int num = 1;
					if (subGroups.Name == "Tool X")
					{
						num = ActiveSliceSettings.Instance.GetValue<int>("extruder_count");
					}
					for (int i = 0; i < num; i++)
					{
						if (subGroups.Name == "Tool X")
						{
							englishString = StringHelper.FormatWith("{0} {1}", new object[2]
							{
								"Tool".Localize(),
								i + 1
							});
						}
						bool flag2 = false;
						FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)3);
						((GuiWidget)val4).set_HAnchor((HAnchor)5);
						((GuiWidget)this).set_HAnchor((HAnchor)5);
						SliceEngineMapping sliceEngineMapping = ActiveSliceSettings.Instance.Helpers.ActiveSliceEngine();
						foreach (SliceSettingData settingData in subGroups.SettingDataList)
						{
							bool flag3 = CheckIfShouldBeShown(settingData);
							if (!(sliceEngineMapping.MapContains(settingData.SlicerConfigName) && flag3))
							{
								continue;
							}
							flag2 = true;
							bool addControl;
							GuiWidget val5 = CreateSettingInfoUIControls(settingData, layerCascade, persistenceLayer, viewFilter, i, out addControl, ref tabIndexForItem);
							if (addControl)
							{
								((GuiWidget)val4).AddChild(val5, -1);
								GuiWidget helpBox = AddInHelpText(val4, settingData);
								if (!sliceSettingsDetailControl.ShowingHelp)
								{
									helpBox.set_Visible(false);
								}
								sliceSettingsDetailControl.ShowHelpChanged += delegate
								{
									helpBox.set_Visible(sliceSettingsDetailControl.ShowingHelp);
								};
								((GuiWidget)val4).AddChild(helpBox, -1);
							}
						}
						if (flag2)
						{
							flag = true;
							AltGroupBox altGroupBox = new AltGroupBox(englishString.Localize());
							altGroupBox.TextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
							altGroupBox.BorderColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
							((GuiWidget)altGroupBox).AddChild((GuiWidget)(object)val4, -1);
							((GuiWidget)altGroupBox).set_HAnchor((HAnchor)5);
							((GuiWidget)altGroupBox).set_Margin(new BorderDouble(3.0, 3.0, 3.0, 0.0));
							((GuiWidget)val3).AddChild((GuiWidget)(object)altGroupBox, -1);
						}
					}
				}
				if (flag)
				{
					SliceSettingListControl scrollOnGroupTab = new SliceSettingListControl();
					((GuiWidget)val3).set_VAnchor((VAnchor)8);
					((GuiWidget)val3).set_HAnchor((HAnchor)5);
					((GuiWidget)scrollOnGroupTab).AddChild((GuiWidget)(object)val3, -1);
					((GuiWidget)val).AddChild((GuiWidget)(object)scrollOnGroupTab, -1);
					leftSideGroupTabs.AddTab((Tab)(object)val2);
					string settingsScrollPosition = StringHelper.FormatWith("SliceSettingsWidget_{0}_{1}_ScrollPosition", new object[2]
					{
						category.Name,
						groups.Name
					});
					UiThread.RunOnIdle((Action)delegate
					{
						//IL_0039: Unknown result type (might be due to invalid IL or missing references)
						int @int = UserSettings.Instance.Fields.GetInt(settingsScrollPosition, -100000);
						if (@int != -100000)
						{
							((ScrollableWidget)scrollOnGroupTab).set_ScrollPosition(new Vector2(0.0, (double)@int));
						}
					});
					((ScrollableWidget)scrollOnGroupTab).add_ScrollPositionChanged((EventHandler)delegate
					{
						//IL_002d: Unknown result type (might be due to invalid IL or missing references)
						if (((GuiWidget)scrollOnGroupTab).get_CanSelect())
						{
							UserSettings.Instance.Fields.SetInt(settingsScrollPosition, (int)((ScrollableWidget)scrollOnGroupTab).get_ScrollPosition().y);
						}
					});
				}
				if (groups.Name == "Connection")
				{
					((GuiWidget)val3).AddChild(CreatePrinterExtraControls(isPrimaryView: true), -1);
				}
			}
			string settingsTypeName = StringHelper.FormatWith("SliceSettingsWidget_{0}_CurrentTab", new object[1]
			{
				category.Name
			});
			string text = UserSettings.Instance.get(settingsTypeName);
			leftSideGroupTabs.SelectTab(text);
			leftSideGroupTabs.get_TabBar().add_TabIndexChanged((EventHandler)delegate
			{
				string selectedTabName = leftSideGroupTabs.get_TabBar().get_SelectedTabName();
				if (!string.IsNullOrEmpty(selectedTabName) && layerCascade == null)
				{
					UserSettings.Instance.set(settingsTypeName, selectedTabName);
				}
			});
			return leftSideGroupTabs;
		}

		private bool CheckIfShouldBeShown(SliceSettingData settingData)
		{
			bool result = ActiveSliceSettings.Instance.ParseShowString(settingData.ShowIfSet, layerCascade);
			if ((viewFilter == NamedSettingsLayers.Material || viewFilter == NamedSettingsLayers.Quality) && !settingData.ShowAsOverride)
			{
				result = false;
			}
			return result;
		}

		private GuiWidget AddInHelpText(FlowLayoutWidget topToBottomSettings, SliceSettingData settingData)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Expected O, but got Unknown
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			double width = 380.0 * GuiWidget.get_DeviceScale();
			((GuiWidget)val).set_Margin(new BorderDouble(0.0));
			((GuiWidget)val).set_Padding(new BorderDouble(5.0));
			((GuiWidget)val).set_BackgroundColor(ActiveTheme.get_Instance().get_TransparentDarkOverlay());
			double num = 10.0;
			GuiWidget val2 = (GuiWidget)new WrappedTextWidget(settingData.HelpText, num, (Justification)0, RGBA_Bytes.White, true, false, default(RGBA_Bytes), true);
			val2.set_Width(width);
			val2.set_Margin(new BorderDouble(5.0, 0.0, 0.0, 0.0));
			((GuiWidget)val).AddChild(val2, -1);
			((GuiWidget)val).set_MinimumSize(new Vector2(0.0, ((GuiWidget)val).get_MinimumSize().y));
			return (GuiWidget)(object)val;
		}

		private static GuiWidget GetExtraSettingsWidget(SliceSettingData settingData)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Expected O, but got Unknown
			//IL_0069: Expected O, but got Unknown
			GuiWidget val = new GuiWidget();
			val.set_HAnchor((HAnchor)5);
			val.set_VAnchor((VAnchor)10);
			val.set_Padding(new BorderDouble(5.0, 0.0));
			val.AddChild((GuiWidget)new WrappedTextWidget(settingData.ExtraSettings.Localize(), 8.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), true), -1);
			return val;
		}

		private static string GetActiveValue(string slicerConfigName, IEnumerable<PrinterSettingsLayer> layerCascade)
		{
			return ActiveSliceSettings.Instance.GetValue(slicerConfigName, layerCascade);
		}

		public static GuiWidget CreatePrinterExtraControls(bool isPrimaryView = false)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Expected O, but got Unknown
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Expected O, but got Unknown
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Expected O, but got Unknown
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Expected O, but got Unknown
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			FlowLayoutWidget val2 = val;
			if (isPrimaryView)
			{
				string text = "March 1, 2016";
				if (ActiveSliceSettings.Instance?.OemLayer != null)
				{
					string value = ActiveSliceSettings.Instance.OemLayer.ValueOrDefault("created_date");
					try
					{
						if (!string.IsNullOrEmpty(value))
						{
							text = Convert.ToDateTime(value).ToLocalTime().ToString("MMMM d, yyyy h:mm tt");
						}
					}
					catch
					{
					}
				}
				FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
				((GuiWidget)val3).set_BackgroundColor(ActiveTheme.get_Instance().get_TertiaryBackgroundColor());
				((GuiWidget)val3).set_Padding(new BorderDouble(5.0));
				((GuiWidget)val3).set_Margin(new BorderDouble(3.0, 20.0, 3.0, 0.0));
				((GuiWidget)val3).set_HAnchor((HAnchor)5);
				FlowLayoutWidget val4 = val3;
				string value2 = ActiveSliceSettings.Instance.GetValue("make");
				string value3 = ActiveSliceSettings.Instance.GetValue("model");
				string text2 = $"{value2} {value3}";
				if (text2 == "Other Other")
				{
					text2 = "Custom Profile".Localize();
				}
				TextWidget val5 = new TextWidget(text2, 0.0, 0.0, 9.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				((GuiWidget)val5).set_Margin(new BorderDouble(0.0, 4.0, 10.0, 4.0));
				val5.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
				((GuiWidget)val4).AddChild((GuiWidget)val5, -1);
				((GuiWidget)val4).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
				TextWidget val6 = new TextWidget(text, 0.0, 0.0, 9.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				((GuiWidget)val6).set_Margin(new BorderDouble(0.0, 4.0, 10.0, 4.0));
				val6.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
				((GuiWidget)val4).AddChild((GuiWidget)val6, -1);
				((GuiWidget)val2).AddChild((GuiWidget)(object)val4, -1);
			}
			TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();
			textImageButtonFactory.borderWidth = 1.0;
			if (ActiveTheme.get_Instance().get_IsDarkTheme())
			{
				textImageButtonFactory.normalBorderColor = new RGBA_Bytes(99, 99, 99);
			}
			else
			{
				textImageButtonFactory.normalBorderColor = new RGBA_Bytes(140, 140, 140);
			}
			textImageButtonFactory.normalTextColor = RGBA_Bytes.Red;
			Button val7 = textImageButtonFactory.Generate("Delete Printer".Localize());
			((GuiWidget)val7).set_Name("Delete Printer Button");
			((GuiWidget)val7).set_HAnchor((HAnchor)2);
			((GuiWidget)val7).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				StyledMessageBox.ShowMessageBox(delegate(bool doDelete)
				{
					if (doDelete)
					{
						ActiveSliceSettings.Instance.Helpers.SetMarkedForDelete(markedForDelete: true);
					}
				}, "Are you sure you want to delete your currently selected printer?".Localize(), "Delete Printer?".Localize(), StyledMessageBox.MessageType.YES_NO, "Delete Printer".Localize());
			});
			((GuiWidget)val2).AddChild((GuiWidget)(object)val7, -1);
			return (GuiWidget)(object)val2;
		}

		public static GuiWidget CreateSettingControl(string sliceSettingsKey, ref int tabIndex)
		{
			bool addControl;
			GuiWidget result = CreateSettingInfoUIControls(SliceSettingsOrganizer.Instance.GetSettingsData(sliceSettingsKey), null, ActiveSliceSettings.Instance.UserLayer, NamedSettingsLayers.All, 0, out addControl, ref tabIndex);
			if (addControl)
			{
				return result;
			}
			return null;
		}

		private static GuiWidget CreateSettingInfoUIControls(SliceSettingData settingData, List<PrinterSettingsLayer> layerCascade, PrinterSettingsLayer persistenceLayer, NamedSettingsLayers viewFilter, int extruderIndex, out bool addControl, ref int tabIndexForItem)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Expected O, but got Unknown
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Expected O, but got Unknown
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Expected O, but got Unknown
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Expected O, but got Unknown
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Expected O, but got Unknown
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Expected O, but got Unknown
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Expected O, but got Unknown
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05aa: Expected O, but got Unknown
			//IL_0967: Unknown result type (might be due to invalid IL or missing references)
			//IL_096e: Expected O, but got Unknown
			//IL_0ac1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac8: Expected O, but got Unknown
			//IL_0c5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc9: Expected O, but got Unknown
			//IL_0f3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f59: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f62: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f90: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fb0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fbf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fc9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fd5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fe4: Expected O, but got Unknown
			//IL_10b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_10bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_10e3: Expected O, but got Unknown
			//IL_11ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_11b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_11cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_11dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_11e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_11ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_11f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_120a: Expected O, but got Unknown
			//IL_13c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1424: Unknown result type (might be due to invalid IL or missing references)
			//IL_142d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1433: Unknown result type (might be due to invalid IL or missing references)
			//IL_1436: Unknown result type (might be due to invalid IL or missing references)
			//IL_143b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1442: Unknown result type (might be due to invalid IL or missing references)
			//IL_1455: Unknown result type (might be due to invalid IL or missing references)
			//IL_1465: Expected O, but got Unknown
			//IL_1491: Unknown result type (might be due to invalid IL or missing references)
			//IL_1496: Unknown result type (might be due to invalid IL or missing references)
			//IL_149e: Unknown result type (might be due to invalid IL or missing references)
			//IL_14b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_14bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_14c4: Expected O, but got Unknown
			//IL_14da: Unknown result type (might be due to invalid IL or missing references)
			//IL_14e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_14e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_14ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_14f7: Expected O, but got Unknown
			//IL_1549: Unknown result type (might be due to invalid IL or missing references)
			//IL_154e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1552: Unknown result type (might be due to invalid IL or missing references)
			//IL_15d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_1676: Unknown result type (might be due to invalid IL or missing references)
			//IL_16d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_16e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_16e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_16ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_16ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_16f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_1709: Unknown result type (might be due to invalid IL or missing references)
			//IL_1719: Expected O, but got Unknown
			//IL_1745: Unknown result type (might be due to invalid IL or missing references)
			//IL_174a: Unknown result type (might be due to invalid IL or missing references)
			//IL_175d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1767: Unknown result type (might be due to invalid IL or missing references)
			//IL_176e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1778: Expected O, but got Unknown
			//IL_178e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1797: Unknown result type (might be due to invalid IL or missing references)
			//IL_179d: Unknown result type (might be due to invalid IL or missing references)
			//IL_17a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_17ab: Expected O, but got Unknown
			//IL_1857: Unknown result type (might be due to invalid IL or missing references)
			//IL_185d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1863: Unknown result type (might be due to invalid IL or missing references)
			//IL_1869: Unknown result type (might be due to invalid IL or missing references)
			//IL_186c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1871: Unknown result type (might be due to invalid IL or missing references)
			//IL_1877: Unknown result type (might be due to invalid IL or missing references)
			//IL_1881: Unknown result type (might be due to invalid IL or missing references)
			//IL_1882: Unknown result type (might be due to invalid IL or missing references)
			//IL_188e: Expected O, but got Unknown
			//IL_18b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_18be: Unknown result type (might be due to invalid IL or missing references)
			//IL_18c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_18d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_18dc: Expected O, but got Unknown
			//IL_18dc: Expected O, but got Unknown
			//IL_18dc: Expected O, but got Unknown
			//IL_18dc: Expected O, but got Unknown
			//IL_18d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_18e1: Expected O, but got Unknown
			//IL_18dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_18e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_18fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_1903: Unknown result type (might be due to invalid IL or missing references)
			//IL_1928: Unknown result type (might be due to invalid IL or missing references)
			//IL_1932: Unknown result type (might be due to invalid IL or missing references)
			//IL_1947: Expected O, but got Unknown
			addControl = true;
			string activeValue = GetActiveValue(settingData.SlicerConfigName, layerCascade);
			GuiWidget val = new GuiWidget();
			val.set_HAnchor((HAnchor)5);
			val.set_VAnchor((VAnchor)10);
			GuiWidget val2 = val;
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			GuiWidget val4 = new GuiWidget();
			val4.set_HAnchor((HAnchor)0);
			val4.set_VAnchor((VAnchor)10);
			val4.set_Width(settingData.ShowAsOverride ? (50.0 * GuiWidget.get_DeviceScale()) : 5.0);
			GuiWidget val5 = val4;
			GuiWidget val6 = new GuiWidget();
			val6.set_HAnchor((HAnchor)0);
			val6.set_VAnchor((VAnchor)10);
			val6.set_Width(settingData.ShowAsOverride ? (30.0 * GuiWidget.get_DeviceScale()) : 0.0);
			GuiWidget val7 = val6;
			SettingsRow settingsRow = new SettingsRow(layerCascade)
			{
				SettingsKey = settingData.SlicerConfigName,
				SettingsValue = activeValue
			};
			((GuiWidget)settingsRow).AddChild(val2, -1);
			((GuiWidget)settingsRow).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)settingsRow).AddChild(val5, -1);
			((GuiWidget)settingsRow).AddChild(val7, -1);
			((GuiWidget)settingsRow).set_Name(settingData.SlicerConfigName + " Edit Field");
			if (!PrinterSettings.KnownSettings.Contains(settingData.SlicerConfigName))
			{
				TextWidget val8 = new TextWidget($"Setting '{settingData.SlicerConfigName}' not found in known settings", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				val8.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
				val2.AddChild((GuiWidget)(object)val8, -1);
				val2.set_BackgroundColor(RGBA_Bytes.Red);
			}
			else
			{
				int num = (int)(60.0 * GuiWidget.get_DeviceScale() + 0.5);
				int num2 = (int)(60.0 * GuiWidget.get_DeviceScale() + 0.5);
				int num3 = (int)(60.0 * GuiWidget.get_DeviceScale() + 0.5);
				int num4 = (int)(120.0 * GuiWidget.get_DeviceScale() + 0.5);
				if (settingData.DataEditType != SliceSettingData.DataEditTypes.MULTI_LINE_TEXT)
				{
					GuiWidget val9 = new GuiWidget();
					val9.set_Padding(new BorderDouble(0.0, 0.0, 5.0, 0.0));
					val9.set_VAnchor((VAnchor)10);
					val9.set_HAnchor((HAnchor)5);
					GuiWidget val10 = val9;
					val10.AddChild((GuiWidget)new WrappedTextWidget(settingData.PresentationName.Localize(), 10.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), true), -1);
					val2.AddChild(val10, -1);
				}
				switch (settingData.DataEditType)
				{
				case SliceSettingData.DataEditTypes.INT:
				{
					FlowLayoutWidget val26 = new FlowLayoutWidget((FlowDirection)0);
					int.TryParse(activeValue, out var result5);
					MHNumberEdit mHNumberEdit7 = new MHNumberEdit(result5, 0.0, 0.0, 12.0, num, 0.0, allowNegatives: false, allowDecimals: false, -2147483648.0, 2147483647.0, 1.0, tabIndexForItem++);
					((GuiWidget)mHNumberEdit7).set_ToolTipText(settingData.HelpText);
					mHNumberEdit7.SelectAllOnFocus = true;
					((GuiWidget)mHNumberEdit7).set_Name(settingData.PresentationName + " Edit");
					MHNumberEdit intEditWidget = mHNumberEdit7;
					((TextEditWidget)intEditWidget.ActuallNumberEdit).add_EditComplete((EventHandler)delegate(object sender, EventArgs e)
					{
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						ActiveSliceSettings.Instance.SetValue(settingData.SlicerConfigName, ((NumberEdit)sender).get_Value().ToString(), persistenceLayer);
						settingsRow.UpdateStyle();
					});
					((GuiWidget)val26).AddChild((GuiWidget)(object)intEditWidget, -1);
					val5.AddChild(GetExtraSettingsWidget(settingData), -1);
					if (settingData.QuickMenuSettings.Count > 0)
					{
						((GuiWidget)val3).AddChild(CreateQuickMenu(settingData, persistenceLayer, (GuiWidget)(object)val26, ((TextEditWidget)intEditWidget.ActuallNumberEdit).get_InternalTextEditWidget(), layerCascade), -1);
					}
					else
					{
						((GuiWidget)val3).AddChild((GuiWidget)(object)val26, -1);
					}
					settingsRow.ValueChanged = delegate(string text)
					{
						((GuiWidget)intEditWidget).set_Text(text);
					};
					break;
				}
				case SliceSettingData.DataEditTypes.DOUBLE:
				{
					double.TryParse(activeValue, out var result6);
					MHNumberEdit mHNumberEdit8 = new MHNumberEdit(result6, 0.0, 0.0, 12.0, num2, 0.0, allowNegatives: true, allowDecimals: true, -2147483648.0, 2147483647.0, 1.0, tabIndexForItem++);
					((GuiWidget)mHNumberEdit8).set_ToolTipText(settingData.HelpText);
					mHNumberEdit8.SelectAllOnFocus = true;
					MHNumberEdit doubleEditWidget = mHNumberEdit8;
					((TextEditWidget)doubleEditWidget.ActuallNumberEdit).add_EditComplete((EventHandler)delegate(object sender, EventArgs e)
					{
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						ActiveSliceSettings.Instance.SetValue(settingData.SlicerConfigName, ((NumberEdit)sender).get_Value().ToString(), persistenceLayer);
						settingsRow.UpdateStyle();
					});
					((GuiWidget)val3).AddChild((GuiWidget)(object)doubleEditWidget, -1);
					val5.AddChild(GetExtraSettingsWidget(settingData), -1);
					settingsRow.ValueChanged = delegate(string text)
					{
						double result7 = 0.0;
						double.TryParse(text, out result7);
						doubleEditWidget.ActuallNumberEdit.set_Value(result7);
					};
					break;
				}
				case SliceSettingData.DataEditTypes.POSITIVE_DOUBLE:
				{
					FlowLayoutWidget val20 = new FlowLayoutWidget((FlowDirection)0);
					MHNumberEdit mHNumberEdit6 = new MHNumberEdit(0.0, 0.0, 0.0, 12.0, num2, 0.0, allowNegatives: false, allowDecimals: true, -2147483648.0, 2147483647.0, 1.0, tabIndexForItem++);
					((GuiWidget)mHNumberEdit6).set_ToolTipText(settingData.HelpText);
					((GuiWidget)mHNumberEdit6).set_Name(settingData.PresentationName + " Textbox");
					mHNumberEdit6.SelectAllOnFocus = true;
					MHNumberEdit doubleEditWidget2 = mHNumberEdit6;
					bool ChangesMultipleOtherSettings = settingData.SetSettingsOnChange.Count > 0;
					double result4;
					if (ChangesMultipleOtherSettings)
					{
						bool flag = true;
						string activeValue2 = GetActiveValue(settingData.SetSettingsOnChange[0]["TargetSetting"], layerCascade);
						for (int i = 1; i < settingData.SetSettingsOnChange.Count; i++)
						{
							string activeValue3 = GetActiveValue(settingData.SetSettingsOnChange[i]["TargetSetting"], layerCascade);
							if (activeValue2 != activeValue3)
							{
								flag = false;
								break;
							}
						}
						if (flag && activeValue2.EndsWith("mm"))
						{
							double.TryParse(activeValue2.Substring(0, activeValue2.Length - 2), out result4);
							doubleEditWidget2.ActuallNumberEdit.set_Value(result4);
						}
						else
						{
							((GuiWidget)doubleEditWidget2.ActuallNumberEdit.get_InternalNumberEdit()).set_Text("-");
						}
					}
					else
					{
						double.TryParse(activeValue, out result4);
						doubleEditWidget2.ActuallNumberEdit.set_Value(result4);
					}
					((TextEditWidget)doubleEditWidget2.ActuallNumberEdit).get_InternalTextEditWidget().MarkAsStartingState();
					((TextEditWidget)doubleEditWidget2.ActuallNumberEdit).add_EditComplete((EventHandler)delegate(object sender, EventArgs e)
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Expected O, but got Unknown
						NumberEdit val33 = (NumberEdit)sender;
						if (ChangesMultipleOtherSettings && ((GuiWidget)val33).get_Text() != "-")
						{
							ActiveSliceSettings.Instance.SetValue(settingData.SetSettingsOnChange[0]["TargetSetting"], val33.get_Value() + "mm", persistenceLayer);
						}
						ActiveSliceSettings.Instance.SetValue(settingData.SlicerConfigName, val33.get_Value().ToString(), persistenceLayer);
						settingsRow.UpdateStyle();
					});
					((GuiWidget)val20).AddChild((GuiWidget)(object)doubleEditWidget2, -1);
					val5.AddChild(GetExtraSettingsWidget(settingData), -1);
					if (settingData.QuickMenuSettings.Count > 0)
					{
						((GuiWidget)val3).AddChild(CreateQuickMenu(settingData, persistenceLayer, (GuiWidget)(object)val20, ((TextEditWidget)doubleEditWidget2.ActuallNumberEdit).get_InternalTextEditWidget(), layerCascade), -1);
					}
					else
					{
						((GuiWidget)val3).AddChild((GuiWidget)(object)val20, -1);
					}
					settingsRow.ValueChanged = delegate(string text)
					{
						double result10 = 0.0;
						double.TryParse(text, out result10);
						doubleEditWidget2.ActuallNumberEdit.set_Value(result10);
					};
					break;
				}
				case SliceSettingData.DataEditTypes.OFFSET:
				{
					double.TryParse(activeValue, out var result3);
					MHNumberEdit mHNumberEdit5 = new MHNumberEdit(result3, 0.0, 0.0, 12.0, num2, 0.0, allowNegatives: true, allowDecimals: true, -2147483648.0, 2147483647.0, 1.0, tabIndexForItem++);
					((GuiWidget)mHNumberEdit5).set_ToolTipText(settingData.HelpText);
					mHNumberEdit5.SelectAllOnFocus = true;
					MHNumberEdit doubleEditWidget3 = mHNumberEdit5;
					((TextEditWidget)doubleEditWidget3.ActuallNumberEdit).add_EditComplete((EventHandler)delegate(object sender, EventArgs e)
					{
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						ActiveSliceSettings.Instance.SetValue(settingData.SlicerConfigName, ((NumberEdit)sender).get_Value().ToString(), persistenceLayer);
						settingsRow.UpdateStyle();
					});
					((GuiWidget)val3).AddChild((GuiWidget)(object)doubleEditWidget3, -1);
					val5.AddChild(GetExtraSettingsWidget(settingData), -1);
					settingsRow.ValueChanged = delegate(string text)
					{
						double.TryParse(text, out var result11);
						doubleEditWidget3.ActuallNumberEdit.set_Value(result11);
					};
					break;
				}
				case SliceSettingData.DataEditTypes.DOUBLE_OR_PERCENT:
				{
					FlowLayoutWidget val23 = new FlowLayoutWidget((FlowDirection)0);
					MHTextEditWidget mHTextEditWidget = new MHTextEditWidget(activeValue, 0.0, 0.0, 12.0, num2 - 2, 0.0, multiLine: false, tabIndexForItem++);
					((GuiWidget)mHTextEditWidget).set_ToolTipText(settingData.HelpText);
					mHTextEditWidget.SelectAllOnFocus = true;
					MHTextEditWidget stringEdit4 = mHTextEditWidget;
					stringEdit4.ActualTextEditWidget.add_EditComplete((EventHandler)delegate(object sender, EventArgs e)
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Expected O, but got Unknown
						TextEditWidget val31 = (TextEditWidget)sender;
						string text6 = ((GuiWidget)val31).get_Text().Trim();
						bool num8 = text6.Contains("%");
						if (num8)
						{
							text6 = text6.Substring(0, text6.IndexOf("%"));
						}
						double.TryParse(text6, out var result9);
						text6 = result9.ToString();
						if (num8)
						{
							text6 += "%";
						}
						((GuiWidget)val31).set_Text(text6);
						ActiveSliceSettings.Instance.SetValue(settingData.SlicerConfigName, ((GuiWidget)val31).get_Text(), persistenceLayer);
						settingsRow.UpdateStyle();
					});
					stringEdit4.ActualTextEditWidget.get_InternalTextEditWidget().add_AllSelected((EventHandler)delegate(object sender, EventArgs e)
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Expected O, but got Unknown
						InternalTextEditWidget val30 = (InternalTextEditWidget)sender;
						int num7 = ((GuiWidget)val30).get_Text().IndexOf("%");
						if (num7 != -1)
						{
							val30.SetSelection(0, num7 - 1);
						}
					});
					((GuiWidget)val23).AddChild((GuiWidget)(object)stringEdit4, -1);
					val5.AddChild(GetExtraSettingsWidget(settingData), -1);
					if (settingData.QuickMenuSettings.Count > 0)
					{
						((GuiWidget)val3).AddChild(CreateQuickMenu(settingData, persistenceLayer, (GuiWidget)(object)val23, stringEdit4.ActualTextEditWidget.get_InternalTextEditWidget(), layerCascade), -1);
					}
					else
					{
						((GuiWidget)val3).AddChild((GuiWidget)(object)val23, -1);
					}
					settingsRow.ValueChanged = delegate(string text)
					{
						((GuiWidget)stringEdit4).set_Text(text);
					};
					break;
				}
				case SliceSettingData.DataEditTypes.INT_OR_MM:
				{
					FlowLayoutWidget val24 = new FlowLayoutWidget((FlowDirection)0);
					MHTextEditWidget mHTextEditWidget2 = new MHTextEditWidget(activeValue, 0.0, 0.0, 12.0, num2 - 2, 0.0, multiLine: false, tabIndexForItem++);
					((GuiWidget)mHTextEditWidget2).set_ToolTipText(settingData.HelpText);
					mHTextEditWidget2.SelectAllOnFocus = true;
					MHTextEditWidget stringEdit3 = mHTextEditWidget2;
					string startingText = ((GuiWidget)stringEdit3).get_Text();
					stringEdit3.ActualTextEditWidget.add_EditComplete((EventHandler)delegate(object sender, EventArgs e)
					{
						//IL_000f: Unknown result type (might be due to invalid IL or missing references)
						//IL_0019: Expected O, but got Unknown
						TextEditWidget textEditWidget = (TextEditWidget)sender;
						if (!((GuiWidget)textEditWidget).get_ContainsFocus())
						{
							string text4 = ((GuiWidget)textEditWidget).get_Text();
							text4 = text4.Trim();
							bool num6 = text4.Contains("mm");
							if (num6)
							{
								text4 = text4.Substring(0, text4.IndexOf("mm"));
							}
							double.TryParse(text4, out var result8);
							text4 = result8.ToString();
							text4 = ((!num6) ? ((double)(int)result8).ToString() : (text4 + "mm"));
							((GuiWidget)textEditWidget).set_Text(text4);
							startingText = ((GuiWidget)stringEdit3).get_Text();
						}
						ActiveSliceSettings.Instance.SetValue(settingData.SlicerConfigName, ((GuiWidget)textEditWidget).get_Text(), persistenceLayer);
						settingsRow.UpdateStyle();
						if (((GuiWidget)textEditWidget).get_ContainsFocus())
						{
							UiThread.RunOnIdle((Action)delegate
							{
								string text5 = ((GuiWidget)textEditWidget).get_Text();
								int charIndexToInsertBefore = textEditWidget.get_InternalTextEditWidget().get_CharIndexToInsertBefore();
								((GuiWidget)textEditWidget).set_Text(startingText);
								textEditWidget.get_InternalTextEditWidget().MarkAsStartingState();
								((GuiWidget)textEditWidget).set_Text(text5);
								textEditWidget.get_InternalTextEditWidget().set_CharIndexToInsertBefore(charIndexToInsertBefore);
							});
						}
					});
					stringEdit3.ActualTextEditWidget.get_InternalTextEditWidget().add_AllSelected((EventHandler)delegate(object sender, EventArgs e)
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Expected O, but got Unknown
						InternalTextEditWidget val29 = (InternalTextEditWidget)sender;
						int num5 = ((GuiWidget)val29).get_Text().IndexOf("mm");
						if (num5 != -1)
						{
							val29.SetSelection(0, num5 - 1);
						}
					});
					((GuiWidget)val24).AddChild((GuiWidget)(object)stringEdit3, -1);
					val5.AddChild(GetExtraSettingsWidget(settingData), -1);
					if (settingData.QuickMenuSettings.Count > 0)
					{
						((GuiWidget)val3).AddChild(CreateQuickMenu(settingData, persistenceLayer, (GuiWidget)(object)val24, stringEdit3.ActualTextEditWidget.get_InternalTextEditWidget(), layerCascade), -1);
					}
					else
					{
						((GuiWidget)val3).AddChild((GuiWidget)(object)val24, -1);
					}
					settingsRow.ValueChanged = delegate(string text)
					{
						((GuiWidget)stringEdit3).set_Text(text);
					};
					break;
				}
				case SliceSettingData.DataEditTypes.CHECK_BOX:
				{
					CheckBox val27 = new CheckBox("");
					((GuiWidget)val27).set_Name(settingData.PresentationName + " Checkbox");
					((GuiWidget)val27).set_ToolTipText(settingData.HelpText);
					((GuiWidget)val27).set_VAnchor((VAnchor)1);
					val27.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
					val27.set_Checked(activeValue == "1");
					CheckBox checkBoxWidget = val27;
					((GuiWidget)checkBoxWidget).add_Click((EventHandler<MouseEventArgs>)delegate
					{
						ActiveSliceSettings.Instance.SetValue(settingData.SlicerConfigName, checkBoxWidget.get_Checked() ? "1" : "0", persistenceLayer);
					});
					checkBoxWidget.add_CheckedStateChanged((EventHandler)delegate
					{
						foreach (Dictionary<string, string> item3 in settingData.SetSettingsOnChange)
						{
							if (item3.TryGetValue(checkBoxWidget.get_Checked() ? "OnValue" : "OffValue", out var value))
							{
								ActiveSliceSettings.Instance.SetValue(item3["TargetSetting"], value, persistenceLayer);
							}
						}
						settingsRow.UpdateStyle();
					});
					((GuiWidget)val3).AddChild((GuiWidget)(object)checkBoxWidget, -1);
					settingsRow.ValueChanged = delegate(string text)
					{
						checkBoxWidget.set_Checked(text == "1");
					};
					break;
				}
				case SliceSettingData.DataEditTypes.STRING:
				{
					MHTextEditWidget mHTextEditWidget3 = new MHTextEditWidget(activeValue, 0.0, 0.0, 12.0, settingData.ShowAsOverride ? 120 : 200, 0.0, multiLine: false, tabIndexForItem++);
					((GuiWidget)mHTextEditWidget3).set_Name(settingData.PresentationName + " Edit");
					MHTextEditWidget stringEdit2 = mHTextEditWidget3;
					((GuiWidget)stringEdit2).set_ToolTipText(settingData.HelpText);
					stringEdit2.ActualTextEditWidget.add_EditComplete((EventHandler)delegate(object sender, EventArgs e)
					{
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						ActiveSliceSettings.Instance.SetValue(settingData.SlicerConfigName, ((GuiWidget)(TextEditWidget)sender).get_Text(), persistenceLayer);
						settingsRow.UpdateStyle();
					});
					((GuiWidget)val3).AddChild((GuiWidget)(object)stringEdit2, -1);
					settingsRow.ValueChanged = delegate(string text)
					{
						((GuiWidget)stringEdit2).set_Text(text);
					};
					break;
				}
				case SliceSettingData.DataEditTypes.MULTI_LINE_TEXT:
				{
					string text3 = activeValue.Replace("\\n", "\n");
					MHTextEditWidget mHTextEditWidget4 = new MHTextEditWidget(text3, 0.0, 0.0, 12.0, 320.0, num4, multiLine: true, tabIndexForItem++, "", ApplicationController.MonoSpacedTypeFace);
					((GuiWidget)mHTextEditWidget4).set_HAnchor((HAnchor)5);
					MHTextEditWidget stringEdit = mHTextEditWidget4;
					stringEdit.DrawFromHintedCache();
					stringEdit.ActualTextEditWidget.add_EditComplete((EventHandler)delegate(object sender, EventArgs e)
					{
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						ActiveSliceSettings.Instance.SetValue(settingData.SlicerConfigName, ((GuiWidget)(TextEditWidget)sender).get_Text().Replace("\n", "\\n"), persistenceLayer);
						settingsRow.UpdateStyle();
					});
					val2.set_HAnchor((HAnchor)0);
					val2.set_Width(0.0);
					((GuiWidget)val3).AddChild((GuiWidget)(object)stringEdit, -1);
					((GuiWidget)val3).set_HAnchor((HAnchor)5);
					settingsRow.ValueChanged = delegate(string text)
					{
						((GuiWidget)stringEdit).set_Text(text.Replace("\\n", "\n"));
					};
					break;
				}
				case SliceSettingData.DataEditTypes.COM_PORT:
				{
					EventHandler localUnregisterEvents = null;
					bool canChangeComPort = !PrinterConnectionAndCommunication.Instance.PrinterIsConnected && PrinterConnectionAndCommunication.Instance.CommunicationState != PrinterConnectionAndCommunication.CommunicationStates.AttemptingToConnect;
					DropDownList val25 = new DropDownList("None".Localize(), (Direction)1, 200.0, false);
					((GuiWidget)val25).set_ToolTipText(settingData.HelpText);
					((GuiWidget)val25).set_Margin(default(BorderDouble));
					((GuiWidget)val25).set_Name("Serial Port Dropdown");
					((GuiWidget)val25).set_Enabled(canChangeComPort);
					val25.set_TextColor((RGBA_Bytes)(canChangeComPort ? ActiveTheme.get_Instance().get_PrimaryTextColor() : new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 150)));
					val25.set_BorderColor((RGBA_Bytes)(canChangeComPort ? ActiveTheme.get_Instance().get_SecondaryTextColor() : new RGBA_Bytes(ActiveTheme.get_Instance().get_SecondaryTextColor(), 150)));
					DropDownList selectableOptions = val25;
					((GuiWidget)selectableOptions).add_Click((EventHandler<MouseEventArgs>)delegate
					{
						AddComMenuItems(settingData, persistenceLayer, settingsRow, selectableOptions);
					});
					AddComMenuItems(settingData, persistenceLayer, settingsRow, selectableOptions);
					((GuiWidget)val3).AddChild((GuiWidget)(object)selectableOptions, -1);
					settingsRow.ValueChanged = delegate
					{
						selectableOptions.set_SelectedLabel(ActiveSliceSettings.Instance.Helpers.ComPort());
					};
					PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)delegate
					{
						//IL_0049: Unknown result type (might be due to invalid IL or missing references)
						//IL_0053: Unknown result type (might be due to invalid IL or missing references)
						//IL_005f: Unknown result type (might be due to invalid IL or missing references)
						//IL_007c: Unknown result type (might be due to invalid IL or missing references)
						//IL_0086: Unknown result type (might be due to invalid IL or missing references)
						//IL_0092: Unknown result type (might be due to invalid IL or missing references)
						canChangeComPort = !PrinterConnectionAndCommunication.Instance.PrinterIsConnected && PrinterConnectionAndCommunication.Instance.CommunicationState != PrinterConnectionAndCommunication.CommunicationStates.AttemptingToConnect;
						((GuiWidget)selectableOptions).set_Enabled(canChangeComPort);
						selectableOptions.set_TextColor((RGBA_Bytes)(canChangeComPort ? ActiveTheme.get_Instance().get_PrimaryTextColor() : new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 150)));
						selectableOptions.set_BorderColor((RGBA_Bytes)(canChangeComPort ? ActiveTheme.get_Instance().get_SecondaryTextColor() : new RGBA_Bytes(ActiveTheme.get_Instance().get_SecondaryTextColor(), 150)));
					}, ref localUnregisterEvents);
					((GuiWidget)selectableOptions).add_Closed((EventHandler<ClosedEventArgs>)delegate
					{
						localUnregisterEvents?.Invoke(null, null);
					});
					break;
				}
				case SliceSettingData.DataEditTypes.LIST:
				{
					DropDownList val22 = new DropDownList("None".Localize(), (Direction)1, 200.0, false);
					((GuiWidget)val22).set_ToolTipText(settingData.HelpText);
					((GuiWidget)val22).set_Margin(default(BorderDouble));
					DropDownList selectableOptions2 = val22;
					string[] array2 = settingData.ExtraSettings.Split(new char[1]
					{
						','
					});
					foreach (string text2 in array2)
					{
						MenuItem obj = selectableOptions2.AddItem(text2, (string)null, 12.0);
						if (((GuiWidget)obj).get_Text() == activeValue)
						{
							selectableOptions2.set_SelectedLabel(activeValue);
						}
						obj.add_Selected((EventHandler)delegate(object sender, EventArgs e)
						{
							//IL_0001: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Expected O, but got Unknown
							MenuItem val32 = (MenuItem)sender;
							ActiveSliceSettings.Instance.SetValue(settingData.SlicerConfigName, ((GuiWidget)val32).get_Text(), persistenceLayer);
							settingsRow.UpdateStyle();
						});
					}
					((GuiWidget)val3).AddChild((GuiWidget)(object)selectableOptions2, -1);
					settingsRow.ValueChanged = delegate(string text)
					{
						selectableOptions2.set_SelectedLabel(text);
					};
					break;
				}
				case SliceSettingData.DataEditTypes.HARDWARE_PRESENT:
				{
					CheckBox val21 = new CheckBox("");
					((GuiWidget)val21).set_Name(settingData.PresentationName + " Checkbox");
					((GuiWidget)val21).set_ToolTipText(settingData.HelpText);
					((GuiWidget)val21).set_VAnchor((VAnchor)1);
					val21.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
					val21.set_Checked(activeValue == "1");
					CheckBox checkBoxWidget2 = val21;
					((GuiWidget)checkBoxWidget2).add_Click((EventHandler<MouseEventArgs>)delegate(object sender, MouseEventArgs e)
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						bool @checked = ((CheckBox)sender).get_Checked();
						ActiveSliceSettings.Instance.SetValue(settingData.SlicerConfigName, @checked ? "1" : "0", persistenceLayer);
						settingsRow.UpdateStyle();
					});
					((GuiWidget)val3).AddChild((GuiWidget)(object)checkBoxWidget2, -1);
					settingsRow.ValueChanged = delegate(string text)
					{
						checkBoxWidget2.set_Checked(text == "1");
					};
					break;
				}
				case SliceSettingData.DataEditTypes.VECTOR2:
				{
					string[] array = activeValue.Split(new char[1]
					{
						','
					});
					if (array.Length != 2)
					{
						array = new string[2]
						{
							"0",
							"0"
						};
					}
					double.TryParse(array[0], out var result);
					MHNumberEdit mHNumberEdit3 = new MHNumberEdit(result, 0.0, 0.0, 12.0, num3, 0.0, allowNegatives: false, allowDecimals: true, -2147483648.0, 2147483647.0, 1.0, tabIndexForItem++);
					((GuiWidget)mHNumberEdit3).set_ToolTipText(settingData.HelpText);
					mHNumberEdit3.SelectAllOnFocus = true;
					MHNumberEdit xEditWidget = mHNumberEdit3;
					double.TryParse(array[1], out var result2);
					MHNumberEdit mHNumberEdit4 = new MHNumberEdit(result2, 0.0, 0.0, 12.0, num3, 0.0, allowNegatives: false, allowDecimals: true, -2147483648.0, 2147483647.0, 1.0, tabIndexForItem++);
					((GuiWidget)mHNumberEdit4).set_ToolTipText(settingData.HelpText);
					mHNumberEdit4.SelectAllOnFocus = true;
					((GuiWidget)mHNumberEdit4).set_Margin(new BorderDouble(20.0, 0.0, 0.0, 0.0));
					MHNumberEdit yEditWidget = mHNumberEdit4;
					((TextEditWidget)xEditWidget.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
					{
						ActiveSliceSettings.Instance.SetValue(settingData.SlicerConfigName, xEditWidget.ActuallNumberEdit.get_Value() + "," + yEditWidget.ActuallNumberEdit.get_Value(), persistenceLayer);
						settingsRow.UpdateStyle();
					});
					((GuiWidget)val3).AddChild((GuiWidget)(object)xEditWidget, -1);
					TextWidget val17 = new TextWidget("X", 0.0, 0.0, 10.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
					((GuiWidget)val17).set_VAnchor((VAnchor)2);
					((GuiWidget)val17).set_Margin(new BorderDouble(5.0, 0.0));
					((GuiWidget)val3).AddChild((GuiWidget)val17, -1);
					((TextEditWidget)yEditWidget.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
					{
						ActiveSliceSettings.Instance.SetValue(settingData.SlicerConfigName, xEditWidget.ActuallNumberEdit.get_Value() + "," + yEditWidget.ActuallNumberEdit.get_Value(), persistenceLayer);
						settingsRow.UpdateStyle();
					});
					((GuiWidget)val3).AddChild((GuiWidget)(object)yEditWidget, -1);
					GuiWidget val18 = new GuiWidget();
					val18.set_VAnchor((VAnchor)10);
					val18.set_Padding(new BorderDouble(5.0, 0.0));
					val18.set_HAnchor((HAnchor)5);
					GuiWidget val19 = val18;
					val19.AddChild((GuiWidget)new WrappedTextWidget("Y", 9.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), true), -1);
					val5.AddChild(val19, -1);
					settingsRow.ValueChanged = delegate(string text)
					{
						string[] array3 = text.Split(new char[1]
						{
							','
						});
						if (array3.Length != 2)
						{
							array3 = new string[2]
							{
								"0",
								"0"
							};
						}
						double.TryParse(array3[0], out var result12);
						xEditWidget.ActuallNumberEdit.set_Value(result12);
						double.TryParse(array3[1], out result12);
						yEditWidget.ActuallNumberEdit.set_Value(result12);
					};
					break;
				}
				case SliceSettingData.DataEditTypes.OFFSET2:
				{
					Vector2 val13 = ActiveSliceSettings.Instance.Helpers.ExtruderOffset(extruderIndex);
					MHNumberEdit mHNumberEdit = new MHNumberEdit(val13.x, 0.0, 0.0, 12.0, num3, 0.0, allowNegatives: true, allowDecimals: true, -2147483648.0, 2147483647.0, 1.0, tabIndexForItem++);
					((GuiWidget)mHNumberEdit).set_ToolTipText(settingData.HelpText);
					mHNumberEdit.SelectAllOnFocus = true;
					MHNumberEdit xEditWidget2 = mHNumberEdit;
					MHNumberEdit mHNumberEdit2 = new MHNumberEdit(val13.y, 0.0, 0.0, 12.0, num3, 0.0, allowNegatives: true, allowDecimals: true, -2147483648.0, 2147483647.0, 1.0, tabIndexForItem++);
					((GuiWidget)mHNumberEdit2).set_ToolTipText(settingData.HelpText);
					mHNumberEdit2.SelectAllOnFocus = true;
					((GuiWidget)mHNumberEdit2).set_Margin(new BorderDouble(20.0, 0.0, 0.0, 0.0));
					MHNumberEdit yEditWidget2 = mHNumberEdit2;
					((TextEditWidget)xEditWidget2.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
					{
						SaveCommaSeparatedIndexSetting(extruderIndex, layerCascade, settingData.SlicerConfigName, xEditWidget2.ActuallNumberEdit.get_Value() + "x" + yEditWidget2.ActuallNumberEdit.get_Value(), persistenceLayer);
						settingsRow.UpdateStyle();
					});
					((GuiWidget)val3).AddChild((GuiWidget)(object)xEditWidget2, -1);
					TextWidget val14 = new TextWidget("X", 0.0, 0.0, 10.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
					((GuiWidget)val14).set_VAnchor((VAnchor)2);
					((GuiWidget)val14).set_Margin(new BorderDouble(5.0, 0.0));
					((GuiWidget)val3).AddChild((GuiWidget)val14, -1);
					((TextEditWidget)yEditWidget2.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
					{
						SaveCommaSeparatedIndexSetting(extruderIndex, layerCascade, settingData.SlicerConfigName, xEditWidget2.ActuallNumberEdit.get_Value() + "x" + yEditWidget2.ActuallNumberEdit.get_Value(), persistenceLayer);
						settingsRow.UpdateStyle();
					});
					((GuiWidget)val3).AddChild((GuiWidget)(object)yEditWidget2, -1);
					GuiWidget val15 = new GuiWidget();
					val15.set_Padding(new BorderDouble(5.0, 0.0));
					val15.set_HAnchor((HAnchor)5);
					val15.set_VAnchor((VAnchor)10);
					GuiWidget val16 = val15;
					val16.AddChild((GuiWidget)new WrappedTextWidget("Y", 9.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), true), -1);
					val5.AddChild(val16, -1);
					settingsRow.ValueChanged = delegate
					{
						//IL_0015: Unknown result type (might be due to invalid IL or missing references)
						//IL_001a: Unknown result type (might be due to invalid IL or missing references)
						//IL_0026: Unknown result type (might be due to invalid IL or missing references)
						//IL_003c: Unknown result type (might be due to invalid IL or missing references)
						Vector2 val34 = ActiveSliceSettings.Instance.Helpers.ExtruderOffset(extruderIndex);
						xEditWidget2.ActuallNumberEdit.set_Value(val34.x);
						yEditWidget2.ActuallNumberEdit.set_Value(val34.y);
					};
					break;
				}
				case SliceSettingData.DataEditTypes.TOOL:
				{
					((GuiWidget)settingsRow).RemoveAllChildren();
					TabIndexKeeper tabIndexKeeper = new TabIndexKeeper(tabIndexForItem);
					((GuiWidget)settingsRow).AddChild((GuiWidget)(object)new ToolSettingsWidget(extruderIndex, tabIndexKeeper), -1);
					tabIndexForItem = tabIndexKeeper.TabIndex;
					break;
				}
				default:
				{
					TextWidget val11 = new TextWidget($"Missing the setting for '{settingData.DataEditType.ToString()}'.", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
					val11.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
					((GuiWidget)val11).set_BackgroundColor(RGBA_Bytes.Red);
					TextWidget val12 = val11;
					((GuiWidget)val3).AddChild((GuiWidget)(object)val12, -1);
					break;
				}
				}
			}
			Button restoreButton = null;
			if (settingData.ShowAsOverride)
			{
				Button val28 = new Button((GuiWidget)new ButtonViewStates((GuiWidget)new ImageWidget(restoreNormal), (GuiWidget)new ImageWidget(restoreHover), (GuiWidget)new ImageWidget(restorePressed), (GuiWidget)new ImageWidget(restoreNormal)));
				((GuiWidget)val28).set_Name("Restore " + settingData.SlicerConfigName);
				((GuiWidget)val28).set_VAnchor((VAnchor)2);
				((GuiWidget)val28).set_Margin(new BorderDouble(0.0, 0.0, 5.0, 0.0));
				((GuiWidget)val28).set_ToolTipText("Restore Default".Localize());
				restoreButton = val28;
				((GuiWidget)restoreButton).add_Click((EventHandler<MouseEventArgs>)delegate
				{
					if (persistenceLayer == null)
					{
						ActiveSliceSettings.Instance.ClearValue(settingData.SlicerConfigName);
					}
					else
					{
						ActiveSliceSettings.Instance.ClearValue(settingData.SlicerConfigName, persistenceLayer);
					}
					settingsRow.RefreshValue(layerCascade);
				});
				val7.AddChild((GuiWidget)(object)restoreButton, -1);
			}
			settingsRow.UpdateStyle = delegate
			{
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_010e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0136: Unknown result type (might be due to invalid IL or missing references)
				//IL_015e: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_021b: Unknown result type (might be due to invalid IL or missing references)
				if (persistenceLayer.ContainsKey(settingData.SlicerConfigName))
				{
					switch (viewFilter)
					{
					case NamedSettingsLayers.All:
						if (settingData.ShowAsOverride)
						{
							IEnumerable<PrinterSettingsLayer> defaultLayerCascade = ActiveSliceSettings.Instance.defaultLayerCascade;
							Tuple<string, string> valueAndLayerName = ActiveSliceSettings.Instance.GetValueAndLayerName(settingData.SlicerConfigName, Enumerable.Skip<PrinterSettingsLayer>(defaultLayerCascade, 1));
							Tuple<string, string> valueAndLayerName2 = ActiveSliceSettings.Instance.GetValueAndLayerName(settingData.SlicerConfigName, defaultLayerCascade);
							string item = valueAndLayerName2.Item1;
							string item2 = valueAndLayerName2.Item2;
							if (valueAndLayerName.Item1 == item)
							{
								if (item2.StartsWith("Material"))
								{
									((GuiWidget)settingsRow).set_BackgroundColor(materialSettingBackgroundColor);
								}
								else if (item2.StartsWith("Quality"))
								{
									((GuiWidget)settingsRow).set_BackgroundColor(qualitySettingBackgroundColor);
								}
								else
								{
									((GuiWidget)settingsRow).set_BackgroundColor(RGBA_Bytes.Transparent);
								}
								if (restoreButton != null)
								{
									((GuiWidget)restoreButton).set_Visible(false);
								}
							}
							else
							{
								((GuiWidget)settingsRow).set_BackgroundColor(userSettingBackgroundColor);
								if (restoreButton != null)
								{
									((GuiWidget)restoreButton).set_Visible(true);
								}
							}
						}
						break;
					case NamedSettingsLayers.Material:
						((GuiWidget)settingsRow).set_BackgroundColor(materialSettingBackgroundColor);
						if (restoreButton != null)
						{
							((GuiWidget)restoreButton).set_Visible(true);
						}
						break;
					case NamedSettingsLayers.Quality:
						((GuiWidget)settingsRow).set_BackgroundColor(qualitySettingBackgroundColor);
						if (restoreButton != null)
						{
							((GuiWidget)restoreButton).set_Visible(true);
						}
						break;
					case NamedSettingsLayers.User:
						break;
					}
				}
				else if (layerCascade == null)
				{
					if (ActiveSliceSettings.Instance.SettingExistsInLayer(settingData.SlicerConfigName, NamedSettingsLayers.Material))
					{
						((GuiWidget)settingsRow).set_BackgroundColor(materialSettingBackgroundColor);
					}
					else if (ActiveSliceSettings.Instance.SettingExistsInLayer(settingData.SlicerConfigName, NamedSettingsLayers.Quality))
					{
						((GuiWidget)settingsRow).set_BackgroundColor(qualitySettingBackgroundColor);
					}
					else
					{
						((GuiWidget)settingsRow).set_BackgroundColor(RGBA_Bytes.Transparent);
					}
					if (restoreButton != null)
					{
						((GuiWidget)restoreButton).set_Visible(false);
					}
				}
				else
				{
					if (restoreButton != null)
					{
						((GuiWidget)restoreButton).set_Visible(false);
					}
					((GuiWidget)settingsRow).set_BackgroundColor(RGBA_Bytes.Transparent);
				}
			};
			settingsRow.UpdateStyle();
			return (GuiWidget)(object)settingsRow;
		}

		private static void AddComMenuItems(SliceSettingData settingData, PrinterSettingsLayer persistenceLayer, SettingsRow settingsRow, DropDownList selectableOptions)
		{
			((Collection<MenuItem>)(object)((Menu)selectableOptions).MenuItems).Clear();
			string text = ActiveSliceSettings.Instance.Helpers.ComPort();
			string[] portNames = FrostedSerialPort.GetPortNames();
			foreach (string text2 in portNames)
			{
				MenuItem obj = selectableOptions.AddItem(text2, (string)null, 12.0);
				if (((GuiWidget)obj).get_Text() == text)
				{
					selectableOptions.set_SelectedLabel(text);
				}
				obj.add_Selected((EventHandler)delegate(object sender, EventArgs e)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Expected O, but got Unknown
					MenuItem val = (MenuItem)sender;
					if (persistenceLayer == null)
					{
						ActiveSliceSettings.Instance.Helpers.SetComPort(((GuiWidget)val).get_Text());
					}
					else
					{
						ActiveSliceSettings.Instance.Helpers.SetComPort(((GuiWidget)val).get_Text(), persistenceLayer);
					}
					settingsRow.UpdateStyle();
				});
			}
		}

		private static ImageBuffer ColorCircle(int size, RGBA_Bytes color)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Expected O, but got Unknown
			ImageBuffer val = new ImageBuffer(size, size);
			Graphics2D obj = val.NewGraphics2D();
			Vector2 val2 = default(Vector2);
			((Vector2)(ref val2))._002Ector((double)size / 2.0, (double)size / 2.0);
			obj.Circle(val2, (double)size / 2.0, color);
			obj.Line(val2 + new Vector2((double)(-size) / 4.0, (double)(-size) / 4.0), val2 + new Vector2((double)size / 4.0, (double)size / 4.0), RGBA_Bytes.White, 2.0 * GuiWidget.get_DeviceScale());
			obj.Line(val2 + new Vector2((double)(-size) / 4.0, (double)size / 4.0), val2 + new Vector2((double)size / 4.0, (double)(-size) / 4.0), RGBA_Bytes.White, 2.0 * GuiWidget.get_DeviceScale());
			return val;
		}

		private static GuiWidget CreateQuickMenu(SliceSettingData settingData, PrinterSettingsLayer persistenceLayer, GuiWidget content, InternalTextEditWidget internalTextWidget, List<PrinterSettingsLayer> layerCascade)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected O, but got Unknown
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			string activeValue = GetActiveValue(settingData.SlicerConfigName, layerCascade);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			DropDownList selectableOptions = new DropDownList("Custom", (Direction)1, 200.0, false);
			((GuiWidget)selectableOptions).set_Margin(new BorderDouble(0.0, 0.0, 10.0, 0.0));
			foreach (QuickMenuNameValue quickMenuSetting in settingData.QuickMenuSettings)
			{
				string valueLocal = quickMenuSetting.Value;
				MenuItem obj = selectableOptions.AddItem(quickMenuSetting.MenuName, (string)null, 12.0);
				if (activeValue == valueLocal)
				{
					selectableOptions.set_SelectedLabel(quickMenuSetting.MenuName);
				}
				obj.add_Selected((EventHandler)delegate
				{
					ActiveSliceSettings.Instance.SetValue(settingData.SlicerConfigName, valueLocal, persistenceLayer);
					((GuiWidget)internalTextWidget).set_Text(valueLocal);
					internalTextWidget.OnEditComplete((EventArgs)null);
				});
			}
			selectableOptions.AddItem("Custom", (string)null, 12.0);
			((GuiWidget)val).AddChild((GuiWidget)(object)selectableOptions, -1);
			content.set_VAnchor((VAnchor)2);
			((GuiWidget)val).AddChild(content, -1);
			EventHandler localUnregisterEvents = null;
			ActiveSliceSettings.SettingChanged.RegisterEvent((EventHandler)delegate
			{
				bool flag = false;
				foreach (QuickMenuNameValue quickMenuSetting2 in settingData.QuickMenuSettings)
				{
					string menuName = quickMenuSetting2.MenuName;
					if (GetActiveValue(settingData.SlicerConfigName, layerCascade) == quickMenuSetting2.Value)
					{
						selectableOptions.set_SelectedLabel(menuName);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					selectableOptions.set_SelectedLabel("Custom");
				}
			}, ref localUnregisterEvents);
			((GuiWidget)val).add_Closed((EventHandler<ClosedEventArgs>)delegate(object s, ClosedEventArgs e)
			{
				localUnregisterEvents?.Invoke(s, null);
			});
			return (GuiWidget)(object)val;
		}

		private static void SaveCommaSeparatedIndexSetting(int extruderIndexLocal, List<PrinterSettingsLayer> layerCascade, string slicerConfigName, string newSingleValue, PrinterSettingsLayer persistenceLayer)
		{
			string[] array = GetActiveValue(slicerConfigName, layerCascade).Split(new char[1]
			{
				','
			});
			if (array.Length > extruderIndexLocal)
			{
				array[extruderIndexLocal] = newSingleValue;
			}
			else
			{
				string[] array2 = new string[extruderIndexLocal + 1];
				for (int i = 0; i < extruderIndexLocal + 1; i++)
				{
					array2[i] = "";
					if (i < array.Length)
					{
						array2[i] = array[i];
					}
					else if (i == extruderIndexLocal)
					{
						array2[i] = newSingleValue;
					}
				}
				array = array2;
			}
			string settingsValue = string.Join(",", array);
			ActiveSliceSettings.Instance.SetValue(slicerConfigName, settingsValue, persistenceLayer);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			((GuiWidget)this).OnDraw(graphics2D);
		}
	}
}
