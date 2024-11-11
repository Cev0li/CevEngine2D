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
#endregion

namespace cevEngine2D.source.world.units.mobs {

    internal class Imp : Mob {
        //constructor takes array of 4 values used to crop a sprite sheet.
        public Imp(Vector2 pos, Vector2 size, Rectangle sRect) : base("FIREBALL", pos, size, sRect) {

        }

        public override void Update(Player player) {
            base.Update(player);
        }

        public override void Draw() {
            base.Draw();
        }
    }
}
