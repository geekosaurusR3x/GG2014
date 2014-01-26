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

namespace GameStateManagementSample
{
    class Enemis : Object
    {
        Vector2 mDir;
        public Enemis(float x, float y, Vector2 dir)
            : base(x, y)
        {
            this.mDir = dir;
        }

        public Vector2 getDir()
        {
            return mDir;
        }
    }
}
