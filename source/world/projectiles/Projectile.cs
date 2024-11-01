#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogameTutorial.source.engine;
using monogameTutorial.source.world.units;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
#endregion

namespace monogameTutorial.source.world.projectiles {
    internal class Projectile : Unit {
        protected float _speed;
        protected Vector2 _direction;
        protected Vector2 _location; //where projectile spawns
        protected Vector2 _distance;
        protected float rotation;
        protected bool _done;
        public bool Done {get { return _done; } }
        protected GameTimer _timer;
        //TODO: Add owner  variable and replace hardcoded center screen logic with owners dRect
        public Projectile(String texture, Vector2 pos, Vector2 size, Rectangle sRect, Vector2 target, int setTimer) : base(texture, pos, size, sRect) {
            _speed = 5.0f;
            _done = false;
            _location = new Vector2(pos.X + size.X / 2, pos.Y + size.Y / 2);
            _direction = target - _location;
            _direction.Normalize();
            _timer = new GameTimer(setTimer);
            _distance = new Vector2(target.X - pos.X, target.Y - pos.Y);
            _distance.Normalize();
            rotation = (float)Math.Atan2(_distance.Y, _distance.X);
        }

        public virtual void Update(List<Unit> units) {
            _dRect.Offset(_speed * _direction);

            _timer.UpdateTimer();
            if (_timer.Test()) {
                _done = true;
            }

            if (hitSomething(units)) {
                _done = true;
            }
        }

        public virtual bool hitSomething(List<Unit> units) {
            //check collision
            return false;
        }

        public override void Draw() {
            Globals.spriteBatch.Draw(
                _texture,
                _dRect,
                _sRect,
                Color.White,
                rotation,
                new Vector2(SpriteTexture.Height / 2, SpriteTexture.Width / 2),
                SpriteEffects.None,
                0.0f);
            Debug.WriteLine(_dRect.X + " " + _dRect.Y);
        }
    }
}
