using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

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
        
 */

namespace monogameTutorial {
    public class TutGame : Game {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Viewport _viewport;
        private FollowCamera camera;
        private Player player;
        private int tilesize = 32;
        private Texture2D rectangleTexture; //debug variable for rectHollow method
        int count = 0; //debug variable

        TileLayer baseLayer;
        TileLayer stoneLayer;

        public TutGame() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
            
        protected override void Initialize() {
            _viewport = _graphics.GraphicsDevice.Viewport;
            camera = new FollowCamera(Vector2.Zero, _viewport);

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

            stoneLayer = new(
                "../../../data/tutorialmap._stone.csv",
                Content.Load<Texture2D>("ground_Tiles"),
                21
                );

            // currently loads a scaled spider
            Texture2D texture = Content.Load<Texture2D>("rpgcritters2");
            Rectangle dest = new(
            //350,
            //190,
            //50,
            //50
               _viewport.Width / 2 - 25,
               _viewport.Height / 2 - 25,
               50,
               50
           );
            Vector2 position = new Vector2(400, 400);
            int[] crop = { 15, 12, 32, 32 };
            player = new Player(
                texture,
                dest,
                position,
                crop, 
                _viewport);

            rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            rectangleTexture.SetData(new Color[] { new(255, 0, 0, 255) });
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            bool collision = false;
            //player.Update(Keyboard.GetState());
            player.Update();
            foreach (var item in stoneLayer.tileMap) {
                Vector2 position = item.Key;
                Rectangle mapLocation= new(
                        (int)position.X * tilesize + (int)camera.position.X,
                        (int)position.Y * tilesize + (int)camera.position.Y,
                        tilesize,
                        tilesize
                );
                if (mapLocation.Top < player.DRect.Bottom &&
                        mapLocation.Bottom > player.DRect.Top &&
                        mapLocation.Left < player.DRect.Right &&
                        mapLocation.Right > player.DRect.Left) 
                {
                    Debug.WriteLine("collision " + count);
                    count++;
                    collision = true;

                    int[] possibleIntersections = new int[] {
                        Math.Abs(mapLocation.Top - player.DRect.Top),
                        Math.Abs(mapLocation.Right - player.DRect.Right),
                        Math.Abs(mapLocation.Bottom - player.DRect.Bottom),
                        Math.Abs(mapLocation.Left - player.DRect.Left)
                        };
                    int maxValue = -1;
                    int maxIndex = -1;
                    for (int i = 0; i < possibleIntersections.Length; i++) {
                        if (possibleIntersections[i] > maxValue) {
                            maxValue = possibleIntersections[i];
                            maxIndex = i;

                        }
                    }

                    switch (maxIndex) {
                        case 0:
                            Debug.WriteLine("top");
                            break;
                        case 1:
                            Debug.WriteLine("Right");
                            break;
                        case 2:
                            Debug.WriteLine("Bottom");
                            break;
                        case 3:
                            Debug.WriteLine("Left");
                            player.DRect.X = mapLocation.Left - player.DRect.Width;
                            break;
                    }
                    
                }
            }

            camera.follow(player.position, new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            baseLayer.Draw(_spriteBatch, camera.position);
            stoneLayer.Draw(_spriteBatch, camera.position);

            foreach (var item in stoneLayer.tileMap) {
                Vector2 position = item.Key;
                Rectangle mapLocation = new(
                        (int)position.X * tilesize + (int)camera.position.X,
                        (int)position.Y * tilesize + (int)camera.position.Y,
                        tilesize,
                        tilesize
                    );
                DrawRectHollow(_spriteBatch, mapLocation, 4);
            }
            
            DrawRectHollow(_spriteBatch, player.DRect, 4);
            player.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

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
