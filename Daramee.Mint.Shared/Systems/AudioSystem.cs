using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Daramee.Mint.Collections;
using Daramee.Mint.Components;
using Daramee.Mint.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Daramee.Mint.Systems
{
	public sealed class AudioSystem : ISystem, IDisposable
	{
		public bool IsParallelExecution => true;
		public int Order => int.MinValue;

		public bool IsTarget ( Entity entity ) => entity.HasComponent<Components.AudioEmitter> () || entity.HasComponent<Components.AudioListener> ();

		Microsoft.Xna.Framework.Audio.AudioListener listener = new Microsoft.Xna.Framework.Audio.AudioListener ();
		SpinLockedList<SoundEffectInstance> playingInstances = new SpinLockedList<SoundEffectInstance> ();
		ConcurrentDictionary<SoundEffectInstance, Components.AudioEmitter> instanceDict
			= new ConcurrentDictionary<SoundEffectInstance, Components.AudioEmitter> ();
		List<SoundEffectInstance> removingList = new List<SoundEffectInstance> ();
		
		public void Dispose ()
		{
			foreach ( var instance in playingInstances )
				instance.Dispose ();
			playingInstances.Clear ();
			playingInstances = null;
			instanceDict = null;
			removingList = null;
		}

		private void Apply3D ( Transform2D transform, Components.AudioEmitter emitter,
			SoundEffectInstance instance )
		{
			Microsoft.Xna.Framework.Audio.AudioEmitter xnaEmitter = new Microsoft.Xna.Framework.Audio.AudioEmitter
			{
				Position = new Vector3 ( transform.Position.X, 0, transform.Position.Y ),
				Forward = new Vector3 (),
				Up = new Vector3 ( 0, 1, 0 ),
				Velocity = new Vector3 ( emitter.Velocity.X, 0, emitter.Velocity.Y ),
				DopplerScale = emitter.DopplerScale,
			};
			instance.Apply3D ( listener, xnaEmitter );
		}

		public void PreExecute ()
		{

		}

		public void Execute ( Entity entity, GameTime gameTime )
		{
			if ( entity.HasComponent<Components.AudioEmitter> () )
			{
				var emitter = entity.GetComponent<Components.AudioEmitter> ();
				foreach ( var instance in emitter.instances )
				{
					if ( entity.HasComponent<Transform2D> () )
					{
						var transform = entity.GetComponent<Transform2D> ();
						Apply3D ( transform, emitter, instance );
					}
					instance.Play ();

					playingInstances.Add ( instance );
					instanceDict.TryAdd ( instance, emitter );
				}
				emitter.instances.Clear ();

				if ( emitter.stopFlag )
				{
					foreach ( var kv in instanceDict )
						if ( kv.Value == emitter )
							kv.Key.Stop ();

					emitter.stopFlag = false;
				}
			}
			else if ( entity.HasComponent<Components.AudioListener> () )
			{
				var transform = entity.GetComponent<Transform2D> ();
				listener.Position = new Vector3 ( transform.Position.X, 0, transform.Position.Y );
				listener.Up = new Vector3 ( 0, 1, 0 );
				listener.Forward = new Vector3 ();

				foreach ( var instance in playingInstances )
				{
					var emitter = instanceDict [ instance ];
					var emitterTransform = EntityManager.SharedManager.GetEntityByComponent ( emitter ).GetComponent<Transform2D> ();
					Apply3D ( emitterTransform, emitter, instance );
				}
			}
		}

		public void PostExecute ()
		{
			foreach ( var instance in playingInstances )
			{
				if ( instance.State == SoundState.Stopped )
					removingList.Add ( instance );
			}
			foreach ( var instance in removingList )
			{
				playingInstances.Remove ( instance );
				instanceDict.TryRemove ( instance, out var temp );
				instance.Dispose ();
			}
			removingList.Clear ();
		}
	}
}
