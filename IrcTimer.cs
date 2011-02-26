/******************************************************************************\
 * IceChat 2009 Internet Relay Chat Client
 *
 * Copyright (C) 2011 Paul Vanderzee <snerf@icechat.net>
 *                                    <www.icechat.net> 
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
 *
 * Please consult the LICENSE.txt file included with this project for
 * more details
 *
\******************************************************************************/

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
