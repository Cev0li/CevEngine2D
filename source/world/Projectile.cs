#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using cevEngine2D.source.engine;
using cevEngine2D.source.engine.input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
#endregion

namespace cevEngine2D.source.world
{
    internal class Projectile : Unit
    {
        protected Vector2 _direction, _location, _distance;
        protected float rotation;
        protected bool _done;
        protected GameTimer _timer;
        //Properties
        public bool Done { get { return _done; } }

        //TODO: Add owner variable and replace hardcoded center screen logic with owners dRect
        public Projectile(string texture, Vector2 pos, Vector2 size, Rectangle sRect, Vector2 target, int setTimer) : base(texture, pos, size, sRect)
        {
            _speed = 5.0f;
            _done = false;
            _location = new Vector2(pos.X + SpriteTexture.Height / 2, pos.Y + SpriteTexture.Width / 2);
            _direction = target - _location;
            _direction.Normalize();
            _timer = new GameTimer(setTimer);
            //_distance = new Vector2(target.X - pos.X, target.Y - pos.Y);
            //_distance.Normalize();
            rotation = (float)Math.Atan2(_direction.Y, _direction.X);
        }

        public virtual void Update(List<Unit> units)
        {
            _pos += _direction * _speed;

            _timer.UpdateTimer();
            if (_timer.Test())
            {
                _done = true;
            }

            if (hitSomething(units))
            {
                _done = true;
            }

            base.Update();
        }

        public virtual bool hitSomething(List<Unit> units)
        {
            //check collision
            for (int i = 0; i < units.Count(); i++)
            {
                if (units[i].DRect.Intersects(DRect))
                {
                    units[i].GetHit();
                    return true;
                }
            }
            return false;
        }

        //public override void Update() {
        //    base.Update();
        //}

        public override void Draw()
        {
            Globals.spriteBatch.Draw(
                _texture,
                new Rectangle(
                    (int)(_pos.X - _size.X / 2), 
                    (int)(_pos.Y - _size.Y / 2), 
                    (int)Size.X, 
                    (int)Size.Y),
                _sRect,
                Color.White,
                rotation,
                new Vector2(SpriteTexture.Height / 2, SpriteTexture.Width / 2),
                SpriteEffects.None,
                0.0f);
        }
    }
}
