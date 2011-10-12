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

        private Vector3 position;
        private Vector3 target;
        private Vector2 mousePos;

        public FppCamera(Game game, Vector3 position, Vector3 target, Vector3 up) : base(game, position, target, up)
        {
            this.position = position;
            this.target = target;

            mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Speed = 0.1f;
            MouseSpeed = 10;
        }

        public override void Initialize()
        {
            target.X += (Mouse.GetState().X - mousePos.X) * MouseSpeed / 1000;
            target.Y += (Mouse.GetState().Y - mousePos.Y) * MouseSpeed / 1000;
            mousePos.X = Mouse.GetState().X;
            mousePos.Y = Mouse.GetState().Y;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            target.X += (Mouse.GetState().X - mousePos.X) * MouseSpeed / 1000;
            target.Y += (Mouse.GetState().Y - mousePos.Y) * MouseSpeed / 1000;

            if (MathHelper.ToDegrees(target.Y) > 90)
            {
                target.Y = MathHelper.ToRadians(90);
            }
            else if (MathHelper.ToDegrees(target.Y) < -90)
            {
                target.Y = MathHelper.ToRadians(-90);
            }

            mousePos.X = Mouse.GetState().X;
            mousePos.Y = Mouse.GetState().Y;

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

            base.Update(gameTime);
        }
    }
}
