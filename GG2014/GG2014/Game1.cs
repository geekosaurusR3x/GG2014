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
        Texture2D TextureCorde;
        int idNoteCorde;
        Corde[] cordes;
        //Corde C1;
        //Corde C2;
        //Corde C3;
        //Corde C4;
        List<Object> ListObject;

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
            graphics.PreferredBackBufferHeight = 650;

            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            ListObject = new List<Object>();
            TextureCorde = new Texture2D(GraphicsDevice, 1, 1);
            TextureCorde.SetData<Color>(new Color[] { Color.White });
            spriteBatch = new SpriteBatch(GraphicsDevice);
            cordes = new Corde[4];
            cordes[0]=new Corde(350, 300, 100, 600);
            cordes[1]=new Corde(385, 300, 300, 600);
            cordes[2]=new Corde(415, 300, 500, 600);
            cordes[3]=new Corde(450, 300, 700, 600);
             
            AnimationTime = 0;

            Texture2D tex1, tex2, tex3;
            tex1 = Content.Load<Texture2D>("noire-32");
            tex2 = Content.Load<Texture2D>("double-croche-32");
            tex3 = Content.Load<Texture2D>("triple-croche-32");
            note = new Note(0, 0, tex1, tex2, tex3,3);
            note.setPosition((int)cordes[0].getEnd().X, (int)cordes[0].getEnd().Y);
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
                ListObject.Add(new Object(cordes[0].getStart().X - 10, cordes[0].getStart().Y - 10, cordes[0].getVectorDir()));
                System.Console.WriteLine("prpout");
            }

            for (int i = 0; i< ListObject.Count-1; i++)
            {
                Object temp2 = ListObject[i];
                if (temp2.getPos().Y > 600)
                {
                    ListObject.Remove(temp2);
                }
                else
                {
                    Object temp =  new Object((temp2.getPos().X + (temp2.getDir().X/5)), (temp2.getPos().Y + (temp2.getDir().Y/5)), temp2.getDir());
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
                    
            note.setPosition((int)cordes[idNoteCorde].getEnd().X, (int) cordes[idNoteCorde].getEnd().Y);
           
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
                    note.setPosition((int)cordes[idNoteCorde].getEnd().X, (int)cordes[idNoteCorde].getEnd().Y);
                }
            }
            System.Console.WriteLine((int)cordes[idNoteCorde].getEnd().Y);
                    
      
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            note.Draw(spriteBatch);
            DrawLine(spriteBatch, cordes[0].getStart(), cordes[0].getEnd());
            DrawLine(spriteBatch, cordes[1].getStart(), cordes[1].getEnd());
            DrawLine(spriteBatch, cordes[2].getStart(), cordes[2].getEnd());
            DrawLine(spriteBatch, cordes[3].getStart(), cordes[3].getEnd());

            for (int i = 0; i < ListObject.Count - 1; i++)
            {
                ;
                Texture2D dummyTexture = new Texture2D(GraphicsDevice, 1, 1);
                dummyTexture.SetData(new Color[] { Color.White });
                Object temp2 = ListObject[i];
                Rectangle temp = new Rectangle((int)(temp2.getPos().X),((int)temp2.getPos().Y),(int)temp2.getSize(),(int)temp2.getSize());
                spriteBatch.Draw(dummyTexture, temp, Color.Black);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle = (float)Math.Atan2(edge.Y, edge.X);


            sb.Draw(TextureCorde,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    1), //width of line, change this to make thicker line
                null,
                Color.White, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }
    }
}
