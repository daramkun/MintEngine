using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Scenes
{
	public abstract class Scene
	{
		public abstract string Name { get; }

		protected abstract void Enter ();
		protected abstract void Exit ();

		internal void InnerEnter () { Enter (); }
		internal void InnerExit () { Exit (); }
	}
}
