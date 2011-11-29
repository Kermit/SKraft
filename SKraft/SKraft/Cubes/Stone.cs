using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SKraft.MapGen;

namespace SKraft.Cubes
{
    class Stone : Cube
    {
        public static Texture2D SampleTex { get; private set; }

        public Stone(Vector3 position)
            : base(position)
        {
            this.life = 300;
            this.name = "Stone";
            this.Power = 20;
            this.Index = 2;

            Bonus bonus = new Bonus { type = typeof(Cube), bonus = 0 };
            this.BonusObjects.Add(bonus);

            model = cubeModel;
            texture = SampleTex;
        }

        public override void LoadContent(ContentManager content)
        {
            if (cubeModel == null)
            {
                cubeModel = content.Load<Model>(@"models\cubeNoInst");
            }

            if (SampleTex == null)
            {
                SampleTex = content.Load<Texture2D>(@"textures\texturestone");
            }

            model = cubeModel;
            texture = SampleTex;
        }
    }
}
