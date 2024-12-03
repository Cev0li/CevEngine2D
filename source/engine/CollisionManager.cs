
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
using SharpDX.Direct3D9;
#endregion

namespace cevEngine2D.source.engine {
    public class CollisionManager<T> : ICollisionManager where T : IGameElement {
        private RepeatingKeyDictionary<int, Rectangle> _collisionObjects = new();

        public event EventHandler<CollisionEventArgs> CollisionEvent;

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
                if (unit.Hitbox.Intersects(rect)) {
                    Vector2 pen = PenetrationVector(rect, unit.Hitbox);
                    Debug.WriteLine($"{pen.X} {pen.Y}");
                    CollisionEvent?.Invoke(this, new CollisionEventArgs(pen));
                }
            }
        }

        public static Rectangle Intersection(Rectangle first, Rectangle second) {
            Rectangle result = new Rectangle();
            Vector2 firstMinimum = new Vector2(first.X, first.Y);
            Vector2 firstMaximum = new Vector2(first.X + first.Width, first.Y + first.Height);
            Vector2 secondMinimum = new Vector2(second.X, second.Y);
            Vector2 secondMaximum = new Vector2(second.X + second.Width, second.Y + second.Height);

            var minimum = CalculateMaximumVector2(firstMinimum, secondMinimum);
            var maximum = CalculateMinimumVector2(firstMaximum, secondMaximum);

            if ((maximum.X < minimum.X) || (maximum.Y < minimum.Y))
                result = new Rectangle();
            else
                result = CreateFrom(minimum, maximum);

            return result;
        }

        public static Rectangle CreateFrom(Vector2 minimum, Vector2 maximum) {
            Rectangle result = new Rectangle(
                (int)minimum.X,
                (int)minimum.Y,
                (int)(maximum.X - minimum.X),
                (int)(maximum.Y - minimum.Y));

            return result;
        }

        public static Vector2 CalculateMinimumVector2(Vector2 first, Vector2 second) {
            return new Vector2 {
                X = first.X < second.X ? first.X : second.X,
                Y = first.Y < second.Y ? first.Y : second.Y
            };
        }

        public static Vector2 CalculateMaximumVector2(Vector2 first, Vector2 second) {
            return new Vector2 {
                X = first.X > second.X ? first.X : second.X,
                Y = first.Y > second.Y ? first.Y : second.Y
            };
        }

        private static Vector2 PenetrationVector(Rectangle rect1, Rectangle rect2) {
            var intersectingRectangle = Intersection(rect1, rect2);
            Debug.Assert(!intersectingRectangle.IsEmpty,
                "Violation of: !intersect.IsEmpty; Rectangles must intersect to calculate a penetration vector.");

            Vector2 penetration;
            if (intersectingRectangle.Width < intersectingRectangle.Height) {
                var d = rect1.Center.X < rect2.Center.X
                    ? intersectingRectangle.Width
                    : -intersectingRectangle.Width;
                penetration = new Vector2(d, 0);
            } else {
                var d = rect1.Center.Y < rect2.Center.Y
                    ? intersectingRectangle.Height
                    : -intersectingRectangle.Height;
                penetration = new Vector2(0, d);
            }

            return penetration;
        }
    }

    public class CollisionEventArgs : EventArgs {
        public Vector2 PenetrationVector { get; set; }

        public CollisionEventArgs(Vector2 penetrationVector) {
            PenetrationVector = penetrationVector;
        }
    }
}
