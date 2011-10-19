using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace SKraft.Cameras
{
    public abstract class Camera : Microsoft.Xna.Framework.GameComponent
    {
        public static Camera ActiveCamera { get; set; }
        public Matrix Projection { get; protected set; }
        public Matrix View { get; protected set; }

        public Camera(Game game, Vector3 position, Vector3 target, Vector3 up)
            : base(game)
        {
            View = Matrix.CreateLookAt(position, target, up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                             (float)Game.Window.ClientBounds.Width /
                                                             (float)Game.Window.ClientBounds.Height, 0.5f, 20);
            
            if (ActiveCamera == null)
            {
                ActiveCamera = this;
            }
        }

        

        //Przeniesc docelowo do gracza
        public static void CheckClickedModel(int x, int y, List<Object3D> objects, GraphicsDeviceManager graphics)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {

                Vector3 nearsource = new Vector3(x, y, 0f);
                Vector3 farsource = new Vector3(x, y, 1f);

                Vector3 nearPoint = graphics.GraphicsDevice.Viewport.Unproject(nearsource, ActiveCamera.Projection,
                                                                               ActiveCamera.View,
                                                                               Matrix.CreateTranslation(0, 0, 0));
                Vector3 farPoint = graphics.GraphicsDevice.Viewport.Unproject(farsource, ActiveCamera.Projection,
                                                                              ActiveCamera.View,
                                                                              Matrix.CreateTranslation(0, 0, 0));

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
