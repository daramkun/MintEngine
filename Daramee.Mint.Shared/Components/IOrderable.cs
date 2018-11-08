using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Components
{
	public interface IOrderable
	{
		int SortOrder { get; }
	}
}
