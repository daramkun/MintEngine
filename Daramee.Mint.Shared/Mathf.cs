using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint
{
	public static class Mathf
	{
		public static Vector2 Abs ( Vector2 v ) { return new Vector2 ( Math.Abs ( v.X ), Math.Abs ( v.Y ) ); }
		public static Vector3 Abs ( Vector3 v ) { return new Vector3 ( Math.Abs ( v.X ), Math.Abs ( v.Y ), Math.Abs ( v.Z ) ); }
		public static Vector4 Abs ( Vector4 v ) { return new Vector4 ( Math.Abs ( v.X ), Math.Abs ( v.Y ), Math.Abs ( v.Z ), Math.Abs ( v.W ) ); }
		public static Vector2 Floor ( Vector2 v ) { return new Vector2 ( ( float ) Math.Floor ( v.X ), ( float ) Math.Floor ( v.Y ) ); }

		public static Vector2 CalculateRotatedMovingUnit ( float angle )
		{
			return new Vector2 (
				( float ) Math.Cos ( MathHelper.ToRadians ( angle ) ),
				( float ) Math.Sin ( MathHelper.ToRadians ( angle ) )
			);
		}
		public static Vector3 CalculateRotationMovingUnit ( Vector3 angle )
		{
			float X = ( float ) Math.Sin ( MathHelper.ToRadians ( angle.Y ) ) * ( float ) Math.Cos ( MathHelper.ToRadians ( angle.X ) );
			float Y = ( float ) Math.Sin ( MathHelper.ToRadians ( -angle.X ) );
			float Z = ( float ) Math.Cos ( MathHelper.ToRadians ( angle.Y ) ) * ( float ) Math.Cos ( MathHelper.ToRadians ( angle.X ) );
			return new Vector3 ( X, Y, Z );
		}

		public static float FollowedAngle ( Vector2 a, Vector2 b )
		{
			Vector2 ab = a - b;
			return MathHelper.ToDegrees ( ( float ) Math.Atan2 ( ab.Y, ab.X ) );
		}
		public static Vector3 FollowedAngle ( Vector3 a, Vector3 b )
		{
			return new Vector3 (
				FollowedAngle ( new Vector2 ( a.Z, a.Y ), new Vector2 ( b.Z, b.Y ) ),
				FollowedAngle ( new Vector2 ( a.X, a.Z ), new Vector2 ( b.X, b.Z ) ),
				FollowedAngle ( new Vector2 ( a.X, a.Y ), new Vector2 ( b.X, b.Y ) )
			);
		}

		public static float Distance ( Vector2 a, Vector2 b )
		{
			Vector2 ab = Abs ( a - b );
			return ( ab * ab ).Length ();
		}
	}
}
