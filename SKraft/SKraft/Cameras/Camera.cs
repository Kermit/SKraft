using System;
using System.Collections.Generic;
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
        public abstract Matrix View { get; protected set; }

        public Camera(Game game, Vector3 position, Vector3 target, Vector3 up)
            : base(game)
        {
            View = Matrix.CreateLookAt(position, target, up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                             (float)Game.Window.ClientBounds.Width /
                                                             (float)Game.Window.ClientBounds.Height, 0.5f, 1000);

            if (ActiveCamera == null)
            {
                ActiveCamera = this;
            }
        }
    }
}
