using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Components
{
	public interface IRenderable
	{
		bool IsVisible { get; }
		bool IsCameraIndependency { get; }
	}
}
