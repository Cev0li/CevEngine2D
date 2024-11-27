
#region
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using cevEngine2D.source.engine;
using cevEngine2D.source.engine.animate;
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
#endregion

namespace cevEngine2D.source.engine {
    internal class CollisionManager<T> where T: IGameElement {
        private MultiValueMap<int, Rectangle> _collisionObjects = new();

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
            Globals.DrawRectHollow(unit.UnitPerimeter, 1);
            int index = _collisionObjects.CheckInsertionPoint(unit.UnitPerimeter.X);
            index--; //Decrement index to capture necessary lists from _collisionObjects

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

            int count = 0;
            foreach (var rect in checkRects) {
                if (unit.UnitPerimeter.Intersects(rect)) {
                    Globals.DrawRectHollow(rect, 1);
                    Debug.WriteLine("INTERSECT " + count);
                    count++;
                }
            }
        }
    }

    public class MultiValueMap<TKey, TValue> where TKey : IComparable {
        private readonly SortedDictionary<TKey, List<TValue>> _map = new();
        private Dictionary<int, TKey> _keyIndexes = new();
        private int _keyCount = 0;

        public MultiValueMap() { 
            
        }

        public void Add(TKey key, TValue value) {
            if (!_map.ContainsKey(key)) {
                _map.Add(key, new List<TValue>());

                int index = 0;
                _keyCount = 0;
                foreach (var mapKey in _map.Keys) {
                    _keyIndexes[index] = mapKey;
                    index++;
                    _keyCount++;
                }
            }

            _map[key].Add(value);
        }

        public List<TValue> GetValuesByIndex(int index) {
            if (index > -1 && index < _keyCount) {
                return _map[_keyIndexes[index]];
            } else {
                return null;
            }
        }

        public List<TValue> GetValues(TKey key) {
            if (_map.TryGetValue(key, out List<TValue> values)) {
                return values;
            }
            return new List<TValue>();
        }

        public int CheckInsertionPoint(TKey value) {
            TKey[] tkeys = _map.Keys.ToArray();
            int index = Array.BinarySearch(tkeys, value);
            index = index > 0 ? index : ~index;

            return index;

            //if (_map.TryGetValue(key, out List<TValue> values)) {
            //    return values;
            //}

            //// Find the closest key that is less than the target key
            //TKey closestKey = _map.Keys.LastOrDefault(k => k.CompareTo(key) <= 0);

            //if (closestKey != null) {
            //    return _map[closestKey];
            //}

            //// Handle the case where no suitable key is found
            //return null; //new List<TValue>();
        }
    }
}
