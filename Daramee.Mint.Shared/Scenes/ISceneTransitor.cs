using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Scenes
{
	public interface ISceneTransitor
	{
		void DoneTransition ();
		void Transition ( string sceneName, TimeSpan transitInterval, bool unloadContents = true );
	}
}
