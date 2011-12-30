using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SKraft.Tools
{
    class Pickaxe : Object3D
    {
        private static Model pickaxeModel;

        public Pickaxe(Vector3 position)
        {
            if (model == null)
            {
                pickaxeModel = SKraft.SKraftContent.Load<Model>(@"Models\pickaxe");
            }

            model = pickaxeModel;
            this.Position = position;
            this.name = "Kilof";
            this.life = 3000;
            this.Power = 300;
        }
    }
}
