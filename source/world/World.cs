﻿#region includes
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


namespace monogameTutorial.source.world
{
    internal class World {
        public Player player; //TODO;handle collision logic so this is private
        private List<Projectile> projectiles = new();
        private List<Mob> mobs = new();
        private List<SpawnPoint> spawnPoints = new();
        public World() {
            //currently hardcoded scaling etc...
            player = new(
                "rpgCritters2",
                new Vector2(
                    Globals.viewport.Width / 2 - 50 / 2,
                    Globals.viewport.Height / 2 - 50 / 2),
                new Vector2(50, 50),
                new Rectangle(15, 12, 32, 32),
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
            player.Draw();

            for (int i = 0; i < projectiles.Count; i++) {
                projectiles[i].Draw();
            }

            for (int i = 0; i < spawnPoints.Count; i++) {
                spawnPoints[i].Draw();
            }

            for (int i = 0; i < mobs.Count; i++) {
                mobs[i].Draw();
            }
        }

    }
}
