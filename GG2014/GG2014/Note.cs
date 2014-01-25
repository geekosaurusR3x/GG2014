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

namespace GG2014
{
    class Note : Object
    {
        private int mVie;
        private Texture2D[] textures;
        float angle;
        
        public Note(int x,int y,Texture2D tex1,Texture2D tex2,Texture2D tex3,int nbVie=3): base(x,y)
        {
            this.mVie = nbVie;
            this.textures = new Texture2D[nbVie];
            textures[0]=tex1;
            textures[1]=tex2;
            textures[2]=tex3;
            this.setSize(32);
            this.angle = MathHelper.PiOver2;
        }

        public int vie 
        {
            get{return mVie;}
            set { mVie = value; }
        }


        public void Draw(SpriteBatch sb)
        {
            Vector2 center = new Vector2((float)this.getSize() / 2, (float)this.getSize());
            Rectangle destination = new Rectangle((int)this.getPos().X, (int)this.getPos().Y, (int)this.getSize(), (int)this.getSize());

            sb.Draw(textures[mVie - 1], destination, this.getSource(), Color.White, angle - MathHelper.PiOver2, center, SpriteEffects.None, 0);
        }

        public void increaseAngle()
        {
            angle += 0.1f;
            if (angle > MathHelper.TwoPi)
            {
                angle = 0;
            }

        }

        public void decreaseAngle()
        {
            angle -= 0.1f;
            if (angle < 0)
            {
                angle = MathHelper.TwoPi;
            }

        }

        public int getLivesLeft()
        {
            return mVie;
        }

        public void kill()
        {
            mVie--;
        }

        public void cheetah()
        {
            mVie = 3;
        }

        public void resetAngle()
        {
            this.angle = MathHelper.PiOver2;
        }

        public double getAngle()
        {
            return this.angle;
        }
    }
}
