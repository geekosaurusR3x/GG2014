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
        bool anim;
        Vector2 p1;
        Vector2 p2;
        Vector2 Corde;
        float JumAngle;

        public Enemis(float x, float y, Vector2 dir)
            : base(x, y)
        {
            this.mDir = dir;
            anim = false;
        }

        public Vector2 getDir()
        {
            return mDir;
        }

        public bool isAnim()
        {
            return anim;
        }

        public void setAnim(bool anim)
        {
            this.anim = anim;
        }

        public Vector2 getP1()
        {
            return p1;
        }

        public void setP1(Vector2 p1)
        {
            this.p1 = p1;
        }

        public Vector2 getP2()
        {
            return p2;
        }

        public void setP2(Vector2 p2)
        {
            this.p2 = p2;
        }

        public Vector2 getCorde()
        {
            return Corde;
        }

        public void setCorde(Vector2 c)
        {
            this.Corde = c;
        }

        public void setJumpAngle(float a)
        {
            this.JumAngle = a;
        }

        public float getJumpAngle()
        {

            return this.JumAngle;
        }
    }
}
