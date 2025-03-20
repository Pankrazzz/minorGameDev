using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence
{
    public class Camera
    {
        private Vector2 _position;
        private Viewport _viewport;
        private float _zoom;
        private Rectangle _playArea;

        public Camera(Viewport viewport, Rectangle playArea)
        {
            _viewport = viewport;
            _zoom = 1.0f;
            _position = Vector2.Zero;
            _playArea = playArea;
        }

        public Matrix GetViewMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(-_position, 0.0f)) *
                   Matrix.CreateScale(_zoom) *
                   Matrix.CreateTranslation(new Vector3(_viewport.Width * 0.5f, _viewport.Height * 0.5f, 0.0f));
        }

        public void Follow(Vector2 targetPosition)
        {
            _position = targetPosition;

            // Clamp the camera position to ensure it doesn't go past the play area
            float cameraHalfWidth = _viewport.Width * 0.5f;
            float cameraHalfHeight = _viewport.Height * 0.5f;

            _position.X = MathHelper.Clamp(_position.X, cameraHalfWidth, _playArea.Width - cameraHalfWidth);
            _position.Y = MathHelper.Clamp(_position.Y, cameraHalfHeight, _playArea.Height - cameraHalfHeight);
        }

        public Vector2 Position => _position;
    }
}
