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
    public class Penguin : DrawableGameComponent
    {
        private Model modelPenguin;
        public Vector3 penguinPos { get; set; }
        public BoundingSphere penguinBoundingSphere { get; set; }
        private float penguinRot = 0;

        private Game game;
        private Camera camera;
        private float scale = 0.001f;

        public bool selected { get; set; }
        public bool harvesting { get; set; }
        private int curTileIndex = -1;

        private World world;
        private GameTime gt;

        private int fps = 1; //stands for "fish per second" of course - number of fish collected per second when harvesting
        private bool moving = false;
        private Vector3 destPosition;
        private Vector3 traj;

        private Random rand;
        private HUD hud;

         public Penguin(Game mainGame, Vector3 startingPos) : base(mainGame) 
         {
             game = mainGame;
             selected = false;
             penguinPos = startingPos; //new Vector3(0, 0, 1.0375f);
             rand = new Random();
         }

         public override void Initialize() 
         {
             world = getWorld();
             hud = getHUD();
             base.Initialize(); 
         }

         protected override void LoadContent()
         {
             modelPenguin = game.Content.Load<Model>("Models\\penguin");
             penguinBoundingSphere = modelPenguin.Meshes[0].BoundingSphere.Transform(Matrix.CreateScale(scale) * Matrix.CreateTranslation(penguinPos));
             getCamera();
             base.LoadContent();
         }

         private bool atStopPosition()
         {
             double len = (penguinPos - destPosition).Length();
             bool stop = (len < 1.1f);
             return stop;
         }

        #region Update
        public override void Update(GameTime gameTime)
        {
            // update the bounding sphere to account for movement
            penguinBoundingSphere = modelPenguin.Meshes[0].BoundingSphere.Transform(Matrix.CreateScale(scale) * Matrix.CreateTranslation(penguinPos));

            gt = gameTime;
            KeyboardState state = Keyboard.GetState();

            // deselect all penguins if x is pressed
            if (state.IsKeyDown(Keys.X))
                selected = false;

            float speed = 0.1f;

            if (moving)
            {
                UpdatePenguinTrajectory();
                penguinPos += traj * speed;

                if (atStopPosition())
                {
                    selected = false;
                    curTileIndex = -1;
                    moving = false;
                }
            }

            // can only harvest if standing still
            if (!moving && curTileIndex == -1)
                CheckForHarvest();

            if (harvesting)
                Harvest();

            base.Update(gameTime);
        }
        #endregion

        #region Draw
         public override void Draw(GameTime gameTime)
         {
             Matrix[] transforms = new Matrix[modelPenguin.Bones.Count];
             modelPenguin.CopyAbsoluteBoneTransformsTo(transforms);
             
             foreach (ModelMesh mesh in modelPenguin.Meshes)
             {
                 // This is where the mesh orientation is set, as well 
                 // as our camera and projection.
                 foreach (BasicEffect effect in mesh.Effects)
                 {
                     effect.EnableDefaultLighting();

                     if (camera.isMap)
                     {
                         effect.FogEnabled = false;
                     }
                     else
                     {
                         effect.FogEnabled = true;
                         effect.FogStart = 30;
                         effect.FogEnd = 35;
                     }

                     if (selected)
                        effect.EmissiveColor = new Vector3(255, 0, 0);
                     else if (harvesting)
                         effect.EmissiveColor = new Vector3(0, 0, 255);
                     else
                        effect.EmissiveColor = new Vector3(0, 0, 0);
                     
                     effect.Projection = camera.projection;
                     effect.View = camera.view;
                     effect.World = Matrix.Identity * mesh.ParentBone.Transform *
                                 Matrix.CreateScale(scale) * Matrix.CreateRotationX(MathHelper.PiOver2) *
                                 Matrix.CreateRotationZ(penguinRot) *
                                 Matrix.CreateTranslation(penguinPos.X, penguinPos.Y, penguinPos.Z);
                 }
                 // Draw the mesh, using the effects set above.
                 mesh.Draw();
             }
        }
        #endregion

        #region Pengiun Movement
        public void CommandMovement(Vector3 pos)
        {
            destPosition = pos;

            moving = true;
            harvesting = false;

            traj = destPosition - new Vector3(penguinPos.X, penguinPos.Y, 0);
            traj.Normalize();
            selected = false;
        }

        private void UpdatePenguinTrajectory()
        {
            traj = destPosition - new Vector3(penguinPos.X, penguinPos.Y, 0);
            traj.Normalize();

            // calculate the current penguin angle
            if (traj.X < 0)
                penguinRot = (float)(Math.Atan(traj.Y / traj.X) - (Math.PI/2.0));
            else
                penguinRot = (float)(Math.Atan(traj.Y / traj.X) + (Math.PI/2.0));

            Vector3 newDestPos = destPosition;

            while (world.igloo.IglooPointCollision(newDestPos) || CheckPenguinDestinationCollision(newDestPos))
            {
                newDestPos = new Vector3((float) (newDestPos.X + (rand.Next(0, 2)-1)),
                                         (float) (newDestPos.Y + (rand.Next(0, 2)-1)), newDestPos.Z);
            }

            CommandMovement(newDestPos);
        }
        #endregion

        #region Collisions
        private bool CheckPenguinDestinationCollision(Vector3 pos)
        {
            // loop through all penguins and look for intersections
            for (int i = 0; i < world.penguins.Count; i++)
            {
                Penguin curPenguin = world.penguins[i];

                // don't compare a penguin to itself
                if (curPenguin.penguinPos == penguinPos)
                    continue;

                // distance between destination positions
                Vector3 destPosDiff = (curPenguin.destPosition - pos);

                // change the destination position, which will change the trajectory on the next loop
                if (destPosDiff.Length() < 0.7f)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Harvesting
        private void CheckForHarvest()
        {
            float px = penguinPos.X;
            float py = penguinPos.Y;

            // look through all fish tiles to check for intersection
            for (int i = 0; i < world.mapTiles.Count; i++)
            {
                if (world.mapTiles[i] is FishTile)
                {
                    if (px >= world.mapTiles[i].rect.X && px <= world.mapTiles[i].rect.X+world.mapTiles[i].rect.Width &&
                        py >= world.mapTiles[i].rect.Y && py <= world.mapTiles[i].rect.Y+world.mapTiles[i].rect.Height)
                    {
                        curTileIndex = i;
                        harvesting = true;
                        break;
                    }
                }
                else
                {
                    curTileIndex = i;
                    harvesting = false;
                }
            }
        }

        private double t = 0; //maybe not best to have this as a global variable?
        private void Harvest()
        {
            Tile curTile = world.mapTiles[curTileIndex];

            FishTile newFishTile = (FishTile) curTile;
            if (newFishTile.fishQuanity > 0)
            {
                t += gt.ElapsedGameTime.TotalMilliseconds;
                if (t >= 1000)
                {
                    hud.fish += 1;
                    newFishTile.fishQuanity -= 1;
                    world.mapTiles[curTileIndex] = newFishTile;
                    t -= 1000;
                }
            }
            else
            {
                harvesting = false;
                curTileIndex = -1;
            }
        }
        #endregion

        #region GetGameComponents
         private HUD getHUD()
         {
             foreach (GameComponent c in game.Components)
             {
                 if (c is HUD)
                     hud = (HUD)c;
             }
             return hud;
         }

         private World getWorld()
         {
             foreach (GameComponent c in game.Components)
             {
                 if (c is World)
                     world = (World)c;
             }
             return world;
         }

         private void getCamera()
         {
             // find the camera in the components list
             foreach (GameComponent c in game.Components)
             {
                 if (c is Camera)
                     camera = (Camera)c;
             }
         }
         #endregion
    }
}
