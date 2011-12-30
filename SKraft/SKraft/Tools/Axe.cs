using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SKraft.Tools
{
    class Axe : Object3D
    {
        private static Model axeModel;

        public Axe(Vector3 position)
        {
            if (model == null)
            {
                axeModel = SKraft.SKraftContent.Load<Model>(@"Models\axe");
            }

            model = axeModel;
            this.Position = position;
            this.name = "Siekiera";
            this.life = 3000;
            this.Power = 100;
        }
    }
}
