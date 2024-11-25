using cevEngine2D.source.engine.sprites;
using cevEngine2D.source.world;
using cevEngine2D.source.world.units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

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
namespace cevEngine2D.source.engine.tilemap
{
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
        public List<BasicUnit> MapObjects = new();
        public Dictionary<string, Texture2D> SpriteSheetLookup = new();

        public void Load() {
            setLayerSpriteSheets();
            createSpriteSheetLookupTable();
            setLayerMapMatrix();
            setTilesetAtlas();
            setObjectProperties();
            createMapUnits();
        }

        public void createMapUnits() {
            foreach (var layer in Layers) {
                if (layer.Type == "objectgroup") {
                    foreach (var obj in layer.Objects) {
                        MapUnit mapObject = new MapUnit(
                            layer.SpriteSheet,
                            new Vector2((((int)Math.Round(obj.X) / this.TileWidth) * GameGlobals.tileSize), (((int)Math.Round(obj.Y) / this.TileHeight) * GameGlobals.tileSize)),
                            new Vector2((int)obj.Width / this.TileWidth * GameGlobals.tileSize, (int)obj.Height / this.TileHeight * GameGlobals.tileSize),
                            obj.SourceRect
                        );
                        MapObjects.Add(mapObject);
                    }
                }
            }
        }

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
            int[] firstgids;
            Dictionary<int, string> tilesetData = new(); //holds firstGID of each tileset in TileMap
            //Set lookup table for firstGID/name of tileset
            foreach (var set in Tilesets) {
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
        //DEPENDENCY: If object has an source rectangle PropName=SourceRect
        public void setObjectProperties() {
            foreach (var layer in Layers) {
                if (layer.Type == "objectgroup") {
                    //Handle object Properties
                    for (int i = 0; i < layer.Objects.Length; i++) {
                        LayerProperty[] props = layer.Objects[i].ObjectData;
                        //Initalize source Rectangle by parsing the object property SourceRect string containing rectangle args
                        if (props != null) {
                            LayerProperty sourceRectProp = props.FirstOrDefault(p => p.Name == "SourceRect");
                            if (sourceRectProp != null) {
                                int[] rectangleIntegers = sourceRectProp.Value.Split(',')
                                    .Select(s => {
                                        if (int.TryParse(s, out int rectProp)) {
                                            //Debug.WriteLine(rectProp);
                                            return rectProp;
                                        } else {
                                            return -1;
                                        }
                                    })
                                    .ToArray();
                                layer.Objects[i].SourceRect = new Rectangle(
                                    rectangleIntegers[0],
                                    rectangleIntegers[1],
                                    rectangleIntegers[2],
                                    rectangleIntegers[3]
                                    );
                            }
                        } else {
                            Debug.WriteLine($"Object in {layer.Name} does not possess any custom properties in level editor");
                        }
                    }

                    //Remove tile layer with same name as object layer. This allows for multiple tiled objects to be redenered/handles as one object.
                    //This is effectively removing a visual layer in tiled, and giving the engine ability to handle multiple tiled elements as a single entity.
                    //Setting the object layer sprite sheet with the sheet from tile layer.
                    string objectLayerName = layer.Name;
                    Layer tileLayer = Layers.FirstOrDefault<Layer>(l => l.Name == objectLayerName && l.Type == "tilelayer");
                    if (tileLayer != null) {
                        layer.SpriteSheet = tileLayer.SpriteSheet;
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
        public MapObject[] Objects { get; set; }
        [JsonPropertyName("data")]
        public int[] Data { get; set; }
        public string SpriteSheet { get; set; }

        [JsonPropertyName("properties")]
        public LayerProperty[] LayerProps { get; set; }
        public Dictionary<Vector2, int> MapMatrix { get; set; }
    }

    public class MapObject {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("height")]
        public float Height { get; set; }
        [JsonPropertyName("width")]
        public float Width { get; set; }
        [JsonPropertyName("x")]
        public float X { get; set; }
        [JsonPropertyName("y")]
        public float Y { get; set; }
        public Rectangle SourceRect { get; set; }
        [JsonPropertyName("properties")]
        public LayerProperty[] ObjectData { get; set; }
        [JsonPropertyName("visible")]
        public bool Visible { get; set; }
    }

    public class LayerProperty {
        //stores source rectangle for object layer tilesets. Allows draw calls to use multiple tiles as a singular Sprite.
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("value")]
        public string Value { get; set; }
        public Rectangle SRect { get; set; }
    }

}
