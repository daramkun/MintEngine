using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Daramee.Mint.Diagnostics.Writers
{
	public class StreamLogWriter : ILogWriter, IDisposable
	{
		Stream stream;
		StreamWriter writer;

		public StreamLogWriter ( Stream stream, bool leaveOpen = false )
		{
			this.stream = stream;
			writer = new StreamWriter ( stream, Encoding.UTF8, 4096, leaveOpen );
		}

		~StreamLogWriter ()
		{
			Dispose ();
		}

		public void Dispose ()
		{
			writer.Dispose ();
			GC.SuppressFinalize ( this );
		}

		public void Write ( string message )
		{
			writer.WriteLine ( message );
		}
	}
}
