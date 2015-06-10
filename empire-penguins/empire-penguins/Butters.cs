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
    class Butters : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        Texture2D buttersTexture;
        SoundEffect meow;

        Vector2 currPos;
        int buttersSpeed = 225;
        Vector2 directionf;
        Vector2 directionb;

        Game g;
        HUD hud;
        Tower tower;

        Vector3 towerLocation;

        float scale = 0.15f;

        Vector2 buttersOrigin = new Vector2(-145, 80);
        bool attacking = true;

        public Butters(Game game)
            : base(game)
        {
            g = game;

        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            buttersTexture = g.Content.Load<Texture2D>("butters");
            meow = g.Content.Load<SoundEffect>("meow");
            currPos = buttersOrigin;
            getHUD();
            getTower();
            towerLocation = tower.towerloc;
            base.LoadContent();

        }

        public override void Update(GameTime gameTime)
        {
            towerLocation = tower.towerloc;
            Vector2 moveTo = new Vector2(towerLocation.X, towerLocation.Y);
            directionf = moveTo - buttersOrigin;
            directionf.Normalize();
            directionb = buttersOrigin - moveTo;
            directionb.Normalize();
            if ((hud.stinkiness >= 50))
            {
                //attacking = true;
                if (attacking)
                {
                    currPos += directionf * buttersSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                    currPos += directionf * (-1 * buttersSpeed) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (((Math.Abs(currPos.X - moveTo.X) <= 2f) && (Math.Abs(currPos.Y - moveTo.Y) <= 2f)))
                {
                    attacking = false;
                    //hud.stinkiness = 0;//butters stays at towerLoc
                }
                if ((Math.Abs(currPos.X - moveTo.X) > Math.Abs(buttersOrigin.X - moveTo.X)) && ((Math.Abs(currPos.Y - moveTo.Y) > Math.Abs(buttersOrigin.Y - moveTo.Y))))
                {
                    attacking = true;
                    meow.Play();
                    hud.stinkiness = 25;
                    hud.tower_completion = hud.tower_completion / 2;
                }
            }
               
            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(buttersTexture, currPos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.End();
            base.Draw(gameTime);

        }


        #region getGameCompenents
        private void getHUD()
        {
            foreach (GameComponent c in g.Components)
            {
                if (c is HUD)
                    hud = (HUD)c;
            }
        }

        private void getTower()
        {
            foreach (GameComponent c in g.Components)
            {
                if (c is Tower)
                    tower = (Tower)c;
            }
        }
        #endregion
    }
}
