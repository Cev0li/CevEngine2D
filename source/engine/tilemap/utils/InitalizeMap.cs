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

namespace cevEngine2D.source.engine.tilemap.utils
{
    //TILED JSON file requires addition of height and width in the tileset objects
    internal class InitalizeMap {
        private TileMap map;

        public InitalizeMap(string filepath) {
            //string filePath = "../../../JSON/spawn.tmj";
            string JSONString = File.ReadAllText(filepath);

            map = JsonSerializer.Deserialize<TileMap>(JSONString);

            map.setLayerSpriteSheets(); //CAREFUL: need to call this before creating sprite lookup table
            map.createSpriteSheetLookupTable();
            map.setLayerMapMatrix();
            map.setTilesetAtlas();
            map.setObjectSourceRectangles();
        }

        public TileMap getMapObject() {
            return map;
        }
    }
}
