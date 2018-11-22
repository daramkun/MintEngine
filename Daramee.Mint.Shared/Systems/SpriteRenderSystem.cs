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
		SpriteBatch spriteBatch;
		GeometryRenderer renderer;
		int order;
		bool cameraIndependency;

		public int Order => order;
		public bool IsParallelExecution => false;

		public bool IsTarget ( Entity entity ) => ( entity.IsActived && entity.HasComponent<Transform2D> () ) && ( entity.HasComponent<SpriteRender> () || entity.HasComponent<TextRender> () || entity.HasComponent<RectangleRender> () );

		public SamplerState DrawSampler = SamplerState.PointClamp;

		internal SpriteRenderSystem ( bool cameraIndependent = false )
		{
			if ( !cameraIndependent )
			{
				order = int.MaxValue - 256;
			}
			else
			{
				order = int.MaxValue - 255;
			}
			cameraIndependency = cameraIndependent;

			spriteBatch = new SpriteBatch ( Engine.SharedEngine.GraphicsDevice );
			renderer = new GeometryRenderer ( spriteBatch );
		}

		public void Dispose ()
		{
			renderer.Dispose ();
			renderer = null;
		}

		public void PreExecute ()
		{
			var cameraEntity = EntityManager.SharedManager.GetEntitiesByComponent<Camera> ().FirstOrDefault ();
			Matrix? cameraMatrix = null;
			if ( cameraEntity != null && !cameraIndependency )
			{
				var transform = cameraEntity.GetComponent<Transform2D> ();
				cameraMatrix = Matrix.CreateTranslation ( new Vector3 ( -transform.Position, 0 ) );
			}
			spriteBatch.Begin ( samplerState: DrawSampler, blendState: BlendState.NonPremultiplied, transformMatrix: cameraMatrix );
		}

		public void Execute ( Entity entity, GameTime gameTime )
		{
			var transform = entity.GetComponent<Transform2D> ();
			if ( entity.HasComponent<SpriteRender> () )
			{
				var spriteRender = entity.GetComponent<SpriteRender> ();
				if ( spriteRender.IsCameraIndependency != cameraIndependency )
					return;
				if ( !spriteRender.IsVisible || spriteRender.Sprite == null )
					return;
				spriteBatch.Draw ( spriteRender.Sprite,
					transform.Position - new Vector2 ( spriteRender.Sprite.Width / 2, spriteRender.Sprite.Height / 2 ),
					null, spriteRender.OverlayColor,
					transform.Rotation, transform.Origin, transform.Scale,
					SpriteEffects.None, spriteRender.SortOrder );
			}
			else if ( entity.HasComponent<TextRender> () )
			{
				var textRender = entity.GetComponent<TextRender> ();
				if ( textRender.IsCameraIndependency != cameraIndependency )
					return;
				if ( !textRender.IsVisible )
					return;
				var measured = textRender.Font.MeasureString ( textRender.Text );
				spriteBatch.DrawString ( textRender.Font, textRender.Text,
					transform.Position - measured / 2,
					textRender.ForegroundColor,
					transform.Rotation, transform.Origin, transform.Scale,
					SpriteEffects.None, textRender.SortOrder);
			}
			else if ( entity.HasComponent<RectangleRender> () )
			{
				var rectRender = entity.GetComponent<RectangleRender> ();
				if ( rectRender.IsCameraIndependency != cameraIndependency )
					return;
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
			spriteBatch.End ();
		}
	}
}
