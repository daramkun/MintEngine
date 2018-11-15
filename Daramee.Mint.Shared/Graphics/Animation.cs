using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Graphics
{
	public class Animation
	{
		TimeSpan interval;
		Queue<string> resources;

		TimeSpan elapsedTime;

		public Animation ( TimeSpan interval, params string [] names )
		{
			this.interval = interval;
			resources = new Queue<string> ( names );
		}

		public Texture2D GetCurrentImage => Engine.SharedEngine.Content.Load<Texture2D> ( resources.Peek () );

		public void Update ( GameTime gameTime )
		{
			elapsedTime += gameTime.ElapsedGameTime;
			if ( elapsedTime >= interval )
			{
				resources.Enqueue ( resources.Dequeue () );
				elapsedTime -= interval;
			}
		}
	}
}
