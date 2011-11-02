using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SKraft.Cubes
{
    class SampleCube : Cube
    {
        private static Texture2D sampleTex;
        public SampleCube(Vector3 position)
            : base(position)
        {
            this.life = 1000;
            this.name = "Sample Cube";
            this.Power = 1;

            Bonus bonus = new Bonus {type = typeof(Cube), bonus = 1};
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
