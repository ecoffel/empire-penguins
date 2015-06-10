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
    public class Building: DrawableGameComponent
    {
        private HUD hud;
        Game game;
        Camera camera;

        public Building(Game g)
            : base(g)
        {
            game = g;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        #region get game components
        public HUD getHUD()
        {
            foreach (GameComponent x in Game.Components)
            {

                if (x is HUD)
                    hud = (HUD)x;
            }
            return hud;
        }

        public World getWorld()
        {
            World world = new World(game);
            foreach (GameComponent x in Game.Components)
            {

                if (x is World)
                    world = (World)x;
            }
            return world;
        }

        public Camera getCamera()
        {
            // find the camera in the components list
            Camera camera = new Camera(game, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f));
            foreach (GameComponent c in game.Components)
            {
                if (c is Camera)
                    camera = (Camera)c;
            }
            return camera;
        }
        #endregion
    }
}
