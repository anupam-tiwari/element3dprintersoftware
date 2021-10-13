using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MatterHackers.Agg.PlatformAbstract;
using Microsoft.Win32;

namespace MatterHackers.MatterControl.FrostedSerial
{
	[MonitoringDescription("")]
	[DesignerCategory("")]
	public class FrostedSerialPort : Component, IFrostedSerialPort
	{
		public const int InfiniteTimeout = -1;

		private const int DefaultReadBufferSize = 4096;

		private const int DefaultWriteBufferSize = 2048;

		private const int DefaultBaudRate = 9600;

		private const int DefaultDataBits = 8;

		private const Parity DefaultParity = Parity.None;

		private const StopBits DefaultStopBits = StopBits.One;

		private bool is_open;

		private int baud_rate;

		private Parity parity;

		private StopBits stop_bits;

		private Handshake handshake;

		private int data_bits;

		private bool break_state;

		private bool dtr_enable;

		private bool rts_enable;

		private IFrostedSerialStream stream;

		private Encoding encoding = Encoding.ASCII;

		private string new_line = Environment.NewLine;

		private string port_name;

		private int read_timeout = -1;

		private int write_timeout = -1;

		private int readBufferSize = 4096;

		private int writeBufferSize = 2048;

		private object error_received = new object();

		private object data_received = new object();

		private object pin_changed = new object();

		private static Regex linuxDefaultUIFilter = new Regex("/dev/ttyS*\\d+", (RegexOptions)520);

		public static bool DebugPort;

		[Browsable(false)]
		[DesignerSerializationVisibility(/*Could not decode attribute arguments.*/)]
		public Stream BaseStream
		{
			get
			{
				CheckOpen();
				return (Stream)stream;
			}
		}

		[DefaultValue(9600)]
		[Browsable(true)]
		[MonitoringDescription("")]
		public int BaudRate
		{
			get
			{
				return baud_rate;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (is_open)
				{
					stream.SetAttributes(value, parity, data_bits, stop_bits, handshake);
				}
				baud_rate = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(/*Could not decode attribute arguments.*/)]
		public bool BreakState
		{
			get
			{
				return break_state;
			}
			set
			{
				CheckOpen();
				if (value != break_state)
				{
					stream.SetBreakState(value);
					break_state = value;
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(/*Could not decode attribute arguments.*/)]
		public int BytesToRead
		{
			get
			{
				CheckOpen();
				return stream.BytesToRead;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(/*Could not decode attribute arguments.*/)]
		public int BytesToWrite
		{
			get
			{
				CheckOpen();
				return stream.BytesToWrite;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(/*Could not decode attribute arguments.*/)]
		public bool CDHolding
		{
			get
			{
				CheckOpen();
				return (stream.GetSignals() & SerialSignal.Cd) != 0;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(/*Could not decode attribute arguments.*/)]
		public bool CtsHolding
		{
			get
			{
				CheckOpen();
				return (stream.GetSignals() & SerialSignal.Cts) != 0;
			}
		}

		[DefaultValue(8)]
		[Browsable(true)]
		[MonitoringDescription("")]
		public int DataBits
		{
			get
			{
				return data_bits;
			}
			set
			{
				if (value < 5 || value > 8)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (is_open)
				{
					stream.SetAttributes(baud_rate, parity, value, stop_bits, handshake);
				}
				data_bits = value;
			}
		}

		[Browsable(true)]
		[MonitoringDescription("")]
		[DefaultValue(false)]
		public bool DiscardNull
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(/*Could not decode attribute arguments.*/)]
		public bool DsrHolding
		{
			get
			{
				CheckOpen();
				return (stream.GetSignals() & SerialSignal.Dsr) != 0;
			}
		}

		[DefaultValue(false)]
		[Browsable(true)]
		[MonitoringDescription("")]
		public bool DtrEnable
		{
			get
			{
				return dtr_enable;
			}
			set
			{
				if (value != dtr_enable)
				{
					if (is_open)
					{
						stream.SetSignal(SerialSignal.Dtr, value);
					}
					dtr_enable = value;
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(/*Could not decode attribute arguments.*/)]
		[MonitoringDescription("")]
		public Encoding Encoding
		{
			get
			{
				return encoding;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				encoding = value;
			}
		}

		[DefaultValue(Handshake.None)]
		[Browsable(true)]
		[MonitoringDescription("")]
		public Handshake Handshake
		{
			get
			{
				return handshake;
			}
			set
			{
				if (value < Handshake.None || value > Handshake.RequestToSendXOnXOff)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (is_open)
				{
					stream.SetAttributes(baud_rate, parity, data_bits, stop_bits, value);
				}
				handshake = value;
			}
		}

		[Browsable(false)]
		public bool IsOpen => is_open;

		[DefaultValue("\n")]
		[Browsable(false)]
		[MonitoringDescription("")]
		public string NewLine
		{
			get
			{
				return new_line;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length == 0)
				{
					throw new ArgumentException("NewLine cannot be null or empty.", "value");
				}
				new_line = value;
			}
		}

		[DefaultValue(Parity.None)]
		[Browsable(true)]
		[MonitoringDescription("")]
		public Parity Parity
		{
			get
			{
				return parity;
			}
			set
			{
				if (value < Parity.None || value > Parity.Space)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (is_open)
				{
					stream.SetAttributes(baud_rate, value, data_bits, stop_bits, handshake);
				}
				parity = value;
			}
		}

		[Browsable(true)]
		[MonitoringDescription("")]
		[DefaultValue(63)]
		public byte ParityReplace
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[Browsable(true)]
		[MonitoringDescription("")]
		[DefaultValue("COM1")]
		public string PortName
		{
			get
			{
				return port_name;
			}
			set
			{
				if (is_open)
				{
					throw new InvalidOperationException("Port name cannot be set while port is open.");
				}
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length == 0 || value.StartsWith("\\\\"))
				{
					throw new ArgumentException("value");
				}
				port_name = value;
			}
		}

		[DefaultValue(4096)]
		[Browsable(true)]
		[MonitoringDescription("")]
		public int ReadBufferSize
		{
			get
			{
				return readBufferSize;
			}
			set
			{
				if (is_open)
				{
					throw new InvalidOperationException();
				}
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (value > 4096)
				{
					readBufferSize = value;
				}
			}
		}

		[DefaultValue(-1)]
		[Browsable(true)]
		[MonitoringDescription("")]
		public int ReadTimeout
		{
			get
			{
				return read_timeout;
			}
			set
			{
				if (value < 0 && value != -1)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (is_open)
				{
					stream.ReadTimeout = value;
				}
				read_timeout = value;
			}
		}

		[DefaultValue(1)]
		[Browsable(true)]
		[MonitoringDescription("")]
		public int ReceivedBytesThreshold
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				throw new NotImplementedException();
			}
		}

		[DefaultValue(false)]
		[Browsable(true)]
		[MonitoringDescription("")]
		public bool RtsEnable
		{
			get
			{
				return rts_enable;
			}
			set
			{
				if (value != rts_enable)
				{
					if (is_open)
					{
						stream.SetSignal(SerialSignal.Rts, value);
					}
					rts_enable = value;
				}
			}
		}

		[DefaultValue(StopBits.One)]
		[Browsable(true)]
		[MonitoringDescription("")]
		public StopBits StopBits
		{
			get
			{
				return stop_bits;
			}
			set
			{
				if (value < StopBits.One || value > StopBits.OnePointFive)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (is_open)
				{
					stream.SetAttributes(baud_rate, parity, data_bits, value, handshake);
				}
				stop_bits = value;
			}
		}

		[DefaultValue(2048)]
		[Browsable(true)]
		[MonitoringDescription("")]
		public int WriteBufferSize
		{
			get
			{
				return writeBufferSize;
			}
			set
			{
				if (is_open)
				{
					throw new InvalidOperationException();
				}
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (value > 2048)
				{
					writeBufferSize = value;
				}
			}
		}

		[DefaultValue(-1)]
		[Browsable(true)]
		[MonitoringDescription("")]
		public int WriteTimeout
		{
			get
			{
				return write_timeout;
			}
			set
			{
				if (value < 0 && value != -1)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (is_open)
				{
					stream.WriteTimeout = value;
				}
				write_timeout = value;
			}
		}

		[MonitoringDescription("")]
		public event SerialErrorReceivedEventHandler ErrorReceived
		{
			add
			{
				((Component)this).get_Events().AddHandler(error_received, (Delegate)value);
			}
			remove
			{
				((Component)this).get_Events().RemoveHandler(error_received, (Delegate)value);
			}
		}

		[MonitoringDescription("")]
		public event SerialPinChangedEventHandler PinChanged
		{
			add
			{
				((Component)this).get_Events().AddHandler(pin_changed, (Delegate)value);
			}
			remove
			{
				((Component)this).get_Events().RemoveHandler(pin_changed, (Delegate)value);
			}
		}

		[MonitoringDescription("")]
		public event SerialDataReceivedEventHandler DataReceived
		{
			add
			{
				((Component)this).get_Events().AddHandler(data_received, (Delegate)value);
			}
			remove
			{
				((Component)this).get_Events().RemoveHandler(data_received, (Delegate)value);
			}
		}

		public FrostedSerialPort()
			: this(GetDefaultPortName(), 9600, Parity.None, 8, StopBits.One)
		{
		}

		public FrostedSerialPort(IContainer container)
			: this()
		{
		}

		public FrostedSerialPort(string portName)
			: this(portName, 9600, Parity.None, 8, StopBits.One)
		{
		}

		public FrostedSerialPort(string portName, int baudRate)
			: this(portName, baudRate, Parity.None, 8, StopBits.One)
		{
		}

		public FrostedSerialPort(string portName, int baudRate, Parity parity)
			: this(portName, baudRate, parity, 8, StopBits.One)
		{
		}

		public FrostedSerialPort(string portName, int baudRate, Parity parity, int dataBits)
			: this(portName, baudRate, parity, dataBits, StopBits.One)
		{
		}

		public FrostedSerialPort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
			: this()
		{
			port_name = portName;
			baud_rate = baudRate;
			data_bits = dataBits;
			stop_bits = stopBits;
			this.parity = parity;
		}

		public static bool EnsureDeviceAccess()
		{
			return true;
		}

		private static IEnumerable<string> FilterPortsForMac(List<string> allPorts)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			IEnumerable<string> enumerable = (((int)OsInformation.get_OperatingSystem() != 3) ? Enumerable.Where<string>((IEnumerable<string>)allPorts, (Func<string, bool>)((string portName) => portName.StartsWith("/dev/tty.") || portName.StartsWith("COM"))) : Enumerable.Where<string>((IEnumerable<string>)allPorts, (Func<string, bool>)((string portName) => portName != "/dev/tty" && !((Group)linuxDefaultUIFilter.Match(portName)).get_Success())));
			if (!Enumerable.Any<string>(enumerable))
			{
				return allPorts;
			}
			return enumerable;
		}

		public static string[] GetPortNames(bool filter = true)
		{
			PlatformID platform = Environment.OSVersion.Platform;
			List<string> list = new List<string>();
			if (platform == PlatformID.Unix || platform == PlatformID.MacOSX)
			{
				string[] files = Directory.GetFiles("/dev/", "tty*");
				if (!filter)
				{
					return files;
				}
				string[] array = files;
				foreach (string text in array)
				{
					if (text != "/dev/tty" && text.StartsWith("/dev/tty") && !text.StartsWith("/dev/ttyC"))
					{
						list.Add(text);
					}
				}
			}
			else
			{
				RegistryKey val = Registry.LocalMachine.OpenSubKey("HARDWARE\\DEVICEMAP\\SERIALCOMM");
				try
				{
					if (val != null)
					{
						string[] array = val.GetValueNames();
						foreach (string text2 in array)
						{
							string text3 = val.GetValue(text2, (object)"").ToString();
							if (text3 != "")
							{
								list.Add(text3);
							}
						}
					}
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			return Enumerable.ToArray<string>(FilterPortsForMac(list));
		}

		public static string GetDefaultPortName()
		{
			string[] portNames = GetPortNames();
			if (portNames.Length != 0)
			{
				return portNames[0];
			}
			int platform = (int)Environment.OSVersion.Platform;
			if (platform == 4 || platform == 128 || platform == 6)
			{
				return "ttyS0";
			}
			return "COM1";
		}

		public void Close()
		{
			((Component)this).Dispose(true);
		}

		protected override void Dispose(bool disposing)
		{
			if (is_open)
			{
				is_open = false;
				if (disposing)
				{
					stream.Close();
				}
				stream = null;
			}
		}

		public void DiscardInBuffer()
		{
			CheckOpen();
			stream.DiscardInBuffer();
		}

		public void DiscardOutBuffer()
		{
			CheckOpen();
			stream.DiscardOutBuffer();
		}

		public void Open()
		{
			if (is_open)
			{
				throw new InvalidOperationException("Port is already open");
			}
			stream = new FrostedSerialPortStream(port_name, baud_rate, data_bits, parity, stop_bits, dtr_enable, rts_enable, handshake, read_timeout, write_timeout, readBufferSize, writeBufferSize);
			is_open = true;
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			CheckOpen();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException("offset or count less than zero.");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("offset+count", "The size of the buffer is less than offset + count.");
			}
			return stream.Read(buffer, offset, count);
		}

		public int Read(char[] buffer, int offset, int count)
		{
			CheckOpen();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException("offset or count less than zero.");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("offset+count", "The size of the buffer is less than offset + count.");
			}
			int i;
			for (i = 0; i < count; i++)
			{
				int num;
				if ((num = ReadChar()) == -1)
				{
					break;
				}
				buffer[offset + i] = (char)num;
			}
			return i;
		}

		internal int read_byte()
		{
			byte[] array = new byte[1];
			if (stream.Read(array, 0, 1) > 0)
			{
				return array[0];
			}
			return -1;
		}

		public int ReadByte()
		{
			CheckOpen();
			return read_byte();
		}

		public int ReadChar()
		{
			CheckOpen();
			byte[] array = new byte[16];
			int num = 0;
			do
			{
				int num2 = read_byte();
				if (num2 == -1)
				{
					return -1;
				}
				array[num++] = (byte)num2;
				char[] chars = encoding.GetChars(array, 0, 1);
				if (chars.Length != 0)
				{
					return chars[0];
				}
			}
			while (num < array.Length);
			return -1;
		}

		public string ReadExisting()
		{
			CheckOpen();
			int bytesToRead = BytesToRead;
			byte[] array = new byte[bytesToRead];
			int count = stream.Read(array, 0, bytesToRead);
			return new string(encoding.GetChars(array, 0, count));
		}

		public string ReadLine()
		{
			return ReadTo(new_line);
		}

		public string ReadTo(string value)
		{
			CheckOpen();
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Length == 0)
			{
				throw new ArgumentException("value");
			}
			byte[] bytes = encoding.GetBytes(value);
			int num = 0;
			List<byte> list = new List<byte>();
			while (true)
			{
				int num2 = read_byte();
				if (num2 == -1)
				{
					break;
				}
				list.Add((byte)num2);
				if (num2 == bytes[num])
				{
					num++;
					if (num == bytes.Length)
					{
						return encoding.GetString(list.ToArray(), 0, list.Count - bytes.Length);
					}
				}
				else
				{
					num = ((bytes[0] == num2) ? 1 : 0);
				}
			}
			return encoding.GetString(list.ToArray());
		}

		public void Write(string str)
		{
			CheckOpen();
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			byte[] bytes = encoding.GetBytes(str);
			Write(bytes, 0, bytes.Length);
		}

		public void Write(byte[] buffer, int offset, int count)
		{
			CheckOpen();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("offset+count", "The size of the buffer is less than offset + count.");
			}
			stream.Write(buffer, offset, count);
		}

		public void Write(char[] buffer, int offset, int count)
		{
			CheckOpen();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("offset+count", "The size of the buffer is less than offset + count.");
			}
			byte[] bytes = encoding.GetBytes(buffer, offset, count);
			stream.Write(bytes, 0, bytes.Length);
		}

		public void WriteLine(string str)
		{
			Write(str + new_line);
		}

		private void CheckOpen()
		{
			if (!is_open)
			{
				throw new InvalidOperationException("Specified port is not open.");
			}
		}

		internal void OnErrorReceived(SerialErrorReceivedEventArgs args)
		{
			((SerialErrorReceivedEventHandler)((Component)this).get_Events().get_Item(error_received))?.Invoke(this, args);
		}

		internal void OnDataReceived(SerialDataReceivedEventArgs args)
		{
			((SerialDataReceivedEventHandler)((Component)this).get_Events().get_Item(data_received))?.Invoke(this, args);
		}

		internal void OnDataReceived(SerialPinChangedEventArgs args)
		{
			((SerialPinChangedEventHandler)((Component)this).get_Events().get_Item(pin_changed))?.Invoke(this, args);
		}
	}
}
