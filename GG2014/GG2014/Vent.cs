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
        private int _force;
        private int _direction;
        private Rectangle source;
        private Rectangle[] destination;
        private Texture2D texture;

        public Vent(float x,float y, Texture2D tex,int dir):base(x,y)
        {

            this.source = new Rectangle(0, 0, 96, 96);
            this.destination = new Rectangle[4];
            texture = tex;
            for (int i = 0; i < 4; i++)
            {
                destination[i] = new Rectangle((int)(x +(i * 32)), (int)y, 32, 32);
            }
            _force = 32;
            _direction = dir;

        }

        public int Force
        {
            get { return _force; }
            set { _force = value; }
        }

        public int Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        public void setPosition(int xi,int yi)
        {
            if(xi>destination[0].X)
            {
                _direction = 1;
            }
            else
            {
                _direction = -1;
            }
            for(int i=0;i<4;i++)
                destination[i] = new Rectangle(xi*(i+1), yi, 96, 96);
            if (_force > 0)
            {
                _force--;
            }

            System.Console.WriteLine("Direction : " + _direction + "; Force : " + _force);
        }
        
        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < 4; i++)
                sb.Draw(texture, destination[i], source, Color.White);
        }

    }
}
