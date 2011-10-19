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

        private SKraft game;
        private Vector3 position;
        public Vector3 target { get; set; }
        private Vector2 mousePos;

        public FppCamera(SKraft _game, Vector3 position, Vector3 target, Vector3 up) : base(_game, position, target, up)
        {
            this.position = position;
            this.target = target;

            this.game = game;

            Speed = 0.1f;
            MouseSpeed = 5;
        }

        public override void Initialize()
        {
            mousePos.X = Mouse.GetState().X;
            mousePos.Y = Mouse.GetState().Y;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            View = Matrix.CreateTranslation(-position);
            View *= Matrix.CreateRotationY(target.X) * Matrix.CreateRotationX(target.Y);
            base.Update(gameTime);
        }

        public void Move(float x, float z)
        {
            position.X += x;
            position.Z -= z;
        }
    }
}
