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
    class Note
    {
        private int _vie;
        private Texture2D[] textures;
        private Rectangle source;
        private int x,y;
        
        public Note(int x,int y,Texture2D tex1,Texture2D tex2,Texture2D tex3,int nbVie=3)
        {
            this._vie = nbVie;
            this.textures = new Texture2D[nbVie];
            textures[0]=tex1;
            textures[1]=tex2;
            textures[2]=tex3;
            source = new Rectangle(0, 0, 32, 32);
            
        }

        public int vie 
        {
            get{return _vie;}
            set { _vie = value; }
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        public void setPosition(int x, int y)
        {
            this.x = x-16;
            this.y = y-16;
        }

        public void Draw(SpriteBatch sb)
        {
            Rectangle destination = new Rectangle(x, y, 32, 32);
            sb.Draw(textures[_vie-1], destination, source, Color.White);
        }
    }
}
