using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SKraft.Cubes;
using Microsoft.Xna.Framework.Graphics;
using SKraft.Cameras;

namespace SKraft.MapGen
{
    public class Map
    {
        private Cube[] cubes;
        Matrix[] instanceTransforms;
        Model instancedModel;
        Matrix[] instancedModelBones;
        DynamicVertexBuffer instanceVertexBuffer;
        ContentManager content;
        private MemoryMap memoryMap = new MemoryMap();

        // To store instance transform matrices in a vertex buffer, we use this custom
        // vertex type which encodes 4x4 matrices as a set of four Vector4 values.
        static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );

        public void Initialize()
        {
            /*for (int x = 0; x < 50; ++x)
            {
                for (int z = 0; z < 30; ++z)
                {
                    cubes.Add(new SampleCube(new Vector3(x, 0, z)));
                }
            }

            for (int z = 10; z < 30; ++z)
            {
                for (int y = 0; y < 5; ++y)
                {
                    cubes.Add(new SampleCube(new Vector3(10, y, z)));
                }
            }*/
        }

        public void LoadContent(ContentManager content)
        {
            this.content = content;

            instancedModel = content.Load<Model>(@"models\cube");
            instancedModelBones = new Matrix[instancedModel.Bones.Count];
            instancedModel.CopyAbsoluteBoneTransformsTo(instancedModelBones);
        }

        public void Update(Player player)
        {
            cubes = memoryMap.GetDrawingCubes(player.Position, 10);
        }

        public void Draw(GraphicsDevice graphicsDevice)
        {
            Array.Resize(ref instanceTransforms, cubes.Length);

            for (int i = 0; i < cubes.Length; i++)
            {
                instanceTransforms[i] = Matrix.CreateTranslation(cubes[i].Position);
            }
            // Rysujemy wszystko za jednym zamachem
                
            // Poszerzany bufory jeśli potrzeba.
            if ((instanceVertexBuffer == null) || (cubes.Length > instanceVertexBuffer.VertexCount))
            {
                if (instanceVertexBuffer != null)
                {
                    instanceVertexBuffer.Dispose();
                }

                instanceVertexBuffer = new DynamicVertexBuffer(graphicsDevice, instanceVertexDeclaration, cubes.Length, BufferUsage.WriteOnly);
            }

            instanceVertexBuffer.SetData(instanceTransforms, 0, instanceTransforms.Length, SetDataOptions.Discard);               

            foreach (ModelMesh mesh in instancedModel.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                    graphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                        new VertexBufferBinding(instanceVertexBuffer, 0, 1)
                    );

                    graphicsDevice.Indices = meshPart.IndexBuffer;

                    // Set up the instance rendering effect.
                    Effect effect = meshPart.Effect;

                    effect.CurrentTechnique = effect.Techniques["SKraft"];

                    effect.Parameters["World"].SetValue(instancedModelBones[mesh.ParentBone.Index]);
                    effect.Parameters["View"].SetValue(Camera.ActiveCamera.View);
                    effect.Parameters["Projection"].SetValue(Camera.ActiveCamera.Projection);
                    //effect.Parameters["Texture"].SetValue(content.Load<Texture2D>(@"textures\texture2low2"));

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                            meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount, cubes.Length);
                    }
                }
            }            
        }
    }
}
