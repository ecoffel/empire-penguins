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
    public class IceTile : Tile
    {
        public IceTile(Game game, Vector3 pos, float sideLen, Color color)
            : base(game, pos, sideLen, color)
        {
            resourceType = ResourceType.Type.ICE;
            resourceColor = Color.White;
            color = baseColor;
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
