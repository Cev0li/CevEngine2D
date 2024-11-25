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
/*
 * Map unit class is for interactable map objects usually found drawn in level editor. 
 * The top layers that tie the tilemap into the game mechanics.
 * This class is for those elements specifically because they do not draw themselves from the center of the sprite out. 
 * By drawing from the top left corner the relationship to the tile layers is not changed.
 * Generally speaking, this is class represents a stationary, interactable game element.
 */ 
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
            _hitbox = new Rectangle(
                _dRect.X + _dRect.Width / 2 - _hitbox.Width / 2,
                _dRect.Y + _dRect.Height / 2 - _hitbox.Height / 2,
                _hitbox.Width,
                _hitbox.Height
            );
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
