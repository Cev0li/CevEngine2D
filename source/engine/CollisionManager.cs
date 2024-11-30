
#region
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using cevEngine2D.source.engine.animate;
using cevEngine2D.source.engine.DataStructures;
using cevEngine2D.source.engine.input;
using cevEngine2D.source.engine.tilemap;
using cevEngine2D.source.world;
using cevEngine2D.source.world.projectiles;
using cevEngine2D.source.world.units;
using cevEngine2D.source.engine.exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using cevEngine2D.source.engine.tilemap.utils;
using SharpDX.Direct2D1;
using System.Windows.Forms;
using System.Collections;
using cevEngine2D.source.engine.sprites;
using cevEngine2D.source.engine.interfaces;
#endregion

namespace cevEngine2D.source.engine
{
    public class CollisionManager<T> : ICollisionManager where T : IGameElement {
        private RepeatingKeyDictionary<int, Rectangle> _collisionObjects = new();

        public event Action<int> CollisionEvent;

        public CollisionManager(List<T> collisionObjects) {
                foreach (var collisionObject in collisionObjects) {
                    Rectangle dRect = new(
                        (int)collisionObject.POS.X,
                        (int)collisionObject.POS.Y,
                        (int)collisionObject.DRect.Width,
                        (int)collisionObject.DRect.Height
                        );

                    _collisionObjects.Add((int)collisionObject.POS.X, dRect);
            }
        }
        int count = 0;
        public void CheckCollisions(Unit unit) {
            //Globals.DrawRectHollow(unit.UnitPerimeter, 1);
            //Locate units collision perimeter on the X axis against collision manager objects
            int index = _collisionObjects.CheckInsertionPoint(unit.UnitPerimeter.X);
            index--; //Decrement index to capture necessary lists from _collisionObjects

            //Capture index before the collision location, at location, and after the location
            List<Rectangle> checkRects = new();
            for (int i = 0; i < 3; i++) {
                if (index > -1) {
                    List<Rectangle> rectsToAdd = _collisionObjects.GetValuesByIndex(index);

                    if (rectsToAdd != null) {
                        checkRects.AddRange(rectsToAdd);
                    }
                }
                index++;
            }
 
            foreach (var rect in checkRects) {
                if (unit.UnitPerimeter.Intersects(rect)) { //If larger perimeter intersects collision managers objects
                    for (int i = 0; i < unit.UnitPerimeterSliced.Count; i++) { //check direction of collision
                        Rectangle perimeterCheck = unit.UnitPerimeterSliced.ElementAt(i);
                        if (perimeterCheck.Intersects(rect)) {
                            if (rect.Intersects(unit.Hitbox)) {
                               // Debug.WriteLine("Event" + count);
                                count++;
                                CollisionEvent?.Invoke(i); 
                            } //check for collision on hitbox
                        } 
                    }
                    //Globals.DrawRectHollow(rect, 1);
                }
            }
        }
    }
}
