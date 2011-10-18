using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SKraft.Cameras
{
    class FppCamera : Camera
    {
        public float Speed { get; set; }
        public float MouseSpeed { get; set; }

        private Game game;
        private Vector3 position;
        private Vector3 target;

        public FppCamera(Game game, Vector3 position, Vector3 target, Vector3 up) : base(game, position, target, up)
        {
            this.position = position;
            this.target = target;

            this.game = game;

            Speed = 0.1f;
            MouseSpeed = 5;
        }

        public override void Initialize()
        {
            target.X += (Mouse.GetState().X - game.GraphicsDevice.Viewport.Width / 2) * MouseSpeed / 1000;
            target.Y += (Mouse.GetState().Y - game.GraphicsDevice.Viewport.Height / 2) * MouseSpeed / 1000;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (game.IsActive)
            {
                target.X += (Mouse.GetState().X - game.GraphicsDevice.Viewport.Width / 2) * MouseSpeed / 1000;
                target.Y += (Mouse.GetState().Y - game.GraphicsDevice.Viewport.Height / 2) * MouseSpeed / 1000;

                if (MathHelper.ToDegrees(target.Y) > 90)
                {
                    target.Y = MathHelper.ToRadians(90);
                }
                else if (MathHelper.ToDegrees(target.Y) < -90)
                {
                    target.Y = MathHelper.ToRadians(-90);
                }

                Mouse.SetPosition(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2);

                Matrix forwardMovement = Matrix.CreateRotationY(target.X);
                KeyboardState state = Keyboard.GetState();
                if (state.IsKeyDown(Keys.W))
                {
                    Vector3 v = new Vector3(0, 0, Speed);
                    v = Vector3.Transform(v, forwardMovement);
                    position.Z -= v.Z;
                    position.X += v.X;
                }

                if (state.IsKeyDown(Keys.S))
                {
                    Vector3 v = new Vector3(0, 0, -Speed);
                    v = Vector3.Transform(v, forwardMovement);
                    position.Z -= v.Z;
                    position.X += v.X;
                }

                if (state.IsKeyDown(Keys.A))
                {
                    Vector3 v = new Vector3(-Speed, 0, 0);
                    v = Vector3.Transform(v, forwardMovement);
                    position.Z -= v.Z;
                    position.X += v.X;
                }

                if (state.IsKeyDown(Keys.D))
                {
                    Vector3 v = new Vector3(Speed, 0, 0);
                    v = Vector3.Transform(v, forwardMovement);
                    position.Z -= v.Z;
                    position.X += v.X;
                }


                View = Matrix.CreateTranslation(-position);
                View *= Matrix.CreateRotationY(target.X) * Matrix.CreateRotationX(target.Y);
            }

            base.Update(gameTime);
        }
    }
}
