using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SKraft.Cameras
{
    class FppCamera : Camera
    {
        public override Matrix View { get; protected set; }

        /*private Vector3 angle = new Vector3();
        private Vector3 position;
        private float speed = 1f;
        private float turnSpeed = 90f;*/

        private Vector2 mousePos;
        private Vector3 position;
        private float yaw;
        private float pitch;

        public FppCamera(Game game, Vector3 position, Vector3 target, Vector3 up) : base(game, position, target, up)
        {
            this.position = position;
        }

        public override void Initialize()
        {
            /*int centerX = Game.Window.ClientBounds.Width / 2;
            int centerY = Game.Window.ClientBounds.Height / 2;

            Mouse.SetPosition(centerX, centerY);

            mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);*/

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            /*float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            int centerX = Game.Window.ClientBounds.Width / 2;
            int centerY = Game.Window.ClientBounds.Height / 2;

            Mouse.SetPosition(centerX, centerY);

            angle.X += MathHelper.ToRadians((mouse.Y - centerY) * turnSpeed * 0.01f); // pitch
            angle.Y += MathHelper.ToRadians((mouse.X - centerX) * turnSpeed * 0.01f); // yaw

            Vector3 forward = Vector3.Normalize(new Vector3((float)Math.Sin(-angle.Y), (float)Math.Sin(angle.X), (float)Math.Cos(-angle.Y)));
            System.Diagnostics.Debug.WriteLine(forward);
            Vector3 left = Vector3.Normalize(new Vector3((float)Math.Cos(angle.Y), 0f, (float)Math.Sin(angle.Y)));
            
            if (keyboard.IsKeyDown(Keys.W))
            {
                position -= forward*speed*delta;
            }

            if (keyboard.IsKeyDown(Keys.S))
            {
                position += forward*speed*delta;
            }

            if (keyboard.IsKeyDown(Keys.D))
            {
                position -= left*speed*delta;
            }

            if (keyboard.IsKeyDown(Keys.A))
            {
                position += left*speed*delta;
            }

            if (keyboard.IsKeyDown(Keys.Space))
            {
                position += Vector3.Down*speed*delta;
            }

            if (keyboard.IsKeyDown(Keys.C))
            {
                position += Vector3.Up*speed*delta;
            }

            View = Matrix.Identity;
            View *= Matrix.CreateTranslation(-position);
            View *= Matrix.CreateRotationZ(angle.Z);
            View *= Matrix.CreateRotationY(angle.Y);
            View *= Matrix.CreateRotationX(angle.X);*/

            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.W))
            {
                Matrix forwardMovement = Matrix.CreateRotationY(yaw) * Matrix.CreateRotationX(pitch);
                Vector3 v = Vector3.Backward;
                v = Vector3.Transform(v, forwardMovement);
                //position.Z += v.Z;
                //position.X += v.X;

                View *= Matrix.CreateTranslation(v.X, 0, v.Z);
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

            yaw = (Mouse.GetState().X - mousePos.X) * 1.0f/75f;
            pitch = (Mouse.GetState().Y - mousePos.Y)*1.0f/75f;

            //View = Matrix.Identity;
            //View = Matrix.CreateTranslation(position);
            View *= Matrix.CreateRotationY(yaw);
            View *= Matrix.CreateRotationX(pitch);
            //View *= Matrix.CreateRotationY((Mouse.GetState().X - mousePos.X) * 1.0f / 75f);
            //View *= Matrix.CreateRotationX((Mouse.GetState().Y - mousePos.Y) * 1.0f / 75f);


            mousePos.X = Mouse.GetState().X;
            mousePos.Y = Mouse.GetState().Y;


            base.Update(gameTime);
        }
    }
}
