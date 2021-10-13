using System;
using System.Collections.ObjectModel;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class PresetsContext
	{
		public ObservableCollection<PrinterSettingsLayer> PresetLayers
		{
			get;
		}

		public PrinterSettingsLayer PersistenceLayer
		{
			get;
			set;
		}

		public Action<string> SetAsActive
		{
			get;
			set;
		}

		public Action DeleteLayer
		{
			get;
			set;
		}

		public NamedSettingsLayers LayerType
		{
			get;
			set;
		}

		public PresetsContext(ObservableCollection<PrinterSettingsLayer> settingsLayers, PrinterSettingsLayer activeLayer)
		{
			PersistenceLayer = activeLayer;
			PresetLayers = settingsLayers;
		}
	}
}
