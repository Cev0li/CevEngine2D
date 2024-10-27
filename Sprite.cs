using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogameTutorial {
    public class Sprite {
        /* TO DO: Make sprite class an interface. Common game state varibles will be handled in 
         * this interface i.e. Enemies, Players, Friendly units etc..
         * Possibly bring position variable back to sprite interface */

        public Texture2D texture;


        public Sprite(Texture2D texture/*, Vector2 position*/) {
        this.texture = texture;

        }
    }
}
