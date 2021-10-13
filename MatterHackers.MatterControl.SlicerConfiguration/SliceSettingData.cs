using System.Collections.Generic;
using MatterHackers.Localizations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class SliceSettingData
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public enum DataEditTypes
		{
			TOOL,
			STRING,
			INT,
			INT_OR_MM,
			DOUBLE,
			POSITIVE_DOUBLE,
			OFFSET,
			DOUBLE_OR_PERCENT,
			VECTOR2,
			OFFSET2,
			CHECK_BOX,
			LIST,
			MULTI_LINE_TEXT,
			HARDWARE_PRESENT,
			COM_PORT
		}

		public List<QuickMenuNameValue> QuickMenuSettings = new List<QuickMenuNameValue>();

		public List<Dictionary<string, string>> SetSettingsOnChange = new List<Dictionary<string, string>>();

		public string SlicerConfigName
		{
			get;
			set;
		}

		public string PresentationName
		{
			get;
			set;
		}

		public string ShowIfSet
		{
			get;
			set;
		}

		public string DefaultValue
		{
			get;
			set;
		}

		public DataEditTypes DataEditType
		{
			get;
			set;
		}

		public string HelpText
		{
			get;
			set;
		} = "";


		public string ExtraSettings
		{
			get;
			set;
		} = "";


		public bool ShowAsOverride
		{
			get;
			set;
		} = true;


		public bool ResetAtEndOfPrint
		{
			get;
			set;
		}

		public bool RebuildGCodeOnChange
		{
			get;
			set;
		} = true;


		public bool ReloadUiWhenChanged
		{
			get;
			set;
		}

		public SliceSettingData(string slicerConfigName, string presentationName, DataEditTypes dataEditType, string extraSettings = "", string helpText = "")
		{
			ExtraSettings = extraSettings ?? "";
			SlicerConfigName = slicerConfigName;
			PresentationName = presentationName;
			DataEditType = dataEditType;
			HelpText = LocalizedString.Get(helpText);
		}
	}
}
