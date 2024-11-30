#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using cevEngine2D.source.engine.input;
using cevEngine2D.source.world.projectiles;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using cevEngine2D.source.engine.sprites;
#endregion

namespace cevEngine2D.source.world.units {
    public class Unit : BasicUnit {

        protected bool _dead;
        protected float _speed;
        public bool Dead { get { return _dead; } set { _dead = value; } }
        public float Speed { get { return _speed; } }
        public List<Rectangle> UnitPerimeterSliced = new();
        public Rectangle UnitPerimeter;

        public Unit(string texture, Vector2 pos, Vector2 size, Rectangle sRect) : base(texture, pos, size, sRect) {
            CreateUnitPerimeterIndex();

            //UnitPerimeter = _hitbox;
            _dead = false;
        }

        //Uses top left corner from CreateUnitPerimeterIndex
        public Rectangle CreateUnitPerimeter(Rectangle scaleRect) {
            Rectangle generalCollisionCheck = new Rectangle(
                scaleRect.X,
                scaleRect.Y,
                scaleRect.Width * 3,
                scaleRect.Height * 3
                );

            return generalCollisionCheck;
        }

        public void CreateUnitPerimeterIndex() {
            Rectangle topLeftCorner = new Rectangle(
                    (int)Hitbox.X - Hitbox.Width,
                    (int)Hitbox.Y - Hitbox.Height,
                    Hitbox.Width,
                    Hitbox.Height
                    );
            UnitPerimeterSliced.Add(topLeftCorner);
            this.UnitPerimeter = CreateUnitPerimeter(topLeftCorner);

            for (int i = 0; i <= 2; i++) {
                if (i > 0) { //avoid adding corner twice
                    UnitPerimeterSliced.Add(new Rectangle(
                        topLeftCorner.X + Hitbox.Width * i,
                        topLeftCorner.Y,
                        Hitbox.Width,
                        Hitbox.Height
                        )
                    );
                }

                for (int j = 1; j < 3; j++) {
                    UnitPerimeterSliced.Add(new Rectangle(
                        topLeftCorner.X + Hitbox.Width * i,
                        topLeftCorner.Y + Hitbox.Height * j,
                        Hitbox.Width,
                        Hitbox.Height
                        )
                    );
                }
            }
        }

        public override void Update() {
            UnitPerimeterSliced.Clear();
            CreateUnitPerimeterIndex();
            base.Update();
        }

        public virtual void GetHit() {
            Dead = true;
        }

        public override void Draw() {
            base.Draw();
        }
    }
}
