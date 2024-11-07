#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogameTutorial.source.engine;
using monogameTutorial.source.engine.input;
using monogameTutorial.source.world.projectiles;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
#endregion

namespace monogameTutorial.source.world.units
{
    internal class Mob : Unit {
        //constructor takes array of 4 values used to crop a sprite sheet.
        public Mob(string texture, Vector2 pos, Vector2 size, Rectangle sRect) : base(texture, pos, size, sRect) {
            
        }

        public virtual void Update(Player player) {
            AI(player);
            base.Update();
        }

        public virtual void AI(Player player) {
            _pos += Globals.RadialMovement(player.MapPosition + player.track, _pos, _speed);
        }

        public override void Draw() {
            base.Draw();
        }
    }
}