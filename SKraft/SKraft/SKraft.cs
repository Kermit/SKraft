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
        private Texture2D crosshair;

        public SKraft()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1440;
            graphics.PreferredBackBufferHeight = 900;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            Components.Add(new FppCamera(this, new Vector3(0, 2, 10), Vector3.Zero, Vector3.Up));
            Components.Add(new Debug(this));
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
            for (int x = 0; x < 30; ++x)
            {
                for (int z = 0; z < 30; ++z)
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

            crosshair = Content.Load<Texture2D>("crosshair");

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

            Camera.CheckClickedModel(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2, objects3D, graphics);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            int drawCalls = 0;
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Debug.Start("drawing");
            for (int i = 0; i < objects3D.Count; ++i)
            {
                objects3D[i].Draw();
            }
            Debug.Stop("drawing");

            spriteBatch.Begin();
            spriteBatch.Draw(crosshair,
                new Vector2(GraphicsDevice.Viewport.Width / 2 - crosshair.Width / 2 + 5, GraphicsDevice.Viewport.Height / 2 - crosshair.Height / 2 + 5),
                Color.White);
            spriteBatch.End();
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            this.Window.Title = "FPS: " + fpsCounter.Update(gameTime);
            Debug.AddString(String.Format("FPS: {0}", fpsCounter.Update(gameTime)));
            Debug.AddString(String.Format("Draw calls: {0}", drawCalls));
            Debug.AddStringTimer("Drawing time", "drawing");

            base.Draw(gameTime);
        }
    }
}
