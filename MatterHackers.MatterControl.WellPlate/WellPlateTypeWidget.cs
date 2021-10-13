using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.WellPlate
{
	public class WellPlateTypeWidget : GuiWidget, IWellPlatePerameters
	{
		private class WellPlatePerameters : IWellPlatePerameters
		{
			public WellShape WellShape
			{
				get;
				set;
			}

			public int HorizontalWellCount
			{
				get;
				set;
			}

			public int VerticalWellCount
			{
				get;
				set;
			}

			public double HorizontalWellSpacing
			{
				get;
				set;
			}

			public double VerticalWellSpacing
			{
				get;
				set;
			}

			public double WellWidth
			{
				get;
				set;
			}

			public double WellDepth
			{
				get;
				set;
			}

			public double ZToWellBottom
			{
				get;
				set;
			}

			public Vector2 WellPlateTopLeftOffset
			{
				get;
				set;
			}

			public Vector2 WellPlateTopRightOffset
			{
				get;
				set;
			}
		}

		public EventHandler OnParametersUpdated;

		private bool isPetri;

		private WellPlateWidget wellPlateWidget;

		private WellPlate2D wellPlate2D;

		private GuiWidget configurationPanel;

		private EventHandler unregisterEvents;

		private List<IWellPlate> wellPlateVisulizatioins = new List<IWellPlate>();

		private TextImageButtonFactory buttonFactory = new TextImageButtonFactory();

		private WellPlatePerameters savedPerameters = new WellPlatePerameters();

		public WellShape WellShape
		{
			get;
			set;
		}

		public int HorizontalWellCount
		{
			get;
			set;
		}

		public int VerticalWellCount
		{
			get;
			set;
		}

		public double HorizontalWellSpacing
		{
			get;
			set;
		}

		public double VerticalWellSpacing
		{
			get;
			set;
		}

		public double WellWidth
		{
			get;
			set;
		}

		public double WellDepth
		{
			get;
			set;
		}

		public double ZToWellBottom
		{
			get;
			set;
		}

		public Vector2 WellPlateTopLeftOffset
		{
			get;
			set;
		}

		public Vector2 WellPlateTopRightOffset
		{
			get;
			set;
		}

		public WellPlateTypeWidget(WellPlateWidget wellPlateWidget, WellPlate2D wellPlate2D)
			: this()
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected O, but got Unknown
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Expected O, but got Unknown
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			this.wellPlateWidget = wellPlateWidget;
			this.wellPlate2D = wellPlate2D;
			buttonFactory.checkedBorderColor = RGBA_Bytes.White;
			GetDefaultsFromDB();
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)13);
			((GuiWidget)val).set_VAnchor((VAnchor)13);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			TextWidget val2 = new TextWidget("Cultureware Configuration".Localize(), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val2).set_HAnchor((HAnchor)2);
			((GuiWidget)val2).set_Margin(new BorderDouble(3.0));
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			Button switchToWellPlateButton = buttonFactory.Generate("Switch to Well Plate".Localize());
			((GuiWidget)switchToWellPlateButton).set_HAnchor((HAnchor)2);
			((GuiWidget)switchToWellPlateButton).set_Visible(isPetri);
			((GuiWidget)val).AddChild((GuiWidget)(object)switchToWellPlateButton, -1);
			Button configureWellPlateButton = buttonFactory.Generate("Configure Well Plate".Localize());
			((GuiWidget)configureWellPlateButton).set_HAnchor((HAnchor)2);
			((GuiWidget)configureWellPlateButton).add_Click((EventHandler<MouseEventArgs>)ShowWellPlateConfigurationPanel);
			((GuiWidget)configureWellPlateButton).set_Visible(!isPetri);
			((GuiWidget)val).AddChild((GuiWidget)(object)configureWellPlateButton, -1);
			Button switchToPetriButton = buttonFactory.Generate("Switch to Petri".Localize());
			((GuiWidget)switchToPetriButton).set_HAnchor((HAnchor)2);
			((GuiWidget)switchToPetriButton).set_Visible(!isPetri);
			((GuiWidget)val).AddChild((GuiWidget)(object)switchToPetriButton, -1);
			Button configurePetriButton = buttonFactory.Generate("Configure Petri".Localize());
			((GuiWidget)configurePetriButton).set_HAnchor((HAnchor)2);
			((GuiWidget)configurePetriButton).add_Click((EventHandler<MouseEventArgs>)ShowPetriConfigurationPanel);
			((GuiWidget)configurePetriButton).set_Visible(isPetri);
			((GuiWidget)val).AddChild((GuiWidget)(object)configurePetriButton, -1);
			((GuiWidget)switchToWellPlateButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				((GuiWidget)switchToWellPlateButton).set_Visible(false);
				((GuiWidget)configureWellPlateButton).set_Visible(true);
				((GuiWidget)switchToPetriButton).set_Visible(true);
				((GuiWidget)configurePetriButton).set_Visible(false);
				isPetri = false;
				UserSettings.Instance.set("isPetri", isPetri.ToString());
				GetDefaultsFromDB();
				ParametersUpdated();
			});
			((GuiWidget)switchToPetriButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				((GuiWidget)switchToWellPlateButton).set_Visible(true);
				((GuiWidget)configureWellPlateButton).set_Visible(false);
				((GuiWidget)switchToPetriButton).set_Visible(false);
				((GuiWidget)configurePetriButton).set_Visible(true);
				isPetri = true;
				UserSettings.Instance.set("isPetri", isPetri.ToString());
				GetDefaultsFromDB();
				ParametersUpdated();
			});
		}

		private void GetDefaultsFromDB()
		{
			if (UserSettings.Instance.get("isPetri") == null)
			{
				UserSettings.Instance.set("isPetri", false.ToString());
			}
			isPetri = bool.Parse(UserSettings.Instance.get("isPetri"));
			if (isPetri)
			{
				GetPetriDefaultsFromDB();
			}
			else
			{
				GetWellPlateDefaultsFromDB();
			}
		}

		private void GetWellPlateDefaultsFromDB()
		{
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			if (UserSettings.Instance.get("HorizontalWellCount") == null)
			{
				UserSettings.Instance.set("HorizontalWellCount", "24");
			}
			HorizontalWellCount = int.Parse(UserSettings.Instance.get("HorizontalWellCount"));
			if (UserSettings.Instance.get("VerticalWellCount") == null)
			{
				UserSettings.Instance.set("VerticalWellCount", "16");
			}
			VerticalWellCount = int.Parse(UserSettings.Instance.get("VerticalWellCount"));
			if (UserSettings.Instance.get("HorizontalWellSpacing") == null)
			{
				UserSettings.Instance.set("HorizontalWellSpacing", "4.6");
			}
			HorizontalWellSpacing = double.Parse(UserSettings.Instance.get("HorizontalWellSpacing"));
			if (UserSettings.Instance.get("VerticalWellSpacing") == null)
			{
				UserSettings.Instance.set("VerticalWellSpacing", "4.6");
			}
			VerticalWellSpacing = double.Parse(UserSettings.Instance.get("VerticalWellSpacing"));
			if (UserSettings.Instance.get("WellWidth") == null)
			{
				UserSettings.Instance.set("WellWidth", "3.5");
			}
			WellWidth = double.Parse(UserSettings.Instance.get("WellWidth"));
			if (UserSettings.Instance.get("ZToWellBottom") == null)
			{
				UserSettings.Instance.set("ZToWellBottom", "3.61");
			}
			ZToWellBottom = double.Parse(UserSettings.Instance.get("ZToWellBottom"));
			if (UserSettings.Instance.get("WellPlateTopLeftOffset") == null)
			{
				UserSettings.Instance.set("WellPlateTopLeftOffset", "100,110");
			}
			string[] array = UserSettings.Instance.get("WellPlateTopLeftOffset").Split(new char[1]
			{
				','
			});
			WellPlateTopLeftOffset = new Vector2(double.Parse(array[0]), double.Parse(array[1]));
			if (UserSettings.Instance.get("WellPlateTopRightOffset") == null)
			{
				UserSettings.Instance.set("WellPlateTopRightOffset", "110,110");
			}
			string[] array2 = UserSettings.Instance.get("WellPlateTopRightOffset").Split(new char[1]
			{
				','
			});
			WellPlateTopRightOffset = new Vector2(double.Parse(array2[0]), double.Parse(array2[1]));
			if (UserSettings.Instance.get("WellShape") == null)
			{
				UserSettings.Instance.set("WellShape", WellShape.SQUARE.ToString());
			}
			WellShape = (WellShape)Enum.Parse(typeof(WellShape), UserSettings.Instance.get("WellShape"));
		}

		private void GetPetriDefaultsFromDB()
		{
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			HorizontalWellCount = 1;
			VerticalWellCount = 1;
			HorizontalWellSpacing = 1.0;
			VerticalWellSpacing = 1.0;
			if (UserSettings.Instance.get("PetriWidth") == null)
			{
				UserSettings.Instance.set("PetriWidth", "50");
			}
			WellWidth = double.Parse(UserSettings.Instance.get("PetriWidth"));
			if (UserSettings.Instance.get("PetriBottomThick") == null)
			{
				UserSettings.Instance.set("PetriBottomThick", "3.61");
			}
			ZToWellBottom = double.Parse(UserSettings.Instance.get("PetriBottomThick"));
			if (UserSettings.Instance.get("PetriOffset") == null)
			{
				UserSettings.Instance.set("PetriOffset", "150,90");
			}
			string[] array = UserSettings.Instance.get("PetriOffset").Split(new char[1]
			{
				','
			});
			WellPlateTopLeftOffset = new Vector2(double.Parse(array[0]), double.Parse(array[1]));
			WellPlateTopRightOffset = new Vector2(WellPlateTopLeftOffset.x + 1.0, WellPlateTopLeftOffset.y);
			WellShape = WellShape.CIRCLE;
		}

		private void SetDefaultsInDB()
		{
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			UserSettings.Instance.set("isPetri", isPetri.ToString());
			if (isPetri)
			{
				UserSettings.Instance.set("PetriWidth", WellWidth.ToString());
				UserSettings.Instance.set("PetriBottomThick", ZToWellBottom.ToString());
				UserSettings.Instance.set("PetriOffset", StringHelper.FormatWith("{0},{1}", new object[2]
				{
					WellPlateTopLeftOffset.x,
					WellPlateTopLeftOffset.y
				}));
				return;
			}
			UserSettings.Instance.set("HorizontalWellCount", HorizontalWellCount.ToString());
			UserSettings.Instance.set("VerticalWellCount", VerticalWellCount.ToString());
			UserSettings.Instance.set("HorizontalWellSpacing", HorizontalWellSpacing.ToString());
			UserSettings.Instance.set("VerticalWellSpacing", VerticalWellSpacing.ToString());
			UserSettings.Instance.set("WellWidth", WellWidth.ToString());
			UserSettings.Instance.set("ZToWellBottom", ZToWellBottom.ToString());
			UserSettings.Instance.set("WellPlateTopLeftOffset", StringHelper.FormatWith("{0},{1}", new object[2]
			{
				WellPlateTopLeftOffset.x,
				WellPlateTopLeftOffset.y
			}));
			UserSettings.Instance.set("WellPlateTopRightOffset", StringHelper.FormatWith("{0},{1}", new object[2]
			{
				WellPlateTopRightOffset.x,
				WellPlateTopRightOffset.y
			}));
			UserSettings.Instance.set("WellShape", WellShape.ToString());
		}

		public void AddWellPlate(IWellPlate wellPlate)
		{
			wellPlateVisulizatioins.Add(wellPlate);
			CopyConfig(this, wellPlate);
			wellPlate.IsPetri = isPetri;
			wellPlate.ParametersUpdated();
		}

		public void ParametersUpdated()
		{
			foreach (IWellPlate wellPlateVisulizatioin in wellPlateVisulizatioins)
			{
				CopyConfig(this, wellPlateVisulizatioin);
				wellPlateVisulizatioin.IsPetri = isPetri;
				wellPlateVisulizatioin.ParametersUpdated();
			}
			OnParametersUpdated?.Invoke(this, null);
		}

		private void CreateWellPlateConfigurationPanel()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Expected O, but got Unknown
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Expected O, but got Unknown
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Expected O, but got Unknown
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Expected O, but got Unknown
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Expected O, but got Unknown
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Expected O, but got Unknown
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Expected O, but got Unknown
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Expected O, but got Unknown
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Expected O, but got Unknown
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Expected O, but got Unknown
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_0559: Unknown result type (might be due to invalid IL or missing references)
			//IL_055f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_0567: Unknown result type (might be due to invalid IL or missing references)
			//IL_0574: Expected O, but got Unknown
			//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Expected O, but got Unknown
			//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bd: Expected O, but got Unknown
			//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0636: Unknown result type (might be due to invalid IL or missing references)
			//IL_063f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0645: Unknown result type (might be due to invalid IL or missing references)
			//IL_0648: Unknown result type (might be due to invalid IL or missing references)
			//IL_064d: Unknown result type (might be due to invalid IL or missing references)
			//IL_065a: Expected O, but got Unknown
			//IL_0691: Unknown result type (might be due to invalid IL or missing references)
			//IL_0698: Expected O, but got Unknown
			//IL_0751: Unknown result type (might be due to invalid IL or missing references)
			//IL_0758: Expected O, but got Unknown
			configurationPanel = new GuiWidget();
			configurationPanel.AnchorAll();
			configurationPanel.set_BackgroundColor(new RGBA_Bytes(0, 0, 0, 100));
			FlowLayoutWidget configurationBox = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)configurationBox).AnchorCenter();
			((GuiWidget)configurationBox).set_VAnchor((VAnchor)10);
			((GuiWidget)configurationBox).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)configurationBox).set_Width(((GuiWidget)wellPlateWidget).get_Parent().get_Width() / 2.0);
			((GuiWidget)configurationBox).set_Padding(new BorderDouble(4.0));
			((GuiWidget)wellPlateWidget).get_Parent().add_BoundsChanged((EventHandler)delegate
			{
				double num = 300.0;
				double num2 = ((GuiWidget)wellPlateWidget).get_Parent().get_Width() / 2.0;
				((GuiWidget)configurationBox).set_Width((num2 < num) ? num : num2);
			});
			configurationPanel.AddChild((GuiWidget)(object)configurationBox, -1);
			TextWidget val = new TextWidget("Well Plate Configuration".Localize(), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val).set_HAnchor((HAnchor)2);
			((GuiWidget)configurationBox).AddChild((GuiWidget)(object)val, -1);
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)configurationBox).AddChild((GuiWidget)(object)val2, -1);
			SettingRowFactory settingRowFactory = new SettingRowFactory();
			MHNumberEdit horizontalWellCountEdit = settingRowFactory.MakeIntEdit(HorizontalWellCount, "Number of wells in a row".Localize());
			((TextEditWidget)horizontalWellCountEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				HorizontalWellCount = (int)horizontalWellCountEdit.ActuallNumberEdit.get_Value();
			});
			((GuiWidget)val2).AddChild((GuiWidget)(object)settingRowFactory.NewRowWithLablesAndEdit("Horizontal Well Count", (GuiWidget)(object)horizontalWellCountEdit, ""), -1);
			MHNumberEdit verticalWellCountEdit = settingRowFactory.MakeIntEdit(VerticalWellCount, "Number of wells in a column".Localize());
			((TextEditWidget)verticalWellCountEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				VerticalWellCount = (int)verticalWellCountEdit.ActuallNumberEdit.get_Value();
			});
			((GuiWidget)val2).AddChild((GuiWidget)(object)settingRowFactory.NewRowWithLablesAndEdit("Vertical Well Count", (GuiWidget)(object)verticalWellCountEdit, ""), -1);
			MHNumberEdit wellSpacingEdit = settingRowFactory.MakeDoubleEdit(HorizontalWellSpacing, "Distance between well centers".Localize());
			((TextEditWidget)wellSpacingEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				HorizontalWellSpacing = wellSpacingEdit.ActuallNumberEdit.get_Value();
				VerticalWellSpacing = wellSpacingEdit.ActuallNumberEdit.get_Value();
			});
			((GuiWidget)val2).AddChild((GuiWidget)(object)settingRowFactory.NewRowWithLablesAndEdit("Well Spacing", (GuiWidget)(object)wellSpacingEdit, "mm"), -1);
			MHNumberEdit wellWidthEdit = settingRowFactory.MakeDoubleEdit(WellWidth, "Width of a single well".Localize());
			((TextEditWidget)wellWidthEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				WellWidth = wellWidthEdit.ActuallNumberEdit.get_Value();
			});
			((GuiWidget)val2).AddChild((GuiWidget)(object)settingRowFactory.NewRowWithLablesAndEdit("Well Width", (GuiWidget)(object)wellWidthEdit, "mm"), -1);
			MHNumberEdit zHeightEdit = settingRowFactory.MakeDoubleEdit(ZToWellBottom, "Z height to bottom of well".Localize());
			((TextEditWidget)zHeightEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				ZToWellBottom = zHeightEdit.ActuallNumberEdit.get_Value();
			});
			((GuiWidget)val2).AddChild((GuiWidget)(object)settingRowFactory.NewRowWithLablesAndEdit("Well Bottom Height", (GuiWidget)(object)zHeightEdit, "mm"), -1);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)3);
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val4, -1);
			MHNumberEdit topLeftXOffsetEdit = settingRowFactory.MakeDoubleEdit(WellPlateTopLeftOffset.x);
			((TextEditWidget)topLeftXOffsetEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				Vector2 wellPlateTopLeftOffset2 = WellPlateTopLeftOffset;
				WellPlateTopLeftOffset = new Vector2(topLeftXOffsetEdit.ActuallNumberEdit.get_Value(), WellPlateTopLeftOffset.y);
				if (WellPlateTopLeftOffset == WellPlateTopRightOffset)
				{
					WellPlateTopLeftOffset = wellPlateTopLeftOffset2;
					topLeftXOffsetEdit.ActuallNumberEdit.set_Value(wellPlateTopLeftOffset2.x);
				}
			});
			TextWidget val5 = new TextWidget("x".Localize() + " ", 0.0, 0.0, 8.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val5).set_VAnchor((VAnchor)2);
			((GuiWidget)val4).AddChild((GuiWidget)val5, -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)topLeftXOffsetEdit, -1);
			((GuiWidget)val4).AddChild(settingRowFactory.MakeUnits("mm"), -1);
			((GuiWidget)val3).AddChild(new GuiWidget(1.0, 2.0, (SizeLimitsToSet)1), -1);
			FlowLayoutWidget val6 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val6, -1);
			MHNumberEdit topLeftYOffsetEdit = settingRowFactory.MakeDoubleEdit(WellPlateTopLeftOffset.y);
			((TextEditWidget)topLeftYOffsetEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				Vector2 wellPlateTopLeftOffset = WellPlateTopLeftOffset;
				WellPlateTopLeftOffset = new Vector2(WellPlateTopLeftOffset.x, topLeftYOffsetEdit.ActuallNumberEdit.get_Value());
				if (WellPlateTopLeftOffset == WellPlateTopRightOffset)
				{
					WellPlateTopLeftOffset = wellPlateTopLeftOffset;
					topLeftYOffsetEdit.ActuallNumberEdit.set_Value(wellPlateTopLeftOffset.y);
				}
			});
			TextWidget val7 = new TextWidget("y".Localize() + " ", 0.0, 0.0, 8.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val7).set_VAnchor((VAnchor)2);
			((GuiWidget)val6).AddChild((GuiWidget)val7, -1);
			((GuiWidget)val6).AddChild((GuiWidget)(object)topLeftYOffsetEdit, -1);
			((GuiWidget)val6).AddChild(settingRowFactory.MakeUnits("mm"), -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)settingRowFactory.NewRowWithLablesAndEdit("Top Left Well Location", (GuiWidget)(object)val3), -1);
			FlowLayoutWidget val8 = new FlowLayoutWidget((FlowDirection)3);
			FlowLayoutWidget val9 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val8).AddChild((GuiWidget)(object)val9, -1);
			MHNumberEdit topRightXOffsetEdit = settingRowFactory.MakeDoubleEdit(WellPlateTopRightOffset.x);
			((TextEditWidget)topRightXOffsetEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				Vector2 wellPlateTopRightOffset2 = WellPlateTopRightOffset;
				WellPlateTopRightOffset = new Vector2(topRightXOffsetEdit.ActuallNumberEdit.get_Value(), WellPlateTopRightOffset.y);
				if (WellPlateTopLeftOffset == WellPlateTopRightOffset)
				{
					WellPlateTopRightOffset = wellPlateTopRightOffset2;
					topRightXOffsetEdit.ActuallNumberEdit.set_Value(wellPlateTopRightOffset2.x);
				}
			});
			TextWidget val10 = new TextWidget("x".Localize() + " ", 0.0, 0.0, 8.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val10).set_VAnchor((VAnchor)2);
			((GuiWidget)val9).AddChild((GuiWidget)val10, -1);
			((GuiWidget)val9).AddChild((GuiWidget)(object)topRightXOffsetEdit, -1);
			((GuiWidget)val9).AddChild(settingRowFactory.MakeUnits("mm"), -1);
			((GuiWidget)val8).AddChild(new GuiWidget(1.0, 2.0, (SizeLimitsToSet)1), -1);
			FlowLayoutWidget val11 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val8).AddChild((GuiWidget)(object)val11, -1);
			MHNumberEdit topRightYOffsetEdit = settingRowFactory.MakeDoubleEdit(WellPlateTopRightOffset.y);
			((TextEditWidget)topRightYOffsetEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				Vector2 wellPlateTopRightOffset = WellPlateTopRightOffset;
				WellPlateTopRightOffset = new Vector2(WellPlateTopRightOffset.x, topRightYOffsetEdit.ActuallNumberEdit.get_Value());
				if (WellPlateTopLeftOffset == WellPlateTopRightOffset)
				{
					WellPlateTopRightOffset = wellPlateTopRightOffset;
					topRightYOffsetEdit.ActuallNumberEdit.set_Value(wellPlateTopRightOffset.y);
				}
			});
			TextWidget val12 = new TextWidget("y".Localize() + " ", 0.0, 0.0, 8.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val12).set_VAnchor((VAnchor)2);
			((GuiWidget)val11).AddChild((GuiWidget)val12, -1);
			((GuiWidget)val11).AddChild((GuiWidget)(object)topRightYOffsetEdit, -1);
			((GuiWidget)val11).AddChild(settingRowFactory.MakeUnits("mm"), -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)settingRowFactory.NewRowWithLablesAndEdit("Top Right Well Location", (GuiWidget)(object)val8), -1);
			FlowLayoutWidget val13 = new FlowLayoutWidget((FlowDirection)0);
			RadioButton circleButton = buttonFactory.GenerateRadioButton(" " + "circle".Localize() + " ");
			RadioButton val14 = buttonFactory.GenerateRadioButton(" " + "square".Localize() + " ");
			((GuiWidget)val13).AddChild((GuiWidget)(object)circleButton, -1);
			((GuiWidget)val13).AddChild((GuiWidget)(object)val14, -1);
			if (WellShape == WellShape.CIRCLE)
			{
				circleButton.set_Checked(true);
			}
			else
			{
				val14.set_Checked(true);
			}
			circleButton.add_CheckedStateChanged((EventHandler)delegate
			{
				if (circleButton.get_Checked())
				{
					WellShape = WellShape.CIRCLE;
				}
				else
				{
					WellShape = WellShape.SQUARE;
				}
			});
			((GuiWidget)val2).AddChild((GuiWidget)(object)settingRowFactory.NewRowWithLablesAndEdit("Well Shape", (GuiWidget)(object)val13, ""), -1);
			FlowLayoutWidget val15 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val15).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val15, -1);
			Button val16 = buttonFactory.Generate("Set As Default".Localize());
			((GuiWidget)val16).add_Click((EventHandler<MouseEventArgs>)delegate(object sender, MouseEventArgs e)
			{
				SetDefaultsInDB();
				FinalizeConfiguration(sender, (EventArgs)(object)e);
			});
			((GuiWidget)val15).AddChild((GuiWidget)(object)val16, -1);
			((GuiWidget)val15).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			Button val17 = buttonFactory.Generate("Cancel".Localize());
			((GuiWidget)val17).add_Click((EventHandler<MouseEventArgs>)CancelConfiguration);
			((GuiWidget)val15).AddChild((GuiWidget)(object)val17, -1);
			Button val18 = buttonFactory.Generate("OK".Localize());
			((GuiWidget)val18).add_Click((EventHandler<MouseEventArgs>)FinalizeConfiguration);
			((GuiWidget)val15).AddChild((GuiWidget)(object)val18, -1);
		}

		private void ShowWellPlateConfigurationPanel(object sender, EventArgs e)
		{
			CopyConfig(this, savedPerameters);
			CreateWellPlateConfigurationPanel();
			wellPlateWidget.DoHighlighting = false;
			((GuiWidget)wellPlateWidget).get_Parent().AddChild(configurationPanel, -1);
		}

		private void CreatePetriConfigurationPanel()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Expected O, but got Unknown
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Expected O, but got Unknown
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Expected O, but got Unknown
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Expected O, but got Unknown
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Expected O, but got Unknown
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Expected O, but got Unknown
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Expected O, but got Unknown
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Expected O, but got Unknown
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Expected O, but got Unknown
			configurationPanel = new GuiWidget();
			configurationPanel.AnchorAll();
			configurationPanel.set_BackgroundColor(new RGBA_Bytes(0, 0, 0, 100));
			FlowLayoutWidget configurationBox = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)configurationBox).AnchorCenter();
			((GuiWidget)configurationBox).set_VAnchor((VAnchor)10);
			((GuiWidget)configurationBox).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)configurationBox).set_Width(((GuiWidget)wellPlateWidget).get_Parent().get_Width() / 2.0);
			((GuiWidget)configurationBox).set_Padding(new BorderDouble(4.0));
			((GuiWidget)wellPlateWidget).get_Parent().add_BoundsChanged((EventHandler)delegate
			{
				double num = 300.0;
				double num2 = ((GuiWidget)wellPlateWidget).get_Parent().get_Width() / 2.0;
				((GuiWidget)configurationBox).set_Width((num2 < num) ? num : num2);
			});
			configurationPanel.AddChild((GuiWidget)(object)configurationBox, -1);
			TextWidget val = new TextWidget("Petri Configuration".Localize(), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val).set_HAnchor((HAnchor)2);
			((GuiWidget)configurationBox).AddChild((GuiWidget)(object)val, -1);
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)configurationBox).AddChild((GuiWidget)(object)val2, -1);
			SettingRowFactory settingRowFactory = new SettingRowFactory();
			MHNumberEdit petriDiameterEdit = settingRowFactory.MakeDoubleEdit(WellWidth, "Diameter of the petri dish".Localize());
			((TextEditWidget)petriDiameterEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				WellWidth = petriDiameterEdit.ActuallNumberEdit.get_Value();
			});
			((GuiWidget)val2).AddChild((GuiWidget)(object)settingRowFactory.NewRowWithLablesAndEdit("Petri Diameter", (GuiWidget)(object)petriDiameterEdit, "mm"), -1);
			MHNumberEdit bottomThicknessEdit = settingRowFactory.MakeDoubleEdit(ZToWellBottom, "Z height to print the first layer".Localize());
			((TextEditWidget)bottomThicknessEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				ZToWellBottom = bottomThicknessEdit.ActuallNumberEdit.get_Value();
			});
			((GuiWidget)val2).AddChild((GuiWidget)(object)settingRowFactory.NewRowWithLablesAndEdit("Petri Bottom Height", (GuiWidget)(object)bottomThicknessEdit, "mm"), -1);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)3);
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val4, -1);
			MHNumberEdit xOffsetEdit = settingRowFactory.MakeDoubleEdit(WellPlateTopLeftOffset.x);
			((TextEditWidget)xOffsetEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				Vector2 wellPlateTopLeftOffset2 = WellPlateTopLeftOffset;
				WellPlateTopLeftOffset = new Vector2(xOffsetEdit.ActuallNumberEdit.get_Value(), WellPlateTopLeftOffset.y);
				if (WellPlateTopLeftOffset == WellPlateTopRightOffset)
				{
					WellPlateTopLeftOffset = wellPlateTopLeftOffset2;
					xOffsetEdit.ActuallNumberEdit.set_Value(wellPlateTopLeftOffset2.x);
				}
			});
			TextWidget val5 = new TextWidget("x".Localize() + " ", 0.0, 0.0, 8.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val5).set_VAnchor((VAnchor)2);
			((GuiWidget)val4).AddChild((GuiWidget)val5, -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)xOffsetEdit, -1);
			((GuiWidget)val4).AddChild(settingRowFactory.MakeUnits("mm"), -1);
			((GuiWidget)val3).AddChild(new GuiWidget(1.0, 2.0, (SizeLimitsToSet)1), -1);
			FlowLayoutWidget val6 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val6, -1);
			MHNumberEdit yOffsetEdit = settingRowFactory.MakeDoubleEdit(WellPlateTopLeftOffset.y);
			((TextEditWidget)yOffsetEdit.ActuallNumberEdit).add_EditComplete((EventHandler)delegate
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				Vector2 wellPlateTopLeftOffset = WellPlateTopLeftOffset;
				WellPlateTopLeftOffset = new Vector2(WellPlateTopLeftOffset.x, yOffsetEdit.ActuallNumberEdit.get_Value());
				if (WellPlateTopLeftOffset == WellPlateTopRightOffset)
				{
					WellPlateTopLeftOffset = wellPlateTopLeftOffset;
					yOffsetEdit.ActuallNumberEdit.set_Value(wellPlateTopLeftOffset.y);
				}
			});
			TextWidget val7 = new TextWidget("y".Localize() + " ", 0.0, 0.0, 8.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val7).set_VAnchor((VAnchor)2);
			((GuiWidget)val6).AddChild((GuiWidget)val7, -1);
			((GuiWidget)val6).AddChild((GuiWidget)(object)yOffsetEdit, -1);
			((GuiWidget)val6).AddChild(settingRowFactory.MakeUnits("mm"), -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)settingRowFactory.NewRowWithLablesAndEdit("Petri Center Location", (GuiWidget)(object)val3), -1);
			FlowLayoutWidget val8 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val8).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val8, -1);
			Button val9 = buttonFactory.Generate("Set As Default".Localize());
			((GuiWidget)val9).add_Click((EventHandler<MouseEventArgs>)delegate(object sender, MouseEventArgs e)
			{
				SetDefaultsInDB();
				FinalizeConfiguration(sender, (EventArgs)(object)e);
			});
			((GuiWidget)val8).AddChild((GuiWidget)(object)val9, -1);
			((GuiWidget)val8).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			Button val10 = buttonFactory.Generate("Cancel".Localize());
			((GuiWidget)val10).add_Click((EventHandler<MouseEventArgs>)CancelConfiguration);
			((GuiWidget)val8).AddChild((GuiWidget)(object)val10, -1);
			Button val11 = buttonFactory.Generate("OK".Localize());
			((GuiWidget)val11).add_Click((EventHandler<MouseEventArgs>)FinalizeConfiguration);
			((GuiWidget)val8).AddChild((GuiWidget)(object)val11, -1);
		}

		private void ShowPetriConfigurationPanel(object sender, EventArgs e)
		{
			CopyConfig(this, savedPerameters);
			CreatePetriConfigurationPanel();
			wellPlateWidget.DoHighlighting = false;
			((GuiWidget)wellPlateWidget).get_Parent().AddChild(configurationPanel, -1);
		}

		private void FinalizeConfiguration(object sender, EventArgs e)
		{
			ParametersUpdated();
			((GuiWidget)wellPlateWidget).get_Parent().RemoveChild(configurationPanel);
			wellPlateWidget.DoHighlighting = true;
		}

		private void CancelConfiguration(object sender, EventArgs e)
		{
			CopyConfig(savedPerameters, this);
			((GuiWidget)wellPlateWidget).get_Parent().RemoveChild(configurationPanel);
			wellPlateWidget.DoHighlighting = true;
		}

		private static void CopyConfig(IWellPlatePerameters from, IWellPlatePerameters to)
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			to.WellShape = from.WellShape;
			to.HorizontalWellCount = from.HorizontalWellCount;
			to.VerticalWellCount = from.VerticalWellCount;
			to.HorizontalWellSpacing = from.HorizontalWellSpacing;
			to.VerticalWellSpacing = from.VerticalWellSpacing;
			to.WellWidth = from.WellWidth;
			to.WellDepth = from.WellDepth;
			to.ZToWellBottom = from.ZToWellBottom;
			to.WellPlateTopLeftOffset = from.WellPlateTopLeftOffset;
			to.WellPlateTopRightOffset = from.WellPlateTopRightOffset;
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
		}
	}
}
