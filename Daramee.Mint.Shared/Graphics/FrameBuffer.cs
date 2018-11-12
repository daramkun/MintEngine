using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Graphics
{
	public enum FrameBufferScale
	{
		Scale025 = 0, Scale050 = 1, Scale075 = 2,
		Scale100 = 3, Scale125 = 4, Scale150 = 5,
		Scale200 = 6, Scale300 = 7, Scale400 = 8,
	}

	public sealed class FrameBuffer : IDisposable
	{
		Vector2 size;
		bool msaa = false;
		FrameBufferScale scale = FrameBufferScale.Scale100;
		int msaaSampleCount = 2;

		public event EventHandler SizeChanged;

		public Vector2 Size
		{
			get { return size; }
			set
			{
				if ( size != value )
				{
					size = value;
					RegenerateFrameBuffer ();
					SizeChanged?.Invoke ( this, EventArgs.Empty );
				}
			}
		}
		
		public FrameBufferScale Scale { get { return scale; } set { scale = value; } }

		public bool TurnOnMSAA { get { return msaa; } set { if ( msaa != value ) { msaa = value; RegenerateFrameBuffer (); } } }
		public int MSAASampleCount { get { return msaaSampleCount; } set { if ( msaaSampleCount != value ) { msaaSampleCount = value; RegenerateFrameBuffer (); } } }

		RenderTarget2D [] renderTargets = new RenderTarget2D [ 9 ];

		public RenderTarget2D RenderTarget
		{
			get
			{
				if ( renderTargets [ ( int ) scale ] == null )
				{
					if ( msaa )
					{
						if ( msaaSampleCount < 1 )
							msaaSampleCount = 2;
					}
					Vector2 sizeScaled = size;
					switch ( scale )
					{
						case FrameBufferScale.Scale025: sizeScaled *= 0.25f; break;
						case FrameBufferScale.Scale050: sizeScaled *= 0.5f; break;
						case FrameBufferScale.Scale075: sizeScaled *= 0.75f; break;
						case FrameBufferScale.Scale125: sizeScaled *= 1.25f; break;
						case FrameBufferScale.Scale150: sizeScaled *= 1.5f; break;
						case FrameBufferScale.Scale200: sizeScaled *= 2; break;
						case FrameBufferScale.Scale300: sizeScaled *= 3; break;
						case FrameBufferScale.Scale400: sizeScaled *= 4; break;
					}
					renderTargets [ ( int ) scale ] = new RenderTarget2D ( Engine.SharedEngine.GraphicsDevice,
						( int ) sizeScaled.X, ( int ) sizeScaled.Y, false,
						SurfaceFormat.Bgra32, DepthFormat.Depth24Stencil8,
						msaaSampleCount, RenderTargetUsage.PlatformContents );
				}
				return renderTargets [ ( int ) scale ];
			}
		}

		public FrameBuffer ( Vector2 size )
		{
			Size = size;
		}

		public void Dispose ()
		{
			foreach ( var renderTarget in renderTargets )
				renderTarget.Dispose ();
			renderTargets = null;
		}

		private void RegenerateFrameBuffer ()
		{
			for ( int i = 0; i < 9; ++i )
			{
				renderTargets [ i ]?.Dispose ();
				renderTargets [ i ] = null;
			}
		}
	}
}
