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
        public Vector3 Target { get; set; }
        private Vector2 mousePos;

        public FppCamera(SKraft game, Vector3 position, Vector3 target, Vector3 up) : base(game, position, target, up)
        {
            this.position = position;
            this.Target = target;

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
            View *= Matrix.CreateRotationY(Target.X) * Matrix.CreateRotationX(Target.Y);
            base.Update(gameTime);
        }

        public void Move(Vector3 move)
        {
            position.X += move.X;
            position.Y += move.Y;
            position.Z -= move.Z;
        }
    }
}
