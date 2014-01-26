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
        enum GameState { GAME_MENU = 0, GAME_OVER, GAME_WIN, GAME_PLAYING, GAME_PAUSED };
        GameState Etat_game;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int idNoteCorde;
        Corde[] cordes;
        Song[] Musiques;
        List<Enemis> ListObject;
        bool touche_down;
        bool jump_touche_down;
        bool fall_touche_down;
        Texture2D tex_ennemy_leaf;
        int xi = 1380;
        Texture2D tex_background;
        Texture2D tex_ear;
        float elapsed_time_enemis;
        SpriteFont Font;
        SpriteFont FontGame;
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

        SoundEffect jumpSound;
        SoundEffect swearingSound;

        // How long before an ear gets displayed
        static float endTime = 30.0f;
        // String that is displayed when play fails ("score")
        string remainingTime;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Init vars
            idNoteCorde = 1;
            touche_down = false;
            jump_touche_down = false;
            Pause = false;
            Etat_game = GameState.GAME_PLAYING;

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
            graphics.PreferredBackBufferWidth = 1380;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            ListObject = new List<Enemis>();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            cordes = new Corde[4];
            Musiques = new Song[3];

            int w = GraphicsDevice.Viewport.Bounds.Width;
            int h = GraphicsDevice.Viewport.Bounds.Height;

            cordes[0] = new Corde((int)(w / 2), (int)(2*h / 3), w / 6, h - h / 16);
            cordes[1] = new Corde((int)(w / 2), (int)(2*h / 3), (int)(w / 2.5), h - h / 16);
            cordes[2] = new Corde((int)(w - (w / 2)), (int)(2 * h / 3), (int)(w - (w / 2.5)), h - h / 16);
            cordes[3] = new Corde((int)(w - (w / 2)), (int)(2*h / 3), w - (w / 6), h - h / 16);

            // Textures
            Texture2D tex1, tex2, tex3;
            tex1 = Content.Load<Texture2D>("noire-angel-128");
            tex2 = Content.Load<Texture2D>("double-croche-angel-128");
            tex3 = Content.Load<Texture2D>("triple-croche-angel-128");
            tex_ennemy_leaf = Content.Load<Texture2D>("leaf-128");
            tex_background = Content.Load<Texture2D>("background");
            tex_ear = Content.Load<Texture2D>("ear-128");

            Musiques[0] = Content.Load<Song>("Mozart - March in D major K.189");
            Musiques[1] = Content.Load<Song>("Mozart - March in D major K.215");
            Musiques[2] = Content.Load<Song>("Mozart - March in D major K.237");

            
            for (int i = 0; i <= 3; i++)
            {
                cordes[i].genBaseTexture(GraphicsDevice);
            }
            note = new Note(0, 0, tex1, tex2, tex3, 3);
            origSize = note.getSize();
            vent = new Vent(0, 0, Content.Load<Texture2D>("cloud"), -1);

            // Fonts
            Font = Content.Load<SpriteFont>("font");
            FontGame = Content.Load<SpriteFont>("fontGame");

            Random rand = new Random();
            int id_musiques = rand.Next(0,3);
            MediaPlayer.Play(Musiques[id_musiques]);
            // Audio
            jumpSound = Content.Load<SoundEffect>("wootcha");
            swearingSound = Content.Load<SoundEffect>("putain");
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
            switch (Etat_game)
            {
                case GameState.GAME_MENU:
                    //DrawMenu(gameTime);
                    break;
                case GameState.GAME_PLAYING:
                    UpdateGame(gameTime);
                    break;
                case GameState.GAME_OVER:
                    UpdateGameOver(gameTime);
                    break;
                case GameState.GAME_WIN:
                    UpdateGameWin(gameTime);
                    break;
            }
            base.Update(gameTime);
        }

        protected void UpdateGame(GameTime gameTime)
        {
            updateTimers(gameTime);

            vent.setPosition(xi, 0);
            xi--;

            // Choose a cord randomly
            int cordId = mRandomProvider.getCorde();

            // Update remaining time string in case we die
            remainingTime = ((int)(endTime - gameTime.TotalGameTime.TotalSeconds)).ToString() + " seconds left";

            // Is it time to display a new Enemy?
            if (EnemiTime > elapsed_time_enemis && !Pause)
            {
                EnemiTime -= elapsed_time_enemis;

                Enemis temp = new Enemis(0, 0, cordes[cordId].getVectorDir());
                temp.setSize(16);
                temp.setTexture(tex_ennemy_leaf);
                temp.setAnim(true);
                temp.setP1(new Vector2(0, 0));
                temp.setP2(cordes[cordId].getStart());
                temp.setCorde(cordes[cordId].getStart());
                temp.setJumpAngle(3*MathHelper.PiOver2);

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

            if (kState.IsKeyDown(Keys.RightControl) && kState.IsKeyDown(Keys.OemOpenBrackets))
            {
                // CHEAT
                note.cheetah();
            }

            double angle = note.getAngle();

            if (kState.IsKeyDown(Keys.Space) && !jump_touche_down)
            {
                last_cord_id = idNoteCorde;
                if (angle <= MathHelper.PiOver2 - 0.2)
                {
                    idNoteCorde--;
                    JumpAngle = MathHelper.PiOver2;
                    jump_touche_down = true;
                    jumpSound.Play();
                }
                else if (angle >= MathHelper.PiOver2 + 0.2)
                {
                    idNoteCorde++;
                    JumpAngle = 3 * MathHelper.PiOver2;
                    jump_touche_down = true;
                    jumpSound.Play();
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

        }

        private void updateEar()
        {
            // Update ear
            if (ear != null)
            {
                ear.setPosition((ear.getPos().X + (ear.getDir().X / 1)), (ear.getPos().Y + (ear.getDir().Y / 1)));
                ear.setSize(ear.getSize() + 0.5f);
                // Collision check
                if (ear.getPos().X >= note.getPos().X - 20 && ear.getPos().X <= note.getPos().X + 20 && ear.getPos().Y > note.getPos().Y && !jump_touche_down)
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
                    if(ListObject[i].isAnim())
                    {
                        int r = (int)(ListObject[i].getP2().X - ListObject[i].getP1().X) / 2;
                        int x = (int)ListObject[i].getP2().X - r;
                        int y = (int)ListObject[i].getP2().Y;
                        float nx = (float)(x + r * Math.Sin(ListObject[i].getJumpAngle()));
                        float ny = (float)(y + r * Math.Cos(ListObject[i].getJumpAngle()));
                        ListObject[i].setPosition(nx, ny);
                        ListObject[i].setJumpAngle(ListObject[i].getJumpAngle() - 0.05f);
                        if (ListObject[i].getJumpAngle() <= MathHelper.PiOver2)
                        {
                            ListObject[i].setAnim(false);
                            ListObject[i].setPosition(ListObject[i].getCorde().X, ListObject[i].getCorde().Y);
                        }
                    }
                    else
                    {
                        ListObject[i].setPosition((temp2.getPos().X + (temp2.getDir().X * 3)), (temp2.getPos().Y + (temp2.getDir().Y * 3)));
                        ListObject[i].setSize(temp2.getSize() + 0.5f);
                    }
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
            Etat_game = GameState.GAME_WIN;
        }

        private void restartGame()
        {
            note.reset();
            ListObject.Clear();
            ear = null;
            EnemiTime = TouchTime = JumpTime = FallTime = EndTime = tip_up_elapsed_time = 0;
            touche_down = jump_touche_down = fall_touche_down = false;
            idNoteCorde = 1;
            Etat_game = GameState.GAME_PLAYING;
            Pause = false;
        }

        private void gameOver()
        {
            Pause = true;
            Etat_game = GameState.GAME_OVER;
            MediaPlayer.Stop();
            // Delete remaining ennemies
            ListObject.Clear();
        }

        private bool checkForDeath()
        {
            swearingSound.Play();
            if (note.getLivesLeft() > 1)
            {
                note.kill();
                return true;
            }
            else
            {
                gameOver();
            }
            return false;
        }

        protected void UpdateGameWin(GameTime gameTime)
        {
            KeyboardState kState = Keyboard.GetState();

            // GTFO
            if (kState.IsKeyDown(Keys.Q) || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                System.Environment.Exit(0);
            }

            // Restart
            if (kState.IsKeyDown(Keys.R))
            {
                restartGame();
            }
        }

        protected void UpdateGameOver(GameTime gameTime)
        {
            // Keyboard functions
            KeyboardState kState = Keyboard.GetState();
            Random rand = new Random();
            int x = rand.Next(0, GraphicsDevice.Viewport.Bounds.Width);
            int y = -100;

            Enemis temp = new Enemis(x, y, new Vector2(0,1));
            temp.setSize(64);
            temp.setTexture(tex_ennemy_leaf);
            ListObject.Add(temp);

            for (int i = 0; i < ListObject.Count; i++)
            {
                Enemis leaf = ListObject[i];
                if (leaf.getPos().Y > GraphicsDevice.Viewport.Bounds.Height + leaf.getPos().Y - ((int)leaf.getSize() / 2))
                {
                    ListObject.Remove(leaf);
                }
                else
                {
                    leaf.setPosition((leaf.getPos().X + (leaf.getDir().X * 3)), (leaf.getPos().Y + (leaf.getDir().Y * 3)));
                }
            }

            // GTFO
            if (kState.IsKeyDown(Keys.Q) || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                System.Environment.Exit(0);
            }

            // Restart
            if (kState.IsKeyDown(Keys.R))
            {
                restartGame();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            switch(Etat_game)
            {
                case GameState.GAME_MENU:
                    DrawMenu(gameTime);
                    break;
                case GameState.GAME_PLAYING:
                    DrawLevel(gameTime);
                    break;
                case GameState.GAME_OVER:
                    DrawEnd(gameTime);
                    break;
                case GameState.GAME_WIN:
                    DrawWin(gameTime);
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected void DrawMenu(GameTime gameTime)
        {
            // TODO
        }

        protected void DrawLevel(GameTime gameTime)
        {
            int w = GraphicsDevice.Viewport.Bounds.Width;
            int h = GraphicsDevice.Viewport.Bounds.Height;

            // Background
            Rectangle backgroundRectangle = new Rectangle(0, 0, GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height);
            spriteBatch.Draw(tex_background, backgroundRectangle, Color.White);

            // "Score"
            /* if (ear == null)
            {
                
                string timeTillEnd = ((int)(endTime - gameTime.TotalGameTime.TotalSeconds)).ToString();
                Vector2 size = FontGame.MeasureString(timeTillEnd);
                Vector2 pos = new Vector2((w / 2) - (size.X / 2), 20);
                spriteBatch.DrawString(FontGame, timeTillEnd, pos, Color.BlanchedAlmond);
            } */

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
            note.Draw(spriteBatch);
        }

        protected void DrawWin(GameTime gameTime)
        {
            int w = GraphicsDevice.Viewport.Bounds.Width;
            int h = GraphicsDevice.Viewport.Bounds.Height;

            Rectangle backgroundRectangle = new Rectangle(0, 0, w, h);
            spriteBatch.Draw(tex_background, backgroundRectangle, Color.White);

            // TODO Show score or something?
            string epicWin = string.Format("EPIC WIN");
            Vector2 size = FontGame.MeasureString(epicWin);
           
            Vector2 pos = new Vector2((w / 2) - (size.X / 2), (h / 2) - (size.Y / 2));
            spriteBatch.DrawString(FontGame, epicWin, pos, Color.BlanchedAlmond);
        }

        

        protected void DrawEnd(GameTime gameTime)
        {
            int w = GraphicsDevice.Viewport.Bounds.Width;
            int h = GraphicsDevice.Viewport.Bounds.Height;

            Rectangle backgroundRectangle = new Rectangle(0, 0, w, h);
            spriteBatch.Draw(tex_background, backgroundRectangle, Color.White);


            for (int i = 0; i < ListObject.Count; i++)
            {
                ListObject[i].Draw(spriteBatch);
            }
            string gameover = string.Format("GAME OVER");

            Vector2 size = FontGame.MeasureString(gameover);
            Vector2 pos = new Vector2((w / 2) - (size.X / 2), (h / 2) - (size.Y / 2));

            spriteBatch.DrawString(FontGame, gameover, pos, Color.BlanchedAlmond);

            size = Font.MeasureString(remainingTime);
            pos = new Vector2((w / 2) - (size.X / 2), (h / 2) - (size.Y / 2) + 80);

            spriteBatch.DrawString(Font, remainingTime, pos, Color.BlanchedAlmond);
        }
    }
}
