using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MatterHackers.MatterControl.FrostedSerial
{
	internal class FrostedSerialPortStream : Stream, IFrostedSerialStream, IDisposable
	{
		private int fd;

		private int read_timeout;

		private int write_timeout;

		private bool disposed;

		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => true;

		public override bool CanTimeout => true;

		public override int ReadTimeout
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
				read_timeout = value;
			}
		}

		public override int WriteTimeout
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
				write_timeout = value;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public int BytesToRead
		{
			get
			{
				int num = get_bytes_in_buffer(fd, 1);
				if (num == -1)
				{
					ThrowIOException();
				}
				return num;
			}
		}

		public int BytesToWrite
		{
			get
			{
				int num = get_bytes_in_buffer(fd, 0);
				if (num == -1)
				{
					ThrowIOException();
				}
				return num;
			}
		}

		[DllImport("FrostedSerialHelper", SetLastError = true)]
		private static extern int open_serial(string portName);

		[DllImport("FrostedSerialHelper", SetLastError = true)]
		private static extern int set_attributes(int fd, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handshake);

		public FrostedSerialPortStream(string portName, int baudRate, int dataBits, Parity parity, StopBits stopBits, bool dtrEnable, bool rtsEnable, Handshake handshake, int readTimeout, int writeTimeout, int readBufferSize, int writeBufferSize)
		{
			fd = open_serial(portName);
			if (fd == -1)
			{
				ThrowIOException();
			}
			TryBaudRate(baudRate);
			int num = set_attributes(fd, baudRate, parity, dataBits, stopBits, handshake);
			if (num != 0 && !FrostedSerialPort.DebugPort)
			{
				throw new IOException(num.ToString());
			}
			read_timeout = readTimeout;
			write_timeout = writeTimeout;
			SetSignal(SerialSignal.Dtr, dtrEnable);
			if (handshake != Handshake.RequestToSend && handshake != Handshake.RequestToSendXOnXOff)
			{
				SetSignal(SerialSignal.Rts, rtsEnable);
			}
		}

		private int SetupBaudRate(int baudRate)
		{
			throw new NotImplementedException();
		}

		[DllImport("FrostedSerialHelper", SetLastError = true)]
		private static extern int tcgetattr(int fd, tDeviceInfo newtio);

		private int TCGetAttribute(int fd, tDeviceInfo newtio)
		{
			return tcgetattr(fd, newtio);
		}

		[DllImport("FrostedSerialHelper", SetLastError = true)]
		private static extern int tcsetattr(int fd, uint optional_actions, tDeviceInfo newtio);

		private int TCSetAttribute(int fd, uint optional_actions, tDeviceInfo newtio)
		{
			return tcsetattr(fd, optional_actions, newtio);
		}

		private int CFSetISpeed(tDeviceInfo newtio, int baudRate)
		{
			newtio.c_ispeed = (uint)baudRate;
			return (int)newtio.c_ispeed;
		}

		private int CFSetOSpeed(tDeviceInfo newtio, int baudRate)
		{
			newtio.c_ospeed = (uint)baudRate;
			return (int)newtio.c_ospeed;
		}

		private bool SetFrostedAttributes(int fd, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handshake)
		{
			tDeviceInfo newtio = default(tDeviceInfo);
			if (TCGetAttribute(fd, newtio) == -1)
			{
				return false;
			}
			newtio.c_cflag |= 3u;
			newtio.c_lflag &= 4294967169u;
			newtio.c_oflag &= 1u;
			newtio.c_iflag = 4u;
			baudRate = SetupBaudRate(baudRate);
			newtio.c_cflag &= 4294967284u;
			switch (dataBits)
			{
			case 5:
				newtio.c_cflag |= 0u;
				break;
			case 6:
				newtio.c_cflag |= 4u;
				break;
			case 7:
				newtio.c_cflag |= 4u;
				break;
			default:
				newtio.c_cflag |= 12u;
				break;
			}
			switch (stopBits)
			{
			case StopBits.One:
				newtio.c_cflag &= 4294967280u;
				break;
			case StopBits.Two:
				newtio.c_cflag |= 16u;
				break;
			}
			newtio.c_iflag &= 4294967104u;
			switch (parity)
			{
			case Parity.None:
				newtio.c_cflag &= 4294967103u;
				break;
			case Parity.Odd:
				newtio.c_cflag |= 192u;
				break;
			case Parity.Even:
				newtio.c_cflag &= 4294967167u;
				newtio.c_cflag |= 64u;
				break;
			}
			newtio.c_iflag &= 4294966527u;
			switch (handshake)
			{
			case Handshake.XOnXOff:
			case Handshake.RequestToSendXOnXOff:
				newtio.c_iflag |= 768u;
				break;
			}
			if (CFSetOSpeed(newtio, baudRate) < 0 || CFSetISpeed(newtio, baudRate) < 0 || TCSetAttribute(fd, 1u, newtio) < 0)
			{
				return false;
			}
			return true;
		}

		public override void Flush()
		{
		}

		[DllImport("FrostedSerialHelper", SetLastError = true)]
		private static extern int read_serial(int fd, byte[] buffer, int offset, int count);

		[DllImport("FrostedSerialHelper", SetLastError = true)]
		private static extern bool poll_serial(int fd, out int error, int timeout);

		public override int Read([In][Out] byte[] buffer, int offset, int count)
		{
			CheckDisposed();
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
			int error;
			bool num = poll_serial(fd, out error, read_timeout);
			if (error == -1)
			{
				ThrowIOException();
			}
			if (!num)
			{
				throw new TimeoutException();
			}
			int num2 = read_serial(fd, buffer, offset, count);
			if (num2 == -1)
			{
				ThrowIOException();
			}
			return num2;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		[DllImport("FrostedSerialHelper", SetLastError = true)]
		private static extern int write_serial(int fd, byte[] buffer, int offset, int count, int timeout);

		public override void Write(byte[] buffer, int offset, int count)
		{
			CheckDisposed();
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
			if (write_serial(fd, buffer, offset, count, write_timeout) < 0)
			{
				throw new TimeoutException("The operation has timed-out");
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposed)
			{
				disposed = true;
				close_serial(fd);
			}
		}

		[DllImport("FrostedSerialHelper", SetLastError = true)]
		private static extern int close_serial(int fd);

		public override void Close()
		{
			((IDisposable)this).Dispose();
		}

		void IDisposable.Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		~FrostedSerialPortStream()
		{
			Dispose(disposing: false);
		}

		private void CheckDisposed()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}

		public void SetAttributes(int baud_rate, Parity parity, int data_bits, StopBits sb, Handshake hs)
		{
			if (!SetFrostedAttributes(fd, baud_rate, parity, data_bits, sb, hs))
			{
				ThrowIOException();
			}
		}

		[DllImport("FrostedSerialHelper", SetLastError = true)]
		private static extern int get_bytes_in_buffer(int fd, int input);

		[DllImport("FrostedSerialHelper", SetLastError = true)]
		private static extern int discard_buffer(int fd, bool inputBuffer);

		public void DiscardInBuffer()
		{
			if (discard_buffer(fd, inputBuffer: true) != 0)
			{
				ThrowIOException();
			}
		}

		public void DiscardOutBuffer()
		{
			if (discard_buffer(fd, inputBuffer: false) != 0)
			{
				ThrowIOException();
			}
		}

		[DllImport("FrostedSerialHelper", SetLastError = true)]
		private static extern SerialSignal get_signals(int fd, out int error);

		public SerialSignal GetSignals()
		{
			int error;
			SerialSignal result = get_signals(fd, out error);
			if (error == -1)
			{
				ThrowIOException();
			}
			return result;
		}

		[DllImport("FrostedSerialHelper", SetLastError = true)]
		private static extern int set_signal(int fd, SerialSignal signal, bool value);

		public void SetSignal(SerialSignal signal, bool value)
		{
			if (signal < SerialSignal.Cd || signal > SerialSignal.Rts || signal == SerialSignal.Cd || signal == SerialSignal.Cts || signal == SerialSignal.Dsr)
			{
				throw new Exception("Invalid internal value");
			}
			if (set_signal(fd, signal, value) == -1)
			{
				ThrowIOException();
			}
		}

		[DllImport("FrostedSerialHelper", SetLastError = true)]
		private static extern int breakprop(int fd);

		public void SetBreakState(bool value)
		{
			if (value && breakprop(fd) == -1)
			{
				ThrowIOException();
			}
		}

		[DllImport("libc")]
		private static extern IntPtr strerror(int errnum);

		private static void ThrowIOException()
		{
			if (!FrostedSerialPort.DebugPort)
			{
				throw new IOException(Marshal.PtrToStringAnsi(strerror(Marshal.GetLastWin32Error())));
			}
		}

		[DllImport("FrostedSerialHelper")]
		private static extern bool is_baud_rate_legal(int baud_rate);

		private void TryBaudRate(int baudRate)
		{
			if (!is_baud_rate_legal(baudRate))
			{
				throw new ArgumentOutOfRangeException("baudRate", "Given baud rate is not supported on this platform.");
			}
		}
	}
}
