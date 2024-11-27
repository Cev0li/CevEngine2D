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

namespace cevEngine2D.source.engine {
    internal class NonDrawableElement : IGameElement {
        protected Vector2 _pos, _size;
        protected Rectangle _dRect;

        public Vector2 POS { get { return _pos; } set { _pos = value; } }
        public Vector2 Size { get { return _size; } set { _size = value; } }
        public Rectangle DRect { get { return _dRect; } set { _dRect = value; } }

        public NonDrawableElement(Vector2 pos, Vector2 size) {
            _pos = pos;
            _size = size;

            _dRect = new Rectangle(
                 (int)(_pos.X),
                 (int)(_pos.Y),
                 (int)_size.X,
                 (int)_size.Y
            );
        }
    }
}
