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
    class Febreeze : Building
    {
        private int antistink_regeneration_time = 100; //seconds
        private int fishPerLevel = 70;
        private bool upgradeKeyPressed = false;
        private double updateTime = 0;
        private HUD hud;
        Game game;

        public Febreeze(Game g)
            : base(g)
        {
            game = g;
        }

        protected override void LoadContent()
        {
            hud = getHUD();
            base.LoadContent();
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        #region Update
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            updateTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            hud.time_till_febreeze = ((antistink_regeneration_time * 1000) - (int)Math.Round(updateTime)) / 1000;
            if (updateTime >= antistink_regeneration_time * 1000)
            {
                if (hud.stinkiness > 0)
                {
                    hud.stinkiness -= 1;
                    updateTime = 0;
                }
            }

            Upgrade();
            base.Update(gameTime);
        }
        #endregion

        #region Upgrade
        private void Upgrade()
        {
            KeyboardState k = Keyboard.GetState();

            if (k.IsKeyDown(Keys.F) && !upgradeKeyPressed)
            {
                upgradeKeyPressed = true;
                if (hud.fish >= fishPerLevel)
                {
                    antistink_regeneration_time = (int) ((double) antistink_regeneration_time*0.8);
                    if (antistink_regeneration_time < 2)
                        antistink_regeneration_time = 2;
                    else
                        hud.fish -= fishPerLevel;
                }
                else
                {
                    hud.setMessage("Not enough fish");
                }
            }
            else if (k.IsKeyUp(Keys.F))
                upgradeKeyPressed = false;
        }
        #endregion

    }
}

