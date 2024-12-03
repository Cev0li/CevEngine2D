#region
using cevEngine2D.source.engine.exceptions;
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
using System.Windows.Forms;
#endregion
/*
 * Built for exporting JSON in Tiled map editor.
 * SetLayerSpriteSheets - Parses the data in the Tileset array to set the tileset associated with that layer. 
 *  the tileset can then be retrieved when drawing using the SpriteSheetLookup list. 
 *  ***use one tileset per layer when building maps***
 *  
 * CreateSpriteSheetLookupTable - Associate Texture2D spritesheet with a string of the same name. Used to set Texture2D for draw calls. 
 * 
 * SetLayerMapMatrix - Parse Data field in layers array into a dictionary representing a matrix location mapped to a 
 *  tile location on the spritesheet atlas.
 *  
 * SetTilesetAtlas - Parse tileset data into Dictionary. Key represents the order of tiles counting along X axis. 
 *  Value represents the source rectangle for cropping that same tile from spritesheet.
 *  
 * SetObjectProperties - Check objects in object layers for custom properties. Add conditional logic for each of the custom properties needed for your game.
 *  Current iteration sets source rectangle for any object possessing a custom property "SourceRect". Map objects are often multiple tiles large.
 *  Handling them as one element is necessary for efficent game construction. This method assumes you have a template rectangle placed
 *  over the tile representations of your object layer in the level edior.
 *  The method then deletes the tile layer associated with that layer. The deleted tile layer will 
 *  assign its sprite sheet string to the object layer at this moment. The object layer in the level editor should be named the same as the tile layer. 
 *  
 * CreateMapUnits - Takes the object layers objects and populates the MapUnit list with MapUnit objects. These MapUnit objects are sprites capable of 
 *  defining game mechanics. This list should be passed into areas of the game that handle drawing, Y sorting, etc.. The MapObject class defines the 
 *  behaviour of these game entities. 
 *  
 *  Lists and their purpose:
 *  MapUnits: Created from Tiled object layers that have custom properties defined meeting the MapUnit constructor. 
 *  CollisionObjects: Created from Tiled object layers that do not satisfy the MapUnit constructor. Created for collision checking. 
 *      Example: A tree only needs collisions at its base. The player should be able to walk behind the tree but not into the tree. 
 *      a rectangle object can be placed at the tree base in Tiled, and used to check collisions against this part of the tree in the 
 *      Update loop. The rest of the tree can be walked behind using a Y sort drawing feature.
 */
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
        public List<BasicUnit> MapUnits = new();
        public List<NonDrawableElement> CollisionObjects = new();
        public Dictionary<string, Texture2D> SpriteSheetLookup = new();

        public void Load() {
            setLayerSpriteSheets();
            createSpriteSheetLookupTable();
            setLayerMapMatrix();
            setTilesetAtlas();
            setObjectProperties();
            createMapUnits();
        }


        //BEGIN: Helper methods
        public int EditorToGameDimensions(float editorDim, int mapDim) {
            float toCast = (float)(editorDim / mapDim) * GameGlobals.tileSize;
            Debug.WriteLine($"{editorDim} {mapDim} {toCast}");
            return ((int)toCast);
        }

        //BEGIN: Initalize map methods
        public void createMapUnits() {
            foreach (var layer in Layers) {
                if (layer.Type == "objectgroup") {
                    foreach (var obj in layer.Objects) {
                        if (!(layer.SpriteSheet == null)) {
                            MapUnit mapObject = new(
                                layer.SpriteSheet,
                                new Vector2(EditorToGameDimensions(obj.X, TileWidth), EditorToGameDimensions(obj.Y, TileHeight)),
                                new Vector2(EditorToGameDimensions(obj.Width, TileWidth), EditorToGameDimensions(obj.Height, TileHeight)),
                                obj.SourceRect
                            );
                            MapUnits.Add(mapObject);

                        } else {
                            try {
                                NonDrawableElement mapObject = new(
                                    new Vector2(EditorToGameDimensions(obj.X, TileWidth), EditorToGameDimensions(obj.Y, TileHeight)),
                                    new Vector2(EditorToGameDimensions(obj.Width, TileWidth), EditorToGameDimensions(obj.Height, TileHeight))
                                );
                                CollisionObjects.Add(mapObject);
                            } catch (Exception ex) {
                                System.Windows.Forms.MessageBox.Show($"Error: {layer.Name} failed to create Map Units. \nHandle your custom Map properties accordingly.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
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

        public void setObjectProperties() {
            foreach (var layer in Layers) {
                if (layer.Type == "objectgroup") {
                    //Handle object Properties
                    for (int i = 0; i < layer.Objects.Length; i++) {
                        LayerProperty[] props = layer.Objects[i].ObjectData;
                        //Check for object properties
                        if (props != null) {
                            LayerProperty sourceRectProp = props.FirstOrDefault(p => p.Name == "SourceRect");
                            //Initalize source Rectangle by parsing the object property SourceRect
                            if (sourceRectProp != null) {
                                int[] rectangleIntegers = sourceRectProp.Value.Split(',')
                                    .Select(s => {
                                        if (int.TryParse(s, out int rectProp)) {
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
                            Debug.WriteLine($"Object {i} in {layer.Name} does not possess any custom properties in level editor");
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
        [JsonPropertyName("properties")]
        public LayerProperty[] LayerProps { get; set; }
        public Dictionary<Vector2, int> MapMatrix { get; set; }
        public string SpriteSheet { get; set; }
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
