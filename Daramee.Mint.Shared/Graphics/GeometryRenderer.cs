using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Graphics
{
	public class GeometryRenderer : IDisposable
	{
		SpriteBatch spriteBatch;
		Texture2D tex;

		public GeometryRenderer ( SpriteBatch spriteBatch )
		{
			this.spriteBatch = spriteBatch;
			tex = new Texture2D ( Engine.SharedEngine.GraphicsDevice, 1, 1 );
			tex.SetData<Color> ( new Color [] { Color.White } );
		}

		~GeometryRenderer () { Dispose (); }

		public void Dispose ()
		{
			tex.Dispose ();
			GC.SuppressFinalize ( this );
		}

		public void DrawRectangle ( Rectangle rectangle, Color color, float rotation, Vector2 scale, int sortOrder )
		{
			FillRectangle ( new Rectangle ( rectangle.X, rectangle.Y, rectangle.Width, 1 ), color, rotation, scale, sortOrder );
			FillRectangle ( new Rectangle ( rectangle.X, rectangle.Y, 1, rectangle.Height ), color, rotation, scale, sortOrder );
			FillRectangle ( new Rectangle ( rectangle.X + rectangle.Width - 1, rectangle.Y, 1, rectangle.Height ), color, rotation, scale, sortOrder );
			FillRectangle ( new Rectangle ( rectangle.X, rectangle.Y + rectangle.Height - 1, rectangle.Width, 1 ), color, rotation, scale, sortOrder );
		}

		public void FillRectangle( Rectangle rectangle, Color color, float rotation, Vector2 scale, int sortOrder )
		{
			spriteBatch.Draw ( tex, new Vector2 ( rectangle.X, rectangle.Y ), new Rectangle ( 0, 0, 1, 1 ), color,
				rotation, new Vector2 (), new Vector2 ( rectangle.Width, rectangle.Height ) * scale, SpriteEffects.None, sortOrder );
		}

		public void DrawLine ( Vector2 p1, Vector2 p2, float weight, Color color, int sortOrder )
		{
			Vector2 edge = p2 - p1;
			float angle = ( float ) Math.Atan2 ( edge.Y, edge.X );

			spriteBatch.Draw ( tex,
				new Rectangle ( ( int ) p1.X, ( int ) p2.Y, ( int ) edge.Length (), ( int ) weight ),
				null, color, angle, new Vector2 ( 0, 0 ), SpriteEffects.None, sortOrder );
		}
	}
}
