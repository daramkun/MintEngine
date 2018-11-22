using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Components
{
	public sealed class RectangleRender : IComponent, IOrderable, IRenderable
	{
		public Vector2 Size;
		public Color Color;
		public bool Fill;
		public int SortOrder { get; set; }
		public bool IsVisible { get; set; }
		public bool IsCameraIndependency { get; set; }

		public void Initialize ()
		{
			Size = new Vector2 ();
			Color = Color.Black;
			Fill = false;
			SortOrder = 0;
			IsVisible = true;
		}

		public void CopyFrom ( IComponent component )
		{
			if ( component is RectangleRender )
			{
				Size = ( component as RectangleRender ).Size;
				Color = ( component as RectangleRender ).Color;
				Fill = ( component as RectangleRender ).Fill;
				SortOrder = ( component as RectangleRender ).SortOrder;
				IsVisible = ( component as RectangleRender ).IsVisible;
			}
		}
	}
}
