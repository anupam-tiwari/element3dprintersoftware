using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using MatterHackers.Agg;
using MatterHackers.Agg.PlatformAbstract;
using Microsoft.Win32.SafeHandles;

namespace MatterHackers.MatterControl.FrostedSerial
{
	public class FrostedSerialPortFactory
	{
		private static Dictionary<string, FrostedSerialPortFactory> availableFactories = new Dictionary<string, FrostedSerialPortFactory>();

		public bool IsWindows
		{
			get
			{
				PlatformID platform = Environment.OSVersion.Platform;
				if (platform != PlatformID.Win32Windows)
				{
					return platform == PlatformID.Win32NT;
				}
				return true;
			}
		}

		[DllImport("SetSerial", SetLastError = true)]
		private static extern int set_baud(string portName, int baud_rate);

		public static FrostedSerialPortFactory GetAppropriateFactory(string driverType)
		{
			lock (availableFactories)
			{
				try
				{
					if (availableFactories.Count == 0)
					{
						availableFactories.Add("Raw", new FrostedSerialPortFactory());
						foreach (FrostedSerialPortFactory plugin in new PluginFinder<FrostedSerialPortFactory>((string)null, (IComparer<FrostedSerialPortFactory>)null).Plugins)
						{
							availableFactories.Add(plugin.GetDriverType(), plugin);
						}
						if (!availableFactories.ContainsKey("RepRap"))
						{
							availableFactories.Add("RepRap", new FrostedSerialPortFactory());
						}
					}
					if (!string.IsNullOrEmpty(driverType) && availableFactories.ContainsKey(driverType))
					{
						return availableFactories[driverType];
					}
					return availableFactories["RepRap"];
				}
				catch
				{
					return new FrostedSerialPortFactory();
				}
			}
		}

		protected virtual string GetDriverType()
		{
			return "RepRap";
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr securityAttrs, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

		public virtual bool SerialPortAlreadyOpen(string portName)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			if ((int)OsInformation.get_OperatingSystem() == 1)
			{
				using (SafeFileHandle safeFileHandle = CreateFile("\\\\.\\" + portName, -1073741824, 0, IntPtr.Zero, 3, 1073741824, IntPtr.Zero))
				{
					safeFileHandle.Close();
					return safeFileHandle.IsInvalid;
				}
			}
			return false;
		}

		protected FrostedSerialPortFactory()
		{
		}

		public virtual IFrostedSerialPort Create(string serialPortName)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Invalid comparison between Unknown and I4
			IFrostedSerialPort frostedSerialPort = null;
			string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if ((int)OsInformation.get_OperatingSystem() == 2 && File.Exists(Path.Combine(directoryName, "libFrostedSerialHelper.dylib")))
			{
				return new FrostedSerialPort(serialPortName);
			}
			return new CSharpSerialPortWrapper(serialPortName);
		}

		public virtual IFrostedSerialPort CreateAndOpen(string serialPortName, int baudRate, bool DtrEnableOnConnect)
		{
			IFrostedSerialPort frostedSerialPort = Create(serialPortName);
			bool num = !(frostedSerialPort is FrostedSerialPort) && !IsWindows && baudRate > 115200;
			if (!num)
			{
				frostedSerialPort.BaudRate = baudRate;
			}
			if (DtrEnableOnConnect)
			{
				frostedSerialPort.DtrEnable = true;
			}
			frostedSerialPort.ReadTimeout = 500;
			frostedSerialPort.WriteTimeout = 500;
			frostedSerialPort.Open();
			if (num)
			{
				set_baud(serialPortName, baudRate);
			}
			return frostedSerialPort;
		}
	}
}
