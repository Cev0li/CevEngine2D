#region
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using monogameTutorial.source.world;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace monogameTutorial.source.engine
{
    public class TileLayer
    {

        private string filePath;
        public Dictionary<Vector2, int> tileMap;
        private Texture2D tileSheet;
        private int tileSheetY;

        public TileLayer(string filePath, Texture2D tileSheet, int sheetY)
        {
            this.filePath = filePath;
            this.tileSheet = tileSheet;
            tileSheetY = sheetY; //sheets width in tiles
            loadMap(filePath);
        }

        /* Load in tile map from CSV file made in Tiled
         * CSV number corresponds to the position on sprite sheet
         * Method treats the CSV as 2D array.
         * Loads Dictionary with '2D array' positining Key to tilesheet location value stored as int */
        private void loadMap(string filepath)
        {
            Dictionary<Vector2, int> result = new();
            StreamReader reader = new(filepath);

            int y = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(',');
                for (int x = 0; x < items.Length; x++)
                {
                    if (int.TryParse(items[x], out int value))
                    {
                        if (value > -1)
                        {                           // check for empty space left on screen by incomplete tile map
                            result[new Vector2(x, y)] = value;
                        }
                    }
                }
                y++;
            }
            tileMap = result;
        }

        //DRAW MAP LAYER
        public void Draw(Vector2 offset)
        {
            int srcRectX;
            int srcRectY;
            double srcRectYDouble;

            /* Translates index positioning to pixel positioning for video game world display 
             * Tilesize currently hardcoded. TODO: add tilesize variable to constructor for dynamic tile map loading 
             * Source rect logic divides tile position by row and column lengths to calculate pixel dimensions 
             * for sprite sheet crop */
            foreach (var item in tileMap)
            {
                Rectangle dest = new(
                    (int)item.Key.X * GameGlobals.tileSize + (int)offset.X,
                    (int)item.Key.Y * GameGlobals.tileSize + (int)offset.Y,
                    GameGlobals.tileSize,
                    GameGlobals.tileSize
                );

                //Crop map tile from sprite sheet. TODO: Optomize draw method by loading tilesheet into Dictionary with Vector2, Rectangle KVP
                srcRectX = item.Value % tileSheetY * 32; ;
                srcRectYDouble = item.Value / tileSheetY;
                srcRectYDouble = Math.Ceiling(srcRectYDouble) * GameGlobals.tileSize;
                srcRectY = (int)srcRectYDouble;
                Rectangle src = new Rectangle(
                    srcRectX,
                    srcRectY,
                    GameGlobals.tileSize,
                    GameGlobals.tileSize
                    );
                Globals.spriteBatch.Draw(tileSheet, dest, src, Color.White);
            }
        }
    }

}

