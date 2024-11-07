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

namespace monogameTutorial.source.world.units.mobs {

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
