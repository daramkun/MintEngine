using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Coroutines
{
	public class WaitForSeconds
	{
		public TimeSpan WaitingSeconds { get; private set; }
		TimeSpan seconds;

		public WaitForSeconds ( TimeSpan waitingSeconds )
		{
			WaitingSeconds = seconds = waitingSeconds;
		}

		public bool IsDone ( GameTime gameTime )
		{
			seconds -= gameTime.ElapsedGameTime;
			return seconds <= new TimeSpan ();
		}
	}
}
