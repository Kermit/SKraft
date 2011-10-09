using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SKraft.Cameras
{
    class FreeCamera : Camera
    {
        public override Matrix View { get; protected set; }

        private Vector2 mousePos;

        public FreeCamera(Game game, Vector3 position, Vector3 target, Vector3 up) : base(game, position, target, up) { }

        public override void Initialize()
        {
            mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            base.Initialize();
        }

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

            if (state.IsKeyDown(Keys.Space))
            {
                View *= Matrix.CreateTranslation(0, -0.1f, 0);
            }

            if (state.IsKeyDown(Keys.C))
            {
                View *= Matrix.CreateTranslation(0, 0.1f, 0);
            }

            View *= Matrix.CreateRotationY((Mouse.GetState().X - mousePos.X) * 1.0f / 75f);
            View *= Matrix.CreateRotationX((Mouse.GetState().Y - mousePos.Y) * 1.0f / 75f);
            mousePos.X = Mouse.GetState().X;
            mousePos.Y = Mouse.GetState().Y;

            base.Update(gameTime);
        }
    }
}
