#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using cevEngine2D.source.engine;
using cevEngine2D.source.engine.input;
using cevEngine2D.source.engine.DataStructures.structs;
using cevEngine2D.source.engine.DataStructures.enums;
using cevEngine2D.source.world.projectiles;
using cevEngine2D.source.engine.sprites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using cevEngine2D.source.engine.animate;
using cevEngine2D.source.engine.interfaces;
using System.Drawing.Text;
#endregion

namespace cevEngine2D.source.world.units {
    internal class Player : Unit {
        private AnimationManager animations;
        private ICollisionManager collisionManager;

        private List<Direction> collisionLocation = new();
        private SpriteEffects flipEffect;

        public Player(string texture, Vector2 pos, Vector2 size, Rectangle sRect) : base(texture, pos, size, sRect) {
            _speed = 2;
            flipEffect = SpriteEffects.None;


            animations = new AnimationManager(this);
            animations.AddAnimation("W", 5, 72, false, false, new Vector2(0, 0));
            animations.AddAnimation("A", 5, 72, false, false, new Vector2(2, 0));
            animations.AddAnimation("D", 5, 72, false, false, new Vector2(2, 0));
            animations.AddAnimation("S", 5, 72, false, false, new Vector2(4, 0));
            animations.AddAnimation("WD", 5, 72, false, false, new Vector2(1, 0));
            animations.AddAnimation("WA", 5, 72, false, false, new Vector2(1, 0));
            animations.AddAnimation("SD", 5, 72, false, false, new Vector2(3, 0));
            animations.AddAnimation("SA", 5, 72, false, false, new Vector2(3, 0));
        }

        public void SubscribeToCollisionEvent(ICollisionManager eventRaiser) {
            collisionManager = eventRaiser;
            collisionManager.CollisionEvent += HandleCollisions;
        }

        public void HandleCollisions(int i) {
            Direction direction = (Direction)i;
            collisionLocation.Add(direction);
        }

        public override void Update() {
            Vector2 velocity = Vector2.Zero;

            //Debug.WriteLine(collisionLocation.Count());
            if (Globals.keyboard.GetPress("W") 
                && Globals.keyboard.GetPress("A") 
                && !collisionLocation.Contains(Direction.NE)) {
                velocity.X -= 1;
                velocity.Y -= 1;
                animations.Update("WA");
                flipEffect = SpriteEffects.FlipHorizontally;
            } else if (Globals.keyboard.GetPress("W") && Globals.keyboard.GetPress("D")) {
                velocity.X += 1;
                velocity.Y -= 1;
                animations.Update("WD");
            } else if (Globals.keyboard.GetPress("S") && Globals.keyboard.GetPress("A")) {
                velocity.X -= 1;
                velocity.Y += 1;
                animations.Update("SA");
                flipEffect = SpriteEffects.FlipHorizontally;
            } else if (Globals.keyboard.GetPress("S") && Globals.keyboard.GetPress("D")) {
                velocity.X += 1;
                velocity.Y += 1;
                animations.Update("SD");
            } else if (Globals.keyboard.GetPress("A") && !collisionLocation.Contains(Direction.E) && !collisionLocation.Contains(Direction.NE)) {
                velocity.X -= 1;
                animations.Update("A");
                flipEffect = SpriteEffects.FlipHorizontally;
            } else if (Globals.keyboard.GetPress("D")) {
                velocity.X += 1;
                animations.Update("D");
                flipEffect = SpriteEffects.None;
            } else if (Globals.keyboard.GetPress("W") && !collisionLocation.Contains(Direction.N) && !collisionLocation.Contains(Direction.NE)) {
                velocity.Y -= 1;
                animations.Update("W");
                flipEffect = SpriteEffects.None;
            } else if (Globals.keyboard.GetPress("S")) {
                velocity.Y += 1;
                animations.Update("S");
                flipEffect = SpriteEffects.None;
            }

            if (velocity != Vector2.Zero) {
                velocity.Normalize();
            }

            _pos += velocity * _speed;

            if (Globals.mouse.RightClickRelease()) {
                GameGlobals.PassProjectile(new Fireball(
                    "FIREBALL",
                    new Vector2(_hitbox.X + Size.X / 2, _hitbox.Y + _hitbox.Height),
                    new Vector2(100, 100),
                    new Rectangle(0, 0, 16, 16),
                    new Vector2(Globals.mouse.newMousePos.X, Globals.mouse.newMousePos.Y),
                    100000)
                );
            }

            collisionLocation.Clear();
            base.Update();
        }

        public override void Draw() {
            Globals.spriteBatch.Draw(
                SpriteTexture,
                _dRect,
                _sRect,
                Color.White,
                0f,
                Vector2.Zero,
                flipEffect,
                0f
            );
        }
    }
}
