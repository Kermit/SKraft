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


namespace SKraft
{
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        public Matrix Projection { get; protected set; }
        public Matrix View { get; protected set; }

        private Vector2 mousePos;

        public Camera(Game game, Vector3 position, Vector3 target, Vector3 up)
            : base(game)
        {
            View = Matrix.CreateLookAt(position, target, up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                             (float)Game.Window.ClientBounds.Width /
                                                             (float)Game.Window.ClientBounds.Height, 1, 3000);

            mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.W))
            {
                View *= Matrix.CreateTranslation(0, 0, 0.1f);
            }

            if (state.IsKeyDown(Keys.S))
            {
                View *= Matrix.CreateTranslation(0, 0, -0.1f);
            }

            if (state.IsKeyDown(Keys.A))
            {
                View *= Matrix.CreateTranslation(0.1f, 0, 0);
            }

            if (state.IsKeyDown(Keys.D))
            {
                View *= Matrix.CreateTranslation(-0.1f, 0, 0);
            }

            View *= Matrix.CreateRotationY((Mouse.GetState().X - mousePos.X) / 75);
            View *= Matrix.CreateRotationX((Mouse.GetState().Y - mousePos.Y) / 75);
            mousePos.X = Mouse.GetState().X;
            mousePos.Y = Mouse.GetState().Y;

            base.Update(gameTime);
        }
    }
}
