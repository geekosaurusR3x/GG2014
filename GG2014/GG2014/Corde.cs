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
    class Corde:Object
    {

        Vector2 mEnd;

        public Corde(int x, int y, int x2, int y2):base(x,y)
        {
            mEnd = new Vector2(x2, y2);
        }

        public Vector2 getStart()
        {
            return base.getPos();
        }

        public Vector2 getEnd()
        {
            return mEnd;
        }

        public Vector2 getVectorDir()
        {
            Vector2 temp = new Vector2((mEnd.X - base.getPos().X), (mEnd.Y - base.getPos().Y));
            temp.Normalize();
            return temp;
        }

        public void Draw(SpriteBatch sb)
        {
            Vector2 edge = mEnd - base.getPos();
            // calculate angle to rotate line
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            Rectangle dest = new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)base.getPos().X,
                    (int)base.getPos().Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    1);

            sb.Draw(base.getBaseTexture(),dest,null,Color.White,angle,new Vector2(0, 0),SpriteEffects.None, 0);

        }
    }
}
