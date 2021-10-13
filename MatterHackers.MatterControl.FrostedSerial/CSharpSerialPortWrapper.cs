using System;
using System.ComponentModel;
using System.IO.Ports;
using MatterHackers.SerialPortCommunication.FrostedSerial;

namespace MatterHackers.MatterControl.FrostedSerial
{
	public class CSharpSerialPortWrapper : IFrostedSerialPort
	{
		private SerialPort port;

		public int ReadTimeout
		{
			get
			{
				return port.get_ReadTimeout();
			}
			set
			{
				port.set_ReadTimeout(value);
			}
		}

		public int BytesToRead => port.get_BytesToRead();

		public bool IsOpen => port.get_IsOpen();

		public int WriteTimeout
		{
			get
			{
				return port.get_WriteTimeout();
			}
			set
			{
				port.set_WriteTimeout(value);
			}
		}

		public int BaudRate
		{
			get
			{
				return port.get_BaudRate();
			}
			set
			{
				port.set_BaudRate(value);
			}
		}

		public bool RtsEnable
		{
			get
			{
				return port.get_RtsEnable();
			}
			set
			{
				port.set_RtsEnable(value);
			}
		}

		public bool DtrEnable
		{
			get
			{
				return port.get_DtrEnable();
			}
			set
			{
				port.set_DtrEnable(value);
			}
		}

		internal CSharpSerialPortWrapper(string serialPortName)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			if (FrostedSerialPortFactory.GetAppropriateFactory("RepRap").IsWindows)
			{
				try
				{
					SerialPortFixer.Execute(serialPortName);
				}
				catch (Exception)
				{
				}
			}
			port = new SerialPort(serialPortName);
		}

		public string ReadExisting()
		{
			return port.ReadExisting();
		}

		public void Dispose()
		{
			((Component)port).Dispose();
		}

		public void Open()
		{
			port.Open();
		}

		public void Close()
		{
			try
			{
				port.Close();
			}
			catch (Exception)
			{
			}
		}

		public void Write(string str)
		{
			port.Write(str);
		}

		public void Write(byte[] buffer, int offset, int count)
		{
			port.Write(buffer, offset, count);
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			return port.Read(buffer, offset, count);
		}
	}
}
