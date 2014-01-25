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
    class Vent:Object
    {
        private Rectangle source, destination;
        private Texture2D texture;
        public Vent(float x,float y, Texture2D tex):base(x,y)
        {
            this.source = new Rectangle(0, 0, 32, 32);
            this.destination = new Rectangle((int)x, (int)y, 32, 32);
            this.texture = tex;
        }

        public void setPosition(int xi,int yi)
        {
            destination = new Rectangle(xi, yi, 32, 32);  
        }
        
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, destination, source, Color.White);
        }

    }
}
