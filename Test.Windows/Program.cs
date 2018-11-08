using Daramee.Mint;
using Microsoft.Xna.Framework;
using System;
using Test.Windows.Scenes;

namespace Test.Windows
{
#if WINDOWS || LINUX
	/// <summary>
	/// The main class.
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main ()
		{
			using ( var game = new Engine (
				screenSize: new Vector2 ( 176, 178 ),
				fullscreen: false,
				exclusiveFullscreen: false,
				firstSceneName: "TestScene",
				scenes: new TestScene () ) )
				game.Run ();
		}
	}
#endif
}
