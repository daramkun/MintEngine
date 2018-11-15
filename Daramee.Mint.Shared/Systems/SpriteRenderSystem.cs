using System;
using System.Collections.Generic;
using System.Linq;
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

		public int Order => int.MaxValue - 256;
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
			var cameraEntity = EntityManager.SharedManager.GetEntitiesByComponent<Camera> ().FirstOrDefault ();
			Matrix? cameraMatrix = null;
			if ( cameraEntity != null )
			{
				var transform = cameraEntity.GetComponent<Transform2D> ();
				cameraMatrix = Matrix.CreateTranslation ( new Vector3 ( -transform.Position, 0 ) );
			}
			Engine.SharedEngine.SpriteBatcher.Begin ( samplerState: DrawSampler, blendState: BlendState.NonPremultiplied, transformMatrix: cameraMatrix );
		}

		public void Execute ( Entity entity, GameTime gameTime )
		{
			var transform = entity.GetComponent<Transform2D> ();
			if ( entity.HasComponent<SpriteRender> () )
			{
				var spriteRender = entity.GetComponent<SpriteRender> ();
				if ( !spriteRender.IsVisible || spriteRender.Sprite == null )
					return;
				Engine.SharedEngine.SpriteBatcher.Draw ( spriteRender.Sprite,
					transform.Position - new Vector2 ( spriteRender.Sprite.Width / 2, spriteRender.Sprite.Height / 2 ),
					null, spriteRender.OverlayColor,
					transform.Rotation, transform.Origin, transform.Scale,
					SpriteEffects.None, spriteRender.SortOrder );
			}
			else if ( entity.HasComponent<TextRender> () )
			{
				var textRender = entity.GetComponent<TextRender> ();
				if ( !textRender.IsVisible )
					return;
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
				if ( !rectRender.IsVisible )
					return;
				var pos = transform.Position - rectRender.Size / 2;
				if ( rectRender.Fill )
				{
					renderer.FillRectangle (
						new Rectangle (
							new Point ( ( int ) pos.X, ( int ) pos.Y ),
							new Point ( ( int ) rectRender.Size.X, ( int ) rectRender.Size.Y )
						),
						rectRender.Color,
						transform.Rotation, transform.Scale,
						rectRender.SortOrder );
				}
				else
				{
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
		}

		public void PostExecute ()
		{
			Engine.SharedEngine.SpriteBatcher.End ();
		}
	}
}
