using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Diagnostics
{
	public interface ILogWriter
	{
		void Write ( string message );
	}
}
