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
namespace cevEngine2D.source.world.projectiles
{
    internal class Fireball : Projectile {

        public Fireball(String texture, Vector2 pos, Vector2 size, Rectangle sRect, Vector2 target, int setTimer)
                : base(texture, pos, size, sRect, target, setTimer) { }

        public override void Update(List<Unit> units) {
            base.Update(units);
        }
    }
}
