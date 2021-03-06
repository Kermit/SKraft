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
using SKraft.MapGen;
using SKraft.Menu;

namespace SKraft
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SKraft : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Player player;
        public static Map Map;
        private MainMenu menu;
        
        FpsCounter fpsCounter = new FpsCounter();
        private Texture2D crosshair;
        public static GraphicsDevice Graphics;
        public static ContentManager SKraftContent;
        private Sun sun;



        public SKraft()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            Graphics = GraphicsDevice;
            SKraftContent = this.Content;
            
            menu = new MainMenu(this);
            sun = new Sun();
            Vector3 playerPos = new Vector3(15, 1, 1);
            Map = new Map(playerPos);
            player = new Player(this, playerPos, Map);
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
            Map.Initialize();

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

            menu.LoadContent();
            player.LoadContent();
            Map.LoadContent(Content);            
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
                menu.IsMenuVisible = true;
            }
            
            player.Update(gameTime);
            base.Update(gameTime);                        
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            int drawCalls = 0;

            Graphics = GraphicsDevice;

            Debug.Start("drawing");
            if (menu.IsMenuVisible)
            {
                menu.Draw(spriteBatch);
            }
            else
            {
                sun.Draw();
                Map.Draw(sun);
                player.Draw();
                Debug.Stop("drawing");

                spriteBatch.Begin();
                spriteBatch.Draw(crosshair,
                    new Vector2(Graphics.Viewport.Width / 2 - crosshair.Width / 2 + 5, Graphics.Viewport.Height / 2 - crosshair.Height / 2 + 5),
                    Color.White);
                spriteBatch.End();

                this.Window.Title = "FPS: " + fpsCounter.Update(gameTime);
                Debug.AddString(String.Format("FPS: {0}", fpsCounter.Update(gameTime)));
                Debug.AddString(String.Format("Draw calls: {0}", drawCalls));
                Debug.AddStringTimer("Drawing time", "drawing");
                Debug.AddString(String.Format("Player pos: {0}", player.Position));
                Debug.AddString(String.Format("Camera view: {0}", Camera.ActiveCamera.View.Translation));
                Debug.AddString(String.Format("Collison {0}", player.collison));

                base.Draw(gameTime);
            }

            /*
                * Potrzebne poniewa� mieszamy 3D z 2D
                */
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }
    }
}
