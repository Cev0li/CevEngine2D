#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using cevEngine2D.source.engine.input;
using cevEngine2D.source.world;
using cevEngine2D.source.world.projectiles;
using cevEngine2D.source.world.units;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using cevEngine2D.source.engine;
#endregion

namespace cevEngine2D.source.world
{
    internal class GameGlobals {
        public static PassObject PassProjectile; //initalized in World class
        public static PassObject PassMob; //initalized in World class

        public static FollowCamera camera; //allows for viewport centered player. Translates player movement to the map
        public static int tileSize;
    }
}
