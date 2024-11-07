#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogameTutorial.source.engine;
using monogameTutorial.source.engine.input;
using monogameTutorial.source.world;
using monogameTutorial.source.world.projectiles;
using monogameTutorial.source.world.units;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
#endregion

namespace monogameTutorial {
    public class TutGame : Game {
        private GraphicsDeviceManager _graphics;

        private World world;
        //private FollowCamera camera; //allows for viewport centered player. Translates player movement to the map
        private TileLayer baseLayer; //base display layer for sandbox map
        private TileLayer collisionLayer = null; //collision layer for sandbox map

        private Texture2D rectangleTexture; //debug variable for rectHollow method

        public TutGame() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            Globals.viewport = _graphics.GraphicsDevice.Viewport;
            GameGlobals.tileSize = 32;
            GameGlobals.camera = new FollowCamera(Vector2.Zero); //load camera object

            base.Initialize();
        }

        protected override void LoadContent() {
            Globals.content = this.Content;
            Globals.spriteBatch = new SpriteBatch(GraphicsDevice);

            Globals.keyboard = new CevKeyboard();
            Globals.mouse = new CevMouseControl();

            // load map base layer
            baseLayer = new(
                "../../../data/tutorialmap..csv",
                Content.Load<Texture2D>("ground_tiles"),
                21
                );

            //load temp collision layer
            collisionLayer = new(
                "../../../data/tutorialmap._stone.csv",
                Content.Load<Texture2D>("ground_Tiles"),
                21
                );

            world = new World();

            //Debug stuff for DrawRectHollow()
            rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            rectangleTexture.SetData(new Color[] { new(255, 0, 0, 255) });
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Globals.keyboard.Update();
            Globals.mouse.Update();
            Globals.gameTime = gameTime;

            float[] playerVelocity = { 2f, 2f, 2f, 2f };
            //baseline player movement resets every update. Will change based on game mechanics. Best way currently is to refresh state every frame.

            //if (count % 30 == 0) {
            //    Debug.WriteLine(count);
            //}

            // simple timer logic
            //timer.UpdateTimer();
            //if(timer.Test()) {
            //    Debug.WriteLine("reset timer: " + timer.Timer);
            //    timer.ResetToZero();
            //}
            HandleCollisions(collisionLayer, playerVelocity, world.player); // method directly below for handling collision events
                                                                                              // player.Update(playerVelocity); //Update player according to class logic
            world.Update(playerVelocity);
            GameGlobals.camera.Follow(world.player.MapPosition, new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight)); //update camera offset with player movement

            Globals.keyboard.UpdateOld();
            Globals.mouse.UpdateOld();

            base.Update(gameTime);
        }

        //Define the collision point between rectangles and handle player movement. Needs optimization
        internal void HandleCollisions(TileLayer collisionLayer, float[] playerVelocity, Player player) {
            foreach (var item in collisionLayer.tileMap) {
                Vector2 position = item.Key;
                //Rect for current position of collision layer tiles
                Rectangle mapLocation = new(
                        (int)position.X * GameGlobals.tileSize + (int)GameGlobals.camera.Position.X,
                        (int)position.Y * GameGlobals.tileSize + (int)GameGlobals.camera.Position.Y,
                        GameGlobals.tileSize,
                        GameGlobals.tileSize
                );
                //Evaluates true when Player rect intersects a collision tile.
                if (mapLocation.Intersects(player.DRect)) {
                    //Evaluate state of player and collision rectangle relationship
                    int[] possibleIntersections = new int[] {
                        Math.Abs(mapLocation.Top - player.DRect.Top),
                        Math.Abs(mapLocation.Right - player.DRect.Right),
                        Math.Abs(mapLocation.Bottom - player.DRect.Bottom),
                        Math.Abs(mapLocation.Left - player.DRect.Left)
                        };
                    //Max value evaluates to the location of collision on rectangles
                    int maxValue = -1;
                    int maxIndex = -1;
                    for (int i = 0; i < possibleIntersections.Length; i++) {
                        if (possibleIntersections[i] > maxValue) {
                            maxValue = possibleIntersections[i];
                            maxIndex = i;

                        }
                    }
                    //Stop player movement against collision tile
                    switch (maxIndex) {
                        case 0:
                            playerVelocity[0] = 0;
                            //Debug.WriteLine("top");
                            break;
                        case 1:
                            playerVelocity[1] = 0;
                            //Debug.WriteLine("Right");
                            break;
                        case 2:
                            playerVelocity[2] = 0;
                            //Debug.WriteLine("Bottom");
                            break;
                        case 3:
                            playerVelocity[3] = 0;
                            //Debug.WriteLine("Left");
                            break;
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);
            Globals.spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            baseLayer.Draw(GameGlobals.camera.Position);
            collisionLayer.Draw(GameGlobals.camera.Position);
            world.Draw();

            //Debug method for collision tile hitbox
            foreach (var item in collisionLayer.tileMap) {
                Vector2 position = item.Key;
                Rectangle mapLocation = new(
                        (int)position.X * GameGlobals.tileSize + (int)GameGlobals.camera.Position.X,
                        (int)position.Y * GameGlobals.tileSize + (int)GameGlobals.camera.Position.Y,
                        GameGlobals.tileSize,
                        GameGlobals.tileSize
                );
                DrawRectHollow(Globals.spriteBatch, mapLocation, 4);
            }

            //Debug method for player hitbox. TODO: add method for defining player hitbox in Player class
            DrawRectHollow(Globals.spriteBatch, world.player.DRect, 4);

            Globals.spriteBatch.End();
            base.Draw(gameTime);
        }

        //Debug method for outlining rectangles
        public void DrawRectHollow(SpriteBatch spriteBatch, Rectangle rect, int thickness) {
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Y,
                    rect.Width,
                    thickness
                ),
                Color.White
            );
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Bottom - thickness,
                    rect.Width,
                    thickness
                ),
                Color.White
            );
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Y,
                    thickness,
                    rect.Height
                ),
                Color.White
            );
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.Right - thickness,
                    rect.Y,
                    thickness,
                    rect.Height
                ),
                Color.White
            );
        }
    }
}
