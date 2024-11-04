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

namespace monogameTutorial.source.world {
    internal class Unit : BasicUnit {

        protected bool _dead;
        protected float _speed, _hitDistance;
        //Properties
        public bool Dead { get { return _dead; } set { _dead = value; } }
        public float Speed { get { return _speed; } }

        public Unit(string texture, Vector2 pos, Vector2 size, Rectangle sRect) : base(texture, pos, size, sRect) {
            _dead = false;
            _speed = 1f;
            _hitDistance = 35f;
        }

        public override void Update() {
            base.Update();
        }

        public virtual void GetHit() {
            Dead = true;
        }
    }
}
