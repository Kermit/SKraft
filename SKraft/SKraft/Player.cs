using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SKraft.Cameras;
using Microsoft.Xna.Framework.Input;

namespace SKraft
{
    class Player : DrawableGameComponent
    {
        protected Model model;
        protected Vector3 position;
        protected Texture2D texture;
        private Matrix[] transforms;
        private FppCamera fppCamera;

        public float Speed { get; set; }
        public float MouseSpeed { get; set; }
        private Vector3 target;
        private Vector2 mousePos;

        public Player(SKraft game, Vector3 position) : base(game)
        {
            this.position = position;
            fppCamera = new FppCamera(game, new Vector3(position.X, position.Y + 1, position.Z), Vector3.Zero, Vector3.Up);

            mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Speed = 0.1f;
            MouseSpeed = 10;

            Game.Components.Add(fppCamera);
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>(@"models\player");
            texture = Game.Content.Load<Texture2D>(@"textures\texture2");

            transforms = new Matrix[model.Bones.Count];           

            base.LoadContent();
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

            if (Game.IsActive)
            {
                if (mousePos.X <= 0)
                {
                    mousePos.X = this.GraphicsDevice.Viewport.Width;
                    Mouse.SetPosition((int)mousePos.X, (int)mousePos.Y);
                }
                else if (mousePos.X >= this.GraphicsDevice.Viewport.Width)
                {
                    mousePos.X = 0;
                    Mouse.SetPosition((int)mousePos.X, (int)mousePos.Y);
                }

                if (mousePos.Y <= 0)
                {
                    mousePos.Y = 0;
                    Mouse.SetPosition((int)mousePos.X, (int)mousePos.Y);
                }
                else if (mousePos.Y >= this.GraphicsDevice.Viewport.Height)
                {
                    mousePos.Y = this.GraphicsDevice.Viewport.Height;
                    Mouse.SetPosition((int)mousePos.X, (int)mousePos.Y);
                }
            }

            Matrix forwardMovement = Matrix.CreateRotationY(target.X);
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.W))
            {
                Vector3 v = new Vector3(0, 0, Speed);
                v = Vector3.Transform(v, forwardMovement);
                fppCamera.Move(v.X, v.Z);
                Move(v.X, v.Z);
            }

            if (state.IsKeyDown(Keys.S))
            {
                Vector3 v = new Vector3(0, 0, -Speed);
                v = Vector3.Transform(v, forwardMovement);
                fppCamera.Move(v.X, v.Z);
                Move(v.X, v.Z);
            }

            if (state.IsKeyDown(Keys.A))
            {
                Vector3 v = new Vector3(-Speed, 0, 0);
                v = Vector3.Transform(v, forwardMovement);
                fppCamera.Move(v.X, v.Z);
                Move(v.X, v.Z);
            }

            if (state.IsKeyDown(Keys.D))
            {
                Vector3 v = new Vector3(Speed, 0, 0);
                v = Vector3.Transform(v, forwardMovement);
                fppCamera.Move(v.X, v.Z);
                Move(v.X, v.Z);
            }

            fppCamera.target = new Vector3(target.X, target.Y, 0);

            base.Update(gameTime);
            Rotate(target.X, target.Y);
        }

        public override void Draw(GameTime gameTime)
        {
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.FogEnabled = true;
                    effect.FogColor = Color.CornflowerBlue.ToVector3();
                    effect.FogStart = 17f;
                    effect.FogEnd = 21f;

                    if (texture != null)
                    {
                        effect.Texture = texture;
                    }

                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(position);
                    effect.View = Camera.ActiveCamera.View;
                    effect.Projection = Camera.ActiveCamera.Projection;
                }

                mesh.Draw();
            }

            base.Draw(gameTime);
        }

        public void Move(float x, float z)
        {
            position.Z -= z;
            position.X += x;
        }

        public void Rotate(float x, float y)
        {
            model.Root.Transform = Matrix.CreateRotationY(-x);
        }
    }
}
