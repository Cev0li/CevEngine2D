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
#endregion

namespace cevEngine2D.source.engine {
    internal class BasicUnit {
        internal Texture2D _texture;
        protected Vector2 _pos, _size;
        protected Rectangle _dRect, _sRect; //Destination rectangl/sprite scale, Sprite sheet crop
        //Properties
        public Texture2D SpriteTexture { get { return _texture; } }
        public Vector2 POS { get { return _pos; } }
        public Vector2 Size { get { return _size; } }
        public Rectangle DRect { get { return _dRect; } set { _dRect = value; } }
        public Rectangle SRect { get { return _sRect; } set { _sRect = value; } }


        public BasicUnit(string texture, Vector2 pos, Vector2 size, Rectangle sRect) {
            _texture = Globals.content.Load<Texture2D>(texture);
            _pos = pos;
            _size = size;
            _dRect = new Rectangle(
                (int)(pos.X - size.X / 2), 
                (int)(pos.Y - size.Y / 2), 
                (int)size.X, 
                (int)size.Y
            );
            _sRect = sRect;
        }

        public virtual void Update() { 
            _dRect = new Rectangle(
                (int)(_pos.X - _size.X / 2),
                (int)(_pos.Y - _size.Y / 2),
                (int)_size.X,
                (int)_size.Y
            );
        }

        public virtual void Draw() {
            Globals.spriteBatch.Draw(
                _texture,
                _dRect = new Rectangle(
                    (int)(_pos.X - _size.X / 2) + (int)GameGlobals.camera.Position.X,
                    (int)(_pos.Y - _size.Y / 2) + (int)GameGlobals.camera.Position.Y,
                    (int)_size.X,
                    (int)_size.Y
                ),
                _sRect,
                Color.White);
        }

        public virtual void Draw(Vector2 offset) {
            Globals.spriteBatch.Draw(
                _texture,
                _dRect = new Rectangle(
                    (int)((_pos.X - _size.X / 2) + offset.X),
                    (int)((_pos.Y - _size.Y / 2) + offset.Y),
                    (int)_size.X,
                    (int)_size.Y
                ),
                _sRect,
                Color.White);
        }
    }
}
