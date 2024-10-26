
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace monogameTutorial {
    public class Player : Sprite {

        public int[] cropArgs;
        public Rectangle dRect; //Destination rectangle/sprite scale
        public Rectangle sRect; //Crop rect on sprite sheer
        public Vector2 position; //offset of map in relationship to centered player. Controls camera movement to simulate player movement/ player start on map
        public Vector2 velocity; //Player speed. Sent from main update. Serves for dash features, collisions
        public Viewport viewport; //viewport object for scalable positioning


        //constructor takes array of 4 values used to crop a sprite sheet.
        public Player(Texture2D texture, Rectangle DRect, Vector2 position, int[] cropArgs, Viewport viewport) : base(texture) { 
            this.dRect = DRect;
            this.sRect = new Rectangle(cropArgs[0], cropArgs[1], cropArgs[2], cropArgs[3]);
            this.position = position;
            this.viewport = viewport;
            this.velocity = new Vector2(1, 1);
        }

        public void Update(float[] velocity) {
            this.velocity = new Vector2(1, 1);

            if (Keyboard.GetState().IsKeyDown(Keys.A)) {
                position.X -= velocity[1];
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D)) {
                position.X += velocity[3];
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W)) {
                position.Y -= velocity[2];
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S)) {
                position.Y += velocity[0];
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(
                texture,
                dRect,
                sRect,
                Color.White);
        }
    }
}
