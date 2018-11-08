using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Daramee.Mint.Processors
{
	public class FrameRateCalculateProcessor : IProcessor
	{
		int count = 0;
		TimeSpan elapsedTime = new TimeSpan ();

		public float FrameRate { get; private set; } = 0;

		public void PreProcess () { }
		public void PostProcess () { }

		public void Process ( GameTime gameTime )
		{
			++count;
			elapsedTime += gameTime.ElapsedGameTime;

			if ( elapsedTime > TimeSpan.FromSeconds ( 1 ) )
			{
				FrameRate = count - ( float ) ( count * elapsedTime.TotalSeconds );
				count = 0;
				elapsedTime -= TimeSpan.FromSeconds ( 1 );
			}
		}
	}
}
