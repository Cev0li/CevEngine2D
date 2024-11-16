using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace cevEngine2D.source.engine
{
    public class FollowCamera
    {

        private Vector2 _position; //Set by player position field for map movement
        public Vector2 Position { get { return _position; } }

        public FollowCamera(Vector2 position)
        {
            _position = position;
        }

        public void Follow(Vector2 playerShift, Vector2 screenSize)
        {
            _position = new Vector2(
                -playerShift.X + Globals.viewport.Width / 2,
                -playerShift.Y + Globals.viewport.Height / 2
            );
        }
    }
}
