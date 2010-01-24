using System;

namespace IceChat
{
	public class IrcTimer : System.Timers.Timer
	{
		private string timerID;		
		private int timerRepetitions;
		private int timerCounter;
		private string timerCommand;
		
		public delegate void TimerElapsed(string command);
		public event TimerElapsed OnTimerElapsed;

		public IrcTimer(string ID, double interval, int repetitions, string command)
		{
			this.timerID = ID;			
			this.timerCommand = command;			
			this.timerRepetitions = repetitions;
			this.Interval = interval;
			this.Elapsed+=new System.Timers.ElapsedEventHandler(IrcTimer_Elapsed);
			
            timerCounter = 0;
		}
		
		private void IrcTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (OnTimerElapsed != null)
				OnTimerElapsed(this.timerCommand);
			
			timerCounter++;
			if (timerCounter == timerRepetitions)
			{
				//timer has expired, dispose of it
				this.DisableTimer();
			}
		}
		
		internal void DisableTimer()
		{
			this.Stop();
			System.Diagnostics.Debug.WriteLine("timer " + timerID + " has been disposed");
			base.Dispose();
		}
		
        internal string TimerID
		{
			get
			{
				return this.timerID;
			}
		}
	}

}
