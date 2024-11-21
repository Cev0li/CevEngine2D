using cevEngine2D.source.world.units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//May need to add tileSize data member to Tilesets. Using layer tile size for the current iteration. Used for tileset matrix
namespace cevEngine2D.source.engine.tilemap {
    internal class TileMap {
        [JsonPropertyName("height")]
        public int Height { get; set; }
        [JsonPropertyName("width")]
        public int Width { get; set; }
        [JsonPropertyName("tileheight")]
        public int TileHeight { get; set; }
        [JsonPropertyName("tilewidth")]
        public int TileWidth { get; set; }
        [JsonPropertyName("layers")]
        public Layer[] Layers { get; set; }
        [JsonPropertyName("tilesets")]
        public Tileset[] Tilesets { get; set; }
        public Dictionary<string, Texture2D> SpriteSheetLookup = new();

        //Cannot be called before setLayerSpriteSheet
        public void createSpriteSheetLookupTable() {
            for (int i = 0; i < Tilesets.Length; i++) {
                SpriteSheetLookup.Add(Tilesets[i].Name, Globals.content.Load<Texture2D>(Tilesets[i].Name));
            }
        }

        public void setLayerMapMatrix() {
            foreach (var layer in Layers) {
                if (layer.Type == "tilelayer") {
                    Dictionary<Vector2, int> result = new();
                    Tileset layerSet = Tilesets.FirstOrDefault(t => t.Name == layer.SpriteSheet);
                    //int spriteSheetWidth = layerSet.Width;
                    //int spriteSheetHeight = layerSet.Height;
                    int firstGID = layerSet.firstGID;
                    int x = 0;
                    int y = 0;
                    for (int i = 0; i < layer.Data.Length; i++) {
                        if (i % layer.Width == 0 && i != 0) {
                            x = 0;
                            y++;
                        }

                        if (layer.Data[i] > 0) {
                            int tileGID = layer.Data[i] - firstGID;
                            result.Add(new Vector2(x, y), tileGID);
                        }
                        x++;
                    }
                    layer.MapMatrix = result;
                }
            }
        }

        public void setLayerSpriteSheets() {
            //string pattern = @"([^/\\]+)\.tsx";
            int[] firstgids;
            Dictionary<int, string> tilesetData = new(); //holds firstGID of each tileset in TileMap
            //Set lookup table for firstGID/name of tileset
            foreach (var set in Tilesets) {
                //string text = set.Source;
                //Match match = Regex.Match(text, pattern);
                //string setName = match.Groups[1].Value;
                //set.Source = setName; //clean file path from string in Tilesets array
                tilesetData.Add(set.firstGID, set.Name);
            }

            firstgids = tilesetData.Keys.ToArray(); //Makes array of GIDs to compare to first tile GID found in each layer

            foreach (var layer in Layers) {
                if (layer.Type == "tilelayer") {
                    string tileset = "";
                    int firstNonZero = layer.Data.FirstOrDefault(n => n != 0);

                    for (int i = 0; i < firstgids.Length; i++) {
                        if (firstgids[i] <= firstNonZero) {
                            tileset = tilesetData[firstgids[i]];
                        }
                    }

                    layer.SpriteSheet = tileset;
                }
            }
        }

        public void setObjectSourceRectangles() {
            foreach (var layer in Layers) {
                if(layer.Type == "objectgroup") {
                    string objectLayerName = layer.Name;
                    string[] rectDimsToParse = layer.Objects[0].ObjectData[0].Value.Split(',');
                    int[] rectDims = new int[rectDimsToParse.Length];

                    for (int i = 0; i < rectDims.Length; i++) {
                        rectDims[i] = int.Parse(rectDimsToParse[i]);
                    }

                    layer.Objects[0].ObjectData[0].SRect = new Rectangle(
                        rectDims[0],
                        rectDims[1],
                        rectDims[2],
                        rectDims[3]);

                    //Remove tiled display version of obeject map. This is if you overlaid a map layer representing objects with Tiled objects. 
                    //Instead of loosing the visual in Tiled, The tile layer for the corresponding object layer is removed from TileMap object.
                    //Object layer can now be parsed out of tile layers. For example, object layer can now be used for Y sorting.
                    Layer tileLayer = Layers.FirstOrDefault<Layer>(l => l.Name == objectLayerName && l.Type == "tilelayer");
                    if (tileLayer != null) {
                        int removalIndex = Array.IndexOf(Layers, tileLayer);
                        if (removalIndex >= 0) {
                            Layer[] newLayers = new Layer[Layers.Length - 1];
                            Array.Copy(Layers, 0, newLayers, 0, removalIndex);
                            Array.Copy(Layers, removalIndex + 1, newLayers, removalIndex, Layers.Length - removalIndex - 1);
                            Layers = newLayers;
                        }
                    }
                }
            }
        }

        public void setTilesetAtlas() {
            foreach (var tileset in Tilesets) {
                tileset.Width = tileset.ImageWidth / tileset.TileWidth;
                tileset.Height = tileset.ImageHeight / tileset.TileHeight;
                //Debug.WriteLine("setTilesetAtlas: " + tileset.Width + " " + tileset.Height);
                Dictionary<int, Rectangle> tilesetAtlas = new();
                int tileCount = tileset.Width * tileset.Height;
                int x = 0;
                int y = 0;

                for (int i = 0; i < tileCount; i++) {
                    if (i % tileset.Width == 0 && i != 0) {
                        x = 0;
                        y++;
                    }
                    tilesetAtlas.Add(i, new Rectangle(x * TileWidth, y * TileHeight, TileWidth, TileHeight));
                    x++;
                }
                tileset.TilesetAtlas = tilesetAtlas;
            }
        }
    }

    public class Tileset {
        [JsonPropertyName("firstgid")]
        public int firstGID { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("width")]
        public int Width { get; set; }
        [JsonPropertyName("imagewidth")]
        public int ImageWidth { get; set; }
        [JsonPropertyName("tilewidth")]
        public int TileWidth { get; set; }
        [JsonPropertyName("height")]
        public int Height { get; set; }
        [JsonPropertyName("imageheight")]
        public int ImageHeight { get; set; }
        [JsonPropertyName("tileheight")]
        public int TileHeight { get; set; }
        public Dictionary<int, Rectangle> TilesetAtlas { get; set; } //storage of source rectangles for each tileset. Linear indexing.
    }


    public class Layer {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("height")]
        public int Height { get; set; }
        [JsonPropertyName("width")]
        public int Width { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; } //objectgroup or tilelayer
        [JsonPropertyName("opacity")]
        public float opacity { get; set; }
        [JsonPropertyName("objects")]
        public MapObject[] Objects { get; set; } //objectgroup or tilelayer
        [JsonPropertyName("data")]
        public int[] Data { get; set; }
        public string SpriteSheet { get; set; }
        public Dictionary<Vector2, int> MapMatrix { get; set; }
    }

    public class MapObject {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("height")]
        public float Height { get; set; }

        [JsonPropertyName("properties")]
        public Properties[] ObjectData { get; set; }
        [JsonPropertyName("width")]
        public float Width { get; set; }
        [JsonPropertyName("x")]
        public float X { get; set; }
        [JsonPropertyName("y")]
        public float Y { get; set; }
        [JsonPropertyName("visible")]
        public bool Visible { get; set; }
    }

    public class Properties {
        //stores source rectangle for object layer tilesets. Allows draw calls to use multiple tiles as a singular Sprite.
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("value")]
        public string Value { get; set; }
        public Rectangle SRect { get; set; }
    }

}
