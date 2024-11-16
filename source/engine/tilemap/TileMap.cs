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
namespace cevEngine2D.source.engine.tilemap
{
    internal class TileMap
    {
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
        public void createSpriteSheetLookupTable()
        {
            for (int i = 0; i < Tilesets.Length; i++)
            {
                SpriteSheetLookup.Add(Tilesets[i].Source, Globals.content.Load<Texture2D>(Tilesets[i].Source));
            }
        }

        public void setLayerMapMatrix()
        {
            foreach (var layer in Layers)
            {
                if (layer.Type == "tilelayer")
                {
                    Dictionary<Vector2, int> result = new();
                    Tileset layerSet = Tilesets.FirstOrDefault(t => t.Source == layer.SpriteSheet);
                    int spriteSheetWidth = layerSet.Width;
                    int spriteSheetHeight = layerSet.Height;
                    int firstGID = layerSet.firstGID;
                    int x = 0;
                    int y = 0;
                    for (int i = 0; i < layer.Data.Length; i++)
                    {
                        if (i % layer.Width == 0 && i != 0)
                        {
                            x = 0;
                            y++;
                        }

                        //Used for observing loop for correct data during build
                        if (layer.Data[i] > 0)
                        {
                            int tileGID = layer.Data[i] - firstGID;
                            result.Add(new Vector2(x, y), tileGID);
                            //Debug.WriteLine(x + " " + y + " " + realIndex);
                        }
                        x++;
                    }
                    layer.MapMatrix = result;
                }
            }
        }

        public void setLayerSpriteSheets()
        {
            string pattern = @"([^/\\]+)\.tsx";
            int[] firstgids;
            Dictionary<int, string> tilesetData = new(); //holds firstGID of each tileset in TileMap
            //Set lookup table for firstGID/name of tileset
            foreach (var set in Tilesets)
            {
                string text = set.Source;
                Match match = Regex.Match(text, pattern);
                string setName = match.Groups[1].Value;
                set.Source = setName; //clean file path from string in Tilesets array
                tilesetData.Add(set.firstGID, setName);
            }

            firstgids = tilesetData.Keys.ToArray(); //Makes array of GIDs to compare to first tile GID found in each layer

            foreach (var layer in Layers)
            {
                if (layer.Type == "tilelayer")
                {
                    string tileset = "";
                    int firstNonZero = layer.Data.FirstOrDefault(n => n != 0);

                    for (int i = 0; i < firstgids.Length; i++)
                    {
                        if (firstgids[i] <= firstNonZero)
                        {
                            tileset = tilesetData[firstgids[i]];
                        }
                    }

                    layer.SpriteSheet = tileset; //Associate the tile set layer is drawn from with layer
                }
            }
        }

        public void setTilesetAtlas()
        {
            foreach (var tileset in Tilesets)
            {
                Dictionary<int, Rectangle> tilesetAtlas = new();
                int tileCount = tileset.Width * tileset.Height;
                int x = 0;
                int y = 0;

                for (int i = 0; i < tileCount; i++)
                {
                    if (i % tileset.Width == 0 && i != 0)
                    {
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

    public class Tileset
    {
        [JsonPropertyName("firstgid")]
        public int firstGID { get; set; }
        [JsonPropertyName("source")]
        public string Source { get; set; }
        [JsonPropertyName("width")]
        public int Width { get; set; }
        [JsonPropertyName("height")]
        public int Height { get; set; }
        public Dictionary<int, Rectangle> TilesetAtlas { get; set; } //storage of source rectangles for each tileset. Linear indexing.
    }


    public class Layer
    {
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
        public ObjectLayer[] Objects { get; set; } //objectgroup or tilelayer
        [JsonPropertyName("data")]
        public int[] Data { get; set; }
        public string SpriteSheet { get; set; }
        public Dictionary<Vector2, int> MapMatrix { get; set; }
    }

    public class ObjectLayer
    {
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
        [JsonPropertyName("visible")]
        public bool Visible { get; set; }
    }
}
