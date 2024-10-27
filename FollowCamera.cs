using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogameTutorial {
	internal class FollowCamera {

        private Viewport viewport;

        private Vector2 _position; //Set by player position field for map movement
		public Vector2 Position { get { return _position;  } }

		public FollowCamera(Vector2 position, Viewport viewport) {
			this._position = position;
			this.viewport = viewport;
		}

		public void Follow(Vector2 playerShift, Vector2 screenSize) {
			_position = new Vector2(
				-playerShift.X + viewport.Width / 2,
				-playerShift.Y + viewport.Height / 2
			);
		}
	}
}
