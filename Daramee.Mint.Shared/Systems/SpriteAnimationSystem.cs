using System;
using System.Collections.Generic;
using System.Text;
using Daramee.Mint.Components;
using Daramee.Mint.Entities;
using Microsoft.Xna.Framework;

namespace Daramee.Mint.Systems
{
	public sealed class SpriteAnimationSystem : ISystem
	{
		public bool IsParallelExecution => true;
		public int Order => 0;

		public bool IsTarget ( Entity entity ) => entity.HasComponent<SpriteAnimation> () && entity.HasComponent<SpriteRender> ();

		internal SpriteAnimationSystem () { }

		public void PreExecute ()
		{

		}

		public void Execute ( Entity entity, GameTime gameTime )
		{
			entity.GetComponent<SpriteAnimation> ().Animation?.Update ( gameTime );
			entity.GetComponent<SpriteRender> ().Sprite = entity.GetComponent<SpriteAnimation> ().Animation.GetCurrentImage;
		}

		public void PostExecute ()
		{

		}
	}
}
