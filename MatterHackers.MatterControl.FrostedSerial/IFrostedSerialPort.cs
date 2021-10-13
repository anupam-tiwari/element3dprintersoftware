namespace MatterHackers.MatterControl.FrostedSerial
{
	public interface IFrostedSerialPort
	{
		bool RtsEnable
		{
			get;
			set;
		}

		bool DtrEnable
		{
			get;
			set;
		}

		int BaudRate
		{
			get;
			set;
		}

		int BytesToRead
		{
			get;
		}

		int WriteTimeout
		{
			get;
			set;
		}

		int ReadTimeout
		{
			get;
			set;
		}

		bool IsOpen
		{
			get;
		}

		void Write(string str);

		void Write(byte[] buffer, int offset, int count);

		string ReadExisting();

		int Read(byte[] buffer, int offset, int count);

		void Open();

		void Close();

		void Dispose();
	}
}
