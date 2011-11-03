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
    class SampleCube : Cube
    {
        private static Texture2D sampleTex;
        public SampleCube(Vector3 position)
            : base(position)
        {
            this.life = 100;
            this.name = "Sample Cube";
            this.Power = 20;

            Bonus bonus = new Bonus {type = typeof(Cube), bonus = 0};
            this.BonusObjects.Add(bonus);

            model = cubeModel;
            texture = sampleTex;
        }

        public override void LoadContent(ContentManager content)
        {
            if (cubeModel == null)
            {
                cubeModel = content.Load<Model>(@"models\cubeNoInst");
            }

            if (sampleTex == null)
            {
                sampleTex = content.Load<Texture2D>(@"textures\texture2low2");
            }
            
            model = cubeModel;
            texture = sampleTex;
        }
    }
}
