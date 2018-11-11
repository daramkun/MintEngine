using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Scenes
{
	public abstract class Scene
	{
		public abstract string Name { get; }

		protected virtual void Enter () { }
		protected virtual void Exit () { }

		internal void InnerEnter () { Enter (); }
		internal void InnerExit () { Exit (); }
	}
}
