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
        protected Texture2D mTexture;
        Rectangle mSource;

        public Object(float x, float y)
        {
            mSize = 32;
            this.setPosition(x, y);
            mSource = new Rectangle(0, 0, 32, 32);
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

        public void setTexture(Texture2D texture)
        {
            this.mTexture = texture;
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

        public void setSize(double size)
        {
           this.mSize = size;
           mSource = new Rectangle(0, 0, (int)mSize, (int)mSize);
        }

        public Rectangle getSource()
        {
            return mSource;
        }

        public void Draw(SpriteBatch sb,Texture2D texture = null)
        {
            if (texture == null)
            {
                texture = mTexture;
            }
            Rectangle destination = new Rectangle((int)this.getPos().X - ((int)this.mSize / 2), (int)this.getPos().Y - ((int)this.mSize / 2), (int)this.mSize, (int)this.mSize);
            
            sb.Draw(texture, destination, mSource, Color.White);
        }
    }
}
