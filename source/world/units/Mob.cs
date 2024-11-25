#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using cevEngine2D.source.engine;
using cevEngine2D.source.engine.input;
using cevEngine2D.source.world.projectiles;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
#endregion

namespace cevEngine2D.source.world.units
{
    internal class Mob : Unit {
        public Mob(string texture, Vector2 pos, Vector2 size, Rectangle sRect) : base(texture, pos, size, sRect) {
            _speed = 4f;
        }

        public virtual void Update(Player player) {
            AI(player);
            base.Update();
        }

        public virtual void AI(Player player) {
            _pos += Globals.RadialMovement(player.POS, _pos, _speed);
        }

        public override void Draw() {
            base.Draw();
        }
    }
}