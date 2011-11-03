using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using SKraft.MapGen;

namespace SKraft.Cubes
{
    public abstract class Cube : Object3D
    {
        public struct Bonus
        {
            public Type type;
            public short bonus;
        }

        public int Life 
        { 
            get
            {
                return life;
            } 
            set 
            { 
                life = value;
                if (life < 0)
                {
                    life = 0;
                    Exists = false;
                    SKraft.Map.DeleteCube(this);
                }
            }
        }

        public enum Side { Bottom = 0, Back = 1, Left = 2, Front = 3, Right = 4, Up = 5 }

        public abstract void LoadContent(ContentManager content);

        protected static Model cubeModel;
        private List<Bonus> bonusObjects = new List<Bonus>();
        public List<Bonus> BonusObjects 
        {
            get { return bonusObjects; }
        }

        public Cube(Vector3 position)
        {
            this.Position = position;

            BBox = new BoundingBox(new Vector3(Position.X - 0.5f, Position.Y - 0.5f, Position.Z - 0.5f),
                            new Vector3(Position.X + 0.5f, Position.Y + 0.5f, Position.Z + 0.5f));
        }

        /// <summary>
        /// Zwraca BoundigBoksy każdej ze ścian
        /// </summary>
        /// <returns></returns>
        public BoundingBox[] GetSideBBoxes()
        {
            BoundingBox[] boxes = new BoundingBox[6]; //ściany

            boxes[(int)Side.Bottom] = new BoundingBox(BBox.Min, new Vector3(BBox.Max.X, BBox.Min.Y, BBox.Max.Z)); //dolna
            boxes[(int)Side.Back] = new BoundingBox(BBox.Min, new Vector3(BBox.Max.X, BBox.Max.Y, BBox.Min.Z)); //tylna
            boxes[(int)Side.Left] = new BoundingBox(BBox.Min, new Vector3(BBox.Min.X, BBox.Max.Y, BBox.Max.Z)); //lewa
            boxes[(int)Side.Front] = new BoundingBox(new Vector3(BBox.Min.X, BBox.Min.Y, BBox.Max.Z), BBox.Max); //przednia
            boxes[(int)Side.Right] = new BoundingBox(new Vector3(BBox.Max.X, BBox.Min.Y, BBox.Min.Z), BBox.Max); //prawa
            boxes[(int)Side.Up] = new BoundingBox(new Vector3(BBox.Min.X, BBox.Max.Y, BBox.Min.Z), BBox.Max); //górna

            return boxes;
        }

        public override bool Equals(object cube)
        {
            if (cube == null)
            {
                return false;
            }

            if (cube is Cube)
            {
                if (this == cube)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool operator==(Cube cube1, Cube cube2)
        {
            if ((object)cube1 == null)
            {
                if ((object)cube2 == null)
                {
                    return true;
                }

                return false;
            }

            if ((object)cube2 == null)
            {
                return false;
            }

            if (cube1.Position == cube2.Position && cube1.GetType() == cube2.GetType())
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(Cube cube1, Cube cube2)
        {
            return !(cube1 == cube2);
        }
    }
}
