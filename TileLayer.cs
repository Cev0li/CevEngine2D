using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace monogameTutorial
{
    class TileLayer {

        private String filePath;
        public Dictionary<Vector2, int> tileMap;
        private Texture2D tileSheet;
        private int tileSheetY;

        //CONSTRUCTORS 
        public TileLayer(String filePath, Texture2D tileSheet, int sheetY) { 
            this.filePath = filePath;
            this.tileSheet = tileSheet;
            this.tileSheetY = sheetY; //sheets width in tiles
            loadMap(filePath);
        }

        //LOAD IN TILEMAP FROM CSV
        private void loadMap(string filepath) {
            Dictionary<Vector2, int> result = new();
            StreamReader reader = new(filepath);

            int y = 0;
            string line;
            while ((line = reader.ReadLine()) != null) {
                string[] items = line.Split(',');
                for (int x = 0; x < items.Length; x++) {
                    if (int.TryParse(items[x], out int value)) {
                        if (value > -1) {                           // check for empty space left on screen by incomplete tile map
                            result[new Vector2(x, y)] = value;
                        }
                    }
                }
                y++;
            }
            tileMap = result;
        }

        //DRAW MAP LAYER
        public void Draw(SpriteBatch spriteBatch, Vector2 offset) {
            int srcRectX;
            int srcRectY;
            double srcRectYDouble;

            foreach (var item in tileMap) {
                Rectangle dest = new(
                    (int)item.Key.X * 32 + (int)offset.X,
                    (int)item.Key.Y * 32 + (int)offset.Y,
                    32,
                    32
                );

                srcRectX = (item.Value % tileSheetY) * 32; ;
                srcRectYDouble = (item.Value / tileSheetY);
                srcRectYDouble = Math.Ceiling(srcRectYDouble) * 32;
                srcRectY = (int)srcRectYDouble;
                Rectangle src = new Rectangle(
                    srcRectX,
                    (int)srcRectY,
                    32,
                    32
                    );
                spriteBatch.Draw(tileSheet, dest, src, Color.White);
            }
        }
    }

}

