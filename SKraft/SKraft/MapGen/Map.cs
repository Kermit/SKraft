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
        private Cube[] allCubes;
        Matrix[] instanceTransforms;
        Model instancedModel;
        Matrix[] instancedModelBones;
        DynamicVertexBuffer instanceVertexBuffer;
        ContentManager content;
        private MemoryMap memoryMap;
        Dictionary<int, List<Cube>> cubesList;

        public bool Loading
        {
            get { return memoryMap.Loading; }
        }

        // To store instance transform matrices in a vertex buffer, we use this custom
        // vertex type which encodes 4x4 matrices as a set of four Vector4 values.
        static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );

        public Map(Vector3 playerPos)
        {
            memoryMap = new MemoryMap(playerPos, false);
            allCubes = new Cube[0];
            cubesList = new Dictionary<int, List<Cube>>();
            
        }

        public void Initialize() { }

        public void LoadContent(ContentManager content)
        {
            this.content = content;

            instancedModel = content.Load<Model>(@"models\cube");
            instancedModelBones = new Matrix[instancedModel.Bones.Count];
            instancedModel.CopyAbsoluteBoneTransformsTo(instancedModelBones);

            memoryMap.LoadContent(content);
        }

        public void Update(Vector3 position)
        {
            allCubes = memoryMap.GetDrawingCubes(position, 30);
            cubesList = new Dictionary<int, List<Cube>>();

            foreach (Cube cb in allCubes)
            {
                if (!cubesList.ContainsKey((byte)cb.TypeCube))
                {
                    cubesList.Add((byte)cb.TypeCube, new List<Cube>());
                }

                cubesList[(byte)cb.TypeCube].Add(cb);
            }
        }

        /// <summary>
        /// Zwraca najbliższe cuby z danej pozycji
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Cube[] GetNearestCubes(Vector3 position)
        {
            return memoryMap.GetNearestCubes(position);
        }

        /// <summary>
        /// Usuwa cuba z mapy
        /// </summary>
        /// <param name="cube"></param>
        public void DeleteCube(Cube cube)
        {
            memoryMap.DeleteCube(cube);
        }

        /// <summary>
        /// Dodaje cuba do mapy
        /// </summary>
        /// <param name="cube"></param>
        public void AddCube(Cube cube)
        {
            memoryMap.AddCube(cube);
        }

        public void Draw(Sun sun)
        {
            foreach (List<Cube> cubes in cubesList.Values)
            {
                if (cubes.Count > 0)
                {
                    Debug.AddString("Drawing cubes " + cubes[0].TypeCube + ": "  + cubes.Count);
                    Array.Resize(ref instanceTransforms, cubes.Count);

                    for (int i = 0; i < cubes.Count; i++)
                    {
                        instanceTransforms[i] = Matrix.CreateTranslation(cubes[i].Position);
                    }
                    // Rysujemy wszystko za jednym zamachem

                    // Poszerzany bufory jeśli potrzeba.
                    if ((instanceVertexBuffer == null) || (cubes.Count > instanceVertexBuffer.VertexCount))
                    {
                        if (instanceVertexBuffer != null)
                        {
                            instanceVertexBuffer.Dispose();
                        }

                        instanceVertexBuffer = new DynamicVertexBuffer(SKraft.Graphics, instanceVertexDeclaration,
                                                                       cubes.Count, BufferUsage.WriteOnly);
                    }

                    instanceVertexBuffer.SetData(instanceTransforms, 0, instanceTransforms.Length, SetDataOptions.Discard);

                    foreach (ModelMesh mesh in instancedModel.Meshes)
                    {
                        foreach (ModelMeshPart meshPart in mesh.MeshParts)
                        {
                            // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                            SKraft.Graphics.SetVertexBuffers(
                                new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                                new VertexBufferBinding(instanceVertexBuffer, 0, 1)
                                );

                            SKraft.Graphics.Indices = meshPart.IndexBuffer;

                            // Set up the instance rendering effect.
                            Effect effect = meshPart.Effect;

                            effect.CurrentTechnique = effect.Techniques["SKraft"];

                            effect.Parameters["World"].SetValue(instancedModelBones[mesh.ParentBone.Index]);
                            effect.Parameters["View"].SetValue(Camera.ActiveCamera.View);
                            effect.Parameters["Projection"].SetValue(Camera.ActiveCamera.Projection);
                            effect.Parameters["Texture"].SetValue(cubes[0].Texture);
                            effect.Parameters["LightPower"].SetValue(sun.Light);

                            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                            {
                                pass.Apply();

                                SKraft.Graphics.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                                       meshPart.NumVertices, meshPart.StartIndex,
                                                                       meshPart.PrimitiveCount, cubes.Count);                                
                            }
                        }
                    }
                }
            }
        }
    }
}
