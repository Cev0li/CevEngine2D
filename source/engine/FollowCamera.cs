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

        public void Follow(Vector2 offset)
        {
            _position = new Vector2(
                -offset.X + Globals.viewport.Width / 2,
                -offset.Y + Globals.viewport.Height / 2
            );
        }
    }
}
