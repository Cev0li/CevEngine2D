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
using cevEngine2D.source.world.units;
#endregion

namespace cevEngine2D.source.world.projectiles {
    internal class Projectile : Unit {
        protected Vector2 _direction, _location, _distance;
        protected float rotation;
        protected bool _done;
        protected GameTimer _timer;
        //Properties
        public bool Done { get { return _done; } }

        //TODO: Add owner variable and replace hardcoded center screen logic with owners dRect
        public Projectile(string texture, Vector2 pos, Vector2 size, Rectangle sRect, Vector2 target, int setTimer) : base(texture, pos, size, sRect) {
            _speed = 5.0f;
            _done = false;
            _direction = target - _pos;
            _direction.Normalize();
            _timer = new GameTimer(setTimer);
            rotation = (float)Math.Atan2(_direction.Y, _direction.X);
        }

        public virtual void Update(List<Unit> units) {
            _pos += _direction * _speed;
            _dRect = new Rectangle(
                (int)(_pos.X - _size.X / 2),
                (int)(_pos.Y - _size.Y / 2),
                (int)_size.X,
                (int)_size.Y
            );
            updateHitBox(Vector2.Zero);

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
            for (int i = 0; i < units.Count(); i++) {
                if (units[i].DRect.Intersects(DRect)) {
                    units[i].GetHit();
                    return true;
                }
            }
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
                0.0f
            );
            Rectangle newRect = GetRotatedHitbox(Hitbox, rotation);
            Globals.DrawRectHollow(newRect, 1);
        }

        public Rectangle GetRotatedHitbox(Rectangle originalRect, float rotation) {
            // Calculate the center of the rectangle
            Vector2 center = new Vector2(originalRect.X + originalRect.Width / 2, originalRect.Y + originalRect.Height / 2);

            // Calculate the four corners of the rectangle
            Vector2 topLeft = new Vector2(originalRect.Left, originalRect.Top);
            Vector2 topRight = new Vector2(originalRect.Right, originalRect.Top);
            Vector2 bottomLeft = new Vector2(originalRect.Left, originalRect.Bottom);
            Vector2 bottomRight = new Vector2(originalRect.Right, originalRect.Bottom);

            // Rotate each corner point
            topLeft = RotatePoint(topLeft, center, rotation);
            topRight = RotatePoint(topRight, center, rotation);
            bottomLeft = RotatePoint(bottomLeft, center, rotation);
            bottomRight = RotatePoint(bottomRight, center, rotation);

            // Find the minimum and maximum x and y coordinates
            float minX = Math.Min(Math.Min(topLeft.X, topRight.X), Math.Min(bottomLeft.X, bottomRight.X));
            float minY = Math.Min(Math.Min(topLeft.Y, topRight.Y), Math.Min(bottomLeft.Y, bottomRight.Y));
            float maxX
                = Math.Max(Math.Max(topLeft.X, topRight.X), Math.Max(bottomLeft.X, bottomRight.X));
            float maxY = Math.Max(Math.Max(topLeft.Y, topRight.Y), Math.Max(bottomLeft.Y, bottomRight.Y));

            // Create a new rectangle based on the rotated bounds
            return new Rectangle((int)minX - _hitbox.Width / 2, (int)minY - _hitbox.Height / 2, (int)(maxX - minX), (int)(maxY - minY));
        }

        // Helper method to rotate a point around a center point
        private Vector2 RotatePoint(Vector2 point, Vector2 center, float angle) {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            float x = (point.X - center.X) * cos - (point.Y - center.Y) * sin + center.X;
            float y = (point.X - center.X) * sin + (point.Y - center.Y) * cos + center.Y;
            return new Vector2(x, y);
        }
    }
}
