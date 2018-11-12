#region Using Statements
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Daramee.Mint.Coroutines;
using Daramee.Mint.Diagnostics;
using Daramee.Mint.Entities;
using Daramee.Mint.Graphics;
using Daramee.Mint.Input;
using Daramee.Mint.Processors;
using Daramee.Mint.Scenes;
using Daramee.Mint.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Daramee.Mint
{
	public class Engine : Game
	{
		public static Engine SharedEngine { get; private set; }

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		FrameBuffer frameBuffer;
		bool isCustomSize;

		public GraphicsDeviceManager GraphicsDeviceManager => graphics;
		public SpriteBatch SpriteBatcher => spriteBatch;
		public FrameBuffer FrameBuffer => frameBuffer;

		public Color FrameBufferClearColor = Color.CornflowerBlue, LetterBoxColor = new Color ( 32, 32, 32 );
		public SamplerState FrameBufferDrawSampler = SamplerState.PointClamp;
		public Effect FrameBufferDrawEffect = null;
		
		internal Rectangle backBufferArea;
		internal Vector2 backBufferScaleVector;
		void CalculateBackBufferArea ()
		{
			backBufferScaleVector = new Vector2 ( graphics.PreferredBackBufferWidth / frameBuffer.Size.X, graphics.PreferredBackBufferHeight / frameBuffer.Size.Y );
			var scale = MathHelper.Min ( backBufferScaleVector.X, backBufferScaleVector.Y );
			backBufferArea = new Rectangle (
				( int ) ( backBufferScaleVector.X < backBufferScaleVector.Y ? 0 : ( ( graphics.PreferredBackBufferWidth / 2 ) - ( frameBuffer.Size.X * scale / 2 ) ) ),
				( int ) ( backBufferScaleVector.X > backBufferScaleVector.Y ? 0 : ( ( graphics.PreferredBackBufferHeight / 2 ) - ( frameBuffer.Size.Y * scale / 2 ) ) ),
				( int ) Math.Floor ( frameBuffer.Size.X * scale ),
				( int ) Math.Floor ( frameBuffer.Size.Y * scale )
			);
			InputService.SharedInputService.Reset ();
		}

		public Engine ( string contentDirectory = "Content", Vector2? screenSize = null, bool fullscreen = true, bool exclusiveFullscreen = true,
			string firstSceneName = null, params Scene [] scenes )
		{
			if ( SharedEngine != null )
				throw new InvalidOperationException ();

			graphics = new GraphicsDeviceManager ( this )
			{
				IsFullScreen = fullscreen,
				HardwareModeSwitch = exclusiveFullscreen
			};

			Window.AllowUserResizing = true;
			Window.ClientSizeChanged += ( sender, e ) =>
			{
				if ( graphics == null || GraphicsDevice == null )
					return;
				graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
				graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
				graphics.ApplyChanges ();

				if ( !isCustomSize )
					frameBuffer.Size = new Vector2 ( graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight );

				CalculateBackBufferArea ();
			};
			
			frameBuffer = new FrameBuffer ( screenSize ?? new Vector2 ( graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight ) );
			frameBuffer.SizeChanged += ( sender, e ) => CalculateBackBufferArea ();
			isCustomSize = screenSize != null;

			Content.RootDirectory = contentDirectory;

			Services.AddService ( new EntityManager () );
			Services.AddService ( new SystemManager () );
			Services.AddService ( new ProcessorManager () );
			Services.AddService ( new SceneManager ( firstSceneName, scenes ) );
			Services.AddService ( new Coroutine () );
			Services.AddService ( new Logger () );
			Services.AddService ( new InputService () );

			ProcessorManager.SharedManager.RegisterProcessor ( new FrameRateCalculateProcessor () );

			SharedEngine = this;
		}

		protected override void Dispose ( bool disposing )
		{
			if ( disposing )
			{
				EntityManager.SharedManager.Dispose ();
				SharedEngine = null;
			}
			base.Dispose ( disposing );
		}
		
		protected override void Initialize ()
		{
			base.Initialize ();
		}
		
		protected override void LoadContent ()
		{
			spriteBatch = new SpriteBatch ( GraphicsDevice );
			CalculateBackBufferArea ();
			
			SystemManager.SharedManager.RegisterSystem ( new SpriteRenderSystem () );
			SystemManager.SharedManager.RegisterSystem ( new AudioSystem () );

			SceneManager.SharedManager.StartScene ();
		}
		
		protected override void Update ( GameTime gameTime )
		{
			if ( InputService.SharedInputService.IsKeyPress ( Keys.LeftAlt )
				&& InputService.SharedInputService.IsKeyUp ( Keys.Enter ) )
				graphics.ToggleFullScreen ();

			base.Update ( gameTime );
		}
		
		protected override void Draw ( GameTime gameTime )
		{
			InputService.SharedInputService.Update ();
			ProcessorManager.SharedManager.Process ( gameTime );

			GraphicsDevice.SetRenderTarget ( frameBuffer.RenderTarget );
			GraphicsDevice.Viewport = new Viewport ( 0, 0, ( int ) frameBuffer.Size.X, ( int ) frameBuffer.Size.Y );
			GraphicsDevice.Clear ( FrameBufferClearColor );

			SystemManager.SharedManager.Execute ( gameTime );
			Coroutine.SharedCoroutine.DoCoroutines ( gameTime );

			GraphicsDevice.SetRenderTarget ( null );
			GraphicsDevice.Clear ( LetterBoxColor );
			GraphicsDevice.Viewport = new Viewport ( 0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight );
			
			spriteBatch.Begin ( SpriteSortMode.Immediate, blendState: BlendState.NonPremultiplied, samplerState: FrameBufferDrawSampler, effect: FrameBufferDrawEffect );
			spriteBatch.Draw ( frameBuffer.RenderTarget, backBufferArea, Color.White );
			spriteBatch.End ();

			base.Draw ( gameTime );
		}
	}
}
