using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace empire_penguins
{
    public class HUD : DrawableGameComponent
    {
        private Vector2 position;

        public int stinkiness {get; set;} 
        public int fish { get; set; }
        public int penguins { get; set; }
        public int tower_completion { get; set; }
        public int time_till_febreeze { get; set; }
        public string message { get; set; }
        private bool ismessage = false;
        private bool ishelpmessage = false;

        private SpriteBatch spriteBatch;

        private Texture2D helpScreen;
        private Texture2D dummyTexture;
        private SpriteFont font;
        private Game g;

        private float fps = 0;
        private double t = 0;
        private int updatetime = 30;
        public double tstart = 0;

        public HUD(Game game, Vector2 pos) : base(game) 
        {
            g = game;
            position = pos;
            stinkiness = 0;
            fish = 0;
            penguins = 0;
            tower_completion = 0;
        }

        #region Load Content
        protected override void LoadContent()
        {
            message = "";
            spriteBatch = new SpriteBatch(GraphicsDevice);
            dummyTexture = new Texture2D(GraphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.White });
            helpScreen = g.Content.Load<Texture2D>("helpscreen");
            font = g.Content.Load<SpriteFont>("Arial");
        }
        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {

            StinkyFun(gameTime); //update stinkiness

            if (t >= (tstart + 2000))
            { //after 2 seconds, erase message
                ismessage = false;
            }

            KeyboardState k = Keyboard.GetState();
            if (k.IsKeyDown(Keys.H))
            {
                ishelpmessage = true;
            }
            else
            {
                ishelpmessage = false;
            }
            base.Update(gameTime);
        }
        #endregion

        #region Draw
        public override void Draw(GameTime gameTime)
        {
            fps = 1000.0f / (float)gameTime.ElapsedGameTime.Milliseconds;

            int xpos = (int)Math.Floor(position.X);
            int ypos = (int)Math.Floor(position.Y);
            Rectangle background = new Rectangle(xpos, ypos, 200, 100);
            Rectangle msgbackground = new Rectangle(xpos, 445, 600, 30);

            int helpx = 660;
            int helpy = 445;
            Rectangle helpbox = new Rectangle(helpx, helpy, 130, 30);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            Rectangle helpbackground = new Rectangle(0, 0, 800, 500);
            if (ishelpmessage)
            {
                //int helpboxy = 10;
                //int lineheight = 20;
                spriteBatch.Draw(helpScreen, helpbackground, Color.White);
            }
            else
            {
            spriteBatch.Draw(dummyTexture, background, Color.Black);
            spriteBatch.DrawString(font, "Stinkiness:  "+stinkiness+ "%", new Vector2(xpos+10, ypos+=10), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, "Fish:  " + fish, new Vector2(xpos + 10, ypos+=20), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, "Penguins:  " + penguins, new Vector2(xpos + 10, ypos += 20), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, "Time Until De-Stink:  " + time_till_febreeze, new Vector2(xpos + 10, ypos += 20), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);

            spriteBatch.Draw(dummyTexture, helpbox, Color.Salmon);
            spriteBatch.DrawString(font, "Press H for Help", new Vector2(helpx + 10, helpy + 5), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            }

            if (ismessage) {
                spriteBatch.Draw(dummyTexture, msgbackground, Color.DarkSlateGray);
                spriteBatch.DrawString(font, message, new Vector2(xpos + 10, 450), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            }

            spriteBatch.DrawString(font, string.Format("Fps = {0}", Math.Round(fps)), new Vector2(10, 450),
                                   Color.Black);
        
            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion

        #region Set Message
        public void setMessage(String m) {
            message = m;
            ismessage = true;
            tstart = t;
        }
        #endregion

        #region Stinky Fun
        void StinkyFun(GameTime gameTime)
        {
            t += gameTime.ElapsedGameTime.TotalMilliseconds;
            
            float tower_percent = 1 - (tower_completion / 100f);
            updatetime = (int)(8 * tower_percent + 2);
            if (t >= updatetime * 1000)
            {
                stinkiness += 1;
                t -= updatetime * 1000;
            }
        }
        #endregion

    }
}
