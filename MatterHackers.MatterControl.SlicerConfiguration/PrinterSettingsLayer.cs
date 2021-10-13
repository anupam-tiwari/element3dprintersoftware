using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class PrinterSettingsLayer : Dictionary<string, string>
	{
		public string LayerID
		{
			get
			{
				string text = ValueOrDefault("layer_id");
				if (string.IsNullOrEmpty(text))
				{
					text = (LayerID = Guid.NewGuid().ToString());
				}
				return text;
			}
			set
			{
				base["layer_id"] = value;
			}
		}

		public string Name
		{
			get
			{
				return ValueOrDefault("layer_name");
			}
			set
			{
				base["layer_name"] = value;
			}
		}

		public string Source
		{
			get
			{
				return ValueOrDefault("layer_source");
			}
			set
			{
				base["layer_source"] = value;
			}
		}

		public string ETag
		{
			get
			{
				return ValueOrDefault("layer_etag");
			}
			set
			{
				base["layer_etag"] = value;
			}
		}

		public PrinterSettingsLayer()
		{
		}

		public PrinterSettingsLayer(Dictionary<string, string> settingsDictionary)
		{
			foreach (KeyValuePair<string, string> item in settingsDictionary)
			{
				base[item.Key] = item.Value;
			}
		}

		public string ValueOrDefault(string key, string defaultValue = "")
		{
			TryGetValue(key, out var value);
			return value ?? defaultValue;
		}

		public static PrinterSettingsLayer LoadFromIni(TextReader reader)
		{
			PrinterSettingsLayer printerSettingsLayer = new PrinterSettingsLayer();
			string text;
			while ((text = reader.ReadLine()) != null)
			{
				string[] array = text.Split(new char[1]
				{
					'='
				});
				if (!text.StartsWith("#") && !string.IsNullOrEmpty(text))
				{
					string key = array[0].Trim();
					printerSettingsLayer[key] = array[1].Trim();
				}
			}
			return printerSettingsLayer;
		}

		public static PrinterSettingsLayer LoadFromIni(string filePath)
		{
			var enumerable = Enumerable.Select(Enumerable.Where(Enumerable.Select((IEnumerable<string>)File.ReadAllLines(filePath), (string line) => new
			{
				line = line,
				segments = line.Split(new char[1]
				{
					'='
				})
			}), _003C_003Eh__TransparentIdentifier0 => !_003C_003Eh__TransparentIdentifier0.line.StartsWith("#") && !string.IsNullOrEmpty(_003C_003Eh__TransparentIdentifier0.line) && _003C_003Eh__TransparentIdentifier0.segments.Length == 2), _003C_003Eh__TransparentIdentifier0 => new
			{
				Key = _003C_003Eh__TransparentIdentifier0.segments[0].Trim(),
				Value = _003C_003Eh__TransparentIdentifier0.segments[1].Trim()
			});
			PrinterSettingsLayer printerSettingsLayer = new PrinterSettingsLayer();
			foreach (var item in enumerable)
			{
				printerSettingsLayer[item.Key] = item.Value;
			}
			return printerSettingsLayer;
		}

		public PrinterSettingsLayer Clone()
		{
			string layerID = Guid.NewGuid().ToString();
			return new PrinterSettingsLayer(this)
			{
				LayerID = layerID,
				Name = Name,
				ETag = ETag,
				Source = Source
			};
		}
	}
}
