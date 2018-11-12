using Daramee.Mint.Components;
using Daramee.Mint.Entities;
using Daramee.Mint.Processors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Scenes
{
	public abstract class LogoScene : Scene, IProcessor
	{
		string [] logoSpriteNames;
		Queue<Entity> spriteEntities = new Queue<Entity> ();

		bool fadeIn = true;
		double alphaValue = 0;

		public LogoScene ( params string [] logoResourceNames )
		{
			logoSpriteNames = logoResourceNames;
		}

		protected override void Enter ()
		{
			spriteEntities.Clear ();
			foreach ( var spriteName in logoSpriteNames )
			{
				Entity entity = EntityManager.SharedManager.CreateEntity ();
				entity.AddComponent<Transform2D> ().Position = Engine.SharedEngine.FrameBuffer.Size / 2;
				var sprite = entity.AddComponent<SpriteRender> ();
				sprite.Sprite = Engine.SharedEngine.Content.Load<Texture2D> ( spriteName );
				sprite.OverlayColor = new Color ( 255, 255, 255, 0 );
				spriteEntities.Enqueue ( entity );
			}
			fadeIn = true;
			alphaValue = 0;

			ProcessorManager.SharedManager.RegisterProcessor ( this );
		}

		protected override void Exit ()
		{

		}

		public void Process ( GameTime gameTime )
		{
			if ( spriteEntities.Count > 0 )
			{
				var sprite = spriteEntities.Peek ().GetComponent<SpriteRender> ();

				alphaValue += 255 * gameTime.ElapsedGameTime.TotalSeconds * ( fadeIn ? 1 : -1 );
				if ( fadeIn && alphaValue >= 500 )
				{
					fadeIn = false;
					alphaValue = 255;
				}
				else if ( !fadeIn && alphaValue <= 0 )
				{
					fadeIn = true;
					alphaValue = 0;
					spriteEntities.Dequeue ();
				}
				sprite.OverlayColor = new Color ( 255, 255, 255, ( int ) Math.Min ( alphaValue, 255 ) );
			}
			else
			{
				OnLogoDisplayEnded ();
				ProcessorManager.SharedManager.UnregisterProcessor ( this );
			}
		}

		protected abstract void OnLogoDisplayEnded ();
	}
}
