#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogameTutorial.source.engine;
using monogameTutorial.source.engine.input;
using monogameTutorial.source.world;
using monogameTutorial.source.world.projectiles;
using monogameTutorial.source.world.units;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
#endregion

namespace monogameTutorial.source.world {
    internal class GameGlobals {
        public static PassObject PassProjectile; //initalized in World class
        public static PassObject PassMob; //initalized in World class

        public static FollowCamera camera; //allows for viewport centered player. Translates player movement to the map
        public static int tileSize;
    }
}
