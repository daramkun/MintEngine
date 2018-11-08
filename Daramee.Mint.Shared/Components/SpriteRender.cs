using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Components
{
	public sealed class SpriteRender : IComponent, IOrderable, IRenderable
	{
		public Texture2D Sprite;
		public Color OverlayColor;
		public int SortOrder { get; set; }
		public bool IsVisible { get; set; }

		public void Initialize ()
		{
			Sprite = null;
			OverlayColor = Color.White;
			SortOrder = 0;
			IsVisible = true;
		}

		public void CopyFrom ( IComponent component )
		{
			if ( component is SpriteRender )
			{
				Sprite = ( component as SpriteRender ).Sprite;
				OverlayColor = ( component as SpriteRender ).OverlayColor;
				SortOrder = ( component as SpriteRender ).SortOrder;
				IsVisible = ( component as SpriteRender ).IsVisible;
			}
		}
	}
}
