using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SKraft.Menu
{
    class MainMenu
    {
        private List<Button> buttons = new List<Button>();
        private Texture2D background;
        private SKraft skraft;

        private bool isMenuVisible;

        public bool IsMenuVisible
        {
            get { return isMenuVisible; }
            set { skraft.IsMouseVisible = value; isMenuVisible = value; }
        }


        public MainMenu(SKraft skraft)
        {
            this.skraft = skraft;
            IsMenuVisible = true;

            buttons.Add(new Button(new Vector2(0, 174 * SKraft.Graphics.Viewport.Height / 600)));
            buttons.Add(new Button(new Vector2(0, 216 * SKraft.Graphics.Viewport.Height / 600)));
        }

        public void LoadContent()
        {
            background = SKraft.SKraftContent.Load<Texture2D>("Menu\\menubackground");
            buttons[0].LoadContent(new string[] { "newgame", "newgameactiv" });

            buttons[0].OnMouseClick += new Button.MouseClick(MainMenu_OnMouseClick);

            buttons[1].LoadContent(new string[] { "quitgame", "quitgameactiv" });
            buttons[1].OnMouseClick += new Button.MouseClick(MainMenu_OnMouseClick2);
        }

        private void MainMenu_OnMouseClick()
        {
            IsMenuVisible = false;
        }

        private void MainMenu_OnMouseClick2()
        {
            skraft.Exit();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsMenuVisible)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(background, new Rectangle(0, 0, SKraft.Graphics.Viewport.Width, SKraft.Graphics.Viewport.Height), Color.White);
                foreach (Button btn in buttons)
                {
                    btn.Draw(spriteBatch);
                }

                //spriteBatch.Draw(SKraft.SKraftContent.Load<Texture2D>("Menu\\newgame"), Vector2.Zero, Color.White);
                spriteBatch.End();
            }
        }
    }
}
