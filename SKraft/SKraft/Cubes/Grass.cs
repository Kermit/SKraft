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
    class Grass : Cube
    {
        public static Texture2D SampleTex { get; private set; }

        public Grass(Vector3 position)
            : base(position)
        {
            this.life = 100;
            this.name = "Grass";
            this.Power = 20;
            this.Index = 1;

            Bonus bonus = new Bonus {type = typeof(Cube), bonus = 0};
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
                SampleTex = content.Load<Texture2D>(@"textures\texturegrass");
            }
            
            model = cubeModel;
            texture = SampleTex;
        }
    }
}
