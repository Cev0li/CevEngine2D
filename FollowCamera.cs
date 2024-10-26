using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogameTutorial {
	public class FollowCamera {

		public Vector2 position; //Set by player position field for map movement
		Viewport viewport;
		public FollowCamera(Vector2 position, Viewport viewport) {
			this.position = position;
			this.viewport = viewport;
		}

		public void follow(Vector2 playerShift, Vector2 screenSize) {
			position = new Vector2(
				-playerShift.X + viewport.Width / 2,
				-playerShift.Y + viewport.Height / 2
			);
		}
	}
}
