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
    class Object
    {
        Vector2 mPos;
        double size;
        

        public Object(float x, float y)
        {
            size = 20;
            mPos.X = x;
            mPos.Y = y;   
        }


        public Vector2 getPos()
        {
            return mPos;
        }

        public double getSize()
        {
            return this.size;
        }

        public void setSize(double size)
        {
           this.size = size;
        }



    }
}
