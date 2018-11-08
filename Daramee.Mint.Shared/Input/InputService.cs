using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramee.Mint.Input
{
	public enum LatestUpdated { KeyboardAndMouse, GamePad }
	public class InputService
	{
		public static InputService SharedInputService { get; private set; }
		public static Vector2 CorrectionPosition ( Vector2 windowResolution )
		{
			return ( windowResolution - Engine.SharedEngine.backBufferArea.Location.ToVector2 () ) * Engine.SharedEngine.backBufferScaleVector;
		}

		LatestUpdated latest = LatestUpdated.KeyboardAndMouse;

		KeyboardState lastKeyboardState, currentKeyboardState;
		MouseState lastMouseState, currentMouseState;
		GamePadState lastGamePadState, currentGamePadState;
		TouchCollection lastTouchState, currentTouchState;

		public KeyboardState LastKeyboardState => lastKeyboardState;
		public KeyboardState CurrentKeyboardState => currentKeyboardState;
		public MouseState LastMouseState => lastMouseState;
		public MouseState CurrentMouseState => currentMouseState;
		public GamePadState LastGamePadState => lastGamePadState;
		public GamePadState CurrentGamePadState => currentGamePadState;
		public TouchCollection LastTouchCollection => lastTouchState;
		public TouchCollection CurrentTouchCollection => currentTouchState;

		public LatestUpdated LatestUpdate => latest;

		public Func<GamePadState> GettingGamePadState;

		internal InputService ()
		{
			if ( SharedInputService != null )
				throw new InvalidOperationException ();

			Reset ();

			SharedInputService = this;
		}
		~InputService ()
		{
			SharedInputService = null;
		}

		public void Reset ()
		{
			currentKeyboardState = lastKeyboardState = Keyboard.GetState ();
			currentMouseState = lastMouseState = Mouse.GetState ();
			currentGamePadState = lastGamePadState = GettingGamePadState?.Invoke () ?? GamePad.GetState ( PlayerIndex.One );
			currentTouchState = lastTouchState = TouchPanel.GetState ();
		}

		public void Update ()
		{
			lastKeyboardState = currentKeyboardState;
			lastMouseState = currentMouseState;
			lastGamePadState = currentGamePadState;
			lastTouchState = currentTouchState;

			currentKeyboardState = Keyboard.GetState ();
			currentMouseState = Mouse.GetState ();
			currentGamePadState = GettingGamePadState?.Invoke () ?? GamePad.GetState ( PlayerIndex.One );
			currentTouchState = TouchPanel.GetState ();

			if ( ( currentKeyboardState.Equals ( lastKeyboardState ) || currentMouseState.Equals ( lastMouseState ) || currentTouchState.Equals ( lastTouchState ) )
				&& !currentGamePadState.Equals ( lastGamePadState ) )
				latest = LatestUpdated.GamePad;
			else if ( !currentKeyboardState.Equals ( lastKeyboardState ) && currentGamePadState.Equals ( lastGamePadState ) )
				latest = LatestUpdated.KeyboardAndMouse;
		}

		public bool IsKeyPress ( Keys key ) => currentKeyboardState.IsKeyDown ( key );
		public bool IsKeyDown ( Keys key ) => currentKeyboardState.IsKeyDown ( key ) && lastKeyboardState.IsKeyUp ( key );
		public bool IsKeyUp ( Keys key ) => currentKeyboardState.IsKeyUp ( key ) && lastKeyboardState.IsKeyDown ( key );
		public bool IsAnyKeyPress ( params Keys [] exclude )
			=> ( from key in currentKeyboardState.GetPressedKeys () where exclude == null || !exclude.Contains ( key ) select key ).Count () > 0;

		public Vector2 CurrentMousePosition => CorrectionPosition ( currentMouseState.Position.ToVector2 () );
		public Vector2 LastMousePosition => CorrectionPosition ( lastMouseState.Position.ToVector2 () );
		public bool IsMouseButtonPress ( int button )
		{
			switch ( button )
			{
				case 0: return currentMouseState.LeftButton == ButtonState.Pressed;
				case 1: return currentMouseState.RightButton == ButtonState.Pressed;
				case 2: return currentMouseState.MiddleButton == ButtonState.Pressed;
				case 3: return currentMouseState.XButton1 == ButtonState.Pressed;
				case 4: return currentMouseState.XButton2 == ButtonState.Pressed;
				default: throw new ArgumentOutOfRangeException ();
			}
		}
		public bool IsMouseButtonDown ( int button )
		{
			switch ( button )
			{
				case 0: return currentMouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released;
				case 1: return currentMouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released;
				case 2: return currentMouseState.MiddleButton == ButtonState.Pressed && lastMouseState.MiddleButton == ButtonState.Released;
				case 3: return currentMouseState.XButton1 == ButtonState.Pressed && lastMouseState.XButton1 == ButtonState.Released;
				case 4: return currentMouseState.XButton2 == ButtonState.Pressed && lastMouseState.XButton2 == ButtonState.Released;
				default: throw new ArgumentOutOfRangeException ();
			}
		}
		public bool IsMouseButtonUp ( int button )
		{
			switch ( button )
			{
				case 0: return currentMouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed;
				case 1: return currentMouseState.RightButton == ButtonState.Released && lastMouseState.RightButton == ButtonState.Pressed;
				case 2: return currentMouseState.MiddleButton == ButtonState.Released && lastMouseState.MiddleButton == ButtonState.Pressed;
				case 3: return currentMouseState.XButton1 == ButtonState.Released && lastMouseState.XButton1 == ButtonState.Pressed;
				case 4: return currentMouseState.XButton2 == ButtonState.Released && lastMouseState.XButton2 == ButtonState.Pressed;
				default: throw new ArgumentOutOfRangeException ();
			}
		}

		public bool IsGamePadButtonPress ( Buttons button ) => currentGamePadState.IsButtonDown ( button );
		public bool IsGamePadButtonDown ( Buttons button ) => currentGamePadState.IsButtonDown ( button ) && lastGamePadState.IsButtonUp ( button );
		public bool IsGamePadButtonUp ( Buttons button ) => currentGamePadState.IsButtonUp ( button ) && lastGamePadState.IsButtonDown ( button );

		public Vector2 RelativeMousePosition => new Vector2 ( currentMouseState.X - lastMouseState.X, currentMouseState.Y - lastMouseState.Y );
	}
}
