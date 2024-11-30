#region includes
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using cevEngine2D.source.engine.tilemap.utils;
#endregion


namespace cevEngine2D.source.engine.DataStructures {
    public class RepeatingKeyDictionary<TKey, TValue> where TKey : IComparable {
        private readonly SortedDictionary<TKey, List<TValue>> _map = new();
        private Dictionary<int, TKey> _keyIndexes = new();
        private int _keyCount = 0;

        public RepeatingKeyDictionary() {

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
            Debug.WriteLine(index);
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

