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
        double mSize;
        Texture2D mTexture;
        Rectangle mSource;
        float angle;

        public Object(float x, float y)
        {
            mSize = 32;
            this.setPosition(x, y);
            mSource = new Rectangle(0, 0, 32, 32);
            this.angle = MathHelper.PiOver2;
        }

        public void genBaseTexture(GraphicsDevice graphic)
        {
            //definition d'une texture toute pourie
            mTexture = new Texture2D(graphic, 1, 1);
            mTexture.SetData(new Color[] { Color.White });
        }

        public Texture2D getBaseTexture()
        {
            return mTexture;
        }
        public Vector2 getPos()
        {
            return mPos;
        }

        public void setPosition(float x, float y)
        {
            this.mPos.X = x;
            this.mPos.Y = y;
        }

        public double getSize()
        {
            return this.mSize;
        }

        public void increaseAngle()
        {
            angle+=0.1f;
            if (angle > MathHelper.TwoPi)
            {
                angle = 0;
            }

        }

        public void decreaseAngle()
        {
            angle-=0.1f;
            if (angle < 0)
            {
                angle = MathHelper.TwoPi;
            }

        }

        public void setSize(double size)
        {
           this.mSize = size;
        }

        public void Draw(SpriteBatch sb,Texture2D texture = null)
        {
            if (texture == null)
            {
                texture = mTexture;
            }
            Vector2 center = new Vector2((float)this.mSize/2, (float)this.mSize);
            Rectangle destination = new Rectangle((int)this.getPos().X, (int)this.getPos().Y, (int)this.mSize, (int)this.mSize);
            
            sb.Draw(texture, destination, mSource, Color.White, angle - MathHelper.PiOver2, center, SpriteEffects.None, 0 );
        }

    }
}
