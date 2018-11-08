using Daramee.Mint.Entities;
using Daramee.Mint.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Windows.Scenes
{
	public class TestScene : Scene
	{
		public override string Name => "TestScene";

		protected override void Enter ()
		{
			var entity = EntityManager.SharedManager.CreateEntity ();

		}

		protected override void Exit ()
		{

		}
	}
}
