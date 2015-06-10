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
    public class ResourceType
    {
        public enum Type { ICE, FISH };
    }

    public class Tile
    {
        public ResourceType.Type resourceType;

        public Color color;
        protected Color baseColor;
        protected Color resourceColor;
        protected Color selectedColor;
        public bool textured { get; set; }
        public Texture2D tex { get; set; }
        public Vector3 topLeftPos;
        public float size;

        public bool selected { get; set; }
        public BoundingBox boundingBox { get; set; }
        public Rectangle rect { get; set; }
        public List<VertexPositionColor> verticies { get; set; }

        protected BasicEffect effect;
        protected Camera camera;

        public Tile(Game game, Vector3 pos, float sideLen, Color tileColor)
        {
            topLeftPos = pos;
            baseColor = tileColor;
            color = tileColor;
            selectedColor = Color.Black;
            size = sideLen;
            selected = false;

            boundingBox = new BoundingBox(topLeftPos, new Vector3(topLeftPos.X + sideLen, topLeftPos.Y + sideLen, topLeftPos.Z + 0.1f));
            rect = new Rectangle((int) topLeftPos.X, (int) topLeftPos.Y, (int) sideLen, (int) sideLen);

            verticies = new List<VertexPositionColor>();
            // top left triangle
            verticies.Add(new VertexPositionColor(new Vector3(topLeftPos.X, topLeftPos.Y, topLeftPos.Z), color));
            verticies.Add(new VertexPositionColor(new Vector3(topLeftPos.X, topLeftPos.Y + size, topLeftPos.Z), color));
            verticies.Add(new VertexPositionColor(new Vector3(topLeftPos.X + size, topLeftPos.Y, topLeftPos.Z), color));

            // bottom right triangle
            verticies.Add(new VertexPositionColor(new Vector3(topLeftPos.X + size, topLeftPos.Y, topLeftPos.Z), color));
            verticies.Add(new VertexPositionColor(new Vector3(topLeftPos.X, topLeftPos.Y + size, topLeftPos.Z), color));
            verticies.Add(new VertexPositionColor(new Vector3(topLeftPos.X + size, topLeftPos.Y + size, topLeftPos.Z), color));

            // find the camera in the components list
            foreach (GameComponent c in game.Components)
            {
                if (c is Camera)
                    camera = (Camera)c;
            }
        }

        public virtual void Update()
        {
            
        }

        public void Select(bool val)
        {
            selected = val;
            if (val)
                ChangeColor(selectedColor);
            else
                ChangeColor(color);
        }

        protected void ChangeColor(Color c)
        {
            for (int i = 0; i < verticies.Count; i++)
            {
                verticies[i] = new VertexPositionColor(verticies[i].Position, c);
            }
        }

        public Vector3 center()
        {
            Vector3 center = new Vector3(0.5f * size, 0.5f * size, 0);
            return topLeftPos + center;
        }
    }
}
