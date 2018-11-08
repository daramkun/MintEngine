using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Processors
{
	public interface IProcessor
	{
		void Process ( GameTime gameTime );
	}
}
