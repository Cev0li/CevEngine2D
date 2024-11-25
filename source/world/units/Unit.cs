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

namespace cevEngine2D.source.world.units
{
    internal class Unit : BasicUnit
    {

        protected bool _dead, _diagnolLeft, _diagnolRight;
        protected float _speed;
        public bool Dead { get { return _dead; } set { _dead = value; } }
        public float Speed { get { return _speed; } }

        public Unit(string texture, Vector2 pos, Vector2 size, Rectangle sRect) : base(texture, pos, size, sRect)
        {
            _dead = false;
            _diagnolLeft = false;
            _diagnolRight = false;
        }

        public override void Update()
        {
            base.Update();
        }

        public virtual void GetHit()
        {
            Dead = true;
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
