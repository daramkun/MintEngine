using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Daramee.Mint.Diagnostics.Writers
{
	class DebugLogWriter : ILogWriter
	{
		public void Write ( string message )
		{
			Debug.WriteLine ( message );
		}
	}
}
