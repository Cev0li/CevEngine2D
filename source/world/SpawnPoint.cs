#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using cevEngine2D.source.engine;
using cevEngine2D.source.engine.input;
using cevEngine2D.source.world;
using cevEngine2D.source.world.projectiles;
using cevEngine2D.source.world.units;
using cevEngine2D.source.world.units.mobs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
#endregion

namespace cevEngine2D.source.world.units {
    internal class SpawnPoint : BasicUnit {

        protected bool _dead;
        //protected float _hitDistance;
        protected GameTimer _spawnTimer = new(2000);
        //Properties
        public bool Dead { get { return _dead; } set { _dead = value; } }

        public SpawnPoint(Vector2 pos) : base("circle", pos, new Vector2(75, 75), new Rectangle(0,0,128, 128)) {
            _dead = false;
            //_hitDistance = 35f;
        }

        public virtual void GetHit() {
            Dead = true;
        }

        public override void Update() {
            _spawnTimer.UpdateTimer();
            if (_spawnTimer.Test()) {
                SpawnMob();
                _spawnTimer.ResetToZero();
            }
            base.Update();
        }

        public virtual void SpawnMob() {
            GameGlobals.PassMob(new Imp(
                new Vector2(_pos.X, _pos.Y),
                new Vector2(50, 50),
                new Rectangle(0, 0, 16, 16))
            );
        }
    }
}
