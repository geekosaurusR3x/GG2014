using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GG2014
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
