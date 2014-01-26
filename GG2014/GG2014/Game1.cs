using System;
using System.Diagnostics;
using System.Threading;
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
        bool touche_down;
        bool jump_touche_down;
        bool fall_touche_down;
        Texture2D tex_ennemy_leaf;
        int xi = 1380;
        Texture2D tex_background;
        Texture2D tex_ear;
        float elapsed_time_enemis;

        Vent vent;
        Note note;
        Ear ear;
        double EnemiTime;
        double TouchTime;
        double JumpTime;
        double EndTime;
        double FallTime;
        double tip_up_elapsed_time;

        double JumpAngle;
        int last_cord_id;
        
        bool Pause;
        GenerateurObjet mRandomProvider;
        double origSize;

        // How long before an ear gets displayed
        static float endTime = 2.0f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Init vars
            idNoteCorde = 1;
            touche_down = false;
            jump_touche_down = false;
            Pause = false;

            // Timers
            EndTime = 0;
            EnemiTime = 0;
            TouchTime = 0;
            JumpTime = 0;
            JumpAngle = 0;
            elapsed_time_enemis = 2.0f;
            tip_up_elapsed_time = 0;

            // Random generator
            mRandomProvider = new GenerateurObjet();
        }

        protected override void Initialize()
        {
            graphics.IsFullScreen = false;
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
            int w = GraphicsDevice.Viewport.Bounds.Width;
            int h = GraphicsDevice.Viewport.Bounds.Height;

            cordes[0] = new Corde((int)(w / 2), (int)(2*h / 3), w / 6, h - h / 16);
            cordes[1] = new Corde((int)(w / 2), (int)(2*h / 3), (int)(w / 2.5), h - h / 16);
            cordes[2] = new Corde((int)(w - (w / 2)), (int)(2 * h / 3), (int)(w - (w / 2.5)), h - h / 16);
            cordes[3] = new Corde((int)(w - (w / 2)), (int)(2*h / 3), w - (w / 6), h - h / 16);

            // Textures
            Texture2D tex1, tex2, tex3;
            tex1 = Content.Load<Texture2D>("noire-128");
            tex2 = Content.Load<Texture2D>("double-croche-128");
            tex3 = Content.Load<Texture2D>("triple-croche-128");
            tex_ennemy_leaf = Content.Load<Texture2D>("leaf-128");
            tex_background = Content.Load<Texture2D>("background");
            tex_ear = Content.Load<Texture2D>("ear-128");
            for (int i = 0; i <= 3; i++)
            {
                cordes[i].genBaseTexture(GraphicsDevice);
            }
            note = new Note(0, 0, tex1, tex2, tex3, 3);
            origSize = note.getSize();
            vent = new Vent(0, 0, Content.Load<Texture2D>("cloud"), -1);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void updateTimers(GameTime gameTime)
        {
            // Update timers
            double time = gameTime.ElapsedGameTime.TotalSeconds;
            EnemiTime += time;
            TouchTime += time;
            JumpTime += time;
            FallTime += time;
            EndTime += time;
            tip_up_elapsed_time += time;
        }

        protected override void Update(GameTime gameTime)
        {
            updateTimers(gameTime);

            vent.setPosition(xi, 0);
            xi--;

            // Choose a cord randomly
            int cordId = mRandomProvider.getCorde();

            // Is it time to display a new Enemy?
            if (EnemiTime > elapsed_time_enemis && !Pause)
            {
                EnemiTime -= elapsed_time_enemis;
                Enemis temp = new Enemis(cordes[cordId].getStart().X, cordes[cordId].getStart().Y, cordes[cordId].getVectorDir());
                temp.setSize(16);
                temp.setTexture(tex_ennemy_leaf);
                ListObject.Add(temp);
            }

            if (tip_up_elapsed_time > 10.0f && !Pause)
            {
                tip_up_elapsed_time -= 10.0f;
                elapsed_time_enemis -= 0.1f;
            }
            if (TouchTime > 0.01f && !Pause)
            {
                TouchTime -= 0.01f;
                touche_down = false;
            }

            if (FallTime > 0.5f && !Pause)
            {
                FallTime -= 0.5f;
                fall_touche_down = false;
            }

            // Is it time to display the end? (ear)
            if (EndTime > endTime && ear == null)
            {
                ear = new Ear(cordes[cordId].getStart().X, cordes[cordId].getStart().Y, cordes[cordId].getVectorDir());
                ear.setTexture(tex_ear);
            }

            // Update enemies & ear
             if (!Pause) {
                 updateEnnemies();
                 updateEar();
            }

            // Keyboard functions
            KeyboardState kState = Keyboard.GetState();

            // GTFO
            if (kState.IsKeyDown(Keys.Q) || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                System.Environment.Exit(0);
            }

            if (kState.IsKeyDown(Keys.Left) && !touche_down)
            {
                note.decreaseAngle();
                touche_down = true;
            }

            if (kState.IsKeyDown(Keys.Right) && !touche_down)
            {
                note.increaseAngle();
                touche_down = true;
            }

            // Keyboard functions
            if (kState.IsKeyDown(Keys.RightControl) && kState.IsKeyDown(Keys.OemOpenBrackets))
            {
                // CHEAT
                note.cheetah();
            }

            double angle = note.getAngle();
            if (kState.IsKeyDown(Keys.Space) && !jump_touche_down)
            {
                last_cord_id = idNoteCorde;
                if (angle <= MathHelper.PiOver2-0.2)
                {
                    idNoteCorde--;
                    JumpAngle = MathHelper.PiOver2;
                    jump_touche_down = true;
                }
                else if (angle >= MathHelper.PiOver2 + 0.2)
                {
                    idNoteCorde++;
                    JumpAngle = 3*MathHelper.PiOver2;
                    jump_touche_down = true;
                }
            }

            
            if (jump_touche_down && !Pause)
            {
                jump();
            }

            /*
            if (idNoteCorde < 0 || idNoteCorde > 3)
            {
                System.Console.WriteLine("GAME OVER (" + idNoteCorde + ")");
                System.Environment.Exit(0);
            }
             * */

            // Check angle 
            if (angle > MathHelper.Pi && !fall_touche_down && !jump_touche_down)
            {
                if (checkForDeath())
                {
                    // Readjust angle
                    note.resetAngle();
                }
                fall_touche_down = true;
            }

            if (!jump_touche_down)
            {
                note.setPosition(cordes[idNoteCorde].getEnd().X, cordes[idNoteCorde].getEnd().Y);
            }

            base.Update(gameTime);
        }

        private void updateEar()
        {
            // Update ear
            if (ear != null)
            {
                ear.setPosition((ear.getPos().X + (ear.getDir().X / 1)), (ear.getPos().Y + (ear.getDir().Y / 1)));

                // Collision check
                if (ear.getPos().X >= ear.getPos().X - 20 && ear.getPos().X <= note.getPos().X + 20 && ear.getPos().Y > note.getPos().Y && !jump_touche_down)
                {
                    Pause = true;
                    epicWin();
                }
            }
        }

        private void updateEnnemies()
        {
            for (int i = 0; i < ListObject.Count - 1; i++)
            {
                Enemis temp2 = ListObject[i];
                if (temp2.getPos().Y > graphics.PreferredBackBufferWidth)
                {
                    ListObject.Remove(temp2);
                }
                else
                {
                    ListObject[i].setPosition((temp2.getPos().X + (temp2.getDir().X * 3)), (temp2.getPos().Y + (temp2.getDir().Y * 3)));
                    ListObject[i].setSize(temp2.getSize() + 0.5f);
                }
                if (temp2.getPos().X >= note.getPos().X - 20 && temp2.getPos().X <= note.getPos().X + 20 && temp2.getPos().Y > note.getPos().Y && !jump_touche_down)
                {
                    ListObject.Remove(temp2);
                    checkForDeath();
                }
            }
        }

        private void jump()
        {
            if (idNoteCorde > 3) // Suicide (right)
            {
                int r = (int)(cordes[last_cord_id].getEnd().X + 120 - cordes[last_cord_id].getEnd().X) / 2;
                int x = (int)cordes[last_cord_id].getEnd().X + 120 - r;
                int y = (int)cordes[last_cord_id].getEnd().Y;
                float nx = (float)(x + r * Math.Sin(JumpAngle));
                float ny = (float)(y + r * Math.Cos(JumpAngle));
                note.setPosition(nx, ny);
                JumpAngle -= 0.05f;
                note.increaseAngle();
                note.setSize(note.getSize() - 1.0);
                if (JumpAngle <= MathHelper.PiOver2)
                {
                    checkForDeath();
                    note.setSize(origSize);
                    note.resetAngle();
                    idNoteCorde = 1;
                    jump_touche_down = false;
                }
            }
            else if (idNoteCorde < 0) // Suicide (left)
            {
                int r = (int)(cordes[0].getEnd().X - 120) / 2;
                int x = (int)cordes[0].getEnd().X - 100 - r;
                int y = (int)cordes[0].getEnd().Y;
                float nx = (float)(x + r * Math.Sin(JumpAngle));
                float ny = (float)(y + r * Math.Cos(JumpAngle));
                note.setPosition(nx, ny);
                JumpAngle += 0.05f;
                note.decreaseAngle();
                note.setSize(note.getSize() - 1.0);
                if (JumpAngle >= 3 * MathHelper.PiOver2)
                {
                    checkForDeath();
                    note.setSize(origSize);
                    note.resetAngle();
                    idNoteCorde = 1;
                    jump_touche_down = false;
                }
            }
            else if (idNoteCorde > last_cord_id)
            {
                int r = (int)(cordes[idNoteCorde].getEnd().X - cordes[last_cord_id].getEnd().X) / 2;
                int x = (int)cordes[idNoteCorde].getEnd().X - r;
                int y = (int)cordes[idNoteCorde].getEnd().Y;
                float nx = (float)(x + r * Math.Sin(JumpAngle));
                float ny = (float)(y + r * Math.Cos(JumpAngle));
                note.setPosition(nx, ny);
                JumpAngle -= 0.05f;
                note.increaseAngle();
                if (JumpAngle <= MathHelper.PiOver2)
                {
                    note.resetAngle();
                    jump_touche_down = false;
                }
            }
            else
            {
                int r = (int)(cordes[last_cord_id].getEnd().X - cordes[idNoteCorde].getEnd().X) / 2;
                int x = (int)cordes[last_cord_id].getEnd().X - r;
                int y = (int)cordes[last_cord_id].getEnd().Y;
                float nx = (float)(x + r * Math.Sin(JumpAngle));
                float ny = (float)(y + r * Math.Cos(JumpAngle));
                note.setPosition(nx, ny);
                JumpAngle += 0.05f;
                note.decreaseAngle();
                if (JumpAngle >= 3 * MathHelper.PiOver2)
                {
                    note.resetAngle();
                    jump_touche_down = false;
                }
            }
        }

        private void epicWin()
        {
            // TODO
            System.Console.WriteLine("You made it!");
        }

        private bool checkForDeath()
        {
            if (note.getLivesLeft() > 1)
            {
                note.kill();
                return true;
            }
            else
            {
                Pause = true;
                System.Console.WriteLine("You got screwed");
            }
            return false;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            // Background
            Rectangle backgroundRectangle = new Rectangle(0, 0, GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height);
            spriteBatch.Draw(tex_background, backgroundRectangle, Color.White);

            for (int i = 0; i <= 3; i++)
            {
                cordes[i].Draw(spriteBatch);
            }

            for (int i = 0; i < ListObject.Count - 1; i++)
            {
                ListObject[i].Draw(spriteBatch);
            }

            if (ear != null)
            {
                ear.Draw(spriteBatch);
            }
            if (!Pause)
            {
                note.Draw(spriteBatch);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
