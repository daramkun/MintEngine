using System;
using System.Collections.Generic;
using System.Text;
using Daramee.Mint.Components;
using Daramee.Mint.Entities;
using Daramee.Mint.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Daramee.Mint.Systems
{
	public sealed class SpriteRenderSystem : ISystem, IDisposable
	{
		GeometryRenderer renderer = new GeometryRenderer ();

		public int Order => int.MaxValue;
		public bool IsParallelExecution => false;

		public bool IsTarget ( Entity entity ) => ( entity.IsActived && entity.HasComponent<Transform2D> () ) && ( true );

		public SamplerState DrawSampler = SamplerState.PointClamp;

		public void Dispose ()
		{
			renderer.Dispose ();
			renderer = null;
		}

		public void PreExecute ()
		{
			EntityManager.SharedManager.GetEntitiesByComponent<Camera> ();
			Engine.SharedEngine.SpriteBatcher.Begin ( samplerState: DrawSampler );
		}

		public void Execute ( Entity entity, GameTime gameTime )
		{
			var transform = entity.GetComponent<Transform2D> ();
			if ( entity.HasComponent<SpriteRender> () )
			{
				var spriteRender = entity.GetComponent<SpriteRender> ();
				Engine.SharedEngine.SpriteBatcher.Draw ( spriteRender.Sprite,
					transform.Position - new Vector2 ( spriteRender.Sprite.Width / 2, spriteRender.Sprite.Height / 2 ),
					null, spriteRender.OverlayColor,
					transform.Rotation, transform.Origin, transform.Scale,
					SpriteEffects.None, spriteRender.SortOrder );
			}
			else if ( entity.HasComponent<TextRender> () )
			{
				var textRender = entity.GetComponent<TextRender> ();
				var measured = textRender.Font.MeasureString ( textRender.Text );
				Engine.SharedEngine.SpriteBatcher.DrawString ( textRender.Font, textRender.Text,
					transform.Position - measured / 2,
					textRender.ForegroundColor,
					transform.Rotation, transform.Origin, transform.Scale,
					SpriteEffects.None, textRender.SortOrder);
			}
			else if ( entity.HasComponent<RectangleRender> () )
			{
				var rectRender = entity.GetComponent<RectangleRender> ();
				var pos = transform.Position - rectRender.Size / 2;
				renderer.DrawRectangle (
					new Rectangle ( 
						new Point ( ( int ) pos.X, ( int ) pos.Y ),
						new Point ( ( int ) rectRender.Size.X, ( int ) rectRender.Size.Y )
					),
					rectRender.Color,
					transform.Rotation, transform.Scale,
					rectRender.SortOrder );
			}
		}

		public void PostExecute ()
		{
			Engine.SharedEngine.SpriteBatcher.End ();
		}
	}
}
