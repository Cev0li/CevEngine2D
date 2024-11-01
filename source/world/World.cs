#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogameTutorial.source.engine;
using monogameTutorial.source.engine.input;
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
    internal class World {
        public Player player; //TODO;handle collision logic so this is private
        private List<Projectile> projectiles = new();

        public World() {

            player = new(
                "rpgCritters2",
                new Vector2(
                    Globals.viewport.Width / 2 - 50 / 2,
                    Globals.viewport.Height / 2 - 50 / 2),
                new Vector2(50, 50),
                new Rectangle(15, 12, 32, 32),
                new Vector2(200, 200)
            );

            GameGlobals.PassProjectile = AddProjectile;
        }

        public void Update(float[] playerVelocity) {

            player.Update(playerVelocity);

            for (int i = 0; i < projectiles.Count; i++) {
                projectiles[i].Update(null);

                if (projectiles[i].Done) {
                    projectiles.RemoveAt(i);
                    i--;
                }
            }
        }

        public virtual void AddProjectile(object projectile) {
            projectiles.Add((Projectile)projectile);
        }

        public void Draw() {
            player.Draw();

            for (int i = 0; i < projectiles.Count; i++) {
                projectiles[i].Draw();
            }
        }

    }
}
