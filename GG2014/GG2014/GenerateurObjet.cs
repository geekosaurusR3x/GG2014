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
    class GenerateurObjet
    {
        Random rand;
        public GenerateurObjet()
        {
           rand = new Random();
        }

        public bool getEvent()
        { 
           return (rand.Next(1,6)%2)==0;
        }

        public int getCorde()
        {
            return rand.Next(0, 4);
        }

        public bool getObjet()
        { 
            if(rand.Next(0,11)<=5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int directionVent()
        {
            int dir = rand.Next(0, 11);
            if (dir > 5)
                return 1;
            else
                return -1;
        }
        
    }
}
