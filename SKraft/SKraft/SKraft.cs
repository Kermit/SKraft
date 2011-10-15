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
        List<Object3D> objects3D = new List<Object3D>();
        FpsCounter fpsCounter = new FpsCounter();

        public SKraft()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1440;
            graphics.PreferredBackBufferHeight = 900;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            Components.Add(new FppCamera(this, new Vector3(0, 2, 10), Vector3.Zero, Vector3.Up));
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
            for (int x = 0; x < 50; ++x)
            {
                for (int z = 0; z < 50; ++z)
                {
                    objects3D.Add(new SampleCube(new Vector3(x, 0, z)));
                }
            }

            for (int z = 10; z < 30; ++z)
            {
                for (int y = 0; y < 5; ++y)
                {
                    objects3D.Add(new SampleCube(new Vector3(10, y, z)));
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

            foreach (Cube cube in objects3D)
            {
                cube.LoadContent(Content);
            }
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            for (int i = 0; i < objects3D.Count; ++i)
            {
                objects3D[i].Draw(i == objects3D.Count - 1);
            }

            this.Window.Title = "FPS: " + fpsCounter.Update(gameTime);

            base.Draw(gameTime);
        }
    }
}
