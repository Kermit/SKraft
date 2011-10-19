using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SKraft.Cameras;
using SKraft.Cubes;

namespace SKraft
{
    public abstract class Object3D
    {
        protected short life;
        protected string name;
        protected Texture2D icon;
        public Model model; //zabezpieczyc
        public Vector3 Position { get; protected set; }
        protected Texture2D texture;

        /// <summary>
        /// Określa moc danego obiektu podczas uderzania
        /// </summary>
        public short Power { get; protected set; }

        /// <summary>
        /// Określa, czy dany obiekt jest w plecaku gracza
        /// </summary>
        public bool IsInInventory { get; protected set; }

        /// <summary>
        /// Określa, czy dany obiekt jest możliwy do zebrania przez gracza
        /// </summary>
        public bool Loot { get; protected set; }

        public bool Exists { get; set; }

        protected Object3D()
        {
            Exists = true;
        }

        public void Hit(Cube cube)
        {
            int hitPower = 0;
            short bonus = 0;

            foreach (Cube.Bonus bonusObject in cube.BonusObjects)
            {
                if (bonusObject.type == this.GetType())
                {
                    bonus = bonusObject.bonus;
                    break;
                }
            }

            hitPower = Power + bonus;

            if (!(this is Cube))
            {
                --life;
            }

            if (life <= 0)
            {
                Exists = false;
            }
        }

        public virtual void Draw()
        {
            if (this.Exists && !this.IsInInventory)
            {
                BoundingFrustum viewFrustum = new BoundingFrustum(Camera.ActiveCamera.View * Camera.ActiveCamera.Projection);
                BoundingSphere sourceSphere = new BoundingSphere(Position, model.Meshes[0].BoundingSphere.Radius);

                if (viewFrustum.Intersects(sourceSphere))
                {
                    Matrix[] transforms = new Matrix[model.Bones.Count];
                    model.CopyAbsoluteBoneTransformsTo(transforms);
                    
                    foreach (ModelMesh mesh in model.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.FogEnabled = true;
                            effect.FogColor = Color.CornflowerBlue.ToVector3();
                            effect.FogStart = 17f;
                            effect.FogEnd = 21f;

                            if (texture != null)
                            {
                                effect.Texture = texture;
                            }

                            //effect.EnableDefaultLighting();
                            effect.World = transforms[mesh.ParentBone.Index]*
                                           //Matrix.CreateRotationY(modelRotation)
                                           Matrix.CreateTranslation(Position);
                            effect.View = Camera.ActiveCamera.View;
                            effect.Projection = Camera.ActiveCamera.Projection;
                        }

                        mesh.Draw();
                    }
                }
            }
        }

        public virtual void Update(GameTime gameTime) { }

        public override string ToString()
        {
            return name;
        }
    }
}
