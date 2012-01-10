using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SKraft.Menu
{
    public class Button
    {
        private Vector2 position;
        private Texture2D[] textures = new Texture2D[2];
        private Rectangle buttonRect;
        public delegate void MouseClick();
        public event MouseClick OnMouseClick;

        public Button(Vector2 position)
        {
            this.position = position;
            buttonRect = new Rectangle((int)position.X, (int)position.Y, 0, 0);
        }

        public void LoadContent(string[] fileNames)
        {
            for (int i = 0; i < fileNames.Length; ++ i) 
            {
                this.textures[i] = SKraft.SKraftContent.Load<Texture2D>("Menu\\" + fileNames[i]);
            }            
        }

        private bool IsOver()
        {
            MouseState state = Mouse.GetState();
            if (buttonRect.Intersects(new Rectangle(state.X, state.Y, 1, 1)))
            {
                return true;
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            buttonRect.Width = (int)(textures[0].Width * SKraft.Graphics.Viewport.Width / 1024);
            buttonRect.Height = (int)(textures[0].Height * SKraft.Graphics.Viewport.Height / 768);
            
            if (!IsOver())
            {
                spriteBatch.Draw(textures[0], buttonRect, Color.White);
            }
            else
            {
                spriteBatch.Draw(textures[1], buttonRect, Color.White);

                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    OnMouseClick();
                }
            }
        }
    }
}
