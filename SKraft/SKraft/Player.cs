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
        private FppCamera fppCamera;

        public float Speed { get; set; }
        public float MouseSpeed { get; set; }
        private Vector3 target;
        private Vector3 targetRemember = new Vector3(); //zapamietuje obrot, aby updetowac mape
        private Vector2 mousePos;
        private Map map;

        private Cube clickingCube; //cube, który jest aktualnie klikany
        private int clickingCount; //liczy dlugosc trzymanej kliknietej myszki na obiekcie
        private bool clickingCountPositive = true; //czy cliking count rosnie

        private bool rightPressed; //sprawdza czy prawa mysz wcisnieta

        /// <summary>
        /// Określa obiekt będący w dłoni gracza
        /// </summary>
        private Object3D inHand;

        public Player(SKraft game, Vector3 position, Map map)
        {
            this.Position = position;
            this.game = game;
            this.map = map;
            fppCamera = new FppCamera(game, new Vector3(position.X, position.Y + 1, position.Z), Vector3.Zero, Vector3.Up);
            Position = new Vector3(Position.X, 0, Position.Z);

            mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Speed = 0.1f;
            MouseSpeed = 10;
            
            game.Components.Add(fppCamera);
        }

        public void LoadContent()
        {
            model = game.Content.Load<Model>(@"models\player");          
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
                MouseClickLeft(ref mapUpdate);
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                clickingCount = 0;
                clickingCountPositive = true;
                clickingCube = null;
            }
            
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                if (!rightPressed)
                {
                    rightPressed = true;
                    MouseClickRight();
                    mapUpdate = true;
                }
            }
            else if (Mouse.GetState().RightButton == ButtonState.Released)
            {
                rightPressed = false;
            }

            if (target != targetRemember)
            {
                mapUpdate = true;
                targetRemember = target;
            }

            UpdateItemInHand(state);

            fppCamera.Target = new Vector3(target.X, target.Y, 0);
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

        private void UpdateItemInHand(KeyboardState state)
        {
            if (state.IsKeyDown(Keys.I))
            {
                inHand = new SampleCube(Vector3.Zero);
            }
            if (inHand != null)
            {
                Matrix matrix = Matrix.CreateRotationX(-target.Y)*Matrix.CreateRotationY(-target.X)
                             * Matrix.CreateTranslation(new Vector3(Position.X, Position.Y + 2, Position.Z));
                inHand.Position = Vector3.Transform(new Vector3(0.25f, -0.18f - clickingCount / 170f, -0.5f - clickingCount / 40f), matrix);
                inHand.RotationY = -target.X;
                inHand.RotationX = -target.Y - clickingCount / 70f;
                inHand.Scale = 0.2f;
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
        private Object3D CheckClickedModel(Object3D[] objects)
        {
            Vector3 nearsource = new Vector3(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2, 0f);
            Vector3 farsource = new Vector3(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2, 1f);

            Vector3 nearPoint = game.GraphicsDevice.Viewport.Unproject(nearsource, 
                fppCamera.Projection, fppCamera.View,Matrix.CreateTranslation(0, 0, 0));
            Vector3 farPoint = game.GraphicsDevice.Viewport.Unproject(farsource, 
                fppCamera.Projection, fppCamera.View, Matrix.CreateTranslation(0, 0, 0));

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            Ray pickRay = new Ray(nearPoint, direction);

            float selectedDistance = 2.5f;
            int selectedIndex = -1;
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].Exists)
                {
                    float? result = pickRay.Intersects(objects[i].BBox);
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
                return objects[selectedIndex];
            }

            return null;
        }

        private void MouseClickLeft(ref bool update)
        {
            if (clickingCountPositive)
            {
                clickingCount += 3;

                if (clickingCount > 20)
                {
                    clickingCount = 20;
                    clickingCountPositive = false;

                    Cube cube = (Cube)CheckClickedModel(map.GetNearestCubes(new Vector3(Position.X, Position.Y + 1, Position.Z)));
                    if (cube != null)
                    {
                        if (clickingCube == null || clickingCube != cube)
                        {
                            clickingCube = cube;
                        }

                        if (inHand != null)
                        {
                            inHand.Hit(clickingCube);
                            update = true;
                        }
                    }
                }
            }
            else
            {
                --clickingCount;

                if (clickingCount < 0)
                {
                    clickingCount = 0;
                    clickingCountPositive = true;
                }
            }
        }

        private void MouseClickRight()
        {
            Cube cube = (Cube)CheckClickedModel(map.GetNearestCubes(new Vector3(Position.X, Position.Y + 1, Position.Z)));
            if (cube != null)
            {
                BoundingBox[] boxes = cube.GetSideBBoxes();

                Vector3 nearsource = new Vector3(game.GraphicsDevice.Viewport.Width/2,
                                                 game.GraphicsDevice.Viewport.Height/2, 0f);
                Vector3 farsource = new Vector3(game.GraphicsDevice.Viewport.Width/2,
                                                game.GraphicsDevice.Viewport.Height/2, 1f);

                Vector3 nearPoint = game.GraphicsDevice.Viewport.Unproject(nearsource,
                                                                           fppCamera.Projection, fppCamera.View,
                                                                           Matrix.CreateTranslation(0, 0, 0));
                Vector3 farPoint = game.GraphicsDevice.Viewport.Unproject(farsource,
                                                                          fppCamera.Projection, fppCamera.View,
                                                                          Matrix.CreateTranslation(0, 0, 0));

                Vector3 direction = farPoint - nearPoint;
                direction.Normalize();
                Ray pickRay = new Ray(nearPoint, direction);

                float selectedDistance = 2.5f;
                int selectedIndex = -1;
                for (int i = 0; i < boxes.Length; i++)
                {
                    float? result = pickRay.Intersects(boxes[i]);
                    if (result.HasValue)
                    {
                        if (result.Value < selectedDistance)
                        {
                            selectedIndex = i;
                            selectedDistance = result.Value;
                        }
                    }
                }

                if (selectedIndex > -1) //kliknięto jakąs ścianę
                {
                    Vector3 newCubePos = cube.Position;

                    switch (selectedIndex)
                    {
                        case (int)Cube.Side.Up:
                            if (newCubePos.Y + 1 < MemoryMap.SectorSize.Y)
                            {
                                newCubePos.Y += 1;
                            }
                            break;
                        case (int)Cube.Side.Bottom:
                            newCubePos.Y -= 1;
                            break;
                        case (int)Cube.Side.Back:
                            newCubePos.Z -= 1;
                            break;
                        case (int)Cube.Side.Front:
                            newCubePos.Z += 1;
                            break;
                        case (int)Cube.Side.Left:
                            newCubePos.X -= 1;
                            break;
                        case (int)Cube.Side.Right:
                            newCubePos.X += 1;
                            break;
                    }

                    map.AddCube(new SampleCube(newCubePos));
                }
            }
        }

        public override void Draw()
        {
            if (inHand != null)
            {
                inHand.Draw();
            }
        }
    }
}
