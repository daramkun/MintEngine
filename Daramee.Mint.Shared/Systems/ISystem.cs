using Daramee.Mint.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Systems
{
	public interface ISystem
	{
		bool IsTarget ( Entity entity );
		bool IsParallelExecution { get; }
		int Order { get; }

		void PreExecute ();
		void PostExecute ();

		void Execute ( Entity entity, GameTime gameTime );
	}
}
