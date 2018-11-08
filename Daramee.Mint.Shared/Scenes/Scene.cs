using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Scenes
{
	public abstract class Scene
	{
		public abstract string Name { get; }

		internal protected virtual void Enter () { }
		internal protected virtual void Exit () { }
	}
}
