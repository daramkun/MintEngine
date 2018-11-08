using Daramee.Mint.Diagnostics.Writers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Daramee.Mint.Diagnostics
{
	public sealed class Logger : IDisposable
	{
		public static Logger SharedLogger { get; private set; }

		ConcurrentBag<ILogWriter> writers = new ConcurrentBag<ILogWriter> ();

		public Logger ()
		{
			if ( SharedLogger != null )
				throw new InvalidOperationException ();

			writers.Add ( new DebugLogWriter () );

			SharedLogger = this;
		}
		~Logger () { Dispose (); }

		public void Dispose ()
		{
			foreach ( var writer in writers )
			{
				if ( writer is IDisposable )
					( writer as IDisposable ).Dispose ();
			}
			writers = null;

			GC.SuppressFinalize ( this );
		}

		public void RegisterLogWriter ( ILogWriter writer )
		{
			writers.Add ( writer );
		}

		public void Log ( string message )
		{
			StringBuilder builder = new StringBuilder ();
			builder.AppendFormat ( "[{0}]", DateTime.Now ).Append ( message );
			var builded = builder.ToString ();
			foreach ( var writer in writers )
				writer.Write ( builded );
		}
	}
}
