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
    public class Tower : DrawableGameComponent
    {
        private int percent_complete = 0;
        private Texture2D display_background;
        private Texture2D dummyTexture;
        private Game game;
        private World world;
        private Camera camera;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private HUD hud;
        private int fishPerPercent = 50;
        private bool upgradeKeyPressed = false;
        
        private int levels = 0;
        private int sideLen = 4;
        private List<VertexPositionColor> towerCubeVertices;
        private int wallLength = 1;
        private Color sidecolor = Color.AliceBlue;
        private Color sidecolor2 = Color.AntiqueWhite;

        private VertexBuffer vb;

        public Vector3 towerloc { get; set; }
        public Tower(Game g) :base(g) 
        {
            game = g;
        }

        #region Load Content
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            display_background = game.Content.Load<Texture2D>("displaybackground");
            dummyTexture = new Texture2D(GraphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.White });
            font = game.Content.Load<SpriteFont>("Arial");
            getHUD();
            getWorld();
            getCamera();
            towerloc = new Vector3(0, 20, 0);

            #region Tower Vertices
            towerCubeVertices = new List<VertexPositionColor>();

            // front face
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(0, 0, 0), sidecolor));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(0, 0, wallLength), sidecolor));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(wallLength, 0, 0), sidecolor));

            towerCubeVertices.Add(new VertexPositionColor(new Vector3(0, 0, wallLength), sidecolor));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(wallLength, 0, 0), sidecolor));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(wallLength, 0, wallLength), sidecolor));

            //right side
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(wallLength, 0, 0), sidecolor));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(wallLength, 0, wallLength), sidecolor));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(wallLength, wallLength, 0), sidecolor2));

            towerCubeVertices.Add(new VertexPositionColor(new Vector3(wallLength, 0, wallLength), sidecolor));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(wallLength, wallLength, 0), sidecolor2));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(wallLength, wallLength, wallLength), sidecolor2));

            // back side
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(wallLength, wallLength, 0), sidecolor2));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(wallLength, wallLength, wallLength), sidecolor2));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(0, wallLength, 0), sidecolor));

            towerCubeVertices.Add(new VertexPositionColor(new Vector3(wallLength, wallLength, wallLength), sidecolor2));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(0, wallLength, 0), sidecolor));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(0, wallLength, wallLength), sidecolor));

            // left side
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(0, wallLength, 0), sidecolor));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(0, wallLength, wallLength), sidecolor));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(0, 0, 0), sidecolor2));

            towerCubeVertices.Add(new VertexPositionColor(new Vector3(0, wallLength, wallLength), sidecolor));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(0, 0, 0), sidecolor2));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(0, 0, wallLength), sidecolor2));

            // top
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(0, 0, wallLength), sidecolor2));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(wallLength, 0, wallLength), sidecolor2));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(wallLength, wallLength, wallLength), sidecolor2));

            towerCubeVertices.Add(new VertexPositionColor(new Vector3(0, wallLength, wallLength), sidecolor2));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(wallLength, wallLength, wallLength), sidecolor2));
            towerCubeVertices.Add(new VertexPositionColor(new Vector3(0, 0, wallLength), sidecolor2));
            #endregion

            vb = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, towerCubeVertices.Count, BufferUsage.None);
            vb.SetData(towerCubeVertices.ToArray());

            base.LoadContent();
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            KeyboardState k = Keyboard.GetState();

            if (k.IsKeyDown(Keys.Q) && !upgradeKeyPressed)
            {
                upgradeKeyPressed = true;
                if (hud.fish >= fishPerPercent) //& we have enough fish
                {
                    hud.tower_completion += 1;
                    levels++;
                    hud.fish -= fishPerPercent;
                }
                else
                {
                    hud.setMessage("Not enough fish");
                }
            }
            else if (k.IsKeyUp(Keys.Q))
                upgradeKeyPressed = false;

            percent_complete = hud.tower_completion;
            base.Update(gameTime);
        }
        #endregion

        #region Draw
        public override void Draw(GameTime gameTime)
        {
            #region 3D
            world.effect.World = Matrix.Identity*Matrix.CreateTranslation(towerloc);
            world.effect.View = camera.view;
            world.effect.Projection = camera.projection;

            GraphicsDevice.SetVertexBuffer(vb);
            for (int i = 0; i < levels; i++)
            {
                for (int j = 0; j < sideLen; j++)
                {
                    for (int k = 0; k < sideLen; k++)
                    {
                        foreach (EffectPass pass in world.effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            // move to the current location
                            world.effect.World = Matrix.Identity*Matrix.CreateTranslation(new Vector3(towerloc.X + k, towerloc.Y + j, towerloc.Z + i));

                            // draw the cube
                            GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, towerCubeVertices.Count/3);

                        }
                    }
                }
            }

            #endregion

            #region 2D
            int topOfBox = 10;
            int bottomOfBox = 339;
            int scale = (bottomOfBox - topOfBox)/100;
            int draw_complete = percent_complete * scale;
            Rectangle completion_bar = new Rectangle(745, bottomOfBox - draw_complete ,20, draw_complete);
            Rectangle percent_background = new Rectangle(730, bottomOfBox+10, 53, 30);
            spriteBatch.Begin();
                spriteBatch.Draw(display_background, new Vector2(730, topOfBox), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(dummyTexture, percent_background, Color.Black);
                spriteBatch.DrawString(font, percent_complete + "%", new Vector2(740, bottomOfBox + 15), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(dummyTexture, completion_bar, Color.Salmon);
            spriteBatch.End();
            #endregion
            base.Draw(gameTime);
        }
        #endregion

        #region getGameComponents
        private void getHUD()
        {
            foreach (GameComponent c in game.Components)
            {
                if (c is HUD)
                {
                    hud = (HUD) c;
                    break;
                }
            }
        }

        private void getWorld()
        {
            foreach (GameComponent c in game.Components)
            {
                if (c is World)
                {
                    world = (World) c;
                }
            }
        }

        private void getCamera()
        {
            foreach (GameComponent c in game.Components)
            {
                if (c is Camera)
                {
                    camera = (Camera)c;
                }
            }
        }
        #endregion
    }
}
