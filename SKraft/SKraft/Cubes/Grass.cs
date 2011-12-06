//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;
//using SKraft.MapGen;

//namespace SKraft.Cubes
//{
//    class Grass : Cube
//    {
//        public static Texture2D SampleTex { get; private set; }

//        public Grass(Vector3 position)
//            : base(position)
//        {
            

//            model = cubeModel;
//            texture = SampleTex;
//        }

//        public override void LoadContent(ContentManager content)
//        {
//            if (cubeModel == null)
//            {
//                cubeModel = content.Load<Model>(@"models\cubeNoInst");
//            }

//            if (SampleTex == null)
//            {
//                SampleTex = content.Load<Texture2D>(@"textures\texturegrass");
//            }
            
//            model = cubeModel;
//            texture = SampleTex;
//        }
//    }
//}
