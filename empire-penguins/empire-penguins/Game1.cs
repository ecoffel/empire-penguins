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

namespace empire_penguins
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Camera camera { get; set; }
        public World world { get; set; }
        public Penguin penguin { get; set; }
        public Tower mytower { get; set; }
        public HUD hud { get; set; }

        private Song background_music;

        private Texture2D winScreen;

        private bool winning = false;
        private bool stateSet = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            camera = new Camera(this, new Vector3(0, -10, 15), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            world = new World(this);

            hud = new HUD(this, new Vector2(10, 10));
            hud.DrawOrder = 3;

            mytower = new Tower(this);
            mytower.DrawOrder = 2;

            Igloo igloo = new Igloo(this);
            //igloo.DrawOrder = 2;

            Butters butters = new Butters(this);
            butters.DrawOrder = 3;

            Febreeze febreeze = new Febreeze(this);

            Components.Add(camera);
            Components.Add(world);
            Components.Add(hud);
            Components.Add(mytower);
            Components.Add(igloo);
            Components.Add(butters);
            Components.Add(febreeze);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        /// 
        Model myModel;
        float aspectRatio;
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            winScreen = this.Content.Load<Texture2D>("winscreen");
            background_music = this.Content.Load<Song>("ManuelMora-TetrismusicA");
            try
            {
                MediaPlayer.Play(background_music);
                MediaPlayer.IsRepeating = true;
                //background_music.Play();
            }
            catch { }
            
            // show the mouse cursor over the window
            this.IsMouseVisible = true;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Escape))
                this.Exit();

            if (hud.tower_completion >= 100 && !winning)
            {
                winning = true;
                //GameComponentCollection collection = this.Components;
                //foreach (GameComponent c in collection)
                Components.Clear();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // turn off backface culling
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rs;

            // turn on the depth buffer
            DepthStencilState depthStencilState = new DepthStencilState();
            depthStencilState.DepthBufferEnable = true;
            depthStencilState.DepthBufferFunction = CompareFunction.LessEqual;
            depthStencilState.DepthBufferWriteEnable = true;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            GraphicsDevice.Clear(Color.Black);

            Rectangle helpbackground = new Rectangle(0, 0, 800, 500);

            if (winning)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(winScreen, helpbackground, Color.White);
                spriteBatch.End();
            }
            
            base.Draw(gameTime);
        }
    }
}
