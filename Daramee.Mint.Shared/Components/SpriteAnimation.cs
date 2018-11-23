using Daramee.Mint.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Components
{
	public class SpriteAnimation : IComponent
	{
		public Animation Animation;
		public float Speed;

		public void Initialize ()
		{
			Animation = null;
			Speed = 1;
		}

		public void CopyFrom ( IComponent component )
		{
			if ( component is SpriteAnimation )
			{
				Animation = ( component as SpriteAnimation ).Animation;
				Speed = ( component as SpriteAnimation ).Speed;
			}
		}
	}
}
