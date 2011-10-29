using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SKraft.Cameras;
using Microsoft.Xna.Framework.Input;
using SKraft.Cubes;
using SKraft.MapGen;

namespace SKraft
{
    public class Player : Object3D
    {
        private SKraft game;
        private Matrix[] transforms;
        private FppCamera fppCamera;

        public float Speed { get; set; }
        public float MouseSpeed { get; set; }
        private Vector3 target;
        private Vector3 targetRemember = new Vector3(); //zapamietuje obrot, aby updetowac mape
        private Vector2 mousePos;
        private Map map;

        public Player(SKraft game, Vector3 position, Map map)
        {
            this.Position = position;
            this.game = game;
            this.map = map;
            fppCamera = new FppCamera(game, new Vector3(position.X, position.Y + 1, position.Z), Vector3.Zero, Vector3.Up);

            mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Speed = 0.1f;
            MouseSpeed = 10;
            
            game.Components.Add(fppCamera);
        }

        public void LoadContent()
        {
            model = game.Content.Load<Model>(@"models\player");

            transforms = new Matrix[model.Bones.Count];           
        }

        public override void Update(GameTime gameTime)
        {
            bool mapUpdate = false;

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

            if (game.IsActive)
            {
                if (mousePos.X <= 0)
                {
                    mousePos.X = game.GraphicsDevice.Viewport.Width;
                    Mouse.SetPosition((int)mousePos.X, (int)mousePos.Y);
                }
                else if (mousePos.X >= game.GraphicsDevice.Viewport.Width)
                {
                    mousePos.X = 0;
                    Mouse.SetPosition((int)mousePos.X, (int)mousePos.Y);
                }

                if (mousePos.Y <= 0)
                {
                    mousePos.Y = 0;
                    Mouse.SetPosition((int)mousePos.X, (int)mousePos.Y);
                }
                else if (mousePos.Y >= game.GraphicsDevice.Viewport.Height)
                {
                    mousePos.Y = game.GraphicsDevice.Viewport.Height;
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
                mapUpdate = true;
            }

            if (state.IsKeyDown(Keys.S))
            {
                Vector3 v = new Vector3(0, 0, -Speed);
                v = Vector3.Transform(v, forwardMovement);
                fppCamera.Move(v.X, v.Z);
                Move(v.X, v.Z);
                mapUpdate = true;
            }

            if (state.IsKeyDown(Keys.A))
            {
                Vector3 v = new Vector3(-Speed, 0, 0);
                v = Vector3.Transform(v, forwardMovement);
                fppCamera.Move(v.X, v.Z);
                Move(v.X, v.Z);
                mapUpdate = true;
            }

            if (state.IsKeyDown(Keys.D))
            {
                Vector3 v = new Vector3(Speed, 0, 0);
                v = Vector3.Transform(v, forwardMovement);
                fppCamera.Move(v.X, v.Z);
                Move(v.X, v.Z);
                mapUpdate = true;
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Cube[] cubes = map.GetNearestCubes(Position);
            }

            if (target != targetRemember)
            {
                mapUpdate = true;
                targetRemember = target;
            }

            fppCamera.target = new Vector3(target.X, target.Y, 0);
            Rotate(target.X, target.Y);

            if (mapUpdate || map.Loading)
            {
                if (map.Loading)
                {
                    Debug.AddString("Loading");
                }

                map.Update(Position);
            }
        }

        public void Draw(GameTime gameTime)
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
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(Position);
                    effect.View = Camera.ActiveCamera.View;
                    effect.Projection = Camera.ActiveCamera.Projection;
                }

                mesh.Draw();
            }
        }

        public void Move(float x, float z)
        {
            Position = new Vector3(Position.X + x, 0, Position.Z - z);
        }

        public void Rotate(float x, float y)
        {
            model.Root.Transform = Matrix.CreateRotationY(-x);
        }

        /// <summary>
        /// Funkcja sprawdzająca, który klocek został kliknięty.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="objects"></param>
        /// <param name="graphics"></param>
        public void CheckClickedModel(int x, int y, List<Object3D> objects, GraphicsDeviceManager graphics)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {

                Vector3 nearsource = new Vector3(x, y, 0f);
                Vector3 farsource = new Vector3(x, y, 1f);

                Vector3 nearPoint = graphics.GraphicsDevice.Viewport.Unproject(nearsource, 
                    fppCamera.Projection, fppCamera.View,Matrix.CreateTranslation(0, 0, 0));
                Vector3 farPoint = graphics.GraphicsDevice.Viewport.Unproject(farsource, 
                    fppCamera.Projection, fppCamera.View, Matrix.CreateTranslation(0, 0, 0));

                Vector3 direction = farPoint - nearPoint;
                direction.Normalize();
                Ray pickRay = new Ray(nearPoint, direction);

                float selectedDistance = 2.5f;
                int selectedIndex = -1;
                for (int i = 0; i < objects.Count; i++)
                {
                    if (objects[i].Exists)
                    {
                        Vector3 pos = objects[i].Position;
                        BoundingBox bounding = new BoundingBox(new Vector3(pos.X - 0.5f, pos.Y - 0.5f, pos.Z - 0.5f),
                            new Vector3(pos.X + 0.5f, pos.Y + 0.5f, pos.Z + 0.5f));
                        Nullable<float> result = pickRay.Intersects(bounding);
                        if (result.HasValue)
                        {
                            if (result.Value < selectedDistance)
                            {
                                selectedIndex = i;
                                selectedDistance = result.Value;
                            }
                        }
                    }
                }

                if (selectedIndex > -1)
                {
                    objects[selectedIndex].Exists = false;
                }
            }
        }
    }
}
