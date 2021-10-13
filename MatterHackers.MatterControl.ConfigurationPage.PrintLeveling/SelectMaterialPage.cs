using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class SelectMaterialPage : InstructionsPage
	{
		public SelectMaterialPage(string pageDescription, string instructionsText)
			: base(pageDescription, instructionsText)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			PresetSelectorWidget presetSelectorWidget = new PresetSelectorWidget(string.Format(string.Format("{0} {1}", "Material".Localize(), num + 1)), RGBA_Bytes.Transparent, NamedSettingsLayers.Material, num);
			((GuiWidget)presetSelectorWidget).set_BackgroundColor(RGBA_Bytes.Transparent);
			((GuiWidget)presetSelectorWidget).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 15.0));
			((GuiWidget)topToBottomControls).AddChild((GuiWidget)(object)presetSelectorWidget, -1);
		}
	}
}
