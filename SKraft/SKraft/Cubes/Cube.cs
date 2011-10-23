using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SKraft.Cubes
{
    public abstract class Cube : Object3D
    {
        public struct Bonus
        {
            public Type type;
            public short bonus;
        }

        public abstract void LoadContent(ContentManager content);

        protected static Model cubeModel;
        private List<Bonus> bonusObjects = new List<Bonus>();
        public List<Bonus> BonusObjects 
        {
            get { return bonusObjects; }
        }

        public BoundingBox BBox { get; private set; }

        public Cube(Vector3 position)
        {
            this.Position = position;

            BBox = new BoundingBox(new Vector3(Position.X - 0.5f, Position.Y - 0.5f, Position.Z - 0.5f),
                            new Vector3(Position.X + 0.5f, Position.Y + 0.5f, Position.Z + 0.5f));

        }
    }
}
