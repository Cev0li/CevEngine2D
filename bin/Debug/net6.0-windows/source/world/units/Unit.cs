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
//TODO: Add source rectangle firld to handle sprite sheets
namespace monogameTutorial.source.world.units
{
    public class Unit
    {
        protected Texture2D _texture;
        public Texture2D SpriteTexture { get { return _texture; } }
        protected Vector2 _pos;
        protected Vector2 _size;
        protected Rectangle _dRect; //Destination rectangle/sprite scale
        public Rectangle DRect { get { return _dRect; } set { _dRect = value; } }
        protected Rectangle _sRect; //Destination rectangle/sprite scale
        public Rectangle SRect { get { return _sRect; } set { _sRect = value; } }


        public Unit(string texture, Vector2 pos, Vector2 size, Rectangle sRect)
        {
            _texture = Globals.content.Load<Texture2D>(texture);
            _pos = pos;
            _size = size;
            _dRect = new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
            _sRect = sRect;
        }

        public virtual void Update() { }

        public virtual void Draw()
        {
            Globals.spriteBatch.Draw(_texture, _dRect, Color.White);
        }
    }
}
