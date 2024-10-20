using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using SharpDX.Direct3D11;
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
 */

namespace monogameTutorial {
    public class TutGame : Game {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private FollowCamera camera;
        private Player player;
        private int tilesize = 32;
        private Texture2D rectangleTexture;

        TileLayer baseLayer;
        TileLayer stoneLayer;

        public TutGame() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            camera = new(Vector2.Zero);
        }

        protected override void Initialize() {
            // TODO: Add your initialization logic here

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
            int[] crop = { 0, 0, 50, 50 };
            player = new Player(texture, new Vector2(0, 0), new Vector2(50, 50), crop, _graphics.GraphicsDevice.Viewport);

            rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            rectangleTexture.SetData(new Color[] { new(255, 0, 0, 255) });
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player.Update();

            foreach (var item in stoneLayer.tileMap) {
                Vector2 position = item.Key;
                Rectangle mapLocation= new(
                        (int)position.X * tilesize + (int)camera.position.X,
                        (int)position.Y * tilesize + (int)camera.position.X,
                        tilesize,
                        tilesize
                    );
            }

            camera.follow(player.dRect, new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            baseLayer.Draw(_spriteBatch, camera.position);
            stoneLayer.Draw(_spriteBatch, camera.position);
            player.Draw(_spriteBatch);

            //DrawRectHollow(_spriteBatch, player.dRect, 4);

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
