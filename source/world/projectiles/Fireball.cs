#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogameTutorial.source.engine;
using monogameTutorial.source.world.projectiles;
using monogameTutorial.source.world.units;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
#endregion

namespace monogameTutorial {
    internal class Fireball : Projectile {

        public Fireball(String texture, Vector2 pos, Vector2 size, Rectangle sRect, Vector2 target, int setTimer)
                : base(texture, pos, size, sRect, target, setTimer) { }

        public override void Update(List<Unit> units) {
            base.Update(units);
        }
    }
}
