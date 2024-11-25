#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using cevEngine2D.source.engine.input;
using cevEngine2D.source.world;
using cevEngine2D.source.world.projectiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using cevEngine2D.source.world.units;
#endregion

namespace cevEngine2D.source.engine.sprites {
    internal class MapUnit : BasicUnit {
        public MapUnit(string texture, Vector2 pos, Vector2 size, Rectangle sRect) : base(texture, pos, size, sRect) {
            _dRect = new Rectangle(
                 (int)(_pos.X),
                 (int)(_pos.Y),
                 (int)_size.X,
                 (int)_size.Y
            );
        }

        public override void Update() {
            _dRect = new Rectangle(
                (int)(_pos.X),
                (int)(_pos.Y),
                (int)_size.X,
                (int)_size.Y
            );
            updateHitBox(GameGlobals.camera.Position);
        }

        public override void Draw() {
            Globals.spriteBatch.Draw(
                _texture,
                _dRect = new Rectangle(
                    (int)(_pos.X),
                    (int)(_pos.Y),
                    (int)_size.X,
                    (int)_size.Y
                ),
                _sRect,
                Color.White);
        }

    }
}
