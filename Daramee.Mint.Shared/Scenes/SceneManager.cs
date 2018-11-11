using Daramee.Mint.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Scenes
{
	public sealed class SceneManager : IDisposable
	{
		public static SceneManager SharedManager { get; private set; }

		Scene scene;
		readonly Dictionary<string, Scene> scenes;

		public Scene CurrentScene => scene;

		internal SceneManager ( string startSceneName, params Scene [] scenes )
		{
			if ( SharedManager != null )
				throw new InvalidOperationException ();

			this.scenes = new Dictionary<string, Scene> ();
			foreach ( var scene in scenes )
			{
				this.scenes.Add ( scene.Name, scene );
				if ( startSceneName == scene.Name )
					this.scene = scene;
			}

			SharedManager = this;
		}

		internal void StartScene ()
		{
			scene?.InnerEnter ();
		}

		public void Dispose ()
		{
			scene?.InnerExit ();
			GC.SuppressFinalize ( this );
		}

		public Scene Transition ( string nextSceneName, bool unloadContents = true )
		{
			scene?.InnerExit ();
			if ( unloadContents )
				Engine.SharedEngine.Content.Unload ();
			EntityManager.SharedManager.ClearEntities ();
			scene = scenes [ nextSceneName ];
			scene?.InnerEnter ();
			return scene;
		}
	}
}
