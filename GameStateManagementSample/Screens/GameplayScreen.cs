#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;
using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;
#endregion

namespace GameStateManagementSample
{
        
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        Random random = new Random();

        float pauseAlpha;

        InputAction pauseAction;

        #endregion
        //GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int idNoteCorde;
        Corde[] cordes;
        List<Enemis> ListObject;
        bool touche_down;
        bool jump_touche_down;
        bool fall_touche_down;
        Texture2D tex_ennemy_leaf;
        int xi = 1380;
        bool nuages = false;
        int x0nuage, xMaxNuage;
        int skyTime;
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
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);
            
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");
               
                //graphics = new GraphicsDeviceManager(ScreenManager.Game);
                gameFont = content.Load<SpriteFont>("gamefont");
                ListObject = new List<Enemis>();
                spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);
                x0nuage = 0;
                xMaxNuage = ScreenManager.GraphicsDevice.Viewport.Bounds.Width;

                cordes = new Corde[4];
                int w = ScreenManager.GraphicsDevice.Viewport.Bounds.Width;
                int h = ScreenManager.GraphicsDevice.Viewport.Bounds.Height;
                
                //graphics.IsFullScreen = false;
                //graphics.PreferredBackBufferWidth = 800;
                //ScreenManager.dev = 600;
                //graphics.ApplyChanges();

                cordes[0] = new Corde((int)(w / 2), (int)(2 * h / 3), w / 6, h - h / 16);
                cordes[1] = new Corde((int)(w / 2), (int)(2 * h / 3), (int)(w / 2.5), h - h / 16);
                cordes[2] = new Corde((int)(w - (w / 2)), (int)(2 * h / 3), (int)(w - (w / 2.5)), h - h / 16);
                cordes[3] = new Corde((int)(w - (w / 2)), (int)(2 * h / 3), w - (w / 6), h - h / 16);

                // Timers
                EndTime = 0;
                EnemiTime = 0;
                TouchTime = 0;
                JumpTime = 0;
                skyTime = 0;
                JumpAngle = 0;
                elapsed_time_enemis = 2.0f;
                tip_up_elapsed_time = 0;
                mRandomProvider = new GenerateurObjet();

                // Textures
                Texture2D tex1, tex2, tex3;
                tex1 = content.Load<Texture2D>("noire-128");
                tex2 = content.Load<Texture2D>("double-croche-128");
                tex3 = content.Load<Texture2D>("triple-croche-128");
                tex_ennemy_leaf = content.Load<Texture2D>("leaf-128");
                tex_background = content.Load<Texture2D>("background");
                tex_ear = content.Load<Texture2D>("ear-128");
                for (int i = 0; i <= 3; i++)
                {
                    cordes[i].genBaseTexture(ScreenManager.GraphicsDevice);
                }
                note = new Note(0, 0, tex1, tex2, tex3, 3);
                idNoteCorde = 1;

                vent = new Vent(0, 0, content.Load<Texture2D>("cloud"), -1);

                // Init
                touche_down = false;
                jump_touche_down = false;
                Pause = false;

                // A real game would probably have more content than this sample, so
                // it would take longer to load. We simulate that by delaying for a
                // while, giving you a chance to admire the beautiful loading screen.
                Thread.Sleep(1000);

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }

#if WINDOWS_PHONE
            if (Microsoft.Phone.Shell.PhoneApplicationService.Current.State.ContainsKey("PlayerPosition"))
            {
                playerPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"];
                enemyPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"];
            }
#endif
        }


        public override void Deactivate()
        {
#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"] = playerPosition;
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"] = enemyPosition;
#endif

            base.Deactivate();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            content.Unload();

#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("PlayerPosition");
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("EnemyPosition");
#endif
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                // Apply some random jitter to make the enemy move around.
                const float randomization = 10;

                enemyPosition.X += (float)(random.NextDouble() - 0.5) * randomization;
                enemyPosition.Y += (float)(random.NextDouble() - 0.5) * randomization;

                // Apply a stabilizing force to stop the enemy moving off the screen.
                Vector2 targetPosition = new Vector2(
                    ScreenManager.GraphicsDevice.Viewport.Width / 2 - gameFont.MeasureString("Insert Gameplay Here").X / 2, 
                    200);

                enemyPosition = Vector2.Lerp(enemyPosition, targetPosition, 0.05f);

                // TODO: this game isn't very fun! You could probably improve
                // it by inserting something more interesting in this space :-)
        #region /*** Debut update importé**/
                if (nuages)
                {
                    if (vent.Direction == -1)
                    {
                        if (vent.getfirstPosition() < 0)
                        {
                            xMaxNuage = ScreenManager.GraphicsDevice.Viewport.Width;
                            nuages = false;
                        }
                        else
                        {
                            xMaxNuage--;
                            vent.setPosition(xMaxNuage, 0);
                            //if(note.getAngle()<2)
                            //{note.decreaseAngle();}

                        }

                    }
                    else
                    {
                        if (vent.getLastPosition() > ScreenManager.GraphicsDevice.Viewport.Width)
                        {
                            x0nuage = 0;
                            nuages = false;
                        }
                        else
                        {
                            x0nuage++;
                            vent.setPosition(x0nuage, 0);
                            //if (note.getAngle()<-2)
                            //{ note.increaseAngle(); }
                        }

                    }
                }
                else
                {
                    skyTime = gameTime.TotalGameTime.Seconds;
                    if ((skyTime % 20 == 0) && (mRandomProvider.getEvent()))
                    {
                        int dir = mRandomProvider.directionVent();
                        if (dir == 1)
                            vent = new Vent(0, 0, content.Load<Texture2D>("cloud"), dir);
                        else
                            vent = new Vent(ScreenManager.GraphicsDevice.Viewport.Width, 0, content.Load<Texture2D>("cloud"), dir);
                        nuages = true;
                    }
                }

                // Update timers
                double time = gameTime.ElapsedGameTime.TotalSeconds;
                EnemiTime += time;
                TouchTime += time;
                JumpTime += time;
                FallTime += time;
                EndTime += time;
                tip_up_elapsed_time += time;

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
                if (EndTime > 3.0f && ear == null)
                {
                    ear = new Ear(cordes[cordId].getStart().X, cordes[cordId].getStart().Y, cordes[cordId].getVectorDir());
                    ear.setTexture(tex_ear);
                }

                // Update enemies
                if (!Pause)
                {
                    for (int i = 0; i < ListObject.Count - 1; i++)
                    {
                        Enemis temp2 = ListObject[i];
                        if (temp2.getPos().Y > ScreenManager.GraphicsDevice.Viewport.Width)
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
        #endregion /**fin update importé**/

                // Update ear
                if (ear != null)
                {
                    ear.setPosition((ear.getPos().X + (ear.getDir().X / 1)), (ear.getPos().Y + (ear.getDir().Y / 1)));
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
                Keys[] currentPressedKeys = kState.GetPressedKeys();
                // CHEAT
                if (kState.IsKeyDown(Keys.RightControl) && kState.IsKeyDown(Keys.OemOpenBrackets))
                {
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
                    }
                    else if (angle >= MathHelper.PiOver2 + 0.2)
                    {
                        idNoteCorde++;
                        JumpAngle = 3 * MathHelper.PiOver2;
                        jump_touche_down = true;
                    }

                }

                if (jump_touche_down && !Pause)
                {
                    if (idNoteCorde > last_cord_id)
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

                // Check angle 
                if (note.getAngle() > MathHelper.Pi && !fall_touche_down && !jump_touche_down)
                {
                    if (checkForDeath())
                    {
                        // Readjust angle
                        note.resetAngle();
                    }
                    fall_touche_down = true;
                }

                if (idNoteCorde < 0 || idNoteCorde > 3)
                {
                    System.Console.WriteLine("GAME OVER (" + idNoteCorde + ")");
                    System.Environment.Exit(0);
                }
                if (!jump_touche_down)
                {
                    note.setPosition(cordes[idNoteCorde].getEnd().X, cordes[idNoteCorde].getEnd().Y);
                }
            }
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

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
#if WINDOWS_PHONE
                ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer);
#else
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
#endif
            }
            else
            {
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                if (input.TouchState.Count > 0)
                {
                    Vector2 touchPosition = input.TouchState[0].Position;
                    Vector2 direction = touchPosition - playerPosition;
                    direction.Normalize();
                    movement += direction;
                }

                if (movement.Length() > 1)
                    movement.Normalize();

                playerPosition += movement * 8f;
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.DrawString(gameFont, "// TODO", playerPosition, Color.Green);

            spriteBatch.DrawString(gameFont, "Insert Gameplay Here",
                                   enemyPosition, Color.DarkRed);

            spriteBatch.End();

        #region /**Debut Draw importé**/
            spriteBatch.Begin();

            // Background
            Rectangle backgroundRectangle = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Bounds.Width, ScreenManager.GraphicsDevice.Viewport.Bounds.Height);
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
            if (nuages)
            {
                vent.Draw(spriteBatch);
            }
            spriteBatch.End();
    #endregion /*** fin Draw importé ***/

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }


        #endregion
    }
}
