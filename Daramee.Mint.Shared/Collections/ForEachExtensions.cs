using Daramee.Mint.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.Mint.Collections
{
	public static class ForEachExtensions
	{
		public static void RunAsSequencial ( this IEnumerable<Entity> entities, Action<Entity> action, int threads = 0 )
		{
			if ( threads > 0 )
				throw new ArgumentOutOfRangeException ();
			foreach ( var entity in entities )
				action ( entity );
		}

		public static void RunAsParallel ( this IEnumerable<Entity> entities, Action<Entity> action, int threads = 0 )
		{
			if ( threads <= 0 ) entities.AsParallel ().ForAll ( action );
			else if ( threads == 1 ) RunAsSequencial ( entities, action );
			else Parallel.ForEach<Entity> ( entities, new ParallelOptions () { MaxDegreeOfParallelism = threads }, action );
		}

		public static void RunAsSequencialParallel ( this IEnumerable<Entity> entities, Action<Entity> action, int threads = 0 )
		{
			if ( threads <= 0 )
				threads = Environment.ProcessorCount;
			if ( threads == 1 )
				RunAsSequencial ( entities, action );
			else
			{
				int totalCount = entities.Count ();
				int correction = totalCount / threads * threads;
				int per = correction / threads + ( totalCount - correction > 0 ? 1 : 0 );
				Parallel.For ( 0, threads, ( int i ) => RunAsSequencial ( entities.Skip ( i * per ).Take ( per ), action ) );
			}
		}
	}
}
