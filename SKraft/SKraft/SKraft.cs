using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SKraft.Cameras;
using SKraft.Cubes;

namespace SKraft
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SKraft : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //List<Object3D> objects3D = new List<Object3D>();
        FpsCounter fpsCounter = new FpsCounter();

        List<Object3D> instances = new List<Object3D>();
        Matrix[] instanceTransforms;
        Model instancedModel;
        Matrix[] instancedModelBones;
        DynamicVertexBuffer instanceVertexBuffer;

        public enum InstancingTechnique
        {
            HardwareInstancing,
            NoInstancing,
            NoInstancingOrStateBatching
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

        public SKraft()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1440;
            graphics.PreferredBackBufferHeight = 900;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            Components.Add(new FppCamera(this, new Vector3(0, 2, 0), Vector3.Zero, Vector3.Up));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            for (int x = 0; x < 200; ++x)
            {
                for (int z = 0; z < 200; ++z)
                {
                    instances.Add(new SampleCube(new Vector3(x, 0, z)));
                }
            }

            for (int z = 10; z < 30; ++z)
            {
                for (int y = 0; y < 5; ++y)
                {
                    instances.Add(new SampleCube(new Vector3(10, y, z)));
                }
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (Cube cube in instances)
            {
                cube.LoadContent(Content);
            }

            instancedModel = Content.Load<Model>(@"models\cube");
            instancedModelBones = new Matrix[instancedModel.Bones.Count];
            instancedModel.CopyAbsoluteBoneTransformsTo(instancedModelBones);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = graphics.GraphicsDevice;
            GraphicsDevice.Clear(Color.CornflowerBlue);

            /*
            for (int i = 0; i < objects3D.Count; ++i)
            {
                objects3D[i].Draw(i == objects3D.Count - 1);
            }
             */

            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;

            // Gather instance transform matrices into a single array.
            Array.Resize(ref instanceTransforms, instances.Count);

            for (int i = 0; i < instances.Count; i++)
            {
                instanceTransforms[i] = instances[i].getPos();
            }

            // Draw all the instances, using the currently selected rendering technique.
            DrawModelHardwareInstancing(instancedModel, instancedModelBones, instanceTransforms, Camera.ActiveCamera.View, Camera.ActiveCamera.Projection);


            this.Window.Title = "FPS: " + fpsCounter.Update(gameTime);

            base.Draw(gameTime);
        }

        void DrawModelHardwareInstancing(Model model, Matrix[] modelBones,
                                         Matrix[] instances, Matrix view, Matrix projection)
        {
            if (instances.Length == 0)
                return;

            // If we have more instances than room in our vertex buffer, grow it to the neccessary size.
            if ((instanceVertexBuffer == null) ||
                (instances.Length > instanceVertexBuffer.VertexCount))
            {
                if (instanceVertexBuffer != null)
                    instanceVertexBuffer.Dispose();

                instanceVertexBuffer = new DynamicVertexBuffer(GraphicsDevice, instanceVertexDeclaration,
                                                               instances.Length, BufferUsage.WriteOnly);
            }

            // Transfer the latest instance transform matrices into the instanceVertexBuffer.
            instanceVertexBuffer.SetData(instances, 0, instances.Length, SetDataOptions.Discard);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                    GraphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                        new VertexBufferBinding(instanceVertexBuffer, 0, 1)
                    );

                    GraphicsDevice.Indices = meshPart.IndexBuffer;

                    // Set up the instance rendering effect.
                    Effect effect = meshPart.Effect;

                    effect.CurrentTechnique = effect.Techniques["HardwareInstancing"];

                    effect.Parameters["World"].SetValue(modelBones[mesh.ParentBone.Index] * instances[mesh.ParentBone.Index]);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);

                    // Draw all the instance copies in a single call.
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               meshPart.NumVertices, meshPart.StartIndex,
                                                               meshPart.PrimitiveCount, instances.Length);
                    }
                }
            }
        }
    }
}
