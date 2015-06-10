using System;
using System.IO;
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
    public class World : DrawableGameComponent
    {
        private VertexDeclaration vertexDeclaration;
        private List<VertexPositionColor> verticies;
        public BasicEffect effect;

        private List<VertexPositionColor> vbVerticesToAdd;
        private VertexBuffer vb;

        public List<Tile> mapTiles { get; set; }
        public float worldWidth = 0;
        public float worldHeight = 0;
        private bool mapChanged = false;

        private Game game;
        private Camera camera;
        public Igloo igloo;
        public List<Penguin> penguins;

        private bool dragging = false;
        private bool mousePressed = false;
        private bool waitingForMovementCommand = false;
        private Vector2 dragStartPoint = new Vector2();
        private Vector2 dragCurPoint = new Vector2();
        public int tileSelected1Index = -1;
        public int tileSelected2Index = -1;
        public int tileMoveCommand = -1;

        private HUD hud;
        private Random rand;
        private string mapFile = "default.txt";

        // each number in the map file translates into this number of tiles
        private int mapCompression = 4;

        public World(Game mainGame) : base(mainGame)
        {
            game = mainGame;
            mapTiles = new List<Tile>();
            verticies = new List<VertexPositionColor>();
            vbVerticesToAdd = new List<VertexPositionColor>();
            penguins = new List<Penguin>();
            rand = new Random();
        }

        #region LoadContent
        protected override void LoadContent()
        {
            getHUD();
            getIgloo();

            effect = new BasicEffect(Game.GraphicsDevice);
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = false;

            // this is required for textures that are not a power of 2
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            ss.AddressW = TextureAddressMode.Clamp;
            GraphicsDevice.SamplerStates[0] = ss;

            BuildMap();

            // find the camera in the components list
            foreach (GameComponent c in game.Components)
            {
                if (c is Camera)
                    camera = (Camera) c;
            }

            base.LoadContent();
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            foreach (Tile mapTile in mapTiles)
                mapTile.Update();

            UpdateMap();

            #region Mouse Selection
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                tileMoveCommand = -1;

                if (!mousePressed)
                {
                    dragStartPoint = new Vector2(mouseState.X, mouseState.Y);
                    mousePressed = true;
                }

                dragCurPoint = new Vector2(mouseState.X, mouseState.Y);

                // if the mouse has moved while down, start the dragging code
                if (!dragging && (dragCurPoint-dragStartPoint).Length() > 5)
                {
                    //UnselectPenguins();
                    dragging = true;
                    waitingForMovementCommand = false;

                    tileSelected1Index = GetSelectedTileIndex(dragStartPoint);
                    tileSelected2Index = GetSelectedTileIndex(dragCurPoint);
                }
                else if (dragging)
                {
                    tileSelected2Index = GetSelectedTileIndex(dragCurPoint);
                    waitingForMovementCommand = false;

                    #region Select Box of Tiles
                    if (mapTiles[tileSelected1Index] != null && mapTiles[tileSelected2Index] != null)
                    {
                        // select the tiles within the box
                        for (int i = 0; i < mapTiles.Count; i++)
                        {
                            int minX = Math.Min(mapTiles[tileSelected1Index].rect.X, mapTiles[tileSelected2Index].rect.X);
                            int maxX = Math.Max(mapTiles[tileSelected1Index].rect.X, mapTiles[tileSelected2Index].rect.X);

                            int minY = Math.Min(mapTiles[tileSelected1Index].rect.Y, mapTiles[tileSelected2Index].rect.Y);
                            int maxY = Math.Max(mapTiles[tileSelected1Index].rect.Y, mapTiles[tileSelected2Index].rect.Y);

                            if (mapTiles[i].rect.X >= minX && mapTiles[i].rect.Y >= minY &&
                                mapTiles[i].rect.X <= maxX && mapTiles[i].rect.Y <= maxY)
                            {
                                mapTiles[i].Select(true);
                            }
                            else
                            {
                                mapTiles[i].Select(false);
                            }
                        }
                    }
                    #endregion
                }
                else if (waitingForMovementCommand)
                {
                    // select the current tile
                    int curTile = GetSelectedTileIndex(dragCurPoint);
                    tileSelected2Index = GetSelectedTileIndex(dragCurPoint);
                    mapTiles[curTile].Select(true);
                }
            }
            else
            {
                if (dragging)
                {
                    dragging = false;

                    // select penguins based on the tiles
                    if (tileSelected1Index != -1 && tileSelected2Index != -1)
                    {
                        int numPenguinsSelected = SelectPenguins(mapTiles[tileSelected1Index],
                                                                 mapTiles[tileSelected2Index]);

                        if (numPenguinsSelected > 0)
                            waitingForMovementCommand = true;

                        for (int i = 0; i < mapTiles.Count; i++)
                            mapTiles[i].Select(false);
                    }
                }
                else if (mousePressed)
                {
                    // deselect all map tiles
                    for (int i = 0; i < mapTiles.Count; i++)
                        mapTiles[i].Select(false);

                    // if we're waiting to select a movement location for the selected penguins
                    if (waitingForMovementCommand)
                    {
                        tileMoveCommand = GetSelectedTileIndex(dragCurPoint);
                        // find seleced penguins
                        for (int i = 0; i < penguins.Count; i++ )
                        {
                            if (penguins[i].selected)
                                penguins[i].CommandMovement(mapTiles[tileMoveCommand].center());
                        }
                        waitingForMovementCommand = false;
                    }
                    // otherwise just select 
                    else
                    {
                        Ray ray = GetUnprojectRay(dragCurPoint);

                        for (int i = 0; i < penguins.Count; i++)
                        {
                            if (ray.Intersects(penguins[i].penguinBoundingSphere) != null)
                            {
                                penguins[i].selected = true;
                                waitingForMovementCommand = true;
                            }
                            else
                            {
                                penguins[i].selected = false;
                            }
                        }    
                    }
                }
                
                mousePressed = false;
                dragging = false;
                dragStartPoint = new Vector2();
                dragCurPoint = new Vector2();
                tileSelected1Index = -1;
                tileSelected2Index = -1;
            }
            #endregion

            if (mapChanged)
                UpdateVertexBuffer();

            base.Update(gameTime);
        }
        #endregion

        #region Unproject Functions

        private Ray GetUnprojectRay(Vector2 mousePos)
        {
            Matrix unProjectWorldMatrix = Matrix.CreateTranslation(0f, 0f, 0f);

            Vector3 nearSource1 = new Vector3(mousePos.X, mousePos.Y, 0f);
            Vector3 farSource1 = new Vector3(mousePos.X, mousePos.Y, 1f);

            Vector3 nearPoint1 = GraphicsDevice.Viewport.Unproject(nearSource1, camera.projection, camera.view, unProjectWorldMatrix);
            Vector3 farPoint1 = GraphicsDevice.Viewport.Unproject(farSource1, camera.projection, camera.view, unProjectWorldMatrix);

            Vector3 direction1 = farPoint1 - nearPoint1;
            direction1.Normalize();
            return new Ray(nearPoint1, direction1);
        }

        private int GetSelectedTileIndex(Vector2 mousePos)
        {
            Ray ray = GetUnprojectRay(mousePos);

            // find the tile
            for (int i = 0; i < mapTiles.Count; i++)
            {
                if (ray.Intersects(mapTiles[i].boundingBox) != null)
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion
        
        #region Draw
        public override void Draw(GameTime gameTime)
        {
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

            effect.World = Matrix.Identity;
            effect.View = camera.view;
            if (camera.changed)
            {
                effect.Projection = camera.projection;
                camera.changed = false;
            }

            // change to the map vertex buffer
            GraphicsDevice.SetVertexBuffer(vb);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                // draw out of the vertex buffer
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vb.VertexCount/3);
            }

            base.Draw(gameTime);
        }
        #endregion

        #region Vertex Buffers
        public void AddToVertexBuffer(List<VertexPositionColor> v)
        {
            vbVerticesToAdd.AddRange(v);
            mapChanged = true;
        }

        private void UpdateVertexBuffer()
        {
            vb = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, vbVerticesToAdd.Count, BufferUsage.None);
            vb.SetData(vbVerticesToAdd.ToArray());
            GraphicsDevice.SetVertexBuffer(vb);
            vbVerticesToAdd.Clear();
            mapChanged = false;
        }
        #endregion

        #region Select / Unselect Penguins
        private int SelectPenguins(Tile t1, Tile t2)
        {
            int numPenguins = 0;

            int minX = Math.Min(mapTiles[tileSelected1Index].rect.X, mapTiles[tileSelected2Index].rect.X);
            int maxX = Math.Max(mapTiles[tileSelected1Index].rect.X+1, mapTiles[tileSelected2Index].rect.X+1);

            int minY = Math.Min(mapTiles[tileSelected1Index].rect.Y, mapTiles[tileSelected2Index].rect.Y);
            int maxY = Math.Max(mapTiles[tileSelected1Index].rect.Y+1, mapTiles[tileSelected2Index].rect.Y+1);

            for (int i = 0; i < penguins.Count; i++)
            {
                if (penguins[i].penguinPos.X >= minX && penguins[i].penguinPos.X <= maxX &&
                    penguins[i].penguinPos.Y >= minY && penguins[i].penguinPos.Y <= maxY)
                {
                    penguins[i].selected = true;
                    numPenguins++;
                }
            }

            return numPenguins;
        }

        private void UnselectPenguins()
        {
            for (int i = 0; i < penguins.Count; i++)
                penguins[i].selected = false;
        }
        #endregion

        #region Build / Update Map
        private void UpdateMap()
        {
            foreach (Tile t in mapTiles)
            {
                AddToVertexBuffer(t.verticies);
            }
        }

        private void BuildMap()
        {
            string[] mapLines = File.ReadAllLines(mapFile);

            worldHeight = mapLines.Length*mapCompression;

            List<Tile> tempMapTiles = new List<Tile>();

            bool black = false;
            for (int i = 0; i < mapLines.Length; i++)
            {
                // skip comment lines
                if (mapLines[i].Trim()[0] == ';')
                    continue;

                if (worldWidth == 0)
                    worldWidth = mapLines[i].Length*mapCompression;

                for (int j = 0; j < mapLines[i].Length; j++)
                {
                    char c = mapLines[i].Trim()[j];
                    int tileType = int.Parse(c.ToString());

                    for (int k = 0; k < mapCompression; k++)
                    {
                        black = !black;
                        for (int h = 0; h < mapCompression; h++)
                        {
                            Color color = (black ? Color.LightBlue : Color.GhostWhite);

                            switch (tileType)
                            {
                                case 1:
                                    tempMapTiles.Add(new IceTile(Game, new Vector3((j - mapLines[i].Length / 2) * mapCompression + h, (i - mapLines.Length / 2) * mapCompression + k, 0), 1, color));
                                    break;
                                case 2:
                                    tempMapTiles.Add(new FishTile(Game, new Vector3((j - mapLines[i].Length / 2) * mapCompression + h, (i - mapLines.Length / 2) * mapCompression + k, 0), 1, color));
                                    break;
                            }

                            black = !black;
                        }
                    }
                }
            }

            for (int i = 0; i < tempMapTiles.Count; i++)
            {
                mapTiles.Add(tempMapTiles[i]);
            }

            verticies.Clear();
            foreach (Tile t in mapTiles)
            {
                AddToVertexBuffer(t.verticies);
            }
        }
        #endregion

        #region Find Idle Penguin
        public Penguin FindIdlePenguin()
        {
            List<Penguin> idle = new List<Penguin>();
            for (int i = 0; i < penguins.Count; i++)
            {
                if (!penguins[i].harvesting)
                    idle.Add(penguins[i]);
            }
            if (idle.Count > 0)
                return idle[rand.Next(0, idle.Count - 1)];
            else
                return null;
        }
        #endregion

        #region Get Game Components
        public void getHUD()
        {
            foreach (GameComponent x in Game.Components)
            {
                if (x is HUD)
                     hud = (HUD)x;
            }
        }

        public int destTile()
        {
            return tileMoveCommand;
        }

        private void getIgloo()
        {
            foreach (GameComponent x in Game.Components)
            {
                if (x is Igloo)
                    igloo = (Igloo)x;
            }
        }

        #endregion
    }
}
    