﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SKraft.Cubes;
using Microsoft.Xna.Framework.Graphics;
using SKraft.Cameras;

namespace SKraft
{
    public class Map
    {
        List<Cube> cubes = new List<Cube>();
        Matrix[] instanceTransforms;
        Model instancedModel;
        Matrix[] instancedModelBones;
        DynamicVertexBuffer instanceVertexBuffer;
        ContentManager content;

        // To store instance transform matrices in a vertex buffer, we use this custom
        // vertex type which encodes 4x4 matrices as a set of four Vector4 values.
        static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );

        public Map(ContentManager content)
        {
            this.content = content;
        }

        public void Initialize()
        {
            for (int x = 0; x < 50; ++x)
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
            }
        }

        public void LoadContent()
        {
            foreach (Cube cube in cubes)
            {
                cube.LoadContent(content);
            }

            instancedModel = content.Load<Model>(@"models\cube");
            instancedModelBones = new Matrix[instancedModel.Bones.Count];
            instancedModel.CopyAbsoluteBoneTransformsTo(instancedModelBones);
        }

        public void Draw(GraphicsDevice graphicsDevice)
        {
            Array.Resize(ref instanceTransforms, cubes.Count);

            for (int i = 0; i < cubes.Count; i++)
            {
                instanceTransforms[i] = Matrix.CreateTranslation(cubes[i].Position);
            }
            /// Rysujemy wszystko za jednym zamachem
                
            /// Poszerzany bufory jeśli potrzeba.
            if ((instanceVertexBuffer == null) || (cubes.Count > instanceVertexBuffer.VertexCount))
            {
                if (instanceVertexBuffer != null)
                {
                    instanceVertexBuffer.Dispose();
                }

                instanceVertexBuffer = new DynamicVertexBuffer(graphicsDevice, instanceVertexDeclaration, cubes.Count, BufferUsage.WriteOnly);
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
                    effect.Parameters["Texture"].SetValue(content.Load<Texture2D>(@"textures\texture2low2"));

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                            meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount, cubes.Count);
                    }
                }
            }            
        }
    }
}