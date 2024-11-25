#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using cevEngine2D.source.engine;
using cevEngine2D.source.engine.input;
using cevEngine2D.source.world.projectiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using cevEngine2D.source.engine.animate;
#endregion
//TODO: State machine
namespace cevEngine2D.source.world.units
{
    internal class Player : Unit {
        private AnimationManager animations;
        private SpriteEffects flipEffect;

        //public Vector2 MapPosition { get { return _mapPosition; } }

        public Player(string texture, Vector2 pos, Vector2 size, Rectangle sRect/*, Vector2 position*/) : base(texture, pos, size, sRect) {
            flipEffect = SpriteEffects.None;

            _dRect = new Rectangle(
                Globals.viewport.Width / 2 - (int)size.X / 2,
                Globals.viewport.Height / 2 - (int)size.Y / 2,
                (int)size.X,
                (int)size.Y
                );

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

        public void Update(float[] velocity) {
            bool diagnolMovement = false;
            List<String> keyStrings = Globals.keyboard.pressedKeys.Select(key => key.key).ToList();
            string[] diagnolKeyTargets = { "W", "A", "S", "D"};
            diagnolMovement = keyStrings.Intersect(diagnolKeyTargets).Count() > 1;


            if (diagnolMovement) {
                if (Globals.keyboard.GetPress("W") && Globals.keyboard.GetPress("D")) {
                    _pos += new Vector2(velocity[1], -velocity[3]);
                    animations.Update("WD");
                    flipEffect = SpriteEffects.None;
                }

                if (Globals.keyboard.GetPress("W") && Globals.keyboard.GetPress("A")) {
                    _pos += new Vector2 (-velocity[1], -velocity[2]);
                    animations.Update("WA");
                    flipEffect = SpriteEffects.FlipHorizontally;
                }

                if (Globals.keyboard.GetPress("S") && Globals.keyboard.GetPress("D")) {
                    _pos += new Vector2(velocity[0], velocity[3]);
                    animations.Update("SD");
                    flipEffect = SpriteEffects.None;
                }

                if (Globals.keyboard.GetPress("S") && Globals.keyboard.GetPress("A")) {
                    _pos += new Vector2(-velocity[0], velocity[1]);
                    animations.Update("SA");
                    flipEffect = SpriteEffects.FlipHorizontally;
                }
            } else {
                if (Globals.keyboard.GetPress("A")) {
                    _pos.X -= velocity[1];
                    animations.Update("A");
                    flipEffect = SpriteEffects.FlipHorizontally;
                }
                if (Globals.keyboard.GetPress("D")) {
                    _pos.X += velocity[3];
                    animations.Update("D");
                    flipEffect = SpriteEffects.None;
                }
                if (Globals.keyboard.GetPress("W")) {
                    _pos.Y -= velocity[2];
                    animations.Update("W");
                    flipEffect = SpriteEffects.None;
                }
                if (Globals.keyboard.GetPress("S")) {
                    _pos.Y += velocity[0];
                    animations.Update("S");
                    flipEffect = SpriteEffects.None;
                }
            }
            if (/*Globals.keyboard.GetPress("E")*/Globals.mouse.RightClickRelease()) {
                GameGlobals.PassProjectile(new Fireball(
                    "FIREBALL",
                    new Vector2(_hitbox.X + Size.X / 2, _hitbox.Y + _hitbox.Height),
                    new Vector2(100, 100),
                    new Rectangle(0, 0, 16, 16), 
                    new Vector2(Globals.mouse.newMousePos.X, Globals.mouse.newMousePos.Y),
                    100000)
                );
            }

            updateHitBox(Vector2.Zero);
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
