using Daramee.Mint.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Components
{
	public sealed class AudioEmitter : IComponent
	{
		public SoundEffect Audio;
		public float DopplerScale;
		public Vector2 Velocity;

		internal SpinLockedList<SoundEffectInstance> instances = new SpinLockedList<SoundEffectInstance> ();
		internal bool stopFlag = false;

		public void Initialize ()
		{
			Audio = null;
			DopplerScale = 1;
			Velocity = new Vector2 ();

			instances.Clear ();
			stopFlag = false;
		}

		public void CopyFrom ( IComponent component )
		{
			if ( component is AudioEmitter )
			{
				Audio = ( component as AudioEmitter ).Audio;
				DopplerScale = ( component as AudioEmitter ).DopplerScale;
				Velocity = ( component as AudioEmitter ).Velocity;
			}
		}

		public void Play ( bool loop = false )
		{
			if ( stopFlag == true ) return;
			var instance = Audio?.CreateInstance ();
			instance.IsLooped = loop;
			instances.Add ( instance );
		}

		public void StopAll ()
		{
			stopFlag = true;
		}
	}
}
