using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace cevEngine2D.source.engine
{
    public class FollowCamera
    {

        public Vector2 Position { get; set; }
        public Matrix TransformMatrix { get; private set; }

        public FollowCamera(Vector2 initialPosition) {
            Position = initialPosition;
        }

        public void Update(GameTime gameTime, Vector2 playerPosition) {
            // Calculate the desired camera position, usually centered on the player
            Position = playerPosition;
            // Calculate the translation offset
            Vector2 translationOffset = new Vector2(Globals.viewport.Width / 2, Globals.viewport.Height / 2) - Position;
            // Create the translation matrix
            TransformMatrix = Matrix.CreateTranslation(translationOffset.X, translationOffset.Y, 0);
        }
    }
}
