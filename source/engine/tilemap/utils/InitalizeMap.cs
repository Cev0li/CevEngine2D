#region includes
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#endregion
/*
 * Load in map from JSON file
 * Save map to TileMap object in game load area using getMapObject()
 */
namespace cevEngine2D.source.engine.tilemap.utils
{
    internal class InitalizeMap {
        private TileMap map;

        public InitalizeMap(string filepath) {
            string JSONString = File.ReadAllText(filepath);
            map = JsonSerializer.Deserialize<TileMap>(JSONString);
        }

        public TileMap getMapObject() {
            return map;
        }
    }
}
