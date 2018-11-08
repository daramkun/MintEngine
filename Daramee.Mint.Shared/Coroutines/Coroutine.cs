using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Coroutines
{
	public sealed class Coroutine
	{
		public static Coroutine SharedCoroutine { get; private set; }
		
		List<IEnumerator> coroutines = new List<IEnumerator> ();
		ConcurrentQueue<IEnumerator> registeringCoroutines = new ConcurrentQueue<IEnumerator> ();
		List<IEnumerator> unregisteringCoroutines = new List<IEnumerator> ();

		internal Coroutine ()
		{
			if ( SharedCoroutine != null )
				throw new InvalidOperationException ();

			SharedCoroutine = this;
		}
		
		public void RegisterCoroutine ( IEnumerator coroutine )
		{
			registeringCoroutines.Enqueue ( coroutine );
		}

		internal void DoCoroutines ( GameTime gameTime )
		{
			foreach ( var enumerator in coroutines )
			{
				if ( enumerator.Current is WaitForSeconds )
				{
					if ( !( enumerator.Current as WaitForSeconds ).IsDone ( gameTime ) )
						continue;
				}
				if ( !enumerator.MoveNext () )
					unregisteringCoroutines.Add ( enumerator );
			}
			foreach ( var coroutine in unregisteringCoroutines )
				coroutines.Remove ( coroutine );
			unregisteringCoroutines.Clear ();
			while ( !registeringCoroutines.IsEmpty )
			{
				if ( registeringCoroutines.TryDequeue ( out IEnumerator coroutine ) )
					coroutines.Add ( coroutine );
			}
		}
	}
}
