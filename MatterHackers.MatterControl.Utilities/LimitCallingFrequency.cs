using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MatterHackers.MatterControl.Utilities
{
	public class LimitCallingFrequency
	{
		private Action functionToCall;

		private double minimumTimeBeforeRepeatSeconds;

		private Stopwatch timeSinceLastCall = new Stopwatch();

		private bool waitingToUpdate;

		public LimitCallingFrequency(double minimumTimeBeforeRepeatSeconds, Action functionToCall)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			this.minimumTimeBeforeRepeatSeconds = minimumTimeBeforeRepeatSeconds;
			this.functionToCall = functionToCall;
		}

		public void CallEvent()
		{
			if (waitingToUpdate)
			{
				return;
			}
			if (!timeSinceLastCall.get_IsRunning() || timeSinceLastCall.get_Elapsed().TotalSeconds > minimumTimeBeforeRepeatSeconds)
			{
				functionToCall?.Invoke();
				timeSinceLastCall.Restart();
				return;
			}
			waitingToUpdate = true;
			Task.Run(delegate
			{
				WaitForTimeAndCallFunction();
			});
		}

		public void CallOccured()
		{
			waitingToUpdate = false;
			timeSinceLastCall.Restart();
		}

		private void WaitForTimeAndCallFunction()
		{
			while (timeSinceLastCall.get_Elapsed().TotalSeconds < minimumTimeBeforeRepeatSeconds)
			{
				Thread.Sleep(1);
				if (!waitingToUpdate)
				{
					return;
				}
			}
			functionToCall?.Invoke();
			waitingToUpdate = false;
			timeSinceLastCall.Restart();
		}
	}
}
