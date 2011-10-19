using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;


namespace SKraft
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Debug : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Game game;
        private SpriteFont font;
        private SpriteBatch spriteBatch;
        private KeyboardState oldState;
        private static List<String> stringList;
        private Vector2 stringMeasure;
        private static Dictionary<String, Stopwatch> timers;
        private bool show = true;

        public Debug(Game game)
            : base(game)
        {
            this.game = game;

            stringList = new List<String>();
            timers = new Dictionary<String, Stopwatch>();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            font = game.Content.Load<SpriteFont>("DejavuSans");
            stringMeasure = font.MeasureString("jy");
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            KeyboardState newState = Keyboard.GetState();

            if (newState.IsKeyDown(Keys.F3))
            {
                if (!oldState.IsKeyDown(Keys.F3))
                {
                    show = !show;
                }
            }

            oldState = newState;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (show && stringList.Count != 0)
            {
                spriteBatch.Begin();
                for (int x = 0; x < stringList.Count; ++x)
                {
                    spriteBatch.DrawString(font, stringList[x], new Vector2(0, stringMeasure.Y * x), Color.Black);
                }
                spriteBatch.End();
                stringList.Clear();
            }

            base.Draw(gameTime);
        }

        static public void AddString(String stringToAdd)
        {
            stringList.Add(stringToAdd);
        }

        static public void AddStringTimer(String stringToAdd, String timerID)
        {
            stringList.Add(String.Format("{0} - {1} ms", stringToAdd, timers[timerID].ElapsedMilliseconds));
            Reset(timerID);
        }

        static public void Start(String timerID)
        {
            if (!timers.ContainsKey(timerID))
            {
                timers.Add(timerID, new Stopwatch());
            }
            timers[timerID].Start();
        }

        static public void Stop(String timerID)
        {
            timers[timerID].Stop();
        }

        static public void Reset(String timerID)
        {
            timers[timerID].Reset();
        }

    }
}