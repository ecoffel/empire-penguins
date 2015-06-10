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
    public class Camera : DrawableGameComponent
    {
        public Vector3 pos { get; set; }
        private Vector3 origPos;
        public Vector3 target { get; set; }
        private Vector3 origTarget;
        public Vector3 up { get; set; }

        public Matrix view { get; set; }
        public Matrix unmapped_view { get; set; }
        public Vector3 unmapped_center { get; set; }

        public bool isMap { get; set; }

        private bool homeKeyPressed = false;
        private bool goHome = false;

        private bool idlePenguinKeyPressed = false;
        private bool goToIdlePenguin = false;
        private Vector3 idlePenguinPos;
        private Vector3 idlePenguinPosTarget;

        private World world;

        private Matrix projectionMatrix;
        public Matrix projection {
            get { return projectionMatrix; } 
            set { projectionMatrix = value; changed = true;} 
        }

        public bool changed { get; set; }

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up) : base(game)
        {
            this.pos = pos;
            origPos = pos;
            this.target = target;
            origTarget = target;
            this.up = up;
            changed = true;
            view = Matrix.CreateLookAt(pos, target, up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float) Game.Window.ClientBounds.Width/
                                                                                 (float) Game.Window.ClientBounds.Height,
                                                             1f, 1000f);
        }

        #region Load Content
        protected override void LoadContent()
        {
            foreach (GameComponent c in Game.Components)
            {
                if (c is World)
                    world = (World) c;
            }
            base.LoadContent();
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.M))
            {
                //top view
                //something changed in the last update that means this does not work any more
                view = Matrix.CreateLookAt(new Vector3(0f, 4f, 150f), new Vector3(0f, 4f, 0f), new Vector3(0f, 1f, 0f));
                isMap = true;
            }
            else
            {
                view = Matrix.CreateLookAt(pos, target, up);
                unmapped_view = view;
                isMap = false;
                ChangePos();
            }

            if (state.IsKeyDown(Keys.O))
            {
                if (!homeKeyPressed && !goHome)
                {
                    homeKeyPressed = true;
                    goHome = true;
                }    
            }
            else
            {
                homeKeyPressed = false;
            }
            
            if (goHome)
            {
                float xDiff = origPos.X - pos.X;
                float yDiff = origPos.Y - pos.Y;
                float zDiff = origPos.Z - pos.Z;

                float xTargDiff = origTarget.X - target.X;
                float yTargDiff = origTarget.Y - target.Y;
                float zTargDiff = origTarget.Z - target.Z;

                pos = new Vector3(pos.X + xDiff/15, pos.Y + yDiff/15, pos.Z + zDiff/15);
                target = new Vector3(target.X + xTargDiff/15, target.Y + yTargDiff/15, target.Z + zTargDiff/15);

                if ((pos - origPos).Length() < 1.0)
                    goHome = false;
            }

            if (state.IsKeyDown(Keys.P))
            {
                if (!idlePenguinKeyPressed && !goToIdlePenguin)
                {
                    Penguin idle = world.FindIdlePenguin();

                    if (idle != null)
                    {
                        idlePenguinPosTarget = idle.penguinPos;
                        idlePenguinPos = new Vector3(idlePenguinPosTarget.X, idlePenguinPosTarget.Y - 10,
                                                     idlePenguinPosTarget.Z + 15);

                        idlePenguinKeyPressed = true;
                        goToIdlePenguin = true;
                    }
                }
            }
            else
            {
                idlePenguinKeyPressed = false;
            }

            if (goToIdlePenguin)
            {
                float xDiff = idlePenguinPos.X - pos.X;
                float yDiff = idlePenguinPos.Y - pos.Y;
                float zDiff = idlePenguinPos.Z - pos.Z;

                float xTargDiff = idlePenguinPosTarget.X - target.X;
                float yTargDiff = idlePenguinPosTarget.Y - target.Y;
                float zTargDiff = idlePenguinPosTarget.Z - target.Z;

                pos = new Vector3(pos.X + xDiff / 15, pos.Y + yDiff / 15, pos.Z + zDiff / 15);
                target = new Vector3(target.X + xTargDiff / 15, target.Y + yTargDiff / 15, target.Z + zTargDiff / 15);

                if ((pos - idlePenguinPos).Length() < 1.0)
                    goToIdlePenguin = false;
            }

            if (isMap)
            {
                goToIdlePenguin = false;
                goHome = false;
            }

            base.Update(gameTime);
        }
        #endregion

        #region Change Position
        private void ChangePos()
        {
            KeyboardState state = Keyboard.GetState();
            //int len_side = 50;
            int left_lim = -30;
            int right_lim = 30;
            int top_lim = 25;
            int bottom_lim = -60;

            #region Key Commands
            if (state.IsKeyDown(Keys.Left))
            {
                if (this.pos.X >= left_lim)
                {
                    this.pos = new Vector3(this.pos.X - 0.1f, this.pos.Y, this.pos.Z);
                    this.target = new Vector3(this.target.X - 0.1f, this.target.Y, this.target.Z);
                }
            }

            if (state.IsKeyDown(Keys.Right))
            {
                if (this.pos.X <= right_lim)
                {
                    this.pos = new Vector3(this.pos.X + 0.1f, this.pos.Y, this.pos.Z);
                    this.target = new Vector3(this.target.X + 0.1f, this.target.Y, this.target.Z);
                }
            }

            if (state.IsKeyDown(Keys.Up))
            {
                if (this.pos.Y <= top_lim)
                {
                    this.pos = new Vector3(this.pos.X, this.pos.Y + 0.1f, this.pos.Z);
                    this.target = new Vector3(this.target.X, this.target.Y + 0.1f, this.target.Z);
                }
            }

            if (state.IsKeyDown(Keys.Down))
            {
                if (this.pos.Y >= bottom_lim)
                {
                    this.pos = new Vector3(this.pos.X, this.pos.Y - 0.1f, this.pos.Z);
                    this.target = new Vector3(this.target.X, this.target.Y - 0.1f, this.target.Z);
                }
            }

            if (state.IsKeyDown(Keys.W))
            {
                //this.pos = new Vector3(this.pos.X, this.pos.Y, this.pos.Z + 0.1f);
                //this.target = new Vector3(this.target.X, this.target.Y, this.target.Z + 0.1f);
            }

            if (state.IsKeyDown(Keys.S))
            {
                //this.pos = new Vector3(this.pos.X, this.pos.Y, this.pos.Z - 0.1f);
                //this.target = new Vector3(this.target.X, this.target.Y, this.target.Z - 0.1f);
            }

            if (state.IsKeyDown(Keys.E))
            {
                if (this.target.Z < 10f)
                    this.target = new Vector3(this.target.X, this.target.Y, this.target.Z + 0.1f);
            }

            if (state.IsKeyDown(Keys.D))
            {
                if (this.target.Z > 0)
                    this.target = new Vector3(this.target.X, this.target.Y, this.target.Z - 0.1f);
            }
            #endregion
        }
        #endregion
    }
}
