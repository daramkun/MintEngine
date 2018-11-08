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
			scene?.Enter ();
		}

		public void Dispose ()
		{
			scene?.Exit ();
			GC.SuppressFinalize ( this );
		}

		public Scene Transition ( string nextSceneName, bool unloadContents = true )
		{
			scene?.Exit ();
			if ( unloadContents )
				Engine.SharedEngine.Content.Unload ();
			EntityManager.SharedManager.ClearEntities ();
			scene = scenes [ nextSceneName ];
			scene?.Enter ();
			return scene;
		}
	}
}
