
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace monogameTutorial {
    internal class Player : Sprite {

        private Rectangle _sRect; //Crop rect on sprite sheet

        private Rectangle _dRect; //Destination rectangle/sprite scale
        public Rectangle DRect { get { return _dRect;  } set { _dRect = value; } }

        private Vector2 _position; //offset of map in relationship to centered player. Controls camera movement to simulate player movement/ player start on map
        public Vector2 Position { get { return _position; } }

        //constructor takes array of 4 values used to crop a sprite sheet.
        public Player(Texture2D texture, Rectangle DRect, Vector2 position, int[] cropArgs, Viewport viewport) : base(texture) { 
            this._dRect = DRect;
            this._sRect = new Rectangle(cropArgs[0], cropArgs[1], cropArgs[2], cropArgs[3]);
            this._position = position;
        }

        public void Update(float[] velocity) {

            if (Keyboard.GetState().IsKeyDown(Keys.A)) {
                _position.X -= velocity[1];
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D)) {
                _position.X += velocity[3];
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W)) {
                _position.Y -= velocity[2];
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S)) {
                _position.Y += velocity[0];
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(
                texture,
                _dRect,
                _sRect,
                Color.White);
        }
    }
}
