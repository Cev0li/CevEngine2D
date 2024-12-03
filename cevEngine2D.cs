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

namespace cevEngine2D
{
    public class cevEngine2D : Game {
        private GraphicsDeviceManager _graphics;

        private World world;
        private TileMap spawnMap;

        public cevEngine2D() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            GameGlobals.tileSize = 32;

            Globals.viewport = _graphics.GraphicsDevice.Viewport;
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

            GameGlobals.collisionManager = new CollisionManager<NonDrawableElement>(spawnMap.CollisionObjects);

            world = new World();

            GameGlobals.camera = new FollowCamera(world.Player.POS); //load camera object
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Globals.keyboard.Update();
            Globals.mouse.Update();
            Globals.gameTime = gameTime;

            GameGlobals.collisionManager.CheckCollisions(world.Player);
            world.Update();

            foreach (var obj in spawnMap.MapUnits) {
                obj.Update();
            }

            GameGlobals.camera.Update(Globals.gameTime, world.Player.POS); //update camera offset with player movement

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
            }

            //foreach(Rectangle rect in world.Player.UnitPerimeterSliced) {
            //    Globals.DrawRectHollow(rect, 1);
            //}
            world.Draw(spawnMap.MapUnits);

            foreach (var obj in spawnMap.CollisionObjects) {
                Globals.DrawRectHollow(obj.DRect, 1);
            }
            Globals.DrawRectHollow(world.Player.Hitbox, 1);

            Globals.spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
