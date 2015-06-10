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
    public class FishTile : Tile
    {
        public int fishQuanity { get; set; }
        private int rDiff, gDiff, bDiff;

        public FishTile(Game game, Vector3 pos, float sideLen, Color color) 
            : base(game, pos, sideLen, color)
        {
            resourceType = ResourceType.Type.FISH;
            resourceColor = Color.Salmon;

            rDiff = resourceColor.R - color.R;
            gDiff = resourceColor.G - color.G;
            bDiff = resourceColor.B - color.B;

            fishQuanity = 100;
        }

        public override void Update()
        {
            color = new Color((int)((float)fishQuanity / 100f * rDiff + (float)baseColor.R),
                              (int)((float)fishQuanity / 100f * gDiff + (float)baseColor.G),
                              (int)((float)fishQuanity / 100f * bDiff + (float)baseColor.B));

            if (!selected)
                ChangeColor(color);

            base.Update();
        }

    }
}
