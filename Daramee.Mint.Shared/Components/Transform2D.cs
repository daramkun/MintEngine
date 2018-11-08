using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Components
{
	public sealed class Transform2D : IComponent
	{
		public Vector2 Position;
		public Vector2 Scale;
		public float Rotation;

		public Vector2 Origin;

		public void Initialize ()
		{
			Position = new Vector2 ();
			Scale = new Vector2 ( 1 );
			Rotation = 0;

			Origin = new Vector2 ();
		}

		public void CopyFrom ( IComponent component )
		{
			if ( component is Transform2D )
			{
				Position = ( component as Transform2D ).Position;
				Scale = ( component as Transform2D ).Scale;
				Rotation = ( component as Transform2D ).Rotation;

				Origin = ( component as Transform2D ).Origin;
			}
		}
	}
}
