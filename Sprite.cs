using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogameTutorial {
    internal class Sprite {

        public Texture2D texture;
        public Vector2 position;

        public Sprite(Texture2D texture, Vector2 position) {
            this.texture = texture;
            this.position = position;
        }
    }
}
