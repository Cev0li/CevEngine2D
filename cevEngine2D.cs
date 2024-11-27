#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using cevEngine2D.source.engine;
using cevEngine2D.source.engine.animate;
using cevEngine2D.source.engine.input;
using cevEngine2D.source.engine.tilemap;
using cevEngine2D.source.world;
using cevEngine2D.source.world.projectiles;
using cevEngine2D.source.world.units;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using cevEngine2D.source.engine.tilemap.utils;
#endregion

namespace cevEngine2D {
    public class cevEngine2D : Game {
        private GraphicsDeviceManager _graphics;

        private World world;
        private TileMap spawnMap;

        private CollisionManager<NonDrawableElement> collisionManager;

        public cevEngine2D() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            Globals.viewport = _graphics.GraphicsDevice.Viewport;
            GameGlobals.tileSize = 24;
            Globals.content = this.Content;
            Globals.spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.keyboard = new CevKeyboard();
            Globals.mouse = new CevMouseControl();
            //Debug stuff for DrawRectHollow()
            Globals.rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            Globals.rectangleTexture.SetData(new Color[] { new(255, 0, 0, 255) });



            base.Initialize();
        }

        protected override void LoadContent() {


            //load spawn region map
            InitalizeMap createSpawnMap = new("../../../data/spawnCollisionTEST.tmj");
            spawnMap = createSpawnMap.getMapObject();
            spawnMap.Load();
            collisionManager = new CollisionManager<NonDrawableElement>(spawnMap.CollisionObjects);

            world = new World();

            GameGlobals.camera = new FollowCamera(world.player.POS); //load camera object
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Globals.keyboard.Update();
            Globals.mouse.Update();
            Globals.gameTime = gameTime;


            float[] playerVelocity = { 2f, 2f, 2f, 2f };

            world.Update(playerVelocity);

            foreach (var obj in spawnMap.MapUnits) {
                obj.Update();
            }

            GameGlobals.camera.Update(Globals.gameTime, world.player.POS); //update camera offset with player movement

            Globals.keyboard.UpdateOld();
            Globals.mouse.UpdateOld();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);
            Globals.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, GameGlobals.camera.TransformMatrix);

            foreach (var layer in spawnMap.Layers) {

                if (layer.Type == "tilelayer") {
                    Texture2D layerTexture = spawnMap.SpriteSheetLookup[layer.SpriteSheet];
                    Tileset tilesetData = spawnMap.Tilesets.FirstOrDefault(set => set.Name == layer.SpriteSheet);
                    foreach (Vector2 key in layer.MapMatrix.Keys) {
                        Rectangle dRect = new Rectangle(
                            (int)key.X * GameGlobals.tileSize,
                            (int)key.Y * GameGlobals.tileSize,
                            GameGlobals.tileSize,
                            GameGlobals.tileSize
                            );
                        Rectangle sRect = tilesetData.TilesetAtlas[layer.MapMatrix[key]];
                        Globals.spriteBatch.Draw(layerTexture, dRect, sRect, Color.White);
                    }
                }

                //if (layer.Name == "Collisions") {
                //    foreach (var obj in layer.Objects) {
                //        float x = obj.Width / spawnMap.TileWidth * GameGlobals.tileSize;
                //        float y = obj.Height / spawnMap.TileHeight * GameGlobals.tileSize;
                //        Globals.DrawRectHollow(new Rectangle(
                //            (((int)Math.Round(obj.X) / spawnMap.TileWidth) * GameGlobals.tileSize),
                //            (((int)Math.Round(obj.Y) / spawnMap.TileHeight) * GameGlobals.tileSize),
                //            (int)x,
                //            (int)y),
                //        1);
                //    }
                //}
            }


            collisionManager.CheckCollisions(world.player); //DEBUG ONLY REMOVE WHEN CHECK COLLISIONS IS DONE
            world.Draw(spawnMap.MapUnits);

            Globals.spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
