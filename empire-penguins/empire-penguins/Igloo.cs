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
    public class Igloo : Building
    {
        private int level = 0;
        private int penguinsPerLevel = 3;
        private int fishToNextLevel = 40;
        private HUD hud;
        private World world;
        private Game game;
        private Random rand;
        private bool upgrade = true;
        private bool upgradeKeyPressed = false;
        //private Model mymodel;
        private Camera camera;

        private Color sidecolor = Color.AliceBlue;
        private Color sidecolor2 = Color.AntiqueWhite;
        private Color roofColor = Color.DarkKhaki;
        private Color roofColor2 = Color.Khaki;
        private float wallLength = 2.0f;

        private VertexBuffer vb;
        private List<VertexPositionColor> iglooVertices;

        private List<Vector3> iglooPositions;

        public Igloo(Game g)
            : base(g)
        {
            game = g;
            iglooPositions = new List<Vector3>();
            iglooPositions.Add(new Vector3(2, 2, 0));
            rand = new Random();
        }

        #region Load Content
        protected override void LoadContent()
        {
            world = getWorld();
            hud = getHUD();
            camera = getCamera();
            if (upgrade)
            {
                Upgrade();
                upgrade = false;
            }

            #region House Vertices
            iglooVertices = new List<VertexPositionColor>();

            // front face
            iglooVertices.Add(new VertexPositionColor(new Vector3(0, 0, 0), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(0, 0, wallLength), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(wallLength, 0, 0), sidecolor));

            iglooVertices.Add(new VertexPositionColor(new Vector3(0, 0, wallLength), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(wallLength, 0, 0), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(wallLength, 0, wallLength), sidecolor));

            //right side
            iglooVertices.Add(new VertexPositionColor(new Vector3(wallLength, 0, 0), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(wallLength, 0, wallLength), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(wallLength, wallLength, 0), sidecolor2));

            iglooVertices.Add(new VertexPositionColor(new Vector3(wallLength, 0, wallLength), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(wallLength, wallLength, 0), sidecolor2));
            iglooVertices.Add(new VertexPositionColor(new Vector3(wallLength, wallLength, wallLength), sidecolor2));

            // back side
            iglooVertices.Add(new VertexPositionColor(new Vector3(wallLength, wallLength, 0), sidecolor2));
            iglooVertices.Add(new VertexPositionColor(new Vector3(wallLength, wallLength, wallLength), sidecolor2));
            iglooVertices.Add(new VertexPositionColor(new Vector3(0, wallLength, 0), sidecolor));

            iglooVertices.Add(new VertexPositionColor(new Vector3(wallLength, wallLength, wallLength), sidecolor2));
            iglooVertices.Add(new VertexPositionColor(new Vector3(0, wallLength, 0), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(0, wallLength, wallLength), sidecolor));

            // left side
            iglooVertices.Add(new VertexPositionColor(new Vector3(0, wallLength, 0), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(0, wallLength, wallLength), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(0, 0, 0), sidecolor2));

            iglooVertices.Add(new VertexPositionColor(new Vector3(0, wallLength, wallLength), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(0, 0, 0), sidecolor2));
            iglooVertices.Add(new VertexPositionColor(new Vector3(0, 0, wallLength), sidecolor2));

            // left roof side
            iglooVertices.Add(new VertexPositionColor(new Vector3(0, 0, wallLength), roofColor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(0, wallLength, wallLength), roofColor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(wallLength / 2.0f, wallLength / 2.0f, wallLength * (3.0f / 2.0f)), roofColor));

            // right roof side
            iglooVertices.Add(new VertexPositionColor(new Vector3(0, 0, wallLength), roofColor2));
            iglooVertices.Add(new VertexPositionColor(new Vector3(wallLength, 0, wallLength), roofColor2));
            iglooVertices.Add(new VertexPositionColor(new Vector3(wallLength / 2.0f, wallLength / 2.0f, wallLength * (3.0f / 2.0f)), roofColor2));


            iglooVertices.Add(new VertexPositionColor(new Vector3(wallLength, 0, wallLength), roofColor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(wallLength, wallLength, wallLength), roofColor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(wallLength / 2.0f, wallLength / 2.0f, wallLength * (3.0f / 2.0f)), roofColor));


            iglooVertices.Add(new VertexPositionColor(new Vector3(0, 0, wallLength), roofColor2));
            iglooVertices.Add(new VertexPositionColor(new Vector3(0, 0, wallLength), roofColor2));
            iglooVertices.Add(new VertexPositionColor(new Vector3(wallLength / 2.0f, wallLength / 2.0f, wallLength * (3.0f / 2.0f)), roofColor2));
            #endregion

            vb = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, iglooVertices.Count, BufferUsage.None);
            vb.SetData(iglooVertices.ToArray());

            base.LoadContent();
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
         {
            hud.penguins = level * penguinsPerLevel;
            KeyboardState k = Keyboard.GetState();

            if (k.IsKeyDown(Keys.I) && !upgradeKeyPressed)
            {
                upgradeKeyPressed = true;
                if (hud.fish >= fishToNextLevel)
                {
                    Upgrade();
                    hud.fish -= fishToNextLevel;
                    hud.message = "";

                    // find new igloo position
                    Vector3 newPos = iglooPositions[rand.Next(0, iglooPositions.Count-1)];
                    while (IglooIglooCollision(newPos))
                    {
                        newPos = new Vector3(newPos.X + rand.Next(0, 4) - 2,
                                             newPos.Y + rand.Next(0, 4) - 2, newPos.Z);
                    }

                    iglooPositions.Add(newPos);

                    // find all the penguins covered by this new position and move them
                    for (int i = 0; i < world.penguins.Count; i++)
                    {
                        if (IglooPointCollision(world.penguins[i].penguinPos))
                        {
                            world.penguins[i].CommandMovement(new Vector3(world.penguins[i].penguinPos.X, world.penguins[i].penguinPos.Y, 0));
                        }
                    }

                }
                else
                {
                    hud.setMessage("Not enough fish");
                }
            }
            else if (k.IsKeyUp(Keys.I))
                upgradeKeyPressed = false;

            base.Update(gameTime);
        }
        #endregion

        #region Igloo Collision
        // collision between an igloo and an ingloo
        public bool IglooIglooCollision(Vector3 pos)
        {
            for (int i = 0; i < iglooPositions.Count; i++)
            {
                if (((pos.X >= iglooPositions[i].X && pos.X <= iglooPositions[i].X+wallLength) ||
                    ((pos.X+wallLength >= iglooPositions[i].X && pos.X+wallLength <= iglooPositions[i].X+wallLength)))
                    &&
                    ((pos.Y >= iglooPositions[i].Y && pos.Y <= iglooPositions[i].Y+wallLength) ||
                    ((pos.Y+wallLength >= iglooPositions[i].Y && pos.Y+wallLength <= iglooPositions[i].Y+wallLength))))
                {
                    return true;
                }
            }
            return false;
        }

        // collision between an igloo and a point
        public bool IglooPointCollision(Vector3 pos)
        {
            for (int i = 0; i < iglooPositions.Count; i++)
            {
                if ((pos.X >= iglooPositions[i].X && pos.X <= iglooPositions[i].X + wallLength) &&
                    (pos.Y >= iglooPositions[i].Y && pos.Y <= iglooPositions[i].Y + wallLength))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Draw
        public override void Draw(GameTime gameTime)
        {
            // change to the igloo vertex buffer
            GraphicsDevice.SetVertexBuffer(vb);

            world.effect.View = camera.view;
            if (camera.changed)
            {
                world.effect.Projection = camera.projection;
                camera.changed = false;
            }

            for (int i = 0; i < iglooPositions.Count; i++)
            {
                // translate to the current igloo
                world.effect.World = Matrix.Identity * Matrix.CreateTranslation(iglooPositions[i]);
                foreach (EffectPass pass in world.effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vb.VertexCount/3);
                }
            }

            base.Draw(gameTime);
        }
        #endregion

        #region Add New Igloo

        private void AddIgloo(Vector3 pos)
        {
            // front face
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X, pos.Y, pos.Z), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X, pos.Y, pos.Z+wallLength), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X+wallLength, pos.Y, pos.Z), sidecolor));

            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X, pos.Y, pos.Z+wallLength), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X+wallLength, pos.Y, pos.Z), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X + wallLength, pos.Y, pos.Z+wallLength), sidecolor));

            //right side
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X+wallLength, pos.Y, pos.Z), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X+wallLength, pos.Y, pos.Z + wallLength), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X + wallLength, pos.Y+wallLength, pos.Z), sidecolor2));

            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X+wallLength, pos.Y, pos.Z + wallLength), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X + wallLength, pos.Y+wallLength, pos.Z), sidecolor2));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X+wallLength, pos.Y + wallLength, pos.Z + wallLength), sidecolor2));

            // back side
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X + wallLength, pos.Y+wallLength, pos.Z), sidecolor2));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X + wallLength, pos.Y + wallLength, pos.Z+wallLength), sidecolor2));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X, pos.Y+wallLength, pos.Z), sidecolor));

            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X+wallLength, pos.Y + wallLength, pos.Z + wallLength), sidecolor2));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X, pos.Y+wallLength, pos.Z), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X, pos.Y+wallLength, pos.Z + wallLength), sidecolor));

            // left side
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X, pos.Y+wallLength, pos.Z), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X, pos.Y + wallLength, pos.Z+wallLength), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X, pos.Y, pos.Z), sidecolor2));

            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X, pos.Y + wallLength, pos.Z+wallLength), sidecolor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X, pos.Y, pos.Z), sidecolor2));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X, pos.Y, pos.Z+wallLength), sidecolor2));

            // left roof side
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X, pos.Y, pos.Z+wallLength), roofColor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X, pos.Y+wallLength, pos.Z + wallLength), roofColor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X + wallLength / 2.0f, pos.Y + wallLength / 2.0f, pos.Z+wallLength * (3.0f / 2.0f)), roofColor));

            // right roof side
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X, pos.Y, pos.Z+wallLength), roofColor2));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X + wallLength, pos.Y, pos.Z+wallLength), roofColor2));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X + wallLength / 2.0f, pos.Y + wallLength / 2.0f, pos.Z+wallLength * (3.0f / 2.0f)), roofColor2));

            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X + wallLength, pos.Y, pos.Z+wallLength), roofColor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X + wallLength, pos.Y + wallLength, pos.Z+wallLength), roofColor));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X + wallLength / 2.0f, pos.Y + wallLength / 2.0f, pos.Z+wallLength * (3.0f / 2.0f)), roofColor));

            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X, pos.Y, pos.Z+wallLength), roofColor2));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X, pos.Y, pos.Z+wallLength), roofColor2));
            iglooVertices.Add(new VertexPositionColor(new Vector3(pos.X + wallLength / 2.0f, pos.Y + wallLength / 2.0f, pos.Z+wallLength * (3.0f / 2.0f)), roofColor2));
        }

        #endregion

        #region Upgrade
        private void Upgrade()
        {
            level += 1;
            for (int i = 0; i < penguinsPerLevel; i++)
            {
                Penguin p = new Penguin(Game, new Vector3(0f, (float)(1*i), 1.0375f));
                
                world.penguins.Add(p);
                game.Components.Add(p);
            }
        }
        #endregion
    }
}
