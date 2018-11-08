using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Daramee.Mint.Collections
{
	public sealed class SpinLockedList<T> : IList<T>
	{
		List<T> list = new List<T> ();
		SpinLock spinLock = new SpinLock ();

		public T this [ int index ]
		{
			get
			{
				bool lockTaken = false;
				spinLock.Enter ( ref lockTaken );
				T ret = default ( T );
				if ( lockTaken )
				{
					ret = list [ index ];
					spinLock.Exit ();
				}
				return ret;
			}
			set
			{
				bool lockTaken = false;
				spinLock.Enter ( ref lockTaken );
				if ( lockTaken )
				{
					list [ index ] = value;
					spinLock.Exit ();
				}
			}
		}

		public int Count => list.Count;

		public bool IsReadOnly => false;

		public void Add ( T item )
		{
			bool lockTaken = false;
			spinLock.Enter ( ref lockTaken );
			if ( lockTaken )
			{

				spinLock.Exit ();
			}
		}

		public void Clear ()
		{
			bool lockTaken = false;
			spinLock.Enter ( ref lockTaken );
			if ( lockTaken )
			{
				list.Clear ();
				spinLock.Exit ();
			}
		}

		public bool Contains ( T item )
		{
			bool lockTaken = false;
			spinLock.Enter ( ref lockTaken );
			bool ret = false;
			if ( lockTaken )
			{
				ret = list.Contains ( item );
				spinLock.Exit ();
			}
			return ret;
		}

		public void CopyTo ( T [] array, int arrayIndex )
		{
			bool lockTaken = false;
			spinLock.Enter ( ref lockTaken );
			if ( lockTaken )
			{
				list.CopyTo ( array, arrayIndex );
				spinLock.Exit ();
			}
		}

		public IEnumerator<T> GetEnumerator () => list.GetEnumerator ();

		public int IndexOf ( T item )
		{
			bool lockTaken = false;
			spinLock.Enter ( ref lockTaken );
			int ret = -1;
			if ( lockTaken )
			{
				ret = list.IndexOf ( item );
				spinLock.Exit ();
			}
			return ret;
		}

		public void Insert ( int index, T item )
		{
			bool lockTaken = false;
			spinLock.Enter ( ref lockTaken );
			if ( lockTaken )
			{
				list.Insert ( index, item );
				spinLock.Exit ();
			}
		}

		public bool Remove ( T item )
		{
			bool lockTaken = false;
			spinLock.Enter ( ref lockTaken );
			bool ret = false;
			if ( lockTaken )
			{
				ret = list.Remove ( item );
				spinLock.Exit ();
			}
			return ret;
		}

		public void RemoveAt ( int index )
		{
			bool lockTaken = false;
			spinLock.Enter ( ref lockTaken );
			if ( lockTaken )
			{
				list.RemoveAt ( index );
				spinLock.Exit ();
			}
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}
}
