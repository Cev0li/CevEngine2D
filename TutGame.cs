using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

/*
    10/13/24 CEV- init project. Wrote Sprite, modSprite, Tile Layer class and methods. Need to optomize TileLayer.draw
    
    10/14/24 CEV- Camera working, need to fix hardcoded draw values for modSprite class and make this class into a new player class.
        Sprite class needs to be what modSprite is now. Still need to optomize TileLayer.draw
    
    10/18/24 CEV- Attempted to get tile map collisions working. Unsuccessful. Need to rework the modSprite class: 
        1. needs to be renamed player class.
        2. destination rect needs to be changed to a static rectangle positioned at center of screen.
        3. movement needs to be realted to camera offsets
        Collision logic needs to check collision layers against the center of screen less than half width and height of player in each direction
    
    10/19/24 CEV- Renamed modSprite to Player. Need to revisit the role of Sprite class and its relationship to Player and other sprites in game.
        will do this when I add enemies. Player destination rect is not separate from variables that move the camera. Specifically,
        Player now contains 2 Vector2s: One is a position that will place the player at a specified area of the map, the second is
        a velocity on X and Y axis that will effect the position of player on map. Position vector is fed to the camera and the
        tile map layers to effectively "move" the player around the map. Collision is detected but need to be handled. Added viewport
        object to facilitate cleaner code when positioning sprites in relaionship to center of window. This also
        scales the positioning of game drawing to any viewport dimension.
    10/20/24 CEV- Collision logic is rudimentary, but the player sprite stops when it collides with tiles on collision layer. Need to write
                    velocity as two vector2s each dealing with one axis and the two directions it can move. This needs to be done to fix a
                    bug causing the camera to continue moving when plyer collides. In this case, the player position Vector should not update in 1 
                    direction. Currently the position Vector in Player can only handle 2 movements: X and Y axis generally.
    10/24/24 CEV- Placed collision logic in its own method to make Update() more readable.
    10/26/24 CEV- Changed scope of classes and fields to closer resemble c# and OOP standards. Cleaned up and simplified field definitions.
 
 */

namespace monogameTutorial {
    public class TutGame : Game {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch; //object for draw method
        private FollowCamera camera; //allows for viewport centered player. Translates player movement to the map
        private Player player;
        Viewport _viewport; //Object for dynamic screen display across devices

        private int tilesize = 32; //map tilesize for handling sprite sheet/draw calls
        private Texture2D rectangleTexture; //debug variable for rectHollow method
        int count = 0; //debug variable

        private TileLayer baseLayer; //base display layer for sandbox map
        private TileLayer collisionLayer; //collision layer for sandbox map

        public TutGame() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
            
        protected override void Initialize() {
            _viewport = _graphics.GraphicsDevice.Viewport;
            camera = new FollowCamera(Vector2.Zero, _viewport); //load camera object

            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

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

            // currently loads a scaled spider as sandbox player
            Texture2D texture = Content.Load<Texture2D>("rpgcritters2");
            Vector2 position = new Vector2(200, 200);
            int[] crop = { 15, 12, 32, 32 }; //TODO: change Player to accept source rect directly
            int spriteScale = 50;
            Rectangle dest = new(
               _viewport.Width / 2 - spriteScale / 2,
               _viewport.Height / 2 - spriteScale / 2,
               spriteScale,
               spriteScale
            );

            player = new Player(
                texture,
                dest,
                position,
                crop, 
                _viewport);

            //Debug stuff for DrawRectHollow()
            rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            rectangleTexture.SetData(new Color[] { new(255, 0, 0, 255) });
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float[] playerVelocity = { 1.5f, 1.5f, 1.5f, 1.5f }; //baseline player movement resets every update. Will change based on game mechanics. Best way currently is to refresh state every frame.
            
            count++; //begining to experiment with frame counting for animations
            if (count % 30 == 0) {
                Debug.WriteLine(count);
            }

            HandleCollisions(collisionLayer, playerVelocity, player, tilesize, camera); // method directly below for handling collision events
            player.Update(playerVelocity); //Update player according to class logic
            camera.Follow(player.Position, new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight)); //update camera offset with player movement

            base.Update(gameTime);
        }

        //Define the collision point between rectangles and handle player movement. Needs optimization
        internal void HandleCollisions(TileLayer collisionLayer, float[] playerVelocity, Player player, int tlesize, FollowCamera camera) {
            foreach (var item in collisionLayer.tileMap) {
                Vector2 position = item.Key;
                //Rect for current position of collision layer tiles
                Rectangle mapLocation = new(
                        (int)position.X * tilesize + (int)camera.Position.X,
                        (int)position.Y * tilesize + (int)camera.Position.Y,
                        tilesize,
                        tilesize
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
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            baseLayer.Draw(_spriteBatch, camera.Position);
            collisionLayer.Draw(_spriteBatch, camera.Position);

            //Debug method for collision tile hitbox
            foreach (var item in collisionLayer.tileMap) {
                Vector2 position = item.Key;
                Rectangle mapLocation = new(
                        (int)position.X * tilesize + (int)camera.Position.X,
                        (int)position.Y * tilesize + (int)camera.Position.Y,
                        tilesize,
                        tilesize
                );
                DrawRectHollow(_spriteBatch, mapLocation, 4);
            }
            
            //Debug method for player hitbox. TODO: add method for defining player hitbox in Player class
            DrawRectHollow(_spriteBatch, player.DRect, 4);
            player.Draw(_spriteBatch);

            _spriteBatch.End();
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
