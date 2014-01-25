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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int idNoteCorde;
        Corde[] cordes;
        List<Enemis> ListObject;

        Note note;
        double AnimationTime;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;

            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            ListObject = new List<Enemis>();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            cordes = new Corde[4];
            cordes[0]=new Corde(350, 300, 100, 600);
            cordes[1]=new Corde(385, 300, 300, 600);
            cordes[2]=new Corde(415, 300, 500, 600);
            cordes[3]=new Corde(450, 300, 700, 600);
            
            for (int i = 0; i <= 3; i++)
            {
                cordes[i].genBaseTexture(GraphicsDevice);
            }
 
            AnimationTime = 0;

            Texture2D tex1, tex2, tex3;
            tex1 = Content.Load<Texture2D>("noire-32");
            tex2 = Content.Load<Texture2D>("double-croche-32");
            tex3 = Content.Load<Texture2D>("triple-croche-32");
            note = new Note(0, 0, tex1, tex2, tex3,3);
            idNoteCorde = 1;
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            double time = gameTime.ElapsedGameTime.TotalSeconds;
            AnimationTime += time;
            if (AnimationTime > 2.0f)
            {
                AnimationTime -= 2.0f;
                Enemis temp = new Enemis(cordes[0].getStart().X - 10, cordes[0].getStart().Y - 10, cordes[0].getVectorDir());
                temp.genBaseTexture(GraphicsDevice);
                ListObject.Add(temp);
            }

            for (int i = 0; i< ListObject.Count-1; i++)
            {
                Enemis temp2 = ListObject[i];
                if (temp2.getPos().Y > 600)
                {
                    ListObject.Remove(temp2);
                }
                else
                {
                    Enemis temp = new Enemis((temp2.getPos().X + (temp2.getDir().X / 5)), (temp2.getPos().Y + (temp2.getDir().Y / 5)), temp2.getDir());
                    temp.genBaseTexture(GraphicsDevice);
                    temp.setSize(temp2.getSize()+0.1);
                    ListObject[i] = temp;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                if (idNoteCorde == 0)
                {
                    //game over
                }
                else
                {
                    idNoteCorde--;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                if (idNoteCorde == 3)
                {
                    //game over
                }
                else 
                {
                    idNoteCorde++;
                }
            }
            note.setPosition((int)cordes[idNoteCorde].getEnd().X, (int)cordes[idNoteCorde].getEnd().Y);
                    
      
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            note.Draw(spriteBatch);

            for (int i = 0; i <= 3; i++)
            {
                cordes[i].Draw(spriteBatch);
            }

            for (int i = 0; i < ListObject.Count - 1; i++)
            {
                ListObject[i].Draw(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
