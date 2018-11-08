using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Components
{
	public interface IComponent
	{
		void Initialize ();
		void CopyFrom ( IComponent component );
	}
}
