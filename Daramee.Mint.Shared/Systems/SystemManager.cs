using Daramee.Mint.Collections;
using Daramee.Mint.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.Mint.Systems
{
	public sealed class SystemManager
	{
		public static SystemManager SharedManager { get; private set; }

		readonly ObservableCollection<ISystem> systems = new ObservableCollection<ISystem> ();
		readonly List<ISystem> loopContainer = new List<ISystem> ();

		internal SystemManager ()
		{
			if ( SharedManager != null )
				throw new InvalidOperationException ();
			SharedManager = this;
		}

		public bool RegisterSystem ( ISystem system )
		{
			//if ( IsSystemRegistered ( system.GetType () ) )
			//	return false;
			systems.Add ( system );
			return true;
		}

		public void UnregisterSystem ( ISystem system )
		{
			systems.Remove ( system );
			if ( system is IDisposable )
				( system as IDisposable ).Dispose ();
		}

		public IEnumerable<ISystem> GetSystems () => new ForEachSafeEnumerable<ISystem> ( systems, null );

		public IEnumerable<ISystem> GetSystems ( Type t )
		{
			foreach ( ISystem system in systems )
				if ( system.GetType () == t )
					yield return system;
		}
		public IEnumerable<ISystem> GetSystems<T> ()
		{
			return GetSystems ( typeof ( T ) );
		}

		public bool IsSystemRegistered ( Type type )
		{
			foreach ( ISystem system in systems )
				if ( system.GetType () == type )
					return true;
			return false;
		}
		public bool IsSystemRegistered<T> ()
		{
			return IsSystemRegistered ( typeof ( T ) );
		}

		public void Execute ( GameTime gameTime )
		{
			////////////////////////////////////////////////////////////////////////////////////////
			// Parallel Execution
			////////////////////////////////////////////////////////////////////////////////////////
			foreach ( ISystem system in
				new ForEachSafeEnumerable<ISystem> ( systems, null )
				.Where ( ( system ) => system.IsParallelExecution )
				.OrderBy ( ( system ) => system.Order ) )
			{
				system.PreExecute ();
				loopContainer.Add ( system );
			}

			if ( loopContainer.Count > 0 )
			{
				ForEachExtensions.RunAsSequencialParallel ( EntityManager.SharedManager.Entities, ( entity ) =>
				{
					foreach ( ISystem system in loopContainer )
					{
						if ( system.IsTarget ( entity ) )
							system.Execute ( entity, gameTime );
					}
				} );

				foreach ( ISystem system in loopContainer )
					system.PostExecute ();
				loopContainer.Clear ();
			}

			////////////////////////////////////////////////////////////////////////////////////////
			// Sequencial Execution
			////////////////////////////////////////////////////////////////////////////////////////
			foreach ( ISystem system in
				new ForEachSafeEnumerable<ISystem> ( systems, null )
				.Where ( ( system ) => !system.IsParallelExecution )
				.OrderBy ( ( system ) => system.Order ) )
			{
				system.PreExecute ();
				loopContainer.Add ( system );
			}

			if ( loopContainer.Count > 0 )
			{
				foreach ( var entity in EntityManager.SharedManager.Entities )
				{
					foreach ( ISystem system in loopContainer )
					{
						if ( system.IsTarget ( entity ) )
							system.Execute ( entity, gameTime );
					}
				}

				foreach ( ISystem system in loopContainer )
					system.PostExecute ();
				loopContainer.Clear ();
			}
		}
	}
}
