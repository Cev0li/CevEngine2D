#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogameTutorial.source.engine;
using monogameTutorial.source.engine.input;
using monogameTutorial.source.world.projectiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
#endregion

namespace monogameTutorial.source.world.units {
    internal class Player : Unit {
        private Vector2 _mapPosition; //offset of map in relationship to centered player. Controls camera movement to simulate player movement/ player start on map
        public Vector2 mapPosition { get { return _mapPosition; } }

        //constructor takes array of 4 values used to crop a sprite sheet.
        public Player(string texture, Vector2 pos, Vector2 size, Rectangle sRect, Vector2 position) : base(texture, pos, size, sRect) {
            _mapPosition = position; //Camera offset variable. This locates player at specific point on map
        }

        public void Update(float[] velocity) {
            if (Globals.keyboard.GetPress("A")) {
                _mapPosition.X -= velocity[1];
            }
            if (Globals.keyboard.GetPress("D")) {
                _mapPosition.X += velocity[3];
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W)) {
                _mapPosition.Y -= velocity[2];
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S)) {
                _mapPosition.Y += velocity[0];
            }
            if (Globals.mouse.LeftClick()) {
                GameGlobals.PassProjectile(new Fireball(
                    "Fireball_Effect_01",
                    new Vector2(_pos.X + _dRect.Width / 2, _pos.Y + _dRect.Height / 2),
                    new Vector2(100, 100),
                    new Rectangle(0, 0, 150, 150), 
                    new Vector2(Globals.mouse.newMousePos.X, Globals.mouse.newMousePos.Y),
                    1000)
                );
                Debug.WriteLine(_pos.X + _dRect.Width / 2);
            }
        }

        public override void Draw() {
            Globals.spriteBatch.Draw(
                SpriteTexture,
                DRect,
                _sRect,
                Color.White
            );
        }
    }
}
