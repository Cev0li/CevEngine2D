using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace monogameTutorial {
    internal class Player : Sprite {

        public int[] cropArgs;
        public Rectangle DRect;
        public Rectangle sRect;
        public Viewport viewport;
        public Vector2 position;
        public Vector2 velocity;

        //constructor takes array of 4 values used to crop a sprite sheet.
        public Player(Texture2D texture, Rectangle DRect, Vector2 position, int[] cropArgs, Viewport viewport) : base(texture) { 
            this.DRect = DRect;
            this.position = position;
            this.sRect = new Rectangle(cropArgs[0], cropArgs[1], cropArgs[2], cropArgs[3]);
            this.viewport = viewport;
            this.velocity = new Vector2(1, 1);
        }

        public void Update() {
            if (Keyboard.GetState().IsKeyDown(Keys.A)) {
                position.X -= velocity.X;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D)) {
                position.X += velocity.X;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W)) {
                position.Y -= velocity.X;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S)) {
                position.Y += velocity.X;
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(
                texture,
                DRect,
                sRect,
                Color.White);
        }
    }
}
