using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace monogameTutorial {
    internal class Player : Sprite {

        public int[] cropArgs;
        public Vector2 scale;
        public Rectangle dRect;
        public Rectangle sRect;
        public Viewport viewport;

        //constructor takes array of 4 values used to crop a sprite sheet.
        public Player(Texture2D texture, Vector2 position, Vector2 scale, int[] arr, Viewport viewport) : base(texture, position) {
            this.cropArgs = arr;
            this.scale = scale;
            this.dRect = new Rectangle((int)position.X, (int)position.Y, (int)scale.X, (int)scale.Y);
            this.sRect = new Rectangle(cropArgs[0], cropArgs[1], cropArgs[2], cropArgs[3]);
            this.viewport = viewport;
        }

        public void Update() {
            if (Keyboard.GetState().IsKeyDown(Keys.A)) {
                dRect.X -= 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D)) {
                dRect.X += 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W)) {
                dRect.Y -= 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S)) {
                dRect.Y += 1;
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            Rectangle dest = new(
                //350,
                //190,
                //50,
                //50
                viewport.Width / 2 - 25,
                viewport.Height / 2 - 25,
                50,
                50
            );

            spriteBatch.Draw(
                texture,
                dest,
                sRect,
                Color.White);
        }
    }
}
