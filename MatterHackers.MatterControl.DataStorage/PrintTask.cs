using System;

namespace MatterHackers.MatterControl.DataStorage
{
	public class PrintTask : Entity
	{
		public string PrintingGCodeFileName
		{
			get;
			set;
		}

		public double RecoveryCount
		{
			get;
			set;
		}

		public double PercentDone
		{
			get;
			set;
		}

		public bool PrintComplete
		{
			get;
			set;
		}

		public DateTime PrintEnd
		{
			get;
			set;
		}

		[Indexed]
		public int PrinterId
		{
			get;
			set;
		}

		[Indexed]
		public int PrintItemId
		{
			get;
			set;
		}

		public string PrintName
		{
			get;
			set;
		}

		public DateTime PrintStart
		{
			get;
			set;
		}

		public int PrintTimeMinutes => (int)(PrintEnd.Subtract(PrintStart).TotalMinutes + 0.5);

		public int PrintTimeSeconds
		{
			get;
			set;
		}

		public float PrintingOffsetX
		{
			get;
			set;
		}

		public float PrintingOffsetY
		{
			get;
			set;
		}

		public float PrintingOffsetZ
		{
			get;
			set;
		}

		public PrintTask()
		{
			PrintStart = DateTime.Now;
		}

		public override void Commit()
		{
			if (PrintEnd != DateTime.MinValue)
			{
				PrintTimeSeconds = (int)PrintEnd.Subtract(PrintStart).TotalSeconds;
			}
			base.Commit();
		}
	}
}
