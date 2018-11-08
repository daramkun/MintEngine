using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daramee.Mint.Components
{
	public sealed class TextRender : IComponent, IOrderable, IRenderable
	{
		public SpriteFont Font;
		public string Text;
		public Color ForegroundColor;
		public int SortOrder { get; set; }
		public bool IsVisible { get; set; }

		public void Initialize ()
		{
			Font = null;
			Text = "";
			ForegroundColor = Color.White;
			SortOrder = 0;
			IsVisible = true;
		}

		public void CopyFrom ( IComponent component )
		{
			if ( component is TextRender )
			{
				Font = ( component as TextRender ).Font;
				Text = ( component as TextRender ).Text;
				ForegroundColor = ( component as TextRender ).ForegroundColor;
				SortOrder = ( component as TextRender ).SortOrder;
				IsVisible = ( component as TextRender ).IsVisible;
			}
		}
	}
}
