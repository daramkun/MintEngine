using Daramee.Mint.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.Mint.Processors
{
	public sealed class ProcessorManager : IDisposable
	{
		public static ProcessorManager SharedManager { get; private set; }

		readonly ObservableCollection<IProcessor> processores = new ObservableCollection<IProcessor> ();
		
		internal ProcessorManager ()
		{
			if ( SharedManager != null )
				throw new InvalidOperationException ();

			SharedManager = this;
		}

		public void Dispose ()
		{
			SharedManager = null;
		}

		public bool RegisterProcessor ( IProcessor processor )
		{
			if ( IsProcessorRegistered ( processor.GetType () ) )
				return false;
			processores.Add ( processor );
			return true;
		}

		public void UnregisterProcessor ( IProcessor processor )
		{
			processores.Remove ( processor );
			if ( processor is IDisposable )
				( processor as IDisposable ).Dispose ();
		}

		public IEnumerable<IProcessor> GetSystems () => new ForEachSafeEnumerable<IProcessor> ( processores, null );

		public IProcessor GetProcessor ( Type t )
		{
			foreach ( IProcessor processor in processores )
				if ( processor.GetType () == t )
					return processor;
			return null;
		}
		public IProcessor GetProcessor<T> ()
		{
			return GetProcessor ( typeof ( T ) );
		}

		public bool IsProcessorRegistered ( Type type )
		{
			foreach ( IProcessor system in processores )
				if ( system.GetType () == type )
					return true;
			return false;
		}
		public bool IsProcessorRegistered<T> ()
		{
			return IsProcessorRegistered ( typeof ( T ) );
		}

		public void Process ( GameTime gameTime )
		{
			Parallel.ForEach ( new ForEachSafeEnumerable<IProcessor> ( processores ), ( processor ) => processor.Process ( gameTime ) );
		}
	}
}
