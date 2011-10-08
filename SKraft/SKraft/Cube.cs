using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SKraft
{
    class Cube
    {
        private Model model;
        private Texture2D texture;

        public void LoadContent(ContentManager content)
        {
            model = content.Load<Model>(@"models\cube2");
            texture = content.Load<Texture2D>(@"textures\texture2");
        }

        Vector3 modelPosition = Vector3.Zero;
        float modelRotation = 0.5f;

        public void Draw(Camera camera)
        {
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in model.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.VertexColorEnabled = true;
                    effect.Texture = texture;
                    //effect.TextureEnabled = true;
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] *
                        Matrix.CreateRotationY(modelRotation)
                        * Matrix.CreateTranslation(modelPosition);
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
    }
}
