#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using cevEngine2D.source.engine;
using cevEngine2D.source.engine.animate;
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
using static System.Net.Mime.MediaTypeNames;
#endregion


namespace cevEngine2D.source.world {
    internal class World {
        public Player player; //TODO;handle collision logic so this is private
        private List<Projectile> projectiles = new();
        private List<Mob> mobs = new();
        private List<SpawnPoint> spawnPoints = new();

        public World() {
            //currently hardcoded scaling etc...
            player = new(
                "archer",
                new Vector2(
                    Globals.viewport.Width / 2 - 50 / 2,
                    Globals.viewport.Height / 2 - 50 / 2),
                new Vector2(100, 100),
                new Rectangle(0, 0, 72, 72),
                new Vector2(200, 200)
            );

            spawnPoints.Add(new SpawnPoint(new Vector2(680, 200)));

            GameGlobals.PassProjectile = AddProjectile; //Add to projectiles list. Manage all projectiles in game.
            GameGlobals.PassMob = AddMob; //Add to mob list. Manage all mobs in game.
        }

        public void Update(float[] playerVelocity) {
            player.Update(playerVelocity);

            for (int i = 0; i < projectiles.Count; i++) {
                //projectiles[i].Update(mobs.ToList<Unit>());
                projectiles[i].Update(mobs.Cast<Unit>().ToList());

                if (projectiles[i].Done) {
                    projectiles.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < spawnPoints.Count; i++) {
                spawnPoints[i].Update();
            }

            for (int i = 0; i < mobs.Count; i++) {
                mobs[i].Update(player);

                if (mobs[i].Dead) {
                    mobs.RemoveAt(i);
                    i--;
                }
            }
        }

        public virtual void AddMob(object mob) {
            mobs.Add((Mob)mob);
        }

        public virtual void AddProjectile(object projectile) {
            projectiles.Add((Projectile)projectile);
        }

        public void Draw() {
            List<BasicUnit> mapSprites = new();
            List<BasicUnit> allSprites = new();

            allSprites.AddRange(mapSprites);
            allSprites.AddRange(projectiles);
            allSprites.AddRange(mobs);
            allSprites.Add(player);
            allSprites.AddRange(spawnPoints);
            allSprites.Sort((s1, s2) => s2.POS.Y.CompareTo(s1.POS.Y));

            for (int i = 0; i < allSprites.Count; i++) {
                allSprites[i].Draw();
            }

        }
    }
}
