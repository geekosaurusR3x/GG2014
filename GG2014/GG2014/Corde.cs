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

namespace GG2014
{
    class Corde
    {
        Vector2 mStart;
        Vector2 mEnd;

        public Corde(int x, int y, int x2, int y2)
        {
            mStart = new Vector2(x, y);
            mEnd = new Vector2(x2, y2);
        }

        public Vector2 getStart()
        {
            return mStart;
        }

        public Vector2 getEnd()
        {
            return mEnd;
        }

        public Vector2 getVectorDir()
        {
            Vector2 temp = new Vector2((mEnd.X - mStart.X), (mEnd.Y - mStart.Y));
            temp.Normalize();
            return temp;
        }
    }
}
